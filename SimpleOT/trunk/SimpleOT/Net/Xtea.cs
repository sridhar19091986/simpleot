using System;

namespace SimpleOT.Net
{
	public static class Xtea
	{
		public unsafe static void Encrypt (Message message, uint[] key)
		{
			var pad = message.ReadableBytes % 8;
			
			if (pad > 0)
				message.WriterIndex += (8 - pad);

            var msgSize = message.ReadableBytes;
			
			fixed (byte* bufferPtr = message.Buffer) {
				var words = (uint*)(bufferPtr + message.ReaderIndex);

				for (var pos = 0; pos < msgSize / 4; pos += 2) {
					uint sum = 0;
					uint count = 32;

					while (count-- > 0) {
						words [pos] += (words [pos + 1] << 4 ^ words [pos + 1] >> 5) + words [pos + 1] ^ sum
                            + key [sum & 3];
						sum += Constants.XteaDelta;
						words [pos + 1] += (words [pos] << 4 ^ words [pos] >> 5) + words [pos] ^ sum
                            + key [sum >> 11 & 3];
					}
				}
			}
		}

		public unsafe static void Decrypt (Message message, uint[] key)
		{
			var msgSize = message.ReadableBytes;

            if (msgSize % 8 > 0)
                throw new Exception("Invalid Xtea encrypted message size.");

		    fixed (byte* bufferPtr = message.Buffer) {
				var words = (uint*)(bufferPtr + message.ReaderIndex);
                
				for (var pos = 0; pos < msgSize / 4; pos += 2) {
					var count = 32;
					var sum = 0xC6EF3720;

					while (count-- > 0) {
						words [pos + 1] -= (words [pos] << 4 ^ words [pos] >> 5) + words [pos] ^ sum
                            + key [sum >> 11 & 3];
						sum -= Constants.XteaDelta;
						words [pos] -= (words [pos + 1] << 4 ^ words [pos + 1] >> 5) + words [pos + 1] ^ sum
                            + key [sum & 3];
					}
				}
			}
		}
	}
}

