using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Scripting;

namespace SimpleOT.Data
{
    public class ConfigManager
    {
        private readonly Server _server;
        private readonly ScriptContext _scriptContext;

        public int LoginPort { get; private set; }
        public int GamePort { get; private set; }

        public ConfigManager(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
            _scriptContext = _server.ScriptEngine.CreateContext();

            _server.ScriptEngine.LoadFile("config.lua", _scriptContext);

            LoginPort = _server.ScriptEngine.GetGlobalInteger(_scriptContext, "login_port");
            GamePort = _server.ScriptEngine.GetGlobalInteger(_scriptContext, "game_port");

        }
    }
}
