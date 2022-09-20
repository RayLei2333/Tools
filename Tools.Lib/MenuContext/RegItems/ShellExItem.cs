using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class ShellExItem : RegItem
    {
        public static readonly string[] DdhParts = { "DragDropHandlers", "-DragDropHandlers" };
        public static readonly string[] CmhParts = { "ContextMenuHandlers", "-ContextMenuHandlers" };
        public static readonly Guid LnkOpenGuid = new Guid("00021401-0000-0000-c000-000000000046");

        private string _DefaultValue => Registry.GetValue(this.Path, "", null)?.ToString();
        private string _ParentPath => RegistryExtension.GetParentPath(this.Path);
        private string _ParentKeyName => RegistryExtension.GetKeyName(this._ParentPath);
        private string _ShellExPath => RegistryExtension.GetParentPath(this._ParentPath);

        private string _BackupPath
        {
            get
            {
                string[] parts = IsDragDropItem ? DdhParts : CmhParts;
                return $@"{_ShellExPath}\{(this.IsEnable ? parts[1] : parts[0])}\{KeyName}";
            }
        }

        public override string ItemFilePath => GuidInfo.GetFilePath(Guid);

        public bool IsDragDropItem => _ParentKeyName.EndsWith(DdhParts[0], StringComparison.OrdinalIgnoreCase);

        public override bool IsEnable
        {
            get
            {
                string[] parts = IsDragDropItem ? DdhParts : CmhParts;
                return this._ParentKeyName.Equals(parts[0], StringComparison.OrdinalIgnoreCase);
            }
            set
            {
                try
                {
                    RegistryExtension.MoveTo(this.Path, this._BackupPath);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                this.Path = this._BackupPath;
            }
        }

        public ShellExItem(Guid guid, string path) : base(path)
        {
            this.Guid = guid;
            this.ItemIcon = GuidInfo.GetImage(guid);
            this.ItemFilePath = GuidInfo.GetFilePath(guid);
            this.ItemText = GuidInfo.GetText(guid) ?? (this.KeyName.Equals(Guid.ToString("B"), StringComparison.OrdinalIgnoreCase) ? this._DefaultValue : this.KeyName);
        }

        public static Dictionary<string, Guid> GetPathAndGuids(string shellExPath)
        {
            Dictionary<string, Guid> result = new Dictionary<string, Guid>();
            //string[] parts = isDragDrop ? DdhParts : CmhParts;
            
            string[] parts = CmhParts;

            foreach (string part in parts)
            {
                using (RegistryKey cmKey = RegistryExtension.GetRegistryKey($@"{shellExPath}\{part}"))
                {
                    if (cmKey == null)
                        continue;
                    foreach (string keyName in cmKey.GetSubKeyNames())
                    {
                        try
                        {
                            using (RegistryKey key = cmKey.OpenSubKey(keyName))
                            {
                                if (!Guid.TryParse(key.GetValue("")?.ToString(), out Guid guid))
                                    Guid.TryParse(keyName, out guid);
                                if (!guid.Equals(Guid.Empty))
                                    result.Add(key.Name, guid);
                            }
                        }
                        catch { continue; }
                    }
                }

            }
            return result;

        }
    }
}
