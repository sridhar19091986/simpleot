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

        public ProtocolInfoAttribute(string protocolName, byte protocolIndentifier)
        {
            this._protocolName = protocolName;
            this._protocolIndentifier = protocolIndentifier;
        }

        public string ProtocolName { get { return _protocolName; } }
        public byte ProtocolIndentifier { get { return _protocolIndentifier; } }

        public bool SingleSocket { get { return _singleSocket; } set { _singleSocket = value; } }
    }

    public abstract class Protocol
    {
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
        private Connection _connection;

        private uint[] _xteaKey;
        private bool _xteaEnabled;

        public virtual void OnConnectionOpen() { }
        public virtual void OnConnectionClosed() { }

        public virtual void OnReceiveFirstMessage(Message message) { }
        public virtual void OnReceiveMessage(Message message) { }

        public virtual void OnSendMessage(Message message)
        {
            message.PutInternalLength(message.WritableBytes - 6);

            if (_xteaEnabled)
            {
            }


        }

        public virtual void OnExceptionCaught(Exception exception)
        {
			logger.Error("Connection error. Details: {0}\nStack Trace: {1}", exception.Message, exception.StackTrace);
			
			if(_connection != null)
				_connection.Close();
        }

        public void EnableXteaEncryption(uint xteaKey)
        {
            _xteaKey = xteaKey;
            _xteaEnabled = true;
        }

        public Connection Connection { get { return _connection; } set { _connection = value; } }
    }
}
