using System;
using NUnit.Framework;
using SimpleOT.Commons.Net;

namespace SimpleOT.Test.Net
{
	[TestFixture()]
	public class XteaTest
	{
		private uint[] _key;
		
		[SetUp]
		public void SetUp ()
		{
			Random random = new Random ();
			_key = new uint[4];
			_key [0] = (uint)random.Next (int.MaxValue);
			_key [1] = (uint)random.Next (int.MaxValue);
			_key [2] = (uint)random.Next (int.MaxValue);
			_key [3] = (uint)random.Next (int.MaxValue);
		}
		
		[Test]
		public void TestEncryptDecrypt ()
		{
			Message message = new Message ();
			message.PutUInt (uint.MaxValue);
			
			Xtea.Encrypt (message, _key, 0);
			
			Assert.AreEqual (8, message.ReadableBytes);
			Assert.AreNotEqual (uint.MaxValue, message.GetUInt ());
			
			message.ReaderIndex = 0;
			Xtea.Decrypt (message, _key, 0);
			
			Assert.AreEqual (uint.MaxValue, message.GetUInt ());
		}
		
		[Test]
		public void TestEncryptDecryptOffset ()
		{
			Message message = new Message ();
			
			message.PutInt (int.MaxValue);
			message.PutUInt (uint.MaxValue);
			
			Xtea.Encrypt (message, _key, 4);
			
			Assert.AreEqual (12, message.ReadableBytes);
			
			Assert.AreEqual (int.MaxValue, message.GetInt ());
			Assert.AreNotEqual (uint.MaxValue, message.GetUInt ());
			
			message.ReaderIndex = 0;
			Xtea.Decrypt (message, _key, 4);
			
			Assert.AreEqual (int.MaxValue, message.GetInt ());
			Assert.AreEqual (uint.MaxValue, message.GetUInt ());
		}
		
	}
}

