﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    public interface IChannelHandler
    {
        IChannel Channel { get; }
    }
}
