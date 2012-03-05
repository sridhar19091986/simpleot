using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    [ProtocolInfo("Login", 0x10)]
    public class LoginProtocol : Protocol
    {
        public override void OnReceiveFirstMessage(Message message)
        {
            message.GetUShort();
            var version = message.GetUShort();
            message.ReaderIndex += 12;

            Rsa.Decrypt(message);

            uint[] key = new uint[4];
            key[0] = message.GetUInt();
            key[1] = message.GetUInt();
            key[2] = message.GetUInt();
            key[3] = message.GetUInt();

            XteaKey = key;
            HasXteaEncryption = true;

            var accountName = message.GetString();
            var password = message.GetString();


        }

    }
}
