using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Domain
{
    public class Player : Creature
    {
        public Account Account { get; set; }

        public string Name { get; set; }
    }
}
