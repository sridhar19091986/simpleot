using System;
using SimpleOT.Collections;
using System.Collections.Generic;
using SimpleOT.IO;
using System.IO;

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
		
		#region Load
		
		public void Load(string fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception(string.Format("File not found {0}.", fileName));

            var loader = new FileLoader();
            loader.OpenFile(fileName);
            var node = loader.GetRootNode();

            PropertyReader props;

            if (!loader.GetProps(node, out props))
                throw new Exception("Could not read root property.");

            props.ReadByte(); // junk?

            var version = props.ReadUInt32();
            var width = props.ReadUInt16();
            var height = props.ReadUInt16();

            var majorVersionItems = props.ReadUInt32();
            var minorVersionItems = props.ReadUInt32();

            if (version <= 0)
            {
                //In otbm version 1 the count variable after splashes/fluidcontainers and stackables
                //are saved as attributes instead, this solves alot of problems with items
                //that is changed (stackable/charges/fluidcontainer/splash) during an update.
                throw new Exception("This map needs to be upgraded by using the latest map editor version to be able to load correctly.");
            }

            if (version > 2)
                throw new Exception("Unknown OTBM version detected, please update your server.");

            if (majorVersionItems < 3)
                throw new Exception("This map needs to be upgraded by using the latest map editor version to be able to load correctly.");

            //if (MajorVersionItems > ItemInfo.MajorVersion)
            //{
            //    Log.Error("The map was saved with a different items.otb version, an upgraded items.otb is required.");
            //    return false;
            //}

            if (minorVersionItems < (uint)ClientVersion.ClientVersion810)
                throw new Exception("This map needs to be updated.");

            //if (MinorVersionItems > ItemInfo.MinorVersion)
            //    Log.Warn("This map needs an updated items.otb.");
            //if (MinorVersionItems == (uint)ClientVersion.ClientVersion854Bad)
            //    Log.Warn("This map needs uses an incorrect version of items.otb.");


            Logger.Info(string.Format("Map size: {0}x{1}", width, height));

            node = node.Child;

            if ((OtbMapNodeType)node.Type != OtbMapNodeType.MapData)
                throw new Exception("Could not read data node.");

            if (!loader.GetProps(node, out props))
                throw new Exception("Could not read map data attributes.");

            while (props.PeekChar() != -1)
            {
                byte attribute = props.ReadByte();
                switch ((OtbmAttribute)attribute)
                {
                    case OtbmAttribute.Description:
                        var description = props.GetString();
                        Logger.Info(string.Format("Map Description: {0}", description));
                        break;
                    case OtbmAttribute.ExtSpawnFile:
                        var spawnFile = props.GetString();
                        break;
                    case OtbmAttribute.ExtHouseFile:
                        var houseFile = props.GetString();
                        break;
                    default:
                        throw new Exception("Unknown header node.");
                }
            }

            var nodeMapData = node.Child;

            while (nodeMapData != null)
            {
                switch ((OtbMapNodeType)nodeMapData.Type)
                {
                    case OtbMapNodeType.TileArea:
                        ParseTileArea(loader, nodeMapData);
                        break;
                    case OtbMapNodeType.Towns:
                        ParseTowns(loader, nodeMapData);
                        break;
                }
                nodeMapData = nodeMapData.Next;
            }
        }

        private void ParseTowns(FileLoader loader, FileLoaderNode otbNode)
        {
            var nodeTown = otbNode.Child;

            while (nodeTown != null)
            {
                PropertyReader props;
                if (!loader.GetProps(nodeTown, out props))
                    throw new Exception("Could not read town data.");

                uint townid = props.ReadUInt32();
                string townName = props.GetString();
                ushort townTempleX = props.ReadUInt16();
                ushort townTempleY = props.ReadUInt16();
                byte townTempleZ = props.ReadByte();

                var town = new Town(townid, townName, new Position(townTempleX, townTempleY, townTempleZ));
                AddTown(town);

                nodeTown = nodeTown.Next;
            }
        }

        private static void ParseTileArea(FileLoader loader, FileLoaderNode otbNode)
        {
            PropertyReader props;
            if (!loader.GetProps(otbNode, out props))
                throw new Exception("Invalid map node.");

            int baseX = props.ReadUInt16();
            int baseY = props.ReadUInt16();
            int baseZ = props.ReadByte();

            var nodeTile = otbNode.Child;

            while (nodeTile != null)
            {
                if (nodeTile.Type == (long)OtbMapNodeType.Tile ||
                    nodeTile.Type == (long)OtbMapNodeType.HouseTile)
                {
                    loader.GetProps(nodeTile, out props);

                    int tileX = baseX + props.ReadByte();
                    int tileY = baseY + props.ReadByte();
                    int tileZ = baseZ;


                    bool isHouseTile = false;
                    //House* house = NULL;
                    //Tile tile = null;
                    //Item groundItem = null;
                    //TileFlags tileflags = TileFlags.None;

                    //var tile = new Tile(tileX, tileY, tileZ);

                    // TODO: houses
                    if (nodeTile.Type == (long)OtbMapNodeType.HouseTile)
                    {
                        uint houseId = props.ReadUInt32();
                    }

                    while (props.PeekChar() != -1)
                    {
                        byte attribute = props.ReadByte();
                        switch ((OtbmAttribute)attribute)
                        {
                            case OtbmAttribute.TileFlags:
                                {
                                    var flags = /*(TileFlags)*/props.ReadUInt32();
                                    //if ((flags & TileFlags.ProtectionZone) == TileFlags.ProtectionZone)
                                    //    tileflags |= TileFlags.ProtectionZone;
                                    //else if ((flags & TileFlags.NoPvpZone) == TileFlags.NoPvpZone)
                                    //    tileflags |= TileFlags.NoPvpZone;
                                    //else if ((flags & TileFlags.PvpZone) == TileFlags.PvpZone)
                                    //    tileflags |= TileFlags.PvpZone;

                                    //if ((flags & TileFlags.NoLogout) == TileFlags.NoLogout)
                                    //    tileflags |= TileFlags.NoLogout;

                                    //if ((flags & TileFlags.Refresh) == TileFlags.Refresh)
                                    //    tileflags |= TileFlags.Refresh;

                                    break;
                                }
                            case OtbmAttribute.Item:
                                {
                                    ushort itemId = props.ReadUInt16();
                                    //Item item = Item.Create(itemId);

                                    //if (item == null)
                                    //{
                                    //    Log.Error("Failed to create item.");
                                    //    return false;
                                    //}

                                    //if (tile != null)
                                    //{
                                    //    tile.InternalAddThing(item);
                                    //    item.StartDecaying();
                                    //}
                                    //else if (item.IsGround)
                                    //    groundItem = item;
                                    //else
                                    //{
                                    //    tile = CreateTile(groundItem, item, tileX, tileY, tileZ);
                                    //    tile.InternalAddThing(item);
                                    //    item.StartDecaying();
                                    //}


                                    break;
                                }
                            default:
                                throw new Exception(string.Format("{0} Unknown tile attribute.", new Position(tileX, tileY, tileZ)));
                        }
                    }

                    var nodeItem = nodeTile.Child;

                    while (nodeItem != null)
                    {
                        if (nodeItem.Type == (long)OtbMapNodeType.Item)
                        {
                            loader.GetProps(nodeItem, out props);
                            ushort itemId = props.ReadUInt16();

                            //// TODO: subclass item, different deserializations
                            //// for different types
                            //Item item = Item.Create(itemId);

                            //if (tile != null)
                            //{
                            //    tile.InternalAddThing(item);
                            //    item.StartDecaying();
                            //}
                            //else if (item.IsGround)
                            //    groundItem = item;
                            //else
                            //{
                            //    // !tile
                            //    tile = CreateTile(groundItem, item, tileX, tileY, tileZ);
                            //    tile.InternalAddThing(item);
                            //    item.StartDecaying();
                            //}
                        }
                        else
                        {
                            throw new Exception(string.Format("{0} Unknown node type.", new Position(tileX, tileY, tileZ)));
                        }

                        nodeItem = nodeItem.Next;
                    }

                    //if (tile == null)
                    //    tile = CreateTile(groundItem, null, tileX, tileY, tileZ);

                    //tile.SetFlag(tileflags);
                    //SetTile(tileX, tileY, tileZ, tile);
                }

                nodeTile = nodeTile.Next;
            }
        }

        private enum ClientVersion
        {
            //ClientVersion750 = 1,
            //ClientVersion755 = 2,
            //ClientVersion760 = 3,
            //ClientVersion770 = 3,
            //ClientVersion780 = 4,
            //ClientVersion790 = 5,
            //ClientVersion792 = 6,
            //ClientVersion800 = 7,
            ClientVersion810 = 8,
            //ClientVersion811 = 9,
            //ClientVersion820 = 10,
            //ClientVersion830 = 11,
            //ClientVersion840 = 12,
            //ClientVersion841 = 13,
            //ClientVersion842 = 14,
            //ClientVersion850 = 15,
            ClientVersion854Bad = 16,
            //ClientVersion854 = 17,
            //ClientVersion855 = 18,
            //ClientVersion860Old = 19,
            //ClientVersion860 = 20,
            //ClientVersion861 = 21
        };

        private enum OtbMapNodeType
        {
            //Root = 1,
            MapData = 2,
            //ItemDef = 3,
            TileArea = 4,
            Tile = 5,
            Item = 6,
            //TileSquare = 7,
            //TileRef = 8,
            //Spawns = 9,
            //SpawnArea = 10,
            //Monster = 11,
            Towns = 12,
            //Town = 13,
            HouseTile = 14,
            //WayPoints = 15,
            //WayPoint = 16
        }

        private enum OtbmAttribute : byte
        {
            Description = 1,
            //ExtFile = 2,
            TileFlags = 3,
            //ActionId = 4,
            //UniqueId = 5,
            //Text = 6,
            //Desc = 7,
            //TELEPORTDestination = 8,
            Item = 9,
            //DepotId = 10,
            ExtSpawnFile = 11,
            //RuneCharges = 12,
            ExtHouseFile = 13,
            //HouseDoorId = 14,
            //Count = 15,
            //Duration = 16,
            //DecayingState = 17,
            //WrittenDate = 18,
            //WrittenBy = 19,
            //SleeperId = 20,
            //SleepStart = 21,
            //Charges = 22
        }
		
		#endregion

        public World World { get { return _world; } }
    }
}

