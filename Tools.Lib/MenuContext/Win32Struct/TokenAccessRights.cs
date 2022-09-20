using System;

namespace Tools.Lib.MenuContext.Win32Struct
{
    [Flags]
    internal enum TokenAccessRights
    {
        /// <summary>向进程附加主令牌的权限.</summary>
        AssignPrimary = 0,
        /// <summary>复制访问令牌的权利.</summary>
        Duplicate = 1,
        /// <summary>向进程附加模拟访问令牌的权限.</summary>
        Impersonate = 4,
        /// <summary>查询访问令牌的权利.</summary>
        Query = 8,
        /// <summary>有权查询访问令牌的来源.</summary>
        QuerySource = 16,
        /// <summary>启用或禁用访问令牌中的特权的权限.</summary>
        AdjustPrivileges = 32,
        /// <summary>调整访问令牌中的组属性的权限.</summary>
        AdjustGroups = 64,
        /// <summary>更改访问令牌的默认所有者、主组或DACL的权限.</summary>
        AdjustDefault = 128,
        /// <summary>正确调整访问令牌的会话ID.</summary>
        AdjustSessionId = 256,
        /// <summary>为令牌组合所有可能的访问权限.</summary>
        AllAccess = AccessTypeMasks.StandardRightsRequired | AssignPrimary | Duplicate | Impersonate
                    | Query | QuerySource | AdjustPrivileges | AdjustGroups | AdjustDefault | AdjustSessionId,
        /// <summary>结合需要阅读的标准权利</summary>
        Read = AccessTypeMasks.StandardRightsRead | Query,
        /// <summary>组合了写入所需的标准权限</summary>
        Write = AccessTypeMasks.StandardRightsWrite | AdjustPrivileges | AdjustGroups | AdjustDefault,
        /// <summary>合并执行所需的标准权限</summary>
        Execute = AccessTypeMasks.StandardRightsExecute | Impersonate
    }
}
