using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SimpleOT.Threading;
using SimpleOT.Collections;
using System.Net;
using SimpleOT.IO;

namespace SimpleOT.Net
{
    public class ServicePort
    {
        private readonly Server _server;

        private readonly ISet<IService> _services;
		private readonly ISet<Connection> _connections;

		private Socket _acceptSocket;

        private readonly int _port;

        public ServicePort(Server server, int port)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
			_port = port;

            _services = new HashSet<IService>();
			_connections = new HashSet<Connection>();
        }

		public void Open()
		{
            lock (this)
            {
                if (_acceptSocket != null)
                    throw new InvalidOperationException("Server already opened.");

                _acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);

                _acceptSocket.Bind(endPoint);
                _acceptSocket.Listen((int)SocketOptionName.MaxConnections);

                Accept(null);
            }
		}
		
		public void Close()
		{
			lock(this)
			{
				if(_acceptSocket == null)
					throw new InvalidOperationException("Server already closed.");

				_acceptSocket.Close();
                _acceptSocket = null;
			}
		}
		
		protected void Accept(SocketAsyncEventArgs acceptEventArgs) 
		{
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
			if(_acceptSocket == null || e.AcceptSocket == null)
				return;

            lock (_connections)
            {
                var connection = new Connection(_server, this, e.AcceptSocket);
				
				connection.SendTimeout = Constants.ConnectionSendTimeout;
				connection.ReceiveTimeout = Constants.ConnectionReceiveTimeout;
				
#if DEBUG_SERVICE_PORT
				Logger.Debug(string.Format("Adding conection from {0}.", connection.RemoteAddress));
#endif

				_connections.Add(connection);

               	if(_services.Any (x=>x.SingleSocket))
					connection.Accept(_services.First().CreateProtocol());	
				else
					connection.Accept();
            }
				
			Accept(e);
		}

        public void AddService(IService service)
        {
            if (service == null)
                throw new ArgumentNullException("service");

            _services.Add(service);
        }

        public void OnConnectionClosed(Connection connection)
        {
#if DEBUG_SERVICE_PORT
			Logger.Debug(string.Format("Removing conection from {0}.", connection.RemoteAddress));
#endif
            lock (_connections)
                _connections.Remove(connection);
        }

        public Protocol CreateProtocol(Message message, bool hasChecksum)
        {
			byte protocolId = message.GetByte();
            var service = _services.FirstOrDefault(x => x.ProtocolIndentifier == protocolId && x.HasChecksum == hasChecksum);
			
			if(service != null)
				return service.CreateProtocol();
			
			
            return null;
        }

        public bool SingleSocket { get { return _services.Count > 0 && _services.First().SingleSocket; } }
        public object ProtocolNames { get { return String.Join(", ", _services.Select(x => x.ProtocolName)); } }
    }
}
