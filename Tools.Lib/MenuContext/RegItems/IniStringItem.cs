namespace Tools.Lib.MenuContext.RegItems
{
    public class IniStringItem : IniItem, IValueItem<string>
    {
        public string ItemValue
        {
            get => IniWriter.GetValue(this.Section, this.KeyName);
            set => IniWriter.SetValue(this.Section, this.KeyName, value);
        }

        public IniStringItem(string path) : base(path)
        {
        }
    }
}
