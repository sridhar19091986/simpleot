using System;

namespace SimpleOT.Net
{
	public static class Xtea
	{
		public unsafe static void Encrypt (Message message, uint[] key, int offset)
		{
			var msgSize = message.ReadableBytes - offset;
			var pad = msgSize % 8;
			
			if (pad > 0) {
				message.WriterIndex += (8 - pad);
				msgSize += 8 - pad;
			}
			
			fixed (byte* bufferPtr = message.Buffer) {
				var words = (uint*)(bufferPtr + offset);

				for (var pos = 0; pos < msgSize / 4; pos += 2) {
					uint sum = 0;
					uint count = 32;

					while (count-- > 0) {
						words [pos] += (words [pos + 1] << 4 ^ words [pos + 1] >> 5) + words [pos + 1] ^ sum
                            + key [sum & 3];
						sum += Constants.XTEA_DELTA;
						words [pos + 1] += (words [pos] << 4 ^ words [pos] >> 5) + words [pos] ^ sum
                            + key [sum >> 11 & 3];
					}
				}
			}
		}

		public unsafe static void Decrypt (Message message, uint[] key, int offset)
		{
			var msgSize = message.ReadableBytes - offset;

            if (msgSize % 8 > 0)
                throw new Exception("Invalid Xtea encrypted message size.");

		    fixed (byte* bufferPtr = message.Buffer) {
				var words = (uint*)(bufferPtr + message.ReaderIndex + offset);
                
				for (var pos = 0; pos < msgSize / 4; pos += 2) {
					var count = 32;
					var sum = 0xC6EF3720;

					while (count-- > 0) {
						words [pos + 1] -= (words [pos] << 4 ^ words [pos] >> 5) + words [pos] ^ sum
                            + key [sum >> 11 & 3];
						sum -= Constants.XTEA_DELTA;
						words [pos] -= (words [pos + 1] << 4 ^ words [pos + 1] >> 5) + words [pos + 1] ^ sum
                            + key [sum & 3];
					}
				}
			}
		}
	}
}

