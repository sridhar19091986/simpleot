using System;
using Npgsql;
using System.Data;

namespace SimpleOT.Data
{
	public class PostgresDbConnectionFactory : IDbConnectionFactory
	{
		private string _connectionString;
		
		public PostgresDbConnectionFactory(string host, int port, string userId, string password, string database)
		{
			this._connectionString = string.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};",
          		host, port, userId, password, database);
		}
		
		public PostgresDbConnectionFactory () : this("127.0.0.1", 5432, "postgres", "postgres", "otserv") { }
		
		public IDbConnection Get ()
		{
			var connection = new NpgsqlConnection(_connectionString);
			connection.Open();
			return connection;
		}
		
		public void Put(IDbConnection connection)
		{
			if(connection != null)
				connection.Close();
		}
	}
}

