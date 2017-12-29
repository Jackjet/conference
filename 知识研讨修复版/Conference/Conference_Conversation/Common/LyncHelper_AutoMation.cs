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
using uc = Microsoft.Office.Uc;

namespace Conference_Conversation.Common
{
    public partial class LyncHelper
    {
        #region lync会话事件注册

        /// <summary>
        /// lync会话事件注册
        /// </summary>      
        public static void LyncConversationEventRegedit()
        {
            try
            {
                //lync客户端对象模型不为null则注册相应事件
                if (ConversationCodeEnterEntity.lyncClient != null)
                {
                    //lync会话添加事件
                    ConversationCodeEnterEntity.conversationManager.ConversationAdded -= LyncHelper.ConversationManager_ConversationAdded;
                    //lync会话添加事件
                    ConversationCodeEnterEntity.conversationManager.ConversationAdded += LyncHelper.ConversationManager_ConversationAdded;

                    //lync会话移除事件
                    ConversationCodeEnterEntity.conversationManager.ConversationRemoved -= LyncHelper.ConversationManager_ConversationRemoved;
                    //lync会话移除事件
                    ConversationCodeEnterEntity.conversationManager.ConversationRemoved += LyncHelper.ConversationManager_ConversationRemoved;
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
                if (ConversationCodeEnterEntity.lyncClient != null && ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedIn && LyncHelper.MainConversation != null && !LyncHelper.MainConversation.IsDocked)
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
                                Thread.Sleep(500);
                                if (LyncHelper.DockNewWindowCallBack != null)
                                {
                                    LyncHelper.DockNewWindowCallBack();
                                }
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

        #region 清除不在线的参会人

        /// <summary>
        /// 清除不在线的参会人
        /// </summary>
        /// <param name="lyncClient"></param>
        /// <param name="conversationWindow"></param>
        public void RemoveNoAtLinePerson(ConversationWindow conversationWindow)
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

        #region 显示/隐藏内容

        /// <summary>
        /// 显示内容
        /// </summary>
        public static void ShowWindowContent()
        {
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                {
                    LyncHelper.MainConversation.ShowContent();
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
        /// 隐藏内容
        /// </summary>
        public static void HidenWindowContent()
        {
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                {
                    LyncHelper.MainConversation.HideContent();
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

        #region 移除会话参与人

        /// <summary>
        /// 移除会话参与人
        /// </summary>
        /// <param name="contactUri">联系人邮箱地址</param>
        public static void RemovePartical(string contactUri)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    foreach (var item in LyncHelper.MainConversation.Conversation.Participants)
                    {
                        if (item.Contact.Uri.Equals("sip:" + contactUri))
                        {
                            LyncHelper.MainConversation.Conversation.RemoveParticipant(item);
                            break;
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

        #endregion

        #region 关闭所有会话

        /// <summary>
        /// 关闭所有会话
        /// </summary>
        public static void CloseAllConversation(Action CallBack)
        {
            try
            {
                //离开会话
                if (LyncHelper.MainConversation != null)
                {
                    //关闭所有会话
                    //离开当前会话
                    //LyncHelper.MainConversation.Close();
                    LyncHelper.MainConversation.Close();
                    foreach (var conversation in ConversationCodeEnterEntity.conversationManager.Conversations)
                    {
                        ConversationWindow window = ConversationCodeEnterEntity.lyncAutomation.GetConversationWindow(conversation);
                        window.Close();
                    }

                    LyncHelper.MainConversation = null;
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


        #region 共享电子白板

        /// <summary>
        /// 共享电子白板
        /// </summary>
        public static void ShareWhiteboard(ConversationWindow conversationWindow, string selfName, Action callBack)
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
                            if (contentShareingModality.CanInvoke(ModalityAction.Connect))
                            {
                                contentShareingModality.BeginConnect(null, null);                             
                            }
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
                                        if (callBack != null)
                                        {
                                            callBack();
                                        }
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

        #region 启动音频

        public static void AvConnect(ConversationWindow conversationWindow)
        {
            try
            {
                
                AVModality avModality = (AVModality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo];
                if (avModality != null)
                {                    
                    if (avModality.CanInvoke(ModalityAction.Connect))
                    {
                        avModality.BeginConnect(null, null);
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
        /// 启动音频
        /// </summary>
        /// <param name="conversationWindow"></param>
        public static void StartAudio(ConversationWindow conversationWindow)
        {
            try
            {
                AVModality avModality = (AVModality)conversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo];
                if (avModality != null)
                {
                    AudioChannel audioChannel = avModality.AudioChannel;
                    if (audioChannel != null && audioChannel.CanInvoke(ChannelAction.Start))
                    {
                        audioChannel.BeginStart(null, null);
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

        #region 启动视频

        /// <summary>
        /// 启动视频
        /// </summary>
        /// <param name="conversationWindow"></param>
        public static void StartVideo(ConversationWindow conversationWindow)
        {
            try
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    AVModality avModality = (AVModality)conversationWindow.Conversation.SelfParticipant.Modalities[ModalityTypes.AudioVideo];
                    if (avModality != null)
                    {

                        VideoChannel videoChannel = avModality.VideoChannel;

                        //object obV = videoChannel.InnerObject;
                        if (videoChannel != null)
                        {

                            if (videoChannel != null && videoChannel.CanInvoke(ChannelAction.Start))
                            {
                                videoChannel.BeginStart(null, null);
                            }

                            TimerJob.StartRun(new Action(() =>
                            {
                                if (videoChannel.State != ChannelState.Connecting)
                                {
                                    if (videoChannel != null && videoChannel.CanInvoke(ChannelAction.Start))
                                    {
                                        videoChannel.BeginStart(null, null);
                                    }
                                }
                            }), 1500);
                            LyncHelper.ExitFullScreen();
                            LyncHelper.FullScreen();
                        }
                    }
                });
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

        #region 关闭音视频

        /// <summary>
        /// 关闭音视频
        /// </summary>
        public static void Close_AV(ConversationWindow conversationWindow)
        {
            try
            {
                AVModality avModality = (AVModality)conversationWindow.Conversation.SelfParticipant.Modalities[ModalityTypes.AudioVideo];
                if (avModality != null)
                {
                    AudioChannel audioChannel = avModality.AudioChannel;
                    if (audioChannel != null && audioChannel.CanInvoke(ChannelAction.Stop))
                    {
                        audioChannel.BeginStop(null, null);
                    }

                    VideoChannel videoChannel = avModality.VideoChannel;

                    if (videoChannel != null && videoChannel.CanInvoke(ChannelAction.Stop))
                    {
                        videoChannel.BeginStop(null, null);
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
                    ConversationCodeEnterEntity.lyncAutomation.BeginStartConversation(
                       AutomationModalities.InstantMessage,
                       null,
                        null,
                         (ar) =>
                         {
                             try
                             {
                                 #region (会话窗体设置,参会人同步【呼叫】)

                                 //获取主会话窗
                                 ConversationWindow window = ConversationCodeEnterEntity.lyncAutomation.EndStartConversation(ar);
                                 //设置会话窗体的事件
                                 SettingConversationWindowEvent(window);

                                 ///注册会话更改事件
                                 window.StateChanged -= MainConversation_StateChanged;
                                 ///注册会话更改事件
                                 window.StateChanged += MainConversation_StateChanged;

                                 //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                 //{
                                 //    ShareWhiteboard(window, selfName);
                                 //}));

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
                    Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(item);
                    //获取参会人状态
                    double s = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));
                    //状态3500,对方不在线(s == 3500 && )
                    if (contact != lyncClient.Self.Contact)
                    {
                        OnlineParticpantList.Add(item);
                    }
                }
                //Dictionary<AutomationModalitySettings, object> dic = new Dictionary<AutomationModalitySettings, object>();
                //dic.Add(AutomationModalitySettings.FirstInstantMessage, ConferenceName + mainConversationAccrodingStr);
                //dic.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);                


                ConversationCodeEnterEntity.lyncAutomation.BeginStartConversation(
                   automationModeality,
                    OnlineParticpantList,
                    null,
                     (ar) =>
                     {
                         try
                         {
                             #region (会话窗体设置,参会人同步【呼叫】)

                             //获取主会话窗
                             ConversationWindow conversationWindow = ConversationCodeEnterEntity.lyncAutomation.EndStartConversation(ar);
                             //设置会话窗体的事件
                             SettingConversationWindowEvent(conversationWindow);



                             ///注册会话更改事件
                             conversationWindow.StateChanged -= MainConversation_StateChanged;
                             ///注册会话更改事件
                             conversationWindow.StateChanged += MainConversation_StateChanged;
                             Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                             {
                                 ShareWhiteboard(conversationWindow, selfName, new Action(() =>
                                     {
                                         //LyncHelper.AvConnect(conversationWindow);

                                         //TimerJob.StartRun(new Action(() =>
                                         //    {
                                         //        Close_AV(conversationWindow);
                                         //    }), 800);
                                     }));
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
                        Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(JoinPerson);
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
                        Contact contact = ConversationCodeEnterEntity.contactManager.GetContactByUri(item);
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

        #region 获取共享的资源

        /// <summary>
        /// 获取共享的资源
        /// </summary>
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

        #region 桌面共享区域

        /// <summary>
        /// 桌面共享
        /// </summary>
        /// <returns></returns>
        public static bool ShareDesk()
        {
            bool canConnect = true;
            try
            {
                if (ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedIn)
                {
                    if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.State == ConversationWindowState.Initialized)
                    {

                        Modality application = LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing];
                        ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)application);
                        if (applicationSharingModality != null)
                        {
                            canConnect = applicationSharingModality.CanInvoke(ModalityAction.Connect);
                            if (canConnect)
                            {
                                if (applicationSharingModality != null)
                                {
                                    if (applicationSharingModality.CanShare(SharingResourceType.Desktop))
                                    {
                                        applicationSharingModality.BeginShareDesktop(null, null);
                                    }
                                }
                                if (ShareDeskCallBack != null)
                                {
                                    ShareDeskCallBack();
                                }
                            }
                            else
                            {
                                canConnect = false;
                            }
                        }
                    }
                    else
                    {
                        //MessageBox.Show("使用桌面共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

            return canConnect;
        }

        /// <summary>
        /// 关闭桌面共享
        /// </summary>
        /// <returns></returns>
        public static bool DisConnectDeskShare()
        {
            bool canDisConnect = true;
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    var application = LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing];
                    //((ApplicationSharingModality)LyncHelper.MainConversation.Conversation.SelfParticipant.Conversation.Modalities[ModalityTypes.ApplicationSharing]).BeginShareDesktop(null,null);
                    ApplicationSharingModality applicationSharingModality = ((ApplicationSharingModality)application);
                    if (applicationSharingModality != null)
                    {
                        canDisConnect = applicationSharingModality.CanInvoke(ModalityAction.Disconnect);
                        if (canDisConnect)
                        {
                            applicationSharingModality.BeginDisconnect(ModalityDisconnectReason.None, null, null);
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
            return canDisConnect;
        }

        #endregion

        #region PPt共享

        /// <summary>
        /// PPt共享
        /// </summary>
        /// <param name="fileName"></param>
        public static void PPtShareHelper(string fileName)
        {
            try
            {
                //封装的会话窗体内部实例
                uc.ConversationWindowClass conversationWindowClass = LyncHelper.MainConversation.InnerObject as uc.ConversationWindowClass;

                if (conversationWindowClass != null)
                {
                    //开启ppt
                    conversationWindowClass.GetConversationWindowActions().AddOfficePowerPointToConversation(fileName, ConversationCodeEnterEntity.LyncIP1 + ConversationCodeEnterEntity.ServicePPTTempFile + System.IO.Path.GetFileName(fileName));
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConversationM), ex);
            }
        }

        #endregion

        #region 获取会议地址

        /// <summary>
        /// 获取会议地址
        /// </summary>
        public static string GetConversation_Address()
        {
            string address = null;
            try
            {
                if (LyncHelper.MainConversation != null && LyncHelper.MainConversation.Conversation.Properties.ContainsKey(ConversationProperty.ConferenceAccessInformation))
                {
                    ConferenceAccessInformation conferenceAccessInformation = (ConferenceAccessInformation)LyncHelper.MainConversation.Conversation.Properties[ConversationProperty.ConferenceAccessInformation];
                    if (!string.IsNullOrEmpty(conferenceAccessInformation.InternalUrl))
                    {
                        address = conferenceAccessInformation.InternalUrl;
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
            return address;
        }

        #endregion

    }
}
