using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.Win32Struct;

namespace Tools.Lib.MenuContext
{
    internal static class IconResource
    {
        /// <summary>获取文件类型的关联图标</summary>
        /// <param name="extension">文件类型的扩展名，如.txt</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetExtensionIcon(string extension)
        {
            FileInfoFlags flags = FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_LARGEICON | FileInfoFlags.SHGFI_USEFILEATTRIBUTES;
            return GetIcon(extension, flags);
        }

        /// <summary>获取文件夹、磁盘驱动器的图标</summary>
        /// <param name="folderPath">文件夹或磁盘驱动器路径</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetFolderIcon(string folderPath)
        {
            FileInfoFlags flags = FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_LARGEICON;
            return GetIcon(folderPath, flags);
        }

        /// <summary>根据文件信息标志提取指定文件路径的图标</summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="flags">文件信息标志</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetIcon(string filePath, FileInfoFlags flags)
        {
            SHFILEINFO info = new SHFILEINFO();
            IntPtr hInfo = Win32DLL.SHGetFileInfo(filePath, 0, ref info, (uint)Marshal.SizeOf(info), flags);
            if (hInfo.Equals(IntPtr.Zero)) return null;
            System.Drawing.Icon icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(info.hIcon).Clone();
            Win32DLL.DestroyIcon(info.hIcon); //释放资源
            return icon;
        }

        /// <summary>获取指定位置的图标</summary>
        /// <param name="iconLocation">图标位置</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetIcon(string iconLocation)
        {
            return GetIcon(iconLocation, out _, out _);
        }

        /// <summary>获取指定位置的图标</summary>
        /// <param name="iconLocation">图标位置</param>
        /// <param name="iconPath">返回图标文件路径</param>
        /// <param name="iconIndex">返回图标索引</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetIcon(string iconLocation, out string iconPath, out int iconIndex)
        {
            iconIndex = 0;
            iconPath = null;
            if (string.IsNullOrWhiteSpace(iconLocation))
                return null;
            iconLocation = Environment.ExpandEnvironmentVariables(iconLocation).Replace("\"", "");
            int index = iconLocation.LastIndexOf(',');
            if (index == -1)
                iconPath = iconLocation;
            else
            {
                if (File.Exists(iconLocation))
                    iconPath = iconLocation;
                else
                {
                    bool flag = int.TryParse(iconLocation.Substring(index + 1), out iconIndex);
                    iconPath = flag ? iconLocation.Substring(0, index) : null;
                }
            }
            return GetIcon(iconPath, iconIndex);
        }

        /// <summary>获取指定文件中指定索引的图标</summary>
        /// <param name="iconPath">图标文件路径</param>
        /// <param name="iconIndex">图标索引</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetIcon(string iconPath, int iconIndex)
        {
            System.Drawing.Icon icon = null;
            if (string.IsNullOrWhiteSpace(iconPath))
                return icon;
            iconPath = Environment.ExpandEnvironmentVariables(iconPath).Replace("\"", "");

            if (Path.GetFileName(iconPath).ToLower() == "shell32.dll")
            {
                iconPath = "shell32.dll";//系统强制文件重定向
                icon = GetReplacedShellIcon(iconIndex);//注册表图标重定向
                if (icon != null)
                    return icon;
            }

            IntPtr hInst = IntPtr.Zero;
            IntPtr[] hIcons = new[] { IntPtr.Zero };
            //iconIndex为负数就是指定资源标识符, 为正数就是该图标在资源文件中的顺序序号, 为-1时不能使用ExtractIconEx提取图标
            if (iconIndex == -1)
            {
                hInst = Win32DLL.LoadLibrary(iconPath);
                hIcons[0] = Win32DLL.LoadImage(hInst, "#1", 1, 0, 0, 0);
                //hIcons[0] = ResourceDLL.LoadImage(hInst, "#1", 1, SystemInformation.IconSize.Width, SystemInformation.IconSize.Height, 0);
            }
            else
                Win32DLL.ExtractIconEx(iconPath, iconIndex, hIcons, null, 1);

            try
            {
                icon = (System.Drawing.Icon)System.Drawing.Icon.FromHandle(hIcons[0]).Clone();
            }
            catch
            {
                icon = null;
            }
            finally
            {
                Win32DLL.DestroyIcon(hIcons[0]);
                Win32DLL.FreeLibrary(hInst);
            }//释放资源
            return icon;
        }


        /// <summary>获取shell32.dll中的图标被替换后的图标</summary>
        /// <param name="iconIndex">图标索引</param>
        /// <returns>获取到的图标</returns>
        public static System.Drawing.Icon GetReplacedShellIcon(int iconIndex)
        {
            string iconLocation = Registry.GetValue(RegistryPath.ShellIconPath, iconIndex.ToString(), null)?.ToString();
            if (iconLocation != null) 
                return GetIcon(iconLocation) ?? GetIcon("imageres.dll", 2);
            else return null;
        }
    }
}
