using System;

namespace SimpleOT
{
	public static class Constants
	{
        public const int MessageHeaderMaxSize = 32; // 2
		public const int MessageDefaultSize = 16384; // 16kb

		public const int OutputMessagePoolSendMaxTime = 100000000; // 10s
		public const int OutputMessagePoolSendMinTime = 100000; // 10ms
		public const int OutputMessagePoolSendMinSize = 1024; // 1kb
		
		public const int SchedulerMinTime = 50; // 50ms
		public const int SchedulerStartId = 10000;
		
		public const int DispatcherTaskExpiration = 2000; //2s
		
		public const uint AdlerChecksumBase = 0xFFF1;
		public const uint AdlerChecksumStart = 0x0001;
		public const int AdlerChecksumSize = 4; //4b
		
		public const uint XteaDelta = 0x9E3779B9;
        public const uint XteaSumStart = 0xC6EF3720;
		
		public const int ConnectionSendTimeout = 30000; //30s
		public const int ConnectionReceiveTimeout = 30000; //30s
	}
}

