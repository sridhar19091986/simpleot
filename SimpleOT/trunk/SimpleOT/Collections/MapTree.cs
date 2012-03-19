using System;
using SimpleOT.Domain;

namespace SimpleOT.Collections
{
    public class MapTree
    {
        private readonly MapTreeNode _root;

        public MapTree()
        {
            _root = new MapTreeNode();
        }

        public MapTreeLeafNode Get(int x, int y)
        {
            MapTreeNode currentNode = _root;

            while (currentNode != null)
            {
                if (!(currentNode is MapTreeLeafNode))
                {
                    int index = ((x & 0x8000) >> 15) | ((y & 0x8000) >> 14);

                    if (currentNode.Children[index] != null)
                    {
                        currentNode = currentNode.Children[index];

                        x = x * 2;
                        y = y * 2;

                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return (MapTreeLeafNode)currentNode;
                }
            }

            return null;
        }

        public MapTreeLeafNode Create(int x, int y)
        {
            return Create(x, y, 15);
        }

        private MapTreeLeafNode Create(int x, int y, int level)
        {
            var newLeaf = false;

            var currentNode = _root;
            var currentX = x;
            var currentY = y;

            while (!(currentNode is MapTreeLeafNode))
            {
                int index = ((currentX & 0x8000) >> 15) | ((currentY & 0x8000) >> 14);

                if (currentNode.Children[index] == null)
                {
                    if (level != Constants.FloorBits)
                    {
                        currentNode.Children[index] = new MapTreeNode();
                    }
                    else
                    {
                        currentNode.Children[index] = new MapTreeLeafNode();
                        newLeaf = true;
                    }

                    currentNode = currentNode.Children[index];

                    --level;
                    currentX *= 2;
                    currentX *= 2;
                }
            }

            if (newLeaf)
            {
                //update north
                var northLeaf = Get(x, y - Constants.FloorSize);
                if (northLeaf != null)
                    northLeaf.SouthNode = currentNode;

                //update west leaf
                var westLeaf = Get(x - Constants.FloorSize, y);
                if (westLeaf != null)
                    westLeaf.EastNode = currentNode;

                //update south
                var southLeaf = Get(x, y + Constants.FloorSize);
                if (southLeaf != null)
                    currentNode.SouthNode = southLeaf;

                //update east
                var eastLeaf = Get(x + Constants.FloorSize, y);
                if (eastLeaf != null)
                    currentNode.EastNode = eastLeaf;
            }

            return (MapTreeLeafNode)currentNode;
        }
    }

    public class MapTreeNode
    {
        protected readonly MapTreeNode[] _children;
        private MapTreeNode _eastNode;
        private MapTreeNode _southNode;

        public MapTreeNode()
        {
            _children = new MapTreeNode[4];
        }

        public MapTreeNode[] Children { get { return _children; } }

        public MapTreeNode EastNode { get { return _eastNode; } set { _eastNode = value; } }

        public MapTreeNode SouthNode { get { return _southNode; } set { _southNode = value; } }
    }

    public class MapTreeLeafNode : MapTreeNode
    {
        private readonly Floor[] _floors;

        public MapTreeLeafNode()
        {
            _floors = new Floor[Constants.MapMaxLayers];
        }

        public Floor CreateFloor(int z)
        {
            return _floors[z] ?? (_floors[z] = new Floor());
        }

        public Floor GetFloor(int z)
        {
            return _floors[z];
        }
    }
}

