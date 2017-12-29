using Conference.TimerControl;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
//using Microsoft.Office.Uc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

namespace Conference.View.Resource
{
    /// <summary>
    /// 会话卡片
    /// </summary>
    public partial class ConversationCard : UserControl, INotifyPropertyChanged
    {
        #region 静态字段

        /// <summary>
        /// 绑定当前选择的实例
        /// </summary>
        public static ConversationCard currentConversationCard = null;

        #endregion

        #region 内部字段

        /// <summary>
        /// 参与人列表
        /// </summary>
        List<string> conversationCardPersonDisplayList = new List<string>();



        /// <summary>
        /// 会话窗体状态管理模型
        /// </summary>
        ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 共享白板的自定义名称
        /// </summary>
        string whiteBoardShareName = "共享白板";

        /// <summary>
        /// 共享白板数量(默认为0,进行计数)
        /// </summary>
        int whiteBoardShareCount = 0;

        #endregion

        #region 绑定属性

        string title;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (this.title != value)
                {
                    title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }

        #endregion

        #region 一般属性

        ConversationWindow conversationWindow;
        /// <summary>
        /// 关联的会话窗体（对应会话卡片）
        /// </summary>
        public ConversationWindow ConversationWindow
        {
            get { return conversationWindow; }
            set { conversationWindow = value; }
        }

        //bool isFullScreen = false;
        ///// <summary>
        ///// 是否为全屏(标示)
        ///// </summary>
        //public bool IsFullScreen
        //{
        //    get { return isFullScreen; }
        //    set { isFullScreen = value; }
        //}

        //bool isHidenContent;
        ///// <summary>
        ///// 当前会话是显示共享内容
        ///// </summary>
        //public bool IsHidenContent
        //{
        //    get { return isHidenContent; }
        //    set { isHidenContent = value; }
        //}

