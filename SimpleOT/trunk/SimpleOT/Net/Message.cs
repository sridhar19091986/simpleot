﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleOT.Collections;
using System.Data;

namespace SimpleOT.Net
{
    public class Message
    {
        private byte[] _buffer;
        private int _readerIndex;
        private int _readerIndexMark;
        private int _writerIndex;
        private int _writerIndexMark;
        private bool _readyOnly;
        private Connection _connection;
        private long _frameTime;

        public Message(byte[] buffer, bool readOnly)
        {
            _buffer = buffer;
            _readyOnly = readOnly;
        }

        public Message(int capacity, bool readOnly)
            : this(new byte[capacity], readOnly)
        {
        }

        public Message(bool readOnly)
            : this(Constants.MESSAGE_DEFAULT_SIZE, readOnly)
        {
        }

        public Message()
            : this(false)
        {
        }

        public void MarkReaderIndex()
        {
            _readerIndexMark = _readerIndex;
        }

        public void ResetReaderIndex()
        {
            _readerIndex = _readerIndexMark;
        }

        public void MarkWriterIndex()
        {
            _writerIndexMark = _writerIndex;
        }

        public void ResetWriterIndex()
        {
            _writerIndex = _writerIndexMark;
        }

        public void Clear()
        {
            _connection = null;
            _writerIndex = 0;
            _readerIndex = 0;
        }

        public void DiscardReadBytes()
        {
            Array.Copy(_buffer, _readerIndex, _buffer, 0, _writerIndex - _readerIndex);
            _writerIndex = _writerIndex - _readerIndex;
            _readerIndex = 0;
        }

        public long FrameTime { get { return _frameTime; } set { _frameTime = value; } }

        public Connection Connection { get { return _connection; } set { _connection = value; } }

        public int ReadableBytes { get { return _writerIndex - _readerIndex; } }

        public int WritableBytes { get { return _buffer.Length - _writerIndex; } }

        public bool IsReadable { get { return _writerIndex - _readerIndex > 0; } }

        public bool IsWritable { get { return _buffer.Length - _writerIndex > 0; } }

        public byte[] Buffer { get { return _buffer; } }

        public int Capacity { get { return _buffer.Length; } }

        public bool IsReadyOnly { get { return _readyOnly; } }

        public int WriterIndex
        {
            get { return _writerIndex; }
            set
            {
                if (value > Capacity || value < _readerIndex)
                    throw new ArgumentException("The value must be greater then reader index and no larger than the message capacity");

                _writerIndex = value;
            }
        }

        public int ReaderIndex
        {
            get { return _readerIndex; }
            set
            {
                if (value > _writerIndex || value < 0)
                    throw new ArgumentException("The value must be non-negative and no larger than the writer index.");

                _readerIndex = value;
            }
        }

        #region Put

        private void VerifyPut(int position, int byteCount)
        {
            if (_readyOnly)
                throw new ReadOnlyException("You can't modify this message because it's read only.");

            if (position < 0)
                throw new ArgumentException("The position must be non-negative");
            if (position + byteCount > Capacity)
                throw new InternalBufferOverflowException("Insufficient space in this message.");
        }

        public unsafe void PutInternalLength(ushort value)
        {
            fixed (byte* bufferPtr = _buffer)
                *(ushort*)(bufferPtr + 6) = value;
        }

        public unsafe void PutLength(ushort value)
        {
            fixed (byte* bufferPtr = _buffer)
                *(ushort*)(bufferPtr) = value;
        }

        public unsafe void PutChecksum(uint value)
        {
            fixed (byte* bufferPtr = _buffer)
                *(uint*)(bufferPtr + 2) = value;
        }

        public void PutByte(byte value)
        {
            VerifyPut(_writerIndex, 1);
            _buffer[_writerIndex] = value;
        }

        public void PutBoolean(bool value)
        {
            VerifyPut(_writerIndex, 1);
            _buffer[_writerIndex] = (byte)(value ? 1 : 0);
        }

