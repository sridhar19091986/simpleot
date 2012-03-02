using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    public interface IService
    {
        Protocol CreateProtocol();

        string ProtocolName { get; }
        byte ProtocolIndentifier { get; }
        bool SingleSocket { get; }
    }
}
