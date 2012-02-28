using System;

namespace SimpleOT.Scripting
{
	public interface IScriptEngine
	{
		IScriptContext CreateContext();
		
		void LoadFile(string fileName, IScriptContext context);
		void LoadEvent(IScriptEvent scriptEvent, IScriptContext context);
	}
}

