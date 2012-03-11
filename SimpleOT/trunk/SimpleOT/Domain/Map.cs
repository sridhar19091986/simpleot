using System;
using SimpleOT.Collections;
using NLog;
using System.Collections.Generic;

namespace SimpleOT.Domain
{
	public class Map
	{
		private readonly static Logger logger = LogManager.GetCurrentClassLogger();
		
		private Server _server;
		private readonly MapTree _mapTree;
		private readonly IList<Town> _towns;
		
		public Map (Server server)
		{
			if(server == null)
				throw new ArgumentNullException("server");
			
			_server = server;
			_mapTree = new MapTree();
			_towns = new List<Town>();
		}
		
		public void SetTile(Position position, Tile tile) 
		{
	      	if (!position.IsValid)
            {
                logger.Error("Attempt to set tile on invalid coordinate {0}.",  position);
                return;
            }
			
			var node = _mapTree.Create(position.X, position.Y);
			var floor = node.CreateFloor(position.Z);
			
			if(floor.HasTile(position.X, position.Y)) 
			{
				logger.Error("Map already have a tile in coordinate {0}.", position);
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
                logger.Error("Attempt to get tile on invalid coordinate {0}.",  position);
                return null;
            }
			
			var node = _mapTree.Get(position.X, position.Y);
			
			if(node != null) 
			{
				var floor = node.GetFloor(position.Z);
				
				if(floor != null)
					return floor.GetTile(position.X, position.Y);
			}
			
			return null;
		}
		
		public Server Server{get{return _server;}}
	}
}

