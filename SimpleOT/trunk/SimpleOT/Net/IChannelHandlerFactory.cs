using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    public interface IChannelHandlerFactory
    {
        bool IsMultiProtocol { get; }

        IChannelHandler GetChannelHandler();
        IChannelHandler GetChannelHandler(Message message);
    }
}
