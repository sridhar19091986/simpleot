using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    [ProtocolInfo("Login", 0x01, HasChecksum = true)]
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

            if (account == null)
            {
                SendError(0x0A, "Account name or password is not correct.");
                Connection.Close();
                return;
            }

        }

        public void SendError(byte error, string description)
        {
            var message = Server.OutputMessagePool.Get(Connection, false);

            if (message != null)
            {
                message.PutByte(error);
                message.PutString(description);

                Connection.Write(message);
            }
        }

    }
}
