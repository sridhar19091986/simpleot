using System;

namespace SimpleOT.Net
{
	public static class Adler
	{
        public static uint Generate(Message message)
        {
            var length = message.WriterIndex;
            var offset = 6;
            var buffer = message.Buffer;

            if (length - offset <= 0)
                throw new InvalidOperationException("Invalid message length.");

            var unSum1 = Constants.AdlerChecksumStart & 0xFFFF;
            var unSum2 = (Constants.AdlerChecksumStart >> 16) & 0xFFFF;

            for (var i = offset; i < length; i++)
            {
                unSum1 = (unSum1 + buffer[i]) % Constants.AdlerChecksumBase;
                unSum2 = (unSum1 + unSum2) % Constants.AdlerChecksumBase;
            }

            return (unSum2 << 16) + unSum1;
        }
	}
}
