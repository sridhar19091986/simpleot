using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class ProtocolInfoAttribute : Attribute
    {
        private readonly string _protocolName;
        private readonly byte _protocolIndentifier;

        public ProtocolInfoAttribute(string protocolName, byte protocolIndentifier)
        {
            this._protocolName = protocolName;
            this._protocolIndentifier = protocolIndentifier;
        }

        public string ProtocolName { get { return _protocolName; } }
        public byte ProtocolIndentifier { get { return _protocolIndentifier; } }
    }

    public abstract class Protocol
    {
        private Connection _connection;

        public virtual void OnConnectionOpen() { }
        public virtual void OnConnectionClosed() { }

        public virtual void OnReceiveFirstMessage(Message message) { }
        public virtual void OnReceiveMessage(Message message) { }

        public virtual void OnSendMessage(Message message) { }

        public virtual void OnExceptionCaught(Exception exception)
        {
        }

        public Connection Connection { get { return _connection; } set { _connection = value; } }
    }
}
