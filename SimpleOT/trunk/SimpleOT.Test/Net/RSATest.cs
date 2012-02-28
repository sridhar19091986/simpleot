using System;
using NUnit.Framework;
using SimpleOT.Commons.Net;

namespace SimpleOT.Test.Net
{
	[TestFixture]
	public class RSATest
	{
		[Test]
		public void TestEncryptDecrypt ()
		{
			Message message = new Message ();
			
			message.PutInt (int.MaxValue);
			message.PutULong (ulong.MaxValue);
			
			message.WriterIndex += 120;
			
			Rsa.Encrypt (message, 4, 128);
			
			Assert.AreEqual (int.MaxValue, message.GetInt ());
			Assert.AreNotEqual (ulong.MaxValue, message.GetULong ());
		}
		
		
		
	}
}

