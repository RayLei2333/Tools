using System;
using System.Drawing;

namespace Tools.Lib.MenuContext.RegItems
{
    public abstract class RegistryItem
    {
        public virtual string Path { get; set; }

        public virtual string ItemFilePath { get; set; }

        public virtual string ItemText { get; set; }

        public virtual bool ItemVisable { get; set; } = true;

        public virtual bool HasDetail
        {
            get
            {
                if (this.Guid == null || Guid == Guid.Empty)
                    return false;
                return XmlDicHelper.DetailedEditGuidDic.ContainsKey(this.Guid);
            }
        }

        public virtual Icon ItemIcon { get; set; }

        public virtual Guid Guid { get; set; }

        public virtual string KeyName { get; set; }


        public RegistryItem(string path)
        {
            this.Path = path;
        }
    }
}
