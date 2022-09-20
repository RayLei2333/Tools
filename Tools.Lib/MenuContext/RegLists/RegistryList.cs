using System;
using System.Collections.Generic;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public abstract class RegistryList
    {
        public List<RegistryItem> Items { get; set; }

        public RegistryList()
        {
            this.Items = new List<RegistryItem>();
        }

        public abstract void LoadItems();

        protected void Sort()
        {
            this.Items.Sort(new TextComparer());
        }
    }

    public class TextComparer : IComparer<RegistryItem>
    {
        public int Compare(RegistryItem x, RegistryItem y)
        {
            if (x.Equals(y))
                return 0;
            string[] strs = { x.ItemText, y.ItemText };
            Array.Sort(strs);
            if (strs[0] == x.ItemText)
                return -1;
            else
                return 1;
        }
    }
}
