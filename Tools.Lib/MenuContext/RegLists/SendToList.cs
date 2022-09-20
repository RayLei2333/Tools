using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class SendToList : RegistryList
    {
        public override void LoadItems()
        {
            this.Items.Clear();
            foreach (string path in Directory.GetFileSystemEntries(RegistryPath.SendToPath))
            {
                if (Path.GetFileName(path).ToLower() == "desktop.ini")
                    continue;
                this.Items.Add(new SendToItem(path));
            }

            this.Sort();
            this.Items.Add(new RegVisbleItem(RegVisbleItem.SendToDrive));
            this.Items.Add(new RegVisbleItem(RegVisbleItem.DeferBuildSendTo));
        }
    }
}
