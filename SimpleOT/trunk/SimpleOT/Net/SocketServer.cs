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
	public class SocketServer
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		private IHandlerFactory _protocolFactory;
		private ISet<Connection> _connections;
		private SocketServerState _state;
		private Socket _acceptSocket;
		private Scheduler _scheduler;
		private OutputMessagePool _outputMessagePool;
		private int _port;
		private int _maxConnections;
		
        public SocketServer(int port, Scheduler scheduler, IHandlerFactory protocolFactory)
        {
			if(scheduler == null)
				throw new ArgumentNullException("scheduler");
			if(protocolFactory == null)
				throw new ArgumentNullException("protocolFactory");
			
			this._port = port;
			this._protocolFactory = protocolFactory;
			this._connections = new HashSet<Connection>();
			this._outputMessagePool = new OutputMessagePool(10, 100);
			this._scheduler = scheduler;
			
			this._state = SocketServerState.Terminated;
        }
		
        internal void OnConnectionClosed(Connection connection)
        {
            lock (_connections)
                _connections.Remove(connection);
        }
		
		public void Open()
		{
			lock(this)
			{
				if(_state == SocketServerState.Running)
					throw new InvalidOperationException("Server already opened.");
				
				if(_state == SocketServerState.Terminated)
				{
					_state = SocketServerState.Running;
					this._acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		    		IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);
					
					_acceptSocket.Bind(endPoint);
		    		_acceptSocket.Listen((int)SocketOptionName.MaxConnections);
		            
		            Accept(null);
				} else {
					_state = SocketServerState.Running;
				}
			}
		}
		
		public void Close()
		{
			lock(this)
			{
				if(_state == SocketServerState.Terminated)
					throw new InvalidOperationException("You can't close a terminated server.");
				if(_state == SocketServerState.Closing)
					throw new InvalidOperationException("Server already closed.");
				
				_state = SocketServerState.Closing;
				_acceptSocket.Close();
			}
		}
		
		public void Shutdown()
		{
			lock(this)
			{
				if(_state == SocketServerState.Terminated)
					throw new InvalidOperationException("Server already terminated.");
				
				_state = SocketServerState.Terminated;
				_acceptSocket.Close();
			}
		}
		
		protected void Accept(SocketAsyncEventArgs acceptEventArgs) 
		{
			if(_state == SocketServerState.Terminated)
				return;
			
			if(acceptEventArgs == null)
			{
				acceptEventArgs = new SocketAsyncEventArgs();
				acceptEventArgs.Completed += AcceptCallback;
			}
			
			acceptEventArgs.AcceptSocket = null;
			
			if(!_acceptSocket.AcceptAsync(acceptEventArgs))
				AcceptCallback(this, acceptEventArgs);
		}

		protected void AcceptCallback (object sender, SocketAsyncEventArgs e)
		{
			if(e.AcceptSocket == null)
				return;
			
			lock(_connections)
			{
				if(_state == SocketServerState.Running && _connections.Count < _maxConnections) {
					Connection connection = new Connection(this, e.AcceptSocket, _protocolFactory.GetHandler());
					
					connection.ReceiveTimeout = 1000;
					connection.SendTimeout = 1000;
					
					connection.Receive(null);
					
					_connections.Add(connection);
				} else {
					logger.Debug("Rejecting connection.");
					e.AcceptSocket.Close();
				}
			}
				
			Accept(e);
		}
		
		public SocketServerState State{get{return _state;}}
		public int MaxConnections{get{return _maxConnections;}set{_maxConnections = value;}}
		public OutputMessagePool OutputMessagePool{get{return _outputMessagePool;}}
		
		internal Scheduler Scheduler{get{return _scheduler;}}
	}
}
