using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class OpenWithItem : ShellItem
    {
        private string _ShellPath => RegistryExtension.GetParentPath(this.Path);
        private string _AppPath => RegistryExtension.GetParentPath(RegistryExtension.GetParentPath(this._ShellPath));
        private bool _NameEquals => RegistryExtension.GetKeyName(this._AppPath).Equals(System.IO.Path.GetFileName(this.ItemFilePath), StringComparison.OrdinalIgnoreCase);

        public override Icon ItemIcon => Icon.ExtractAssociatedIcon(this.ItemFilePath);

        public override string ItemText
        {
            get
            {
                string name = null;
                if (this._NameEquals)
                {
                    name = Registry.GetValue(this._AppPath, "FriendlyAppName", null)?.ToString();
                    name = StringResource.GetDirectString(name);
                }
                if (string.IsNullOrEmpty(name))
                    name = FileVersionInfo.GetVersionInfo(this.ItemFilePath).FileDescription;
                if (string.IsNullOrEmpty(name))
                    name = System.IO.Path.GetFileName(this.ItemFilePath);
                return name;
            }
            set
            {
                Registry.SetValue(this._AppPath, "FriendlyAppName", value);
            }
        }

        public override string ItemCommand
        {
            get => Registry.GetValue(this.Path, "", null)?.ToString();
            set
            {
                if (ObjectPath.ExtractFilePath(value) != this.ItemFilePath)
                    throw new MenuContextException("不允许更改文件路径！");

                Registry.SetValue(this.Path, "", value);
            }
        }

        public override bool IsEnable
        {
            get => Registry.GetValue(this._AppPath, "NoOpenWith", null) == null;
            set
            {
                if (value)
                    RegistryExtension.DeleteValue(this._AppPath, "NoOpenWith");
                else
                    Registry.SetValue(this._AppPath, "NoOpenWith", "");
            }
        }

        public OpenWithItem(string path) : base(path)
        {
            this.ItemFilePath = ObjectPath.ExtractFilePath(this.ItemCommand);
        }
    }
}
