using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Threading;
using NLog;

namespace SimpleOT.Net
{
    public class ServiceManager
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly Server _server;
        private readonly IDictionary<int, ServicePort> _acceptors;

        public ServiceManager(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

			_server = server;
            _acceptors = new Dictionary<int, ServicePort>();
        }

        public void Add<T>(int port) where T : Protocol, new()
        {
            var service = new Service<T>();

            if (port <= 0)
            {
                logger.Error("Invalid port for {0} service. Service disabled.", service.ProtocolName);
                return;
            }

            ServicePort servicePort = null;

            if (_acceptors.ContainsKey(port))
            {
                servicePort = _acceptors[port];

                if (servicePort.SingleSocket)
                {
                    logger.Error("{0} and {1} cannot use the same port {2}.", service.ProtocolName, servicePort.ProtocolNames, port);
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
    }
}
