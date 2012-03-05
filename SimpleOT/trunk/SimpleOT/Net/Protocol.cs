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
		
        private Connection _connection;

        private bool _hasChecksum;
        private bool _hasXteaEncryption;

        private uint[] _xteaKey;

        public virtual void OnConnectionOpen() { }
        public virtual void OnConnectionClosed() { }

        public virtual void OnReceiveFirstMessage(Message message) { }
        
        public virtual void OnReceiveMessage(Message message) 
        {
            if (_hasChecksum && message.GetUInt() != Adler.Generate(message))
                throw new Exception("Invalid message checksum.");


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

        public uint[] XteaKey { get { return _xteaKey; } protected set { _xteaKey = value; } }
        public bool HasXteaEncryption { get { return _hasXteaEncryption; } protected set { _hasXteaEncryption = value; } }
        public bool HasChecksum { get { return _hasChecksum; } protected set { _hasChecksum = value; } }

        public Connection Connection { get { return _connection; } set { _connection = value; } }
    }
}
