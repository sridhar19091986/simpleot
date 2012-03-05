using SimpleOT.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SimpleOT.Tests
{
    [TestClass()]
    public class XteaTest
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
        public void TestXteaEncryptDecrypt()
        {
            Message message = new Message();

            message.PutUInt(uint.MaxValue);
            message.PutShort(short.MinValue);

            Xtea.Encrypt(message, _key);

            Assert.AreEqual(8, message.ReadableBytes);

            message.MarkReaderIndex();

            Assert.AreNotEqual(uint.MaxValue, message.GetUInt());
            Assert.AreNotEqual(short.MinValue, message.GetShort());

            message.ResetReaderIndex();

            Xtea.Decrypt(message, _key);

            Assert.AreEqual(uint.MaxValue, message.GetUInt());
            Assert.AreEqual(short.MinValue, message.GetShort());
        }
    }
}
