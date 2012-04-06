using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Net;
using SimpleOT.Threading;
using SimpleOT.Data;
using SimpleOT.Collections;
using SimpleOT.Scripting;
using SimpleOT.Domain;
using SimpleOT.IO;

namespace SimpleOT
{
    public class Server
    {
		public static Server Instance;
		
        private Dispatcher _dispatcher;
        private Scheduler _scheduler;

        private OutputMessagePool _outputMessagePool;
        private ServiceManager _serviceManager;
        
        private ConfigManager _configManager;
		
		private IConnectionFactory _connectionFactory;
		private IAccountRepository _accountRepository;
		private IPlayerRepository _playerRepository;

        private World _world;

        public Server()
        {
           
        }

        ~Server()
        {
            _dispatcher.AfterDispatchTask -= _outputMessagePool.ProcessEnqueueMessages;

            _scheduler.Shutdown();
            _dispatcher.Shutdown();
        }

        public void Start()
        {
            _dispatcher = new Dispatcher();
            _scheduler = new Scheduler(_dispatcher);

            _dispatcher.Start();
            _scheduler.Start();

            _configManager = new ConfigManager(this);

			ItemType.Load(@"Data\items\items.otb");
			
            _connectionFactory = new PostgresConnectionFactory();
            _accountRepository = new AccountRepository(_connectionFactory);
            _playerRepository = new PlayerRepository(_connectionFactory);

            _world = new World(this);

            _world.Map.Load(@"Data\world\map.otbm");

            _outputMessagePool = new OutputMessagePool(10, 100);
            _dispatcher.AfterDispatchTask += _outputMessagePool.ProcessEnqueueMessages;

            _serviceManager = new ServiceManager(this);

            _serviceManager.Add<LoginProtocol>(_configManager.LoginPort);
            _serviceManager.Add<GameProtocol>(_configManager.GamePort);

            Logger.Info(string.Format("Local ip address: {0}", String.Join(" ", _serviceManager.PrivateIpAddresses)));
            Logger.Info(string.Format("Global ip address: {0}", String.Join(" ", _serviceManager.PublicIpAddresses)));
        }

        public void Stop()
        {

        }

        public IAccountRepository AccountRepository { get { return _accountRepository; } }
		public IPlayerRepository PlayerRepository { get{ return _playerRepository; } }

        public Dispatcher Dispatcher { get { return _dispatcher; } }
        public Scheduler Scheduler { get { return _scheduler; } }

        public OutputMessagePool OutputMessagePool { get { return _outputMessagePool; } }

        public ConfigManager ConfigManager { get { return _configManager; } }

        public ServiceManager ServiceManager { get { return _serviceManager; } }

        public World World { get { return _world; } }

        static void Main(string[] args)
        {
            try
            {
                Instance = new Server();
                Instance.Start();
            }
            catch (Exception e)
            {
                Logger.Fatal("Unable to start the server.", e);
            }
        }
    }
}
