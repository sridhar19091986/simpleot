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
	public class SocketServerChannel : DefaultChannel
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		private ISet<IChannel> _channels;
		private Socket _acceptSocket;
        private Dispatcher _dispatcher;
		private Scheduler _scheduler;
		private OutputMessagePool _outputMessagePool;
		private int _port;
		private int _maxConnections;

        private IChannelHandlerFactory _channelHandlerFactory;

        private Object _attachment;

        public SocketServerChannel(int port, Dispatcher dispatcher, Scheduler scheduler, IChannelHandlerFactory channelHandlerFactory)
        {
            if(dispatcher == null)
                throw new ArgumentNullException("dispatcher");
			if(scheduler == null)
				throw new ArgumentNullException("scheduler");
            if (channelHandlerFactory == null)
                throw new ArgumentNullException("channelHandlerFactory");

			this._port = port;
            this._dispatcher = dispatcher;
            this._scheduler = scheduler;
            this._channelHandlerFactory = channelHandlerFactory;

			this._channels = new HashSet<IChannel>();
			this._outputMessagePool = new OutputMessagePool(10, 100);

            this._dispatcher.AfterDispatchTask += _outputMessagePool.ProcessEnqueueMessages;
        }

        ~SocketServerChannel()
        {
            this.Dispatcher.AfterDispatchTask -= _outputMessagePool.ProcessEnqueueMessages;
        }

		public override void Open()
		{
            lock (this)
            {
                if (_acceptSocket != null)
                    throw new InvalidOperationException("Server already opened.");

                this._acceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, _port);

                _acceptSocket.Bind(endPoint);
                _acceptSocket.Listen((int)SocketOptionName.MaxConnections);

                Accept(null);
            }
		}
		
		public override void Close()
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
			
			lock(_channels)
			{
				if(_channels.Count < _maxConnections) 
                {
                    var channel = new SocketChannel(e.AcceptSocket, this);
                    channel.ChannelClosed += ChannelClosedHandler;

					_channels.Add(channel);
                    OnOpen(channel);
				} 
                else 
                {
					logger.Debug("Rejecting connection.");
					e.AcceptSocket.Close();
				}
			}
				
			Accept(e);
		}

        protected void ChannelClosedHandler(IChannel channel)
        {
            lock (_channels)
            {
                channel.ChannelClosed -= ChannelClosedHandler;
                _channels.Remove(channel);
            }
        }

        public int MaxConnections { get { return _maxConnections; } set { _maxConnections = value; } }

        public override object Attachment { get { return _attachment; } set { _attachment = value; } }

        public override IChannel Parent { get { return null; } }
        public override OutputMessagePool OutputMessagePool { get { return _outputMessagePool; } }
        public override Dispatcher Dispatcher { get { return _dispatcher; } }
        public override Scheduler Scheduler { get { return _scheduler; } }

        public override bool IsChecksumed { get { return false; } set { } }
        public override bool IsEncrypted { get { return false; } set { } }

        public override uint[] XteaKey { get { return null; } set { } }
	}
}
