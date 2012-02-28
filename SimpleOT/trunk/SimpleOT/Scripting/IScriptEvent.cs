using System;

namespace SimpleOT.Commons.Scripting
{
	public interface IScriptEvent
	{
		int Id{get;set;}
		string FileName{get;}
		string FunctionName {get;}
	}
}

