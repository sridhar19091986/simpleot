using System;
using SimpleOT.Collections;

namespace SimpleOT.Domain
{
    public class Tile
    {
        private MapTreeNode _node;
        private ItemList _items;

        public Tile()
        {
        }

        public MapTreeNode Node { get { return _node; } set { _node = value; } }
    }
}

