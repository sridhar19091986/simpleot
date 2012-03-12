using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleOT.Data;

namespace SimpleOT.Tests
{
    [TestClass()]
    public class PlayerRepositoryTest
    {
        private IPlayerRepository _playerRepository;

        [TestInitialize]
        public void Initialize()
        {
            _playerRepository = new PlayerDbRepository(new PostgresDbConnectionFactory());
        }

        [TestMethod]
        public void TestLoadPlayer()
        {
            var player = _playerRepository.Load("Administrator");

            Assert.IsNotNull(player);
            Assert.AreEqual("Administrator", player.Name);

            Assert.IsNotNull(player.Account);

            Assert.AreEqual(1, player.Account.Id);
            Assert.AreEqual("tibia", player.Account.Name);
        }

    }
}
