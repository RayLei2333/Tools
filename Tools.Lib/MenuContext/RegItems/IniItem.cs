namespace Tools.Lib.MenuContext.RegItems
{
    public abstract class IniItem : RegistryItem, IRestartExplorer
    {
        //public virtual object ItemValue { get; set; }

        public string Section { get; set; }

        internal IniWriter IniWriter { get; set; }

        public IniItem(string path) : base(path)
        {
            this.ItemFilePath = path;
            this.IniWriter = new IniWriter(path);
        }
    }
}
