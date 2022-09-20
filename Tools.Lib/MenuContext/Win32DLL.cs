using System;
using System.Runtime.InteropServices;
using System.Text;
using Tools.Lib.MenuContext.Win32Struct;

namespace Tools.Lib.MenuContext
{
    internal static class Win32DLL
    {
        #region user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadImage(IntPtr hInst, string lpFileName, uint uType, int cx, int cy, uint fuLoad);
        #endregion

        #region kernel32.dll
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hLibModule);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetLastError();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetCurrentProcess();
        #endregion

        #region shell32.dll


        [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, FileInfoFlags uFlags);


        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern int ExtractIconEx(string lpFileName, int nIconIndex, IntPtr[] phIconLarge, IntPtr[] phIconSmall, uint nIcons);
        #endregion

        #region advapi32.dll
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AdjustTokenPrivileges([In] IntPtr accessTokenHandle, [In] bool disableAllPrivileges,
            [In] ref TOKEN_PRIVILEGES newState, [In] int bufferLength, [In, Out] ref TOKEN_PRIVILEGES previousState, [In, Out] ref int returnLength);

        [DllImport("advapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeValue([In] string systemName, [In] string name, [In, Out] ref LUID luid);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken([In] IntPtr processHandle, [In] TokenAccessRights desiredAccess, [In, Out] ref IntPtr tokenHandle);
        #endregion

        #region shlwapi.dll
        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode,
           ExactSpelling = true, SetLastError = false, ThrowOnUnmappableChar = true)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, uint cchOutBuf, IntPtr ppvReserved);

        [DllImport("shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint AssocQueryString(AssocF flags, AssocStr str, string pszAssoc, string pszExtra, [Out] StringBuilder pszOut, ref uint pcchOut);
        #endregion
    }
}
