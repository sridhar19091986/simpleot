using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SimpleOT.Threading;
using System.Net;
using SimpleOT.Collections;
using NLog;

namespace SimpleOT.Net
{
    public class Connection
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private enum ReceiveType
        {
            Head,
            Body
        }

        private Socket _socket;
        private ServicePort _servicePort;
        private Protocol _protocol;

        private Message _message;

        private SocketAsyncEventArgs _sendEventArgs;
        private object _sendLock;
        private bool _sending;
        private bool _closing;
        private int _pendingSend;

        private uint _receiveTimeoutScheduleId;
        private uint _sendTimeoutScheduleId;

        private int _receiveTimeout;
        private int _sendTimeout;

        private bool _firstMessageReceived;
		
		private string _remoteAddress;

        public Connection(Socket socket, ServicePort servicePort)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if(servicePort == null)
                throw new ArgumentNullException("servicePort");
            if (!socket.Connected)
                throw new ArgumentException("The socket is not connected.");

            _socket = socket;
            _servicePort = servicePort;

            _message = new Message(true);
            _sendLock = new object();

            _sendEventArgs = new SocketAsyncEventArgs();
            _sendEventArgs.Completed += SendCallback;
			
			_remoteAddress = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
        }

        public void Accept()
        {
            Accept(null);
        }

        public void Accept(Protocol protocol)
        {
            if (protocol != null)
            {
                _protocol = protocol;
				_protocol.Connection = this;
                OnOpen();
            }

            Receive(null);
        }

        protected void Receive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (_socket == null || !_socket.Connected)
                return;

            if (receiveEventArgs == null)
            {
                receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.Completed += ReceiveCallback;
            }
			
			_message.Clear();
            receiveEventArgs.SetBuffer(_message.Buffer, _message.WriterIndex, 2);
            receiveEventArgs.UserToken = ReceiveType.Head;

            if (_receiveTimeout > 0)
                _receiveTimeoutScheduleId = _servicePort.Scheduler.Add(_receiveTimeout, OnMessageReceiveTimeout);

            if (!_socket.ReceiveAsync(receiveEventArgs))
                ReceiveCallback(this, receiveEventArgs);
        }

        protected void ReceiveCallback(object sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (_receiveTimeoutScheduleId > 0)
                _servicePort.Scheduler.Remove(_receiveTimeoutScheduleId);

            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                _message.WriterIndex += receiveEventArgs.BytesTransferred;

                var receiveType = (ReceiveType)receiveEventArgs.UserToken;

                if (receiveType == ReceiveType.Head && receiveEventArgs.BytesTransferred != 2)
                {
                    //Error, we could not retrive the message length.
                    OnExceptionCaught(new Exception("Unable to receive message length."));
                }
                else
                {
                    var length = _message.PeekUShort() + 2;

                    if (length > _message.Capacity)
                    {
                        OnExceptionCaught(new Exception("The message length is larger then the buffer."));
                    }
                    else if (length == _message.ReadableBytes)
                    {
                        //complete message

                        _message.GetUShort(); //discart the header

                        var checksumPassed = _message.PeekUInt() == Adler.Generate(_message);

                        if (checksumPassed)
                            _message.GetUInt();


                        if (!_firstMessageReceived)
                        {
                            _firstMessageReceived = true;

                            if (_protocol != null)
                            {
                                _protocol = _servicePort.CreateProtocol(_message);

                                if (_protocol == null)
                                {
									logger.Error("No protocol found. Closing connection.");
                                    Close();
                                    return;
                                }

                                _protocol.Connection = this;
                            }

                            OnReceiveFirstMessage();
                        }
                        else
                        {
                            OnReceiveMessage();
                        }

                        Receive(receiveEventArgs);
                    }
                    else
                    {
                        //incomplete message
                        receiveEventArgs.SetBuffer(_message.Buffer, _message.WriterIndex, length - _message.ReadableBytes);
                        receiveEventArgs.UserToken = ReceiveType.Body;

                        if (_receiveTimeout > 0)
                            _receiveTimeoutScheduleId = _servicePort.Scheduler.Add(_receiveTimeout, OnMessageReceiveTimeout);

                        if (!_socket.ReceiveAsync(receiveEventArgs))
                            ReceiveCallback(this, receiveEventArgs);
                    }
                }
            }
            else
            {
                Close();
            }
        }

        public void Write(Message message)
        {
            OnSendMessage(message);

            lock (_sendLock)
            {
                _pendingSend++;

                if (_sending)
                {
                    _servicePort.OutputMessagePool.PutSend(message);
                    return;
                }

                if (_sendTimeout > 0)
                    _sendTimeoutScheduleId = _servicePort.Scheduler.Add(_sendTimeout, OnMessageSendTimeout);

                _sendEventArgs.SetBuffer(message.Buffer, _message.ReaderIndex, message.ReadableBytes);
                _sendEventArgs.UserToken = message;
                _sending = true;

                _socket.SendAsync(_sendEventArgs);
            }
        }

        protected void SendCallback(object sender, SocketAsyncEventArgs e)
        {
            lock (_sendLock)
            {
                if (_sendTimeoutScheduleId > 0)
                    _servicePort.Scheduler.Remove(_sendTimeoutScheduleId);

                //TODO: Check if we transfered all the content.

                _pendingSend--;
                _sending = false;

                _servicePort.OutputMessagePool.Put(e.UserToken as Message);
            }
        }

        public void Close()
        {
			lock(this) 
			{
            	if (_closing)
                	return;

            	_closing = true;
			}
			
            try
            {
                _socket.Shutdown(SocketShutdown.Receive);
            }
            catch (Exception)
            {
            }

            CloseSocket();
        }

        private void CloseSocket()
        {
			lock(this) 
			{
            	if (!_closing)
             	   return;
			}

            if (_pendingSend > 0)
            {
                _servicePort.Dispatcher.Add(new Task(CloseSocket));
            }
            else
            {
                try
				{
					_socket.Close();
				}
				catch(Exception)
				{					
				}
				
				_socket = null;
                OnClosed();
            }
        }

        #region Event Raise

        protected virtual void OnOpen()
        {
            try
            {
                if (_protocol != null)
                    _protocol.OnConnectionOpen();
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnClosed()
        {
            try
            {
				_servicePort.OnConnectionClosed(this);
				
                if (_protocol != null)
                    _protocol.OnConnectionClosed();
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected void OnExceptionCaught(Exception exception)
        {
            try
            {
                if (_protocol != null)
                    _protocol.OnExceptionCaught(exception);
            }
            catch (Exception e)
            {
                logger.ErrorException("Error while handling exception.", e);
            }
        }

        protected virtual void OnReceiveFirstMessage()
        {
            try
            {
                if (_protocol != null)
                    _protocol.OnReceiveFirstMessage(_message);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnReceiveMessage()
        {
            try
            {
                if (_protocol != null)
                {


                    _protocol.OnReceiveMessage(_message);
                }
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnSendMessage(Message message)
        {
            try
            {
                if (_protocol != null)
                    _protocol.OnSendMessage(message);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected void OnMessageReceiveTimeout()
        {
            _receiveTimeoutScheduleId = 0;
            Close();
        }

        protected void OnMessageSendTimeout()
        {
            _sendTimeoutScheduleId = 0;
            Close();
        }

        #endregion

        public int ReceiveTimeout { get { return _receiveTimeout; } set { _receiveTimeout = value; } }
        public int SendTimeout { get { return _sendTimeout; } set { _sendTimeout = value; } }
        public string RemoteAddress { get { return _remoteAddress; } }

        public OutputMessagePool OutputMessagePool { get { return _servicePort.OutputMessagePool; } }
        public Dispatcher Dispatcher { get { return _servicePort.Dispatcher; } }
        public Scheduler Scheduler { get { return _servicePort.Scheduler; } }
	}
}