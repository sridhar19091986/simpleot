using System;

namespace SimpleOT.Commons.Threading
{
	public class TaskEventArgs : EventArgs
	{
        public Task Task { get; private set; }

        public TaskEventArgs(Task task)
        {
            this.Task = task;
        }
	}
	
	public class AfterDispatchTaskEventArgs : TaskEventArgs
    {
		public AfterDispatchTaskEventArgs(Task task) : base(task) { }
    }

    public class BeforeDispatchTaskEventArgs : TaskEventArgs
    {
		public BeforeDispatchTaskEventArgs(Task task) : base(task) { }
    }
}

