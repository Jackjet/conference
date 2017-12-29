using Conference.Page;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WindowHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using Microsoft.Win32;
using uc = Microsoft.Office.Uc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.OfficeHelper;
using Conference.View.Space;
using ConferenceCommon.FileDownAndUp;
using System.IO;
using Conference.Control;
using ConferenceModel;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using Conference.Common;
using ConferenceCommon.WPFHelper;
using System.Windows.Forms.Integration;

namespace Conference.View.Resource
{
    /// <summary>
    /// 会话管理
    /// </summary>
    public partial class ConversationM : UserControlBase
    {
        #region 内部字段

        /// <summary>
        /// word
        /// </summary>
        WordManage wordManage = new WordManage();

        /// <summary>
        /// excel
        /// </summary>
        ExcelManage excelManage = new ExcelManage();

        /// <summary>
        /// excel
        /// </summary>
        PPTManage pPTManage = new PPTManage();

        /// <summary>
        /// 视频播放器
        /// </summary>
        MediaPlayerView mediaPlayerView = null;

        /// <summary>
        /// 浏览器
        /// </summary>
        WebView webView = null;

        /// <summary>
        /// 图片编辑器
        /// </summary>
        PictureView pictureView = null;

        /// <summary>
        /// 通过owa打开的文件uri后缀名
        /// </summary>
        string owaWebExtentionName = "?Web=1";

        /// <summary>
        /// 会话宿主
        /// </summary>
        ConversationHost conversationHost = new ConversationHost();

        /// <summary>
        /// 当前用户共享桌面时显示内容
        /// </summary>
        ShowSelfDeskTopView showSelfDeskTopView = new ShowSelfDeskTopView();

        /// <summary>
        /// 会话隐藏视图
        /// </summary>
        ConversationHidenView conversationHidenView = new ConversationHidenView();

        #endregion

        #region 一般属性

        ShowType currentShowType = ShowType.None;
        /// <summary>
        /// 当前选择的视图
        /// </summary>
        public ShowType CurrentShowType
        {
            get { return currentShowType; }
            set { currentShowType = value; }
        }

        bool isCanNavicateConversationView = false;
        /// <summary>
        /// 是否可以转到会话视图
        /// </summary>
        public bool IsCanNavicateConversationView
        {
            get { return isCanNavicateConversationView; }
            set { isCanNavicateConversationView = value; }
        }

        #endregion

        #region 绑定属性

