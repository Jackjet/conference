using Conference.Page;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.ImageHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
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
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ConferenceCommon.LyncHelper
{
    public partial class LyncHelper
    {
        #region lync事件（状态更改、会话、联系人、信息）【提供全局使用】


        /// <summary>
        /// 联系人加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ConversationManager_ConversationAdded(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {

            //查看是否已进入一个会议【倘若未进入会议,则直接进行拒绝,该逻辑有待斟酌】
            bool canOpenTheConversation = true;

            if (HasConferenceCallBack != null)
            {

                HasConferenceCallBack(new Action<bool>((hasConference) =>
                {
                    //if (hasConference)
                    //{
                    //    canOpenTheConversation = false;
                    //}
                }));
            }


            //子线程不可直接调用主线程的UI（需要通过异步委托的机制去执行）
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    #region 响应会话

                    //获取发起人的会话模式
                    var modalities = e.Conversation.Modalities;

                    //通知模式
                    //NotifyType notifyType = NotifyType.InstantMessage;

                    //视频通道
                    var videoChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).VideoChannel;
                    //音频通道
                    var audioChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).AudioChannel;
                    //IM类型
                    var instantMessage = modalities[ModalityTypes.InstantMessage];
                    //查看当前视频会话具体应用
                    if (videoChannel.State == ChannelState.Notified)
                    {
                        if (canOpenTheConversation)
                        {
                            ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                            //notifyType = NotifyType.Video;

                            TimerJob.StartRun(new Action(() =>
                            {
                                //判断是否可以执行该操作
                                if (videoChannel.CanInvoke(ChannelAction.Start))
                                {
                                    ////接受请求,开启摄像头
                                    videoChannel.BeginStart(null, null);
                                    //停止计时器
                                    timerAcept.Stop();
                                }

                            }), 500, out  timerAcept);
                        }
                        else
                        {
                            //拒绝音频会话
                            ((AVModality)modalities[ModalityTypes.AudioVideo]).Reject(ModalityDisconnectReason.NotAcceptableHere);
                            return;
                        }
                    }
                    //查看当前音频会话具体应用
                    else if (audioChannel.State == ChannelState.Notified)
                    {
                        if (canOpenTheConversation)
                        {
                            //接受音频会话
                            ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                            //notifyType = NotifyType.Autio;
                        }
                        else
                        {
                            //拒绝音频会话
                            ((AVModality)modalities[ModalityTypes.AudioVideo]).Reject(ModalityDisconnectReason.NotAcceptableHere);
                        }
                    }
                    //IMM文本会话,查看是否为通知状态（避免与语音、视频通话相互之间发生冲突）
                    if (instantMessage.State == ModalityState.Notified || instantMessage.State == ModalityState.Connected)
                    {
                        if (canOpenTheConversation)
                        {
                            ((InstantMessageModality)modalities[ModalityTypes.InstantMessage]).Accept();

                            #region old solution

                            //if (e.Conversation.Participants.Count <= 2)
                            //{
                            //    //模仿鼠标点击
                            //    Win32API.SetCursorPos(System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - 100, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - 160);
                            //    Win32API.mouse_event(Win32API.MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
                            //    Win32API.mouse_event(Win32API.MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);
                            //}

                            #endregion
                        }
                        else
                        {
                            ((InstantMessageModality)modalities[ModalityTypes.InstantMessage]).Reject(ModalityDisconnectReason.NotAcceptableHere);
                        }
                    }

                    #endregion

                    #region 设置窗体位置和尺寸

                    //获取会话窗体
                    ConversationWindow window = null;

                    //获取会话窗体
                    window = Constant.lyncAutomation.GetConversationWindow(e.Conversation);
                    window.NeedsSizeChange += window_NeedsSizeChange;
                    if (MainConversationOutCallBack != null)
                    {
                        MainConversationOutCallBack(window);
                    }

                    //设置会话窗体的事件
                    SettingConversationWindowEvent(window);

                    #endregion

                    #region 共享设置

                    //string uri = item.Contact.Uri.Replace("sip:", string.Empty); 

                    window.StateChanged += MainConversation_StateChanged;

                    //为共享做准备
                    TimerJob.StartRun(new Action(() =>
                    {
                        //连接共享
                        var modaly = ((ContentSharingModality)e.Conversation.SelfParticipant.Conversation.Modalities[ModalityTypes.ContentSharing]);

                        if (modaly.CanInvoke(ModalityAction.Accept))
                        {
                            modaly.Accept();
                        }

                        if (modaly.CanInvoke(ModalityAction.Connect))
                        {
                            modaly.BeginConnect(null, null);
                        }

                    }));

                    #endregion

                    #region 会话加载完成事件激活

                    if (ConversationAddCompleateCallBack != null)
                    {
                        ConversationAddCompleateCallBack();
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(typeof(LyncHelper), ex);
                }
            }));
        }

        /// <summary>
        /// 会话容器尺寸调整事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void window_NeedsSizeChange(object sender, ConversationWindowNeedsSizeChangeEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 会话卸载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void ConversationManager_ConversationRemoved(object sender, Microsoft.Lync.Model.Conversation.ConversationManagerEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region Lync会话控制

        #region 启动会议

        /// <summary>
        /// 启动会议
        /// </summary>
        public static void StartConference(LyncClient lyncClient, string conferenceName, string selfName, List<string> participantList, ConversationWindow conversationWindow)
        {
            try
            {
                if (lyncClient != null && lyncClient.State == ClientState.SignedIn)
                {
                    //参会人列表有参会人（在正常会议下可进行lync会话）
                    if (participantList.Count > 0)
                    {

                        if (conversationWindow == null)
                        {
                            //会议开启通用方法
                            StartConference_H(lyncClient, conferenceName, selfName, MainConversationAccrodingStr, participantList, AutomationModalities.InstantMessage);
                        }
                    }
                    else
                    {
                        MessageBox.Show("请先进入一个会议", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 启动会议
        /// </summary>
        public void StartConferenceOnlyc(LyncClient lyncClient, string selfUri, ConversationWindow conversationWindow, string selfName)
        {
            try
            {
                try
                {
                    Dictionary<AutomationModalitySettings, object> dic = new Dictionary<AutomationModalitySettings, object>();
                    dic.Add(AutomationModalitySettings.FirstInstantMessage, selfName + MainConversationAccrodingStr);
                    dic.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);
                    Constant.lyncAutomation.BeginStartConversation(
                       AutomationModalities.InstantMessage,
                       null,
                        null,
                         (ar) =>
                         {
                             try
                             {
                                 #region (会话窗体设置,参会人同步【呼叫】)

                                 //获取主会话窗
                                 ConversationWindow window = Constant.lyncAutomation.EndStartConversation(ar);
                                 //设置会话窗体的事件
                                 SettingConversationWindowEvent(window);
                                 ///注册会话更改事件
                                 window.StateChanged += MainConversation_StateChanged;



                                 App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                 {
                                     //        TimerJob.StartRun(new Action(() =>
                                     //            {
                                     //                //启动邀请文件
                                     //                ConversationM.PPtShareHelper(Constant.PaintFileRoot + "\\" + Constant.InviteFile);
                                     //            }), 1500);
                                     ShareWhiteboard(window, selfName);

                                 }));

                                 #endregion
                             }
                             catch (OperationException ex)
                             {
                                 LogManage.WriteLog(typeof(LyncHelper), ex);
                             };
                         },
                         null);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(typeof(LyncHelper), ex);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 启动会议音频

        /// <summary>
        /// 启动音频会议
        /// </summary>
        protected void StartConference_Audio(LyncClient lyncClient, string conferenceName, string selfName, List<string> participantList, ConversationWindow conversationWindow)
        {
            try
            {
                //参会人列表有参会人（在正常会议下可进行lync会话）
                if (participantList.Count > 0)
                {
                    //lync签入状态
                    if (lyncClient != null && lyncClient.State == ClientState.SignedIn)
                    {
                        //主会话窗存在，则直接使用
                        if (conversationWindow == null)
                        {
                            //启动视频会议
                            StartConference_H(lyncClient, conferenceName, selfName, MainConversationAccrodingStr, participantList, AutomationModalities.Audio);
                        }
                        else
                        {
                            //若会话音视频已连接,则无需再连接
                            if (((Modality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).State != ModalityState.Connected)
                            {
                                //给这些参与者发送指定信息
                                ((Modality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).BeginConnect(null, null);
                            }
                            else
                            {

                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("进入会话之前请先确保进入一个会议", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 开启视频会议

        /// <summary>
        /// 开启视频会议
        /// </summary>
        protected void StartConference_Video(LyncClient lyncClient, string ConferenceName, string selfName, List<string> participantList, ConversationWindow conversationWindow)
        {
            try
            {
                //参会人列表有参会人（在正常会议下可进行lync会话）
                if (participantList.Count > 0)
                {
                    //lync签入状态
                    if (lyncClient != null && lyncClient.State == ClientState.SignedIn)
                    {
                        //主会话窗存在，则直接使用
                        if (conversationWindow == null)
                        {
                            StartConference_H(lyncClient, ConferenceName, selfName, MainConversationAccrodingStr, participantList, AutomationModalities.Video);
                        }
                        else
                        {
                            //若会话音视频已连接,则无需再连接
                            if (((AVModality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).VideoChannel.State != ChannelState.SendReceive)
                            {
                                //给这些参与者发送指定信息
                                ((AVModality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).VideoChannel.BeginStart(null, null);
                            }
                            else
                            {
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("进入会话之前请先确保进入一个会议", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 会话状态更改事件（Lync）

        /// <summary>
        /// 会议对话状态更改事件（Lync）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void MainConversation_StateChanged(object sender, ConversationWindowStateChangedEventArgs e)
        {

            //线程异步委托                
            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //主会话被销毁
                    if (e.OldState == ConversationWindowState.Initialized && e.NewState == ConversationWindowState.Destroyed)
                    {
                        if (MainConversationOutCallBack != null)
                        {
                            MainConversationOutCallBack(null);
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

        #region 会议开启通用方法(启动会议、同步添加参会人)

        /// <summary>
        /// 会议开启通用方法
        /// </summary>
        /// <param name="participantList">参会人</param>
        /// <param name="automationModeality">会话类型</param>
        protected static void StartConference_H(LyncClient lyncClient, string ConferenceName, string selfName, string mainConversationAccrodingStr, List<string> participantList, AutomationModalities automationModeality)
        {
            try
            {
                //在线联系人列表
                List<string> OnlineParticpantList = new List<string>();
                //遍历所有参会人并筛选在线参会人
                foreach (var item in participantList)
                {
                    //获取联系人
                    Contact contact = lyncClient.ContactManager.GetContactByUri(item);
                    //获取参会人状态
                    double s = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));
                    //状态3500,对方不在线(s == 3500 && )
                    if (contact != lyncClient.Self.Contact)
                    {
                        OnlineParticpantList.Add(item);
                    }
                }
                Dictionary<AutomationModalitySettings, object> dic = new Dictionary<AutomationModalitySettings, object>();
                dic.Add(AutomationModalitySettings.FirstInstantMessage, ConferenceName + mainConversationAccrodingStr);
                dic.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);
                Constant.lyncAutomation.BeginStartConversation(
                   automationModeality,
                    OnlineParticpantList,
                    null,
                     (ar) =>
                     {
                         try
                         {
                             #region (会话窗体设置,参会人同步【呼叫】)

                             //获取主会话窗
                             ConversationWindow conversationWindow = Constant.lyncAutomation.EndStartConversation(ar);
                             //设置会话窗体的事件
                             SettingConversationWindowEvent(conversationWindow);
                             ///注册会话更改事件
                             conversationWindow.StateChanged += MainConversation_StateChanged;



                             App.Current.Dispatcher.BeginInvoke(new Action(() =>
                             {
                                 //        TimerJob.StartRun(new Action(() =>
                                 //            {
                                 //                //启动邀请文件
                                 //                ConversationM.PPtShareHelper(Constant.PaintFileRoot + "\\" + Constant.InviteFile);
                                 //            }), 1500);
                                 ShareWhiteboard(conversationWindow, selfName);

                             }));

                             #endregion
                         }
                         catch (OperationException ex)
                         {
                             LogManage.WriteLog(typeof(LyncHelper), ex);
                         };
                     },
                     null);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region lync会话资源启动区域

        #region 共享电子白板

        /// <summary>
        /// 共享电子白板
        /// </summary>
        public static void ShareWhiteboard(ConversationWindow conversationWindow, string selfName)
        {
            try
            {
                if (conversationWindow != null)
                {
                    DispatcherTimer timer1 = null;
                    TimerJob.StartRun(new Action(() =>
                    {
                        if (conversationWindow.Conversation == null)
                        {
                            timer1.Stop();
                            return;
                        }

                        //获取电子白板启动模型实例
                        ContentSharingModality contentShareingModality = (ContentSharingModality)conversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing];
                        DispatcherTimer timer2 = null;

                        //查看是否可以共享白板
                        var canShareWhiteboard = contentShareingModality.CanInvoke(ModalityAction.CreateShareableWhiteboardContent);
                        if (canShareWhiteboard)
                        {

                            //获取共享白板的名称                                               
                            string whiteShareName = GetWhiteShareName(selfName, whiteBoardShareName);

                            if (contentShareingModality.CanInvoke(ModalityAction.CreateShareableWhiteboardContent))
                            {
                                //创建一个默认的电子白板
                                IAsyncResult result = contentShareingModality.BeginCreateContent(ShareableContentType.Whiteboard, whiteShareName, null, null);
                                //结束提交
                                ShareableContent whiteBoardContent = contentShareingModality.EndCreateContent(result);

                                TimerJob.StartRun(new Action(() =>
                                {
                                    int reason; //【在此可监控异常】
                                    if (whiteBoardContent.CanInvoke(ShareableContentAction.Upload, out reason))
                                    {
                                        //电子白板上传
                                        whiteBoardContent.Upload();
                                    }
                                    if (whiteBoardContent.State == ShareableContentState.Online)
                                    {
                                        timer2.Stop();

                                        //当前呈现（电子白板）
                                        whiteBoardContent.Present();
                                    }
                                }), 10, out timer2);
                                timer1.Stop();
                            }
                        }
                    }), 400, out timer1);
                }
                else
                {
                    MessageBox.Show("使用电子白板共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 自定义共享白板的名称
        /// </summary>
        protected static string GetWhiteShareName(string selfName, string whiteBoardShareName)
        {
            string whiteShareName = string.Empty;
            try
            {
                whiteBoardShareCount++;
                whiteShareName = selfName + whiteBoardShareName + whiteBoardShareCount;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            return whiteShareName;
        }

        #endregion

        #endregion

        #region 设置会话窗体的事件

        //public static ApplicationSharingModality ApplicationSharingModality = null;

        /// <summary>
        /// 设置会话窗体的事件
        /// </summary>
        protected static void SettingConversationWindowEvent(ConversationWindow conversationWindow)
        {
            try
            {
                if (conversationWindow != null)
                {
                    //监控会话窗体的行为（启动共享白板，ppt，音视频）
                    //conversationWindow.ActionAvailabilityChanged += ConversationWindow_ActionAvailabilityChanged;

                    if (conversationWindow.Conversation != null)
                    {
                        //会话模型
                        ContentSharingModality contentSharingModality = ((ContentSharingModality)(conversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing]));
                        var ContentModal = (conversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing]);
                        ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)(conversationWindow.Conversation.Modalities[ModalityTypes.ApplicationSharing]));
                        var ApplicationModal = (conversationWindow.Conversation.Modalities[ModalityTypes.ApplicationSharing]);
                        var AudioVideoModal = ((AVModality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]);


                        if (contentSharingModality != null)
                        {
                            //电子白板、ppt共享内容添加事件
                            contentSharingModality.ContentAdded += ConversationCard_ContentAdded;

                            //电子白板、ppt共享内容移除事件
                            contentSharingModality.ContentRemoved += ConversationCard_ContentRemoved;
                        }
                        if (ContentModal != null)
                        {
                            //电子白板、ppt共享
                            ContentModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;
                        }
                        if (ApplicationModal != null)
                        {
                            //应用程序共享
                            ApplicationModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;
                            //ApplicationModal.ModalityStateChanged += ApplicationModal_ModalityStateChanged;
                            if (applicationSharingModality != null)
                            {
                                //applicationSharingModality.View.DisplayMode = ApplicationSharingViewDisplayMode.FitToParent;
                                applicationSharingModality.LocalSharedResourcesChanged += applicationSharingModality_LocalSharedResourcesChanged;
                                //applicationSharingModality.SharerChanged += applicationSharingModality_SharerChanged;
                                applicationSharingModality.ControllerChanged += applicationSharingModality_ControllerChanged;
                                //applicationSharingModality.ControlRequestReceived += applicationSharingModality_ControlRequestReceived;
                                //applicationSharingModality.ApplicationSharingModalityPropertyChanged += applicationSharingModality_ApplicationSharingModalityPropertyChanged;
                                //applicationSharingModality.ModalityStateChanged += applicationSharingModality_ModalityStateChanged;
                                applicationSharingModality.ParticipationStateChanged += applicationSharingModality_ParticipationStateChanged;

                            }
                        }
                        //音视频
                        AudioVideoModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;

                        //实时获取会话窗体信息
                        conversationWindow.InformationChanged += ConversationWindow_InformationChanged;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #region new solution

        static void applicationSharingModality_LocalSharedResourcesChanged(object sender, LocalSharedResourcesChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_ModalityStateChanged(object sender, ModalityStateChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_SharerChanged(object sender, SharerChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_ParticipationStateChanged(object sender, ParticipationStateChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (e.OldState == ParticipationState.Viewing && e.NewState == ParticipationState.None)
                        {
                            if (Content_DeskRemoveCompleateCallBack != null)
                            {
                                Content_DeskRemoveCompleateCallBack();
                            }
                        }
                    }));

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_ApplicationSharingModalityPropertyChanged(object sender, ModalityPropertyChangedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_ControlRequestReceived(object sender, ControlRequestReceivedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        static void applicationSharingModality_ControllerChanged(object sender, ControllerChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                       {
                           if (PresentCallBack != null)
                           {
                               PresentCallBack(e.ControllerName);
                           }
                       }));
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

        /// <summary>
        /// 监控会话窗体的行为（启动共享白板，ppt，音视频）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationWindow_ActionAvailabilityChanged(object sender, ConversationWindowActionAvailabilityChangedEventArgs e)
        {
            try
            {
                //是否可以显示主窗体中的会话全屏按钮
                if (e.IsAvailable)
                {
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 电子白板、ppt共享内容移除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationCard_ContentRemoved(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (ShareableContentList.Contains(e.Item))
                    {
                        ShareableContentList.Remove(e.Item);
                    }

                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        /// <summary>
        /// 电子白板、ppt共享内容添加事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationCard_ContentAdded(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {

                    try
                    {
                        if (!ShareableContentList.Contains(e.Item))
                        {
                            //加载共享资源
                            ShareableContentList.Add(e.Item);

                        }

                        if (ContentAddCompleateCallBack != null)
                        {
                            ContentAddCompleateCallBack(e.Item.Title);
                        }
                        //属性更改事件(演示人)
                        e.Item.PropertyChanged += Item_PropertyChanged;

                        if (PresentCallBack != null)
                        {
                            if (e.Item.Owner != null)
                            {
                                object displayName = e.Item.Owner.GetContactInformation(ContactInformationType.DisplayName);
                                if (displayName != null)
                                {
                                    PresentCallBack(Convert.ToString(displayName));
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
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        static void Item_PropertyChanged(object sender, ShareableContentPropertyChangedEventArgs e)
        {
            try
            {
                if (e.Property == ShareableContentProperty.Presenter)
                {
                    App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (PresentCallBack != null)
                    {
                        if (e.Value is Participant)
                        {
                            Participant participant = e.Value as Participant;
                            object displayName = participant.Contact.GetContactInformation(ContactInformationType.DisplayName);
                            if (displayName != null)
                            {
                                PresentCallBack(Convert.ToString(displayName));
                            }
                        }

                    }
                }));
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
        /// 电子白板、ppt共享【音视频、应用程序共享】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationCard_ActionAvailabilityChanged(object sender, ModalityActionAvailabilityChangedEventArgs e)
        {
            try
            {
                if (MainConversationInCallBack != null)
                {
                    MainConversationInCallBack(new Action<ConversationWindow>((conversationWindow) =>
                    {
                        if (conversationWindow != null)
                        {
                            switch (e.Action)
                            {
                                case ModalityAction.Accept:

                                    var modalities = conversationWindow.Conversation.Modalities;

                                    //视频通道
                                    VideoChannel videoChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).VideoChannel;
                                    //音频通道
                                    AudioChannel audioChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).AudioChannel;
                                    //内容共享
                                    ContentSharingModality shareContent = (ContentSharingModality)modalities[ModalityTypes.ContentSharing];
                                    //程序共享
                                    ApplicationSharingModality applicationSharing = (ApplicationSharingModality)modalities[ModalityTypes.ApplicationSharing];


                                    //视频
                                    if (videoChannel != null && videoChannel.State == ChannelState.Notified)
                                    {
                                        App.Current.Dispatcher.BeginInvoke(new Action(() =>
                                        {
                                            try
                                            {
                                                //接受
                                                ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                                            }
                                            catch (Exception ex)
                                            {
                                                LogManage.WriteLog(typeof(LyncHelper), ex);
                                            };
                                        }));
                                    }
                                    //语音
                                    else if (audioChannel != null && audioChannel.State == ChannelState.Notified)
                                    {
                                        ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                                    }
                                    //共享ppt、电子白板
                                    else if (shareContent != null && shareContent.State == ModalityState.Notified)
                                    {
                                        shareContent.Accept();
                                    }
                                    //共享应用程序
                                    else if (applicationSharing != null && applicationSharing.State == ModalityState.Notified)
                                    {
                                        applicationSharing.Accept();
                                    }

                                    break;



                                default:
                                    break;
                            }
                        }
                    }));
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
        ///实时获取会话窗体信息 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationWindow_InformationChanged(object sender, ConversationWindowInformationChangedEventArgs e)
        {
            try
            {
                //this.Dispatcher.BeginInvoke(new Action(() =>
                //{
                //}));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 获取共享的资源

        public void GetResourceList()
        {
            try
            {

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

        #endregion

        #region 联系人同步（研讨客户端与lync同步）【呼叫参会人】

        /// <summary>
        /// 邀请某人进入会话
        /// </summary>
        public static void InviteSomeOneJoinMainConference(LyncClient lyncClient, ConversationWindow conversationWindow, string JoinPerson)
        {
            try
            {
                if (conversationWindow != null)
                {
                    if (lyncClient.State == ClientState.SignedIn)
                    {
                        //获取参会人
                        Contact contact = lyncClient.ContactManager.GetContactByUri(JoinPerson);
                        //判断是否已加载
                        if (conversationWindow.Conversation.Participants.Where(Item => Item.Contact.Uri.Equals(contact.Uri)).Count() <= 0)
                        {
                            //获取参会人状态
                            double s = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));
                            //状态3500,对方在线
                            if (s == 3500 && conversationWindow.Conversation.CanInvoke(ConversationAction.AddParticipant))
                            {
                                //邀请参会人
                                conversationWindow.Conversation.AddParticipant(contact);
                            }
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
        }

        /// <summary>
        ///  联系人同步（研讨客户端与lync同步）
        /// </summary>
        protected static void ParticalsSynchronous(LyncClient lyncClient, ConversationWindow conversationWindow, List<string> participantList)
        {
            try
            {
                if (conversationWindow != null && conversationWindow.Conversation != null)
                {
                    List<string> tempList = new List<string>();
                    //重新填充主会话联系人列表（实际）
                    foreach (var partical in conversationWindow.Conversation.Participants)
                    {
                        tempList.Add(partical.Contact.Uri);
                    }
                    //遍历所有参会人
                    foreach (var item in participantList)
                    {
                        //获取参会人
                        Contact contact = lyncClient.ContactManager.GetContactByUri(item);
                        //获取参会人状态
                        double s = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));
                        //状态3500,对方不在线
                        if (s == 3500)
                        {
                            //呼叫在线参会人加入会话（添加参会人）
                            if (conversationWindow.Conversation.CanInvoke(ConversationAction.AddParticipant))
                            {
                                //添加除自己之外的参会人
                                if (!contact.Equals(lyncClient.Self.Contact))
                                {
                                    //对应实际参会人列表
                                    if (!tempList.Contains(contact.Uri))
                                    {
                                        //主会议关闭状态停止进行呼叫并中断
                                        if (conversationWindow == null)
                                        {
                                            return;
                                        }
                                        //添加在线的参会人
                                        conversationWindow.Conversation.AddParticipant(contact);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion

        #region 清除不在线的参会人

        public void RemoveNoAtLinePerson(LyncClient lyncClient, ConversationWindow conversationWindow)
        {
            try
            {

                if (conversationWindow != null && conversationWindow.Conversation != null)
                {
                    for (int i = conversationWindow.Conversation.Participants.Count - 1; i > -1; i--)
                    {
                        //获取参会人状态
                        double s = Convert.ToDouble(conversationWindow.Conversation.Participants[i].Contact.GetContactInformation(ContactInformationType.Availability));


                        //状态3500,对方在线
                        if (s == 18500 && conversationWindow.Conversation.CanInvoke(ConversationAction.AddParticipant))
                        {
                            //邀请参会人
                            conversationWindow.Conversation.RemoveParticipant(conversationWindow.Conversation.Participants[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
        }

        #endregion
    }
}