        bool isDisplayParticalControlPanel = false;
        /// <summary>
        /// 显示添加会话参与人管理面板
        /// </summary>
        public bool IsDisplayParticalControlPanel
        {
            get { return isDisplayParticalControlPanel; }
            set { isDisplayParticalControlPanel = value; }
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

        #endregion

        #region 实时更新

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region 构造函数

        public ConversationCard()
        {
            try
            {
                InitializeComponent();

                //绑定当前上下文
                this.DataContext = this;
                //点击选择自己填充静态实例
                this.ConversationCardFill();

                //设置会话窗体大小最合适
                //MainWindow.conversationM.SetttingConversationWindowParentSize_Fill();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 添加显示当前参与者

        /// <summary>
        /// 添加显示参与人集合
        /// </summary>
        public void ConversationCardPartical_AddRange(List<string> nameList)
        {
            try
            {
                foreach (var name in nameList)
                {
                    //生成一个文本(参与人显示)
                    TextBlock txt = new TextBlock() { Text = name };
                    //容器添加参与人
                    this.wraPanel.Children.Add(txt);
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 添加显示当前参与者
        /// </summary>
        public void ConversationCardPartical_Add(string name)
        {
            try
            {
                if (!this.conversationCardPersonDisplayList.Contains(name))
                {
                    //生成一个文本(参与人显示)
                    TextBlock txt = new TextBlock() { Text = name };
                    //容器添加参与人
                    this.wraPanel2.Children.Add(txt);
                    this.conversationCardPersonDisplayList.Add(name);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 删除显示当前参与者

        /// <summary>
        /// 删除显示参与人
        /// </summary>
        public void ConversationCardPartical_Remove(string name)
        {
            try
            {
                if (this.conversationCardPersonDisplayList.Contains(name))
                {
                    this.conversationCardPersonDisplayList.Remove(name);
                    //遍历容器子项集合
                    for (int i = this.wraPanel2.Children.Count - 1; i > -1; i--)
                    {
                        //获取子项
                        var item = this.wraPanel2.Children[i];
                        //判断子项类型
                        if (item is TextBlock)
                        {
                            //类型转换并作处理
                            var txt = item as TextBlock;
                            if (txt.Text.Equals(name))
                            {
                                //容器删除子项
                                this.wraPanel2.Children.Remove(txt);
                                break;
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 设置会话窗体的事件

        /// <summary>
        /// 设置会话窗体的事件
        /// </summary>
        public void SettingConversationWindowEvent()
        {
            try
            {
                if (this.ConversationWindow != null)
                {
                    //设置会话添加联系人事件
                    this.ConversationWindow.Conversation.ParticipantAdded += Conversation_ParticipantAdded;
                    //设置会话移除联系人事件
                    this.ConversationWindow.Conversation.ParticipantRemoved += Conversation_ParticipantRemoved;

                    //监控会话窗体的行为（启动共享白板，ppt，音视频）
                    this.ConversationWindow.ActionAvailabilityChanged += ConversationWindow_ActionAvailabilityChanged;

                    //电子白板、ppt共享
                    (this.ConversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing]).ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;

                    //电子白板、ppt共享内容添加事件
                    ((ContentSharingModality)(this.ConversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing])).ContentAdded += ConversationCard_ContentAdded;

                    //电子白板、ppt共享内容移除事件
                    ((ContentSharingModality)(this.ConversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing])).ContentRemoved += ConversationCard_ContentRemoved;                

                    //应用程序共享
                    (this.ConversationWindow.Conversation.Modalities[ModalityTypes.ApplicationSharing]).ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;

                    //音视频
                    ((AVModality)this.ConversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).ActionAvailabilityChanged += ConversationCard_ActionAvailabilityChanged;

                    //实时获取会话窗体信息
                    this.ConversationWindow.InformationChanged += ConversationWindow_InformationChanged;

                   
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
     
        /// <summary>
        /// 电子白板、ppt共享内容移除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_ContentRemoved(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                if (this.ShareableContentList.Contains(e.Item))
                {
                    this.ShareableContentList.Remove(e.Item);

                    //if (MainWindow.conversationResourceWindow != null)
                    //{
                    //    //跨线程（使用异步委托）
                    //    this.Dispatcher.BeginInvoke(new Action(() =>
                    //        {
                    //            try
                    //            {
                    //                //资源呈现刷新
                    //                MainWindow.conversationResourceWindow.RefleshViewByMesuare();
                    //            }
                    //            catch (Exception ex)
                    //            {
                    //                LogManage.WriteLog(this.GetType(), ex);
                    //            };
                    //        }));
                    //}
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 电子白板、ppt共享内容添加事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_ContentAdded(object sender, ContentCollectionChangedEventArgs e)
        {
            try
            {
                if (!this.ShareableContentList.Contains(e.Item))
                {
                    this.ShareableContentList.Add(e.Item);
                    //跨线程（使用异步委托）
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            //if (MainWindow.conversationResourceWindow != null)
                            //{
                            //    //资源呈现刷新
                            //    MainWindow.conversationResourceWindow.RefleshViewByMesuare();
                            //}

                            ////隐藏标题
                            //MainWindow.mainWindow.UnDisplayHeader();

                            ////强制导航到会话管理界面
                            //MainWindow.mainWindow.ForceToNavicate(ConferenceNavicateType.ConvsersationManage);
                        }
                        catch (Exception ex)
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

        /// <summary>
        ///实时获取会话窗体信息 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationWindow_InformationChanged(object sender, ConversationWindowInformationChangedEventArgs e)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
             {
                 ////最小宽度
                 //var widthMin = 0;
                 ////最小高度
                 //var heightMin = 0;

                 ////获取最小宽度和最小高度
                 //foreach (var item in e.ChangedProperties)
                 //{
                 //    if (item.Key.ToString().Equals("WidthMin"))
                 //    {
                 //        widthMin = Convert.ToInt32(item.Value);
                 //    }
                 //    if (item.Key.ToString().Equals("HeightMin"))
                 //    {
                 //        heightMin = Convert.ToInt32(item.Value);
                 //    }
                 //}
                 //if (widthMin > 0 && heightMin > 0)
                 //{
                 //    var column3Width = MainWindow.conversationM.GetColumnWidth();
                 //    var dd = MainWindow.conversationM.ActualHeight;
                 //    if (heightMin > 600)
                 //    {
                 //        if (!MainWindow.conversationM.IsColumn3WidthMax)
                 //        {
                 //            MainWindow.conversationM.PanelVisibilityChangedToMax(ConversationCard.conversationCard);
                 //            MainWindow.conversationM.IsColumn3WidthMax = true;
                 //        }
                 //    }
                 //    //else if (heightMin.Equals(600))
                 //    //{
                 //    //    this.IsDisplayParticalControlPanel = true;
                 //    //    //MainWindow.conversationM.SettingConversationWindowParentSize_Height(620);
                 //    //}
                 //    //else if (heightMin < dd && widthMin < column3Width)
                 //    //{
                 //    //    MainWindow.conversationM.SettingConversationWindowParentSize_Width((int)(MainWindow.conversationM.ActualWidth - 20 - 168 - 25 - 300));
                 //    //    MainWindow.conversationM.SettingConversationWindowParentSize_Height(547);
                 //    //}
                 //}
             }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 电子白板、ppt共享【音视频、应用程序共享】
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_ActionAvailabilityChanged(object sender, ModalityActionAvailabilityChangedEventArgs e)
        {
            switch (e.Action)
            {
                case ModalityAction.Accept:

                    var modalities = this.ConversationWindow.Conversation.Modalities;

                    //视频通道
                    var videoChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).VideoChannel;
                    //音频通道
                    var audioChannel = ((AVModality)modalities[ModalityTypes.AudioVideo]).AudioChannel;
                    //内容共享
                    var shareContent = (ContentSharingModality)modalities[ModalityTypes.ContentSharing];
                    //程序共享
                    var applicationSharing = (ApplicationSharingModality)modalities[ModalityTypes.ApplicationSharing];

                    //视频
                    if (videoChannel.State == ChannelState.Notified)
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      try
                      {
                          //接受
                          ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                      }
                      catch (Exception ex)
                      {
                          LogManage.WriteLog(this.GetType(), ex);
                      };
                  }));
                    }
                    //语音
                    else if (audioChannel.State == ChannelState.Notified)
                    {
                        ((AVModality)modalities[ModalityTypes.AudioVideo]).Accept();
                    }
                    //共享ppt、电子白板
                    else if (shareContent.State == ModalityState.Notified)
                    {
                        shareContent.Accept();

                    }
                    //共享应用程序
                    else if (applicationSharing.State == ModalityState.Notified)
                    {
                        applicationSharing.Accept();
                    }

                    break;

                case ModalityAction.ConsultAndTransfer:
                    this.Dispatcher.BeginInvoke(new Action(() =>
           {

               //if (MainWindow.conversationM != null && ConversationCard.conversationCard != null)
               //{
               //    //隐藏左侧两列
               //    MainWindow.conversationM.PanelVisibilityChangedToMax(ConversationCard.conversationCard);
               //}
           }));
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// 监控会话窗体的行为（启动共享白板，ppt，音视频）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationWindow_ActionAvailabilityChanged(object sender, ConversationWindowActionAvailabilityChangedEventArgs e)
        {
            try
            {
                //是否可以显示主窗体中的会话全屏按钮
                if (e.IsAvailable)
                {
                    //显示
                    //MainWindow.mainWindow.ConversationFullScreenVisibility = System.Windows.Visibility.Visible;
                    //if (e.Action == ConversationWindowAction.FullScreen)
                    //{
                    //    //标示为全屏
                    //    this.IsFullScreen = true;
                    //}
                }
                else
                {
                    //隐藏
                    //MainWindow.mainWindow.ConversationFullScreenVisibility = System.Windows.Visibility.Collapsed;
                    //if (e.Action == ConversationWindowAction.FullScreen)
                    //{
                    //标示
                    //this.IsFullScreen = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 会话添加联系人事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Conversation_ParticipantAdded(object sender, Microsoft.Lync.Model.Conversation.ParticipantCollectionChangedEventArgs e)
        {
            try
            {
                //获取uri(处理)
                string uri = e.Participant.Contact.Uri.Replace("sip:", string.Empty); ;
                if (Constant.DicParticipant.ContainsKey(uri))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                 {
                     try
                     {
                         //添加参与人名称列表
                         this.ConversationCardPartical_Add(Constant.DicParticipant[uri]);
                     }
                     catch (Exception ex)
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

        /// <summary>
        /// 会话移除联系人事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Conversation_ParticipantRemoved(object sender, Microsoft.Lync.Model.Conversation.ParticipantCollectionChangedEventArgs e)
        {
            try
            {
                //获取uri(处理)
                string uri = e.Participant.Contact.Uri.Replace("sip:", string.Empty); ;
                if (Constant.DicParticipant.ContainsKey(uri))
                {
                    this.Dispatcher.BeginInvoke(new Action(() =>
                {
                     try
            {
                    //添加参与人名称列表
                    this.ConversationCardPartical_Remove(Constant.DicParticipant[uri]);
                          }
                                catch (Exception ex)
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

        #region 注册事件

        /// <summary>
        /// 注册事件
        /// </summary>
        public void RouteEventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.PreviewMouseLeftButtonDown += ConversationCard_PreviewMouseLeftButtonDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        public void TouchEventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.TouchDown += ConversationCard_TouchDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 注销事件

        /// <summary>
        /// 注销事件
        /// </summary>
        public void DeleteRouteEventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.PreviewMouseLeftButtonDown -= ConversationCard_PreviewMouseLeftButtonDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 注销事件
        /// </summary>
        public void DeleteTouchEventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.TouchDown -= ConversationCard_TouchDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 点击选择自己填充静态实例

        /// <summary>
        /// 点击选择自己填充静态实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ConversationCardFill();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 点击选择自己填充静态实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.ConversationCardFill();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 点击选择自己填充静态实例
        /// </summary>
        public void ConversationCardFill()
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    //将之前的会话卡片恢复为默认的背景色
                    ConversationCard.currentConversationCard.bor.Background = new SolidColorBrush(Colors.Transparent);
                    //将之前的会话卡片恢复为默认的背景色
                    ConversationCard.currentConversationCard.bor2.Background = new SolidColorBrush(Colors.Transparent);
                }
                //点击选择自己填充静态实例
                ConversationCard.currentConversationCard = this;
                //将选择的会话卡片设置新的背景
                this.bor2.Background = this.bor.Background = new SolidColorBrush(Colors.SkyBlue) { Opacity = 0.3 };


                //foreach (var item in ConversationM.ConversationManageWindow.DicCardAndConversationWidow.Keys)
                //{
                //    if (item.Equals(this.ConversationWindow))
                //    {
                //        //Win32API.ShowWindow(item.Handle, Win32API.SW_SHOW);
                //        //获取会话窗体的状态
                //        Win32API.GetWindowPlacement(item.Handle, ref this.Placement);
                //        //最小话窗体
                //        this.Placement.showCmd = 1;
                //        Win32API.SetWindowPlacement(item.Handle, ref this.Placement);
                //    }
                //    else
                //    {
                //        Win32API.ShowWindow(item.Handle, Win32API.SW_HIDE);
                //    }
                //}

                //if (this.ConversationWindow != null)
                //{
                //    //获取会话窗体的状态
                //    Win32API.SetForegroundWindow(this.ConversationWindow.Handle);
                //}
                //恢复默认会话全屏样式
                //MainWindow.mainWindow.RefleshTheConversationFullScreenStyle();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 自定义共享白板的名称

        /// <summary>
        /// 自定义共享白板的名称
        /// </summary>
        public string GetWhiteShareName()
        {
            string whiteShareName = string.Empty;
            try
            {
                this.whiteBoardShareCount++;
                whiteShareName = this.whiteBoardShareName + this.whiteBoardShareCount;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            return whiteShareName;
        }

        #endregion
    }
}
