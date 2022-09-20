using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Tools.Lib.MenuContext
{
    internal class RegTrustedInstaller
    {
        /// <summary>获取注册表项权限</summary>
        /// <remarks>将注册表项所有者改为当前管理员用户</remarks>
        /// <param name="regPath">要获取权限的注册表完整路径</param>
        public static void TakeRegKeyOwnerShip(string regPath)
        {
            if (string.IsNullOrEmpty(regPath))
                return;

            RegistryKey key = null;
            WindowsIdentity id = null;
            //利用试错判断是否有写入权限
            try
            {
                key = RegistryExtension.GetRegistryKey(regPath, true);
            }
            catch
            {
                try
                {
                    //获取当前用户的ID
                    id = WindowsIdentity.GetCurrent();

                    //添加TakeOwnership特权
                    bool flag = NativeMethod.TrySetPrivilege(NativeMethod.TakeOwnership, true);
                    if (!flag)
                        throw new PrivilegeNotHeldException(NativeMethod.TakeOwnership);

                    //添加恢复特权(必须这样做才能更改所有者)
                    flag = NativeMethod.TrySetPrivilege(NativeMethod.Restore, true);
                    if (!flag)
                        throw new PrivilegeNotHeldException(NativeMethod.Restore);

                    //打开没有权限的注册表路径
                    key = RegistryExtension.GetRegistryKey(regPath, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.TakeOwnership);

                    RegistrySecurity security = key.GetAccessControl(AccessControlSections.All);

                    //使进程用户成为所有者
                    security.SetOwner(id.User);
                    key.SetAccessControl(security);

                    //添加完全控制
                    RegistryAccessRule fullAccess = new RegistryAccessRule(id.User, RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit, PropagationFlags.None, AccessControlType.Allow);
                    security.AddAccessRule(fullAccess);
                    key.SetAccessControl(security);
                }
                catch { }
            }
            finally
            {
                key?.Close();
                id?.Dispose();
            }
        }


        public static void TakeRegTreeOwnerShip(string regPath)
        {
            if (string.IsNullOrEmpty(regPath))
                return;
            TakeRegKeyOwnerShip(regPath);
            try
            {
                using (RegistryKey key = RegistryExtension.GetRegistryKey(regPath))
                {
                    if (key == null)
                        return;
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        TakeRegTreeOwnerShip($@"{key.Name}\{subKeyName}");
                    }
                }
            }
            catch { }

        }
    }
}
