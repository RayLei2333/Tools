using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext
{
    internal class GuidInfo
    {
        private static readonly IniWriter UserDic = new IniWriter(AppConfig.UserGuidInfosDic);
        private static readonly IniReader WebDic = new IniReader(AppConfig.WebGuidInfosDic);
        private static readonly IniReader AppDic = new IniReader(new StringBuilder(AppConfig.GuidInfosDic));

        public static readonly string[] ClsidPaths =
        {
            @"HKEY_CLASSES_ROOT\CLSID",
            @"HKEY_CLASSES_ROOT\WOW6432Node\CLSID",
            @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Classes\CLSID",
        };
        private static readonly Dictionary<Guid, IconLocation> IconLocationDic = new Dictionary<Guid, IconLocation>();
        private static readonly Dictionary<Guid, string> ItemTextDic = new Dictionary<Guid, string>();
        private static readonly Dictionary<Guid, Icon> ItemImageDic = new Dictionary<Guid, Icon>();
        private static readonly Dictionary<Guid, string> FilePathDic = new Dictionary<Guid, string>();
        private static readonly Dictionary<Guid, string> ClsidPathDic = new Dictionary<Guid, string>();
        private static readonly Dictionary<Guid, string> UwpNameDic = new Dictionary<Guid, string>();

        public static string GetFilePath(Guid guid)
        {
            string filePath = null;
            if (guid.Equals(Guid.Empty))
                return filePath;
            if (FilePathDic.ContainsKey(guid))
                filePath = FilePathDic[guid];
            else
            {
                string uwpName = GetUwpName(guid);
                if (!string.IsNullOrEmpty(uwpName))
                    filePath = UwpHelper.GetFilePath(uwpName, guid);
                else
                {
                    foreach (string clsidPath in ClsidPaths)
                    {
                        using (RegistryKey guidKey = RegistryExtension.GetRegistryKey($@"{clsidPath}\{guid:B}"))
                        {
                            if (guidKey == null)
                                continue;
                            foreach (string keyName in new[] { "InprocServer32", "LocalServer32" })
                            {
                                using (RegistryKey key = guidKey.OpenSubKey(keyName))
                                {
                                    if (key == null)
                                        continue;
                                    string value1 = key.GetValue("CodeBase")?.ToString().Replace("file:///", "").Replace('/', '\\');
                                    if (File.Exists(value1))
                                    {
                                        filePath = value1; break;
                                    }
                                    string value2 = key.GetValue("")?.ToString();
                                    value2 = ObjectPath.ExtractFilePath(value2);
                                    if (File.Exists(value2))
                                    {
                                        filePath = value2; break;
                                    }
                                }
                            }

                            if (File.Exists(filePath))
                            {
                                if (ClsidPathDic.ContainsKey(guid))
                                    ClsidPathDic[guid] = guidKey.Name;
                                else
                                    ClsidPathDic.Add(guid, guidKey.Name);
                                break;
                            }
                        }
                    }
                }
                FilePathDic.Add(guid, filePath);
            }
            return filePath;
        }

        public static string GetClsidPath(Guid guid)
        {
            if (ClsidPathDic.ContainsKey(guid))
                return ClsidPathDic[guid];
            foreach (string path in ClsidPaths)
            {
                using (RegistryKey key = RegistryExtension.GetRegistryKey($@"{path}\{guid:B}"))
                {
                    if (key != null)
                        return key.Name;
                }
            }
            return null;
        }

        public static string GetText(Guid guid)
        {
            string itemText = null;
            if (guid.Equals(Guid.Empty))
                return itemText;
            if (ItemTextDic.ContainsKey(guid))
                itemText = ItemTextDic[guid];
            else
            {
                if (TryGetValue(guid, "ResText", out itemText))
                {
                    itemText = GetAbsStr(guid, itemText, true);
                    itemText = StringResource.GetDirectString(itemText);
                }

                if (string.IsNullOrWhiteSpace(itemText))
                {
                    string uiText = CultureInfo.CurrentUICulture.Name + "-Text";
                    TryGetValue(guid, uiText, out itemText);
                    if (string.IsNullOrWhiteSpace(itemText))
                    {
                        TryGetValue(guid, "Text", out itemText);
                        itemText = StringResource.GetDirectString(itemText);
                    }
                }
                if (string.IsNullOrWhiteSpace(itemText))
                {
                    foreach (string clsidPath in ClsidPaths)
                    {
                        foreach (string value in new[] { "LocalizedString", "InfoTip", "" })
                        {
                            itemText = Registry.GetValue($@"{clsidPath}\{guid:B}", value, null)?.ToString();
                            itemText = StringResource.GetDirectString(itemText);
                            if (!string.IsNullOrWhiteSpace(itemText))
                                break;
                        }
                        if (!string.IsNullOrWhiteSpace(itemText))
                            break;
                    }
                }
                if (string.IsNullOrWhiteSpace(itemText))
                {
                    string filePath = GetFilePath(guid);
                    if (File.Exists(filePath))
                    {
                        itemText = FileVersionInfo.GetVersionInfo(filePath).FileDescription;
                        if (string.IsNullOrWhiteSpace(itemText))
                            itemText = Path.GetFileName(filePath);
                    }
                    else
                        itemText = null;
                }
                ItemTextDic.Add(guid, itemText);
            }
            return itemText;
        }

        public static Icon GetImage(Guid guid)
        {
            if (ItemImageDic.TryGetValue(guid, out Icon image))
                return image;
            IconLocation location = GetIconLocation(guid);
            string iconPath = location.IconPath;
            int iconIndex = location.IconIndex;
            if (iconPath == null && iconIndex == 0)
                image = AppImage.SystemFile;
            else if (Path.GetFileName(iconPath).ToLower() == "shell32.dll" && iconIndex == 0)
                image = AppImage.SystemFile;
            else
                image = IconResource.GetIcon(iconPath, iconIndex) ?? AppImage.SystemFile;
            ItemImageDic.Add(guid, image);
            return image;
        }

        public static IconLocation GetIconLocation(Guid guid)
        {
            IconLocation location = new IconLocation();
            if (guid.Equals(Guid.Empty))
                return location;
            if (IconLocationDic.ContainsKey(guid))
                location = IconLocationDic[guid];
            else
            {
                if (TryGetValue(guid, "Icon", out string value))
                {
                    value = GetAbsStr(guid, value, false);
                    int index = value.LastIndexOf(',');
                    if (int.TryParse(value.Substring(index + 1), out int iconIndex))
                    {
                        location.IconPath = value.Substring(0, index);
                        location.IconIndex = iconIndex;
                    }
                    else
                        location.IconPath = value;
                }
                else
                    location.IconPath = GetFilePath(guid);
                IconLocationDic.Add(guid, location);
            }
            return location;
        }

        public static string GetUwpName(Guid guid)
        {
            string uwpName = null;
            if (guid.Equals(Guid.Empty))
                return uwpName;
            if (UwpNameDic.ContainsKey(guid))
                uwpName = UwpNameDic[guid];
            //else
            //{
            //    TryGetValue(guid, "UwpName", out uwpName);
            //    UwpNameDic.Add(guid, uwpName);
            //}
            return uwpName;
        }

        private static bool TryGetValue(Guid guid, string key, out string value)
        {
            //用户自定义字典优先
            string section = guid.ToString();
            value = UserDic.GetValue(section, key);
            if (value != string.Empty)
                return true;
            if (WebDic.TryGetValue(section, key, out value))
                return true;
            if (AppDic.TryGetValue(section, key, out value))
                return true;
            return false;
        }
        private static string GetAbsStr(Guid guid, string relStr, bool isName)
        {
            string absStr = relStr;
            if (isName)
            {
                if (!absStr.StartsWith("@")) return absStr;
                else absStr = absStr.Substring(1);
                if (absStr.StartsWith("{*?ms-resource://") && absStr.EndsWith("}"))
                {
                    absStr = "@{" + UwpHelper.GetPackageName(GetUwpName(guid)) + absStr.Substring(2);
                    return absStr;
                }
            }

            string filePath = GetFilePath(guid);
            if (filePath == null) return relStr;
            string dirPath = Path.GetDirectoryName(filePath);
            if (absStr.StartsWith("*"))
            {
                absStr = filePath + absStr.Substring(1);
            }
            else if (absStr.StartsWith(".\\"))
            {
                absStr = dirPath + absStr.Substring(1);
            }
            else if (absStr.StartsWith("..\\"))
            {
                do
                {
                    dirPath = Path.GetDirectoryName(dirPath);
                    absStr = absStr.Substring(3);
                } while (absStr.StartsWith("..\\"));
                absStr = dirPath + "\\" + absStr;
            }
            if (isName) absStr = "@" + absStr;
            return absStr;
        }
    }

    struct IconLocation
    {
        public string IconPath { get; set; }
        public int IconIndex { get; set; }
        public string Tostring() => $"{IconPath},{IconIndex}";
    }
}
