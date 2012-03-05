using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SimpleOT.Collections;

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
            _readerIndex = Constants.MessageHeaderMaxSize;
            _writerIndex = Constants.MessageHeaderMaxSize;
        }

        public Message(int capacity, bool readOnly)
            : this(new byte[capacity], readOnly)
        {
        }

        public Message(bool readOnly)
            : this(Constants.MessageDefaultSize, readOnly)
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
            _writerIndex = Constants.MessageHeaderMaxSize;
            _readerIndex = Constants.MessageHeaderMaxSize;
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

        public int Capacity { get { return _buffer.Length - Constants.MessageHeaderMaxSize; } }

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
                throw new Exception("You can't modify this message because it's read only.");

            if (position < 0)
                throw new ArgumentException("The position must be non-negative");
            if (position + byteCount > Capacity)
                throw new InternalBufferOverflowException("Insufficient space in this message.");
        }

        public unsafe void PutHeader(ushort value)
        {
            if (_readerIndex < 2)
                throw new InvalidOperationException("Insufficient header space.");

            _readerIndex -= 2;
            fixed (byte* bufferPtr = _buffer)
                *(ushort*)(bufferPtr + _readerIndex) = value;
        }

        public unsafe void PutHeader(uint value)
        {
            if (_readerIndex < 4)
                throw new InvalidOperationException("Insufficient header space.");

            _readerIndex -= 4;
            fixed (byte* bufferPtr = _buffer)
                *(uint*)(bufferPtr + _readerIndex) = value;
        }

        public void PutByte(byte value)
        {
            VerifyPut(_writerIndex, 1);
            _buffer[_writerIndex++] = value;
        }

        public void PutBoolean(bool value)
        {
            VerifyPut(_writerIndex, 1);
            _buffer[_writerIndex++] = (byte)(value ? 1 : 0);
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

        private void VerifyGet(int byteCount)
        {
            if (_writerIndex - _readerIndex < byteCount)
                throw new InternalBufferOverflowException("Insufficient space in this message.");
        }

        public byte GetByte()
        {
            VerifyGet(1);
            return _buffer[_readerIndex++];
        }

        public bool GetBool()
        {
            VerifyGet(1);
            return _buffer[_readerIndex++] == 1;
        }

        public ushort GetUShort()
        {
            VerifyGet(2);
            var value = BitConverter.ToUInt16(_buffer, _readerIndex);
            _readerIndex += 2;
            return value;
        }

        public short GetShort()
        {
            VerifyGet(2);
            var value = BitConverter.ToInt16(_buffer, _readerIndex);
            _readerIndex += 2;
            return value;
        }

        public uint GetUInt()
        {
            VerifyGet(4);
            var value = BitConverter.ToUInt32(_buffer, _readerIndex);
            _readerIndex += 4;
            return value;
        }

        public int GetInt()
        {
            VerifyGet(4);
            var value = BitConverter.ToInt32(_buffer, _readerIndex);
            _readerIndex += 4;
            return value;
        }

        public ulong GetULong()
        {
            VerifyGet(8);
            var value = BitConverter.ToUInt64(_buffer, _readerIndex);
            _readerIndex += 8;
            return value;
        }

        public long GetLong()
        {
            VerifyGet(8);
            var value = BitConverter.ToInt64(_buffer, _readerIndex);
            _readerIndex += 8;
            return value;
        }

        public string GetString()
        {
            ushort length = GetUShort();
            VerifyGet(length);

            var value = Encoding.Default.GetString(_buffer, _readerIndex, length);
            _readerIndex += length;
            return value;
        }

        #endregion

        #region Peek

        private void VerifyPeek(int byteCount)
        {
            if (_writerIndex - _readerIndex < byteCount)
                throw new InternalBufferOverflowException("Insufficient space in this message.");
        }

        public byte PeekByte()
        {
            VerifyPeek(1);
            return _buffer[_readerIndex];
        }

        public bool PeekBool()
        {
            VerifyPeek(1);
            return _buffer[_readerIndex] == 1;
        }

        public ushort PeekUShort()
        {
            VerifyPeek(2);
            return BitConverter.ToUInt16(_buffer, _readerIndex);
        }

        public short PeekShort()
        {
            VerifyPeek(2);
            return BitConverter.ToInt16(_buffer, _readerIndex);
        }

        public uint PeekUInt()
        {
            VerifyPeek(4);
            return BitConverter.ToUInt32(_buffer, _readerIndex);
        }

        public int PeekInt()
        {
            VerifyPeek(4);
            return BitConverter.ToInt32(_buffer, _readerIndex);
        }

        public ulong PeekULong()
        {
            VerifyPeek(8);
            return BitConverter.ToUInt64(_buffer, _readerIndex);
        }

        public long PeekLong()
        {
            VerifyPeek(8);
            return BitConverter.ToInt64(_buffer, _readerIndex);
        }

        #endregion
    }
}
