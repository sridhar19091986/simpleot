using System;
using SimpleOT.Collections;
using System.Collections.Generic;
using SimpleOT.IO;

namespace SimpleOT.Domain
{
    public class Map
    {
        private readonly World _world;
        private readonly MapTree _mapTree;
        private readonly IList<Town> _towns;

        public Map(World world)
        {
            if (world == null)
                throw new ArgumentNullException("world");

            _world = world;
            _mapTree = new MapTree();
            _towns = new List<Town>();
        }

        public void AddTile(Position position, Tile tile)
        {
            if (!position.IsValid)
            {
                Logger.Error(string.Format("Attempt to set tile on invalid coordinate {0}.", position));
                return;
            }

            var node = _mapTree.Create(position.X, position.Y);
            var floor = node.CreateFloor(position.Z);

            if (floor.HasTile(position.X, position.Y))
            {
                Logger.Error(string.Format("Map already have a tile in coordinate {0}.", position));
            }
            else
            {
                floor.SetTile(position.X, position.Y, tile);
                tile.Node = node;
            }
        }

        public Tile GetTile(Position position)
        {
            if (!position.IsValid)
            {
                Logger.Error(string.Format("Attempt to get tile on invalid coordinate {0}.", position));
                return null;
            }

            var node = _mapTree.Get(position.X, position.Y);

            if (node != null)
            {
                var floor = node.GetFloor(position.Z);

                if (floor != null)
                    return floor.GetTile(position.X, position.Y);
            }

            return null;
        }

        public void AddTown(Town town)
        {
            _towns.Add(town);
        }

        public World World { get { return _world; } }
    }
}

