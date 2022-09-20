using System;

namespace Tools.Lib.MenuContext
{
    /// <summary>
    /// 注册表路径
    /// </summary>
    internal class RegistryPath
    {
        /// <summary>文件</summary>
        public readonly static string MenuPathFile = @"HKEY_CLASSES_ROOT\*";

        /// <summary>文件夹</summary>
        public readonly static string MenuPathFolder = @"HKEY_CLASSES_ROOT\Folder";

        /// <summary>目录</summary>
        public readonly static string MenuPathDirectory = @"HKEY_CLASSES_ROOT\Directory";

        /// <summary>目录背景</summary>
        public readonly static string MenuPathBackground = @"HKEY_CLASSES_ROOT\Directory\Background";

        /// <summary>桌面背景</summary>
        public readonly static string MenuPathDesktop = @"HKEY_CLASSES_ROOT\DesktopBackground";

        /// <summary>磁盘分区</summary>
        public readonly static string MenuPathDrive = @"HKEY_CLASSES_ROOT\Drive";

        /// <summary>所有对象</summary>
        public readonly static string MenuPathAllObjects = @"HKEY_CLASSES_ROOT\AllFilesystemObjects";

        /// <summary>此电脑</summary>
        public readonly static string MenuPathComputer = @"HKEY_CLASSES_ROOT\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}";

        /// <summary>回收站</summary>
        public readonly static string MenuPathRecycleBin = @"HKEY_CLASSES_ROOT\CLSID\{645FF040-5081-101B-9F08-00AA002F954E}";

        /// <summary>库</summary>
        public readonly static string MenuPathLibrary = @"HKEY_CLASSES_ROOT\LibraryFolder";

        /// <summary>库背景</summary>
        public readonly static string MenuPathLibraryBackground = @"HKEY_CLASSES_ROOT\LibraryFolder\Background";

        /// <summary>用户库</summary>
        public readonly static string MenuPathLibraryUser = @"HKEY_CLASSES_ROOT\UserLibraryFolder";

        /// <summary>UWP快捷方式</summary>
        public readonly static string MenuPathUWPlnk = @"HKEY_CLASSES_ROOT\Launcher.ImmersiveApplication";

        /// <summary>未知格式</summary>
        public readonly static string MenuPathUnknown = @"HKEY_CLASSES_ROOT\Unknown";

        /// <summary>系统扩展名注册表父路径</summary>
        public readonly static string SysFileassPath = @"HKEY_CLASSES_ROOT\SystemFileAssociations";

        /// <summary>上次打开的注册表项路径记录</summary>
        private readonly static string LASTKEYPATH = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Applets\Regedit";

        /// <summary>新建菜单选项列表</summary>
        public readonly static string ShellNewPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Discardable\PostSetup\ShellNew";

        /// <summary>Shell公共引用子菜单注册表项路径</summary>
        public readonly static string CommandStorePath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\CommandStore\shell";

        public readonly static string FileExtsPath = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts";


        public readonly static string HKCRClasses = @"HKEY_CURRENT_USER\SOFTWARE\Classes";


        public readonly static string HKLMClasses = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes";

        /// <summary>Icon</summary>
        public readonly static string ShellIconPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons";

        /// <summary>在新窗口中打开</summary>
        public readonly static string OpenInNewWindowPath = @"HKEY_CLASSES_ROOT\Folder\shell\opennewwindow";

        /// <summary>发送到</summary>
        public static readonly string SendToPath = Environment.ExpandEnvironmentVariables(@"%AppData%\Microsoft\Windows\SendTo");

        /// <summary>发送到</summary>
        public static readonly string DefaultSendToPath = Environment.ExpandEnvironmentVariables(@"%SystemDrive%\Users\Default\AppData\Roaming\Microsoft\Windows\SendTo");

        /// <summary>Win+X</summary>
        public static readonly string WinXPath = Environment.ExpandEnvironmentVariables(@"%LocalAppData%\Microsoft\Windows\WinX");
    }
}
