using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Lib.MenuContext.RegItems
{
    public class GroupItem : RegItem
    {
        public bool IsGroup => true;

        public PathType PathType { get; set; }

        public List<RegistryItem> Items { get; set; }

        public GroupItem(string path, PathType pathType) : base(path)
        {
            this.Path = path;
            this.PathType = pathType;
            if (pathType == PathType.File || pathType == PathType.Directory)
                this.ItemFilePath = Environment.ExpandEnvironmentVariables(path);
            this.Items = new List<RegistryItem>();

            switch (pathType)
            {
                case PathType.File:
                    this.ItemText = System.IO.Path.GetFileNameWithoutExtension(this.ItemFilePath);
                    this.ItemIcon = IconResource.GetExtensionIcon(this.ItemFilePath);
                    break;
                case PathType.Directory:
                    this.ItemText = System.IO.Path.GetFileNameWithoutExtension(this.ItemFilePath);
                    this.ItemIcon = IconResource.GetFolderIcon(this.ItemFilePath);
                    break;
            }
        }

    }
}
