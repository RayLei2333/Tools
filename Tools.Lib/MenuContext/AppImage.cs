using System.Drawing;

namespace Tools.Lib.MenuContext
{
    internal class AppImage
    {
        ///<summary>Microsoft Store</summary>
        public static Icon MicrosoftStore = Icon.FromHandle(AppConfig.MicrosoftStore.GetHicon());

        ///<summary>系统文件</summary>
        public static readonly Icon SystemFile = GetIconImage(-67);
        ///<summary>资源不存在</summary>
        public static readonly Icon NotFound = GetIconImage(-2);
        ///<summary>管理员小盾牌</summary>
        public static readonly Icon Shield = GetIconImage(-78);
        ///<summary>网络驱动器</summary>
        public static readonly Icon NetworkDrive = GetIconImage(-33);
        ///<summary>发送到</summary>
        public static readonly Icon SendTo = GetIconImage(-185);
        ///<summary>回收站</summary>
        public static readonly Icon RecycleBin = GetIconImage(-55);
        ///<summary>磁盘</summary>
        public static readonly Icon Drive = GetIconImage(-30);
        ///<summary>文件</summary>
        public static readonly Icon File = GetIconImage(-19);
        ///<summary>文件夹</summary>
        public static readonly Icon Folder = GetIconImage(-3);
        ///<summary>目录</summary>
        public static readonly Icon Directory = GetIconImage(-162);
        ///<summary>所有对象</summary>
        public static readonly Icon AllObjects = GetIconImage(-117);
        ///<summary>锁定</summary>
        public static readonly Icon Lock = GetIconImage(-59);
        ///<summary>快捷方式图标</summary>
        public static readonly Icon LnkFile = GetIconImage(-16769, "shell32.dll");
        ///<summary>重启Explorer</summary>
        public static readonly Icon RestartExplorer = GetIconImage(238, "shell32.dll");
        ///<summary>资源管理器</summary>
        public static readonly Icon Explorer = GetIconImage(0, "explorer.exe");


        private static Icon GetIconImage(int iconIndex, string dllName = "imageres.dll")
        {
            using (Icon icon = IconResource.GetIcon(dllName, iconIndex))
                return (Icon)icon.Clone();
        }
    }
}
