using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.Win32Struct;

namespace Tools.Lib.MenuContext.RegItems
{
    public class ShellNewItem : ShellItem
    {
        public static readonly string[] _SnParts = { "ShellNew", "-ShellNew" };
        public static readonly string[] _UnableSortExtensions = { "Folder", ".library-ms" };
        public static readonly string[] _DefaultBeforeSeparatorExtensions = { "Folder", ".library-ms", ".lnk" };
        public static readonly string[] _EffectValueNames = { "NullFile", "Data", "FileName", "Directory", "Command" };
        private static readonly string[] _UnableEditDataValues = { "Directory", "FileName", "Handler", "Command" };
        private static readonly string[] _UnableChangeCommandValues = { "Data", "Directory", "FileName", "Handler" };
        public static bool IsLocked
        {
            get
            {
                using (RegistryKey key = RegistryExtension.GetRegistryKey(RegistryPath.ShellNewPath))
                {
                    RegistrySecurity rs = key.GetAccessControl();
                    foreach (RegistryAccessRule rar in rs.GetAccessRules(true, true, typeof(NTAccount)))
                    {
                        if (rar.AccessControlType.ToString().Equals("Deny", StringComparison.OrdinalIgnoreCase))
                        {
                            if (rar.IdentityReference.ToString().Equals("Everyone", StringComparison.OrdinalIgnoreCase))
                                return true;
                        }
                    }
                }
                return false;
            }
        }

        private string _SnKeyName => RegistryExtension.GetKeyName(this.Path);
        private string _BackupPath => $@"{RegistryExtension.GetParentPath(this.Path)}\{(this.IsEnable ? _SnParts[1] : _SnParts[0])}";
        private string _OpenMode => FileExtension.GetOpenMode(this.Extension);//关联打开方式
        private string _OpenModePath => $@"{RegistryExtension.CLASSES_ROOT}\{_OpenMode}";//关联打开方式注册表路径
        private string _DefaultOpenMode => Registry.GetValue($@"{RegistryExtension.CLASSES_ROOT}\{this.Extension}", "", null)?.ToString();//HKCR默认值打开方式
        private string _DefaultOpenModePath => $@"{RegistryExtension.CLASSES_ROOT}\{_DefaultOpenMode}";//HKCR默认值打开方式路径
        private bool _CanEditData => _UnableEditDataValues.All(value => Registry.GetValue(this.Path, value, null) == null);//能够编辑初始数据的
        private bool _CanChangeCommand => _UnableChangeCommandValues.All(value => Registry.GetValue(this.Path, value, null) == null);//能够更改菜单命令的
        private bool _DefaultBeforeSeparator => _DefaultBeforeSeparatorExtensions.Contains(this.Extension, StringComparer.OrdinalIgnoreCase);//默认显示在分割线上不可更改的

        public override string ItemFilePath
        {
            get
            {
                string filePath = FileExtension.GetExtentionInfo(AssocStr.Executable, this.Extension);
                if (File.Exists(filePath))
                    return filePath;
                using (RegistryKey oKey = RegistryExtension.GetRegistryKey(this._OpenModePath))
                {
                    using (RegistryKey aKey = oKey.OpenSubKey("Application"))
                    {
                        string uwp = aKey?.GetValue("AppUserModelID")?.ToString();
                        if (uwp != null)
                            return "shell:AppsFolder\\" + uwp;
                    }
                    using (RegistryKey cKey = oKey.OpenSubKey("CLSID"))
                    {
                        string value = cKey?.GetValue("")?.ToString();
                        if (Guid.TryParse(value, out Guid guid))
                        {
                            filePath = GuidInfo.GetFilePath(guid);
                            if (filePath != null)
                                return filePath;
                        }
                    }
                }

                return null;
            }
        }

        public override bool IsEnable
        {
            get => _SnKeyName.Equals(_SnParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                RegistryExtension.MoveTo(this.Path, this._BackupPath);
                this.Path = this._BackupPath;
            }
        }

        public override string ItemText
        {
            get
            {
                string name = Registry.GetValue(this.Path, "MenuText", null)?.ToString();
                if (name != null && name.StartsWith("@"))
                {
                    name = StringResource.GetDirectString(name);
                    if (!string.IsNullOrEmpty(name))
                        return name;
                }
                name = Registry.GetValue(this._DefaultOpenModePath, "FriendlyTypeName", null)?.ToString();
                name = StringResource.GetDirectString(name);
                if (!string.IsNullOrEmpty(name))
                    return name;
                name = Registry.GetValue(this._DefaultOpenModePath, "", null)?.ToString();
                if (!string.IsNullOrEmpty(name))
                    return name;
                return null;

            }
            set
            {
                RegistryExtension.DeleteValue(this.Path, "MenuText");
                Registry.SetValue(this._DefaultOpenModePath, "FriendlyTypeName", value);
                // this.Text = ResourceString.GetDirectString(value);
            }
        }

        public override string IconLocation
        {
            get
            {
                string value = Registry.GetValue(this.Path, "IconPath", null)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
                value = Registry.GetValue($@"{this._OpenModePath}\DefaultIcon", "", null)?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
                return ItemFilePath;
            }
            set => Registry.SetValue(this.Path, "IconPath", value);
        }

        public override Icon ItemIcon
        {
            get
            {
                string location = IconLocation;
                if (location == null || location.StartsWith("@"))
                    return IconResource.GetExtensionIcon(this.Extension);
                Icon icon = IconResource.GetIcon(location, out string path, out int index);
                if (icon == null)
                    icon = IconResource.GetIcon(path = "imageres.dll", index = -2);
                this.IconPath = path;
                this.IconIndex = index;
                return icon;
            }
        }

        public override string ItemCommand
        {
            get => Registry.GetValue(this.Path, "Command", null)?.ToString();
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (Registry.GetValue(this.Path, "NullFile", null) != null)
                        RegistryExtension.DeleteValue(this.Path, "Command");
                }
                else
                    Registry.SetValue(this.Path, "Command", value);
            }
        }

        public bool BeforeSeparator
        {
            get
            {
                if (this._DefaultBeforeSeparator)
                    return true;
                else
                    return Registry.GetValue($@"{this.Path}\Config", "BeforeSeparator", null) != null;

            }
            set
            {
                if (value)
                    Registry.SetValue($@"{this.Path}\Config", "BeforeSeparator", "");
                else
                {
                    using (RegistryKey snkey = RegistryExtension.GetRegistryKey(this.Path, true))
                    using (RegistryKey ckey = snkey.OpenSubKey("Config", true))
                    {
                        ckey.DeleteValue("BeforeSeparator");
                        if (ckey.GetValueNames().Length == 0 && ckey.GetSubKeyNames().Length == 0)
                            snkey.DeleteSubKey("Config");
                    }
                }
            }
        }

        public bool CanSort => !_UnableSortExtensions.Contains(this.Extension, StringComparer.OrdinalIgnoreCase);//能够排序的
        public string Extension => this.Path.Split('\\')[1];

        public ShellNewItem(string path) : base(path)
        {
        }
    }
}
