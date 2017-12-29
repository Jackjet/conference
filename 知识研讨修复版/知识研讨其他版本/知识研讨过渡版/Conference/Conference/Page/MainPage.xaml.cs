using Conference.Common;
using ConferenceCommon.TimerHelper;
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
using Conference.View.U_Disk;
using Conference.View.WebBrowser;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceModel;
using ConferenceModel.ConferenceAudioWebservice;
using ConferenceModel.ConferenceInfoWebService;
using ConferenceModel.ConferenceTreeWebService;
using ConferenceModel.FileSyncAppPoolWebService;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.IcoFlash;
using ConferenceCommon.WPFHelper;
using Conference_Space.Common;

namespace Conference.Page
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : MainPageBase
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainPage()
        {
            try
            {
                //UI加载
                InitializeComponent();

                #region 事件注册

                //会议信息
                this.btnMeet.Click += Navacite;
                //信息交流
                this.btnIMM.Click += Navacite;
                //共享资源
                this.btnResource.Click += Navacite;
                //智存空间
                this.btnSpace.Click += Navacite;

                //知识树
                this.btnTree.Click += Navacite;
                //会议投票
                this.btnVote.Click += Navacite;
                //个人笔记
                this.btnNote.Click += Navacite;
                //工具应用
                this.btnTool.Click += Navacite;



                //主页面点击事件
                this.PreviewMouseLeftButtonDown += MainPage_PreviewMouseLeftButtonDown;

                //会议室图例点击事件
                this.ConferenceRoom_View.ItemClickCallBackToMainPage += conferenceRoom_View_ItemClick;

                //lync会话事件注册
                LyncHelper.LyncConversationEventRegedit(HasConferenceEvent, MainConversationAbout, ConversationAddCompleateEvent,
                    MainConversationInEvent, ContentAddCompleateEvent, //更改状态
                               ChangSharePanelState, GetPresenterCallBack);

                //工具箱子项更改事件
                this.ToolCmWindow.SelectItemChanged += ToolCmWindow_SelectItemChanged;

                #endregion

                #region 事件注册（精简模式）

                //会议信息（精简模式）
                this.btnMeet2.Click += Navacite;
                //信息交流
                this.btnIMM2.Click += Navacite;
                //共享资源
                this.btnResource2.Click += Navacite;
                //智存空间
                this.btnSpace2.Click += Navacite;
                //系统设置（精简模式有）
                this.btnSetting.Click += Navacite;

                #endregion

                #region 事件注册(教学模式)

                //我的课堂
                this.btnMeetEducation.Click += Navacite;
                //互动课堂
                this.btnResourceEducation.Click += Navacite;
                //教学资料
                this.btnSpaceEducation.Click += Navacite;
                //课程大纲
                this.btnTreeEducation.Click += Navacite;
                //系统设置（教学模式）
                this.btnSettingEducation.Click += Navacite;

                #endregion

                #region 加入会议并处理资源共享

                //加入当前参与的一个会议
                this.JoinConfereince(new Action(() =>
                    {
                        //加载共享面板
                        //this.ConversationM.IsStart = true;
                        //会话初始化
                        //this.LyncConversationInit();
                    }));

                #endregion

                #region 检测通讯是否正常（知识树）

                //检测通讯（知识树）
                base.CheckAndRepairClientSocekt(this.CheckNetWorkCallBack);

                #endregion

                #region 注册父类事件

                //教育模式
                base.ViewModelChangedEducationEvent += MainPage_ViewModelChangedEducationEvent;
                //精简模式
                base.ViewModelChangedSimpleEvent += MainPage_ViewModelChangedSimpleEvent;
                //页面切换
                base.ForceToNavicateEvent += MainPage_ForceToNavicateEvent;

                #endregion

                #region （lync应用校验）检测大屏投影是否同步

                //this.lyncBigScreenConversationTimer = new DispatcherTimer();
                //this.lyncBigScreenConversationTimer.Tick += lyncBigScreenConversationTimer_Tick;
                //this.lyncBigScreenConversationTimer.Interval = TimeSpan.FromSeconds(2);
                //this.lyncBigScreenConversationTimer.Start();

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

        #region 父类事件

        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="selectedItem"></param>
        void MainPage_ForceToNavicateEvent(ViewSelectedItemEnum selectedItem)
        {
            try
            {
                this.ForceToNavicate(selectedItem);
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
        /// 精简模式
        /// </summary>
        /// <param name="result"></param>
        void MainPage_ViewModelChangedSimpleEvent(bool result)
        {
            try
            {
                this.ViewModelChangedSimple(result);
                MainWindow.IndexInstance.ViewModelChangedSimple(result);
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
        /// 教育模式
        /// </summary>
        /// <param name="result"></param>
        void MainPage_ViewModelChangedEducationEvent(bool result)
        {
            try
            {
                this.ViewModelChangedEducation(result);
                MainWindow.IndexInstance.ViewModelChangedEducation(result);
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

        #region lync会话事件辅助

        /// <summary>
        /// 判断是否有主会话
        /// </summary>
        /// <param name="conversationWindow"></param>
        void MainConversationInEvent(Action<ConversationWindow> conversationWindowCallBack)
        {
            try
            {
                conversationWindowCallBack(LyncHelper.MainConversation);
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
        /// 会话加载完成事件
        /// </summary>
        void ConversationAddCompleateEvent()
        {
            try
            {

                //取消置顶
                TimerJob.StartRun(new Action(() =>
                {
                    MainWindow.mainWindow.Topmost = false;
                }), 1000);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 加载内容完成事件
        /// </summary>
        void ContentAddCompleateEvent(string title)
        {
            try
            {
                //已经处于资源共享页面则不进行闪烁
                if (Conference.MainWindow.MainPageInstance.ViewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource)
                {
                    //资源共享导航按钮闪烁
                    Conference.MainWindow.MainPageInstance.ResourceReceivMessageFlash();
                }

                //this.ConversationM.tabItem1.Header = title;

                //投影到大屏幕
                LyncHelper.InviteSomeOneJoinMainConference(Constant.lyncClient, LyncHelper.MainConversation, Constant.BigScreenName);
                //强制导航到资源共享
                this.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                this.ConversationM.PageIndex = ResourceType.Share;
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
        /// 主会话关联
        /// </summary>
        /// <param name="mainConversationWindow"></param>
        void MainConversationAbout(ConversationWindow mainConversationWindow)
        {
            try
            {
                if (mainConversationWindow != null)
                {
                    //设置会话窗体的坐标位置和尺寸
                    this.ConversationM.DockConversationWindow(Constant.lyncClient, mainConversationWindow);
                }
                LyncHelper.MainConversation = mainConversationWindow;

                SpaceCodeEnterEntity.MainConversation = LyncHelper.MainConversation;
                this.SpaceView.PersonalSpace.ShareInConversationSelfNavicateCallBack = SpaceSelfNavicateCallBack;
                this.SpaceView.MeetingSpace.ShareInConversationSelfNavicateCallBack = SpaceSelfNavicateCallBack;

                this.SpaceView.PersonalSpace.ShareInConversationOtherNavicateCallBack = SpaceOtherNavicateCallBack;
                this.SpaceView.MeetingSpace.ShareInConversationOtherNavicateCallBack = SpaceOtherNavicateCallBack;
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
        /// 是否包含会议
        /// </summary>
        /// <param name="hasConference"></param>
        void HasConferenceEvent(Action<bool> hasConference)
        {
            try
            {
                if (!string.IsNullOrEmpty(Constant.ConferenceName))
                {
                    hasConference(true);
                }
                else
                {
                    hasConference(false);
                }
                //设置DNS
                NetWorkAdapter.SetNetworkAdapter(Constant.RouteIp);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        public void ChangSharePanelState()
        {
            try
            {
                //更改状态
                MainWindow.mainWindow.mainPage.SharingPanel.UpdateState(null);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        public void GetPresenterCallBack(string presenter)
        {
            try
            {
                //显示演示者
                this.ConversationM.ResourcePresenter = presenter;
                this.ConversationM.ResourcePresenterVisibility = System.Windows.Visibility.Visible;
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
        /// 智存空间实施共享回调事件
        /// </summary>
        public void SpaceSelfNavicateCallBack()
        {
              try
            {
                //强制跳转到会话管理页面
                //强制导航到资源共享
                MainWindow.MainPageInstance.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                MainWindow.MainPageInstance.ConversationM.PageIndex = ResourceType.Share;
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
        /// 智存空间实施共享回调事件
        /// </summary>
        public void SpaceOtherNavicateCallBack()
        {
            try
            {
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



        #endregion

        #region 导航到指定视图

        public void Navacite(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.NavicateView(navicateButton.ViewSelectedItemEnum);
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
        /// 导航到指定视图
        /// </summary>
        public void NavicateView(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //前期处理
                this.DealWithBefore(viewSelectedItemEnum);

                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.Meet:
                        this.DealWithMeet();
                        break;
                    case ViewSelectedItemEnum.Tree:
                        this.DealWithTree();
                        break;

                    case ViewSelectedItemEnum.Space:
                        this.DealWidthSpace();
                        break;

                    case ViewSelectedItemEnum.Resource:
                        this.DealWidthResource();
                        break;

                    case ViewSelectedItemEnum.IMM:
                        this.DealWidthIMM();
                        break;
                    case ViewSelectedItemEnum.PersonNote:
                        this.DealWidthPersonNote();
                        break;
                    case ViewSelectedItemEnum.WebBrowserView:
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnVote);
                        //加载会议投票
                        this.borMain.Child = this.WebBrowserView;

                        break;
                    case ViewSelectedItemEnum.U_Disk:
                        //U盘传输
                        this.borMain.Child = this.U_DiskView;
                        break;

                    case ViewSelectedItemEnum.Meet_Change:
                        break;

                    case ViewSelectedItemEnum.Chair:
                        //加载主持人功能
                        this.borMain.Child = this.ChairView;

                        break;
                    case ViewSelectedItemEnum.Studiom:
                        //中控设置
                        this.borMain.Child = this.StudiomView;
                        break;
                    case ViewSelectedItemEnum.SystemSetting:
                        this.DealWidthSystemSetting();
                        break;

                    case ViewSelectedItemEnum.Tool:
                        //工具箱模块处理
                        this.DealWithTool();
                        break;

                    case ViewSelectedItemEnum.ToolUsing:
                        //工具应用
                        this.borMain.Child = this.ApplicationToolView;
                        break;
                    default:
                        break;
                }
                //绑定选择项
                this.ViewSelectedItemEnum = viewSelectedItemEnum;
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
        /// 前期处理
        /// </summary>
        /// <param name="viewSelectedItemEnum"></param>
        public void DealWithBefore(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                if (viewSelectedItemEnum == ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Tool && this.ToolCmWindow.FirstSelected)
                {
                    return;
                }
                else
                {
                    if (LyncHelper.MainConversation != null)
                    {
                        //避免批注之类的影响视图，进行显示的切换
                        if (viewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource)
                        {
                            //设置会话区域显示内容
                            this.ConversationM.SetConversationAreaShow(ShowType.HidenView,false);
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

        /// <summary>
        /// 我的会议处理
        /// </summary>
        public void DealWithMeet()
        {
            try
            {
                //教学模式
                if (TempConferenceInformationEntity != null && !TempConferenceInformationEntity.EducationMode)
                {
                    if (!TempConferenceInformationEntity.SimpleMode)
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnMeet);
                    }
                    else
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnMeet2);
                    }
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnMeetEducation);
                }
                //加载我的会议
                this.borMain.Child = this.MyConferenceView;
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
        /// 知识树处理
        /// </summary>
        public void DealWithTree()
        {
            try
            {
                //教学模式
                if (TempConferenceInformationEntity != null && !TempConferenceInformationEntity.EducationMode)
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnTree);
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnTreeEducation);
                }
                //跳转到知识树
                this.borMain.Child = this.ConferenceTreeView;
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
        /// 智存空间处理
        /// </summary>
        public void DealWidthSpace()
        {
            try
            {
                //教学模式
                if (TempConferenceInformationEntity != null && !TempConferenceInformationEntity.EducationMode)
                {
                    if (!TempConferenceInformationEntity.SimpleMode)
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnSpace);
                    }
                    else
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnSpace2);
                    }
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnSpaceEducation);
                }
                //跳转到会议空间
                this.borMain.Child = this.SpaceView;
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
        /// 共享协作处理
        /// </summary>
        public void DealWidthResource()
        {
            try
            {
                //教学模式
                if (TempConferenceInformationEntity != null && !TempConferenceInformationEntity.EducationMode)
                {
                    if (!TempConferenceInformationEntity.SimpleMode)
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnResource);
                        //隐藏IMM提示
                        this.borResourceTip.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnResource2);
                        //隐藏IMM提示
                        this.borResourceTip2.Visibility = System.Windows.Visibility.Hidden;
                    }
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnResourceEducation);
                    //隐藏IMM提示
                    this.borResourceTipEducation.Visibility = System.Windows.Visibility.Hidden;
                }

               

                //跳转到资源共享
                this.borMain.Child = this.ConversationM;

                //设置会话区域显示内容
                this.ConversationM.SetConversationAreaShow(ShowType.ConversationView, false);

                //停止计时器工作
                if (base.ResourceFlashTimer != null)
                {
                    base.ResourceFlashTimer.Stop();
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
        /// 信息交流处理
        /// </summary>
        public void DealWidthIMM()
        {
            try
            {
                if (base.TempConferenceInformationEntity != null && !base.TempConferenceInformationEntity.SimpleMode)
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnIMM);
                    //隐藏IMM提示
                    this.borImmTip.Visibility = System.Windows.Visibility.Hidden;
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnIMM2);
                    //隐藏IMM提示
                    this.borImmTip2.Visibility = System.Windows.Visibility.Hidden;
                }
                //停止计时器工作
                if (base.IMMFlashTimer != null)
                {
                    base.IMMFlashTimer.Stop();
                }

                //跳转到信息交流
                this.borMain.Child = this.ConferenceAudio_View;
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
        /// 系统设置处理
        /// </summary>
        public void DealWidthSystemSetting()
        {
            try
            {
                //教学模式
                if (TempConferenceInformationEntity != null && !TempConferenceInformationEntity.EducationMode)
                {
                    if (TempConferenceInformationEntity.SimpleMode)
                    {
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnSetting);
                    }
                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnSettingEducation);
                }

                //跳转到系统设置
                this.borMain.Child = this.Setting_View;
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
        /// 个人笔记处理
        /// </summary>
        public void DealWidthPersonNote()
        {
            try
            {
                //导航样式更改
                this.ButtonStyleChanged(this.btnNote);

                //加载个人笔记
                this.borMain.Child = this.PersonalNote;
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

        #region 检测网络异常回调

        public void CheckNetWorkCallBack(NetWorkErrTipType netWorkErrTipType)
        {
            try
            {
                //信息提示
                string message = string.Empty;
                if (this.currentNetWorkState != netWorkErrTipType)
                {
                    switch (netWorkErrTipType)
                    {
                        case NetWorkErrTipType.Normal:

                            MainWindow.mainWindow.NetWork_ViewVisibility = System.Windows.Visibility.Collapsed;
                            //if (LyncHelper.MainConversation != null)
                            //{
                            MainWindow.MainPageInstance.SharingPanel.NetWork_ViewVisibility = System.Windows.Visibility.Collapsed;
                            //}

                            break;

                        case NetWorkErrTipType.ConnectedRouteFailed:
                            //共享面板
                            SharingPanel panel = MainWindow.MainPageInstance.SharingPanel;
                            //导航到首页
                            MainWindow.mainWindow.IndexPageChangedToIndexPage();

                            message = "无法连接到当前网络";
                            MainWindow.mainWindow.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            MainWindow.mainWindow.SettingNetWorkConnectErrorTip(message);
                            panel.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            panel.SettingNetWorkConnectErrorTip(message);

                            break;
                        case NetWorkErrTipType.ConnectedServiceFailed:
                            //共享面板
                            SharingPanel panel2 = MainWindow.MainPageInstance.SharingPanel;
                            //导航到首页
                            MainWindow.mainWindow.IndexPageChangedToIndexPage();

                            message = "无法连接到服务器";
                            MainWindow.mainWindow.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            MainWindow.mainWindow.SettingNetWorkConnectErrorTip(message);
                            panel2.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            panel2.SettingNetWorkConnectErrorTip(message);

                            break;
                        case NetWorkErrTipType.ConnectedWebServiceFailed:

                            //共享面板
                            SharingPanel panel3 = MainWindow.MainPageInstance.SharingPanel;
                            //导航到首页
                            MainWindow.mainWindow.IndexPageChangedToIndexPage();

                            message = "无法访问web服务";
                            MainWindow.mainWindow.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            MainWindow.mainWindow.SettingNetWorkConnectErrorTip(message);
                            panel3.NetWork_ViewVisibility = System.Windows.Visibility.Visible;
                            panel3.SettingNetWorkConnectErrorTip(message);

                            break;
                        default:
                            break;
                    }
                }
                //当前网络状态
                this.currentNetWorkState = netWorkErrTipType;
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