        public unsafe void PutUShort(ushort value)
        {
            VerifyPut(_writerIndex, 2);
            fixed (byte* bufferPtr = _buffer)
                *(ushort*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 2;
        }

        public unsafe void PutShort(short value)
        {
            VerifyPut(_writerIndex, 2);
            fixed (byte* bufferPtr = _buffer)
                *(short*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 2;
        }

        public unsafe void PutUInt(uint value)
        {
            VerifyPut(_writerIndex, 4);
            fixed (byte* bufferPtr = _buffer)
                *(uint*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 4;
        }

        public unsafe void PutInt(int value)
        {
            VerifyPut(_writerIndex, 4);
            fixed (byte* bufferPtr = _buffer)
                *(int*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 4;
        }

        public unsafe void PutULong(ulong value)
        {
            VerifyPut(_writerIndex, 8);
            fixed (byte* bufferPtr = _buffer)
                *(ulong*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 8;
        }

        public unsafe void PutLong(long value)
        {
            VerifyPut(_writerIndex, 8);
            fixed (byte* bufferPtr = _buffer)
                *(long*)(bufferPtr + _writerIndex) = value;
            _writerIndex += 8;
        }

        public void PutString(string value)
        {
            if (value == null)
                value = string.Empty;

            VerifyPut(_writerIndex, value.Length + 2);
            PutUShort((ushort)value.Length);
            Array.Copy(Encoding.Default.GetBytes(value), 0, _buffer, _writerIndex, value.Length);
            _writerIndex += value.Length;
        }

        #endregion

        #region Get

        private void VerifyGet(int position, int byteCount)
        {
            if (_writerIndex - position < byteCount)
                throw new InternalBufferOverflowException("Insufficient space in this message.");
        }

        public ushort GetLength()
        {
            return BitConverter.ToUInt16(_buffer, 0);
        }

        public uint GetChecksum()
        {
            return BitConverter.ToUInt32(_buffer, 2);
        }

        public ushort GetInternalLength()
        {
            return BitConverter.ToUInt16(_buffer, 6);
        }

        public byte GetByte()
        {
            VerifyGet(_readerIndex, 1);
            return _buffer[_readerIndex++];
        }

        public bool GetBool()
        {
            VerifyGet(_readerIndex, 1);
            return _buffer[_readerIndex++] == 1;
        }

        public ushort GetUShort()
        {
            VerifyGet(_readerIndex, 2);
            var value = BitConverter.ToUInt16(_buffer, _readerIndex);
            _readerIndex += 2;
            return value;
        }

        public short GetShort()
        {
            VerifyGet(_readerIndex, 2);
            var value = BitConverter.ToInt16(_buffer, _readerIndex);
            _readerIndex += 2;
            return value;
        }

        public uint GetUInt()
        {
            VerifyGet(_readerIndex, 4);
            var value = BitConverter.ToUInt32(_buffer, _readerIndex);
            _readerIndex += 4;
            return value;
        }

        public int GetInt()
        {
            VerifyGet(_readerIndex, 4);
            var value = BitConverter.ToInt32(_buffer, _readerIndex);
            _readerIndex += 4;
            return value;
        }

        public ulong GetULong()
        {
            VerifyGet(_readerIndex, 8);
            var value = BitConverter.ToUInt64(_buffer, _readerIndex);
            _readerIndex += 8;
            return value;
        }

        public long GetLong()
        {
            VerifyGet(_readerIndex, 8);
            var value = BitConverter.ToInt64(_buffer, _readerIndex);
            _readerIndex += 8;
            return value;
        }

        public string GetString()
        {
            ushort length = GetUShort();

            VerifyGet(_readerIndex, length);

            var value = Encoding.Default.GetString(_buffer, _readerIndex, length);
            _readerIndex += length;
            return value;
        }

        #endregion
    }
}
