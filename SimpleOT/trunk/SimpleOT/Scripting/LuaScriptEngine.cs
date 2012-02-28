using System;

namespace SimpleOT.Commons.Scripting
{
	public class LuaScriptEngine : IScriptEngine
	{
		public LuaScriptEngine ()
		{
		}

		#region IScriptEngine implementation
		public IScriptContext CreateContext ()
		{
			IntPtr state = Lua.Open();
			
			if(state == IntPtr.Zero)
				throw new ScriptException("Unable to open a new lua state");
			
			Lua.OpenBase(state);
			Lua.OpenTable(state);
			Lua.OpenOS(state);
			Lua.OpenString(state);
			Lua.OpenMath(state);
			Lua.OpenDebug(state);
			
			return new LuaScriptContext(state);
		}

		public void LoadFile (string fileName, IScriptContext context)
		{
			var luaContext = context as LuaScriptContext;
			
			if(luaContext == null || luaContext.State == IntPtr.Zero)
				throw new ScriptException("Invalid script context");
			
			
		}

		public void LoadEvent (IScriptEvent scriptEvent, IScriptContext context)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

