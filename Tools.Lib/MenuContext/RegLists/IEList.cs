using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class IEList : RegistryList
    {
        public override void LoadItems()
        {
            List<string> names = new List<string>();
            using (RegistryKey ieKey = RegistryExtension.GetRegistryKey(IEItem.IEPath))
            {
                if (ieKey == null)
                    return;
                foreach (string part in IEItem.MeParts)
                {
                    using (RegistryKey meKey = ieKey.OpenSubKey(part))
                    {
                        if (meKey == null)
                            continue;
                        foreach (string keyName in meKey.GetSubKeyNames())
                        {
                            if (names.Contains(keyName, StringComparer.OrdinalIgnoreCase))
                                continue;
                            using (RegistryKey key = meKey.OpenSubKey(keyName))
                            {
                                if (!string.IsNullOrEmpty(key.GetValue("")?.ToString()))
                                {
                                    this.Items.Add(new IEItem(key.Name));
                                    names.Add(keyName);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
