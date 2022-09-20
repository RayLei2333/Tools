using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Tools.MenuContext
{
    /// <summary>
    /// LeftView.xaml 的交互逻辑
    /// </summary>
    public partial class LeftView : UserControl
    {
        private List<SelectItem> _Items;

        public List<SelectItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                for (int i = 0; i < value.Count; i++)
                {
                    value[i].Index = i;
                }
                this.RefreshItemSource();
            }
        }

        private int _SelectIndex;

        public int SelectIndex
        {
            get { return _SelectIndex; }
            set
            {

                _SelectIndex = value;
                var oldItem = this.Items.Where(t => t != null && t.Selected).FirstOrDefault();
                if (oldItem != null)
                    oldItem.Selected = false;
                this.Items[this.SelectIndex].Selected = true;
                this._SelectItem = this.Items[_SelectIndex];
                this.RefreshItemSource();
            }
        }

        private SelectItem _SelectItem;

        public SelectItem SelectItem
        {
            get { return _SelectItem; }
            set
            {
                //_SelectItem = value;
                this.SelectIndex = this.Items.IndexOf(value);
            }
        }



        public LeftView()
        {
            InitializeComponent();
            this.DataContext = this;

        }

        private void RefreshItemSource()
        {
            this.ItemList.ItemsSource = null;
            this.ItemList.ItemsSource = this.Items;
        }

        public delegate void LeftItemClickHandler(object sender, LeftViewClickEventArgs e);

        public event LeftItemClickHandler OnClick;

        protected void ListItemOnClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as Grid;
            SelectItem toolbarItem = grid.Tag as SelectItem;
            if (toolbarItem == null || toolbarItem.Selected)
                return;
            //this.SelectIndex = this.Items.IndexOf(toolbarItem);
            this.SelectIndex = toolbarItem.Index;
            LeftViewClickEventArgs args = new LeftViewClickEventArgs()
            {
                Data = SelectItem
            };
            OnClick(sender, args);
        }
    }
}
