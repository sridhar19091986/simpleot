using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace SimpleOT.Net
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ProtocolInfoAttribute : Attribute
    {
        private readonly string _protocolName;
        private readonly byte _protocolIndentifier;

        private bool _singleSocket;
        private bool _hasChecksum;

        public ProtocolInfoAttribute(string protocolName, byte protocolIndentifier)
        {
            this._protocolName = protocolName;
            this._protocolIndentifier = protocolIndentifier;
        }

        public string ProtocolName { get { return _protocolName; } }
        public byte ProtocolIndentifier { get { return _protocolIndentifier; } }

        public bool SingleSocket { get { return _singleSocket; } set { _singleSocket = value; } }
        public bool HasChecksum { get { return _hasChecksum; } set { _hasChecksum = value; } }
    }

    public abstract class Protocol
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();

        private Server _server;
        private Connection _connection;

        private bool _hasChecksum;
        private bool _hasXteaEncryption;

        private uint[] _xteaKey;

        private Message _outputBuffer;

        public virtual void OnConnectionOpen() { }
        public virtual void OnConnectionClosed() { }

        public virtual void OnReceiveFirstMessage(Message message) { }

        public virtual void OnReceiveMessage(Message message) 
        {
            if (_hasChecksum && message.GetUInt() != Adler.Generate(message))
                throw new Exception("Invalid message checksum.");

            if (_hasXteaEncryption)
            {
                Xtea.Decrypt(message, _xteaKey);
                var length = message.GetUShort();
                message.WriterIndex = message.ReaderIndex + length;
            }
        }

        public virtual void OnSendMessage(Message message)
        {
            message.PutHeader((ushort)message.ReadableBytes);

            if (_hasXteaEncryption && _xteaKey != null)
                Xtea.Encrypt(message, _xteaKey);
            if (_hasChecksum)
                message.PutHeader(Adler.Generate(message));

            if(_hasXteaEncryption || _hasChecksum)
                message.PutHeader((ushort)message.ReadableBytes);
        }

        public virtual void OnExceptionCaught(Exception exception)
        {
			logger.Error("Connection error. Details: {0}\nStack Trace: {1}", exception.Message, exception.StackTrace);
			
			if(_connection != null)
				_connection.Close();
        }

        protected void Disconnect(byte error, string description)
        {
            var message = Server.OutputMessagePool.Get(Connection, false);

            if (message != null)
            {
                message.PutByte(error);
                message.PutString(description);

                _connection.Write(message);
            }

            _connection.Close();
        }

        protected Message GetOutputBuffer()
        {
            if (_outputBuffer != null && _outputBuffer.ReadableBytes < Constants.MessageDefaultSize - 4096)
                return _outputBuffer;
            else if (_connection != null)
            {
                _outputBuffer = _server.OutputMessagePool.Get(_connection, true);
                return _outputBuffer;
            }

            return null;
        }

        public uint[] XteaKey { get { return _xteaKey; } protected set { _xteaKey = value; } }
        public bool HasXteaEncryption { get { return _hasXteaEncryption; } protected set { _hasXteaEncryption = value; } }
        public bool HasChecksum { get { return _hasChecksum; } protected set { _hasChecksum = value; } }

        public Connection Connection { get { return _connection; } set { _connection = value; } }
        public Server Server { get { return _server; } set { _server = value; } }
    }
}
