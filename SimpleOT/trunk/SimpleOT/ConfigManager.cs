using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Scripting;

namespace SimpleOT
{
    public class ConfigManager
    {
        private readonly Server _server;
        private readonly ScriptContext _scriptContext;

        public string DataDirectory { get; private set; }
        public string MapLocation { get; private set; }

        public string ServerName { get; private set; }
        public string ServerLocation { get; private set; }
        public string ServerIp { get; private set; }

        public int LoginPort { get; private set; }
        public int GamePort { get; private set; }

        public ConfigManager(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
            _scriptContext = new ScriptContext(_server);

            _scriptContext.ExecuteFile("config.lua");

            DataDirectory = GetString("datadir", "data/");
            MapLocation = GetString("map", "data/world/map.otbm");

            ServerName = GetString("servername", "SimpleOT");
            ServerLocation = GetString("location", "Brasil");
            ServerIp = GetString("ip", "127.0.0.1");

            LoginPort = GetInteger("login_port", 7171);
            GamePort = GetInteger("game_port", 7172);
        }

        private int GetInteger(string name, int defaultValue)
        {
            var ret = _scriptContext.GetGlobalInteger(name);

            if (ret == null)
                return defaultValue;

            return ret.Value;
        }

        public string GetString(string name, string defaultValue)
        {
            return _scriptContext.GetGlobalString(name) ?? defaultValue;
        }
    }
}
