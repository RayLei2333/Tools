using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext
{
    public static class XmlDicHelper
    {
        public static readonly Dictionary<Guid, XmlNode> DetailedEditGuidDic = new Dictionary<Guid, XmlNode>();
        //private static readonly Dictionary<Guid, List<GroupItem>> DetailedEditGuidItem = new Dictionary<Guid, List<GroupItem>>();


        public static void RelodDics()
        {
            DetailedEditGuidDic.Clear();
            //DetailedEditGuidItem.Clear();
            LoadDic(DetailedEditGuidDic, AppConfig.DetailedItemDic);
        }

        private static void LoadDic(Dictionary<Guid, XmlNode> dic, string path)
        {
            //if (!File.Exists(path))
            //    return;
            XmlDocument doc = new XmlDocument();
            //Encoding encoding = EncodingType.GetType(path);
            //string xml = File.ReadAllText(path, encoding).Trim();
            //Encoding.Unicode
            doc.LoadXml(path);
            if (doc.DocumentElement == null)
                return;
            XmlNodeList xnl = doc.DocumentElement?.SelectNodes("Group");
            foreach (XmlNode xmlNode in xnl)
            {
                XmlNodeList guidNodeList = xmlNode.SelectNodes("Guid");
                foreach (XmlNode guidNode in guidNodeList)
                {
                    bool result = Guid.TryParse(guidNode.InnerText, out Guid guid);
                    if (result)
                        dic.Add(guid, xmlNode);
                }
            }
        }

        public static bool JudgeCulture(XmlNode itemXN)
        {
            //return true;//测试用
            string culture = itemXN.SelectSingleNode("Culture")?.InnerText;
            if (string.IsNullOrEmpty(culture)) 
                return true;
            //if (culture.Equals(AppConfig.Language, StringComparison.OrdinalIgnoreCase)) return true;
            if (culture.Equals(CultureInfo.CurrentUICulture.Name, StringComparison.OrdinalIgnoreCase)) 
                return true;
            return false;
        }

        public static bool JudgeOSVersion(XmlNode itemXN)
        {
            bool JudgeOne(XmlNode osXN)
            {
                Version ver = new Version(osXN.InnerText);
                Version osVer = Environment.OSVersion.Version;
                int compare = osVer.CompareTo(ver);
                string symbol = ((XmlElement)osXN).GetAttribute("Compare");
                switch (symbol)
                {
                    case ">":
                        return compare > 0;
                    case "<":
                        return compare < 0;
                    case "=":
                        return compare == 0;
                    case ">=":
                        return compare >= 0;
                    case "<=":
                        return compare <= 0;
                    default:
                        return true;
                }
            }

            foreach (XmlNode osXN in itemXN.SelectNodes("OSVersion"))
            {
                if (!JudgeOne(osXN))
                    return false;
            }
            return true;

        }

        public static RegistryValueKind GetValueKind(string type, RegistryValueKind defaultKind)
        {
            if (string.IsNullOrWhiteSpace(type))
                return defaultKind;
            switch (type.ToUpper())
            {
                case "REG_SZ":
                    return RegistryValueKind.String;
                case "REG_BINARY":
                    return RegistryValueKind.Binary;
                case "REG_DWORD":
                    return RegistryValueKind.DWord;
                case "REG_QWORD":
                    return RegistryValueKind.QWord;
                case "REG_MULTI_SZ":
                    return RegistryValueKind.MultiString;
                case "REG_EXPAND_SZ":
                    return RegistryValueKind.ExpandString;
                default:
                    return defaultKind;
            }
        }

        public static byte[] ConvertToBinary(string value)
        {
            try
            {
                string[] strs = value.Split(' ');
                byte[] bs = new byte[strs.Length];
                for (int i = 0; i < strs.Length; i++)
                {
                    bs[i] = Convert.ToByte(strs[i], 16);
                }
                return bs;
            }
            catch { return null; }
        }

        public static string GetAttributeString(this XmlNode itemXN, string key)
        {
            return StringResource.GetDirectString(itemXN.Attributes[key].Value);
        }
    }
}
