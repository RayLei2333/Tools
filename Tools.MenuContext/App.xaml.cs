using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tools.Lib.MenuContext;

namespace Tools.MenuContext
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppConfig.DetailedItemDic = Tools.MenuContext.Properties.Resources.DetailedItemDic;
            AppConfig.GuidInfosDic = Tools.MenuContext.Properties.Resources.GuidInfosDic;
            AppConfig.MicrosoftStore = Tools.MenuContext.Properties.Resources.MicrosoftStore;
            XmlDicHelper.RelodDics();
        }
    }
}
