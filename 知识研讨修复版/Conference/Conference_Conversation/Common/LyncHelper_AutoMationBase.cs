using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Conference_Conversation.Common
{
    public partial class LyncHelper
    {
        #region 共享加载（ppt,电子白板）

        /// <summary>
        /// 共享加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationCard_ContentAdded(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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

                        if (AddContent_Type_CallBack != null)
                        {
                            if (e.Item.Type == ShareableContentType.PowerPoint)
                            {
                                AddContent_Type_CallBack(SharingType.ppt);
                            }
                            else
                            {
                                AddContent_Type_CallBack(SharingType.OherContent);
                            }
                        }

                        //属性更改事件(演示人)
                        e.Item.PropertyChanged -= Item_PropertyChanged;
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

        #endregion

        #region 会话共享移除（本地共享）

        /// <summary>
        /// 会话共享移除（本地共享）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void ConversationCard_ContentRemoved(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (ShareableContentList.Contains(e.Item))
                    {
                        ShareableContentList.Remove(e.Item);
                        if (RemoveContent_Type_CallBack != null)
                        {
                            RemoveContent_Type_CallBack(SharingType.ppt | SharingType.OherContent);
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

        #region modality通道响应

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

                                    Modality_Response_Accept(conversationWindow);

                                    break;

                                case ModalityAction.Connect:

                                    //Modality_Response_Connect(conversationWindow);


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
        /// 通道响应(accept)
        /// </summary>
        /// <param name="conversationWindow">会话窗体</param>
        private static void Modality_Response_Accept(ConversationWindow conversationWindow)
        {
            try
            {
                var modalities = conversationWindow.Conversation.Modalities;

                //视频通道
                VideoChannel videoChannel = null;

                //音频通道
                AudioChannel audioChannel = null;

                AVModality avModality = ((AVModality)modalities[ModalityTypes.AudioVideo]);
                if (avModality != null)
                {
                    //视频通道
                    videoChannel = avModality.VideoChannel;
                    //音频通道
                    audioChannel = avModality.AudioChannel;
                }

                //内容共享
                ContentSharingModality shareContent = (ContentSharingModality)modalities[ModalityTypes.ContentSharing];
                //程序共享
                ApplicationSharingModality applicationSharing = (ApplicationSharingModality)modalities[ModalityTypes.ApplicationSharing];


                //视频
                if (videoChannel != null && videoChannel.State == ChannelState.Receive)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (avModality.CanInvoke(ModalityAction.Accept))
                            {
                                //接受
                                avModality.Accept();

                                if (videoChannel.CanInvoke(ChannelAction.Start))
                                {
                                    videoChannel.BeginStart(null, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManage.WriteLog(typeof(LyncHelper), ex);
                        };
                    }));
                }
                //语音
                else if (audioChannel != null && audioChannel.State == ChannelState.Receive)
                {
                    if (avModality.CanInvoke(ModalityAction.Accept))
                    {
                        //接受
                        avModality.Accept();
                    }
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
                    if (AddContent_Type_CallBack != null)
                    {
                        AddContent_Type_CallBack(SharingType.Application);
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

        private static void Modality_Response_Connect(ConversationWindow conversationWindow)
        {
            try
            {
                var modalities = conversationWindow.Conversation.Modalities;

                //视频通道
                VideoChannel videoChannel = null;

                //音频通道
                AudioChannel audioChannel = null;

                AVModality avModality = ((AVModality)modalities[ModalityTypes.AudioVideo]);
                if (avModality != null)
                {
                    //视频通道
                    videoChannel = avModality.VideoChannel;
                    //音频通道
                    audioChannel = avModality.AudioChannel;
                }

                //视频
                if (videoChannel != null && videoChannel.State == ChannelState.Connecting)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (avModality.CanInvoke(ModalityAction.Accept))
                            {
                                //接受
                                avModality.Accept();

                                if (videoChannel.CanInvoke(ChannelAction.Start))
                                {
                                    videoChannel.BeginStart(null, null);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManage.WriteLog(typeof(LyncHelper), ex);
                        };
                    }));
                }
                //语音
                else if (audioChannel != null && audioChannel.State == ChannelState.Connecting)
                {

                    if (avModality.CanInvoke(ModalityAction.Accept))
                    {
                        //接受
                        avModality.Accept();
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

        #region 当前共享的程序更改事件

        /// <summary>
        ///当前共享的程序更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        #endregion

        #region 主窗体移除(相关)

        /// <summary>
        /// 主窗体移除(相关)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void applicationSharingModality_ParticipationStateChanged(object sender, ParticipationStateChangedEventArgs e)
        {
            try
            {
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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

        #endregion

        #region 共享控制（返回共享人信息）

        /// <summary>
        /// 共享控制（返回共享人信息）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void applicationSharingModality_ControllerChanged(object sender, ControllerChangedEventArgs e)
        {
            try
            {
                //使用异步委托
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    //返回共享人的名称
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

        #region 联系人添加

        /// <summary>
        /// 联系人添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Conversation_ParticipantRemoved(object sender, ParticipantCollectionChangedEventArgs e)
        {
            try
            {
                 //使用异步委托
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Remove_ConversationParticalCallBack != null)
                    {
                        Remove_ConversationParticalCallBack(e.Participant);
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

        #region 联系人移除

        /// <summary>
        /// 联系人移除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Conversation_ParticipantAdded(object sender, ParticipantCollectionChangedEventArgs e)
        {
            try
            {
                  //使用异步委托
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (Add_ConversationParticalCallBack != null)
                    {
                        Add_ConversationParticalCallBack(e.Participant);
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

        #region 演示人替换

        /// <summary>
        /// 演示人替换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Item_PropertyChanged(object sender, ShareableContentPropertyChangedEventArgs e)
        {
            try
            {
                if (e.Property == ShareableContentProperty.Presenter)
                {
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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

        #endregion

        #region 共享窗体信息更改

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

        #region 会话状态更改（Lync）

        /// <summary>
        /// 会议对话状态更改事件（Lync）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected static void MainConversation_StateChanged(object sender, ConversationWindowStateChangedEventArgs e)
        {

            //线程异步委托                
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
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

        #region 会话加载

        /// <summary>
        /// 会话加载
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
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    #region 响应会话

                    //获取发起人的会话模式
                    IDictionary<ModalityTypes, Modality> modalities = e.Conversation.Modalities;

                    //通知模式
                    //NotifyType notifyType = NotifyType.InstantMessage;

                    AVModality avModality = (AVModality)modalities[ModalityTypes.AudioVideo];

                    //视频通道
                    VideoChannel videoChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).VideoChannel;

                    //音频通道
                    AudioChannel audioChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).AudioChannel;
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
                    window = ConversationCodeEnterEntity.lyncAutomation.GetConversationWindow(e.Conversation);
                    window.NeedsSizeChange -= window_NeedsSizeChange;
                    window.NeedsSizeChange += window_NeedsSizeChange;
                    if (MainConversationOutCallBack != null)
                    {
                        MainConversationOutCallBack(window);
                    }

                    //设置会话窗体的事件
                    SettingConversationWindowEvent(window);

                    #endregion

                    #region 共享设置

                    window.StateChanged -= MainConversation_StateChanged;
                    //状态更改
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

        #endregion

        #region 会话卸载

        /// <summary>
        /// 会话卸载
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


        #region 会话容器尺寸调整

        /// <summary>
        /// 会话容器尺寸调整
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

        #endregion


        #region 设置会话窗体

        /// <summary>
        /// 设置会话窗体
        /// </summary>
        protected static void SettingConversationWindowEvent(ConversationWindow conversationWindow)
        {
            try
            {
                if (conversationWindow != null)
                {
                    if (conversationWindow.Conversation != null)
                    {
                        Conversation conversation = conversationWindow.Conversation;
                        IDictionary<ModalityTypes, Modality> dicModality = conversation.Modalities;
                        //会话模型
                        ContentSharingModality contentSharingModality = ((ContentSharingModality)dicModality[ModalityTypes.ContentSharing]);
                        Modality ContentModal = (dicModality[ModalityTypes.ContentSharing]);
                        ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)(dicModality[ModalityTypes.ApplicationSharing]));
                        Modality ApplicationModal = (dicModality[ModalityTypes.ApplicationSharing]);
                        AVModality AudioVideoModal = ((AVModality)dicModality[ModalityTypes.AudioVideo]);

                        //联系人添加事件
                        conversation.ParticipantAdded -= Conversation_ParticipantAdded;
                        //联系人添加事件
                        conversation.ParticipantAdded += Conversation_ParticipantAdded;
                        //联系人移除事件
                        conversation.ParticipantRemoved -= Conversation_ParticipantRemoved;
                        //联系人移除事件
                        conversation.ParticipantRemoved += Conversation_ParticipantRemoved;

                        if (contentSharingModality != null)
                        {
                            //电子白板、ppt共享内容添加事件
                            contentSharingModality.ContentAdded -= ConversationCard_ContentAdded;
                            //电子白板、ppt共享内容添加事件
                            contentSharingModality.ContentAdded += ConversationCard_ContentAdded;

                            //电子白板、ppt共享内容移除事件
                            contentSharingModality.ContentRemoved -= ConversationCard_ContentRemoved;
                            //电子白板、ppt共享内容移除事件
                            contentSharingModality.ContentRemoved += ConversationCard_ContentRemoved;
                        }
                        if (ContentModal != null)
                        {
                            //电子白板、ppt共享
                            ContentModal.ActionAvailabilityChanged -= ConversationCard_ActionAvailabilityChanged;
                            //电子白板、ppt共享
                            ContentModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;
                        }
                        if (ApplicationModal != null)
                        {
                            //应用程序共享
                            ApplicationModal.ActionAvailabilityChanged -= ConversationCard_ActionAvailabilityChanged;
                            //应用程序共享
                            ApplicationModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;

                            if (applicationSharingModality != null)
                            {
                                applicationSharingModality.LocalSharedResourcesChanged -= applicationSharingModality_LocalSharedResourcesChanged;
                                applicationSharingModality.LocalSharedResourcesChanged += applicationSharingModality_LocalSharedResourcesChanged;
                                applicationSharingModality.ControllerChanged -= applicationSharingModality_ControllerChanged;
                                applicationSharingModality.ControllerChanged += applicationSharingModality_ControllerChanged;

                                applicationSharingModality.ParticipationStateChanged -= applicationSharingModality_ParticipationStateChanged;
                                applicationSharingModality.ParticipationStateChanged += applicationSharingModality_ParticipationStateChanged;

                            }
                        }
                        //音视频
                        AudioVideoModal.ActionAvailabilityChanged -= ConversationCard_ActionAvailabilityChanged;
                        //音视频
                        AudioVideoModal.ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;


                        //实时获取会话窗体信息
                        conversationWindow.InformationChanged -= ConversationWindow_InformationChanged;
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


        #endregion

        #region 使用浏览器的方式参会

        /// <summary>
        /// 使用浏览器的方式参会
        /// </summary>
        /// <param name="meetAddress">会议地址</param>
        public static void JoinConversationByWebBrowser(string meet_Address)
        {
            try
            {
                //指定浏览器先释放资源
                if (conversationWebBrowser != null)
                {
                    conversationWebBrowser.Dispose();
                }
                conversationWebBrowser = new System.Windows.Forms.WebBrowser();
                //导航到指定地址
                conversationWebBrowser.Navigate(meet_Address);
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
