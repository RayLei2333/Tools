using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class ShellNewList : RegistryList
    {
        private SeparatorItem _Separator { get; set; }

        public override void LoadItems()
        {
            this.Items.Clear();
            this._Separator = new SeparatorItem();
            this.Items.Add(this._Separator);

            if (ShellNewItem.IsLocked)
                this.LoadLockItems();
            this.LoadUnlockItems();
        }

        /// <summary>根据ShellNewPath的Classes键值扫描</summary>
        private void LoadLockItems()
        {
            string[] extensions = (string[])Registry.GetValue(RegistryPath.ShellNewPath, "Classes", null);
            this.LoadItems(extensions.ToList());
        }

        private void LoadUnlockItems()
        {
            List<string> extensions = new List<string> { "Folder" };//文件夹
            using (RegistryKey root = Registry.ClassesRoot)
            {
                extensions.AddRange(Array.FindAll(root.GetSubKeyNames(), keyName => keyName.StartsWith(".")));
                if (WinOsVersion.Current < WinOsVersion.Win10)
                    extensions.Add("Briefcase");//公文包(Win10没有)
                this.LoadItems(extensions);
            }
        }

        private void LoadItems(List<string> extensions)
        {
            foreach (string extension in ShellNewItem._UnableSortExtensions)
            {
                if (extensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                {
                    extensions.Remove(extension);
                    extensions.Insert(0, extension);
                }
            }

            using (RegistryKey root = Registry.ClassesRoot)
            {
                foreach (string extension in extensions)
                {
                    using (RegistryKey extKey = root.OpenSubKey(extension))
                    {
                        string defalutOpenMode = extKey?.GetValue("")?.ToString();
                        if (string.IsNullOrEmpty(defalutOpenMode) || defalutOpenMode.Length > 255)
                            continue;

                        using (RegistryKey openModeKey = root.OpenSubKey(defalutOpenMode))
                        {
                            if (openModeKey == null)
                                continue;
                            string value1 = openModeKey.GetValue("FriendlyTypeName")?.ToString();
                            string value2 = openModeKey.GetValue("")?.ToString();
                            value1 = StringResource.GetDirectString(value1);
                            if (string.IsNullOrWhiteSpace(value1) && string.IsNullOrWhiteSpace(value2))
                                continue;
                        }

                        using (RegistryKey tKey = extKey.OpenSubKey(defalutOpenMode))
                        {
                            foreach (string part in ShellNewItem._SnParts)
                            {
                                string snPart = part;
                                if (tKey != null)
                                    snPart = $@"{defalutOpenMode}\{snPart}";
                                using (RegistryKey snKey = extKey.OpenSubKey(snPart))
                                {
                                    if (ShellNewItem._EffectValueNames.Any(valueName => snKey?.GetValue(valueName) != null))
                                    {
                                        ShellNewItem item = new ShellNewItem(snKey.Name);

                                        if (item.BeforeSeparator)
                                        {
                                            int index2 = this.Items.IndexOf(this._Separator);
                                            this.Items.Insert(index2, item);
                                        }
                                        else
                                        {
                                            this.Items.Add(item);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
    }
}
