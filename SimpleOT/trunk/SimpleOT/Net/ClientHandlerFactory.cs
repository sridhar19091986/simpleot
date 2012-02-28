using System;
using SimpleOT.Commons.Net;

namespace SimpleOT.Net
{
	public class ClientHandlerFactory : IHandlerFactory
	{
		private LoginServer _loginServer;
		
		public ClientHandlerFactory (LoginServer loginServer)
		{
			this._loginServer = loginServer;
		}
		
		#region IHandlerFactory implementation
		public IHandler GetHandler ()
		{
			return new ClientHandler(_loginServer);
		}
		#endregion
	}
}

