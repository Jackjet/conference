using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace ConferenceVersionUpdate
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            //创建版本更新窗体
            VersionUpdateWindow versionUpdateWindow = new VersionUpdateWindow();
            //显示版本更新窗体
            versionUpdateWindow.Show();
        }
    }
}
