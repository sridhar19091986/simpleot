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

            if (version != Constants.ClientVersionNumber)
            {
                Disconnect(0x0A, "This server requires client version " + Constants.ClientVersion + ".");
                return;
            }

            var login = message.GetString();
            var password = message.GetString();

            if (login.Length < 5)
            {
                Disconnect(0x0A, "Invalid Account Name.");
                return;
            }

            var account = Server.Instance.AccountRepository.Load(login);

            if (account == null)
            {
                Disconnect(0x0A, "Account name or password is not correct.");
                return;
            }

            var output = Server.OutputMessagePool.Get(Connection, false);

            if (output != null)
            {
                var serverIp = Server.ServiceManager.IpAddresses.First().Key;
                foreach (var keyPair in Server.ServiceManager.IpAddresses)
                {
                    if ((keyPair.Key & keyPair.Value) == (Connection.IpAddress & keyPair.Value))
                    {
                        serverIp = keyPair.Key;
                        break;
                    }
                }

                output.PutByte(0x64);
                output.PutByte((byte)account.Characters.Count);

                foreach (var character in account.Characters)
                {
                    output.PutString(character);
                    output.PutString(Server.ConfigManager.WorldName);
                    output.PutUInt(serverIp);
                    output.PutUShort((ushort)Server.ConfigManager.GamePort);
                }

                output.PutUShort(account.PremiumDaysLeft);

                Connection.Write(output);
            }

            Connection.Close();
        }
    }
}
