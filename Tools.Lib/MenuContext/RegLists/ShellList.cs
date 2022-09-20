using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class ShellList : RegistryList
    {
        private static string _CurrentExtension = null;
        private static string _CurrentDirectoryType = null;
        private static string _CurrentPerceivedType = null;
        private static string _CurrentCustomRegPath = null;
        //private static string _CurrentFileObjectPath = null;
        private static string GetShellPath(string scenePath) => $@"{scenePath}\shell";
        private static string GetShellExPath(string scenePath) => $@"{scenePath}\ShellEx";
        private static string GetSysAssExtPath(string typeName) => typeName != null ? $@"{RegistryPath.SysFileassPath}\{typeName}" : null;
        private static string GetOpenMode(string extension) => FileExtension.GetOpenMode(extension);
        private static string GetOpenModePath(string extension) => extension != null ? $@"{RegistryExtension.CLASSES_ROOT}\{GetOpenMode(extension)}" : null;
        private static string GetPerceivedType(string extension) => Registry.GetValue($@"{RegistryExtension.CLASSES_ROOT}\{extension}", "PerceivedType", null)?.ToString();

        public Scenes Scene { get; set; }

        public override void LoadItems()
        {
            this.Items.Clear();
            string scenePath = this.GetScenePath();
            if (scenePath == null)
                return;

            this.LoadItems(scenePath);
            this.LoadVisibleRegItems();
        }

        private string GetScenePath()
        {
            string scenesPath = null;
            switch (Scene)
            {
                case Scenes.File:
                    return RegistryPath.MenuPathFile;
                case Scenes.Folder:
                    return RegistryPath.MenuPathFolder;
                case Scenes.Directory:
                    return RegistryPath.MenuPathDirectory;
                case Scenes.Background:
                    return RegistryPath.MenuPathBackground;
                case Scenes.Desktop:
                    return RegistryPath.MenuPathDesktop;
                case Scenes.Drive:
                    return RegistryPath.MenuPathDrive;
                case Scenes.AllObjects:
                    return RegistryPath.MenuPathAllObjects;
                case Scenes.Computer:
                    return RegistryPath.MenuPathComputer;
                case Scenes.RecycleBin:
                    return RegistryPath.MenuPathRecycleBin;
                case Scenes.UnknownType:
                    return RegistryPath.MenuPathUnknown;

                case Scenes.CustomRegPath:
                    return _CurrentCustomRegPath;
                case Scenes.LnkFile:
                    return GetOpenModePath(".lnk");
                case Scenes.ExeFile:
                    return GetSysAssExtPath(".exe");
                case Scenes.PerceivedType:
                    return GetSysAssExtPath(_CurrentPerceivedType);


                case Scenes.Library:
                    if (WinOsVersion.Current == WinOsVersion.Vista)
                        return null;
                    return RegistryPath.MenuPathLibrary;
                case Scenes.UwpLnk:
                    //Win8之前没有Uwp
                    if (WinOsVersion.Current < WinOsVersion.Win8)
                        return null;
                    return RegistryPath.MenuPathUWPlnk;

                case Scenes.CustomExtension:
                    if (_CurrentExtension?.ToLower() == ".lnk")
                        return GetOpenModePath(".lnk");
                    else
                        return GetSysAssExtPath(_CurrentExtension);
                case Scenes.DirectoryType:
                    if (_CurrentDirectoryType == null)
                        return null;
                    else
                        return GetSysAssExtPath($"Directory.{_CurrentDirectoryType}");

                case Scenes.CommandStore:
                    if (WinOsVersion.Current == WinOsVersion.Vista)
                        return null;
                    RegistryExtension.GetParentPath(RegistryPath.CommandStorePath);
                    //this.AddNewItem(RegistryEx.GetParentPath(ShellItem.CommandStorePath));
                    this.LoadStoreItems();
                    return null;

                case Scenes.DragDrop:
                    return null;

                    //case Scenes.MenuAnalysis:
                    //    this.LoadAnalysisItems();
                    //    return null;
                    //case Scenes.CustomExtensionPerceivedType:
                    //    //if(this.Scene == Scenes.CustomExtension)
                    //    break;
            }

            return scenesPath;
        }
        private void LoadItems(string scenePath)
        {
            RegTrustedInstaller.TakeRegKeyOwnerShip(scenePath);

            this.LoadShellItems(GetShellPath(scenePath));
            this.LoadShellExItems(GetShellExPath(scenePath));
        }

        private void LoadShellItems(string shellPath)
        {
            using (RegistryKey shellKey = RegistryExtension.GetRegistryKey(shellPath))
            {
                if (shellKey == null) 
                    return;
                RegTrustedInstaller.TakeRegTreeOwnerShip(shellKey.Name);
                foreach (string keyName in shellKey.GetSubKeyNames())
                    this.Items.Add(new ShellItem($@"{shellPath}\{keyName}"));
            }
        }

        private void LoadShellExItems(string shellExPath)
        {
            List<string> names = new List<string>();
            using (RegistryKey shellExKey = RegistryExtension.GetRegistryKey(shellExPath))
            {
                if (shellExKey == null)
                    return;

                bool isDragDrop = this.Scene == Scenes.DragDrop;
                RegTrustedInstaller.TakeRegTreeOwnerShip(shellExKey.Name);
                Dictionary<string, Guid> dic = ShellExItem.GetPathAndGuids(shellExPath);
                foreach (string path in dic.Keys)
                {
                    string keyName = RegistryExtension.GetKeyName(path);
                    if (!names.Contains(keyName))
                    {
                        ShellExItem item = new ShellExItem(dic[path], path);
                        names.Add(keyName);
                        this.Items.Add(item);
                    }
                }
            }
        }

        private void LoadStoreItems()
        {
            using (RegistryKey shellKey = RegistryExtension.GetRegistryKey(RegistryPath.CommandStorePath))
            {
                foreach (string itemName in shellKey.GetSubKeyNames())
                {
                    //   if (AppConfig.HideSysStoreItems && itemName.StartsWith("Windows.", StringComparison.OrdinalIgnoreCase)) 
                    if (itemName.StartsWith("Windows.", StringComparison.OrdinalIgnoreCase))
                        continue;
                    this.Items.Add(new StoreItem($@"{RegistryPath.CommandStorePath}\{itemName}", true));
                }
            }
        }

        private void LoadVisibleRegItems()
        {
            switch (Scene)
            {
                case Scenes.Background:
                    this.Items.Add(new RegVisbleItem(RegVisbleItem.CustomFolder));
                    break;
                case Scenes.Computer:
                    this.Items.Add(new RegVisbleItem(RegVisbleItem.NetworkDrive));
                    break;
                case Scenes.RecycleBin:
                    this.Items.Add(new RegVisbleItem(RegVisbleItem.RecycleBinProperties));
                    break;
                case Scenes.Library:
                    this.LoadItems(RegistryPath.MenuPathLibraryBackground);
                    this.LoadItems(RegistryPath.MenuPathLibraryUser);
                    break;
                case Scenes.ExeFile:
                    this.LoadItems(GetOpenModePath(".exe"));
                    break;
                case Scenes.CustomExtension:
                case Scenes.PerceivedType:
                case Scenes.DirectoryType:
                case Scenes.CustomRegPath:
                    //this.InsertItem(new SelectItem(Scene), 0);
                    if (Scene == Scenes.CustomExtension && _CurrentExtension != null)
                    {
                        this.LoadItems(GetOpenModePath(_CurrentExtension));
                        //this.InsertItem(new SelectItem(Scenes.CustomExtensionPerceivedType), 1);
                    }
                    break;
            }
        }
    }
}
