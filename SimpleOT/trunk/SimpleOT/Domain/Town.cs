using System;

namespace SimpleOT.Domain
{
    public class Town
    {
        public uint Id { get; private set; }
        public string Name { get; private set; }
        public Position TemplePosition { get; private set; }

        public Town(uint id, string name, Position templeLocation)
        {
            Id = id;
            Name = name;
            TemplePosition = templeLocation;
        }
    }
}

