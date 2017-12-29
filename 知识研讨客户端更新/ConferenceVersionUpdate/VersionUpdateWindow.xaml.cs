using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ConferenceVersionUpdate
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class VersionUpdateWindow : Window, INotifyPropertyChanged
    {
        #region 绑定更新

        public void OnProertyCHanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region 绑定属性

        Visibility editUpdateVis = Visibility.Visible;

        public Visibility EditUpdateVis
        {
            get { return editUpdateVis; }
            set
            {
                editUpdateVis = value;
                this.OnProertyCHanged("EditUpdateVis");
            }
        }

        Visibility completeVis = Visibility.Hidden;

        public Visibility CompleteVis
        {
            get { return completeVis; }
            set
            {
                completeVis = value;
                this.OnProertyCHanged("CompleteVis");
            }
        }

        #endregion

        #region 非绑定属性

        /// <summary>
        /// 需要更新的文件
        /// </summary>
        string UpdateFile = null;

        /// <summary>
        /// 需要更新的文件（包含目录）
        /// </summary>
        string UpdateRootFile = null;

        /// <summary>
        /// 更新完成之后需要打开的应用程序
        /// </summary>
        string CompleteOpenApp = System.Configuration.ConfigurationManager.AppSettings["CompleteOpenApp"];

        /// <summary>
        /// 在更新前需要干掉的进程（防止干扰）
        /// </summary>
        string CutProcess = System.Configuration.ConfigurationManager.AppSettings["CutProcess"];

        /// <summary>
        /// exe文件解析
        /// </summary>
        string Exe_FalseFileExtention = System.Configuration.ConfigurationManager.AppSettings["Exe_FalseFileExtention"];

        /// <summary>
        /// dll文件解析
        /// </summary>
        string Dll_FalseFileExtention = System.Configuration.ConfigurationManager.AppSettings["Dll_FalseFileExtention"];

        /// <summary>
        /// config文件解析
        /// </summary>
        string Config_FalseFileExtention = System.Configuration.ConfigurationManager.AppSettings["Config_FalseFileExtention"];

        /// <summary>
        /// cer文件解析(证书)
        /// </summary>
        string Cer_FalseFileExtention = System.Configuration.ConfigurationManager.AppSettings["Cer_FalseFileExtention"];

        /// <summary>
        /// 更新服务地址
        /// </summary>
        string HttpService = System.Configuration.ConfigurationManager.AppSettings["HttpService"];

        /// <summary>
        /// 分割字符
        /// </summary>
        string SplitString = System.Configuration.ConfigurationManager.AppSettings["SplitString"];

        /// <summary>
        /// 文件列表
        /// </summary>
        string[] FileList1 = null;

        /// <summary>
        /// 文件列表（带目录）
        /// </summary>
        string[] FileList2 = null;

        /// <summary>
        /// 下载列表
        /// </summary>
        List<string> DownloadList = new List<string>();

        /// <summary>
        /// 完成数量
        /// </summary>
        int CompleteCount = 0;

        #endregion

        #region 构造函数

        public VersionUpdateWindow()
        {
            try
            {
                InitializeComponent();

                this.DataContext = this;

                this.Loaded += VersionUpdateWindow_Loaded;
            }
            catch (Exception ex)
            {
            }
        }

        void VersionUpdateWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {


                ConferenceVersionWebservice.ConferenceVersionWebserviceSoapClient client = new ConferenceVersionWebservice.ConferenceVersionWebserviceSoapClient();
                client.GetUpDateFileCompleted += client_GetUpDateFileCompleted;

                client.GetUpDateFileAsync();
            }
            catch (Exception ex)
            {
            }
        }

        void client_GetUpDateFileCompleted(object sender, ConferenceVersionWebservice.GetUpDateFileCompletedEventArgs e)
        {
            try
            {
                //获取需要关闭的进程
                var ProceList = this.Split(this.CutProcess, SplitString);

                foreach (var item in ProceList)
                {
                    var processList = Process.GetProcessesByName(item);
                    foreach (var item2 in processList)
                    {
                        item2.Kill();
                    }
                }

                this.UpdateFile = e.Result.UpdateFile;
                this.UpdateRootFile = e.Result.UpdateRootFile;


                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1000);
                timer.Tick += (object e1, EventArgs args) =>
            {
                //获取需要关闭的进程
                var ProceList2 = this.Split(this.CutProcess, SplitString);
                Process[] processConferenceArray = null;
                if (ProceList2.Count() >= 1)
                {
                    processConferenceArray = Process.GetProcessesByName(ProceList2[1]);
                }

                if (processConferenceArray.Count() == 0)
                {
                    UpdateInit();
                    timer.Stop();
                }
            };
                timer.Start();
            }
            catch (Exception ex)
            {
            }
        }


        #endregion

        #region 更新初始化

        void UpdateInit()
        {
            try
            {
                //更新列表初始化
                this.DownloadList.Clear();

                //获取要更新的文件集
                this.FileList1 = this.Split(this.UpdateFile, SplitString);

                if (FileList1 != null)
                {
                    foreach (var item in this.FileList1)
                    {
                        this.DownloadList.Add(item);
                    }
                }
                //获取要更新的文件集（带目录的文件）
                this.FileList2 = this.Split(this.UpdateRootFile, SplitString);

                if (FileList2 != null)
                {
                    foreach (var item in this.FileList2)
                    {
                        this.DownloadList.Add(item);
                    }
                }
                //开始更新
                Thread thread = new Thread(() =>
                {
                    foreach (var item in this.DownloadList)
                    {
                        WebClient webClient = new WebClient();
                        webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;

                        string realFile = item;
                        if (item.Contains(this.Exe_FalseFileExtention))
                        {
                            realFile = item.Replace(this.Exe_FalseFileExtention, ".exe");
                        }
                        else if (item.Contains(this.Dll_FalseFileExtention))
                        {
                            realFile = item.Replace(this.Dll_FalseFileExtention, ".dll");
                        }
                        else if (item.Contains(this.Config_FalseFileExtention))
                        {
                            realFile = item.Replace(this.Config_FalseFileExtention, ".exe.config");
                        }
                        else if (item.Contains(this.Cer_FalseFileExtention))
                        {
                            realFile = item.Replace(this.Cer_FalseFileExtention, ".cer");
                        }

                        webClient.DownloadFileAsync(new Uri(this.HttpService + item), Environment.CurrentDirectory + "\\" + realFile);
                    }
                });
                thread.Start();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region 字符串分割方法

        /// <summary>
        /// 字符串分割方法
        /// </summary>
        /// <param name="spliteSource"></param>
        /// <param name="splitFitter"></param>
        /// <returns></returns>
        public string[] Split(string spliteSource, string splitFitter)
        {
            return spliteSource.Split(new string[] { splitFitter }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        #region 文件更新完成

        /// <summary>
        /// 文件下载完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                CompleteCount++;
                if (CompleteCount == this.DownloadList.Count)
                {
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.CompleteVis = System.Windows.Visibility.Visible;
                        this.EditUpdateVis = System.Windows.Visibility.Collapsed;
                    }));
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region 更新完成点击事件

        /// <summary>
        /// 更新完成点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Compleate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string openApp = Environment.CurrentDirectory + "\\" + this.CompleteOpenApp;
                if (File.Exists(openApp))
                {
                    Process process = new Process();
                    process.StartInfo.FileName = openApp;
                    process.Start();

                    Application.Current.Shutdown(0);
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
