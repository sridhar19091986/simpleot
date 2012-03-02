﻿using System;
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

        private readonly IDictionary<int, ServicePort> _acceptors;
        private readonly Dispatcher _dispatcher;
        private readonly Scheduler _scheduler;

        public ServiceManager(Dispatcher dispatcher, Scheduler scheduler)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            if (scheduler == null)
                throw new ArgumentNullException("scheduler");

            _dispatcher = dispatcher;
            _scheduler = scheduler;

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
                servicePort = _acceptors[port];
            else
            {
                servicePort = new ServicePort(port, _dispatcher, _scheduler);
                servicePort.Open();
            }

            servicePort.AddService(service);
        }
    }
}
