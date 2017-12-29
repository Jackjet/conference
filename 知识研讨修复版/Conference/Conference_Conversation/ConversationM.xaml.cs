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
using ConferenceCommon.FileDownAndUp;
using System.IO;
using ConferenceCommon.WPFHelper;
using System.Windows.Forms.Integration;
using ConferenceCommon.WPFControl;
using Conference_Conversation.Common;
using Conference_Conversation.Control;
using ConferenceCommon.RegeditHelper;
using vy = System.Windows.Visibility;

namespace Conference_Conversation
{
    /// <summary>
    /// 会话管理
    /// </summary>
    public partial class ConversationM : UserControlBase
    {
        #region 自定义委托事件(回调)

        /// <summary>
        /// 加载会话回调
        /// </summary>
        public Action<Action<int, int>> DockConversationWindowCallBack = null;

        /// <summary>
        /// 共享页面同步回调
        /// </summary>
        public Action ShareAndSyncCallBack = null;

        /// <summary>
        /// 修复会话回调
        /// </summary>
        public Action RepairConversationCallBack = null;

        #endregion

        #region 内部字段

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

        /// <summary>
        /// 文件打开管理模块实例
        /// </summary>
        FileOpenManage FileOpenManage = null;

        /// <summary>
        /// 本地资源窗体
        /// </summary>
        ApplicationWindow applicationWindow = null;

        /// <summary>
        /// 共享资源窗体
        /// </summary>
        ConversationResourceWindow conversationResourceWindow = null;

        /// <summary>
        /// 下拉控件集合
        /// </summary>
        List<Expander> expanderList = new List<Expander>();

        #endregion

        #region 一般属性

        List<ShareableContent> shareableContentList = new List<ShareableContent>();
        /// <summary>
        /// 共享的资源集合
        /// </summary>
        public List<ShareableContent> ShareableContentList
        {
            get { return shareableContentList; }
            set { shareableContentList = value; }
        }

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
        /// 页面选择
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

        vy resourcePusherVisibility = vy.Collapsed;
        /// <summary>
        /// 推送者显示
        /// </summary>
        public vy ResourcePusherVisibility
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

        vy resourcePresenterVisibility = vy.Collapsed;
        /// <summary>
        /// 演示者显示
        /// </summary>
        public vy ResourcePresenterVisibility
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

        //vy pPTOperationVisibility = vy.Collapsed;
        ///// <summary>
        ///// ppt操作
        ///// </summary>
        //public vy PPTOperationVisibility
        //{
        //    get { return pPTOperationVisibility; }
        //    set
        //    {
        //        if (pPTOperationVisibility != value)
        //        {
        //            pPTOperationVisibility = value;
        //            this.OnPropertyChanged("PPTOperationVisibility");
        //        }
        //    }
        //}

        #endregion

        #region 静态字段

