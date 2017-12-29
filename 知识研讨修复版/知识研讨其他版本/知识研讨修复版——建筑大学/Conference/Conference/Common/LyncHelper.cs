using Conference.Page;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.ImageHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.VersionHelper;
using ConferenceModel;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Conference.Common
{
    public partial class LyncHelper
    {
        #region 字段

        /// <summary>
        /// 共享白板的自定义名称
        /// </summary>
        static string whiteBoardShareName = "所共享的白板";

        /// <summary>
        /// 共享白板数量(默认为0,进行计数)
        /// </summary>
        static int whiteBoardShareCount = 0;

        /// <summary>
        /// 启动会话邀请词
        /// </summary>
        static string MainConversationAccrodingStr = "___邀请会话";

        /// <summary>
        /// 通知接受计时器
        /// </summary>
        public static DispatcherTimer timerAcept = null;

        /// <summary>
        /// 参会人列表
        /// </summary>
        static ObservableCollection<ParticipantsEntity> currentParticipantsEntityList = null;

        /// <summary>
        /// datagrid子项更改事件
        /// </summary>
        public static Action<ObservableCollection<ParticipantsEntity>> BeginRefleshDataGridCallBack = null;

        #endregion

        #region 方法回调

        /// <summary>
        /// e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedIn
        /// </summary>
        public static Action State1CallBack = null;

        /// <summary>
        /// e.OldState == ClientState.SignedIn && e.NewState == ClientState.SigningOut
        /// </summary>
        public static Action State2CallBack = null;

        /// <summary>
        /// e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedOut
        /// </summary>
        public static Action State3CallBack = null;

        /// <summary>
        /// 查看是否包含会议
        /// </summary>
        public static Action<Action<bool>> HasConferenceCallBack = null;

        /// <summary>
        /// 输出atuomation
        /// </summary>
        public static Action<ConversationWindow> MainConversationOutCallBack = null;

        /// <summary>
        /// 会话窗体加载内容完成事件
        /// </summary>
        public static Action ConversationAddCompleateCallBack = null;

        /// <summary>
        /// 输入automation
        /// </summary>
        public static Action<Action<ConversationWindow>> MainConversationInCallBack = null;

        /// <summary>
        /// 会话加载完成事件
        /// </summary>
        public static Action<string> ContentAddCompleateCallBack = null;

        /// <summary>
        /// 共享内容回调事件
        /// </summary>
        public static Action Content_DeskRemoveCompleateCallBack = null;

        /// <summary>
        /// lync人员辅助作用
        /// </summary>
        static LyncContactManage lyncContactManage = null;

        /// <summary>
        /// 主会话
        /// </summary>
        public static ConversationWindow MainConversation = null;


        /// <summary>
        /// 返回演示人
        /// </summary>
        public static Action<string> PresentCallBack = null;

        #endregion

        #region 属性

        static List<ShareableContent> shareableContentList = new List<ShareableContent>();
        /// <summary>
        /// 共享的资源集合
        /// </summary>
        public static List<ShareableContent> ShareableContentList
        {
            get { return shareableContentList; }
            set { shareableContentList = value; }
        }

        //static bool canShowContent = false;
        ///// <summary>
        ///// 是否可以进行 
        ///// </summary>
        //public static bool CanShowContent
        //{
        //    get { return canShowContent; }
        //    set { canShowContent = value; }
        //}

        #endregion

        #region 检测automation是否弹出的情况

        /// <summary>
        /// 检测automation是否弹出的情况
        /// </summary>
        public static void CheckAutomationIsOpenAndActive(IntPtr handle)
        {
            try
            {
                //TimerJob.StartRunNoStop(new Action(() =>
                //{
                //    LyncHelper.DockToNewParentWindow(handle);

                //}), 2000);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 启动全屏

        /// <summary>
        /// 会话窗体状态管理模型
        /// </summary>
        static ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 设置为全屏
        /// </summary>
        public static bool FullScreen()
        {
            bool result = false;
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //Microsoft.Office.Uc.ConversationWindowClass window = LyncHelper.MainConversation.InnerObject as Microsoft.Office.Uc.ConversationWindowClass;

                    //获取当前会话窗体的句柄
                    IntPtr Handle = LyncHelper.MainConversation.Handle;

                    //获取会话窗体的状态
                    Win32API.GetWindowPlacement(Handle, ref Placement);
                    //最小话窗体
                    Placement.showCmd = 1;
                    Win32API.SetWindowPlacement(Handle, ref Placement);


                    if (LyncHelper.MainConversation.CanInvoke(ConversationWindowAction.FullScreen))
                    {
                        if (!LyncHelper.MainConversation.IsFullScreen && LyncHelper.MainConversation.IsDocked)
                        {
                            LyncHelper.MainConversation.ShowFullScreen(0);
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
            return result;
        }

        #endregion

        #region 退出全屏


        /// <summary>
        /// 退出全屏
        /// </summary>
        public static void ExitFullScreen()
        {
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.CanInvoke(ConversationWindowAction.FullScreen))
                {
                    if (LyncHelper.MainConversation.IsFullScreen)
                    {
                        LyncHelper.MainConversation.ExitFullScreen();

                        #region old solution

                        //TimerJob.StartRun(new Action(() =>
                        //{
                        //    //获取当前会话窗体的句柄
                        //    IntPtr Handle = LyncHelper.MainConversation.Handle;

                        //    //获取会话窗体的状态
                        //    Win32API.GetWindowPlacement(Handle, ref Placement);
                        //    //最小话窗体
                        //    Placement.showCmd = 1;
                        //    Win32API.SetWindowPlacement(Handle, ref Placement);
                        //}));
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 附加到新窗体（Dock）

        /// <summary>
        /// 附加到新窗体
        /// </summary>
        /// <param name="handle"></param>
        public static void DockToNewParentWindow(IntPtr handle, Action<bool> callBack)
        {
            try
            {
                if (Constant.lyncClient != null && Constant.lyncClient.State == ClientState.SignedIn && LyncHelper.MainConversation != null && !LyncHelper.MainConversation.IsDocked)
                {
                    if (LyncHelper.MainConversation != null)
                    {
                        if (LyncHelper.MainConversation.IsFullScreen)
                        {
                            //先退出全屏
                            LyncHelper.ExitFullScreen();
                        }

                        TimerJob.StartRun(new Action(() =>
                        {
                            if (!LyncHelper.MainConversation.IsDocked && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                            {
                                LyncHelper.MainConversation.Dock(handle);
                                LyncHelper.FullScreen();
                                callBack(true);
                            }
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 填充参会人列表

        /// <summary>
        /// 填充参会人列表
        /// </summary>
        public static void FillLyncOnlineInfo(DataGrid datagrid, ObservableCollection<ParticipantsEntity> ParticipantsEntityList)
        {
            try
            {
                if (Constant.lyncClient != null)
                {
                    currentParticipantsEntityList = ParticipantsEntityList;

                    //数据清除
                    currentParticipantsEntityList.Clear();

                    //递增序号
                    int number = 0;

                    //遍历添加参会人
                    foreach (var item in Constant.ParticipantList)
                    {
                        //创建参会人子项实体
                        ParticipantsEntity participantsEntity = new ParticipantsEntity();


                        //将参会人的名称取出
                        if (Constant.DicParticipant.ContainsKey(item))
                        {
                            participantsEntity.ParticipantsName = Constant.DicParticipant[item];
                            number++;
                            //序号
                            participantsEntity.Number = number;

                            if (Constant.lyncClient != null)
                            {
                                Contact contact = Constant.lyncClient.ContactManager.GetContactByUri(item);
                                contact.ContactInformationChanged += contact_ContactInformationChanged;
                                //获取当前人的公司名称
                                participantsEntity.Company = Convert.ToString(contact.GetContactInformation(ContactInformationType.Company));
                                //获取当前人的职位
                                participantsEntity.Position = Convert.ToString(contact.GetContactInformation(ContactInformationType.Title));

                                //int state = Convert.ToInt32(contact.GetContactInformation(ContactInformationType.Availability));

                                participantsEntity.LoginUri = item;
                                participantsEntity.LoginName = item.Replace("sip:", string.Empty).Replace(Constant.UserDomain, string.Empty).Replace("@", string.Empty);
                                string uriImg = Constant.TreeServiceAddressFront + Constant.FtpServercePersonImgName + participantsEntity.LoginName + ".png";
                                BitmapImage btimap = new BitmapImage(new Uri(uriImg));
                                string strState = Convert.ToString(contact.GetContactInformation(ContactInformationType.Activity));
                                //bool imgIsExit = UriManage.RemoteFileExists(uriImg);
                                if (strState.Equals("脱机"))
                                {
                                    participantsEntity.LoginState = "未登录";
                                    participantsEntity.StateForeBrush = App.Current.Resources["NormalColorBrush"] as SolidColorBrush;
                                    participantsEntity.HeadPortrait = ImageManage.ToGray(btimap);
                                }
                                else
                                {
                                    participantsEntity.LoginState = "在线";
                                    participantsEntity.StateForeBrush = App.Current.Resources["GreenColorBrush"] as SolidColorBrush;
                                    participantsEntity.HeadPortrait = btimap;
                                }
                                //添加参会人
                                ParticipantsEntityList.Add(participantsEntity);
                            }
                        }
                    }
                    if (BeginRefleshDataGridCallBack != null)
                    {
                        BeginRefleshDataGridCallBack(currentParticipantsEntityList);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 参会人状态更改捕获事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Constant.lyncClient != null && Constant.lyncClient.State == ClientState.SignedIn)
                    {
                        //获取联系人
                        Contact contact = sender as Contact;
                        if (contact != null)
                        {

                            System.Collections.Generic.List<object> list = (System.Collections.Generic.List<object>)contact.GetContactInformation(ContactInformationType.EmailAddresses);
                            if (list.Count > 0 && currentParticipantsEntityList != null)
                            {
                                //double state = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));

                                List<ParticipantsEntity> participantsEntityTempList = currentParticipantsEntityList.Where(item => item.LoginUri.ToLower().Equals(Convert.ToString(list[0]).ToLower())).ToList<ParticipantsEntity>();

                                if (participantsEntityTempList.Count > 0)
                                {
                                    ParticipantsEntity Participant = participantsEntityTempList[0];

                                    string uriImg = Constant.TreeServiceAddressFront + Constant.FtpServercePersonImgName + Participant.LoginName + ".png";
                                    BitmapImage btimap = new BitmapImage(new Uri(uriImg));
                                    string strState = Convert.ToString(contact.GetContactInformation(ContactInformationType.Activity));
                                    //bool imgIsExit = UriManage.RemoteFileExists(uriImg);
                                    if (strState.Equals("脱机"))
                                    {
                                        Participant.LoginState = "未登录";
                                        Participant.StateForeBrush = App.Current.Resources["NormalColorBrush"] as SolidColorBrush;
                                        Participant.HeadPortrait = ImageManage.ToGray(btimap);
                                    }
                                    else
                                    {
                                        Participant.LoginState = "在线";
                                        Participant.StateForeBrush = App.Current.Resources["GreenColorBrush"] as SolidColorBrush;
                                        Participant.HeadPortrait = btimap;
                                    }
                                    //获取当前人的公司名称
                                    Participant.Company = Convert.ToString(contact.GetContactInformation(ContactInformationType.Company));
                                    ////获取当前人的职位
                                    Participant.Position = Convert.ToString(contact.GetContactInformation(ContactInformationType.Title));
                                }
                                if (BeginRefleshDataGridCallBack != null)
                                {
                                    BeginRefleshDataGridCallBack(currentParticipantsEntityList);
                                }
                            }
                        }
                    }
                }));

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 隐藏托盘图标

        /// <summary>
        /// lync程序环境设置（事件、状态、原生态界面抑制、注册表、标示）
        /// </summary>
        public static void SetLyncAplicationEnviroment(Action containCompleateCallBack)
        {
            try
            {
                //获取lync进程
                Process[] processes = Process.GetProcessesByName("Lync");
                if (processes.Count() > 0)
                {
                    IntPtr handle = ProcessManage.GetMainWindowHandle(processes[0]);
                    if (handle.ToInt32() > 0)
                    {
                        //获取LYNC客户端
                        Constant.lyncClient = LyncClient.GetClient();
                        if (Constant.lyncClient != null)
                        {
                            #region 注册Lync事件

                            //lync状态更改事件
                            Constant.lyncClient.StateChanged += lyncClient_StateChanged;

                            #endregion

                            #region lync初始化

                            //首先将捕获到的lync实例进行一次初始化加载
                            if (Constant.lyncClient.State == ClientState.Uninitialized)
                            {
                                //lync客户端初始化
                                Constant.lyncClient.BeginInitialize(null, null);
                            }

                            //签入
                            if (Constant.lyncClient.State == ClientState.SigningIn || Constant.lyncClient.State == ClientState.SignedIn)
                            {
                                //先签出（lync控制）
                                Constant.lyncClient.BeginSignOut(null, null);
                            }
                            if (Constant.lyncClient.State == ClientState.SignedOut)
                            {
                                //可以签入
                                MainWindow.CanSigined = true;
                            }

                            #endregion

                            #region 程序相关设置

                            WindowHide.SetTrayIconVisible("Lync", false);

                            ////将lync的原生态的主窗体封装起来
                            APPContainManage.APP_Conatain(handle);

                            containCompleateCallBack();

                            #endregion
                        }
                    }
                    else
                    {
                        LyncHelper.SetLyncAplicationEnviroment(containCompleateCallBack);
                    }
                }
                else
                {
                    LyncHelper.SetLyncAplicationEnviroment(containCompleateCallBack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region lync事件（状态更改、会话、联系人、信息）【提供全局使用】

        /// <summary>
        /// lync状态更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void lyncClient_StateChanged(object sender, ClientStateChangedEventArgs e)
        {
            //异步委托
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //已签入或者签入中
                    if (e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedIn)
                    {
                        if (State1CallBack != null)
                        {
                            State1CallBack();
                        }

                    }
                    //当前lync状态为正要签出的状态
                    else if (e.OldState == ClientState.SignedIn && e.NewState == ClientState.SigningOut)
                    {
                        #region 恢复未登陆前的状态

                        if (State2CallBack != null)
                        {
                            State2CallBack();
                        }

                        #endregion
                    }
                    else if (e.OldState == ClientState.SigningIn && e.NewState == ClientState.SignedOut)
                    {
                        //登陆面板设置为可用
                        //this.LoginPanelIsEnable = true;
                        if (State3CallBack != null)
                        {
                            State3CallBack();
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(typeof(LyncHelper), ex);
                }
            }));
        }

        #endregion

        #region 签出

        /// <summary>
        /// 签出
        /// </summary>
        public static void LyncSignOut()
        {
            try
            {
                //如果为签入状态，则设置为签出
                if (Constant.lyncClient.State == Microsoft.Lync.Model.ClientState.SignedIn || Constant.lyncClient.State == Microsoft.Lync.Model.ClientState.SigningIn)
                {
                    //lync签出
                    Constant.lyncClient.BeginSignOut(null, null);
                    //lync异常（与本系统无关）
                    //Constant.lyncClient.SignInConfiguration.ForgetMe(Constant.SelfUri);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 退出lync辅助
        /// </summary>
        public static void LyncCloseHelper(Action callBack)
        {
            try
            {
                if (Constant.lyncClient != null)
                {
                    callBack();
                    //签出
                    LyncHelper.LyncSignOut();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 设置当前用户名

        public static void SetCurrentUser()
        {
            try
            {
                #region 设置当前用户名和邮箱（全局使用）


                //设置当前用户名和当前邮箱地址
                if (Constant.lyncClient.Self.Contact != null)
                {
                    string selfName = "@";
                    while (selfName.Contains("@"))
                    {
                        //设置当前参会人名称
                        selfName = Convert.ToString(Constant.lyncClient.Self.Contact.GetContactInformation(ContactInformationType.DisplayName));
                    }
                    //当前参会人
                    Constant.SelfName = selfName;
                    //设置当前参会人邮箱地址
                    Constant.SelfUri = Constant.lyncClient.Self.Contact.Uri.Replace("sip:", string.Empty);
                }

                #endregion
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region lync状态设置

        public static void LyncStateSetting(int stateIndex)
        {
            try
            {
                if (Constant.lyncClient.SignInConfiguration != null)
                {

                    //通过选择的用户状态设置lync config相关状态信息
                    var stateEnum = (UserLoginState)stateIndex;
                    //状态（个人模式时使用）
                    switch (stateEnum)
                    {
                        //在线
                        case UserLoginState.Available:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Free;
                            break;
                        //忙碌
                        case UserLoginState.Busy:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Busy;
                            break;
                        //请勿打扰
                        case UserLoginState.DoNotDisturb:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.DoNotDisturb;
                            break;
                        //离开
                        case UserLoginState.Away:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.TemporarilyAway;
                            break;
                        //下班
                        case UserLoginState.OffWork:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Offline;
                            break;
                        //离开
                        case UserLoginState.Leave:
                            Constant.lyncClient.SignInConfiguration.SignInAsAvailability = ContactAvailability.Away;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 判断是否可以进行嵌入

        /// <summary>
        /// 判断是否可以进行嵌入
        /// </summary>
        public static void IsSignedOutDoSomeThing(Action callBack)
        {
            try
            {
                if (Constant.lyncClient != null && Constant.lyncClient.State == ClientState.SignedOut)
                {
                    callBack();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 取消登录

        public static void CancelLyncSigned(Action callBack)
        {
            try
            {
                //停止嵌入并退出登陆框
                if (Constant.lyncClient.State == ClientState.SigningIn)
                {
                    Constant.lyncClient.BeginSignOut(null, null);
                }
                callBack();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region lync嵌入

        public static void LyncSignning(string UserUri, string Password, Action callBack)
        {
            try
            {
                if (Constant.lyncClient.State != ClientState.SignedIn && Constant.lyncClient.State != ClientState.SigningIn)
                {
                    //开始签入
                    IAsyncResult ar = Constant.lyncClient.BeginSignIn(
                        UserUri,
                        UserUri,
                        Password,
                        null,
                        null);
                }
                callBack();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region lync会话事件注册

        /// <summary>
        /// lync会话事件注册
        /// </summary>
        /// <param name="hasConferenceCallBack"></param>
        /// <param name="mainConversationOutCallBack"></param>
        /// <param name="conversationAddCompleateCallBack"></param>
        /// <param name="mainConversationInCallBack"></param>
        /// <param name="contentAddCompleateCallBack"></param>
        public static void LyncConversationEventRegedit(Action<Action<bool>> hasConferenceCallBack,
            Action<ConversationWindow> mainConversationOutCallBack, Action conversationAddCompleateCallBack,
            Action<Action<ConversationWindow>> mainConversationInCallBack,
            Action<string> contentAddCompleateCallBack, Action content_DeskRemoveCompleateCallBack, Action<string> presentCallBack)
        {
            try
            {
                //lync客户端对象模型不为null则注册相应事件
                if (Constant.lyncClient != null)
                {
                    //是否包含会议
                    HasConferenceCallBack = hasConferenceCallBack;
                    //主会话关联
                    MainConversationOutCallBack = mainConversationOutCallBack;
                    //加载内容完成事件
                    ConversationAddCompleateCallBack = conversationAddCompleateCallBack;
                    //主会话判断事件
                    MainConversationInCallBack = mainConversationInCallBack;
                    //会话加载完成事件
                    ContentAddCompleateCallBack = contentAddCompleateCallBack;
                    //返回演示人
                    PresentCallBack = presentCallBack;
                    //断开桌面共享
                    Content_DeskRemoveCompleateCallBack = content_DeskRemoveCompleateCallBack;

                    //lync会话添加事件
                    Constant.lyncClient.ConversationManager.ConversationAdded += LyncHelper.ConversationManager_ConversationAdded;

                    //lync会话移除事件
                    Constant.lyncClient.ConversationManager.ConversationRemoved += LyncHelper.ConversationManager_ConversationRemoved;

                    #region old solution

                    //lyncEventManage.HasConferenceEvent += lyncEventManage_HasConferenceEvent;

                    //lyncEventManage.MainConversationOutEvent += lyncEventManage_MainConversationAbout;

                    //lyncEventManage.ContentAddCompleateEvent += lyncEventManage_ContentAddCompleateEvent;

                    //lyncEventManage.MainConversationInEvent += lyncEventManage_MainConversationInEvent;

                    //lyncEventManage.ConversationAddCompleateEvent += lyncEventManage_ConversationAddCompleateEvent;



                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion
    }
}
