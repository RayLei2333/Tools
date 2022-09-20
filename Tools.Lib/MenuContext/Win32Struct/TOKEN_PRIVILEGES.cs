using System.Runtime.InteropServices;

namespace Tools.Lib.MenuContext.Win32Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_PRIVILEGES
    {
        public int PrivilegeCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public LUID_AND_ATTRIBUTES[] Privileges;
    }
}
