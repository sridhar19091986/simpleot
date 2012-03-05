using SimpleOT.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SimpleOT.Tests
{

    [TestClass()]
    public class MessageTest
    {
        [TestMethod]
        public void TestMessagePutGet()
        {
            Message message = new Message();
            Assert.AreEqual(0, message.ReadableBytes);

            message.PutByte(byte.MaxValue);
            message.PutByte(byte.MinValue); 

            message.PutUShort(ushort.MaxValue);
            message.PutUShort(ushort.MinValue);
            message.PutShort(short.MaxValue);
            message.PutShort(short.MinValue);

            message.PutUInt(uint.MaxValue);
            message.PutUInt(uint.MinValue);
            message.PutInt(int.MaxValue);
            message.PutInt(int.MinValue);

            message.PutULong(ulong.MaxValue);
            message.PutULong(ulong.MinValue);
            message.PutLong(long.MaxValue);
            message.PutLong(long.MinValue);

            message.PutString(string.Empty);
            message.PutString("123");

            Assert.AreEqual(65, message.ReadableBytes);

            Assert.AreEqual(byte.MaxValue, message.GetByte());
            Assert.AreEqual(byte.MinValue, message.GetByte());

            Assert.AreEqual(ushort.MaxValue, message.GetUShort());
            Assert.AreEqual(ushort.MinValue, message.GetUShort());
            Assert.AreEqual(short.MaxValue, message.GetShort());
            Assert.AreEqual(short.MinValue, message.GetShort());

            Assert.AreEqual(uint.MaxValue, message.GetUInt());
            Assert.AreEqual(uint.MinValue, message.GetUInt());
            Assert.AreEqual(int.MaxValue, message.GetInt());
            Assert.AreEqual(int.MinValue, message.GetInt());

            Assert.AreEqual(ulong.MaxValue, message.GetULong());
            Assert.AreEqual(ulong.MinValue, message.GetULong());
            Assert.AreEqual(long.MaxValue, message.GetLong());
            Assert.AreEqual(long.MinValue, message.GetLong());

            Assert.AreEqual(string.Empty, message.GetString());
            Assert.AreEqual("123", message.GetString());

            Assert.AreEqual(0, message.ReadableBytes);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "You can't modify this message because it's read only.")]
        public void TestMessageReadOnly()
        {
            Message message = new Message(true);

            Assert.IsTrue(message.IsReadyOnly);
            message.PutByte(byte.MaxValue);
        }

        [TestMethod]
        public void TestMessagePutHeader()
        {
            Message message = new Message();

            message.PutLong(long.MinValue);

            Assert.AreEqual(8, message.ReadableBytes);
            Assert.AreEqual(message.ReaderIndex + 8, message.WriterIndex);

            var oldReaderIndex = message.ReaderIndex;
            var oldWriterIndex = message.WriterIndex;

            message.PutHeader((ushort)message.ReadableBytes);

            Assert.AreEqual(10, message.ReadableBytes);
            Assert.AreEqual(oldWriterIndex, message.WriterIndex);
            Assert.AreEqual(oldReaderIndex - 2, message.ReaderIndex);

            oldReaderIndex = message.ReaderIndex;
            oldWriterIndex = message.WriterIndex;

            message.PutHeader((uint)message.ReadableBytes);

            Assert.AreEqual(14, message.ReadableBytes);
            Assert.AreEqual(oldWriterIndex, message.WriterIndex);
            Assert.AreEqual(oldReaderIndex - 4, message.ReaderIndex);
        }
    }
}
