using System;
using NUnit.Framework;
using SimpleOT.Commons.Net;

namespace SimpleOT.Test.Net
{
	[TestFixture]
	public class MessageTest
	{
		[Test]
		public void TestPutAndGet()
		{
			Message message = new Message();
			
			message.PutByte(byte.MaxValue); //1
			message.PutBool(true); //1
			message.PutUShort(ushort.MaxValue); //2
			message.PutShort(short.MaxValue); //2
			message.PutUInt(uint.MaxValue); //4
			message.PutInt(int.MaxValue); //4
			message.PutULong(ulong.MaxValue); //8
			message.PutLong(long.MaxValue); //8
			message.PutString(String.Empty); //2
			message.PutString("123"); //5
			
			Assert.AreEqual(37, message.ReadableBytes);
			
			Assert.AreEqual(byte.MaxValue, message.GetByte());
			Assert.AreEqual(true, message.GetBool());
			Assert.AreEqual(ushort.MaxValue, message.GetUShort());
			Assert.AreEqual(short.MaxValue, message.GetShort());
			Assert.AreEqual(uint.MaxValue, message.GetUInt());
			Assert.AreEqual(int.MaxValue, message.GetInt());
			Assert.AreEqual(ulong.MaxValue, message.GetULong());
			Assert.AreEqual(long.MaxValue, message.GetLong());
			Assert.AreEqual(string.Empty, message.GetString());
			Assert.AreEqual("123", message.GetString());
			
			Assert.AreEqual(0, message.ReadableBytes);
		}
		
		[Test]
		public void TestClear()
		{
			Message message = new Message();
			message.PutString("123");
			
			Assert.AreEqual(5, message.ReadableBytes);
			Assert.AreEqual(message.Capacity - message.WriterIndex, message.WritableBytes);
			
			message.Clear();
			
			Assert.AreEqual(0, message.ReadableBytes);
			Assert.AreEqual(message.Capacity, message.WritableBytes);
		}
		
		[Test]
		public void TestMark()
		{
			Message message = new Message();
			
			message.PutBool(true);
			message.PutString("123");
			Assert.AreEqual(6, message.ReadableBytes);
			
			message.MarkWriterIndex();
			
			message.PutString("123456");
			Assert.AreEqual(14, message.ReadableBytes);
			
			message.ResetWriterIndex();
			Assert.AreEqual(6, message.ReadableBytes);
			
			Assert.IsTrue( message.GetBool());
			
			message.MarkReaderIndex();
			Assert.AreEqual("123", message.GetString ());
			
			message.ResetReaderIndex();
			Assert.AreEqual(5, message.ReadableBytes);
			
			Assert.AreEqual("123", message.GetString ());
		}
		
		
	}
}

