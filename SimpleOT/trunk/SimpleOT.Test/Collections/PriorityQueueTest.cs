using System;
using NUnit.Framework;
using SimpleOT.Commons.Collections;
using SimpleOT.Commons.Threading;

namespace SimpleOT.Test.Collections
{
	[TestFixture()]
	public class PriorityQueueTest
	{		
		[Test()]
		public void TestTaskPriorityQueueSort ()
		{
			PriorityQueue<Schedule> queue = new PriorityQueue<Schedule> ();
			
			Assert.IsTrue (queue.IsEmpty);
			
			var schedule1 = new Schedule (10 * 1000, () => {});
			var schedule2 = new Schedule (3 * 1000, () => {});
			var schedule3 = new Schedule (5 * 1000, () => {});
			
			queue.Enqueue (schedule1);
			queue.Enqueue (schedule2);
			queue.Enqueue (schedule3);
			
			Assert.IsFalse (queue.IsEmpty);
			
			Assert.AreEqual (3, queue.Count);
			
			Assert.AreSame (schedule2, queue.Peek ());
			Assert.AreSame (schedule2, queue.Dequeue ());
			
			Assert.AreEqual (2, queue.Count);
			
			queue.Clear ();
			
			Assert.IsTrue (queue.IsEmpty);
		}
		
	}
}

