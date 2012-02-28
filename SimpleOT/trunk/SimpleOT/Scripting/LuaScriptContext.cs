using System;

namespace SimpleOT.Scripting
{
	public class LuaScriptContext : IScriptContext
	{
		private IntPtr _state;
		
		public LuaScriptContext (IntPtr state)
		{
			this._state = state;
		}
		
		public IntPtr State{get{return _state;}}
	}
}

