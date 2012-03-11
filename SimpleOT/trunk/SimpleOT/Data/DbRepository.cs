using System;

namespace SimpleOT.Data
{
	public abstract class DbRepository
	{
		private IDbConnectionFactory _dbConnectionFactory;
		
		protected DbRepository (IDbConnectionFactory dbConnectionFactory)
		{
			if(dbConnectionFactory == null)
				throw new ArgumentNullException("dbConnectionFactory");
			
			this._dbConnectionFactory = dbConnectionFactory;
		}
		
		protected IDbConnectionFactory DbConnectionFactory{get{return _dbConnectionFactory;}}
	}
}

