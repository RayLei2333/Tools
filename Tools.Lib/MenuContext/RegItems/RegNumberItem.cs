using Microsoft.Win32;
using System;

namespace Tools.Lib.MenuContext.RegItems
{
    public class RegNumberItem : RegItem, IValueItem<int>, IRestartExplorer
    {
        public string ValueName { get; set; }
        public RegistryValueKind ValueKind { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int DefaultValue { get; set; }
        public int ItemValue
        {
            get
            {
                object value = Registry.GetValue(this.Path, this.ValueName, null);
                if (value == null)
                    return this.DefaultValue;
                int num = Convert.ToInt32(value);
                if (num > this.MaxValue)
                    return this.MaxValue;
                if (num < this.MinValue)
                    return this.MinValue;
                else
                    return num;
            }
            set
            {
                Registry.SetValue(this.Path, this.ValueName, value, this.ValueKind);
            }
        }

        public RegNumberItem(string path) : base(path)
        {
        }
    }
}
