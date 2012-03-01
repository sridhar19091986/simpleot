using System;

namespace SimpleOT
{
	public enum ClientOS : ushort
	{
		Windows = 1,
		Linux = 2
	}

    public enum Protocol : byte
    {
        Login,
        Game,
        Status
    }
	
	public enum ClientVersion : ushort 
	{
		V861 = 861
	}
}

