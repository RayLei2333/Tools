using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.Lib.MenuContext.RegLists
{
    public class WinXList : RegistryList
    {
        public override void LoadItems()
        {
            this.Items.Clear();
            this.LoadWinXItems();
        }

        private void LoadWinXItems()
        {
            string[] dirPaths = Directory.GetDirectories(RegistryPath.WinXPath);
            Array.Reverse(dirPaths);
            //bool sorted = false;
            foreach (string dirPath in dirPaths)
            {
                //WinXGroupItem groupItem = new WinXGroupItem(dirPath);
                string[] lnkPaths;
                //if (AppConfig.WinXSortable)
                //{
                //    lnkPaths = GetSortedPaths(dirPath, out bool flag);
                //    if (flag) sorted = true;
                //}
                //else
                //{
                lnkPaths = Directory.GetFiles(dirPath, "*.lnk");
                Array.Reverse(lnkPaths);
                //}
                foreach (string path in lnkPaths)
                {
                    WinXItem winXItem = new WinXItem(path);
                    this.Items.Add(winXItem);
                }
            }
        }
    }
}
