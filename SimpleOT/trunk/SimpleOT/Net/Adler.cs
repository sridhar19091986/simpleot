using System;

namespace SimpleOT.Net
{
	public static class Adler
	{
        public static uint Generate(Message message)
        {
            if (message.ReadableBytes <= 0)
                throw new InvalidOperationException("Invalid message length.");

            var end = message.WriterIndex;
            var start = message.ReaderIndex;
            var buffer = message.Buffer;

            var unSum1 = Constants.AdlerChecksumStart & 0xFFFF;
            var unSum2 = (Constants.AdlerChecksumStart >> 16) & 0xFFFF;

            for (var i = start; i < end; i++)
            {
                unSum1 = (unSum1 + buffer[i]) % Constants.AdlerChecksumBase;
                unSum2 = (unSum1 + unSum2) % Constants.AdlerChecksumBase;
            }

            return (unSum2 << 16) + unSum1;
        }
	}
}
