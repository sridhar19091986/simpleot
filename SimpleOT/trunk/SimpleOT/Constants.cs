using System;

namespace SimpleOT
{
	public static class Constants
	{
		public const int MESSAGE_DEFAULT_SIZE = 16384; // 16kb
		public const int MESSAGE_SEND_MAX_TIME = 100000000; // 10s
		public const int MESSAGE_SEND_MIN_TIME = 100000; // 10ms
		public const int MESSAGE_SEND_MIN_SIZE = 1024; // 1kb
		public const int MESSAGE_HEADER_SIZE = 2; // 2
		
		public const int SCHEDULER_MIN_TIME = 50; // 50ms
		public const int SCHEDULER_START_ID = 10000;
		
		public const int DISPATCHER_TASK_EXPIRATION = 2000; //2s
		
		public const uint ALDER_CHECKSUM_BASE = 0xFFF1;
		public const uint ALDER_CHECKSUM_START = 0x0001;
		public const int ADLER_CHECKSUM_SIZE = 4; //4b
		
		public const uint XTEA_DELTA = 0x9E3779B9;
		
	}
}

