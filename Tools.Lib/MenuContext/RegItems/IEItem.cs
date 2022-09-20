using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class IEItem : ShellItem
    {
        public static readonly string[] MeParts = { "MenuExt", "-MenuExt" };
        public const string IEPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer";

        private string _MeKeyName => RegistryExtension.GetKeyName(RegistryExtension.GetParentPath(this.Path));
        private string _BackupPath => $@"{IEPath}\{(this.IsEnable ? MeParts[1] : MeParts[0])}\{KeyName}";

        public override string KeyName => RegistryExtension.GetKeyName(this.Path);

        public override string ItemText
        {
            get => RegistryExtension.GetKeyName(this.Path);

            set
            {
                string newPath = $@"{RegistryExtension.GetParentPath(this.Path)}\{value.Replace("\\", "")}";
                string defaultValue = Registry.GetValue(newPath, "", null)?.ToString();
                if (!string.IsNullOrWhiteSpace(defaultValue))
                    throw new MenuContextException("此项目已被添加");

                RegistryExtension.MoveTo(this.Path, newPath);
                this.Path = newPath;
            }

        }

        public override bool IsEnable
        {
            get => _MeKeyName.Equals(MeParts[0], StringComparison.OrdinalIgnoreCase);
            set
            {
                RegistryExtension.MoveTo(this.Path, _BackupPath);
                this.Path = _BackupPath;
            }
        }


        public override string ItemCommand
        {
            get => Registry.GetValue(this.Path, "", null)?.ToString();
            set => Registry.SetValue(this.Path, "", value);
        }

        public override string ItemFilePath => ObjectPath.ExtractFilePath(this.ItemCommand);

        public override Icon ItemIcon => IconResource.GetIcon(ItemFilePath) ?? IconResource.GetExtensionIcon(ItemFilePath);

        public IEItem(string path) : base(path)
        {
        }
    }
}
