using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class RegStringItem : RegItem, IValueItem<string>, IRestartExplorer
    {

        public string ValueName { get; set; }

        public string ItemValue
        {
            get => Registry.GetValue(this.Path, this.ValueName, null)?.ToString();
            set => Registry.SetValue(this.Path, this.ValueName, value);
        }


        public RegStringItem(string path) : base(path)
        {
        }

    }
}
