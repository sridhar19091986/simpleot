using System;
using System.Data;

namespace SimpleOT.Commons.Data
{
	public interface IConnectionFactory
	{
		IDbConnection Get();
		void Put(IDbConnection connection);
	}
}

