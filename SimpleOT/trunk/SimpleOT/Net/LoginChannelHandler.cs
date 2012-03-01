using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    public class LoginChannelHandler : DefaultChannelHandler
    {
        public LoginChannelHandler(IChannel channel) : base(channel) { }

        protected override void ChannelMessageReceivedHandler(IChannel channel, Message message)
        {

        }

    }
}
