using System;

namespace SimpleOT.Domain
{
	public class Floor
	{
		private readonly Tile[,] _tiles;
		
		public Floor ()
		{
			_tiles = new Tile[Constants.FloorSize, Constants.FloorSize];
		}
		
		public Tile GetTile(int x, int y)
		{
			return _tiles[x & Constants.FloorMask, y & Constants.FloorMask];
		}
		
		public bool HasTile(int x, int y)
		{
			return _tiles[x & Constants.FloorMask, y & Constants.FloorMask] != null;
		}
		
		public void SetTile(int x, int y, Tile tile)
		{
			_tiles[x & Constants.FloorMask, y & Constants.FloorMask] = tile;
		}
	}
}

