using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Net;

namespace SimpleOT.Domain
{
    public class Player : Creature
    {
		public GameProtocol Protocol{get;set;}
        public Account Account { get; set; }

        public string Name { get; set; }
    }
}
