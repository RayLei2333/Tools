using System;
using System.Collections.Generic;
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
using Tools.Lib.MenuContext;
using Tools.Lib.MenuContext.RegItems;
using Tools.Lib.MenuContext.RegLists;

namespace Tools.MenuContext
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private LeftView _LeftView = new LeftView();
        private MainView _MainView = new MainView();


        readonly List<SelectItem> LeftItems = new List<SelectItem>()
        {
            new SelectItem("文件"),
            new SelectItem("文件夹"),
            new SelectItem("目录"),
            new SelectItem("目录背景"),
            new SelectItem("桌面背景"),
            new SelectItem("磁盘分区"),
            new SelectItem("所有对象"),
            new SelectItem("此电脑"),
            new SelectItem("回收站"),
            new SelectItem("库"),
            new SelectSeparate(),
            new SelectItem("新建菜单"),
            new SelectItem("发送到"),
            new SelectItem("打开方式"),
            new SelectSeparate(),
            new SelectItem("Win+X"),
            new SelectItem("IE")
        };
        readonly List<Scenes> SceneList = new List<Scenes>()
        {
            Scenes.File,
            Scenes.Folder,
            Scenes.Directory,
            Scenes.Background,
            Scenes.Desktop,
            Scenes.Drive,
            Scenes.AllObjects,
            Scenes.Computer,
            Scenes.RecycleBin,
            Scenes.Library
        };

        readonly ShellList _ShellList = new ShellList();
        readonly ShellNewList _ShellNewList = new ShellNewList();
        readonly SendToList _SendToList = new SendToList();
        readonly OpenWithList _OpenWithList = new OpenWithList();
        readonly WinXList _WinXList = new WinXList();
        readonly IEList _IEList = new IEList();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            //this.OnKeyUp += 


            this._LeftView.Items = this.LeftItems;
            this._LeftView.SetValue(Grid.ColumnProperty, 0);
            this._LeftView.OnClick += SelectIndexChange;
            this.Grid.Children.Add(this._LeftView);

            this._MainView.SetValue(Grid.ColumnProperty, 1);
            this.Grid.Children.Add(this._MainView);

            this.Jump(0);
        }

        private void Jump(int selectIndex)
        {
            this._LeftView.SelectIndex = selectIndex;
            this.SwitchItemInfo();

        }

        private void SelectIndexChange(object sender, LeftViewClickEventArgs e)
        {
            SelectItem item = e.Data as SelectItem;
            this.Jump(item.Index);
        }

        private void SwitchItemInfo()
        {
            List<RegistryItem> items;
            RegistryList registryList = null;
            List<RegisterItemWrap> list = new List<RegisterItemWrap>();
            switch (this._LeftView.SelectIndex)
            {
                case 11:
                    registryList = this._ShellNewList;
                    break;
                case 12:
                    registryList = this._SendToList;
                    break;
                case 13:
                    registryList = this._OpenWithList;
                    break;
                case 15:
                    registryList = this._WinXList;
                    break;
                case 16:
                    registryList = this._IEList;
                    break;
                default:
                    this._ShellList.Scene = SceneList[this._LeftView.SelectIndex];
                    registryList = this._ShellList;
                    break;
            }
            registryList.LoadItems();
            items = registryList.Items;
            foreach (var item in items)
            {
                RegisterItemWrap wrap = new RegisterItemWrap()
                {
                    Register = item
                };
                if (item.HasDetail)
                {
                    DetailList detailList = new DetailList(item.Guid);
                    detailList.LoadItems();
                    wrap.DetailGroup = detailList.GroupItem;
                }
                list.Add(wrap);
            }

            this._MainView.Items = list;
        }


        

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.IsUp && e.Key == Key.Escape)
                Application.Current.Shutdown();
        }
    }
}
