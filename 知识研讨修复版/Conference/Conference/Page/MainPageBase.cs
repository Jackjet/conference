using Conference.Common;
using Conference.View.AppcationTool;
using Conference.View.Chair;
using Conference.View.Setting;
using Conference.View.Space;
//using Conference.View.Studiom;
using Conference.View.Tool;
//using Conference.View.U_Disk;
using Conference_Conversation;
using Conference_Conversation.Common;
using Conference_IMM;
using Conference_IMM.Common;
using Conference_MyConference;
using Conference_MyConference.Common;
using Conference_Note;
using Conference_Note.Common;
using Conference_Space;
using Conference_Space.Common;
using Conference_Tree;
using Conference_WebBrowser;
using Conference_WebBrowser.Common;
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
using ConferenceModel.Common;
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
using vy = System.Windows.Visibility;

namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 自定义委托事件(回调)

        /// <summary>
        /// 教学模式更改回调
        /// </summary>
        public Action<bool> ViewModelChangedEducationCallBack = null;

        /// <summary>
        /// 精简模式更改事件
        /// </summary>
        public Action<bool> ViewModelChangedSimpleCallBack = null;

        /// <summary>
        /// 页面导航事件
        /// </summary>
        public Action<ConferenceCommon.EnumHelper.ViewSelectedItemEnum> ForceToNavicateCallBack = null;

        #endregion

        #region 内部字段

        /// <summary>
        /// 甩屏接收窗体
        /// </summary>
        AppSyncDataAcceptWindow AppSyncDataAcceptWindow;

        /// <summary>
        /// 之前选中的导航按钮
        /// </summary>
        protected NavicateButton beforeSelectedButton = null;

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object Communication_Object = new object();

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

        /// <summary>
        /// 会议地址
        /// </summary>
        string meetAddress = null;

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
                    this.SPaceDataInitPrepare();
                    this.spaceView = new SpaceView();
                    this.CallBackEvent_Space();
                }
                return spaceView;
            }
        }

        private CommunicationManage communicationManage;
        /// <summary>
        /// 通讯管理机制
        /// </summary>
        public CommunicationManage CommunicationManage
        {
            get
            {
                //创建单例
                if (this.communicationManage == null)
                {
                    //通讯管理
                    this.communicationManage = new CommunicationManage();
                    //获取服务器所传送的数据
                    this.communicationManage.clientSocket_TCPDataArrivalCallBack = this.clientSocket_TCPDataArrival;
                    //页面刷新回调事件
                    this.communicationManage.PageRefleshCallBack = this.PageRefleshCallBack;
                    //修复主窗体并导航到首页回调事件
                    this.communicationManage.RepareMainWindowAndNavicateToIndeCallBack = this.RepareMainWindowAndNavicateToIndeCallBack;
                }
                return communicationManage;
            }
        }

        ConferenceRoom_View conferenceRoom_View = null;
        /// <summary>
        /// 会议室
        /// </summary>
        public ConferenceRoom_View ConferenceRoom_View
        {
            get
            {
                //创建单例
                if (this.conferenceRoom_View == null)
                {
                    //我的会议数据初始化
                    this.MyConferenceDataInitPrepare();
                    //创建会议室
                    this.conferenceRoom_View = new ConferenceRoom_View();
                    //加载卡片事件回调
                    this.conferenceRoom_View.AddCardEventCallBack = AddCardEventCallBack;
                }
                return conferenceRoom_View;
            }
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
                    //应用工具
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
                    //系统设置
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
                    //资源共享
                    this.conversationM = new ConversationM();
                    //共享协作回调中心
                    this.CallBackEvent_Conversation();
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
                    //信息交流数据初始化
                    this.IMMDataInitPrepare();
                    this.conferenceAudio_View = new ConferenceAudio_View();
                    //得到消息提醒（闪烁）
                    this.conferenceAudio_View.IMMFalshCallBack = IMMFalshCallBack;
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
                    //知识树数据初始化
                    this.TreeDataInitPrepare();
                    this.conferenceTreeView = new ConferenceTreeView();
                    this.CallBackEvent_Tree();
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
                    //个人笔记数据初始化
                    this.NoteDataInitPrepare();
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
                    //我的会议数据初始化
                    this.MyConferenceDataInitPrepare();
                    this.myConferenceView = new MyConferenceView();
                    //填充数据回调
                    this.myConferenceView.FillDataGridCallBack = FillDataGridCallBack;
                    //datagrid子项更改事件回调
                    LyncHelper.BeginRefleshDataGridCallBack = this.myConferenceView.DataGridItemChangeCallBack;
                    //没有参会人回调
                    LyncHelper.NoParticalCallBack = NoParticalCallBack;
                }
                return myConferenceView;
            }
        }

        protected WebBrowserView webBrowserView;
        /// <summary>
        /// 网络浏览
        /// </summary>
        public WebBrowserView WebBrowserView
        {
            get
            {   //创建单例
                if (this.webBrowserView == null)
                {
                    //网络浏览数据初始化
                    this.WebBrowserViewDataInitPrepare();
                    this.webBrowserView = new WebBrowserView();
                    //浏览器共享回调
                    this.webBrowserView.ShareWebBrowserCallBack = ShareWebBrowserCallBack;
                }
                return webBrowserView;
            }
        }

        //protected U_DiskView u_DiskView;
        ///// <summary>
        ///// U盘传输
        ///// </summary>
        //public U_DiskView U_DiskView
        //{
        //    get
        //    {   //创建单例
        //        if (this.u_DiskView == null)
        //        {
        //            this.u_DiskView = new U_DiskView();
        //        }
        //        return u_DiskView;
        //    }
        //}

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

        //StudiomView studiomView;
        ///// <summary>
        ///// 主持人功能
        ///// </summary>
        //public StudiomView StudiomView
        //{
        //    get
        //    {   //创建单例
        //        if (this.studiomView == null)
        //        {
        //            this.studiomView = new StudiomView();
        //        }
        //        return studiomView;
        //    }
        //}

        //List<ShareableContent> shareableContentList = new List<ShareableContent>();
        ///// <summary>
        ///// 共享的资源集合
        ///// </summary>
        //public List<ShareableContent> ShareableContentList
        //{
        //    get { return shareableContentList; }
        //    set { shareableContentList = value; }
        //}


        #endregion

        #region 静态字段

        ///// <summary>
        ///// 主会话联系人列表（实际）
        ///// </summary>
        //public static ObservableCollection<string> participanetList = new ObservableCollection<string>();


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

        #region 工具箱高级面板显示设置

        /// <summary>
        /// 工具箱高级面板显示设置
        /// </summary>
        protected void ToolPopupDisplay()
        {
            try
            {
                //工具箱弹出框显示设置
                if (this.ToolCmWindow.Visibility == vy.Collapsed)
                {
                    //鼠标单击显示
                    this.ToolCmWindow.Visibility = vy.Visible;
                    //显示
                    this.ToolCmWindow.Show();
                }
                else
                {
                    //鼠标双击隐藏
                    this.ToolCmWindow.Visibility = vy.Collapsed;
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
        /// <param name="conferenceInformationEntity">会议信息数据实体</param>
        protected void ItemClick_DisposeSourceAndInitAgain(ConferenceInformationEntityPC conferenceInformationEntity)
        {
            try
            {
                if (conferenceInformationEntity != null)
                {
                    //临时存储的会议信息
                    Constant.TempConferenceInformationEntity = conferenceInformationEntity;
                    //模式确定
                    MainPage.mainPage.IsSimpleModel();
                    //模式确定
                    MainPage.mainPage.IsEdacationModel();
                    //模式确定
                    MainPage.mainPage.IsNormalModel();

                    //释放相关资源
                    this.DisposeAllResource();
                    //释放之前使用的通讯节点
                    this.DisPoseCommunicate(conferenceInformationEntity);
                    //全局信息填充及初始化
                    this.GlobleInfoLoadAndInit(conferenceInformationEntity);
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
        /// <param name="conferenceInformationEntity">会议信息数据实体</param>
        /// <param name="callBack"></param>
        private void GlobleInfoLoadAndInit(ConferenceInformationEntityPC conferenceInformationEntity)
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
                        //模式管理中心
                        this.Model_ManageCenter(conferenceInformationEntity.EducationMode, conferenceInformationEntity.SimpleMode);
                        //填充全局字段（参会人列表、会议名称、会议ID）
                        this.GlobalDataInit(conferenceInformationEntity);

                        //会话标示服务器判断移除
                        ModelManage.ConferenceLyncConversation.RemoveConversation(Constant.ConferenceName, new Action<bool>((successed) =>
                        {
                            if (successed)
                            {
                                this.canBeginLyncConversationInit = true;
                            }
                        }));
                        //判断是否为会议主持人(是的话设置全局标示)
                        MainPageBase.PresentSetting(conferenceInformationEntity);

                        //共享协作数据准备
                        this.ConversationDataInitPrepare();
                        //填充参会人信息
                        LyncHelper.AddContacts();

                        //重新进行会话配置
                        this.Conversation_Configure();
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

        #region 释放所有资源

        /// <summary>
        /// 释放所有资源
        /// </summary>
        private void DisposeAllResource()
        {
            try
            {
                //设置为不可进行会话初始化
                this.canBeginLyncConversationInit = false;

                //参会人信息清空
                ConversationCodeEnterEntity.DicParticipant.Clear();

                //离开坐席
                MyConferenceView.LeaveSeat();

                //释放资源
                this.UIAndSourceDispose();

                //检测是否可以会话初始化计时数量置零
                this.CheckConversationInitCount = 0;

                //还原默认设置
                this.ConversationM.CurrentShowType = ShowType.None;
                //可以转到会话视图
                this.ConversationM.IsCanNavicateConversationView = false;
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

        #region 释放之前使用的通讯节点

        /// <summary>
        /// 释放之前使用的通讯节点
        /// </summary>
        /// <param name="conferenceInformationEntity">会议信息实体</param>
        private void DisPoseCommunicate(ConferenceInformationEntityPC conferenceInformationEntity)
        {
            try
            {
                //检测是否释放所有通讯节点
                this.IsDisposeAllSocekt = false;
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //释放之前使用的通讯节点
                    this.DisPoseServerSocketArray(conferenceInformationEntity.MeetingName, new Action<bool>((isCompleate) =>
                    {
                        if (isCompleate)
                        {
                            //检测是否释放所有通讯节点
                            this.IsDisposeAllSocekt = true;
                        }
                    }));
                });
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

        #region 模式管理中心

        /// <summary>
        /// 模式管理中心
        /// </summary>
        /// <param name="isEducationMode">会议信息实体</param>
        public void Model_ManageCenter(bool isEducationMode, bool isSimpleMode)
        {
            try
            {
                #region 是否为教学模式

                if (isEducationMode)
                {
                    if (this.ViewModelChangedEducationCallBack != null)
                    {
                        //更改视图为教学模式
                        this.ViewModelChangedEducationCallBack(true);
                    }
                }
                else
                {
                    if (this.ViewModelChangedEducationCallBack != null)
                    {
                        //更改视图为非教学模式
                        this.ViewModelChangedEducationCallBack(false);
                    }
                }

                #endregion

                #region 是否为精简模式

                if (isSimpleMode)
                {
                    if (this.ViewModelChangedSimpleCallBack != null)
                    {
                        //更改视图为精简模式
                        this.ViewModelChangedSimpleCallBack(true);
                    }
                }
                else
                {
                    if (this.ViewModelChangedSimpleCallBack != null)
                    {
                        //更改视图为非精简模式
                        this.ViewModelChangedSimpleCallBack(false);
                    }
                }

                #endregion
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

        #region 填充全局字段（参会人列表、会议名称、会议ID）

        /// <summary>
        /// 填充全局字段（参会人列表、会议名称、会议ID）
        /// </summary>
        /// <param name="conferenceInformationEntity">会议信息实体</param>
        private void GlobalDataInit(ConferenceInformationEntityPC conferenceInformationEntity)
        {
            try
            {
                //参会人列表
                Constant.ParticipantList = conferenceInformationEntity.JoinPeople.ToList<string>();
                //参会名称
                Constant.ConferenceName = conferenceInformationEntity.MeetingName;
                //会议室名称
                Constant.ConferenceRoomName = conferenceInformationEntity.RoomName;
                //会议主持人
                Constant.ConferenceHost = conferenceInformationEntity.ApplyPeople;
                //教育模式（确认,主页面顶部显示信息）
                MainPage.mainPage.ViewModelChangedEducation(conferenceInformationEntity.EducationMode);
                //参会ID
                Constant.MeetingID = conferenceInformationEntity.MeetingID;
                //网络浏览地址
                Constant.WebUri = conferenceInformationEntity.WebUri;
                //显示参会名称
                this.MeetingName = Constant.ConferenceName;
                //投影大屏幕
                Constant.BigScreenName = conferenceInformationEntity.BigScreenName;
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

        #region 判断当前用户是否为主持人（该标示在程序全局上使用）

        /// <summary>
        /// 判断当前用户是否为主持人（该标示在程序全局上使用）
        /// </summary>
        /// <param name="conferenceInformationEntity">会议信息实体</param>
        private static void PresentSetting(ConferenceInformationEntityPC conferenceInformationEntity)
        {
            try
            {
                //主持人邮箱
                string applyPeople = conferenceInformationEntity.ApplyPeople;

                //判断当前用户是否为主持人（该标示在程序全局上使用）
                if (Constant.SelfUri.Equals(applyPeople) || Constant.SelfName.Equals(applyPeople))
                {
                    Constant.IsMeetingPresenter = true;
                }
                else
                {
                    Constant.IsMeetingPresenter = false;
                }

                ////会议主持人
                conferenceInformationEntity.ApplyPeople = LyncHelper.GetUserName(applyPeople);
                string state = LyncHelper.GetInformationAcording(applyPeople, ContactInformationType.Activity);
                string info = conferenceInformationEntity.ApplyPeople + "(" + state + ")";
                MyConferenceView.myConferenceView.PresenterInfoSetting(info);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(MainPageBase), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 重新进行会话配置

        /// <summary>
        /// 重新进行会话配置
        /// </summary>
        private void Conversation_Configure()
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                {
                    if (this.canBeginLyncConversationInit)
                    {
                        this.CheckConversationInitTimer.Stop();

                        #region 获取服务端口

                        //通讯模块数据准备
                        this.CommunicationDataInitPrepare();

                        //获取服务端口
                        this.CommunicationManage.GetServicePort(new Action(() =>
                        {
                            this.MyConferenceDataInitPrepare();
                            //位置信息获取并设置
                            this.MyConferenceView.IntoOneSeat_AboutMyConference();
                        }));

                        #endregion

                        //检测通讯是否正常（知识树）
                        this.CommunicationManage.CheckAndRepairClientSocekt(MainPage.mainPage.CheckNetWorkCallBack);

                        //会话初始化
                        this.LyncConversationInit();
                    }
                }), 1000, out this.CheckConversationInitTimer);
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
