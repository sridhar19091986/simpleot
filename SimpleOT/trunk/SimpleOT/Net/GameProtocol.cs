using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Net
{
    [ProtocolInfo("Game", 0x00, SingleSocket = true)]
    public class GameProtocol : Protocol
    {
		private static readonly Random random = new Random();
		
		public override void OnConnectionOpen ()
		{
			var output = Server.OutputMessagePool.Get (Connection, false);
			
			output.PutByte(0x1F);
			
			output.PutByte ((byte)random.Next(0, 0xFF));
			output.PutByte ((byte)random.Next(0, 0xFF));
			
			output.PutUShort(0);
			
			output.PutByte ((byte)random.Next(0, 0xFF));
			
			HasChecksum = true;
			
			Connection.Write(output);
		}
		
		public override void OnReceiveFirstMessage (Message message)
		{
            message.GetUShort(); //OS
            var version = message.GetUShort();
			
			Rsa.Decrypt(message);

            uint[] key = new uint[4];
            key[0] = message.GetUInt();
            key[1] = message.GetUInt();
            key[2] = message.GetUInt();
            key[3] = message.GetUInt();

            XteaKey = key;
            HasXteaEncryption = true;
			
			var isGm = message.GetBool();
			var accountName = message.GetString();
			var playerName = message.GetString();
			var password = message.GetString();
			
			message.ReaderIndex += 6;
			
			if (version != Constants.ClientVersionNumber)
            {
                Disconnect(0x0A, "This server requires client version " + Constants.ClientVersion + ".");
                return;
            }
			
			Disconnect(0x14, "Under construction.");
		}
		
		
		
    }
}
