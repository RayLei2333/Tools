using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class RegVisbleItem : RegItem, IValueItem<bool>, IRestartExplorer
    {
        public struct RegRule
        {
            public string Path { get; set; }
            public string ValueName { get; set; }
            public RegistryValueKind ValueKind { get; set; }
            public object TurnOnValue { get; set; }
            public object TurnOffValue { get; set; }

            public RegRule(string path, string valueName, object turnOnValue,
                object turnOffValue, RegistryValueKind valueKind = RegistryValueKind.DWord)
            {
                this.Path = path;
                this.ValueName = valueName;
                this.TurnOnValue = turnOnValue;
                this.TurnOffValue = turnOffValue;
                this.ValueKind = valueKind;
            }
        }

        public struct RuleAndInfo
        {
            public RegRule[] Rules { get; set; }

            public string ItemText { get; set; }

            public Icon ItemIcon { get; set; }
        }

        public RegRule[] Rules { get; set; }

        public string ValueName => Rules[0].ValueName;

        public bool ItemValue
        {
            get
            {
                for (int i = 0; i < Rules.Length; i++)
                {
                    RegRule rule = Rules[i];
                    using (RegistryKey key = RegistryExtension.GetRegistryKey(rule.Path))
                    {
                        string value = key?.GetValue(rule.ValueName)?.ToString().ToLower();
                        string turnOnValue = rule.TurnOnValue?.ToString().ToLower();
                        string turnOffValue = rule.TurnOffValue?.ToString().ToLower();
                        if (value == null || key.GetValueKind(rule.ValueName) != rule.ValueKind)
                        {
                            if (i < Rules.Length - 1)
                                continue;
                        }
                        if (value == turnOnValue)
                            return true;
                        if (value == turnOffValue)
                            return false;
                    }
                }
                return true;
            }
            set
            {
                foreach (RegRule rule in Rules)
                {
                    object data = value ? rule.TurnOnValue : rule.TurnOffValue;
                    if (data != null)
                    {
                        Registry.SetValue(rule.Path, rule.ValueName, data, rule.ValueKind);
                    }
                    else
                    {
                        RegistryExtension.DeleteValue(rule.Path, rule.ValueName);
                    }
                }
            }
        }

        public override bool IsEnable
        {
            get => this.ItemValue;
            set
            {
                this.ItemValue = value;
            }
        }

        public RegVisbleItem(RegRule[] rules) : base(null)
        {
            this.Rules = rules;
            this.Path = rules[0].Path;
        }

        public RegVisbleItem(string path) : base(path)
        {
        }

        public RegVisbleItem(RuleAndInfo ruleAndInfo) : this(ruleAndInfo.Rules)
        {
            this.ItemText = ruleAndInfo.ItemText;
            this.ItemIcon = ruleAndInfo.ItemIcon;
        }

        const string LM_SMWCPE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string CU_SMWCPE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer";
        const string LM_SMWCE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
        const string CU_SMWCE = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer";
        const string LM_SPMWE = @"HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\Explorer";
        const string CU_SPMWE = @"HKEY_CURRENT_USER\Software\Policies\Microsoft\Windows\Explorer";

        public static readonly RuleAndInfo CustomFolder = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoCustomizeThisFolder", null, 1),
                new RegRule(LM_SMWCPE, "NoCustomizeWebView", null, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeThisFolder", null, 1),
                new RegRule(CU_SMWCPE, "NoCustomizeWebView", null, 1)
            },
            ItemIcon = AppImage.Folder,
            ItemText = "Custom Folder"//AppString.Other.CustomFolder
        };

        public static readonly RuleAndInfo NetworkDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoNetConnectDisconnect", null, 1),
                new RegRule(CU_SMWCPE, "NoNetConnectDisconnect", null, 1)
            },
            ItemText = $"{StringResource.GetDirectString("@AppResolver.dll,-8556")} && {StringResource.GetDirectString("@AppResolver.dll,-8557")}",
            ItemIcon = AppImage.NetworkDrive
        };

        public static readonly RuleAndInfo RecycleBinProperties = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoPropertiesRecycleBin", null, 1),
                new RegRule(CU_SMWCPE, "NoPropertiesRecycleBin", null, 1)
            },
            ItemText = StringResource.GetDirectString("@AppResolver.dll,-8553"),
            ItemIcon = AppImage.RecycleBin
        };

        public static readonly RuleAndInfo SendToDrive = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCPE, "NoDrivesInSendToMenu", null, 1),
                new RegRule(CU_SMWCPE, "NoDrivesInSendToMenu", null, 1)
            },
            ItemText = StringResource.GetDirectString("@shell32.dll,-9309"),
            ItemIcon = AppImage.Drive
        };

        public static readonly RuleAndInfo DeferBuildSendTo = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SMWCE, "DelaySendToMenuBuild", null, 1),
                new RegRule(CU_SMWCE, "DelaySendToMenuBuild", null, 1)
            },
            ItemText = "快速构建发送到子菜单",
            ItemIcon = AppImage.SendTo
        };

        public static readonly RuleAndInfo UseStoreOpenWith = new RuleAndInfo
        {
            Rules = new[] {
                new RegRule(LM_SPMWE, "NoUseStoreOpenWith", null, 1),
                new RegRule(CU_SPMWE, "NoUseStoreOpenWith", null, 1)
            },
            ItemText = StringResource.GetDirectString("@shell32.dll,-5383"),
            ItemIcon = AppImage.MicrosoftStore
        };
    }
}
