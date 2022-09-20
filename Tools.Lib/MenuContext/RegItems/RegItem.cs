using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegLists;

namespace Tools.Lib.MenuContext.RegItems
{
    public abstract class RegItem : RegistryItem, IEnableItem
    {

        public string IconPath { get; set; }

        public int IconIndex { get; set; }

        //public virtual GroupItem GroupItem
        //{
        //    get
        //    {
        //        if (!this.HasDetail)
        //            return null;
        //        DetailList detailList = new DetailList(this.Guid);
        //        detailList.LoadItems();
        //        return detailList.GroupItem;

        //    }
        //}

        public virtual bool IsEnable { get; set; }

        public override string KeyName => RegistryExtension.GetKeyName(this.Path);


        protected RegItem(string path) : base(path)
        {
        }


    }
}
