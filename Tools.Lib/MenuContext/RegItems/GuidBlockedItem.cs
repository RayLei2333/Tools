using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class GuidBlockedItem : RegItem
    {
        public const string HKLMBLOCKED = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public const string HKCUBLOCKED = @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Blocked";
        public static readonly string[] BlockedPaths = { HKLMBLOCKED, HKCUBLOCKED };

        public override string Path
        {
            get
            {
                foreach (string path in BlockedPaths)
                {
                    using (var key = RegistryExtension.GetRegistryKey(path))
                    {
                        if (key == null) 
                            continue;
                        if (key.GetValueNames().Contains(Value, StringComparer.OrdinalIgnoreCase)) 
                            return path;
                    }
                }
                return null;
            }
        }

        public override string ItemText
        {
            get
            {
                string text;
                if (Guid.TryParse(Value, out Guid guid))
                    text = GuidInfo.GetText(guid);
                else
                    text = "格式不正确的Guid";
                text += "\n" + Value;
                return text;
            }
        }

        public string Value { get; set; }

        public GuidBlockedItem(string value) : base(null)
        {
            this.Value = value;
            if (Guid.TryParse(value, out Guid guid))
            {
                this.Guid = guid;
                this.ItemIcon = GuidInfo.GetImage(guid);
                this.ItemFilePath = GuidInfo.GetFilePath(Guid);
            }
            else
            {
                this.Guid = Guid.Empty;
                this.ItemIcon = AppImage.SystemFile;
            }
        }
    }
}
