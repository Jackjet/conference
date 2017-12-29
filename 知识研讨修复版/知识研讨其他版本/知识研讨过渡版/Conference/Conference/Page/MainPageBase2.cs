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
using Conference.View.U_Disk;
using Conference.View.WebBrowser;
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
        #region 会话初始化

        /// <summary>
        /// 会话初始化
        /// </summary>
        public void LyncConversationInit()
        {
            try
            {
                //检查当前会议是否包含会话
                ModelManage.ConferenceLyncConversation.ContainConversation(Constant.ConferenceName, new Action<bool, string>((successed, meetAddress) =>
                {
                    ModelManage.ConferenceLyncConversation.CheckConversationInit(Constant.ConferenceName, new Action<bool>((canInit) =>
                    {
                        this.CheckConversationInitCount++;
                        if (this.CheckConversationInitCount == 8 && !string.IsNullOrEmpty(meetAddress))
                        {
                            #region 允许其他成员进行会议查询并进行初始化

                            ModelManage.ConferenceLyncConversation.AllowConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                            {

                            }));

                            #endregion
                        }
                        if (canInit)
                        {
                            //设置DNS
                            NetWorkAdapter.SetNetworkAdapter(Constant.DNS1);
                            //设置置顶
                            MainWindow.mainWindow.Topmost = true;

                            #region 会话配置

                            if (successed && string.IsNullOrEmpty(meetAddress))
                            {
                                #region 禁止其他人进行查询完之后直接进行会话初始化（有可能在其中过程当中,第一个开启会话的人还没有准备完毕，所以记进行一次通知）

                                ModelManage.ConferenceLyncConversation.ForbiddenConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                                {

                                }));

                                #endregion

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
                                }
                            }
                            else
                            {
                                //使用浏览器的方式参会
                                this.JoinConversationByWebBrowser(meetAddress);
                            }

                            #endregion
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

        public void UPloadMeetAddressAndCancelLock(bool successed)
        {
            try
            {
                #region 获取会议地址并上传到服务器

                //只有会话初始化完毕才有的结果
                DispatcherTimer lynTimer = null;
                TimerJob.StartRun(new Action(() =>
                {
                    if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.Conversation.Properties.ContainsKey(ConversationProperty.ConferenceAccessInformation))
                    {
                        ConferenceAccessInformation conferenceAccessInformation = (ConferenceAccessInformation)LyncHelper.MainConversation.Conversation.Properties[ConversationProperty.ConferenceAccessInformation];
                        if (!string.IsNullOrEmpty(conferenceAccessInformation.InternalUrl))
                        {
                            //开启会话
                            ModelManage.ConferenceLyncConversation.FillConversation(Constant.ConferenceName, conferenceAccessInformation.InternalUrl, new Action<bool>((issuccessed) =>
                            {
                                if (successed)
                                {
                                    #region 允许其他成员进行会议查询并进行初始化

                                    ModelManage.ConferenceLyncConversation.AllowConversationInit(Constant.ConferenceName, new Action<bool>((flg) =>
                                    {

                                    }));

                                    #endregion
                                }
                            }));

                            #region 释放dns（改为自由获取）,主窗体状态还原（非置顶）

                            TimerJob.StartRun(new Action(() =>
                            {
                                //设置DNS
                                NetWorkAdapter.SetNetworkAdapter(Constant.RouteIp);
                                //取消置顶
                                MainWindow.mainWindow.Topmost = false;
                            }));
                            #endregion

                            lynTimer.Stop();
                        }
                    }
                }), 500, out lynTimer);

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

        #region 使用浏览器的方式参会

        /// <summary>
        /// 使用浏览器的方式参会
        /// </summary>
        public void JoinConversationByWebBrowser(string meetAddress)
        {
            try
            {
                //主会议地址
                //this.MeetAddressMain = meetAddress;

                #region 使用浏览器的方式参会

                if (this.webBrowser != null)
                {
                    this.webBrowser.Dispose();
                }
                this.webBrowser = new System.Windows.Forms.WebBrowser();
                //导航到指定地址
                this.webBrowser.Navigate(meetAddress);

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

        #region 获取服务端口

        /// <summary>
        /// 获取服务端口
        /// </summary>
        public void GetServicePort(Action callBack)
        {
            try
            {
                //获取知识树服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceTree, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {
                }));

                //获取语音服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceAudio, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {
                }));

                //获取消息服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceInfoSync, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {
                }));

                //获取lync服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.LyncConversationSync, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {

                }));

                //获取智存空间服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceSpaceSync, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {

                }));

                //获取矩阵服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceMatrixSync, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, MeetPort) =>
                {

                }));

                //获取文件服务端口【甩屏】
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceFileSync, new Action<bool, ConferenceModel.ConferenceInfoWebService.PortTypeEntity>((Successed, portTypeEntity) =>
                {
                    //调用成功
                    if (Successed)
                    {
                        TimerJob.StartRun(new Action(() =>
                        {
                            switch (portTypeEntity.conferenceClientAcceptType)
                            {
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceTree:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.TreeClientSocket, portTypeEntity.ServerPoint);
                                    //进行通讯检测
                                    //this.needCheckSocekt = true;

                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceAudio:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.AudioClientSocket, portTypeEntity.ServerPoint);
                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceFileSync:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.FileClientSocket, portTypeEntity.ServerPoint);
                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceSpaceSync:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.SpaceClientSocket, portTypeEntity.ServerPoint);
                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceInfoSync:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.InfoClientSocket, portTypeEntity.ServerPoint);
                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.LyncConversationSync:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.LyncClientSocket, portTypeEntity.ServerPoint);
                                    break;
                                case ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceMatrixSync:
                                    //通知服务端进行套接字的收集
                                    this.Communication_Server_Client(this.MatrixClientSocket, portTypeEntity.ServerPoint);

                                    #region 位置信息获取并设置

                                    ModelManage.ConferenceMatrix.IntoOneSeat(Constant.ConferenceName, TempConferenceInformationEntity.SettingIpList, Constant.SelfName, Constant.LocalIp, new Action<bool, List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity>>((successed, seatEntityList) =>
                                    {
                                        if (successed)
                                        {
                                            //刷新座位分布情况
                                            //this.ChairView.Reflesh(seatEntityList);
                                            //刷新座位分布情况
                                            this.MyConferenceView.Reflesh(seatEntityList);
                                            callBack();
                                        }
                                    }));

                                    #endregion

                                    break;

                                default:
                                    break;
                            }

                        }), 200);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 通讯机制

        /// <summary>
        /// 通知服务端进行套接字的收集
        /// </summary>
        protected void Communication_Server_Client(ClientSocket clientSocket, int port)
        {
            try
            {
                //服务器对该会议开放的端口
                this.intPortNow = port;
                //远程连接
                clientSocket.ConnectRemotePoint(Constant.TreeServiceIP, port);
                //数据接收事件
                clientSocket.TCPDataArrival += clientSocket_TCPDataArrival;

                //发送通知
                ConferenceWebCommon.Common.ConferenceClientAccept conferenceClientAccept = new ConferenceWebCommon.Common.ConferenceClientAccept();
                //会议名称
                conferenceClientAccept.ConferenceName = Constant.ConferenceName;
                //当前参会人uri地址
                conferenceClientAccept.SelfUri = Constant.SelfUri;

                //发送
                clientSocket.SendRemoteData(conferenceClientAccept);
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
        /// 关闭本地套接字的连接
        /// </summary>
        protected void Communication_Server_Client_Disopose()
        {
            try
            {
                if (this.TreeClientSocket != null)
                {
                    this.TreeClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.TreeClientSocket.CloseConnect();
                }

                if (this.AudioClientSocket != null)
                {
                    this.AudioClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.AudioClientSocket.CloseConnect();
                }

                if (this.FileClientSocket != null)
                {
                    this.FileClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.FileClientSocket.CloseConnect();
                }

                if (this.LyncClientSocket != null)
                {
                    this.LyncClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.LyncClientSocket.CloseConnect();
                }

                if (this.SpaceClientSocket != null)
                {
                    this.SpaceClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.SpaceClientSocket.CloseConnect();
                }

                if (this.MatrixClientSocket != null)
                {
                    this.MatrixClientSocket.TCPDataArrival -= clientSocket_TCPDataArrival;
                    this.MatrixClientSocket.CloseConnect();
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

        #region 接收到其他参会人所发送的数据（获取到服务器发送的数据）

        /// <summary>
        /// 接收到其他参会人所发送的数据
        /// </summary>
        /// <param name="args"></param>
        protected void clientSocket_TCPDataArrival(ConferenceWebCommon.Common.PackageBase args)
        {
            try
            {
                lock (obj1)
                {
                    switch (args.ConferenceClientAcceptType)
                    {
                        //研讨树
                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceTree:

                            this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       if (this.conferenceTreeView != null)
                       {
                           //知识树同步辅助
                           this.ConferenceTreeSyncHelper(args);
                       }
                   }));
                            break;

                        //语音研讨
                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceAudio:
                            //非同一线程，进行线程异步委托
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                if (this.conferenceAudio_View != null )
                                {
                                    //收到通知获当前同步取节点
                                    this.ConferenceAudio_View.Information_Sync(EntityTransfer.AudioTransferEntityChanged(args.ConferenceAudioItemTransferEntity));
                                }
                            }));
                            break;

                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceFileSync:
                            //甩屏同步辅助
                            this.ConferenceFileSyncHelper();

                            break;

                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceSpaceSync:

                            this.Dispatcher.BeginInvoke(new Action(() =>
               {

                   //智存空间同步辅助
                   this.ConferenceSpaceSyncHelper(args);
               }));

                            break;
                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceInfoSync:
                            //信息同步辅助
                            this.ConferenceInfoSyncHelper(args);

                            break;

                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.LyncConversationSync:
                            //lync同步辅助
                            this.LyncConversationSyncHelper(args);

                            break;

                        case ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceMatrixSync:
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
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

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
                                    this.AppSyncDataAcceptWindow.Closing += AppSyncDataAccept_Window_Closing;
                                    this.AppSyncDataAcceptWindow.Show();
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

        void ConferenceTreeSyncHelper(ConferenceWebCommon.Common.PackageBase args)
        {
            try
            {
                if (args.ConferenceTreeFlg != null)
                {

                    switch (args.ConferenceTreeFlg.ConferenceTreeFlgType)
                    {
                        case ConferenceWebCommon.EntityHelper.ConferenceTree.ConferenceTreeFlgType.normal:
                            //知识树子节点点对点进行同步
                            ConferenceTreeItem.Information_Sync(EntityTransfer.TreeTransferEntityChanged(args.ConferenceTreeFlg.ConferenceTreeItemTransferEntity));
                            break;
                        case ConferenceWebCommon.EntityHelper.ConferenceTree.ConferenceTreeFlgType.instead:
                            ConferenceWebCommon.EntityHelper.ConferenceTree.ConferenceTreeInsteadEntity insteadEntity = args.ConferenceTreeFlg.ConferenceTreeInsteadEntity;

                            this.ConferenceTreeView.InsteadElement(insteadEntity.BeforeItemGuid, insteadEntity.NowItemGuid);

                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 矩阵同步辅助
        /// </summary>
        void ConferenceMatrixSyncHelper(ConferenceWebCommon.Common.PackageBase args)
        {
            try
            {
                //矩阵切换
                if (args.ConferenceMatrixBase is ConferenceWebCommon.EntityHelper.ConferenceMatrix.ConferenceMatrixEntity)
                {
                    //类型还原
                    ConferenceWebCommon.EntityHelper.ConferenceMatrix.ConferenceMatrixEntity conferenceMatrixEntity = args.ConferenceMatrixBase as ConferenceWebCommon.EntityHelper.ConferenceMatrix.ConferenceMatrixEntity;
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
                else if (args.ConferenceMatrixBase is ConferenceWebCommon.EntityHelper.ConferenceMatrix.SeatEntity)
                {
                    //非同一线程，进行线程异步委托
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            //更改座位信息
                            //this.ChairView.UpdateOneSeat(args.ConferenceMatrixBase as ConferenceWebCommon.EntityHelper.ConferenceMatrix.SeatEntity);
                            //更改座位信息
                            this.MyConferenceView.UpdateOneSeat(args.ConferenceMatrixBase as ConferenceWebCommon.EntityHelper.ConferenceMatrix.SeatEntity);
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

        /// <summary>
        /// 智存空间同步辅助
        /// </summary>
        void ConferenceSpaceSyncHelper(ConferenceWebCommon.Common.PackageBase args)
        {

            try
            {

                //获取智存空间文件共享映射实体数据
                ConferenceWebCommon.EntityHelper.ConferenceOffice.ConferenceSpaceAsyncEntity conferenceOfficeAsyncEntity = args.ConferenceSpaceAsyncEntity;
                try
                {
                    if (ForceToNavicateEvent != null)
                    {
                        this.ForceToNavicateEvent(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                    }
                    //打开文件【word,excel,ppt】
                    this.ConversationM.FileOpenByExtension((FileType)conferenceOfficeAsyncEntity.FileType, conferenceOfficeAsyncEntity.Uri);
                    this.ConversationM.PageIndex = ResourceType.Normal;
                    this.ConversationM.ResourcePusher = conferenceOfficeAsyncEntity.Sharer;
                    this.ConversationM.ResourcePusherVisibility = System.Windows.Visibility.Visible;
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

        /// <summary>
        /// 信息同步辅助
        /// </summary>
        void ConferenceInfoSyncHelper(ConferenceWebCommon.Common.PackageBase args)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        switch (args.ConferenceInfoFlg.ConferenceInfoFlgType)
                        {
                            case ConferenceWebCommon.EntityHelper.ConferenceInfo.ConferenceInfoFlgType.ConferenceInfoEntity:
                                //获取信息同步映射实体数据
                                ConferenceWebCommon.EntityHelper.ConferenceInfo.ConferenceInfoEntity conferenceInfoEntity = args.ConferenceInfoFlg.ConferenceInfoEntity;

                                if (ForceToNavicateEvent != null)
                                {
                                    this.ForceToNavicateEvent((ConferenceCommon.EnumHelper.ViewSelectedItemEnum)conferenceInfoEntity.ConferencePageType);
                                }
                                break;
                            case ConferenceWebCommon.EntityHelper.ConferenceInfo.ConferenceInfoFlgType.ConferenceInfoTypeChangeEntity:

                                bool isSimpleModel = args.ConferenceInfoFlg.ConferenceInfoTypeChangeEntity.IsSimpleModel;
                                bool isEducationModel = args.ConferenceInfoFlg.ConferenceInfoTypeChangeEntity.IsEducationModel;

                                //教育模式
                                if (this.ViewModelChangedEducationEvent != null)
                                {
                                    this.ViewModelChangedEducationEvent(isEducationModel);
                                }
                                //精简模式
                                if (this.ViewModelChangedSimpleEvent != null)
                                {
                                    this.ViewModelChangedSimpleEvent(isSimpleModel);
                                }
                                if (TempConferenceInformationEntity != null)
                                {
                                    //当前临时会议信息更改（模式相关字段）
                                    TempConferenceInformationEntity.SimpleMode = isSimpleModel;
                                    TempConferenceInformationEntity.EducationMode = isEducationModel;
                                }

                                break;
                            case ConferenceWebCommon.EntityHelper.ConferenceInfo.ConferenceInfoFlgType.ConferenceClientControlEntity:
                                string sharer = args.ConferenceInfoFlg.ConferenceClientControlEntity.Sharer;
                                switch (args.ConferenceInfoFlg.ConferenceClientControlEntity.ClientControlType)
                                {
                                    case ConferenceWebCommon.EntityHelper.ConferenceInfo.ClientControlType.Close:
                                        if (sharer != Constant.SelfUri)
                                        {
                                            Application.Current.Shutdown(0);
                                        }
                                        break;
                                    case ConferenceWebCommon.EntityHelper.ConferenceInfo.ClientControlType.VersionUpdate:
                                        if (sharer != Constant.SelfUri)
                                        {
                                            VersionUpdateManage.JustUpdate(Constant.ConferenceVersionUpdateExe);
                                        }
                                        break;
                                    default:
                                        break;
                                }
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
        /// lync同步辅助
        /// </summary>
        /// <param name="args"></param>
        void LyncConversationSyncHelper(ConferenceWebCommon.Common.PackageBase args)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        switch (args.LyncConversationFlg.LyncConversationFlgType)
                        {
                            case ConferenceWebCommon.EntityHelper.LyncConversation.LyncConversationFlgType.InviteContact:
                                //获取信息同步映射实体数据
                                ConferenceWebCommon.EntityHelper.LyncConversation.LyncConversationEntity conferenceInfoEntity = args.LyncConversationFlg.LyncConversationEntity;
                                //邀请某人进入会话
                                //lyncEventManage.InviteSomeOneJoinMainConference(Constant.lyncClient, MainConversation, conferenceInfoEntity.JonConferencePerson);
                                break;
                            case ConferenceWebCommon.EntityHelper.LyncConversation.LyncConversationFlgType.EnterBigScreen:
                                ConferenceWebCommon.EntityHelper.LyncConversation.BigScreenEnterEntity bigScreenEnterEntity = args.LyncConversationFlg.BigScreenEnterEntity;

                                //更改状态
                                MainWindow.mainWindow.mainPage.SharingPanel.UpdateState(bigScreenEnterEntity.Sharer);
                                //显示投影人
                                MainWindow.MainPageInstance.GetPresenterCallBack(bigScreenEnterEntity.Sharer);

                                if (!bigScreenEnterEntity.Sharer.Equals(Constant.SelfName))
                                {

                                    //强制导航到资源共享
                                    MainWindow.MainPageInstance.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);

                                    this.ConversationM.PageIndex = ResourceType.Share;
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
                                break;

                            case ConferenceWebCommon.EntityHelper.LyncConversation.LyncConversationFlgType.PPTControl:

                                if (args.LyncConversationFlg.PPTControlEntity.Controler.Equals(Constant.LoginUserName))
                                {
                                    if (ViewSelectedItemEnum == ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource)
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

                                break;

                            #region old solution

                            //if (MainWindow.MainPageInstance.TempConferenceInformationEntity.RunCKOAPP)
                            //{
                            //    if (bigScreenEnterEntity.Sharer.Equals(Constant.SelfUri))
                            //    {
                            //        #region old solution

                            //        //主窗体最小化
                            //        MainWindow.mainWindow.WindowState = WindowState.Minimized;
                            //        Process[] processList = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(Constant.CkoAirFileName));

                            //        if (processList.Count() > 0)
                            //        {
                            //            System.Windows.Forms.SendKeys.SendWait("^+{f}");
                            //        }
                            //        else
                            //        {
                            //            this.CKOIsRuning = true;
                            //            ProcessManage.OpenFileByLocalAddress(Environment.CurrentDirectory + "\\" + Constant.CkoAirFileName);
                            //        }

                            //        #endregion

                            //        #region old solution

                            //        //if (!this.CKOIsRuning)
                            //        //{
                            //        //    this.CKOIsRuning = true;
                            //        //    ProcessManage.OpenFileByLocalAddress(Environment.CurrentDirectory + "\\" + Constant.CkoAirFileName);
                            //        //    //TimerJob.StartRun(new Action(() =>
                            //        //    //    {
                            //        //    //        Process[] processList = Process.GetProcessesByName("CkoAir");
                            //        //    //        if (processList.Count() > 0 && processList[0].MainWindowHandle.ToInt32() > 0)
                            //        //    //        {
                            //        //    //            //获取当前会话窗体的句柄
                            //        //    //            IntPtr Handle = processList[0].MainWindowHandle;

                            //        //    //            ////获取会话窗体的状态
                            //        //    //            //Win32API.GetWindowPlacement(Handle, ref this.Placement2);
                            //        //    //            ////最小话窗体
                            //        //    //            //this.Placement2.showCmd =2;
                            //        //    //            //Win32API.SetWindowPlacement(Handle, ref this.Placement2);

                            //        //    //            var dd = WindowHide.GetHandleList(Handle.ToInt32());
                            //        //    //            var dd2 = WindowHide.GetHandleList(processList[0].Handle.ToInt32());

                            //        //    //            CkoManageTimer.Stop();
                            //        //    //        }
                            //        //    //    }), 100, out CkoManageTimer);
                            //        //}
                            //        //else
                            //        //{
                            //        //    System.Windows.Forms.SendKeys.SendWait("^+{f}");
                            //        //}

                            //        #endregion
                            //    }
                            //    else
                            //    {
                            //        //TimerJob.StartRun(new Action(() =>
                            //        //   {
                            //        //System.Windows.Forms.SendKeys.SendWait("^+{s}");
                            //        //}));
                            //        //主窗体最小化
                            //        MainWindow.mainWindow.WindowState = WindowState.Maximized;
                            //    }
                            //}

                            #endregion


                            case ConferenceWebCommon.EntityHelper.LyncConversation.LyncConversationFlgType.LeaveConversation:
                                if (LyncHelper.MainConversation != null)
                                {
                                    ConferenceWebCommon.EntityHelper.LyncConversation.LeaveConversationEntity leaveConversationEntity = args.LyncConversationFlg.LeaveConversationEntity;
                                    string contactUri = leaveConversationEntity.ContactUri;

                                    foreach (var item in LyncHelper.MainConversation.Conversation.Participants)
                                    {
                                        if (item.Contact.Uri.Equals("sip:" + contactUri))
                                        {
                                            LyncHelper.MainConversation.Conversation.RemoveParticipant(item);
                                            break;
                                        }
                                    }
                                }
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

        #endregion

        #endregion

        #region 主持人结束甩屏窗体

        /// <summary>
        /// 主持人结束甩屏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppSyncDataAccept_Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                this.AppSyncDataAcceptWindow = null;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 服务器应用池持续性设置(old solution)

        ///// <summary>
        ///// 服务器应用池持续性设置
        ///// </summary>
        //protected void KeepWebServiceAppPoolAlive()
        //{
        //    try
        //    {
        //        //TimerJob.StartRun(new Action(() =>
        //        //{
        //        //    //保持服务器应用持续
        //        //    ModelManage.ConferenceInfo.SetKeepAlive();
        //        //}), 30000, out KeepWebSericeAppPoolAliveTimer);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        ///// <summary>
        ///// 停止保持服务器web服务持续激活
        ///// </summary>
        //protected void CloseWebServiceAppPoolAliveHelper()
        //{
        //    try
        //    {
        //        if (this.KeepWebSericeAppPoolAliveTimer != null)
        //        {
        //            //停止保持服务器web服务持续激活
        //            if (this.KeepWebSericeAppPoolAliveTimer.IsEnabled)
        //            {
        //                this.KeepWebSericeAppPoolAliveTimer.Stop();
        //                this.KeepWebSericeAppPoolAliveTimer = null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

        #region 主页UI卸载处理机制

        /// <summary>
        /// 主页UI卸载处理机制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MainPage_Unloaded()
        {
            try
            {

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
