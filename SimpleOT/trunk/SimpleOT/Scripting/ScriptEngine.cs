using System;

namespace SimpleOT.Scripting
{
    public class ScriptEngine
    {
        public ScriptEngine()
        {
        }

        public ScriptContext CreateContext()
        {
            IntPtr state = Lua.Open();

            if (state == IntPtr.Zero)
                throw new Exception("Unable to open a new lua state");

            Lua.OpenBase(state);
            Lua.OpenTable(state);
            Lua.OpenOS(state);
            Lua.OpenString(state);
            Lua.OpenMath(state);
            Lua.OpenDebug(state);

            return new ScriptContext(this, state);
        }

        public void LoadFile(string fileName, ScriptContext context)
        {
            if (context == null)
                throw new Exception("Invalid script context");

            if (Lua.DoFile(context.State, fileName) != LuaError.None)
                throw new Exception(Lua.PopString(context.State));
        }

        public string GetErrorMessage(ScriptContext context)
        {
            if (context == null)
                throw new Exception("Invalid script context");

            return Lua.PopString(context.State);
        }

        public string GetGlobalString(ScriptContext context, string name)
        {
            if (context == null)
                throw new Exception("Invalid script context");

            return Lua.GetGlobalString(context.State, name);
        }

        public long GetGlobalLong(ScriptContext context, string name)
        {
            if (context == null)
                throw new Exception("Invalid script context");

            return Lua.GetGlobalLong(context.State, name);
        }

        public int GetGlobalInteger(ScriptContext context, string name)
        {
            if (context == null)
                throw new Exception("Invalid script context");

            return Lua.GetGlobalInteger(context.State, name);
        }


        //public void LoadEvent(IScriptEvent scriptEvent, IScriptContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

