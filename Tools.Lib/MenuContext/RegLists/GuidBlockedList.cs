using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class GuidBlockedList : RegistryList
    {
        public override void LoadItems()
        {
            this.Items.Clear();
            this.LoadBlockedItems();
        }

        private void LoadBlockedItems()
        {
            List<string> values = new List<string>();
            foreach (var item in GuidBlockedItem.BlockedPaths)
            {
                using (RegistryKey key = RegistryExtension.GetRegistryKey(item))
                {
                    if (key == null)
                        continue;
                    foreach (string value in key.GetValueNames())
                    {
                        if (values.Contains(value, StringComparer.OrdinalIgnoreCase))
                            continue;
                        this.Items.Add(new GuidBlockedItem(value));
                        values.Add(value);
                    }
                }
            }
        }
    }
}
