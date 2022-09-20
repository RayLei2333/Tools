using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class WinXItem : RegItem
    {
        public ShellLink ShellLink { get; set; }

        public override string ItemText
        {
            get
            {
                string name = this.ShellLink.Description?.Trim();
                if (string.IsNullOrWhiteSpace(name))
                    name = DesktopIni.GetLocalizedFileNames(this.Path, true);
                if (name == string.Empty)
                    name = System.IO.Path.GetFileNameWithoutExtension(this.Path);
                return name;
            }
            set
            {
                this.ShellLink.Description = value;
                this.ShellLink.Save();
                DesktopIni.SetLocalizedFileNames(this.Path, value);
            }
        }

        public override bool IsEnable
        {

            get => (File.GetAttributes(this.Path) & FileAttributes.Hidden) != FileAttributes.Hidden;
            set
            {
                FileAttributes attributes = File.GetAttributes(this.Path);
                if (value)
                    attributes &= ~FileAttributes.Hidden;
                else
                    attributes |= FileAttributes.Hidden;
                File.SetAttributes(this.ItemFilePath, attributes);
            }
        }

        public override string ItemFilePath
        {
            get
            {
                string path = ShellLink.TargetPath;
                if (!File.Exists(path) && !Directory.Exists(path))
                    path = this.Path;
                return path;
            }
        }

        public override Icon ItemIcon
        {
            get
            {
                ShellLink.ICONLOCATION iconLocation = ShellLink.IconLocation;
                string iconPath = iconLocation.IconPath;
                int iconIndex = iconLocation.IconIndex;
                if (string.IsNullOrEmpty(iconPath))
                    iconPath = this.Path;
                Icon icon = IconResource.GetIcon(iconPath, iconIndex);
                if (icon == null)
                {
                    string path = this.ItemFilePath;
                    if (File.Exists(path))
                        icon = IconResource.GetExtensionIcon(path);
                    else if (Directory.Exists(path))
                        icon = IconResource.GetFolderIcon(path);
                }
                return icon;
            }
        }

        public WinXItem(string path) : base(path)
        {
            this.ShellLink = new ShellLink(path);
        }
    }
}
