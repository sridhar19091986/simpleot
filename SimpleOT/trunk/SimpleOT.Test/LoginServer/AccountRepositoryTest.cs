using System;
using NUnit.Framework;
using SimpleOT.Commons.Data;
using SimpleOT.LoginServer;

namespace SimpleOT.Test.LoginServer
{
	[TestFixture]
	public class AccountRepositoryTest
	{
		private static IConnectionFactory _connectionFactory;
	
		private AccountRepository _accountRepository;
		
		[TestFixtureSetUp]
		public void Init()
		{
			_connectionFactory = new PostgresConnectionFactory("localhost", 5432, "simpleot", "secret", "simpleot");
		}
		
		[SetUp]
		public void SetUp()
		{
			_accountRepository = new AccountRepository(_connectionFactory);
		}
		
		[Test]
		public void TestFindByLogin()
		{
			Account account = _accountRepository.FindByLogin("login");
			
			Assert.IsNotNull(account);
			Assert.AreEqual(1, account.Id);
			Assert.AreEqual("login", account.Login);
			Assert.AreEqual("secret", account.Password);
			Assert.AreEqual(1, account.Characters.Count);
			
			Assert.AreEqual(1, account.Characters[0].Id);
			Assert.AreEqual("Character 1", account.Characters[0].Name);
			
			account = _accountRepository.FindByLogin("invalid");
			Assert.IsNull(account);
		}
	}
}

