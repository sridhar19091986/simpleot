using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Domain
{
    public class Item : Thing
    {
        private ItemType _itemType;

        public Item(ItemType itemType)
        {
            _itemType = itemType;
        }

        public bool IsAlwaysOnTop
        {
            get { return _itemType.IsAlwaysOnTop; }
        }

        public byte TopOrder
        {
            get { return _itemType.TopOrder; }
        }
    }
}
