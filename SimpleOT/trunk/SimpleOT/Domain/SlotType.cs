namespace SimpleOT.Domain
{
    public enum SlotType : byte
    {
        Whereever = 0,
        First = 1,
        Head = First,
        Necklace = 2,
        Backpack = 3,
        Armor = 4,
        Right = 5,
        Left = 6,
        Legs = 7,
        Feet = 8,
        Ring = 9,
        Ammo = 10,
        Depot = 11,

        // Special slot, covers two, not a real slot
        Hand = 12,
        TwoHand = Hand, // alias

        // Last real slot is depot
        Last = Depot
    };
}