        /// <summary>
        /// 自我绑定
        /// </summary>
        public static ConversationM conversationM = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConversationM()
        {
            try
            {
                //UI加载
                InitializeComponent();
                //自绑定
                conversationM = this;
                //事件注册
                this.EventRegedit();
                //初始化加载
                this.ParmetersInit();
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

        #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //本地资源共享
                this.btnPPT.Click += btnPPT_Click;
                //电子白板
                this.btnWhiteboard.Click += btnWhiteboard_Click;
                //启动音频
                this.btnAudio.Click += btnAudio_Click;
                //启动视频
                this.btnVideo.Click += btnVideo_Click;

                //显示批注
                this.btnPostil.Click += btnPostil_Click;
                //桌面共享
                this.btnDeskShare.Click += btnDeskShare_Click;
                //接任演示
                this.btnDemonstration.Click += btnDemonstration_Click;
                //手机投影
                //this.btnprojection.Click += btnprojection_Click;
                //选项卡更改事件（演示,推送）
                this.tabControl.SelectionChanged += tabControl_SelectionChanged;
                //关闭音视频
                this.btnAVClose.Click += btnAVClose_Click;
                //修复会话
                this.btnConversationRepair.Click += btnConversationRepair_Click;
                //加载共享类型回调
                LyncHelper.AddContent_Type_CallBack = AddContent_Type_CallBack;
                //移除共享类型回调
                LyncHelper.RemoveContent_Type_CallBack = RemoveContent_Type_CallBack;
                //移除联系人回调
                LyncHelper.Remove_ConversationParticalCallBack = Remove_ConversationParticalCallBack;
                //添加联系人回调
                LyncHelper.Add_ConversationParticalCallBack = Add_ConversationParticalCallBack;
                //本地资源
                this.btnLocalResource.Click += btnLocalResource_Click;
                //共享资源
                this.btnServiceResource.Click += btnServiceResource_Click;
                //音视频操作区域
                this.expanderAV.Expanded += expanderAV_Expanded;
                //联系人列表区域
                this.expanderParticalList.Expanded += expanderAV_Expanded;
                //ppt共享区域
                this.expanderPPT.Expanded += expanderAV_Expanded;
                //资源管理区域
                this.expanderResource.Expanded += expanderAV_Expanded;
                //其他共享区域
                this.expanderOtherShare.Expanded += expanderAV_Expanded;
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

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileUri"></param>
        private void ParmetersInit()
        {
            try
            {
                this.expanderList.Add(this.expanderAV);
                this.expanderList.Add(this.expanderPPT);
                this.expanderList.Add(this.expanderResource);
                this.expanderList.Add(this.expanderParticalList);
                this.expanderList.Add(this.expanderOtherShare);
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

        #region 展开操作区域

        /// <summary>
        /// 展开操作区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void expanderAV_Expanded(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var item in this.expanderList)
                {
                    if (!sender.Equals(item))
                    {
                        item.IsExpanded = false;
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

        #region 选项卡更改事件（演示,推送）

        /// <summary>
        /// 选项卡更改事件（演示,推送）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //页面显示
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
                            if (this.DockConversationWindowCallBack != null)
                            {
                                this.DockConversationWindowCallBack(new Action<int, int>((width, height) =>
                                {

                                    //获取工作区域的宽度
                                    int borWidth = width;
                                    //获取工作区域的高度
                                    int borHeight = height;

                                    WindowsFormsHost host = this.conversationHost.winHost;
                                    System.Windows.Forms.Panel panel = this.conversationHost.panel;
                                    host.Width = panel.Width = borWidth - 110;
                                    host.Height = panel.Height = borHeight - 30;

                                    //panel.MinimumSize = new System.Drawing.Size() { Height = 696 };

                                    //if(borHeight<715)
                                    //{
                                    //    host.Height = panel.Height = borHeight - 20;
                                    //}
                                    //else
                                    //{
                                    //    host.Height = panel.Height = borHeight - 30;
                                    //}

                                    LyncHelper.DockToNewParentWindow(panel.Handle, this.DockInit);
                                }));
                            }
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

        /// <summary>
        /// 嵌入初始化
        /// </summary>
        /// <param name="isSuccessed"></param>
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
                       
                        LyncHelper.ShowWindowContent();
                        LyncHelper.FullScreen();

                        this.borConversation.Child = null;
                        this.borConversation.Child = this.conversationHost;
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
        /// 会话视图显示2
        /// </summary>
        /// <returns></returns>
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
        /// 会话视图显示（会话隐藏视图）
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
        /// 会话视图显示（桌面共享）
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

        #region 移除共享类型回调

        /// <summary>
        /// 移除共享类型回调
        /// </summary>
        private void RemoveContent_Type_CallBack(SharingType type)
        {
            try
            {
                if (this.conversationResourceWindow != null)
                {
                    this.conversationResourceWindow.RefleshViewByMesuare();
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

        #region 加载共享类型回调

        /// <summary>
        /// 加载共享类型回调
        /// </summary>
        private void AddContent_Type_CallBack(SharingType type)
        {
            try
            {
                //if (type == SharingType.ppt)
                //{
                //    this.PPTOperationVisibility = vy.Visible;
                //}
                //else
                //{
                //    this.PPTOperationVisibility = vy.Collapsed;

                //}
                if (this.conversationResourceWindow != null)
                {
                    this.conversationResourceWindow.RefleshViewByMesuare();
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

        #region 共享资源

        /// <summary>
        /// 共享资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnServiceResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.conversationResourceWindow == null)
                {
                    this.conversationResourceWindow = new ConversationResourceWindow();
                    this.conversationResourceWindow.Show();
                }
                else
                {
                    this.conversationResourceWindow.ShowOrHiddenTheView();
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

        #region 本地资源演示

        /// <summary>
        /// 本地资源演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLocalResource_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.applicationWindow == null)
                {
                    this.applicationWindow = new ApplicationWindow();
                    this.applicationWindow.Show();
                }
                else
                {
                    this.applicationWindow.ShowOrHiddenTheView();
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

        #region 隐藏所有资源管理窗体

       /// <summary>
        /// 隐藏所有资源管理窗体
       /// </summary>
        public void HidenAllResourceWindow()
        {
            try
            {
                if (this.applicationWindow != null)
                {
                    this.applicationWindow.HiddenTheView();
                }
                if (this.conversationResourceWindow != null)
                {
                    this.conversationResourceWindow.HiddenTheView();
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

        #region 添加\移除联系人回调       

        /// <summary>
        /// 添加联系人回调
        /// </summary>
        private void Add_ConversationParticalCallBack(Participant participant)
        {
            try
            {
                this.partical_View.AddItem(participant);
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
        /// 移除联系人回调
        /// </summary>
        private void Remove_ConversationParticalCallBack(Participant participant)
        {
            try
            {
                this.partical_View.RemoveItem(participant);
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

        #region 初始化人员列表
        
        /// <summary>
        /// 初始化人员列表
        /// </summary>
        public void ParticalListInit()
        {
            try
            {
                this.partical_View.ParticalListInit();
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
