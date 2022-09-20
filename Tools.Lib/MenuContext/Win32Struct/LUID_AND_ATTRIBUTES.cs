using System.Runtime.InteropServices;

namespace Tools.Lib.MenuContext.Win32Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID_AND_ATTRIBUTES
    {
        public LUID Luid;
        public int Attributes;
    }
}
