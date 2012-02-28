using System;

namespace SimpleOT.Commons.Scripting
{
	public interface IScriptEngine
	{
		IScriptContext CreateContext();
		
		void LoadFile(string fileName, IScriptContext context);
		void LoadEvent(IScriptEvent scriptEvent, IScriptContext context);
	}
}

