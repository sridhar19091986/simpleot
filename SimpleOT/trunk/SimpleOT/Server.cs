using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Net;
using SimpleOT.Threading;
using SimpleOT.Data;
using SimpleOT.Collections;
using SimpleOT.Scripting;

namespace SimpleOT
{
    public class Server
    {
		public static Server Instance;
		
        private readonly Dispatcher _dispatcher;
        private readonly Scheduler _scheduler;

        private readonly OutputMessagePool _outputMessagePool;
        private readonly ServiceManager _serviceManager;
        private readonly IConnectionFactory _connectionFactory;
        private readonly AccountRepository _accountRepository;

        private readonly ScriptEngine _scriptEngine;

        private readonly ConfigManager _configManager;

        public Server()
        {
            _dispatcher = new Dispatcher();
            _scheduler = new Scheduler(_dispatcher);

            _dispatcher.Start();
            _scheduler.Start();

            _connectionFactory = new PostgresConnectionFactory();
            _accountRepository = new AccountRepository(_connectionFactory);

            _scriptEngine = new ScriptEngine();

            _configManager = new ConfigManager(this);

            _outputMessagePool = new OutputMessagePool(10, 100);
            _dispatcher.AfterDispatchTask += _outputMessagePool.ProcessEnqueueMessages;

            _serviceManager = new ServiceManager(this);

            _serviceManager.Add<LoginProtocol>(_configManager.LoginPort);
            _serviceManager.Add<GameProtocol>(_configManager.GamePort);
        }

        ~Server()
        {
            _dispatcher.AfterDispatchTask -= _outputMessagePool.ProcessEnqueueMessages;

            _scheduler.Shutdown();
            _dispatcher.Shutdown();
        }

        public AccountRepository AccountRepository { get { return _accountRepository; } }

        public Dispatcher Dispatcher { get { return _dispatcher; } }
        public Scheduler Scheduler { get { return _scheduler; } }

        public OutputMessagePool OutputMessagePool { get { return _outputMessagePool; } }

        public ScriptEngine ScriptEngine { get { return _scriptEngine; } }

        static void Main(string[] args)
        {
            Instance = new Server();
        }
    }
}
