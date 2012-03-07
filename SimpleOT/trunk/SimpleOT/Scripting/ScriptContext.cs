using System;

namespace SimpleOT.Scripting
{
	public class ScriptContext
	{
		private IntPtr _state;
        private readonly Server _server;
		
		public ScriptContext (Server server)
		{
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
			_state = Lua.Open();

            if (_state == IntPtr.Zero)
                throw new Exception("Unable to open a new lua state");

            Lua.OpenBase(_state);
            Lua.OpenTable(_state);
            Lua.OpenOS(_state);
            Lua.OpenString(_state);
            Lua.OpenMath(_state);
            Lua.OpenDebug(_state);
		}

        public void ExecuteFile(string fileName)
        {
            if (Lua.DoFile(_state, fileName) != LuaError.None)
                throw new Exception(Lua.PopString(_state));
        }

        public void ExecuteString(string value)
        {
            if(Lua.DoString(_state, value) != LuaError.None)
                throw new Exception(Lua.PopString(_state));
        }

        public string GetErrorMessage()
        {
            return Lua.PopString(_state);
        }

        public string GetGlobalString(string name)
        {
            return Lua.GetGlobalString(_state, name);
        }

        public long? GetGlobalLong(string name)
        {
            return Lua.GetGlobalLong(_state, name);
        }

        public int? GetGlobalInteger(string name)
        {
            return Lua.GetGlobalInteger(_state, name);
        }
	}
}

