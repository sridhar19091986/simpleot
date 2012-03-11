using System;
using SimpleOT.Domain;

namespace SimpleOT.Collections
{
	public class MapTree
	{
		private readonly MapTreeNode _root;
		
		public MapTree ()
		{
			_root = new MapTreeNode (false);
		}
		
		public MapTreeNode Get (int x, int y)
		{
			MapTreeNode currentNode = _root;
			
			while (currentNode != null) 
			{
				if (!currentNode.IsLeaf) 
				{	
					int index = ((x & 0x8000) >> 15) | ((y & 0x8000) >> 14);
					
					if (currentNode.Children [index] != null) 
					{
						currentNode = currentNode.Children [index];
						
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
					return currentNode;
				}
			}

			return null;
		}
		
		public MapTreeNode Create(int x, int y)
		{
			return Create (x,y, 15);
		}
		
		private MapTreeNode Create (int x, int y, int level)
		{
			var newLeaf = false;
			
			var currentNode = _root;
			var currentX = x;
			var currentY = y;
			
			while(!currentNode.IsLeaf)
			{
				int index = ((currentX & 0x8000) >> 15) | ((currentY & 0x8000) >> 14);
				
				if(currentNode.Children[index] == null) 
				{
					if(level != Constants.FloorBits) 
					{
						currentNode.Children[index] = new MapTreeNode(false);
					}
					else 
					{
						currentNode.Children[index] = new MapTreeNode(true);
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
			
			return currentNode;
		}
	}
	
	public class MapTreeNode
	{
		protected readonly MapTreeNode[] _children;
		private MapTreeNode _eastNode;
		private MapTreeNode _southNode;
		private readonly bool _leaf;
		private readonly Floor[]  _floors;

		public MapTreeNode (bool leaf)
		{
			_leaf = leaf;
			
			_children = new MapTreeNode[4];
			
			if(_leaf)
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
			
		public bool IsLeaf { get { return _leaf; } }
		
		public MapTreeNode[] Children{get{return _children;}}
		
		public MapTreeNode EastNode{ get { return _eastNode; } set { _eastNode = value; } }

		public MapTreeNode SouthNode{ get { return _southNode; } set { _southNode = value; } }
	}
}

