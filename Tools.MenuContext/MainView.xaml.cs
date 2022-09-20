using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tools.Lib.MenuContext.RegItems;

namespace Tools.MenuContext
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : UserControl, INotifyPropertyChanged
    {
        //private List<RegistryItem> _Items;

        //public List<RegistryItem> Items
        //{
        //    get { return _Items; }
        //    set { _Items = value; }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        private List<RegisterItemWrap> _Items;

        public List<RegisterItemWrap> Items
        {
            get => _Items;
            set
            {
                _Items = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Items"));
                }
            }
        }



        public MainView()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void Expand_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            bool isOpen = ((sender as Label).DataContext as RegisterItemWrap).IsOpen;
            RegisterItemWrap wrap = ((sender as Label).DataContext as RegisterItemWrap);
            //wrap.IsOpen = !wrap.IsOpen;

            int index = this.Items.IndexOf(wrap);
            this.Items[index].IsOpen = !this.Items[index].IsOpen;
            //this.ItemList.ItemsSource = null;
            //this.ItemList.ItemsSource = this.Items;
            //((sender as Label).DataContext as RegisterItemWrap).IsOpen = !isOpen;
            //MessageBox.Show(isOpen.ToString() + "\t\t" + (!isOpen).ToString());
        }
    }
}
