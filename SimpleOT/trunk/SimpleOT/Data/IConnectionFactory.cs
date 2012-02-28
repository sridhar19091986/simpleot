using System;
using System.Data;

namespace SimpleOT.Data
{
	public interface IConnectionFactory
	{
		IDbConnection Get();
		void Put(IDbConnection connection);
	}
}

