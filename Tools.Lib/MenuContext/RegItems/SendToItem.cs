using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class SendToItem : RegItem
    {
        private string _FileExtension => System.IO.Path.GetExtension(this.Path);
        private bool _IsShortcut => this._FileExtension.ToLower() == ".lnk";

        public override string ItemFilePath
        {
            get
            {
                string path = null;
                if (this._IsShortcut)
                    path = ShellLink.TargetPath;
                else
                {
                    using (RegistryKey root = Registry.ClassesRoot)
                    using (RegistryKey extKey = root.OpenSubKey(this._FileExtension))
                    {
                        string guidPath = extKey?.GetValue("")?.ToString();
                        if (!string.IsNullOrEmpty(guidPath))
                        {
                            using (RegistryKey ipsKey = root.OpenSubKey($@"{guidPath}\InProcServer32"))
                            {
                                path = ipsKey?.GetValue("")?.ToString();
                            }
                        }
                    }
                }

                if (!File.Exists(path) && !Directory.Exists(path))
                    path = this.Path;
                return path;

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
                File.SetAttributes(this.Path, attributes);
            }
        }

        public override string ItemText
        {
            get
            {
                string name = DesktopIni.GetLocalizedFileNames(this.Path, true);
                if (name == string.Empty)
                    name = System.IO.Path.GetFileNameWithoutExtension(this.Path);
                if (name == string.Empty)
                    name = this._FileExtension;
                return name;
            }
            set
            {
                DesktopIni.SetLocalizedFileNames(this.Path, value);
            }
        }

        public override Icon ItemIcon
        {
            get
            {
                Icon icon = IconResource.GetIcon(this.IconLocation, out string iconPath, out int iconIndex);
                this.IconPath = iconPath;
                this.IconIndex = iconIndex;
                if (icon != null)
                    return icon;
                if (this._IsShortcut)
                {

                    string path = this.ItemFilePath;
                    if (File.Exists(path))
                        icon = IconResource.GetExtensionIcon(path);
                    else if (Directory.Exists(path))
                        icon = IconResource.GetFolderIcon(path);
                }
                if (icon == null)
                    icon = IconResource.GetExtensionIcon(this._FileExtension);
                return icon;
            }
        }

        public string IconLocation
        {
            get
            {
                string location = null;
                if (this._IsShortcut)
                {
                    ShellLink.ICONLOCATION iconLocation = ShellLink.IconLocation;
                    string iconPath = iconLocation.IconPath;
                    int iconIndex = iconLocation.IconIndex;
                    if (string.IsNullOrEmpty(iconPath))
                        iconPath = ShellLink.TargetPath;
                    location = $@"{iconPath},{iconIndex}";
                }
                else
                {
                    using (RegistryKey root = Registry.ClassesRoot)
                    using (RegistryKey extensionKey = root.OpenSubKey(this._FileExtension))
                    {
                        string guidPath = extensionKey.GetValue("")?.ToString();
                        if (guidPath != null)
                        {
                            using (RegistryKey guidKey = root.OpenSubKey($@"{guidPath}\DefaultIcon"))
                            {
                                location = guidKey.GetValue("")?.ToString();
                            }
                        }
                    }
                }

                return location;
            }

            set
            {
                if (this._IsShortcut)
                {
                    ShellLink.IconLocation = new ShellLink.ICONLOCATION
                    {
                        IconPath = this.IconPath,
                        IconIndex = this.IconIndex
                    };
                    ShellLink.Save();
                }
                else
                {
                    using (RegistryKey root = Registry.ClassesRoot)
                    using (RegistryKey extensionKey = root.OpenSubKey(this._FileExtension))
                    {
                        string guidPath = extensionKey.GetValue("")?.ToString();
                        if (guidPath != null)
                        {
                            string regPath = $@"{root.Name}\{guidPath}\DefaultIcon";
                            RegTrustedInstaller.TakeRegTreeOwnerShip(regPath);
                            Registry.SetValue(regPath, "", value);
                        }
                    }
                }
            }
        }


        public ShellLink ShellLink { get; private set; }

        public SendToItem(string path) : base(path)
        {
            if (this._IsShortcut)
                this.ShellLink = new ShellLink(path);
        }
    }
}
