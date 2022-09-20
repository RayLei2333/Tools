using System;

namespace Tools.Lib.MenuContext.RegItems
{
    public class IniVisbleItem : IniItem, IValueItem<bool>, IEnableItem
    {

        public bool ItemValue
        {
            get => IniWriter.GetValue(this.Section, this.KeyName) == this.TurnOnValue;
            set => IniWriter.SetValue(this.Section, this.KeyName, value ? this.TurnOnValue : this.TurnOffValue);

        }

        public string TurnOnValue { get; set; }
        public string TurnOffValue { get; set; }

        public bool IsEnable
        {
            get => Convert.ToBoolean(ItemValue);
            set =>this.ItemValue = value;
        }

        public IniVisbleItem(string path) : base(path)
        {
        }
    }
}
