using System;

namespace SimpleOT.Commons.Data
{
	public abstract class Repository
	{
		private IConnectionFactory _connectionFactory;
		
		protected Repository (IConnectionFactory connectionFactory)
		{
			if(connectionFactory == null)
				throw new ArgumentNullException("connectionFactory");
			
			this._connectionFactory = connectionFactory;
		}
		
		protected IConnectionFactory ConnectionFactory{get{return _connectionFactory;}}
	}
}

