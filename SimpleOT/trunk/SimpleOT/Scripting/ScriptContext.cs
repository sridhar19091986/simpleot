using System;

namespace SimpleOT.Scripting
{
	public class ScriptContext
	{
		private IntPtr _state;
		
		public ScriptContext (ScriptEngine scriptEngine, IntPtr state)
		{
			this._state = state;
		}

        public IntPtr State { get { return _state; } }
	}
}

