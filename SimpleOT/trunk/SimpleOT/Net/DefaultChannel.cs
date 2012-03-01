using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Threading;
using SimpleOT.Collections;
using NLog;

namespace SimpleOT.Net
{
    public abstract class DefaultChannel : IChannel
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public event ChannelEventHandler ChannelOpen;
        public event ChannelEventHandler ChannelClosed;
        public event ChannelExceptionCaughtEventHandler ExceptionCaught;
        public event ChannelMessageEventHandler MessageReceived;
        public event ChannelMessageEventHandler MessageSent;
        public event ChannelEventHandler MessageReceiveTimeout;
        public event ChannelEventHandler MessageSendTimeout;

        #region Event Raise

        protected virtual void OnOpen(IChannel channel)
        {
            try
            {
                var handler = ChannelOpen;
                if (handler != null)
                    handler(channel);
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
                var handler = ChannelClosed;
                if (handler != null)
                    handler(this);
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
                var handler = ExceptionCaught;
                if (handler != null)
                    handler(this, exception);
            }
            catch (Exception e)
            {
                logger.ErrorException("Error while handling exception.", e);
            }
        }

        protected virtual void OnMessageReceived(Message message)
        {
            try
            {
                var handler = MessageReceived;
                if (handler != null)
                    handler(this, message);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnMessageWritten(Message message)
        {
            try
            {
                var handler = MessageSent;
                if (handler != null)
                    handler(this, message);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnMessageReceiveTimeout()
        {
            try
            {
                var handler = MessageReceiveTimeout;
                if (handler != null)
                    handler(this);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        protected virtual void OnMessageSendTimeout()
        {
            try
            {
                var handler = MessageSendTimeout;
                if (handler != null)
                    handler(this);
            }
            catch (Exception e)
            {
                OnExceptionCaught(e);
            }
        }

        #endregion

        public virtual void Open() { }

        public virtual void Write(Message message) { }

        public abstract void Close();

        public abstract Object Attachment { get; set; }

        public abstract IChannel Parent { get; }

        public abstract Dispatcher Dispatcher { get; }

        public abstract Scheduler Scheduler { get; }

        public abstract OutputMessagePool OutputMessagePool { get; }

        public abstract bool IsChecksumed { get; set; }

        public abstract bool IsEncrypted { get; set; }

        public abstract uint[] XteaKey { get; set; }

    }
}
