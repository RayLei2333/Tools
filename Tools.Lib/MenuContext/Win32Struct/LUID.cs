using System.Runtime.InteropServices;

namespace Tools.Lib.MenuContext.Win32Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LUID
    {
        public int lowPart;
        public int highPart;
    }
}
