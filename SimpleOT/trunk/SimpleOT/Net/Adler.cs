using System;

namespace SimpleOT.Net
{
	public static class Adler
	{
		public static uint Generate (byte[] buffer, int offset, int length)
		{
			if (buffer == null || length - offset <= 0)
				throw new InvalidOperationException ("Invalid message length.");

			var unSum1 = Constants.ALDER_CHECKSUM_START & 0xFFFF;
			var unSum2 = (Constants.ALDER_CHECKSUM_START >> 16) & 0xFFFF;

			for (var i = offset; i < length; i++) {
				unSum1 = (unSum1 + buffer [i]) % Constants.ALDER_CHECKSUM_BASE;
				unSum2 = (unSum1 + unSum2) % Constants.ALDER_CHECKSUM_BASE;
			}

			return (unSum2 << 16) + unSum1;
		}
	}
}
