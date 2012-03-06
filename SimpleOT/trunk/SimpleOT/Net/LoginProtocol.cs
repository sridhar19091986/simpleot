using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    [ProtocolInfo("Login", 0x01, HasChecksum=true)]
    public class LoginProtocol : Protocol
    {
        public LoginProtocol()
        {
            HasChecksum = true;
        }

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

            var login = message.GetString();
            var password = message.GetString();

            var account = Server.Instance.AccountRepository.FindByLogin(login);


        }

        public void SendError(byte code, string message)
        {
            //var message = Connection.OutputMessagePool.Get(
        }

    }
}
