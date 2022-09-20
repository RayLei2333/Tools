using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class ShellItem : RegItem
    {
        /// <summary>Shell类型菜单特殊注册表项名默认名称</summary>
        /// <remarks>字符串资源在windows.storage.dll里面</remarks>
        private static readonly Dictionary<string, int> _DefaultNameIndexs
            = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            {
                { "open", 8496 }, { "edit", 8516 }, { "print", 8497 }, { "find", 8503 },
                { "play", 8498 }, { "runas", 8505 }, { "explore", 8502 }, { "preview", 8499 }
            };

        private string _CommandPath => $@"{this.Path}\command";
        private bool _IsOpenItem => this.KeyName.ToLower() == "open";
        private bool _HasIcon => !string.IsNullOrWhiteSpace(this.IconLocation) || this._HasLUAShield;
        private bool _HasLUAShield
        {
            get => Registry.GetValue(this.Path, "HasLUAShield", null) != null;
            set
            {
                if (value)
                    Registry.SetValue(this.Path, "HasLUAShield", "");
                else
                    RegistryExtension.DeleteValue(this.Path, "HasLUAShield");
            }
        }
        private bool _ShowAsDisabledIfHidden
        {
            get => Registry.GetValue(this.Path, "ShowAsDisabledIfHidden", null) != null;
            set
            {
                //if (!TryProtectOpenItem()) 
                //    return;
                if (value)
                    Registry.SetValue(this.Path, "ShowAsDisabledIfHidden", "");
                else
                    RegistryExtension.DeleteValue(this.Path, "ShowAsDisabledIfHidden");
                if (value && !this.IsEnable)
                    this.IsEnable = false;
            }
        }

        public override bool IsEnable
        {
            get
            {
                if (WinOsVersion.Current >= WinOsVersion.Win10_1703)
                {
                    //HideBasedOnVelocityId键值仅适用于Win10系统1703以上版本
                    if (Convert.ToInt32(Registry.GetValue(this.Path, "HideBasedOnVelocityId", 0)) == 0x639bc8)
                        return false;
                }

                if (!IsSubItem)
                {
                    //LegacyDisable和ProgrammaticAccessOnly键值不适用于子菜单
                    if (Registry.GetValue(this.Path, "LegacyDisable", null) != null)
                        return false;
                    if (Registry.GetValue(this.Path, "ProgrammaticAccessOnly", null) != null)
                        return false;

                    //CommandFlags键值不适用于Vista系统，子菜单中该键值我用来做分割线键值
                    if (WinOsVersion.Current > WinOsVersion.Vista &&
                        Convert.ToInt32(Registry.GetValue(this.Path, "CommandFlags", 0)) % 16 >= 8)
                        return false;

                }

                return true;

            }
            set
            {
                void DeleteSomeValues()
                {
                    RegistryExtension.DeleteValue(this.Path, "LegacyDisable");
                    RegistryExtension.DeleteValue(this.Path, "ProgrammaticAccessOnly");
                    if (WinOsVersion.Current > WinOsVersion.Vista &&
                        Convert.ToInt32(Registry.GetValue(this.Path, "CommandFlags", 0)) % 16 >= 8)
                    {
                        RegistryExtension.DeleteValue(this.Path, "CommandFlags");
                    }
                };

                if (value)
                {
                    RegistryExtension.DeleteValue(this.Path, "HideBasedOnVelocityId");
                    DeleteSomeValues();
                }
                else
                {
                    if (WinOsVersion.Current >= WinOsVersion.Win10_1703)
                        Registry.SetValue(this.Path, "HideBasedOnVelocityId", 0x639bc8);
                    else
                    {
                        if (IsSubItem)
                            return;
                    }
                    if (!IsSubItem)
                    {
                        //当LegaryDisable键值作用于文件夹-"在新窗口中打开"时
                        //会导致点击任务栏explorer图标和 Win+E 快捷键错误访问
                        if (!this.Path.StartsWith(RegistryPath.OpenInNewWindowPath, StringComparison.OrdinalIgnoreCase))
                            Registry.SetValue(this.Path, "LegacyDisable", "");

                        Registry.SetValue(this.Path, "ProgrammaticAccessOnly", "");
                    }

                    if (this._ShowAsDisabledIfHidden)
                        DeleteSomeValues();

                }
            }
        }

        public override string ItemText
        {
            get
            {
                string name;
                //菜单名称优先级别：MUIVerb > 默认值 > 特殊键值名 > 项名
                List<string> valueNames = new List<string> { "MUIVerb" };
                if (!this.IsMultiItem)
                    valueNames.Add("");//多级母菜单不支持使用默认值作为名称
                foreach (string valueName in valueNames)
                {
                    name = Registry.GetValue(this.Path, valueName, null)?.ToString();
                    name = StringResource.GetDirectString(name);
                    if (!string.IsNullOrEmpty(name))
                        return name;
                }
                if (_DefaultNameIndexs.TryGetValue(this.KeyName, out int index))
                {
                    name = $"@windows.storage.dll,-{index}";
                    name = StringResource.GetDirectString(name);
                    if (!string.IsNullOrEmpty(name))
                        return name;
                }

                return KeyName;
            }
            set
            {
                Registry.SetValue(this.Path, "MUIVerb", value);
            }
        }

        public override Guid Guid
        {
            get
            {
                Dictionary<string, string> keyValues = new Dictionary<string, string>
                {
                    { this._CommandPath , "DelegateExecute" },
                    { $@"{this.Path}\DropTarget" , "CLSID" },
                    { this.Path , "ExplorerCommandHandler" },
                };

                foreach (var item in keyValues)
                {
                    string value = Registry.GetValue(item.Key, item.Value, null)?.ToString();
                    if (Guid.TryParse(value, out Guid guid))
                        return guid;
                }
                return Guid.Empty;
            }
        }

        public override string ItemFilePath => GuidInfo.GetFilePath(Guid) ?? ObjectPath.ExtractFilePath(ItemCommand);

        public override Icon ItemIcon
        {
            get
            {
                //菜单图标优先级别：Icon > HasLUAShield
                //只要有Icon键值，不论数据是否为空，HasLUAShield键值就不起作用
                Icon icon;
                string iconPath;
                int iconIndex;
                if (this.IconLocation != null)
                {
                    icon = IconResource.GetIcon(this.IconLocation, out iconPath, out iconIndex);
                    if (icon == null && System.IO.Path.GetExtension(iconPath)?.ToLower() == ".exe")//文件不存在，或为没有图标的exe文件
                        icon = IconResource.GetIcon(iconPath = "imageres.dll", iconIndex = -15);//不含图标的默认exe图标
                }

                else if (this._HasLUAShield)
                    icon = IconResource.GetIcon(iconPath = "imageres.dll", iconIndex = -78);//管理员小盾牌图标
                else
                    icon = IconResource.GetIcon(iconPath = ItemFilePath, iconIndex = 0);//文件第一个图标
                if (icon == null)
                    icon = IconResource.GetExtensionIcon(iconPath = ItemFilePath)//文件类型图标
                        ?? IconResource.GetIcon(iconPath = "imageres.dll", iconIndex = -2);//图标资源不存在，白纸图标
                IconPath = iconPath;
                IconIndex = iconIndex;
                return icon;

            }
            set => base.ItemIcon = value;
        }

        protected virtual bool IsSubItem => false;
        public virtual string IconLocation
        {
            get => Registry.GetValue(this.Path, "Icon", null)?.ToString();
            set
            {
                if (value != null) Registry.SetValue(this.Path, "Icon", value);
                else RegistryExtension.DeleteValue(this.Path, "Icon");
            }
        }

        public virtual string ItemCommand
        {
            get
            {
                if (this.IsMultiItem)
                    return null;
                else
                    return Registry.GetValue(this._CommandPath, "", null)?.ToString();
            }
            set
            {
                Registry.SetValue(this._CommandPath, "", value);
            }
        }

        public bool IsMultiItem
        {
            get
            {
                object value = Registry.GetValue(this.Path, "SubCommands", null);
                if (value != null)
                    return true;
                value = Registry.GetValue(this.Path, "ExtendedSubCommandsKey", null);
                if (!string.IsNullOrEmpty(value?.ToString()))
                    return true;
                return false;
            }
        }

        public ShellItem(string path) : base(path)
        {
        }
    }
}
