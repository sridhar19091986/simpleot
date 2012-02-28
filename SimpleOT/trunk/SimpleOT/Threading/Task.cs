using System;

namespace  SimpleOT.Threading
{
    public class Task
    {
		private Action _action;
        private long _expiration;

        public Task(Action action, int expiration)
        {
            if(expiration > 0)
                _expiration = DateTime.Now.AddMilliseconds(expiration).Ticks;
			
			this._action = action;
        }
		
		public Task(Action action) : this(action, 0)
		{
		}
		
        public bool HasExpired {get{return _expiration > 0 && _expiration <= DateTime.Now.Ticks;}}
		
		public Action Action{get{return _action;}}
		
		public void Execute ()
		{
			_action();
		}
    }
}
