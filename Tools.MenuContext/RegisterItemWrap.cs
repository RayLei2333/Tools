using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.MenuContext
{
    public class RegisterItemWrap : INotifyPropertyChanged
    {
        public RegistryItem Register { get; set; }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this,new PropertyChangedEventArgs("IsOpen"));
                }
            }
        }

        public GroupItem DetailGroup { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
