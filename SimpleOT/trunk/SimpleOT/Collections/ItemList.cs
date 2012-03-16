using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Domain;

namespace SimpleOT.Collections
{
    public class ItemList : List<Item>
    {
        private int _downItemCount;

        public new void Add(Item item)
        {
            if (item == null)
                return;

            if (item.IsAlwaysOnTop)
            {
                bool isInserted = false;

                for (int i = TopItemCount; i < Count; i++)
                {
                    var other = base[i];
                    if (other.TopOrder > item.TopOrder)
                    {
                        base.Insert(i, item);
                        isInserted = true;
                        break;
                    }
                }

                if (!isInserted)
                    base.Add(item);
            }
            else
            {
                base.Insert(_downItemCount, item);
                ++_downItemCount;
            }
        }

        public IEnumerable<Item> GetTopItems()
        {
            for (int i = TopItemCount; i < Count; i++)
            {
                yield return base[i];
            }
        }

        public IEnumerable<Item> GetDownItems()
        {
            for (int i = 0; i < _downItemCount; i++)
            {
                yield return base[i];
            }
        }

        public bool IsEmpty { get { return Count == 0; } }

        public int DownItemCount { get { return _downItemCount; } }

        public int TopItemCount { get { return Count - _downItemCount; } }
    }
}
