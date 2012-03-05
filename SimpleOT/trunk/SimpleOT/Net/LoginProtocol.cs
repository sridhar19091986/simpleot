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

            	uint32_t key[4];
	key[0] = msg.GetU32();
	key[1] = msg.GetU32();
	key[2] = msg.GetU32();
	key[3] = msg.GetU32();
	enableXTEAEncryption();
	setXTEAKey(key);

	std::string accname = msg.GetString();
	std::string password = msg.GetString();

        }

    }
}
