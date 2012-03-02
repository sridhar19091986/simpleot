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
using NLog;
using SimpleOT.Collections;

namespace SimpleOT.Net
{
	public class Service<T> : IService where T : Protocol, new()
	{
        private readonly string _protocolName;
        private readonly byte _protocolIndentifier;

        public Service()
        {
            var attributes = typeof(T).GetCustomAttributes(typeof(ProtocolInfoAttribute), false);

            if (attributes == null || attributes.Length == 0)
                throw new Exception("All protocol must have a ProtocolInfo attribute.");

            var protocolInfo = attributes[0] as ProtocolInfoAttribute;

            _protocolName = protocolInfo.ProtocolName;
            _protocolIndentifier = protocolInfo.ProtocolIndentifier;
        }

        public Protocol CreateProtocol()
        {
            return new T();
        }

        public string ProtocolName { get { return _protocolName; } }
        public byte ProtocolIndentifier { get { return _protocolIndentifier; } }
	}
}
