using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleOT.Data;

namespace SimpleOT.Tests
{
    [TestClass()]
    public class AccountRepositoryTest
    {
        private AccountRepository _accountRepository;

        [TestInitialize]
        public void Initialize()
        {
            _accountRepository = new AccountRepository(new PostgresConnectionFactory());
        }

        [TestMethod]
        public void TestLoadAccount()
        {
            var account = _accountRepository.Load("tibia");

            Assert.IsNotNull(account);
            Assert.AreEqual("tibia", account.Name);
            Assert.AreEqual("tibia", account.Password);
            Assert.AreEqual(0, account.PremmiumEnd);
            Assert.AreEqual(0, account.Warnings);

            Assert.AreEqual(2, account.Characters.Count);
            Assert.IsTrue(account.Characters.Contains("Administrator"));
            Assert.IsTrue(account.Characters.Contains("Player"));
        }

    }
}
