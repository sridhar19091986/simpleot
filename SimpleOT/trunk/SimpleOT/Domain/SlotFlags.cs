using System;

namespace SimpleOT.Domain
{
    [Flags]
    public enum SlotFlags : uint
    {
        WhereEver = 0xFFFFFFFF,
        Head = 1,
        Necklace = 2,
        Backpack = 4,
        Armor = 8,
        Right = 16,
        Left = 32,
        Legs = 64,
        Feet = 128,
        Ring = 256,
        Ammo = 512,
        Depot = 1024,
        TwoHand = 2048,
    }
}