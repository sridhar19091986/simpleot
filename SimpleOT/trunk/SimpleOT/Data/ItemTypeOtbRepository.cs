using System;
using SimpleOT.Domain;
using System.IO;
using NLog;
using SimpleOT.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace SimpleOT.Data
{
	public class ItemTypeOtbRepository : IItemTypeRepository
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();
		
		private IDictionary<ushort, ItemType> _types;
		
		private string _fileName;
		private string _xmlFileName;
		
		private uint _majorVersion;
		private uint _minorVersion;
		private uint _buildNumber;
		
		public ItemTypeOtbRepository (string fileName)
		{
			if(!File.Exists(fileName))
				throw new Exception("Item type file not found.");
			
			var xmlFileName = Path.ChangeExtension(fileName, "xml");
			
			if(!File.Exists(fileName))
				throw new Exception("Item type xml file not found.");
			
			_fileName = fileName;
			_xmlFileName = xmlFileName;
			
			_types = new Dictionary<ushort, ItemType>();
			
			LoadOtb();
		}

		public ItemType Get (ushort id)
		{
			throw new NotImplementedException ();
		}

		public ItemType GetByClientId (ushort id)
		{
			throw new NotImplementedException ();
		}
		
		private void LoadOtb()
        {
            var loader = new FileLoader();
            loader.OpenFile(_fileName);
            var node = loader.GetRootNode();

            PropertyReader props;
			
            if (loader.GetProps(node, out props))
            {
                props.ReadByte(); //junk?

                // 4 byte flags
                // attributes
                // 0x01 = version data
                var flags = props.ReadUInt32();
                var attr = props.ReadByte();

                if (attr == 0x01)
                {
                    var datalen = props.ReadUInt16();

                    if (datalen != 140)
						throw new Exception("Invalid item.otb file.");

                    _majorVersion = props.ReadUInt32();
                    _minorVersion = props.ReadUInt32();
                    _buildNumber = props.ReadUInt32();
                }
            }

            if (_majorVersion == 0xFFFFFFFF)
                logger.Warn("items.otb using generic client version.");
            else if (_majorVersion < 3)
            {
				throw new Exception("Old version of items.otb detected, a newer version of items.otb is required.");
            }
            else if (_majorVersion > 3)
            {
                throw new Exception("New version of items.otb detected, a newer version of the server is required.");
            }
            //else if(Items::dwMinorVersion != CLIENT_VERSION_861){
            //    std::cout << "Another (client) version of items.otb is required." << std::endl;
            //    return ERROR_INVALID_FORMAT;
            //}

            node = node.Child;

            while (node != null)
            {
                if (!loader.GetProps(node, out props))
                    throw new Exception("Invalid item.otb file.");

                var type = new ItemType { Group = (ItemGroup)node.Type };

                var flags = (ItemFlags)props.ReadUInt32();
                type.IsBlocking = (flags & ItemFlags.BlocksSolid) != 0;
                type.IsProjectileBlocking = (flags & ItemFlags.BlocksProjectile) != 0;
                type.IsPathBlocking = (flags & ItemFlags.BlocksPathFinding) != 0;
                type.HasHeight = (flags & ItemFlags.HasHeight) != 0;
                type.IsUseable = (flags & ItemFlags.Useable) != 0;
                type.IsPickupable = (flags & ItemFlags.Pickupable) != 0;
                type.IsMoveable = (flags & ItemFlags.Moveable) != 0;
                type.IsStackable = (flags & ItemFlags.Stackable) != 0;
                type.IsAlwaysOnTop = (flags & ItemFlags.AlwaysOnTop) != 0;
                type.IsVertical = (flags & ItemFlags.Vertical) != 0;
                type.IsHorizontal = (flags & ItemFlags.Horizontal) != 0;
                type.IsHangable = (flags & ItemFlags.Hangable) != 0;
                type.IsDistanceReadable = (flags & ItemFlags.AllowDistanceRead) != 0;
                type.IsRotatable = (flags & ItemFlags.Rotatable) != 0;
                type.IsReadable = (flags & ItemFlags.Readable) != 0;
                type.HasClientCharges = (flags & ItemFlags.ClientCharges) != 0;
                type.CanLookThrough = (flags & ItemFlags.LookThrough) != 0;

                // process flags

                while (props.PeekChar() != -1)
                {
                    var attr = props.ReadByte();
                    var datalen = props.ReadUInt16();
                    switch ((ItemAttribute)attr)
                    {
                        case ItemAttribute.ServerId:
                            type.Id = props.ReadUInt16();

                            if (type.Id > 20000)
                            {
								throw new Exception(string.Format("Invalid item id {0}.", type.Id));
                            }

                            break;
                        case ItemAttribute.ClientId:
                            type.ClientId = props.ReadUInt16();
                            break;
                        case ItemAttribute.Speed:
                            type.Speed = props.ReadUInt16();
                            break;
                        case ItemAttribute.Light2:
                            type.LightLevel = props.ReadUInt16();
                            type.LightColor = props.ReadUInt16();
                            break;
                        case ItemAttribute.TopOrder:
                            type.TopOrder = props.ReadByte();
                            break;
                        default:
                            props.ReadBytes(datalen);
                            break;
                    }
                }

                if (_types.ContainsKey(type.Id))
                    logger.Warn("Duplicated item with id {0}", type.Id);
                else
                    _types.Add(type.Id, type);

                node = node.Next;
            }
        }
		
//		private void LoadXml()
//        {
//            try
//            {
//                var xml = XElement.Load(_xmlFileName);
//
//                foreach (var item in xml.Elements("item"))
//                {
//                    var id = item.Attribute("id").GetUInt16();
//
//                    if (id > 20000 && id < 20100)
//                    {
//                        id = (ushort)(id - 20000);
//                        _types[id] = new ItemType { Id = id };
//                    }
//
//                    var type = _types[id];
//
//                    type.Name = item.Attribute("name").GetString();
//                    type.Article = item.Attribute("article").GetString();
//                    type.Plural = item.Attribute("plural").GetString();
//
//                    LoadAttributes(type, item);
//                }
//
//                return true;
//            }
//            catch (Exception e)
//            {
//                Log.Error("Can not load items.xml", e);
//                return false;
//            }
//        }
//
//        private static void LoadAttributes(ItemType type, XContainer element)
//        {
//            foreach (var property in element.Elements("attribute"))
//            {
//                switch (property.Attribute("key").GetString())
//                {
//                    case "description":
//                        type.Description = property.Attribute("value").GetString();
//                        break;
//                    case "floorchange":
//                        switch (property.Attribute("value").GetString().ToLower())
//                        {
//                            case "down":
//                                type.FloorChange = FloorChangeDirection.Down;
//                                break;
//                            case "north":
//                                type.FloorChange = FloorChangeDirection.North;
//                                break;
//                            case "south":
//                                type.FloorChange = FloorChangeDirection.South;
//                                break;
//                            case "west":
//                                type.FloorChange = FloorChangeDirection.West;
//                                break;
//                            case "east":
//                                type.FloorChange = FloorChangeDirection.East;
//                                break;
//                        }
//
//                        break;
//
//                    case "replaceable":
//                        type.IsReplaceable = property.Attribute("value").GetInt32() != 0;
//                        break;
//                    case "effect":
//                        type.MagicEffect = EnumConverter.ToMagicEffectType(property.Attribute("value").GetString());
//                        break;
//                    case "charges":
//                        type.Charges = property.Attribute("value").GetInt32();
//                        break;
//                    case "weight":
//                        type.Weight = property.Attribute("value").GetInt32() / 100f;
//                        break;
//                    case "defense":
//                        type.Defense = property.Attribute("value").GetInt32();
//                        break;
//                    case "weapontype":
//
//                        switch (property.Attribute("value").GetString().ToLower())
//                        {
//                            case "sword":
//                                type.WeaponType = WeaponType.Sword;
//                                break;
//                            case "club":
//                                type.WeaponType = WeaponType.Club;
//                                break;
//                            case "axe":
//                                type.WeaponType = WeaponType.Axe;
//                                break;
//                            case "shield":
//                                type.WeaponType = WeaponType.Shield;
//                                break;
//                            case "distance":
//                                type.WeaponType = WeaponType.Distance;
//                                break;
//                            case "wand":
//                                type.WeaponType = WeaponType.Wand;
//                                break;
//                            case "ammunition":
//                                type.WeaponType = WeaponType.Ammo;
//                                break;
//                            default:
//                                Log.Warn("Unknown weaponType " + property.Attribute("value").GetString());
//                                break;
//                        }
//
//                        break;
//
//                    case "slottype":
//
//                        switch (property.Attribute("value").GetString().ToLower())
//                        {
//                            case "head":
//                                type.SlotFlags |= SlotFlags.Head;
//                                type.WieldPosition = SlotType.Head;
//                                break;
//                            case "body":
//                                type.SlotFlags |= SlotFlags.Armor;
//                                type.WieldPosition = SlotType.Armor;
//                                break;
//                            case "legs":
//                                type.SlotFlags |= SlotFlags.Legs;
//                                type.WieldPosition = SlotType.Legs;
//                                break;
//                            case "feet":
//                                type.SlotFlags |= SlotFlags.Feet;
//                                type.WieldPosition = SlotType.Feet;
//                                break;
//                            case "backpack":
//                                type.SlotFlags |= SlotFlags.Backpack;
//                                type.WieldPosition = SlotType.Backpack;
//                                break;
//                            case "two-handed":
//                                type.SlotFlags |= SlotFlags.TwoHand;
//                                type.WieldPosition = SlotType.TwoHand;
//                                break;
//                            case "necklace":
//                                type.SlotFlags |= SlotFlags.Necklace;
//                                type.WieldPosition = SlotType.Necklace;
//                                break;
//                            case "ring":
//                                type.SlotFlags |= SlotFlags.Ring;
//                                type.WieldPosition = SlotType.Ring;
//                                break;
//                            case "hand":
//                                type.WieldPosition = SlotType.Hand;
//                                break;
//                            case "ammo":
//                                type.WieldPosition = SlotType.Ammo;
//                                break;
//                            default:
//                                Log.Warn("Unknown slotType " + property.Attribute("value").GetString());
//                                break;
//                        }
//
//                        break;
//                }
//            }
//        }
		
		enum ItemAttribute : byte
        {
            ServerId = 0x10,
            ClientId,
            Name,				/*deprecated*/
            Description,			/*deprecated*/
            Speed,
            Slot,				/*deprecated*/
            MaxItems,			/*deprecated*/
            Weight,			/*deprecated*/
            Weapon,			/*deprecated*/
            Ammunition,				/*deprecated*/
            Armor,			/*deprecated*/
            MagicLevel,			/*deprecated*/
            MagicFieldType,		/*deprecated*/
            Writeable,		/*deprecated*/
            RotateTo,			/*deprecated*/
            Decay,			/*deprecated*/
            SpriteHash,
            MiniMapColor,
            Attr07,
            Attr08,
            Light,

            //1-byte aligned
            Decay2,			/*deprecated*/
            Weapon2,			/*deprecated*/
            Ammunition2,				/*deprecated*/
            Armor2,			/*deprecated*/
            Writeable2,		/*deprecated*/
            Light2,
            TopOrder,
            Writeable3		/*deprecated*/
        }

        [Flags]
        enum ItemFlags : uint
        {
            BlocksSolid = 1,
            BlocksProjectile = 2,
            BlocksPathFinding = 4,
            HasHeight = 8,
            Useable = 16,
            Pickupable = 32,
            Moveable = 64,
            Stackable = 128,
            //FloorChangeDown = 256,
            //FloorChangeNorth = 512,
            //FloorChangeEast = 1024,
            //FloorChangeSouth = 2048,
            //FloorChangeWest = 4096,
            AlwaysOnTop = 8192,
            Readable = 16384,
            Rotatable = 32768,
            Hangable = 65536,
            Vertical = 131072,
            Horizontal = 262144,
            //CannotDecay = 524288,
            AllowDistanceRead = 1048576,
            //Unused = 2097152,
            ClientCharges = 4194304,
            LookThrough = 8388608
        }
	}
	
}

