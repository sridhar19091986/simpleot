using System;
using SimpleOT.Data;
using System.Data;
using NLog;

namespace SimpleOT.LoginServer
{
	public class AccountRepository : Repository
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		public AccountRepository (IConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}
		
		public Account FindByLogin(string login)
		{
			Account account = null;
			IDbConnection connection = null;
			IDbCommand command = null;
			IDataReader reader = null;
			
			try{
				connection = ConnectionFactory.Get();
				
				command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM account WHERE login=@login";
				command.CommandType = CommandType.Text;
				
				IDbDataParameter loginParam = command.CreateParameter();
				loginParam.ParameterName = "@login"; 
				loginParam.Value = login;
				loginParam.DbType = DbType.String; 
				command.Parameters.Add(loginParam); 
				
				reader = command.ExecuteReader();
				
				if(reader.Read())
				{
					account = new Account();
					
					account.Id = (int)reader["id"];
					account.Login = (string)reader["login"];
					account.Password = (string)reader["password"];
					account.Email = (string)reader["email"];
				}
				
			} catch(Exception exception) {
				
				logger.ErrorException("Can't execute query.", exception);
				return null;
				
			} finally {
				
				if(reader != null)
					reader.Close();
				if(command != null)
					reader.Close();
				if(connection != null)
					ConnectionFactory.Put (connection);
			}
			
			if(account != null)
				LoadCharacters(account);
			
			return account;
		}
		
		private void LoadCharacters(Account account)
		{
			IDbConnection connection = null;
			IDbCommand command = null;
			IDataReader reader = null;
			
			try{
				connection = ConnectionFactory.Get();
				
				command = connection.CreateCommand();
				command.CommandText = "SELECT * FROM character WHERE account_id=@accountId";
				command.CommandType = CommandType.Text;
				
				IDbDataParameter loginParam = command.CreateParameter();
				loginParam.ParameterName = "@accountId"; 
				loginParam.Value = account.Id;
				loginParam.DbType = DbType.Int32; 
				command.Parameters.Add(loginParam); 
				
				reader = command.ExecuteReader();
				
				while(reader.Read())
				{
					Character character = new Character();
					character.Id = (int)reader["id"];
					character.Name = (string)reader["name"];
					
					account.Characters.Add(character);
				}
				
			} catch(Exception exception) {
				logger.ErrorException("Can't execute query.", exception);
			} finally {
				
				if(reader != null)
					reader.Close();
				if(command != null)
					reader.Close();
				if(connection != null)
					ConnectionFactory.Put (connection);
			}
			
		}
	}
}

