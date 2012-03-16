using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using SimpleOT.Threading;
using SimpleOT.Collections;

namespace SimpleOT.Net
{
	public class Service<T> : IService where T : Protocol, new()
	{
        private readonly Server _server;

        private readonly string _protocolName;
        private readonly byte _protocolIndentifier;
        private readonly bool _singleSocket;
        private readonly bool _hasChecksum;

        public Service(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;

            var attributes = typeof(T).GetCustomAttributes(typeof(ProtocolInfoAttribute), false);

            if (attributes == null || attributes.Length == 0)
                throw new Exception("All protocol must have a ProtocolInfo attribute.");

            var protocolInfo = attributes[0] as ProtocolInfoAttribute;

            _protocolName = protocolInfo.ProtocolName;
            _protocolIndentifier = protocolInfo.ProtocolIndentifier;
            _singleSocket = protocolInfo.SingleSocket;
            _hasChecksum = protocolInfo.HasChecksum;
        }

        public Protocol CreateProtocol()
        {
            Protocol protocol = new T();
            protocol.Server = _server;

            return protocol;
        }

        public string ProtocolName { get { return _protocolName; } }
        public byte ProtocolIndentifier { get { return _protocolIndentifier; } }
        public bool SingleSocket { get { return _singleSocket; } }
        public bool HasChecksum { get { return _hasChecksum; } }
	}
}