        ResourceType pageIndex = ResourceType.Share;
        /// <summary>
        /// 
        /// </summary>
        public ResourceType PageIndex
        {
            get { return pageIndex; }
            set
            {
                pageIndex = value;
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //TimerJob.StartRun(new Action(() =>
                        //    {
                        switch (value)
                        {
                            case ResourceType.Share:
                                this.tabItem1.Focus();
                                //设置会话区域显示内容
                                this.SetConversationAreaShow(ShowType.ConversationView, false);

                                break;
                            case ResourceType.Normal:
                                this.tabItem2.Focus();
                                //设置会话区域显示内容
                                this.SetConversationAreaShow(ShowType.HidenView, false);

                                break;
                            default:
                                break;
                        }
                        //}));

                    }));
            }
        }

        string resourcePusher;
        /// <summary>
        /// 文件推送者
        /// </summary>
        public string ResourcePusher
        {
            get { return resourcePusher; }
            set
            {
                if (resourcePusher != value)
                {
                    resourcePusher = value;
                    this.OnPropertyChanged("ResourcePusher");
                }
            }
        }

        string resourcePresenter;
        /// <summary>
        /// 文件演示者
        /// </summary>
        public string ResourcePresenter
        {
            get { return resourcePresenter; }
            set
            {
                if (resourcePresenter != value)
                {
                    resourcePresenter = value;
                    this.OnPropertyChanged("ResourcePresenter");
                }
            }
        }

        Visibility resourcePusherVisibility = Visibility.Collapsed;
        /// <summary>
        /// 推送者显示
        /// </summary>
        public Visibility ResourcePusherVisibility
        {
            get { return resourcePusherVisibility; }
            set
            {
                if (resourcePusherVisibility != value)
                {
                    resourcePusherVisibility = value;
                    this.OnPropertyChanged("ResourcePusherVisibility");
                }
            }
        }

        Visibility resourcePresenterVisibility = Visibility.Collapsed;
        /// <summary>
        /// 演示者显示
        /// </summary>
        public Visibility ResourcePresenterVisibility
        {
            get { return resourcePresenterVisibility; }
            set
            {
                if (resourcePresenterVisibility != value)
                {
                    resourcePresenterVisibility = value;
                    this.OnPropertyChanged("ResourcePresenterVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        public ConversationM()
        {
            try
            {
                //UI加载
                InitializeComponent();

                //绑定当前上下文
                this.DataContext = this;

                //本地资源共享
                this.btnLocalResource.Click += btnLocalResource_Click;
                ////退出全屏
                //this.btnExitFullScreen.Click += btnExitFullScreen_Click;
                ////开启全屏
                //this.btnFullScreen.Click += btnFullScreen_Click;
                //电子白板
                this.btnWhiteboard.Click += btnWhiteboard_Click;

                this.btnPostil.Click += btnPostil_Click;
                //桌面共享
                this.btnDeskShare.Click += btnDeskShare_Click;
                //接任演示
                this.btnDemonstration.Click += btnDemonstration_Click;
                //手机投影
                this.btnprojection.Click += btnprojection_Click;
                //选项卡更改事件（演示,推送）
                this.tabControl.SelectionChanged += tabControl_SelectionChanged;

                //this.btnNext2.Click += btnNext_Click;

                //初始化加载
                //this.ParameterInit();

                //检测automation是否弹出的情况
                LyncHelper.CheckAutomationIsOpenAndActive(this.conversationHost.panel.Handle);

                //默认显示
                this.SetConversationAreaShow(ShowType.HidenView, false);
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

        #region 选项卡更改事件（演示,推送）

        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                this.PageIndex = (ResourceType)this.tabControl.SelectedIndex;
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

        #region 初始化加载

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ParameterInit()
        {
            try
            {
                #region old solution

                ////设置会话管理窗体为顶层
                //ConversationM.ConversationManageWindow.Topmost = true;
                ////设置悬浮球位置1
                //this.SettingConversationManageWindowPosition();
                ////显示会话管理窗体
                //ConversationM.ConversationManageWindow.Show();



                //System.Windows.Forms.Integration.ElementHost host = new System.Windows.Forms.Integration.ElementHost() { };
                //host.Child = new KeyControlView() { };
                //this.winHost3.Child = host;

                #endregion
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 获取列的宽度

        /// <summary>
        /// 获取第三列的宽度
        /// </summary>
        /// <returns></returns>
        public double GetColumnWidth()
        {
            double width = 0;
            try
            {
                //width = this.colum3.ActualWidth;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            return width;
        }

        #endregion

        #region 加载会话窗体

        /// <summary>
        /// 加载会话窗体
        /// </summary>
        /// <param name="window">指定窗体</param>
        public void DockConversationWindow(LyncClient lyncClient, ConversationWindow window)
        {
            try
            {
                Application.Current.MainWindow.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (!window.IsDocked)
                        {
                            //获取工作区域的宽度
                            int borWidth = MainWindow.MainPageInstance.GetWorkingArea_Width();
                            //获取工作区域的高度
                            int borHeight = Conference.MainWindow.MainPageInstance.GetWorkingArea_Height();

                            WindowsFormsHost host = this.conversationHost.winHost;
                            System.Windows.Forms.Panel panel = this.conversationHost.panel;
                            host.Width = panel.Width = borWidth - 110;
                            host.Height = panel.Height = borHeight - 30;
                            LyncHelper.DockToNewParentWindow(panel.Handle, this.DockInit);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    };
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        public void DockInit(bool isSuccessed)
        {
            try
            {
                if (isSuccessed)
                {
                    DispatcherTimer timer = null;
                    TimerJob.StartRun(new Action(() =>
                    {
                        LyncHelper.ShowWindowContent();
                        bool result = LyncHelper.FullScreen();
                        if (result)
                        {
                            if (LyncHelper.MainConversation.IsFullScreen)
                            {
                                timer.Stop();
                                this.IsCanNavicateConversationView = true;
                                this.SetConversationAreaShow(ShowType.ConversationView, false);
                            }
                        }
                    }), 1000, out timer);
                }
                else
                {
                    System.Windows.Forms.Panel panel = this.conversationHost.panel;
                    LyncHelper.DockToNewParentWindow(panel.Handle, this.DockInit);
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

        #region 设置会话区域显示内容



        /// <summary>
        /// 设置会话区域显示内容
        /// </summary>
        /// <param name="showType"></param>
        public void SetConversationAreaShow(ShowType showType, bool forceChange)
        {
            try
            {
                bool isChanged = false;

                switch (showType)
                {
                    case ShowType.ConversationView:
                        if (forceChange)
                        {
                            isChanged = this.SetConversationArea_Conversation();
                        }
                        else if (this.CurrentShowType != ShowType.SelfDeskTopShowView)
                        {
                            isChanged = this.SetConversationArea_Conversation();
                        }
                        else if (this.CurrentShowType == ShowType.SelfDeskTopShowView)
                        {
                            isChanged = false;
                            return;
                        }

                        break;
                    case ShowType.SelfDeskTopShowView:
                        if (forceChange)
                        {
                            isChanged = this.SetConversationArea_SelfDeskTopShowView();
                        }

                        break;
                    case ShowType.HidenView:
                        if (forceChange)
                        {
                            isChanged = this.SetConversationArea_HidenView();
                        }
                        else if (this.CurrentShowType != ShowType.SelfDeskTopShowView)
                        {
                            isChanged = this.SetConversationArea_HidenView();
                        }
                        else if (this.CurrentShowType == ShowType.SelfDeskTopShowView)
                        {
                            isChanged = false;
                            return;
                        }

                        break;
                    default:
                        break;
                }

                if (isChanged)
                {
                    //设置当前选择视图类型
                    this.CurrentShowType = showType;
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

        /// <summary>
        /// 会话视图显示
        /// </summary>
        public bool SetConversationArea_Conversation()
        {
            bool isChanged = false;
            try
            {
                if (this.IsCanNavicateConversationView)
                {
                    if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized && LyncHelper.MainConversation.IsDocked)
                    {
                        //double width = this.conversationHost.ActualWidth;
                        //double height = this.conversationHost.ActualHeight;

                        //this.conversationHost.Width = 1;
                        //this.conversationHost.Height = 1;

                        LyncHelper.ShowWindowContent();
                        LyncHelper.FullScreen();

                        this.borConversation.Child = null;
                        this.borConversation.Child = this.conversationHost;



                        //this.conversationHost.Width = width;
                        //this.conversationHost.Height = height;

                        //封装的会话窗体内部实例
                        //uc.ConversationWindowClass conversationWindowClass = LyncHelper.MainConversation.InnerObject as uc.ConversationWindowClass;
                        //if (conversationWindowClass != null)
                        //{
                        //    conversationWindowClass.SetPreferredAnnotationTool(uc.AnnotationTool.ucAnnotationToolDrawArrowLine, 0x000000FF);                            
                        //}

                        isChanged = true;
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
            return isChanged;
        }

        public bool SetConversationArea_Conversation2()
        {
            bool isChanged = false;
            try
            {
                if (this.isCanNavicateConversationView)
                {
                    if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized && LyncHelper.MainConversation.IsDocked)
                    {

                        this.borConversation.Child = null;
                        this.borConversation.Child = this.conversationHost;

                        LyncHelper.ShowWindowContent();
                        LyncHelper.FullScreen();

                        isChanged = true;
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
            return isChanged;
        }

        /// <summary>
        /// 会话视图显示
        /// </summary>
        public bool SetConversationArea_HidenView()
        {
            bool isChanged = false;
            try
            {
                this.borConversation.Child = null;
                LyncHelper.HidenWindowContent();
                this.borConversation.Child = this.conversationHidenView;

                isChanged = true;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return isChanged;
        }

        /// <summary>
        /// 会话视图显示
        /// </summary>
        public bool SetConversationArea_SelfDeskTopShowView()
        {
            bool isChanged = false;
            try
            {

                this.borConversation.Child = null;
                LyncHelper.HidenWindowContent();
                this.borConversation.Child = this.showSelfDeskTopView;
                //

                isChanged = true;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return isChanged;
        }

        #endregion

        #region 设置悬浮球的位置

        /// <summary>
        /// 设置悬浮球位置1
        /// </summary>
        public void SettingConversationManageWindowPosition()
        {
            try
            {
                //ConversationM.ConversationManageWindow.Top = 80;
                //ConversationM.ConversationManageWindow.Left = System.Windows.SystemParameters.WorkArea.Width - ConversationM.ConversationManageWindow.Width - 60;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 设置悬浮球位置2
        /// </summary>
        public void SettingConversationManageWindowPosition2()
        {
            try
            {
                //ConversationM.ConversationManageWindow.Top = 80 + 70;
                //ConversationM.ConversationManageWindow.Left = System.Windows.SystemParameters.WorkArea.Width - ConversationM.ConversationManageWindow.Width - 60;

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 设置悬浮球的显示

        /// <summary>
        /// 设置悬浮球显示
        /// </summary>
        public void SettingConversationManageWindowDisplay()
        {
            try
            {
                //ConversationM.ConversationManageWindow.Visibility = System.Windows.Visibility.Visible;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 设置悬浮球隐藏
        /// </summary>
        public void SettingConversationManageWindowHidden()
        {
            try
            {
                //ConversationM.ConversationManageWindow.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 将scroll容器滚动条置顶

        /// <summary>
        /// 将scroll容器滚动条置顶
        /// </summary>
        public void ScrollToBottom()
        {
            try
            {
                //this.scroll.ScrollToEnd();
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

        #region 电子白板

        /// <summary>
        /// 电子白板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnWhiteboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //共享前释放资源
                    this.ShareBeforeDisposeResrouce();
                    //共享电子白板
                    LyncHelper.ShareWhiteboard(LyncHelper.MainConversation, Constant.SelfName);
                }
                else
                {
                    MessageBox.Show("使用电子白板共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 显示批注

        void btnPostil_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CurrentShowType == ShowType.ConversationView)
                {
                    this.SetConversationAreaShow(ShowType.HidenView, false);
                    LyncHelper.HidenWindowContent();
                    bool result = this.SetConversationArea_Conversation2();
                    if (result)
                    {
                        this.CurrentShowType = ShowType.ConversationView;
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

        #region 桌面共享

        /// <summary>
        /// 桌面共享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDeskShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ShareDesk();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        public void ShareDesk()
        {
            try
            {
                if (Constant.lyncClient.State == ClientState.SignedIn)
                {
                    if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                    {
                        //共享前释放资源
                        this.ShareBeforeDisposeResrouce();
                        //var dd =  LyncHelper.MainConversation.Conversation.SelfParticipant.Conversation.Modalities[ModalityTypes.ApplicationSharing];
                        var application = LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing];
                        //((ApplicationSharingModality)LyncHelper.MainConversation.Conversation.SelfParticipant.Conversation.Modalities[ModalityTypes.ApplicationSharing]).BeginShareDesktop(null,null);
                        ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)application);
                        if (applicationSharingModality != null)
                        {
                            applicationSharingModality.BeginShareDesktop(null, null);
                        }
                        //MainWindow.MainPageInstance.ConversationM.PageIndex = ResourceType.normal;

                        //主窗体最小化
                        MainWindow.mainWindow.WindowState = WindowState.Minimized;

                        ModelManage.ConferenceLyncConversation.EnterBigScreen(Constant.ConferenceName, Constant.SelfName, new Action<bool>((result) =>
                        {
                            if (result)
                            {

                            }
                        }));
                    }
                    else
                    {
                        //MessageBox.Show("使用桌面共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void DisConnectDeskShare()
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    var application = LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing];
                    //((ApplicationSharingModality)LyncHelper.MainConversation.Conversation.SelfParticipant.Conversation.Modalities[ModalityTypes.ApplicationSharing]).BeginShareDesktop(null,null);
                    ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)application);
                    if (applicationSharingModality != null)
                    {
                        applicationSharingModality.BeginDisconnect(ModalityDisconnectReason.None, null, null);
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

        #region 手机投影

        void btnprojection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                #region old solution

                //ModelManage.ConferenceLyncConversation.EnterBigScreen(Constant.ConferenceName, "abc@st.com", new Action<bool>((result) =>
                //{
                //    if (result)
                //    {
                //    }
                //}));

                //ModelManage.ConferenceLyncConversation.EnterBigScreen(Constant.ConferenceName, Constant.SelfUri, new Action<bool>((result) =>
                //{
                //    if (result)
                //    {

                //    }
                //}));

                #endregion

                //将窗体最小化
                MainWindow.mainWindow.WindowState = WindowState.Minimized;
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

        #region 本地资源共享

        /// <summary>
        /// 本地资源共享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLocalResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //打开选项对话框
                    OpenFileDialog dialog = new OpenFileDialog();
                    //指定显示的文件类型
                    //dialog.Filter = "所有文件";
                    //设置为多选
                    dialog.Multiselect = false;
                    if (dialog.ShowDialog() == true)
                    {
                        //获取扩展名
                        string fileExtension = System.IO.Path.GetExtension(dialog.FileName);
                        if (fileExtension.Equals(".pptx") || fileExtension.Equals(".ppt"))
                        {
                            //共享前释放资源
                            this.ShareBeforeDisposeResrouce();
                            //打开ppt共享辅助
                            PPtShareHelper(dialog.FileName);
                        }
                        else
                        {


                            //获取文件名称（不包含扩展名称）
                            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(dialog.FileName);
                            ////文件移动
                            //System.IO.File.Copy(dialog.FileName, Constant.LocalTempRoot + "\\" + fileNameWithoutExtension + fileExtension, true);
                            //Thread.Sleep(1000);
                            //通过进程打开一个文件
                            ProcessManage.OpenFileByLocalAddressReturnHandel(dialog.FileName, new Action<int, IntPtr>((processID, intptr) =>
                                {
                                    //获取共享模型
                                    var shareModality = ((ApplicationSharingModality)(LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing]));

                                    //遍历可以共享的资源
                                    foreach (var item in shareModality.ShareableResources)
                                    {
                                        //&& intptr != new IntPtr(0)
                                        //指定要共享的程序名称
                                        if (item.Id.Equals(processID))
                                        {
                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                        else if (item.Id == intptr.ToInt32())
                                        {
                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                        else if (item.Name.Contains(fileNameWithoutExtension))
                                        {

                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                    }
                                }));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("使用桌面共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        public void ShareAndSync(IntPtr intptr, ApplicationSharingModality shareModality, SharingResource item)
        {
            try
            {
                //共享程序置顶
                Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                //开始共享该程序
                shareModality.BeginShareResources(item, null, null);
                //同步页面
                Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
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
        /// 打开ppt共享辅助
        /// </summary>
        /// <param name="fileName"></param>
        public static void PPtShareHelper(string fileName)
        {
            try
            {
                //封装的会话窗体内部实例
                uc.ConversationWindowClass conversationWindowClass = LyncHelper.MainConversation.InnerObject as uc.ConversationWindowClass;

                if (conversationWindowClass != null)
                {
                    //开启ppt
                    conversationWindowClass.GetConversationWindowActions().AddOfficePowerPointToConversation(fileName, Constant.LyncIP1 + Constant.ServicePPTTempFile + System.IO.Path.GetFileName(fileName));
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConversationM), ex);
            }
        }

        #endregion

        #region 接任演示

        /// <summary>
        /// 接任演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDemonstration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var resourceList = MainWindow.MainPageInstance.ShareableContentList;
                if (LyncHelper.MainConversation != null && resourceList != null)
                {
                    foreach (var item in resourceList)
                    {
                        if (item.State == ShareableContentState.Active)
                        {
                            item.Present();
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

        #region 共享前释放资源

        /// <summary>
        /// 共享前释放资源
        /// </summary>
        public void ShareBeforeDisposeResrouce()
        {
            try
            {
                //int reson = 0;
                //foreach (var item in MainWindow.MainPageInstance.ShareableContentList)
                //{
                //    //状态为连接
                //    if (item.IsActive && item.CanInvoke(ShareableContentAction.StopPresenting, out reson))
                //    {
                //        item.StopPresenting();
                //        break;
                //    }
                //}
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

        #region 资源推送区域

        /// <summary>
        /// 根据文件的类型使用相应的方式打开
        /// </summary>
        public void FileOpenByExtension(FileType fileType, string uri)
        {
            try
            {
                switch (fileType)
                {
                    case FileType.docx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.doc:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.xlsx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.Txt:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.PPtx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.PPt:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.one:
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.Mp4:
                        this.OpenVideoAudioFile(uri);

                        break;
                    case FileType.avi:
                        this.OpenVideoAudioFile(uri);

                        break;
                    case FileType.mp3:
                        this.OpenVideoAudioFile(uri);

                        break;

                    case FileType.Jpg:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Png:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Ico:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Xml:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.txt:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.record:
                        this.OpenRecordFile(uri);

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #region 文件类型

        /// <summary>
        /// 通过浏览器打开文件
        /// </summary>
        public void OpenFileByBrowser(string uri)
        {
            try
            {

                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();
                if (this.webView == null)
                {
                    this.webView = new WebView(uri);
                }
                else
                {
                    this.webView.OpenUri(uri);
                }
                //加载UI事件（比如多媒体播放器,浏览器）
                this.book_LoadUI(this.webView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开图片文件
        /// </summary>
        /// <param name="uri"></param>
        public void OpenPictureFile(string uri)
        {
            try
            {
                //获取文件名称（包含扩展名称）
                var fileName = System.IO.Path.GetFileName(uri);
                //本地地址
                var loaclF = Constant.LocalTempRoot + "\\" + fileName;
                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();

                //文件下载
                webClientManage.FileDown(uri, loaclF, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    try
                    {
                        if (IsSuccessed)
                        {
                            if (File.Exists(loaclF))
                            {
                                wordManage.ClearDocuments();
                                pPTManage.ClearDocuments();
                                excelManage.ClearDocuments();
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    try
                                    {
                                        if (this.pictureView == null)
                                        {
                                            this.pictureView = new PictureView(loaclF);
                                        }
                                        else
                                        {
                                            this.pictureView.OpenUri(loaclF);
                                        }
                                        //加载UI事件（比如多媒体播放器,浏览器）
                                        this.book_LoadUI(this.pictureView);
                                    }
                                    catch (Exception ex2)
                                    {
                                        LogManage.WriteLog(this.GetType(), ex2);
                                    };
                                }));
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        LogManage.WriteLog(this.GetType(), ex2);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开office文件
        /// </summary>
        /// <param name="uri"></param>
        void OpenOfficeFile(string uri)
        {
            try
            {
                uri = uri + this.owaWebExtentionName;
                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();
                if (this.webView == null)
                {
                    this.webView = new WebView(uri);
                }
                else
                {
                    this.webView.OpenUri(uri);
                }
                //加载UI事件（比如多媒体播放器,浏览器）
                this.book_LoadUI(this.webView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        /// <summary>
        /// 打开视频文件
        /// </summary>
        public void OpenVideoAudioFile(string uri)
        {
            try
            {
                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();

                if (this.mediaPlayerView == null)
                {
                    this.mediaPlayerView = new MediaPlayerView(uri);
                }
                else
                {
                    this.mediaPlayerView.OpenVideo(uri);
                }
                //加载UI事件（比如多媒体播放器）
                this.book_LoadUI(this.mediaPlayerView);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开record文件
        /// </summary>
        /// <param name="uri"></param>
        public void OpenRecordFile(string uri)
        {
            try
            {
                //获取文件名称（包含扩展名称）
                var fileName = System.IO.Path.GetFileName(uri);
                //本地地址
                var loaclF = Constant.LocalTempRoot + "\\" + fileName;

                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();

                //文件下载
                webClientManage.FileDown(uri, loaclF, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    if (IsSuccessed)
                    {
                        try
                        {
                            if (File.Exists(loaclF))
                            {
                                //通过流去获取文件字符串
                                using (FileStream fs = new FileStream(loaclF, FileMode.Open, FileAccess.Read))
                                {
                                    StreamReader reader = new StreamReader(fs, Encoding.UTF8);
                                    string recordUri = reader.ReadToEnd();
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        try
                                        {
                                            //打开视频文件
                                            this.OpenVideoAudioFile(recordUri);
                                        }
                                        catch (Exception ex2)
                                        {
                                            LogManage.WriteLog(this.GetType(), ex2);
                                        };
                                    }));
                                }
                            }
                        }
                        catch (Exception ex2)
                        {
                            LogManage.WriteLog(this.GetType(), ex2);
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 加载UI事件（比如多媒体播放器）

        /// <summary>
        /// 加载UI事件（比如多媒体播放器）
        /// </summary>
        /// <param name="element"></param>
        public void book_LoadUI(FrameworkElement element)
        {
            try
            {
                //隐藏装饰UI
                this.borDecorate.Visibility = System.Windows.Visibility.Collapsed;
                //显示视频UI
                this.borContent.Visibility = System.Windows.Visibility.Visible;
                //加载视频元素
                this.borContent.Child = element;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #endregion


    }

    public enum ResourceType
    {
        Share = 0,
        Normal = 1,
    }

    public enum ShowType
    {
        ConversationView,
        SelfDeskTopShowView,
        HidenView,
        None,
    }
}



#region old solution

//if (borWidth < 1264)
//{
//    this.winHost.Width = this.panel.Width = 1264;
//}
//else
//{
//this.winHost.Width = this.panel.Width = borWidth - 70;
//}
//if (borHeight < 716)
//{
//    this.winHost.Height = this.panel.Height = 716;
//}
//else
//{
//this.winHost.Height = this.panel.Height = borHeight - 30;
//}

//if(borWidth<1366)
//{
//    borWidth = 1366;
//}
//if(borHeight <768)
//{
//    borHeight = 768;
//}

//LogManage.WriteLog(this.GetType(), string.Format("宽度{0},高度{1}",this.winHost.Width,this.winHost.Height));
//window.Resize((int)this.winHost.Width, (int)this.winHost.Height);

#endregion
