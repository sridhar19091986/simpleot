using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Threading;
using System.Net;
using System.Net.Sockets;
using SimpleOT.IO;

namespace SimpleOT.Net
{
    public class ServiceManager
    {
        private readonly Server _server;
        private readonly IDictionary<int, ServicePort> _acceptors;
        private readonly IDictionary<uint, uint> _ipList;

        public ServiceManager(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
            _acceptors = new Dictionary<int, ServicePort>();

            _ipList = new Dictionary<uint, uint>();

            //local ip address
            _ipList[IPConverter.ToUInt32("127.0.0.1")] = 0xFFFFFFFF;

            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                    _ipList[IPConverter.ToUInt32(address.ToString())] = 0x0000FFFF;
            }

            //public ip address
            foreach (var address in Dns.GetHostAddresses(_server.ConfigManager.ServerIp))
            {
                if(address.AddressFamily == AddressFamily.InterNetwork)
                    _ipList[IPConverter.ToUInt32(address.ToString())] = 0;
            }
        }

        public void Add<T>(int port) where T : Protocol, new()
        {
            var service = new Service<T>(_server);

            if (port <= 0)
            {
                Logger.Error(string.Format("Invalid port for {0} service. Service disabled.", service.ProtocolName));
                return;
            }

            ServicePort servicePort = null;

            if (_acceptors.ContainsKey(port))
            {
                servicePort = _acceptors[port];

                if (servicePort.SingleSocket)
                {
                    Logger.Error(string.Format("{0} and {1} cannot use the same port {2}.", service.ProtocolName, servicePort.ProtocolNames, port));
                    return;
                }
            }
            else
            {
                servicePort = new ServicePort(_server, port);
                servicePort.Open();
            }

            servicePort.AddService(service);
        }

        public Server Server { get { return _server; } }

        public IDictionary<uint, uint> IpAddresses { get { return _ipList; } }

        public string[] PublicIpAddresses 
        {
            get
            {
                return _ipList.Where(x => x.Value == 0).Select(x => IPConverter.ToString(x.Key)).ToArray();
            }
        }

        public string[] PrivateIpAddresses
        {
            get
            {
                return _ipList.Where(x => x.Value == 0xFFFFFFFF || x.Value == 0x0000FFFF).Select(x => IPConverter.ToString(x.Key)).ToArray();
            }
        }
    }
}
