using System;
using SimpleOT.Collections;

namespace SimpleOT.Domain
{
	public class Tile
	{
		private MapTreeNode _node;
		
		public Tile ()
		{
		}
		
		public MapTreeNode Node{get{return _node;}set{_node = value;}}
	}
}

