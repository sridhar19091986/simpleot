using SimpleOT.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SimpleOT.Tests
{
    [TestClass()]
    public class AdlerTest
    {
        private uint[] _key;

        [TestInitialize]
        public void Initialize()
        {
            Random random = new Random();

            _key = new uint[4];
            _key[0] = (uint)random.Next(0, int.MaxValue);
            _key[1] = (uint)random.Next(0, int.MaxValue);
            _key[2] = (uint)random.Next(0, int.MaxValue);
            _key[3] = (uint)random.Next(0, int.MaxValue);
        }

        [TestMethod]
        public void TestAdlerGenerate()
        {
            Message message = new Message();
            message.PutULong(ulong.MaxValue);

            var checksum = Adler.Generate(message);

            Xtea.Encrypt(message, _key);

            Assert.AreNotEqual(checksum, Adler.Generate(message));

            Xtea.Encrypt(message, _key);

            Assert.AreEqual(checksum, Adler.Generate(message));
        }
    }
}
