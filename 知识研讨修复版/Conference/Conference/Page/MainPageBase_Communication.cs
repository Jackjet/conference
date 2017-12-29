using Conference.Common;
using Conference.View.AppcationTool;
using Conference.View.Chair;

using Conference.View.Setting;
using Conference.View.Space;
using Conference.View.Tool;
using Conference_Conversation;
using Conference_Conversation.Common;
using Conference_Tree;
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
using C_BaseData = ConferenceWebCommon.Common.PackageBase;
using maxtrix = ConferenceWebCommon.EntityHelper.ConferenceMatrix;
using lync_webData = ConferenceWebCommon.EntityHelper.LyncConversation;
using tree_webData = ConferenceWebCommon.EntityHelper.ConferenceTree;
using space_webData = ConferenceWebCommon.EntityHelper.ConferenceOffice;
using info_webData = ConferenceWebCommon.EntityHelper.ConferenceInfo;
using view_Selected = ConferenceCommon.EnumHelper.ViewSelectedItemEnum;
using aceeptType_Common = ConferenceWebCommon.Common.ConferenceClientAcceptType;


namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 会话初始化

        /// <summary>
        /// 会话初始化
        /// </summary>
        public void LyncConversationInit()
        {
            try
            {
                //检查当前会议是否包含会话
                ModelManage.ConferenceLyncConversation.ContainConversation(Constant.ConferenceName, new Action<bool, string>((successed, meet_Address) =>
                {
                    ModelManage.ConferenceLyncConversation.CheckConversationInit(Constant.ConferenceName, new Action<bool>((canInit) =>
                    {
                        ////检测是否可以会话初始化计时数量
                        //this.CheckConversationInitCount++;
                        //if (this.CheckConversationInitCount == 8 && !string.IsNullOrEmpty(meet_Address))
                        //{
                        //    //允许其他成员进行会议查询并进行初始化
                        //    ModelManage.ConferenceLyncConversation.AllowConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                        //    {

                        //    }));
                        //}
                        if (canInit)
                        {
                            //设置DNS
                            NetWorkAdapter.SetNetworkAdapter(Constant.DNS1);
                            //设置置顶
                            MainWindow.mainWindow.Topmost = true;
                            //创建或进入会话
                            this.MeetSetupOrEnter(successed, meet_Address);
                        }
                        else
                        {
                            TimerJob.StartRun(new Action(() =>
                            {
                                this.LyncConversationInit();
                            }), 1200);
                        }
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

        #region 获取会议地址上传到服务器（解除锁定）

        /// <summary>
        /// 获取会议地址上传到服务器（解除锁定）
        /// </summary>
        /// <param name="successed">是否上传成功</param>
        public void UPloadMeetAddressAndCancelLock(bool successed)
        {
            try
            {
                //只有会话初始化完毕才有的结果
                DispatcherTimer lynTimer = null;
                TimerJob.StartRun(new Action(() =>
                {
                    string address = LyncHelper.GetConversation_Address();
                    if (!string.IsNullOrEmpty(address))
                    {
                        lynTimer.Stop();
                        //开启会话
                        ModelManage.ConferenceLyncConversation.FillConversation(Constant.ConferenceName, address, new Action<bool>((issuccessed) =>
                        {
                            if (successed)
                            {
                                //允许其他成员进行会议查询并进行初始化
                                ModelManage.ConferenceLyncConversation.AllowConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                                {
                                }));
                            }
                        }));

                        //释放dns（改为自由获取）,主窗体状态还原（非置顶）
                        TimerJob.StartRun(new Action(() =>
                        {
                            //设置DNS
                            NetWorkAdapter.SetNetworkAdapter(Constant.RouteIp);
                            //取消置顶
                            MainWindow.mainWindow.Topmost = false;
                        }));
                    }

                }), 500, out lynTimer);
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

        #region 创建或进入会话

        /// <summary>
        /// 创建或进入会话
        /// </summary>
        /// <param name="successed">执行是否包含会话是否成功</param>
        /// <param name="meetAddress">会议地址</param>
        private void MeetSetupOrEnter(bool successed, string meet_Address)
        {
            try
            {
                if (successed && string.IsNullOrEmpty(meet_Address))
                {
                    //禁止其他人进行查询完之后直接进行会话初始化（有可能在其中过程当中,第一个开启会话的人还没有准备完毕，所以记进行一次通知）
                    ModelManage.ConferenceLyncConversation.ForbiddenConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                    {
                    }));

                    //判断主会话是否存在
                    if (LyncHelper.MainConversation == null)
                    {
                        //参会人列表
                        List<string> list = Constant.ParticipantList.Where(Item => Item.Equals(Constant.SelfUri)).ToList<string>();
                        //添加对应大屏
                        list.Add(Constant.BigScreenName);
                        //开启会话
                        LyncHelper.StartConference(Constant.lyncClient, Constant.ConferenceName, Constant.SelfName, list, LyncHelper.MainConversation);
                        //获取会议地址上传到服务器
                        this.UPloadMeetAddressAndCancelLock(successed);
                        this.meetAddress = meet_Address;
                    }
                }
                else
                {
                    this.meetAddress = meet_Address;
                    //使用浏览器的方式参会
                    LyncHelper.JoinConversationByWebBrowser(meet_Address);
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

        #region 接收到其他参会人所发送的数据（获取到服务器发送的数据）

        /// <summary>
        /// 接收到其他参会人所发送的数据
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        protected void clientSocket_TCPDataArrival(C_BaseData args)
        {
            try
            {
                lock (Communication_Object)
                {
                    switch (args.ConferenceClientAcceptType)
                    {
                        //研讨树
                        case aceeptType_Common.ConferenceTree:
                            //知识树同步辅助
                            this.ConferenceTreeSyncHelper(args);

                            break;

                        //语音研讨
                        case aceeptType_Common.ConferenceAudio:
                            //信息交流同步辅助
                            ConferenceIMMSyncHelper(args);
                            break;

                        case aceeptType_Common.ConferenceFileSync:
                            //甩屏同步辅助
                            this.ConferenceFileSyncHelper();

                            break;

                        case aceeptType_Common.ConferenceSpaceSync:

                            //智存空间同步辅助
                            this.ConferenceSpaceSyncHelper(args);

                            break;
                        case aceeptType_Common.ConferenceInfoSync:
                            //信息同步辅助
                            this.ConferenceInfoSyncHelper(args);

                            break;

                        case aceeptType_Common.LyncConversationSync:
                            //lync同步辅助
                            this.LyncConversationSyncHelper(args);

                            break;

                        case aceeptType_Common.ConferenceMatrixSync:
                            //矩阵同步辅助
                            this.ConferenceMatrixSyncHelper(args);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 信息交流同步辅助

        /// <summary>
        /// 信息交流同步辅助
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private void ConferenceIMMSyncHelper(C_BaseData args)
        {
            try
            {
                //非同一线程，进行线程异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.conferenceAudio_View != null)
                    {
                        //收到通知获当前同步取节点
                        this.ConferenceAudio_View.Information_Sync(EntityTransfer.AudioTransferEntityChanged(args.ConferenceAudioItemTransferEntity));
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

        #region 甩屏同步辅助

        /// <summary>
        /// 甩屏同步辅助
        /// </summary>
        void ConferenceFileSyncHelper()
        {
            try
            {
                //会议主持人无需进行甩屏操作，只需接收参会人甩屏的数据即可
                if (Constant.IsMeetingPresenter)
                {
                    //非同一线程，进行线程异步委托
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            //最后还是通过webservice进行获取相应数据（底层使用的套接字构造，仅仅为了起到通知的效果）
                            ModelManage.FileSyncAppPool.GetSyncServiceData(Constant.ConferenceName, new Action<bool, FileSyncAppEntity>((successed, result) =>
                            {
                                //主持人响应参会人的甩屏
                                if (this.AppSyncDataAcceptWindow == null && successed)
                                {
                                    this.AppSyncDataAcceptWindow = new AppSyncDataAcceptWindow();
                                    //刷屏窗体关闭事件
                                    this.AppSyncDataAcceptWindow.Closing -= AppSyncDataAccept_Window_Closing;
                                    this.AppSyncDataAcceptWindow.Closing += AppSyncDataAccept_Window_Closing;
                                    //甩屏窗体显示
                                    this.AppSyncDataAcceptWindow.Show();
                                    //甩屏窗体激活
                                    this.AppSyncDataAcceptWindow.Activate();
                                }
                                this.AppSyncDataAcceptWindow.FillData(result.ImgBytes);
                            }));
                        }
                        catch (OperationException ex)
                        {
                            LogManage.WriteLog(this.GetType(), ex);
                        };
                    }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 知识树区域同步辅助

        /// <summary>
        /// 知识树区域同步辅助
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        void ConferenceTreeSyncHelper(C_BaseData args)
        {
            try
            {
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //知识树判断不为null
                    if (this.conferenceTreeView != null)
                    {
                        if (args.ConferenceTreeFlg != null)
                        {
                            //知识树操作模式
                            switch (args.ConferenceTreeFlg.ConferenceTreeFlgType)
                            {
                                case tree_webData.ConferenceTreeFlgType.normal:
                                    //知识树子节点点对点进行同步
                                    ConferenceTreeItem.Information_Sync(EntityTransfer.TreeTransferEntityChanged(args.ConferenceTreeFlg.ConferenceTreeItemTransferEntity));
                                    break;
                                case tree_webData.ConferenceTreeFlgType.instead:
                                    //知识树拖动
                                    tree_webData.ConferenceTreeInsteadEntity insteadEntity = args.ConferenceTreeFlg.ConferenceTreeInsteadEntity;
                                    //知识树节点替换
                                    this.ConferenceTreeView.InsteadElement(insteadEntity.BeforeItemGuid, insteadEntity.NowItemGuid);

                                    break;
                                default:
                                    break;
                            }
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

        #region 矩阵同步辅助

        /// <summary>
        /// 矩阵同步辅助
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        void ConferenceMatrixSyncHelper(C_BaseData args)
        {
            try
            {
                //矩阵切换
                if (args.ConferenceMatrixBase is maxtrix.ConferenceMatrixEntity)
                {
                    //类型还原
                    maxtrix.ConferenceMatrixEntity conferenceMatrixEntity = args.ConferenceMatrixBase as maxtrix.ConferenceMatrixEntity;
                    //非同一线程，进行线程异步委托
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            //接受到投影通知
                            //this.ChairView.RecieveSilexNotification(conferenceMatrixEntity.Sharer, (int)conferenceMatrixEntity.ConferenceMatrixOutPut);
                        }
                        catch (Exception ex)
                        {
                            LogManage.WriteLog(this.GetType(), ex);
                        }
                        finally
                        {
                        }

                    }));
                }
                //位置变动
                else if (args.ConferenceMatrixBase is maxtrix.SeatEntity)
                {
                    //非同一线程，进行线程异步委托
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            //更改座位信息
                            //this.ChairView.UpdateOneSeat(args.ConferenceMatrixBase as maxtrix.SeatEntity);
                            //更改座位信息
                            this.MyConferenceView.UpdateOneSeat(args.ConferenceMatrixBase as maxtrix.SeatEntity);
                        }
                        catch (Exception ex)
                        {
                            LogManage.WriteLog(this.GetType(), ex);
                        }
                        finally
                        {
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 智存空间同步辅助

        /// <summary>
        /// 智存空间同步辅助
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        void ConferenceSpaceSyncHelper(C_BaseData args)
        {
            try
            {
                //获取智存空间文件共享映射实体数据
                space_webData.ConferenceSpaceAsyncEntity conferenceOfficeAsyncEntity = args.ConferenceSpaceAsyncEntity;
                try
                {
                    //异步委托
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (ForceToNavicateCallBack != null)
                        {
                            //强制导航到共享页面
                            this.ForceToNavicateCallBack(view_Selected.Resource);
                        }
                        //打开文件【word,excel,ppt】Wait_Singned
                        this.ConversationM.FileOpenByExtension((ConferenceCommon.WPFControl.FileType)conferenceOfficeAsyncEntity.FileType, conferenceOfficeAsyncEntity.Uri);
                        //共享协作页面指定
                        this.ConversationM.PageIndex = ResourceType.Normal;
                        //共享协作共享演示者
                        this.ConversationM.ResourcePusher = conferenceOfficeAsyncEntity.Sharer;
                        //推送者显示
                        this.ConversationM.ResourcePusherVisibility = System.Windows.Visibility.Visible;
                    }));
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
            catch (OperationException ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            };
        }

        #endregion

        #region 信息同步辅助

        /// <summary>
        /// 信息同步辅助
        /// </summary>
        void ConferenceInfoSyncHelper(C_BaseData args)
        {
            try
            {
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        switch (args.ConferenceInfoFlg.ConferenceInfoFlgType)
                        {
                            case info_webData.ConferenceInfoFlgType.ConferenceInfoEntity:
                                //页面同步
                                this.Info_Page(args);
                                break;
                            case info_webData.ConferenceInfoFlgType.ConferenceInfoTypeChangeEntity:
                                //场景模式切换
                                this.Info_Model(args);
                                break;
                            case info_webData.ConferenceInfoFlgType.ConferenceClientControlEntity:
                                //信息控制
                                MainPageBase.Info_Control(args);
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 信息控制
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private static void Info_Control(C_BaseData args)
        {
            try
            {
                //操作人
                string sharer = args.ConferenceInfoFlg.ConferenceClientControlEntity.Sharer;
                switch (args.ConferenceInfoFlg.ConferenceClientControlEntity.ClientControlType)
                {
                    case info_webData.ClientControlType.Close:
                        //关闭其他参会人
                        if (sharer != Constant.SelfUri)
                        {
                            Application.Current.Shutdown(0);
                        }
                        break;
                    case info_webData.ClientControlType.VersionUpdate:
                        //强制其他参会人进行版本更新
                        if (sharer != Constant.SelfUri)
                        {
                            //版本检测更新
                            VersionUpdateManage.JustUpdate(Constant.ConferenceVersionUpdateExe);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(MainPageBase), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 场景模式切换
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private void Info_Model(C_BaseData args)
        {
            try
            {
                //是否为精简模式
                bool isSimpleModel = args.ConferenceInfoFlg.ConferenceInfoTypeChangeEntity.IsSimpleModel;
                //是否为教育模式
                bool isEducationModel = args.ConferenceInfoFlg.ConferenceInfoTypeChangeEntity.IsEducationModel;
                //模式管理中心
                this.Model_ManageCenter(isEducationModel, isSimpleModel);
                if (Constant.TempConferenceInformationEntity != null)
                {
                    //当前临时会议信息更改（模式相关字段）
                    Constant.TempConferenceInformationEntity.SimpleMode = isSimpleModel;
                    Constant.TempConferenceInformationEntity.EducationMode = isEducationModel;
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
        /// 页面同步
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private void Info_Page(C_BaseData args)
        {
            try
            {
                //获取信息同步映射实体数据
                info_webData.ConferenceInfoEntity conferenceInfoEntity = args.ConferenceInfoFlg.ConferenceInfoEntity;
                //强制指定到具体页面                               
                if (ForceToNavicateCallBack != null)
                {
                    this.ForceToNavicateCallBack((view_Selected)conferenceInfoEntity.ConferencePageType);
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

        #region lync同步辅助

        /// <summary>
        /// lync同步辅助
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        void LyncConversationSyncHelper(C_BaseData args)
        {
            try
            {
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        switch (args.LyncConversationFlg.LyncConversationFlgType)
                        {
                            case lync_webData.LyncConversationFlgType.InviteContact:
                                //获取信息同步映射实体数据
                                //lync_webData.LyncConversationEntity conferenceInfoEntity = args.LyncConversationFlg.LyncConversationEntity;
                                //邀请某人进入会话
                                //lyncEventManage.InviteSomeOneJoinMainConference(Constant.lyncClient, MainConversation, conferenceInfoEntity.JonConferencePerson);
                                break;
                            case lync_webData.LyncConversationFlgType.EnterBigScreen:
                                //投影大屏幕
                                this.Lync_BigScreenEnter(args);
                                break;

                            case lync_webData.LyncConversationFlgType.PPTControl:
                                //ppt控制
                                this.Lync_PPT(args);
                                break;

                            case lync_webData.LyncConversationFlgType.LeaveConversation:
                                //离开会议
                                MainPage.Lync_Leave(args);

                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                    finally
                    {
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 离开会议
        /// </summary>
        /// <param name="args"></param>
        private static void Lync_Leave(C_BaseData args)
        {
            try
            {
                //lync联系人异常实体
                lync_webData.LeaveConversationEntity leaveConversationEntity = args.LyncConversationFlg.LeaveConversationEntity;
                //退出会议的参会人
                string contactUri = leaveConversationEntity.ContactUri;
                //异常参会人
                LyncHelper.RemovePartical(contactUri);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(MainPageBase), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// ppt控制
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private void Lync_PPT(C_BaseData args)
        {
            try
            {
                //ppt共享
                if (args.LyncConversationFlg.PPTControlEntity.Controler.Equals(Constant.LoginUserName))
                {
                    if (ViewSelectedItemEnum == view_Selected.Resource)
                    {
                        if (this.ConversationM.PageIndex == ResourceType.Share)
                        {
                            //模仿鼠标点击
                            Win32API.SetCursorPos(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2);
                            Win32API.mouse_event(Win32API.MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                            Win32API.mouse_event(Win32API.MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
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
        /// 投影大屏幕
        /// </summary>
        /// <param name="args">通讯基础数据包</param>
        private void Lync_BigScreenEnter(C_BaseData args)
        {
            try
            {
                //大屏实体
                lync_webData.BigScreenEnterEntity bigScreenEnterEntity = args.LyncConversationFlg.BigScreenEnterEntity;
                //更改状态
                MainWindow.mainWindow.mainPage.SharingPanel.UpdateState(bigScreenEnterEntity.Sharer);
                //显示投影人
                MainPage.mainPage.presentCallBack(bigScreenEnterEntity.Sharer);
                //判断大屏投影人是否为当前参会人
                if (!bigScreenEnterEntity.Sharer.Equals(Constant.SelfName))
                {
                    //强制导航到资源共享
                    MainPage.mainPage.ForceToNavicate(view_Selected.Resource);
                    //共享协作页面
                    this.ConversationM.PageIndex = ResourceType.Share;
                    //进行最大化显示
                    if (MainWindow.mainWindow.WindowState == WindowState.Minimized)
                    {
                        MainWindow.mainWindow.WindowState = WindowState.Maximized;
                    }
                    //设置会话区域显示内容
                    this.ConversationM.SetConversationAreaShow(ShowType.ConversationView, true);
                }
                else if (bigScreenEnterEntity.Sharer.Equals(Constant.SelfName))
                {
                    //加载大屏幕
                    LyncHelper.InviteSomeOneJoinMainConference(Constant.lyncClient, LyncHelper.MainConversation, Constant.BigScreenName);
                    //设置会话区域显示内容
                    this.ConversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);
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

        #region 通讯检测回调

        /// <summary>
        /// 页面刷新回调
        /// </summary>
        private void PageRefleshCallBack()
        {
            try
            {
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       //刷新页面
                       MainPage.mainPage.PageReflesh(viewSelectedItemEnum);
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

        /// <summary>
        /// 通讯中断修复之后页面恢复
        /// </summary>
        private void RepareMainWindowAndNavicateToIndeCallBack()
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       //主窗体设置位置
                       if (MainWindow.mainWindow.Top != 0)
                       {
                           MainWindow.mainWindow.Top = 0;
                       }
                       if (MainWindow.mainWindow.Left != 0)
                       {
                           MainWindow.mainWindow.Left = 0;
                       }

                       //强制导航到资源共享
                       MainPage.mainPage.ForceToNavicate(this.ViewSelectedItemEnum);
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

        #region 释放之前使用的通讯节点

        /// <summary>
        /// 释放之前使用的通讯节点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="compleateCallback">释放通讯节点回调</param>
        public void DisPoseServerSocketArray(string conferenceName, Action<bool> compleateCallback)
        {
            try
            {
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //关闭本地套接字的连接
                    this.CommunicationManage.Communication_Server_Client_Disopose();

                    //移除矩阵应用通讯节点
                    ModelManage.ConferenceInfo.RemoveCurrentUser_AllClientSocket(conferenceName, Constant.SelfUri, new Action<bool>((successed) =>
                    {
                        if (successed)
                        {
                            if (compleateCallback != null)
                            {
                                //函数回调
                                compleateCallback(true);
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                if (compleateCallback != null)
                {
                    //函数回调
                    compleateCallback(false);
                }
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion
    }
}