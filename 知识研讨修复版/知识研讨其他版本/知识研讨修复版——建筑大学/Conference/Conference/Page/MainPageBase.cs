using Conference.Common;
using Conference.View.AppcationTool;
using Conference.View.Chair;
using Conference.View.ConferenceRoom;
using Conference.View.IMM;
using Conference.View.MyConference;
using Conference.View.Note;
using Conference.View.Resource;
using Conference.View.Setting;
using Conference.View.Space;
using Conference.View.Studiom;
using Conference.View.Tool;
using Conference.View.Tree;
using Conference.View.U_Disk;
using Conference.View.WebBrowser;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.ApplicationHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.VersionHelper;
using ConferenceCommon.WebHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using ConferenceModel.ConferenceInfoWebService;
using ConferenceModel.FileSyncAppPoolWebService;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;

namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 自定义委托事件

        public delegate void ViewModelChangedEducationEventHandle(bool result);
        /// <summary>
        /// 教学模式更改事件
        /// </summary>
        protected event ViewModelChangedEducationEventHandle ViewModelChangedEducationEvent = null;

        public delegate void ViewModelChangedSimpleEventHandle(bool result);
        /// <summary>
        /// 精简模式更改事件
        /// </summary>
        protected event ViewModelChangedSimpleEventHandle ViewModelChangedSimpleEvent = null;

        public delegate void ForceNavicateEventHandle(ConferenceCommon.EnumHelper.ViewSelectedItemEnum selectedItem);
        /// <summary>
        /// 页面导航事件
        /// </summary>
        public event ForceNavicateEventHandle ForceToNavicateEvent = null;

        #endregion

        #region 内部字段

        /// <summary>
        /// 知识树服务对应套接字
        /// </summary>
        ClientSocket TreeClientSocket = new ClientSocket();

        /// <summary>
        /// 语音服务对应套接字
        /// </summary>
        ClientSocket AudioClientSocket = new ClientSocket();

        /// <summary>
        /// 消息服务对应套接字
        /// </summary>
        ClientSocket InfoClientSocket = new ClientSocket();

        /// <summary>
        /// 文件服务对应套接字【甩屏】
        /// </summary>
        ClientSocket FileClientSocket = new ClientSocket();

        /// <summary>
        /// Lync服务对应套接字
        /// </summary>
        ClientSocket LyncClientSocket = new ClientSocket();

        /// <summary>
        /// Office服务对应套接字
        /// </summary>
        ClientSocket SpaceClientSocket = new ClientSocket();

        /// <summary>
        /// 矩阵服务对应套接字
        /// </summary>
        ClientSocket MatrixClientSocket = new ClientSocket();

        /// <summary>
        /// 会话窗体状态管理模型
        /// </summary>
        //ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 投影模型（状态设置）
        /// </summary>
        //ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement2 = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 甩屏接收窗体
        /// </summary>
        AppSyncDataAcceptWindow AppSyncDataAcceptWindow;

        /// <summary>
        /// 之前选中的导航按钮
        /// </summary>
        protected Button beforeSelectedButton = null;

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object obj1 = new object();

        /// <summary>
        /// 计时器（信息交流）
        /// </summary>
        protected DispatcherTimer IMMFlashTimer = null;

        /// <summary>
        /// 计时器(资源共享)
        /// </summary>
        protected DispatcherTimer ResourceFlashTimer = null;

        /// <summary>
        /// 会话检查计时器
        /// </summary>
        protected DispatcherTimer CheckConversationInitTimer = null;

        /// <summary>
        /// 检测释放所有通讯节点
        /// </summary>
        public DispatcherTimer CheckDisposeAllSocketTimer = null;

        /// <summary>
        /// 检测是否释放所有通讯节点
        /// </summary>
        bool IsDisposeAllSocekt = false;

        /// <summary>
        /// 远程服务器端口（tree,conference）
        /// </summary>
        int intPortNow = 0;

        /// <summary>
        /// web凭据管理模型
        /// </summary>
        //WebCredentialManage manage = null;

        /// <summary>
        /// 打开lync会话地址
        /// </summary>
        System.Windows.Forms.WebBrowser webBrowser = null;

        /// <summary>
        /// 是否可以开始进行会话初始化
        /// </summary>
        bool canBeginLyncConversationInit = false;

        /// <summary>
        /// 检测是否可以会话初始化计时数量
        /// </summary>
        int CheckConversationInitCount = 0;

        /// <summary>
        /// 当前网络状态
        /// </summary>
        protected NetWorkErrTipType currentNetWorkState = NetWorkErrTipType.Undefined;

        #endregion

        #region 一般属性【运用到了单例模式】

        protected SpaceView spaceView = null;
        /// <summary>
        /// 会议空间
        /// </summary>
        public SpaceView SpaceView
        {
            get
            {
                //创建单例
                if (this.spaceView == null)
                {
                    this.spaceView = new SpaceView();
                }
                return spaceView;
            }
        }

        ConferenceRoom_View conferenceRoom_View = null;
        ///// <summary>
        ///// 会议室
        ///// </summary>
        public ConferenceRoom_View ConferenceRoom_View
        {
            get
            {
                //创建单例
                if (this.conferenceRoom_View == null)
                {
                    this.conferenceRoom_View = new ConferenceRoom_View();
                }
                return conferenceRoom_View;
            }
            //set
            //{

            //}
        }

        ApplicationToolView applicationToolView;
        /// <summary>
        /// 应用工具
        /// </summary>
        public ApplicationToolView ApplicationToolView
        {
            get
            {   //创建单例
                if (this.applicationToolView == null)
                {
                    this.applicationToolView = new ApplicationToolView();
                }
                return applicationToolView;
            }
        }

        protected Setting_View setting_View;
        /// <summary>
        /// 系统设置
        /// </summary>
        public Setting_View Setting_View
        {
            get
            {   //创建单例
                if (this.setting_View == null)
                {
                    this.setting_View = new Setting_View();
                }
                return setting_View;
            }
        }

        ConversationM conversationM;
        /// <summary>
        /// 资源共享
        /// </summary>
        public ConversationM ConversationM
        {
            get
            {   //创建单例
                if (this.conversationM == null)
                {
                    this.conversationM = new ConversationM();
                }
                return conversationM;
            }
        }

        protected ConferenceAudio_View conferenceAudio_View;
        /// <summary>
        /// 信息交流
        /// </summary>
        public ConferenceAudio_View ConferenceAudio_View
        {
            get
            {   //创建单例
                if (this.conferenceAudio_View == null)
                {
                    this.conferenceAudio_View = new ConferenceAudio_View();
                }
                return conferenceAudio_View;
            }
        }

        protected ConferenceTreeView conferenceTreeView;
        /// <summary>
        /// 知识树
        /// </summary>
        public ConferenceTreeView ConferenceTreeView
        {
            get
            {   //创建单例
                if (this.conferenceTreeView == null)
                {
                    this.conferenceTreeView = new ConferenceTreeView();
                }
                return conferenceTreeView;
            }
            set
            {
                conferenceTreeView = value;
            }
        }

        ToolCmWindow toolCmWindow;
        /// <summary>
        /// 工具箱弹出框
        /// </summary>
        public ToolCmWindow ToolCmWindow
        {
            get
            {   //创建单例
                if (this.toolCmWindow == null)
                {
                    this.toolCmWindow = new ToolCmWindow();
                }
                return toolCmWindow;
            }
        }

        SharingPanel sharingPanel = null;
        /// <summary>
        /// 共享面板
        /// </summary>
        public SharingPanel SharingPanel
        {
            get
            {
                //创建单例
                if (this.sharingPanel == null)
                {
                    this.sharingPanel = new SharingPanel();
                }
                return sharingPanel;
            }
        }

        protected PersonalNote personalNote;
        /// <summary>
        /// 个人笔记
        /// </summary>
        public PersonalNote PersonalNote
        {
            get
            {   //创建单例
                if (this.personalNote == null)
                {
                    this.personalNote = new PersonalNote();
                }
                return personalNote;
            }
        }

        ViewSelectedItemEnum viewSelectedItemEnum;
        /// <summary>
        /// 工具箱选择项
        /// </summary>
        public ViewSelectedItemEnum ViewSelectedItemEnum
        {
            get { return viewSelectedItemEnum; }
            set { viewSelectedItemEnum = value; }
        }

        protected MyConferenceView myConferenceView;
        /// <summary>
        /// 我的会议
        /// </summary>
        public MyConferenceView MyConferenceView
        {
            get
            {   //创建单例
                if (this.myConferenceView == null)
                {
                    this.myConferenceView = new MyConferenceView();
                }
                return myConferenceView;
            }
        }

        protected WebBrowserView webBrowserView;
        /// <summary>
        /// 会议投票
        /// </summary>
        public WebBrowserView WebBrowserView
        {
            get
            {   //创建单例
                if (this.webBrowserView == null)
                {
                    this.webBrowserView = new WebBrowserView();
                }
                return webBrowserView;
            }
        }

        protected U_DiskView u_DiskView;
        /// <summary>
        /// U盘传输
        /// </summary>
        public U_DiskView U_DiskView
        {
            get
            {   //创建单例
                if (this.u_DiskView == null)
                {
                    this.u_DiskView = new U_DiskView();
                }
                return u_DiskView;
            }
        }

        protected ChairView chairView;
        /// <summary>
        /// 主持人功能
        /// </summary>
        public ChairView ChairView
        {
            get
            {   //创建单例
                if (this.chairView == null)
                {
                    this.chairView = new ChairView();
                }
                return chairView;
            }
        }

        StudiomView studiomView;
        /// <summary>
        /// 主持人功能
        /// </summary>
        public StudiomView StudiomView
        {
            get
            {   //创建单例
                if (this.studiomView == null)
                {
                    this.studiomView = new StudiomView();
                }
                return studiomView;
            }
        }

        QRWindow qRWindow;
        /// <summary>
        /// 二维码窗体
        /// </summary>
        public QRWindow QRWindow
        {
            get
            {   //创建单例
                if (this.qRWindow == null)
                {
                    this.qRWindow = new QRWindow();
                }
                return qRWindow;
            }
        }


        List<ShareableContent> shareableContentList = new List<ShareableContent>();
        /// <summary>
        /// 共享的资源集合
        /// </summary>
        public List<ShareableContent> ShareableContentList
        {
            get { return shareableContentList; }
            set { shareableContentList = value; }
        }

        //string meetAddressMain;
        ///// <summary>
        ///// 主会议地址
        ///// </summary>
        //public string MeetAddressMain
        //{
        //    get { return meetAddressMain; }
        //    set { meetAddressMain = value; }
        //}

       

        #endregion

        #region 静态字段

        ///// <summary>
        ///// 主会议
        ///// </summary>
        //public static ConversationWindow MainConversation = null;

        /// <summary>
        /// 主会话联系人列表（实际）
        /// </summary>
        public static ObservableCollection<string> participanetList = new ObservableCollection<string>();

        //临时存储的会议信息
        public ConferenceInformationEntityPC TempConferenceInformationEntity = null;

        #endregion

        #region 绑定属性

        string meetingName = string.Empty;
        /// <summary>
        /// 研讨会议名称
        /// </summary>
        public string MeetingName
        {
            get { return meetingName; }
            set
            {
                if (this.meetingName != value)
                {
                    this.meetingName = value;
                    this.OnPropertyChanged("MeetingName");
                }
            }
        }

        #endregion

        #region 加入当前参与的一个会议

        /// <summary>
        /// 加入当前参与的一个会议
        /// </summary>
        protected void JoinConfereince(Action action)
        {
            try
            {

                this.ConferenceRoom_View.ConferenceRoomFlesh(new Action(() =>
                 {
                     //会议开始时间和当前时间的间距
                     double timeSpace = 0;

                     //遍历所有会议信息（获取最接近当前时间的会议）
                     foreach (var item in this.ConferenceRoom_View.ConferenceInformationEntityList)
                     {
                         if (item.BeginTime < DateTime.Now.AddMinutes(-30))
                         {
                             //获取当前会议开始时间与当前时间的时间差
                             var timeSpace1 = (DateTime.Now - item.BeginTime).TotalSeconds;

                             if (timeSpace == 0 && item.JoinPeople.Contains(Constant.SelfUri))
                             {
                                 timeSpace = timeSpace1;
                                 TempConferenceInformationEntity = item;
                             }
                             //将更接近当前时间的会议做一个标示
                             else if (timeSpace1 < timeSpace && item.JoinPeople.Contains(Constant.SelfUri))
                             {
                                 timeSpace = timeSpace1;
                                 //设置为临时存储的会议
                                 TempConferenceInformationEntity = item;
                             }
                         }
                     }

                     MainWindow.MainPageInstance.MyConferenceView.TipShow(true);
                     //会议进入事件
                     this.conferenceRoom_View_ItemClick(TempConferenceInformationEntity, new Action(() =>
                     {
                         //调用回调事件
                         action();
                         MainWindow.MainPageInstance.MyConferenceView.TipShow(false);
                         //填充参会人信息
                         MainWindow.MainPageInstance.MyConferenceView.FillDataSource();

                         //强制更改样式
                         MainWindow.MainPageInstance.ConferenceRoom_View.RoomItemForceToSelectedStyle(TempConferenceInformationEntity);
                     }));
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

        #region 工具箱高级面板显示设置

        /// <summary>
        /// 工具箱高级面板显示设置
        /// </summary>
        protected void ToolPopupDisplay()
        {
            try
            {
                //工具箱弹出框显示设置
                if (this.ToolCmWindow.Visibility == System.Windows.Visibility.Collapsed)
                {
                    //鼠标单击显示
                    this.ToolCmWindow.Visibility = System.Windows.Visibility.Visible;
                    //显示
                    this.ToolCmWindow.Show();
                }
                else
                {
                    //鼠标双击隐藏
                    this.ToolCmWindow.Visibility = System.Windows.Visibility.Collapsed;
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

        #region 会议图例点击事件

        /// <summary>
        /// 会议进入事件
        /// </summary>
        /// <param name="conferenceInformationEntity"></param>
        protected void conferenceRoom_View_ItemClick(ConferenceInformationEntityPC conferenceInformationEntity, Action callBack)
        {
            try
            {
                if (conferenceInformationEntity != null)
                {
                    #region 是否为教学模式

                    if (conferenceInformationEntity.EducationMode)
                    {
                        if (this.ViewModelChangedEducationEvent != null)
                        {
                            this.ViewModelChangedEducationEvent(true);
                        }
                    }
                    else
                    {
                        if (this.ViewModelChangedEducationEvent != null)
                        {
                            this.ViewModelChangedEducationEvent(false);
                        }
                    }

                    #endregion

                    #region 是否为精简模式

                    if (conferenceInformationEntity.SimpleMode)
                    {
                        if (this.ViewModelChangedSimpleEvent != null)
                        {
                            this.ViewModelChangedSimpleEvent(true);
                        }
                    }
                    else
                    {
                        if (this.ViewModelChangedSimpleEvent != null)
                        {
                            this.ViewModelChangedSimpleEvent(false);
                        }
                    }

                    #endregion

                    #region 释放相关资源（本地套接字、研讨会）

                    //设置为不可进行会话初始化
                    this.canBeginLyncConversationInit = false;

                    //停止保持服务器web服务持续激活（之前使用的）
                    //this.CloseWebServiceAppPoolAliveHelper();

                    //参会人信息清空
                    Constant.DicParticipant.Clear();

                    //离开坐席
                    //ChairView.LeaveSeat();
                    //离开坐席
                    MyConferenceView.LeaveSeat();

                    //释放资源
                    this.UIAndSourceDispose();

                    //检测是否可以会话初始化计时数量置零
                    this.CheckConversationInitCount = 0;

                    //还原默认设置
                    this.ConversationM.CurrentShowType = ShowType.None;
                    this.ConversationM.IsCanNavicateConversationView = false;

                    #endregion

                    #region 释放之前使用的通讯节点

                    this.IsDisposeAllSocekt = false;

                    ThreadPool.QueueUserWorkItem((o) =>
                {

                    //释放之前使用的通讯节点
                    this.DisPoseServerSocketArray(conferenceInformationEntity.MeetingName, new Action<bool>((isCompleate) =>
                        {
                            if (isCompleate)
                            {
                                this.IsDisposeAllSocekt = true;
                            }
                        }));
                });

                    #endregion

                    //全局信息填充及初始化
                    GlobleInfoLoadAndInit(conferenceInformationEntity, callBack);
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        

        #endregion

        #region 全局信息填充及初始化

        /// <summary>
        /// 全局信息填充及初始化
        /// </summary>
        /// <param name="conferenceInformationEntity"></param>
        /// <param name="callBack"></param>
        private void GlobleInfoLoadAndInit(ConferenceInformationEntityPC conferenceInformationEntity, Action callBack)
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                {
                    if (this.IsDisposeAllSocekt)
                    {
                        if (this.CheckDisposeAllSocketTimer != null)
                        {
                            this.CheckDisposeAllSocketTimer.Stop();
                        }

                        #region 填充全局字段（参会人列表、会议名称、会议ID）

                        //参会人列表
                        Constant.ParticipantList = conferenceInformationEntity.JoinPeople.ToList<string>();
                        //参会名称
                        Constant.ConferenceName = conferenceInformationEntity.MeetingName;
                        //会议室名称
                        Constant.ConferenceRoomName = conferenceInformationEntity.RoomName;
                        //会议主持人
                        Constant.ConferenceHost = conferenceInformationEntity.ApplyPeople;
                        if (conferenceInformationEntity.EducationMode)
                        {
                            //主窗体标题2显示
                            MainWindow.mainWindow.MainWindowHeader2 = Constant.EducationTittle1 + Constant.ConferenceName;
                            //设置主窗体标题2
                            MainWindow.mainWindow.MainWindowHeader3 = "     " + Constant.EducationTittle2 + Constant.SelfName;
                        }
                        else
                        {
                            //主窗体标题2显示
                            MainWindow.mainWindow.MainWindowHeader2 = Constant.DiscussTittle1 + Constant.ConferenceName;
                            //设置主窗体标题2
                            MainWindow.mainWindow.MainWindowHeader3 = "     " + Constant.DiscussTittle2 + Constant.SelfName;
                        }
                        //参会ID
                        Constant.MeetingID = conferenceInformationEntity.MeetingID;
                        //网络浏览地址
                        Constant.WebUri = conferenceInformationEntity.WebUri;
                        //显示参会名称
                        this.MeetingName = Constant.ConferenceName;
                        //投影大屏幕
                        Constant.BigScreenName = conferenceInformationEntity.BigScreenName;

                        #endregion

                        #region 会话标示服务器判断移除

                        ModelManage.ConferenceLyncConversation.RemoveConversation(Constant.ConferenceName, new Action<bool>((successed) =>
                        {
                            if (successed)
                            {
                                this.canBeginLyncConversationInit = true;
                            }
                        }));

                        #endregion

                        #region 判断是否为会议主持人(是的话设置全局标示，服务器应用池持续性设置)

                        //判断当前用户是否为主持人（该标示在程序全局上使用）
                        if (Constant.SelfUri.Equals(conferenceInformationEntity.ApplyPeople) || Constant.SelfName.Equals(conferenceInformationEntity.ApplyPeople))
                        {
                            Constant.IsMeetingPresenter = true;
                        }
                        else
                        {
                            Constant.IsMeetingPresenter = false;
                        }

                        #endregion

                        //填充参会人信息
                        LyncHelper.AddContacts();

                        #region 获取服务端口

                        //获取服务端口
                        this.GetServicePort(callBack);

                        #endregion

                        #region 重新进行会话配置

                        TimerJob.StartRun(new Action(() =>
                        {
                            if (this.canBeginLyncConversationInit)
                            {
                                this.CheckConversationInitTimer.Stop();
                                //会话初始化
                                this.LyncConversationInit();
                            }
                        }), 1000, out this.CheckConversationInitTimer);

                        #endregion
                    }
                }), 800, out this.CheckDisposeAllSocketTimer);
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
