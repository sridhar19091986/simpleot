using System;
using NUnit.Framework;
using SimpleOT.Commons.Threading;

namespace SimpleOT.Test.Threading
{
	[TestFixture]
	public class TaskTest
	{
		[Test()]
		[ExpectedException(typeof(InvalidOperationException))]
		public void TestTaskCreation ()
		{
			var task = new Task (() => {
				throw new InvalidOperationException (); });
			task.Execute ();
		}
	}
}

