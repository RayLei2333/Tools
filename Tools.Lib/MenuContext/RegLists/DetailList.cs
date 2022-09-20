using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class DetailList : RegistryList
    {
        public static readonly Dictionary<Guid, GroupItem> DetailedEditGuidItem = new Dictionary<Guid, GroupItem>();

        public Guid Guid { get; set; }

        public GroupItem GroupItem { get; set; }

        public DetailList(Guid guid)
        {
            this.Guid = guid;
        }

        public override void LoadItems()
        {
            if(DetailedEditGuidItem.ContainsKey(this.Guid))
            {
                this.GroupItem = DetailedEditGuidItem[this.Guid];
                return;
            }

            XmlNode groupNode = XmlDicHelper.DetailedEditGuidDic[this.Guid];
            if (groupNode == null)
                return;

            bool isIniGroup = groupNode.SelectSingleNode("IsIniGroup") != null;
            this.GroupItem = this.CreateGroup(groupNode);


            foreach (XmlNode itemNode in groupNode.SelectNodes("Item"))
            {
                if (!XmlDicHelper.JudgeOSVersion(itemNode))
                    continue;

                string itemText = StringResource.GetDirectString(itemNode.SelectSingleNode("Text").GetAttributeString("Value"));

                RegistryItem registryItem;
                if (isIniGroup)
                {
                    IniItem item = this.IniGroup(itemNode);
                   // item.ItemText = itemText;

                    if (string.IsNullOrWhiteSpace(item.Path))
                        item.Path = this.GroupItem.Path;
                    registryItem = item;

                }
                else
                {
                    RegItem item = this.RegGroup(this.GroupItem.Path, itemNode);
                    //item.ItemText = itemText;
                    registryItem = item;
                }
                registryItem.ItemText = itemText;

                this.Items.Add(registryItem);

            }

            this.GroupItem.Items = this.Items;

            DetailedEditGuidItem.Add(this.Guid, this.GroupItem);
        }

        private GroupItem CreateGroup(XmlNode groupNode)
        {
            bool isIniGroup = groupNode.SelectSingleNode("IsIniGroup") != null;
            string attribute = isIniGroup ? "FilePath" : "RegPath";
            PathType pathType = isIniGroup ? PathType.File : PathType.Registry;
            string groupPath = groupNode.SelectSingleNode(attribute)?.InnerText;
            GroupItem groupItem = new GroupItem(groupPath, pathType)
            {
                ItemText = StringResource.GetDirectString(groupNode.SelectSingleNode("Text").GetAttributeString("Value")),
                ItemIcon = GuidInfo.GetImage(this.Guid)
            };
            return groupItem;
        }

        private IniItem IniGroup(XmlNode itemNode)
        {
            XmlNode ruleNode = itemNode.SelectSingleNode("Rule");
            string path = ruleNode.GetAttributeString("FilePath");
            IniItem item;

            if (itemNode.SelectSingleNode("IsNumberItem") != null)
                item = this.IniNumberItem(path, ruleNode);
            else if (itemNode.SelectSingleNode("IsStringItem") != null)
                item = new IniStringItem(path);
            else
                item = this.IniVisbleItem(path, ruleNode);

            item.Section = ruleNode.GetAttributeString("Section");
            item.KeyName = ruleNode.GetAttributeString("KeyName");
            return item;

        }

        private IniItem IniNumberItem(string path, XmlNode ruleNode)
        {
            IniNumberItem item = new IniNumberItem(path)
            {
                DefaultValue = ruleNode.Attributes["Default"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Default")) : 0,
                MinValue = ruleNode.Attributes["Min"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Min")) : int.MinValue,
                MaxValue = ruleNode.Attributes["Max"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Max")) : int.MaxValue
            };

            return item;
        }

        private IniItem IniVisbleItem(string path, XmlNode ruleNode)
        {
            IniVisbleItem item = new IniVisbleItem(path)
            {
                TurnOnValue = ruleNode.Attributes["On"] == null ? ruleNode.GetAttributeString("On") : null,
                TurnOffValue = ruleNode.Attributes["Off"] == null ? ruleNode.GetAttributeString("Off") : null,
            };
            return item;

        }

        private RegItem RegGroup(string groupPath, XmlNode itemNode)
        {
            XmlNode ruleNode = itemNode.SelectSingleNode("Rule");
            string path = ruleNode.Attributes["RegPath"]?.Value;
            if (string.IsNullOrEmpty(path))
                path = groupPath;
            else if (path.StartsWith("\\"))
                path = groupPath + path;

            RegItem item;
            if (itemNode.SelectSingleNode("IsNumberItem") != null)
                item = this.RegNumberItem(path, ruleNode);
            else if (itemNode.SelectSingleNode("IsStringItem") != null)
                item = new RegStringItem(path)
                {
                    ValueName = ruleNode.Attributes["ValueName"].Value,
                };
            else
                item = this.RegVisbleItem(path, itemNode);

            return item;
        }

        private RegItem RegNumberItem(string path, XmlNode ruleNode)
        {
            RegNumberItem item = new RegNumberItem(path)
            {
                ValueName = ruleNode.Attributes["ValueName"].Value,
                ValueKind = XmlDicHelper.GetValueKind(ruleNode.GetAttributeString("ValueKind"), RegistryValueKind.DWord),
                DefaultValue = ruleNode.Attributes["Default"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Default")) : 0,
                MinValue = ruleNode.Attributes["Min"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Min")) : int.MinValue,
                MaxValue = ruleNode.Attributes["Max"] == null ? Convert.ToInt32(ruleNode.GetAttributeString("Max")) : int.MaxValue
            };

            return item;
        }

        private RegItem RegVisbleItem(string path, XmlNode itemNode)
        {
            XmlNodeList ruleNodeList = itemNode.SelectNodes("Rule");
            //RegVisbleItem
            var rules = new RegVisbleItem.RegRule[ruleNodeList.Count];
            for (int i = 0; i < ruleNodeList.Count; i++)
            {
                XmlNode ruleNode = ruleNodeList[i];
                rules[i] = new RegVisbleItem.RegRule()
                {
                    Path = path,
                    ValueName = ruleNode.Attributes["ValueName"].Value,
                    ValueKind = XmlDicHelper.GetValueKind(ruleNode.Attributes["ValueKind"]?.Value, RegistryValueKind.DWord)
                };
                string turnOn = ruleNode.Attributes["On"] != null ? ruleNode.Attributes["On"].Value : null;
                string turnOff = ruleNode.Attributes["Off"] != null ? ruleNode.Attributes["Off"].Value : null;
                switch (rules[i].ValueKind)
                {
                    case RegistryValueKind.Binary:
                        rules[i].TurnOnValue = turnOn != null ? XmlDicHelper.ConvertToBinary(turnOn) : null;
                        rules[i].TurnOffValue = turnOff != null ? XmlDicHelper.ConvertToBinary(turnOff) : null;
                        break;
                    case RegistryValueKind.DWord:
                        if (turnOn == null)
                            rules[i].TurnOnValue = null;
                        else
                            rules[i].TurnOnValue = Convert.ToInt32(turnOn);
                        if (turnOff == null)
                            rules[i].TurnOffValue = null;
                        else
                            rules[i].TurnOffValue = Convert.ToInt32(turnOff);
                        break;
                    default:
                        rules[i].TurnOnValue = turnOn;
                        rules[i].TurnOffValue = turnOff;
                        break;
                }
            }

            RegVisbleItem item = new RegVisbleItem(rules);
            return item;

        }
    }
}
