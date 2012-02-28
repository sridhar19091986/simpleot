using System;
using SimpleOT.Commons.Threading;
using NUnit.Framework;
using System.Threading;

namespace SimpleOT.Test.Concurrent
{
	[TestFixture]
	public class DispatcherTest
	{
		private Dispatcher _dispatcher;
		
		[SetUp]
		public void SetUp ()
		{
			_dispatcher = new Dispatcher ();
			_dispatcher.Start ();
		}
		
		[TearDown]
		public void TearDown ()
		{
			_dispatcher.Stop ();
		}
		
		[Test]
		public void TestTaskDispatcher ()
		{
			_dispatcher.AfterDispatchTask += delegate() {
				lock (this)
					Monitor.Pulse (this);
			};
			
			bool executed = false;
			
			lock (this) {				
				_dispatcher.Add (new Task (() => {
					executed = true; }));
				Monitor.Wait (this, 500);
			}
			
			Assert.IsTrue (executed);
		}
		
		[Test]
		public void TestTaskExpiration ()
		{
			_dispatcher.AfterDispatchTask += delegate() {
				lock (this)
					Monitor.Pulse (this);
			};
			
			bool executed = false;
			
			var task = new Task (() => {
				executed = true; }, 50);
			
			Thread.Sleep (100);
			
			lock (this) {				
				_dispatcher.Add (task);
				Monitor.Wait (this, 500);
			}
			
			Assert.IsFalse (executed);
		}
	}
}

