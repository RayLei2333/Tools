using System;

namespace Tools.Lib.MenuContext.RegItems
{
    public class IniNumberItem : IniItem, IValueItem<int>
    {
        public int ItemValue
        {
            get
            {
                string value = this.IniWriter.GetValue(this.Section, this.KeyName);
                if (string.IsNullOrWhiteSpace(value))
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
                IniWriter.SetValue(this.Section, this.KeyName, value);
            }
        }

        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int DefaultValue { get; set; }

        public IniNumberItem(string path) : base(path)
        {
        }


    }
}
