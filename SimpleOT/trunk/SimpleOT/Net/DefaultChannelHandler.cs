using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    public abstract class DefaultChannelHandler : IChannelHandler
    {
        private IChannel _channel;

        public DefaultChannelHandler(IChannel channel)
        {
            if (channel == null)
                throw new ArgumentNullException("channel");

            this._channel = channel;

            Channel.ChannelClosed += new ChannelEventHandler(ChannelChannelClosedHandler);
            Channel.MessageReceived += new ChannelMessageEventHandler(ChannelMessageReceivedHandler);
            Channel.MessageSent += new ChannelMessageEventHandler(ChannelMessageSentHandler);
            Channel.MessageSendTimeout += new ChannelEventHandler(ChannelMessageSendTimeoutHandler);
            Channel.MessageReceiveTimeout += new ChannelEventHandler(ChannelMessageReceiveTimeoutHandler);
            Channel.ExceptionCaught += new ChannelExceptionCaughtEventHandler(ChannelExceptionCaughtHandler);
        }

        protected virtual void ChannelExceptionCaughtHandler(IChannel channel, Exception exception)
        {
        }

        protected virtual void ChannelMessageReceiveTimeoutHandler(IChannel channel)
        {
        }

        protected virtual void ChannelMessageSentHandler(IChannel channel, Message message)
        {
        }

        protected virtual void ChannelMessageReceivedHandler(IChannel channel, Message message)
        {
        }

        protected virtual void ChannelChannelClosedHandler(IChannel channel)
        {
        }

        protected virtual void ChannelMessageSendTimeoutHandler(IChannel channel)
        {
        }

        public IChannel Channel{get { return _channel; }}
    }
}
