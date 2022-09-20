using System;
using Tools.Lib.MenuContext.Win32Struct;

namespace Tools.Lib.MenuContext
{
    internal static class NativeMethod
    {
        public const string TakeOwnership = "SeTakeOwnershipPrivilege";
        public const string Restore = "SeRestorePrivilege";

        public static bool TrySetPrivilege(string sPrivilege, bool enablePrivilege)
        {
            bool blRc;
            TOKEN_PRIVILEGES newTP = new TOKEN_PRIVILEGES();
            TOKEN_PRIVILEGES oldTP = new TOKEN_PRIVILEGES();
            LUID luid = new LUID();
            int retrunLength = 0;
            IntPtr processToken = IntPtr.Zero;

            //本地进程令牌恢复
            blRc = Win32DLL.OpenProcessToken(Win32DLL.GetCurrentProcess(), TokenAccessRights.AllAccess, ref processToken);
            if (blRc == false) return false;

            //恢复特权的唯一标识符空间
            blRc = Win32DLL.LookupPrivilegeValue(null, sPrivilege, ref luid);
            if (blRc == false) return false;

            //建立或取消特权
            newTP.PrivilegeCount = 1;
            newTP.Privileges = new LUID_AND_ATTRIBUTES[64];
            newTP.Privileges[0].Luid = luid;

            if (enablePrivilege) newTP.Privileges[0].Attributes = (int)PrivilegeAttributes.Enabled;
            else newTP.Privileges[0].Attributes = (int)PrivilegeAttributes.Disabled;

            oldTP.PrivilegeCount = 64;
            oldTP.Privileges = new LUID_AND_ATTRIBUTES[64];
            blRc = Win32DLL.AdjustTokenPrivileges(processToken, false, ref newTP, 16, ref oldTP, ref retrunLength);

            if (blRc == false)
            {
                Win32DLL.GetLastError();
                return false;
            }
            return true;
        }
    }
}
