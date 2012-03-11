using System;
using System.Data;

namespace SimpleOT.Data
{
	public interface IDbConnectionFactory
	{
		IDbConnection Get();
		void Put(IDbConnection connection);
	}
}

