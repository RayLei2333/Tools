using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class StoreItem : ShellItem
    {
        public bool IsPublic { get; set; }

        public StoreItem(string path, bool isPublic) : base(path)
        {
            this.IsPublic = isPublic;
        }
    }
}
