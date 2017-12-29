using Conference.Common;
using Conference.Page;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.RegeditHelper;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.Sharing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference.View.WebBrowser
{
    /// <summary>
    /// MeetVoteView.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserView : UserControlBase
    {
        #region 内部字段



        #endregion

        #region 绑定属性


        ObservableCollection<UrlEntity> urlEntityList = new ObservableCollection<UrlEntity>();
        /// <summary>
        /// uri列表
        /// </summary>
        public ObservableCollection<UrlEntity> UrlEntityList
        {
            get { return urlEntityList; }
            set
            {
                if (this.urlEntityList != value)
                {
                    this.urlEntityList = value;
                    this.OnPropertyChanged("UrlEntityList");
                }
            }
        }

        UrlEntity selectedUlEntity;
        /// <summary>
        /// 当前选中的地址
        /// </summary>
        public UrlEntity SelectedUlEntity
        {
            get { return selectedUlEntity; }
            set
            {
                if (this.selectedUlEntity != value)
                {
                    this.selectedUlEntity = value;
                    this.OnPropertyChanged("SelectedUlEntity");
                }
            }
        }

        #endregion
        
        #region 构造函数

        public WebBrowserView()
        {
            try
            {
                //加载UI
                InitializeComponent();

                //导航到某页面
                this.webBrowser.Navigate(Constant.WebUri);
                //显示导航地址
                this.txtInput.Text = Constant.WebUri;
                //绑定当前上下文
                this.DataContext = this;

                #region 注册事件

                //加载文档
                this.webBrowser.Navigated += webBrowser_Navigated;
                //新窗体加载事件
                this.webBrowser.NewWindow += new CancelEventHandler(this.webBrowser_NewWindow);
                //导航到指定地址
                this.btnNavicate.Click += btnNavicate_Click;
                //收藏地址
                this.btnCollection.Click += btnCollection_Click;
                //演示
                this.btnShare.Click += btnShare_Click;
                //url列表选中事件
                this.btnCmb.SelectionChanged += btnCmb_SelectionChanged;
                //url移除事件
                this.btnUrlRemove.Click += btnUrlRemove_Click;
                //上一页
                this.btnUp.Click += btnUp_Click;
                //下一页
                this.btnNext.Click += btnNext_Click;
                //进入主页
                this.btnHome.Click += btnHome_Click;
                //转到百度
                this.btnBaidu.Click += btnBaidu_Click;

                #endregion

                //加载uri列表
                this.LoadUrlStoreData();
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

        #region 转到百度

        void btnBaidu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //转到百度
                this.webBrowser.Navigate("http://61.135.169.121");
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

        #region 进入主页

        void btnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载默认页面
                this.webBrowser.Navigate(Constant.WebUri);
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

        #region 下一页

        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.webBrowser.CanGoForward)
                {
                    this.webBrowser.GoForward();
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

        #region 上一页

        void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.webBrowser.CanGoBack)
                {
                    this.webBrowser.GoBack();
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

        #region url移除事件

        void btnUrlRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.UrlEntityList.Remove(this.SelectedUlEntity);
                this.ReSave();
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

        #region url列表选中事件

        void btnCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //导航到地址
                this.webBrowser.Navigate(this.SelectedUlEntity.Url);
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

        #region 开始演示

        void btnShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取当前地址
                Uri uri = this.webBrowser.Url;
                RegeditManage.OpenAplicationByRegedit2("IEXPLORE.EXE", uri.OriginalString, new Action<int, IntPtr>((processID, intptr) =>
                    {
                        //获取共享模型
                        var shareModality = ((ApplicationSharingModality)(LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing]));

                        //遍历可以共享的资源
                        foreach (var item in shareModality.ShareableResources)
                        {
                            //指定要共享的程序名称
                            if (item.Id.Equals(processID))
                            {
                                //判断是否可以进行共享该程序
                                if (shareModality.CanShare(item.Type))
                                {
                                    //共享程序置顶
                                    Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                                    //开始共享该程序
                                    shareModality.BeginShareResources(item, null, null);
                                    //同步页面
                                    Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                                    break;
                                }
                            }
                            else if (item.Id == intptr.ToInt32())
                            {
                                //判断是否可以进行共享该程序
                                if (shareModality.CanShare(item.Type))
                                {
                                    //共享程序置顶
                                    Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                                    //开始共享该程序
                                    shareModality.BeginShareResources(item, null, null);
                                    //同步页面
                                    Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                                    break;
                                }
                            }
                            //else if (item.Name.Contains(""))
                            //{

                            //    //判断是否可以进行共享该程序
                            //    if (shareModality.CanShare(item.Type))
                            //    {
                            //        共享程序置顶
                            //        Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                            //        //开始共享该程序
                            //        shareModality.BeginShareResources(item, null, null);
                            //        //同步页面
                            //        Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                            //        break;
                            //    }
                            //}
                        }
                    }));

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

        #region 浏览器打开之前（清除缓存）

        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            try
            {
                this.webBrowser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);

                e.Cancel = true;
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

        #region 加载文档(浏览器)

        private void webBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            try
            {
                //this.webBrowser.Document.Window.Error -= new System.Windows.Forms.HtmlElementErrorEventHandler(this.Window_Error);
                this.webBrowser.Document.Window.Error += new System.Windows.Forms.HtmlElementErrorEventHandler(this.Window_Error);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }

        }

        /// <summary>
        /// 浏览器加载异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Error(object sender, System.Windows.Forms.HtmlElementErrorEventArgs e)
        {
            try
            {
                e.Handled = true;
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

        #region 收藏地址

        void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dd = this.webBrowser.Url;
                this.urlEntityList.Add(new UrlEntity(this.webBrowser.Document.Title, this.webBrowser.Url.AbsoluteUri));

                this.ReSave();
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

        #region 加载uri列表库

        private void LoadUrlStoreData()
        {
            try
            {
                string currentDirectory = Environment.CurrentDirectory;
                string path = currentDirectory + "\\" + Constant.urlStoreFileName;
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
                else
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        if (fileStream.Length > 0L)
                        {
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            object obj = binaryFormatter.Deserialize(fileStream);
                            if (obj is ObservableCollection<UrlEntity>)
                            {
                                this.urlEntityList = (obj as ObservableCollection<UrlEntity>);
                            }
                        }
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

        #region 重新加载uri存储文件

        private void ReSave()
        {
            try
            {
                string currentDirectory = Environment.CurrentDirectory;
                string path = currentDirectory + "\\" + Constant.urlStoreFileName;
                if (!File.Exists(path))
                {
                    File.Create(path);
                }
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Write))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, this.urlEntityList);
                    fileStream.Flush();
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

        #region 导航到指定地址

        void btnNavicate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //网址导航
                this.webBrowser.Navigate(this.txtInput.Text.Trim());
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
    }
}
