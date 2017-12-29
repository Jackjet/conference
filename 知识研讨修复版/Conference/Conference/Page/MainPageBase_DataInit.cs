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
using fileType = ConferenceWebCommon.EntityHelper.ConferenceOffice.FileType;
using wpfHelperFileType = ConferenceCommon.WPFControl.FileType;

namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 我的会议模块数据准备

        /// <summary>
        /// 我的会议数据初始化
        /// </summary>
        public void MyConferenceDataInitPrepare()
        {
            try
            {
                //MyConferenceCodeEnterEntity.ConferenceHost = Constant.ConferenceHost;
                //会议名称
                MyConferenceCodeEnterEntity.ConferenceName = Constant.ConferenceName;
                ////会议室名称
                //MyConferenceCodeEnterEntity.ConferenceRoomName = Constant.ConferenceRoomName;
                //当前机器的IP地址
                MyConferenceCodeEnterEntity.LocalIp = Constant.LocalIp;
                //当前用户名称
                MyConferenceCodeEnterEntity.SelfName = Constant.SelfName;
                //当前用户Url地址
                MyConferenceCodeEnterEntity.SelfUri = Constant.SelfUri;
                //智存空间登陆用户名
                MyConferenceCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
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
        /// 没有参会人回调事件
        /// </summary>
        private void NoParticalCallBack()
        {
            try
            {
                ////开启会话
                //ModelManage.ConferenceLyncConversation.ForceRemoveConversation(Constant.ConferenceName, new Action<bool>((issuccessed) =>
                //{
                //}));
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
        /// 填充数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dataGrid">视图控件</param>
        private void FillDataGridCallBack(DataGrid dataGrid)
        {
            try
            {
                //填充参会人列表
                LyncHelper.FillLyncOnlineInfo(dataGrid);
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
        /// 加载卡片
        /// </summary>
        private void AddCardEventCallBack()
        {
            try
            {
                //关闭所有会话
                LyncHelper.CloseAllConversation(new Action(() =>
                {
                    //离开会话
                    ModelManage.ConferenceLyncConversation.LeaveConversation(MyConferenceCodeEnterEntity.ConferenceName, MyConferenceCodeEnterEntity.SelfUri, new Action<bool>((isSuccessed) =>
                    {

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

        #region 知识树模块数据准备

        /// <summary>
        /// 知识树模块数据准备
        /// </summary>
        public void TreeDataInitPrepare()
        {
            try
            {
                //客户端上下文管理
                TreeCodeEnterEntity.ClientContextManage = Constant.clientContextManage;
                //会议纪要模板名称
                TreeCodeEnterEntity.ConferenceCommentHtmlTemp = Constant.ConferenceCommentHtmlTemp;
                //会议名称
                TreeCodeEnterEntity.ConferenceName = Constant.ConferenceName;
                //是否为会议主持人
                TreeCodeEnterEntity.IsMeetPresenter = Constant.IsMeetingPresenter;
                //PaintFileRoot路径
                TreeCodeEnterEntity.LocalTempRoot = Constant.LocalTempRoot;
                //用户登录名
                TreeCodeEnterEntity.LoginUserName = Constant.LoginUserName;
                //主窗体引用
                TreeCodeEnterEntity.MainWindow = MainWindow.mainWindow;
                //会议根目录
                TreeCodeEnterEntity.MeetingFolderName = Constant.MeetingFolderName;
                //PaintFileRoot路径
                TreeCodeEnterEntity.FileRoot = Constant.FileRoot;
                //参会人列表
                TreeCodeEnterEntity.ParticipantList = Constant.ParticipantList;
                //上传的录制视频名称
                TreeCodeEnterEntity.ReacordUploadFileName = Constant.ReacordUploadFileName;
                //录制视频格式
                TreeCodeEnterEntity.RecordExtention = Constant.RecordExtention;
                //录制文件夹名称
                TreeCodeEnterEntity.RecordFolderName = Constant.RecordFolderName;
                //用户名称
                TreeCodeEnterEntity.SelfName = Constant.SelfName;
                //当前用户的uri地址
                TreeCodeEnterEntity.SelfUri = Constant.SelfUri;
                //SharePoint服务IP地址
                TreeCodeEnterEntity.SPSiteAddressFront = Constant.SPSiteAddressFront;
                //临时存储的会议信息
                TreeCodeEnterEntity.TempConferenceInformationEntity = Constant.TempConferenceInformationEntity;
                //节点默认名称
                TreeCodeEnterEntity.TreeItemEmptyName = Constant.TreeItemEmptyName;
                //知识树xml文件名称
                TreeCodeEnterEntity.TreeXmlFileName = Constant.TreeXmlFileName;
                //域名
                TreeCodeEnterEntity.UserDomain = Constant.UserDomain;
                //投票标题集合
                TreeCodeEnterEntity.VoteChatTittleList = Constant.VoteChatTittleList;
                //智存空间登陆密码
                TreeCodeEnterEntity.WebLoginPassword = Constant.WebLoginPassword;
                //智存空间登陆用户名
                TreeCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
                //智存空间地址
                TreeCodeEnterEntity.SpaceWebSiteUri = Constant.SpaceWebSiteUri;
                //pdf转换器
                TreeCodeEnterEntity.PdfTransferAppName = Constant.PdfTransferAppName;
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
        /// 知识树回调中心
        /// </summary>
        public void CallBackEvent_Tree()
        {
            try
            {
                //普通文件共享
                this.ConferenceTreeView.FileShareCallBack = FileShareCallBack;
                //推送文件回调
                this.ConferenceTreeView.SendFileCallBack = SendFileCallBack;
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

        #region 智存空间数据准备

        /// <summary>
        /// 智存空间模块数据准备
        /// </summary>
        public void SPaceDataInitPrepare()
        {
            try
            {
                //客户端上下文管理
                SpaceCodeEnterEntity.ClientContextManage = Constant.clientContextManage;
                //会议名称
                SpaceCodeEnterEntity.ConferenceName = Constant.ConferenceName;
                //PaintFileRoot路径
                SpaceCodeEnterEntity.LocalTempRoot = Constant.LocalTempRoot;
                //用户登录名
                SpaceCodeEnterEntity.LoginUserName = Constant.LoginUserName;
                //会议根目录
                SpaceCodeEnterEntity.MeetingFolderName = Constant.MeetingFolderName;
                //用户名称
                SpaceCodeEnterEntity.SelfName = Constant.SelfName;
                //会议根目录
                SpaceCodeEnterEntity.SPSiteAddressFront = Constant.SPSiteAddressFront;
                //域名
                SpaceCodeEnterEntity.UserDomain = Constant.UserDomain;
                //智存空间登陆密码
                SpaceCodeEnterEntity.WebLoginPassword = Constant.WebLoginPassword;
                //智存空间登陆用户名
                SpaceCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
                //Tree服务IP
                SpaceCodeEnterEntity.TreeServiceIP = Constant.TreeServiceIP;
                //个人存储文件夹名称
                SpaceCodeEnterEntity.PesonalFolderName = Constant.PesonalFolderName;
                //服务区缓存文件夹
                SpaceCodeEnterEntity.ServicePPTTempFile = Constant.ServicePPTTempFile;
                //智存空间地址
                SpaceCodeEnterEntity.SpaceWebSiteUri = Constant.SpaceWebSiteUri;
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
        /// 智存空间回调中心
        /// </summary>
        public void CallBackEvent_Space()
        {
            try
            {


                //当前用户导航到指定页面
                this.SpaceView.PersonalSpace.ShareInConversationSelfNavicateCallBack = SpaceSelfNavicateCallBack;
                //文件共享
                this.SpaceView.PersonalSpace.FileShareCallBack = FileShareCallBack;
                //推送文件回调
                this.SpaceView.PersonalSpace.SendFileCallBack = SendFileCallBack;

                //当前用户导航到指定页面
                this.SpaceView.MeetingSpace.ShareInConversationSelfNavicateCallBack = SpaceSelfNavicateCallBack;
                //文件共享
                this.SpaceView.MeetingSpace.FileShareCallBack = FileShareCallBack;
                //推送文件回调
                this.SpaceView.MeetingSpace.SendFileCallBack = SendFileCallBack;
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
        /// 推送文件回调
        /// </summary>
        /// <param name="uri">推送文件uri地址</param>
        /// <param name="fileType">推送文件类型</param>
        private void SendFileCallBack(string uri, wpfHelperFileType fileType)
        {
            try
            {
                //填充word服务缓存数据
                ModelManage.ConferenceWordAsync.FillConferenceOfficeServiceData(Constant.ConferenceName, Constant.SelfName, uri, (ConferenceModel.ConferenceSpaceAsyncWebservice.FileType)fileType, new Action<bool>((isSuccessed) =>
                {
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
        /// 智存空间实施共享回调事件
        /// </summary>
        public void SpaceSelfNavicateCallBack(Action<bool> canShare)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //主会话不为null，则回调并通知相应模块改共享不能使用
                    if (LyncHelper.MainConversation != null)
                    {
                        //强制导航到资源共享
                        MainPage.mainPage.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                        MainPage.mainPage.ConversationM.PageIndex = ResourceType.Share;
                        canShare(true);
                    }
                    else
                    {
                        canShare(false);
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

        /// <summary>
        /// 智存空间实施共享回调事件
        /// </summary>
        public void SpaceOtherNavicateCallBack()
        {
            try
            {
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //同步页面
                    MainPage.mainPage.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
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
        /// 文件共享回调
        /// </summary>
        /// <param name="fileName">共享文件名称</param>
        private void FileShareCallBack(string fileName, wpfHelperFileType fileType)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                   {
                       if (fileType == wpfHelperFileType.pptx || fileType == wpfHelperFileType.ppt)
                       {
                           //指定ppt进行共享
                           LyncHelper.PPtShareHelper(fileName);
                       }
                       else
                       {
                           //打开本地文件
                           this.ConversationM.OpenLocalFileHelper(fileName);

                           //设置会话区域显示内容
                           this.ConversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);

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

        #region 通讯模块数据准备

        /// <summary>
        /// 通讯模块数据准备
        /// </summary>
        public void CommunicationDataInitPrepare()
        {
            try
            {
                //会议名称
                CommunicationCodeEnterEntity.ConferenceName = Constant.ConferenceName;
                //研讨同步服务目录（树）
                CommunicationCodeEnterEntity.ConferenceTreeServiceWebName = Constant.ConferenceTreeServiceWebName;
                //路由器ip
                CommunicationCodeEnterEntity.RouteIp = Constant.RouteIp;
                //当前用户的uri地址
                CommunicationCodeEnterEntity.SelfUri = Constant.SelfUri;
                // Tree服务web服务地址
                CommunicationCodeEnterEntity.TreeServiceAddressFront = Constant.TreeServiceAddressFront;
                //Tree服务IP
                CommunicationCodeEnterEntity.TreeServiceIP = Constant.TreeServiceIP;
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

        #region 共享协作数据准备

        /// <summary>
        /// 共享协作数数据准备
        /// </summary>
        public void ConversationDataInitPrepare()
        {
            try
            {
                //大屏幕名称
                ConversationCodeEnterEntity.BigScreenName = Constant.BigScreenName;
                //用户头像文件目录（）
                ConversationCodeEnterEntity.FtpServercePersonImgName = Constant.FtpServercePersonImgName;
                // PaintFileRoot路径
                ConversationCodeEnterEntity.LocalTempRoot = Constant.LocalTempRoot;
                // 登陆用户名（single_Left）
                ConversationCodeEnterEntity.LoginUserName = Constant.LoginUserName;
                //lync(automation)
                ConversationCodeEnterEntity.lyncAutomation = Constant.lyncAutomation;
                //lync1的IP
                ConversationCodeEnterEntity.LyncIP1 = Constant.LyncIP1;
                //参会人列表
                ConversationCodeEnterEntity.ParticipantList = Constant.ParticipantList;
                //服务区缓存文件夹
                ConversationCodeEnterEntity.ServicePPTTempFile = Constant.ServicePPTTempFile;
                //Tree服务web服务地址
                ConversationCodeEnterEntity.TreeServiceAddressFront = Constant.TreeServiceAddressFront;
                //域名
                ConversationCodeEnterEntity.UserDomain = Constant.UserDomain;
                //智存空间登陆密码
                ConversationCodeEnterEntity.WebLoginPassword = Constant.WebLoginPassword;
                //智存空间登陆用户名
                ConversationCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
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
        /// 共享协作回调中心
        /// </summary>
        public void CallBackEvent_Conversation()
        {
            try
            {
                //加载会话回调
                this.ConversationM.DockConversationWindowCallBack = DockConversationWindowCallBack;
                //共享页面同步回调
                this.ConversationM.ShareAndSyncCallBack = ShareAndSyncCallBack;
                //修复会话回调
                this.ConversationM.RepairConversationCallBack = RepairConversationCallBack;
                //lync会话事件注册
                LyncHelper.LyncConversationEventRegedit();
                //查看是否包含会议
                LyncHelper.HasConferenceCallBack = hasConferenceCallBack;
                //输出atuomation
                LyncHelper.MainConversationOutCallBack = mainConversationOutCallBack;
                //会话窗体加载内容完成事件
                LyncHelper.ConversationAddCompleateCallBack = conversationAddCompleateCallBack;
                //输入automation
                LyncHelper.MainConversationInCallBack = mainConversationInCallBack;
                //会话加载完成事件
                LyncHelper.ContentAddCompleateCallBack = contentAddCompleateCallBack;
                //会话移除事件
                LyncHelper.Content_DeskRemoveCompleateCallBack = Content_DeskRemoveCompleateCallBack;
                //返回演示人
                LyncHelper.PresentCallBack = presentCallBack;
                //共享窗体回调事件
                LyncHelper.ShareDeskCallBack = ShareDeskCallBack;
                //附加到新窗体回调
                LyncHelper.DockNewWindowCallBack = DockNewWindowCallBack;
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
        /// 修复会话
        /// </summary>
        private void RepairConversationCallBack()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.meetAddress))
                {
                    MainWindow.mainWindow.Topmost = true;
                    TimerJob.StartRun(new Action(() =>
                    {
                        //取消置顶
                        MainWindow.mainWindow.Topmost = false;
                    }), 2000);
                    LyncHelper.CloseAllConversation(new Action(() =>
                    {
                    }));

                    LyncHelper.JoinConversationByWebBrowser(this.meetAddress);
                    //释放dns（改为自由获取）,主窗体状态还原（非置顶）

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
        /// 附加到新窗体
        /// </summary>
        private void DockNewWindowCallBack()
        {
            try
            {
                //强制导航到资源共享
                MainPage.mainPage.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                //资源共享页面
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
        /// 共享桌面停止事件回调
        /// </summary>
        private void Content_DeskRemoveCompleateCallBack()
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

        /// <summary>
        /// 共享桌面回调
        /// </summary>
        private void ShareDeskCallBack()
        {
            try
            {
                //主窗体最小化
                MainWindow.mainWindow.WindowState = WindowState.Minimized;
                //投影大屏幕
                ModelManage.ConferenceLyncConversation.EnterBigScreen(Constant.ConferenceName, Constant.SelfName, new Action<bool>((result) =>
                {
                    if (result)
                    {
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

        /// <summary>
        /// 共享同步回调
        /// </summary>
        private void ShareAndSyncCallBack()
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //同步页面
                        MainPage.mainPage.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
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
        /// 会话嵌入回调
        /// </summary>
        /// <param name="CallBack">加载窗体指定获取尺寸回调</param>
        private void DockConversationWindowCallBack(Action<int, int> CallBack)
        {
            try
            {
                //获取工作区域的宽度
                int borWidth = MainPage.mainPage.GetWorkingArea_Width();
                //获取工作区域的高度
                int borHeight = MainPage.mainPage.GetWorkingArea_Height();

                CallBack(borWidth, borHeight);
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
        /// 判断是否有主会话
        /// </summary>
        /// <param name="conversationWindow"></param>
        /// <param name="conversationWindowCallBack">获取会话窗体回调</param>
        void mainConversationInCallBack(Action<ConversationWindow> conversationWindowCallBack)
        {
            try
            {
                //获取主会话窗体
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
        void conversationAddCompleateCallBack()
        {
            try
            {
                //取消置顶
                TimerJob.StartRun(new Action(() =>
                {
                    MainWindow.mainWindow.Topmost = false;
                }), 1000);

                //加载会话人员列表
                this.ConversationM.ParticalListInit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 加载内容完成事件
        /// </summary>
        /// <param name="title">加载共享内容标题</param>
        void contentAddCompleateCallBack(string title)
        {
            try
            {
                //已经处于资源共享页面则不进行闪烁
                if (MainPage.mainPage.ViewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource)
                {
                    //资源共享导航按钮闪烁
                    MainPage.mainPage.ResourceReceivMessageFlash();
                }
                //投影到大屏幕
                LyncHelper.InviteSomeOneJoinMainConference(Constant.lyncClient, LyncHelper.MainConversation, Constant.BigScreenName);                
                //资源共享页面
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
        /// <param name="mainConversationWindow">关联会话窗体回调</param>
        void mainConversationOutCallBack(ConversationWindow mainConversationWindow)
        {
            try
            {
                if (mainConversationWindow != null)
                {
                    //设置会话窗体的坐标位置和尺寸
                    this.ConversationM.DockConversationWindow(Constant.lyncClient, mainConversationWindow);
                }
                //设置主会话窗体
                LyncHelper.MainConversation = mainConversationWindow;
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
        /// <param name="hasConference">是否包含会议回调</param>
        void hasConferenceCallBack(Action<bool> hasConference)
        {
            try
            {
                //查看当前是否有会议
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

        /// <summary>
        /// 共享内容移除回调
        /// </summary>
        public void content_DeskRemoveCompleateCallBack()
        {
            try
            {
                ////更改状态
                //MainWindow.mainWindow.mainPage.SharingPanel.UpdateState(null);
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
        /// 显示演示者回调
        /// </summary>
        /// <param name="presenter">演示者</param>
        public void presentCallBack(string presenter)
        {
            try
            {
                //显示演示者
                this.ConversationM.ResourcePresenter = presenter;
                //演示者显示
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

        #endregion

        #region 网络浏览共享协作数据准备

        /// <summary>
        /// 浏览器数据初始化
        /// </summary>
        public void WebBrowserViewDataInitPrepare()
        {
            try
            {
                //uri列表存储地址
                WebBrowserCodeEnterEntity.urlStoreFileName = Constant.urlStoreFileName;
                //智存空间登陆用户名
                WebBrowserCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
                //智存空间登陆密码
                WebBrowserCodeEnterEntity.WebLoginPassword = Constant.WebLoginPassword;
                //网络浏览地址
                WebBrowserCodeEnterEntity.WebUri = Constant.WebUri;
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
        /// 浏览器共享回调
        /// </summary>
        /// <param name="uri">共享浏览器地址</param>
        private void ShareWebBrowserCallBack(Uri uri)
        {
            try
            {
                //共享网络地址
                this.ConversationM.ShareWebBrowser(uri);
                this.ConversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);
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

        #region 个人笔记数据准备

        /// <summary>
        /// 个人笔记数据准备
        /// </summary>
        public void NoteDataInitPrepare()
        {
            try
            {
                //SharePoint客户端对象模型管理
                NoteCodeEnterEntity.clientContextManage = Constant.clientContextManage;
                //本地个人笔记名称
                NoteCodeEnterEntity.LocalPersonalNoteFile = Constant.LocalPersonalNoteFile;
                //登陆用户名（single_Left）
                NoteCodeEnterEntity.LoginUserName = Constant.LoginUserName;
                //PaintFileRoot路径
                NoteCodeEnterEntity.FileRoot = Constant.FileRoot;
                //个人存储文件夹名称
                NoteCodeEnterEntity.PesonalFolderName = Constant.PesonalFolderName;
                //当前用户名称
                NoteCodeEnterEntity.SelfName = Constant.SelfName;
                //SharePoint服务IP地址
                NoteCodeEnterEntity.SPSiteAddressFront = Constant.SPSiteAddressFront;
                //智存空间登陆密码
                NoteCodeEnterEntity.WebLoginPassword = Constant.WebLoginPassword;
                //智存空间登陆用户名
                NoteCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
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

        #region 信息交流数据准备

        /// <summary>
        /// 信息交流数据准备
        /// </summary>
        public void IMMDataInitPrepare()
        {
            try
            {
                //语音文件类型
                IMMCodeEnterEntity.AudioFile_Extention = Constant.AudioFile_Extention;
                //语音文件名称
                IMMCodeEnterEntity.AudioFile_Name = Constant.AudioFile_Name;
                //语音文件目录
                IMMCodeEnterEntity.AudioFile_Root = Constant.AudioFile_Root;
                //研讨ftp服务地址
                IMMCodeEnterEntity.ConferenceFtpWebAddressFront = Constant.ConferenceFtpWebAddressFront;
                //当前参加的研讨会议名称
                IMMCodeEnterEntity.ConferenceName = Constant.ConferenceName;
                //网页客户端地址
                IMMCodeEnterEntity.ConferenceWebAppAddress = Constant.ConferenceWebAppAddress;
                //ftp用户密码
                IMMCodeEnterEntity.FtpPassword = Constant.FtpPassword;
                //ftp音频文件目录（）
                IMMCodeEnterEntity.FtpServerceAudioName = Constant.FtpServerceAudioName;
                //用户头像文件目录
                IMMCodeEnterEntity.FtpServercePersonImgName = Constant.FtpServercePersonImgName;
                //ftp用户
                IMMCodeEnterEntity.FtpUserName = Constant.FtpUserName;
                //是否为会议主持人
                IMMCodeEnterEntity.IsMeetingPresenter = Constant.IsMeetingPresenter;
                //登陆用户名（single_Left）
                IMMCodeEnterEntity.LoginUserName = Constant.LoginUserName;
                //当前用户名称
                IMMCodeEnterEntity.SelfName = Constant.SelfName;
                //Tree服务web服务地址
                IMMCodeEnterEntity.TreeServiceAddressFront = Constant.TreeServiceAddressFront;
                ////智存空间登陆用户名
                //IMMCodeEnterEntity.WebLoginUserName = Constant.WebLoginUserName;
                //pdf转换器
                IMMCodeEnterEntity.PdfTransferAppName = Constant.PdfTransferAppName;
                //pantroot文件目录
                IMMCodeEnterEntity.FileRoot = Constant.FileRoot;
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
        /// 信息交流收到信息回调
        /// </summary>
        private void IMMFalshCallBack()
        {
            try
            {
                if (MainPage.mainPage.ViewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.IMM)
                {
                    //消息闪烁
                    MainPage.mainPage.IMMReceivMessageFlash();
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
    }
}
