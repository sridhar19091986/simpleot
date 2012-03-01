using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using SimpleOT.Threading;
using System.Net;
using SimpleOT.Collections;

namespace SimpleOT.Net
{
    public class SocketChannel : DefaultChannel
    {
        private Socket _socket;
        private Message _message;
        private IChannel _parent;

        private SocketAsyncEventArgs _sendEventArgs;
        private object _sendLock;
        private bool _sending;
        private int _pendingSend;
        private bool _closing;

        private uint _receiveTimeoutScheduleId;
        private uint _sendTimeoutScheduleId;

        private int _receiveTimeout;
        private int _sendTimeout;

        private bool _isChecksumed;

        private uint[] _xteaKey;
        private bool _isEncrypted;

        private Object _attachment;

        public SocketChannel(Socket socket, IChannel parent)
        {
            if (socket == null)
                throw new ArgumentNullException("socket");
            if(parent == null)
                throw new ArgumentNullException("parent");
            if (!socket.Connected)
                throw new ArgumentException("The socket is not connected.");

            this._socket = socket;
            this._parent = parent;

            this._message = new Message(true);
            this._sendLock = new object();

            this._sendEventArgs = new SocketAsyncEventArgs();
            this._sendEventArgs.Completed += SendCallback;

            Receive(null);
        }

        protected void Receive(SocketAsyncEventArgs receiveEventArgs)
        {
            if (!_socket.Connected)
                return;

            if (receiveEventArgs == null)
            {
                receiveEventArgs = new SocketAsyncEventArgs();
                receiveEventArgs.Completed += ReceiveCallback;
            }

            receiveEventArgs.SetBuffer(_message.Buffer, _message.WriterIndex, _message.WritableBytes);

            if (_receiveTimeout > 0)
                _receiveTimeoutScheduleId = _parent.Scheduler.Add(_receiveTimeout, OnMessageReceiveTimeout);

            if (!_socket.ReceiveAsync(receiveEventArgs))
                ReceiveCallback(this, receiveEventArgs);
        }

        protected void ReceiveCallback(object sender, SocketAsyncEventArgs receiveEventArgs)
        {
            if (_receiveTimeoutScheduleId > 0)
                _parent.Scheduler.Remove(_receiveTimeoutScheduleId);

            if (receiveEventArgs.BytesTransferred > 0 && receiveEventArgs.SocketError == SocketError.Success)
            {
                _message.WriterIndex += receiveEventArgs.BytesTransferred;

                OnMessageReceived(_message);
                _message.DiscardReadBytes();

                Receive(receiveEventArgs);
            }
            else
            {
                Close();
            }
        }

        public override void Write(Message message)
        {
            OnMessageWritten(message);

            lock (_sendLock)
            {

                _pendingSend++;

                if (_sending)
                {
                    _parent.OutputMessagePool.PutSend(message);
                    return;
                }

                if (_sendTimeout > 0)
                    _sendTimeoutScheduleId = _parent.Scheduler.Add(_sendTimeout, OnMessageSendTimeout);

                _sendEventArgs.SetBuffer(message.Buffer, 0, message.WriterIndex);
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
                    _parent.Scheduler.Remove(_sendTimeoutScheduleId);

                //TODO: Check if we transfered all the content.

                _pendingSend--;
                _sending = false;

                _parent.OutputMessagePool.Put(e.UserToken as Message);
            }
        }

        public override void Close()
        {
            if (_closing)
                return;

            _closing = true;

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
            if (!_closing)
                return;

            if (_pendingSend > 0)
            {
                _parent.Scheduler.Dispatcher.Add(new Task(CloseSocket));
            }
            else
            {
                _socket.Close();
                OnClosed();
            }
        }

        public int ReceiveTimeout { get { return _receiveTimeout; } set { _receiveTimeout = value; } }
        public int SendTimeout { get { return _sendTimeout; } set { _sendTimeout = value; } }
        public string RemoteAddress { get { return ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString(); } }

        public override object Attachment { get { return _attachment; } set { _attachment = value; } }

        public override IChannel Parent { get { return _parent; } }
        public override OutputMessagePool OutputMessagePool { get { return _parent.OutputMessagePool; } }
        public override Dispatcher Dispatcher { get { return _parent.Dispatcher; } }
        public override Scheduler Scheduler { get { return _parent.Scheduler; } }

        public override bool IsChecksumed { get { return _isChecksumed; } set { _isChecksumed = value; } }
        public override bool IsEncrypted { get { return _isEncrypted; } set { _isEncrypted = value; } }

        public override uint[] XteaKey { get { return _xteaKey; } set { _xteaKey = value; } }
	}
}