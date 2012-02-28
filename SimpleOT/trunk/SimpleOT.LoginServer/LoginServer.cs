using System;
using System.Collections.Generic;
using NLog;
using SimpleOT.Commons.Net;
using SimpleOT.Commons.Threading;

namespace SimpleOT.LoginServer
{
	public class LoginServer
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		private SocketServer _socketServer;
		private ISet<ClientHandler> _clients;
		private Dispatcher _dispatcher;
		private Scheduler _scheduler;
		private AccountRepository _accountRepository;
		
		public LoginServer ()
		{
			_dispatcher = new Dispatcher();
			_scheduler = new Scheduler(_dispatcher);
			_dispatcher.Start();
			_scheduler.Start();
			
			_clients = new HashSet<ClientHandler> ();
			_socketServer = new SocketServer (7171, _scheduler, new ClientHandlerFactory (this));
			_socketServer.MaxConnections = 1;
			_socketServer.Open ();
		}
		
		~LoginServer()
		{
			_socketServer.Close();
			_scheduler.Shutdown();
			_dispatcher.Shutdown();
		}
		
		public void Login(ClientHandler client, string login, string password)
		{
		}
		
		public void ClientConnect (ClientHandler client)
		{	
			lock (_clients) {
				_clients.Add (client);
				logger.Debug("Client connected from {0}, There are {1} clients connected.", client.RemoteAddress, _clients.Count);
			}
		}
		
		public void ClientDisconnected (ClientHandler client)
		{
			lock (_clients) {
				_clients.Remove (client);
				logger.Debug("Client disconnected from {0}, There are {1} clients connected.", client.RemoteAddress, _clients.Count);
			}
		}
		
		public Dispatcher Dispatcher{get{return _dispatcher;}}
	}
}
