using System;

namespace SimpleOT.Commons.Net
{
	public interface IHandler
	{
		void ConnectionOpen (Connection connecion);
		
		void ConnectionClosed (Connection connection);
		
		void ExceptionCaught (Connection connection, Exception exception);

		void MessageReceived (Connection connection, Message buffer);
		
		void MessageSent (Connection connection, Message buffer);
		
		void MessageReceiveTimeout(Connection connection);
		
		void MessageSendTimeout(Connection connection);
	}
}

