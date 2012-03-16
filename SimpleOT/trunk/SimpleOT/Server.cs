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
		
        private readonly Dispatcher _dispatcher;
        private readonly Scheduler _scheduler;

        private readonly OutputMessagePool _outputMessagePool;
        private readonly ServiceManager _serviceManager;
        
        private readonly ConfigManager _configManager;
		
		private readonly IDbConnectionFactory _dbConnectionFactory;
		private readonly IAccountRepository _accountRepository;
		private readonly IPlayerRepository _playerRepository;
		private readonly IItemTypeRepository _itemTypeRepository;

        private readonly World _world;

        public Server()
        {
            _dispatcher = new Dispatcher();
            _scheduler = new Scheduler(_dispatcher);

            _dispatcher.Start();
            _scheduler.Start();

            _configManager = new ConfigManager(this);

            _dbConnectionFactory = new PostgresDbConnectionFactory();
            _accountRepository = new AccountDbRepository(_dbConnectionFactory);
			_playerRepository = new PlayerDbRepository(_dbConnectionFactory);
			_itemTypeRepository = new ItemTypeFileRepository(@"Data\items\items.otb");

            _world = new World(this);

            _outputMessagePool = new OutputMessagePool(10, 100);
            _dispatcher.AfterDispatchTask += _outputMessagePool.ProcessEnqueueMessages;

            _serviceManager = new ServiceManager(this);

            _serviceManager.Add<LoginProtocol>(_configManager.LoginPort);
            _serviceManager.Add<GameProtocol>(_configManager.GamePort);

            Logger.Info(string.Format("Local ip address: {0}", String.Join(" ", _serviceManager.PrivateIpAddresses)));
            Logger.Info(string.Format("Global ip address: {0}", String.Join(" ", _serviceManager.PublicIpAddresses)));
        }

        ~Server()
        {
            _dispatcher.AfterDispatchTask -= _outputMessagePool.ProcessEnqueueMessages;

            _scheduler.Shutdown();
            _dispatcher.Shutdown();
        }

        public IAccountRepository AccountRepository { get { return _accountRepository; } }
		public IPlayerRepository PlayerRepository { get{ return _playerRepository; } }
		public IItemTypeRepository ItemRepository{get{return _itemTypeRepository;}}

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
            }
            catch (Exception e)
            {
                Logger.Fatal("Unable to start the server.", e);
            }
        }
    }
}
