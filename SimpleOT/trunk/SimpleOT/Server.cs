using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Net;
using SimpleOT.Threading;

namespace SimpleOT
{
    class Server
    {
        private readonly Dispatcher _dispatcher;
        private readonly Scheduler _scheduler;

        private readonly ServiceManager _serviceManager;


        public Server()
        {
            _dispatcher = new Dispatcher();
            _scheduler = new Scheduler(_dispatcher);

            _dispatcher.Start();
            _scheduler.Start();

            _serviceManager = new ServiceManager(_dispatcher, _scheduler);

            _serviceManager.Add<LoginProtocol>(0);
            _serviceManager.Add<GameProtocol>(7172);
        }

        ~Server()
        {
            _scheduler.Shutdown();
            _dispatcher.Shutdown();
        }

        static void Main(string[] args)
        {
            new Server();
        }
    }
}
