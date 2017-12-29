using ConferenceVersionUpdate.Common.LogHelper;
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

        /// <summary>
        /// 事件激发
        /// </summary>
        /// <param name="propertyName"></param>
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

        /// <summary>
        /// 构造函数
        /// </summary>
        public VersionUpdateWindow()
        {
            //日志加载初始化
            LogManage.LogInit();
            try
            {
                //UI加载
                InitializeComponent();
                //绑定当前上下文
                this.DataContext = this;

                //创建版本更新客户端对象模型
                ConferenceVersionWebservice.ConferenceVersionWebserviceSoapClient client = new ConferenceVersionWebservice.ConferenceVersionWebserviceSoapClient();

                //注册版本更新完成事件
                client.GetUpDateFileCompleted += client_GetUpDateFileCompleted;

                //获取需要更新的版本文件
                client.GetUpDateFileAsync();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 获取版本更新文件完成事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void client_GetUpDateFileCompleted(object sender, ConferenceVersionWebservice.GetUpDateFileCompletedEventArgs e)
        {
            try
            {
                //获取需要关闭的进程
                var ProceList = this.Split(this.CutProcess, SplitString);
                //通过遍历去关闭需要关闭的进程
                foreach (var item in ProceList)
                {
                    //获取遍历中的进程(通过进程名称)
                    var processList = Process.GetProcessesByName(item);
                    //遍历干掉对应的进程
                    foreach (var item2 in processList)
                    {
                        //结束进程
                        item2.Kill();
                    }
                }
                //获取需要更新的文件
                this.UpdateFile = e.Result.UpdateFile;
                //获取需要更新的目录
                this.UpdateRootFile = e.Result.UpdateRootFile;

                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                        timer.Interval = 1000;
                        timer.Tick += ((object obj1, EventArgs e1) =>
                        {
                            //更新初始化
                            UpdateInit();
                            timer.Stop();
                        });
                        timer.Start();    
                    }));
                             
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 更新初始化

        /// <summary>
        /// 更新初始化
        /// </summary>
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
                    //遍历加载文件
                    foreach (var item in this.FileList1)
                    {
                        //下载文件列表加载子项
                        this.DownloadList.Add(item);
                    }
                }
                //获取要更新的文件集（带目录的文件）
                this.FileList2 = this.Split(this.UpdateRootFile, SplitString);

                if (FileList2 != null)
                {
                    //遍历加载文件（带目录）
                    foreach (var item in this.FileList2)
                    {
                        //创建目录
                        this.RootCreate(item);
                        //下载文件列表加载子项
                        this.DownloadList.Add(item);
                    }
                }
                //开始更新
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        //遍历需要下载的文件
                        foreach (var item in this.DownloadList)
                        {
                            //web客户端对象模型
                            WebClient webClient = new WebClient();
                            //下载完成事件
                            webClient.DownloadFileCompleted += webClient_DownloadFileCompleted;
                            //真实文件名称
                            string realFile = item;

                            //【.exe_F】转为【.exe】
                            if (item.Contains(this.Exe_FalseFileExtention))
                            {
                                //去掉_F
                                realFile = item.Replace(this.Exe_FalseFileExtention, ".exe");
                            }
                            //【.dll_F】转为【.dll】
                            else if (item.Contains(this.Dll_FalseFileExtention))
                            {
                                realFile = item.Replace(this.Dll_FalseFileExtention, ".dll");
                            }
                            //【.config_F】转为【.exe.config】
                            else if (item.Contains(this.Config_FalseFileExtention))
                            {
                                realFile = item.Replace(this.Config_FalseFileExtention, ".exe.config");
                            }

                            if (realFile.Contains("/"))
                            {
                                //替换
                                realFile = realFile.Replace("/", "\\");
                            }

                            //开始下载文件
                            webClient.DownloadFileAsync(new Uri(this.HttpService + item), Environment.CurrentDirectory + "\\" + realFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                    finally
                    {

                    }
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 映射创建文件夹

        /// <summary>
        /// 映射创建文件夹
        /// </summary>
        public void RootCreate(string FileName)
        {
            try
            {
                //获取当前系统目录
                string systemRoot = Environment.CurrentDirectory;
                //获取目录
                string root = FileName.Remove(FileName.LastIndexOf('/'));
                //获取到目录（层次级）
                Array strRootArray = root.Split(new char[] { '/' }).ToArray<String>();
                //左侧root
                string rootLeft = string.Empty;
                //遍历层级目录
                foreach (var item in strRootArray)
                {
                    //拼接root
                    rootLeft += "\\" + item;
                    //本地完整目录
                    string compleatelyRoot = systemRoot + rootLeft;
                    //如果不存在该目录则创建
                    if (!Directory.Exists(compleatelyRoot))
                    {
                        //创建完整目录
                        Directory.CreateDirectory(compleatelyRoot);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
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
                //完成数量递增
                CompleteCount++;
                //达到完成数量
                if (CompleteCount == this.DownloadList.Count)
                {
                    //完成之后使用异步委托将
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        //更新完成后设置为可见（确定按钮）
                        this.CompleteVis = System.Windows.Visibility.Visible;
                        //等待提示按钮隐藏（提示正在进行更新）
                        this.EditUpdateVis = System.Windows.Visibility.Collapsed;
                    }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
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
                //获取需要开启的应用程序路径
                string openApp = Environment.CurrentDirectory + "\\" + this.CompleteOpenApp;
                //检验是否存在该文件（本地）
                if (File.Exists(openApp))
                {
                    //创建一个进程
                    Process process = new Process();
                    //指定应用程序名称
                    process.StartInfo.FileName = openApp;
                    //进程启动
                    process.Start();
                    //关闭当前应用程序
                    Application.Current.Shutdown(0);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 字符串分割方法

        /// <summary>
        /// 字符串分割方法
        /// </summary>
        /// <param name="spliteSource">分割源</param>
        /// <param name="splitFitter">分割字符</param>
        /// <returns>返回分割后的结果</returns>
        public string[] Split(string spliteSource, string splitFitter)
        {
            //声明一个数组
            string[] stringArray = null;
            try
            {
                //返回分割后生成的字符串数组
                stringArray = spliteSource.Split(new string[] { splitFitter }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return stringArray;
        }

        #endregion
    }
}
