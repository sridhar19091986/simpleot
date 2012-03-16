using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Domain
{
    public class World
    {
        private readonly Server _server;
        private readonly Map _map;

        public World(Server server)
        {
            if (server == null)
                throw new ArgumentNullException("server");

            _server = server;
            _map = new Map(this);
        }

    }
}
