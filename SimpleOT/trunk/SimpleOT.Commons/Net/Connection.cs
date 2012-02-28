using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SimpleOT.Commons.Threading;
using System.Net;

namespace SimpleOT.Commons.Net
{
	public class Connection
	{
		private Socket _socket;
		private SocketServer _socketServer;
		private Message _buffer;
		private IHandler _handler;
		private SocketAsyncEventArgs _sendEventArgs;
		private object _sendLock;
		private bool _sending;
		private int _pendingSend;
		private bool _closing;
		
		private uint _receiveTimeoutScheduleId;
		private uint _sendTimeoutScheduleId;
		
		private int _receiveTimeout;
		private int _sendTimeout;
		
		public Connection (SocketServer socketServer, Socket socket, IHandler protocol)
		{
			if (socketServer == null)
				throw new ArgumentNullException ("socketServer");
			if (socket == null)
				throw new ArgumentNullException ("socket");
			if (protocol == null)
				throw new ArgumentNullException ("protocol");
			if (!socket.Connected)
				throw new ArgumentException ("The socket is not connected.");
			
			this._socketServer = socketServer;
			this._socket = socket;
			this._handler = protocol;
			
			this._buffer = new Message (true);
			this._sendLock = new object ();
			
			this._sendEventArgs = new SocketAsyncEventArgs ();
			this._sendEventArgs.Completed += SendCallback;
			
			OnOpen ();
		}

		internal void Receive (SocketAsyncEventArgs receiveEventArgs)
		{
			if (!_socket.Connected)
				return;
			
			if (receiveEventArgs == null) {
				receiveEventArgs = new SocketAsyncEventArgs ();
				receiveEventArgs.Completed += ReceiveCallback;
			}

			receiveEventArgs.SetBuffer (_buffer.Buffer, _buffer.WriterIndex, _buffer.WritableBytes);
			
			if(_receiveTimeout > 0)
				_receiveTimeoutScheduleId = _socketServer.Scheduler.Add(new Schedule(_receiveTimeout, OnMessageReceiveTimeout));
			
			if (!_socket.ReceiveAsync (receiveEventArgs))
				ReceiveCallback (this, receiveEventArgs);
		}

		protected void ReceiveCallback (object sender, SocketAsyncEventArgs receiveEventArgs)
		{
			if(_receiveTimeoutScheduleId > 0)
				_socketServer.Scheduler.Remove(_receiveTimeoutScheduleId);
			
			if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success) {
				_buffer.WriterIndex += receiveEventArgs.BytesTransferred;
				
				OnMessageReceived (_buffer);
				Receive (receiveEventArgs);
			} else {
				Close ();
			}
		}

		public void Send (Message message)
		{
			OnMessageWritten (message);

			lock (_sendLock) {
				
				_pendingSend++;
				
				if (_sending) {
					_socketServer.OutputMessagePool.PutSend(message);
					return;
				}
				
				if(_sendTimeout > 0)
					_sendTimeoutScheduleId = _socketServer.Scheduler.Add(new Schedule(_sendTimeout, OnMessageSendTimeout));
				
				_sendEventArgs.SetBuffer (message.Buffer, 0, message.WriterIndex);
				_sendEventArgs.UserToken = message;
				_sending = true;
				
				_socket.SendAsync (_sendEventArgs);
			}
		}

		protected void SendCallback (object sender, SocketAsyncEventArgs e)
		{
			lock (_sendLock) {
				
				if(_sendTimeoutScheduleId > 0)
					_socketServer.Scheduler.Remove(_sendTimeoutScheduleId);
				
				//TODO: Check if we transfered all the content.
				
				_pendingSend--;
				_sending = false;
				
				_socketServer.OutputMessagePool.Put(e.UserToken as Message);
			}
		}

		public void Close ()
		{
			if(_closing)
				return;
			
			_closing = true;
			
			try {
				_socket.Shutdown (SocketShutdown.Receive);
			} catch (Exception) {
			}
			
			CloseSocket();
		}
		
		private void CloseSocket()
		{
			if(!_closing)
				return;
			
			if(_pendingSend > 0) {
				_socketServer.Scheduler.Dispatcher.Add(new Task(CloseSocket));
			} else {
				_socket.Close ();
				OnClosed ();
			}
		}
		
		#region Event Raise
		
		protected void OnOpen ()
		{
			try {
				_handler.ConnectionOpen (this);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		protected void OnClosed ()
		{
			try {
				_socketServer.OnConnectionClosed (this);
				_handler.ConnectionClosed (this);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		protected void OnExceptionCaught (Exception exception)
		{
			try {
				
				_handler.ExceptionCaught (this, exception);
			} catch (Exception e) {
				
				Console.WriteLine ("Error while handling exception. Details: " + e.Message);
			}
		}
		
		protected void OnMessageReceived (Message message)
		{
			try {
				var readableBytes = 0;

				do {
					readableBytes = message.ReadableBytes;
					_handler.MessageReceived (this, message);

				} while (readableBytes != message.ReadableBytes);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		protected void OnMessageWritten (Message message)
		{
			try {
				_handler.MessageSent (this, message);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		protected void OnMessageReceiveTimeout ()
		{
			_receiveTimeoutScheduleId = 0;
			
			try {
				_handler.MessageReceiveTimeout (this);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		protected void OnMessageSendTimeout ()
		{
			_sendTimeoutScheduleId = 0;
			
			try {
				_handler.MessageSendTimeout (this);
			} catch (Exception e) {
				OnExceptionCaught (e);
			}
		}
		
		#endregion
		
		public int ReceiveTimeout{get{return _receiveTimeout;}set{_receiveTimeout=value;}}
		public int SendTimeout{get{return _sendTimeout;}set{_sendTimeout=value;}}
		public string RemoteAddress{get{return ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();}}
	}
}