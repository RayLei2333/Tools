using System;

namespace Tools.Lib.MenuContext.Win32Struct
{

    [Flags]
    internal enum PrivilegeAttributes
    {
        /// <summary>特权被禁用.</summary>
        Disabled = 0,
        /// <summary>默认特权.</summary>
        EnabledByDefault = 1,
        /// <summary>特权被激活.</summary>
        Enabled = 2,
        /// <summary>特权被废除.</summary>
        Removed = 4,
        /// <summary>用于访问对象或服务的特权.</summary>
        UsedForAccess = -2147483648
    }
}
