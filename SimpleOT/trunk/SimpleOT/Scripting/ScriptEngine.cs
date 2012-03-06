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
            var luaContext = context as ScriptContext;

            if (luaContext == null || luaContext.State == IntPtr.Zero)
                throw new Exception("Invalid script context");

            if (Lua.DoFile(luaContext.State, fileName) != LuaError.None)
                throw new Exception(Lua.PopString(luaContext.State));
        }



        //public void LoadEvent(IScriptEvent scriptEvent, IScriptContext context)
        //{
        //    throw new NotImplementedException();
        //}
    }
}

