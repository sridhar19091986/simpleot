using System;

namespace SimpleOT.Domain
{
	public class ItemType
	{
		public ushort Id;

        public ushort ClientId;

        public string Name;

        public string Article;

        public string Plural;

        public string Description;

        public bool BlockSolid;

        public bool IsStackable;

        public ushort Speed;

        public int Charges;

        public float Weight;

        public int Defense;

        public byte TopOrder;

        public ushort MaxItems;

        public bool IsBlocking;

        public bool IsProjectileBlocking;

        public bool IsPathBlocking;

        public bool HasHeight;

        public bool IsUseable;

        public bool IsPickupable;

        public bool IsMoveable;

        public bool IsAlwaysOnTop;

        public bool IsVertical;

        public bool IsHorizontal;

        public bool IsHangable;

        public bool IsDistanceReadable;

        public bool IsRotatable;

        public bool IsReadable;

        public bool HasClientCharges;

        public bool CanLookThrough;

        public bool IsReplaceable;

        public ushort LightLevel;

        public ushort LightColor;

        public ItemGroup Group;

        public FloorChangeDirection FloorChange;

        public MagicEffectType MagicEffect;

        public WeaponType WeaponType;

        public SlotFlags SlotFlags;

        public SlotType WieldPosition;

		public ItemType ()
		{
		}
	}
}

