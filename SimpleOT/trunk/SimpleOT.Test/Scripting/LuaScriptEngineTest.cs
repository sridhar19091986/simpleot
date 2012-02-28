using System;
using NUnit.Framework;
using SimpleOT.Commons.Scripting;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SimpleOT.Test.Scripting
{
	[TestFixture()]
	public class LuaScriptEngineTest
	{
		[Test]
		public void TestCreateContext()
		{
			IScriptEngine engine = new LuaScriptEngine();
			
			var wd = Process.GetCurrentProcess().StartInfo.WorkingDirectory;
			
			var context = engine.CreateContext();
			
			//engine.LoadFile("test.lua", context);
		}
		
	}
}

