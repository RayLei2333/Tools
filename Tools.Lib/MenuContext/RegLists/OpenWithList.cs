using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class OpenWithList : RegistryList
    {
        public override void LoadItems()
        {
            this.LoadOpenWithItems();
            this.Sort();
            if (WinOsVersion.Current >= WinOsVersion.Win8)  //Win8及以上版本系统才有在应用商店中查找应用
            {
                RegVisbleItem visibleItem = new RegVisbleItem(RegVisbleItem.UseStoreOpenWith);
                this.Items.Insert(0, visibleItem);
            }
        }

        private void LoadOpenWithItems()
        {
            using (RegistryKey root = Registry.ClassesRoot)
            using (RegistryKey appKey = root.OpenSubKey("Applications"))
            {
                foreach (string appName in appKey.GetSubKeyNames())
                {
                    if (!appName.Contains('.')) //需要为有扩展名的文件名
                        continue;
                    using (RegistryKey shellKey = appKey.OpenSubKey($@"{appName}\shell"))
                    {
                        if (shellKey == null)
                            continue;
                        List<string> names = shellKey.GetSubKeyNames().ToList();
                        if (names.Contains("open", StringComparer.OrdinalIgnoreCase)) names.Insert(0, "open");
                        string keyName = names.Find(name =>
                        {
                            using (RegistryKey cmdKey = shellKey.OpenSubKey(name))
                                return cmdKey.GetValue("NeverDefault") == null;
                        });

                        if (keyName == null)
                            continue;
                        using (RegistryKey commandKey = shellKey.OpenSubKey($@"{keyName}\command"))
                        {
                            string command = commandKey?.GetValue("")?.ToString();
                            if (ObjectPath.ExtractFilePath(command) != null)
                                this.Items.Add(new OpenWithItem(commandKey.Name));
                        }
                    }
                }
            }
        }
    }
}
