using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Threading;
using SimpleOT.Collections;

namespace SimpleOT.Net
{
    public delegate void ChannelEventHandler(IChannel channel);
    public delegate void ChannelExceptionCaughtEventHandler(IChannel channel, Exception exception);
    public delegate void ChannelMessageEventHandler(IChannel channel, Message message);

    public interface IChannel
    {
        event ChannelEventHandler ChannelOpen;
        event ChannelEventHandler ChannelClosed;
        event ChannelExceptionCaughtEventHandler ExceptionCaught;
        event ChannelMessageEventHandler MessageReceived;
        event ChannelMessageEventHandler MessageSent;
        event ChannelEventHandler MessageReceiveTimeout;
        event ChannelEventHandler MessageSendTimeout;

        void Open();
        void Close();

        void Write(Message message);

        Object Attachment { get; set; }
        IChannel Parent { get; }
        Dispatcher Dispatcher { get; }
        Scheduler Scheduler { get; }
        OutputMessagePool OutputMessagePool { get; }

        bool IsChecksumed { get; set; }
        bool IsEncrypted { get; set; }
        uint[] XteaKey { get; set; }
    }
}
