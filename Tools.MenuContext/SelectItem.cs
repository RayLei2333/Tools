using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.MenuContext
{
    public class SelectItem
    {

        public bool Selected { get; set; }

        public string Text { get; set; }

        public int Index { get; set; }


        public SelectItem() { }

        public SelectItem(string text)
        {
            this.Text = text;
        }
    }

    public class SelectSeparate : SelectItem
    {
    }
}
