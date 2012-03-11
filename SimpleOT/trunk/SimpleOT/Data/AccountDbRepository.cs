using System;
using System.Data;
using NLog;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
	public class AccountDbRepository : DbRepository, IAccountRepository
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		public AccountDbRepository (IDbConnectionFactory connectionFactory) : base(connectionFactory)
		{
		}

        public void Save(Account account)
        {
            try
            {
                using (var connection = DbConnectionFactory.Get())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE accounts SET premend = @premmiumEnd, warnings = @warnings WHERE id = @accountId";
                        command.CommandType = CommandType.Text;

                        var premmiumEnd = command.CreateParameter();
                        premmiumEnd.ParameterName = "@premmiumEnd";
                        premmiumEnd.Value = account.PremmiumEnd;
                        premmiumEnd.DbType = DbType.Int64;
                        command.Parameters.Add(premmiumEnd);

                        var warnings = command.CreateParameter();
                        warnings.ParameterName = "@warnings";
                        warnings.Value = account.Warnings;
                        warnings.DbType = DbType.Int16;
                        command.Parameters.Add(warnings);

                        var accountId = command.CreateParameter();
                        accountId.ParameterName = "@accountId";
                        accountId.Value = account.Id;
                        accountId.DbType = DbType.Int32;
                        command.Parameters.Add(accountId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorException("Can't save account.", exception);
            }
        }

        public Account Load(string name)
        {
            Account account = null;

            try
            {
                using (var connection = DbConnectionFactory.Get())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT id, name, password, premend, warnings FROM accounts WHERE name = @accountName";
                        command.CommandType = CommandType.Text;

                        IDbDataParameter accountName = command.CreateParameter();
                        accountName.ParameterName = "@accountName";
                        accountName.Value = name;
                        accountName.DbType = DbType.String;
                        command.Parameters.Add(accountName);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                account = new Account();

                                account.Id = (int)reader["id"];
                                account.Name = (string)reader["name"];
                                account.Password = (string)reader["password"];
                                account.PremmiumEnd = (long)reader["premend"];
                                account.Warnings = (short)reader["warnings"];

                                LoadCharacters(account);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorException("Can't load account.", exception);
                return null;
            }

            return account;
        }

        private void LoadCharacters(Account account)
        {
            try
            {
                using (var connection = DbConnectionFactory.Get())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT name FROM players WHERE account_id = @accountId";
                        command.CommandType = CommandType.Text;

                        IDbDataParameter loginParam = command.CreateParameter();
                        loginParam.ParameterName = "@accountId";
                        loginParam.Value = account.Id;
                        loginParam.DbType = DbType.Int32;
                        command.Parameters.Add(loginParam);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                                account.Characters.Add((string)reader["name"]);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                logger.ErrorException("Can't load account players.", exception);
            }
        }
	}
}

