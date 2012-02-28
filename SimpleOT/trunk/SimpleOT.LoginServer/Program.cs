using System;

namespace SimpleOT.LoginServer
{
	public class Program
	{
		private static LoginServer _loginServer;
		
		public static void Main (string[] args)
		{
			_loginServer = new LoginServer();
		}
	}
}

