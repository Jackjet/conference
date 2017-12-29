using Conference.TimerControl;
using ConferenceCommon.LogHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Conference.View.Resource
{
    /// <summary>
    /// ConversationManageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationManageWindow : Window, INotifyPropertyChanged
    {

        #region 内部字段

        /// <summary>
        /// 联系人卡片集合
        /// </summary>
        List<ContactCustomCard> listContactCustomCard = new List<ContactCustomCard>();

        ///// <summary>
        ///// 会话窗体状态管理模型
        ///// </summary>
        //ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 小组会话邀请信息
        /// </summary>
        string conversationImFirstInfo = "_启动小组会话";

        #endregion

        #region 一般属性

        Dictionary<ConversationWindow, ConversationCard> dicCardAndConversationWidow = new Dictionary<ConversationWindow, ConversationCard>();
        /// <summary>
        /// 会话卡片对应会话窗体的字典集合
        /// </summary>
        public Dictionary<ConversationWindow, ConversationCard> DicCardAndConversationWidow
        {
            get { return dicCardAndConversationWidow; }
            set { dicCardAndConversationWidow = value; }
        }


        //List<string> conversationCard_list = new List<string>();
        ///// <summary>
        ///// 小组参会人员名称表单
        ///// </summary>
        //public List<string> ConversationCard_list
        //{
        //    get { return conversationCard_list; }
        //    set { conversationCard_list = value; }
        //}

        #endregion

        #region 绑定属性

        Visibility startConferenceVisibility = Visibility.Collapsed;
        /// <summary>
        /// 开启会话操作提示
        /// </summary>
        public Visibility StartConferenceVisibility
        {
            get { return startConferenceVisibility; }
            set
            {
                if (value != this.startConferenceVisibility)
                {
                    startConferenceVisibility = value;
                    this.OnPropertyChanged("StartConferenceVisibility");
                }
            }
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

        public ConversationManageWindow()
        {
            try
            {
                InitializeComponent();

                #region 客户端操作模式

                this.RouteEventRegedit();

                #endregion

                //设置当前上下文
                this.DataContext = this;

                //鼠标移出事件
                this.MouseLeave += ConversationManageWindow_MouseLeave;
                ////鼠标进入事件
                //this.btnManage.MouseEnter += ConversationManageWindow_MouseEnter;


                //悬浮窗体展开/闭合
                this.SetHidenContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #region 填充联系人列表

        /// <summary>
        /// 填充联系人列表
        /// </summary>
        public void ContactListFill()
        {
            try
            {
                //清除之前的参会人列表
                this.stackDiscussParticList.Children.Clear();
                //清除联系人卡片列表
                this.listContactCustomCard.Clear();
                //添加当前的参会人
                foreach (var item in Constant.ParticipantList)
                {
                    //筛选
                    if (item.Equals(Constant.SelfUri)) continue;
                    ContactCustomCard contactItem = new ContactCustomCard()
                    {
                        //设置卡片位置
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    };
                    //添加联系人卡片
                    this.listContactCustomCard.Add(contactItem);

                    //设置联系人卡片的源
                    contactItem.Source = item;
                    //容器加载
                    this.stackDiscussParticList.Children.Add(contactItem);
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
        /// 事件注册
        /// </summary>
        public void RouteEventRegedit()
        {
            try
            {
                //启动小组会话
                this.btnStartGourpConversation.Click += btnStartGourpConversation_Click;

                //开启音频
                this.btnAudio.Click += btnAudio_Click;
                //开启视频
                this.btnVideo.Click += btnVideo_Click;
                //会话最大化
                this.btnMax.Click += btnMax_Click;
                //会话最小化
                this.btnMin.Click += btnMin_Click;
                //会话全屏
                this.btnFullScreen.Click += btnFullScreen_Click;
                //会话还原
                this.btnReduction.Click += btnReduction_Click;
                //会话关闭
                this.btnClose.Click += btnClose_Click;
                //悬浮球点击事件
                this.btnManage.MouseLeftButtonDown += btnManage_MouseLeftButtonDown;
                //强制同步
                this.btnAsyn.Click += btnAsyn_Click;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }     

        /// <summary>
        /// 事件注册
        /// </summary>
        public void TouchEventRegedit()
        {
            try
            {
                //启动小组会话
                this.btnStartGourpConversation.TouchDown += btnStartGourpConversation_TouchDown;

                //开启音频
                this.btnAudio.TouchDown += btnAudio_TouchDown;
                //开启视频
                this.btnVideo.TouchDown += btnVideo_TouchDown;
                //会话最大化
                this.btnMax.TouchDown += btnMax_TouchDown;
                //会话最小化
                this.btnMin.TouchDown += btnMin_TouchDown;
                //会话全屏
                this.btnFullScreen.TouchDown += btnFullScreen_TouchDown;
                //会话还原
                this.btnReduction.TouchDown += btnReduction_TouchDown;
                //会话关闭
                this.btnClose.TouchDown += btnClose_TouchDown;
                //悬浮窗体展开/闭合
                this.btnManage.TouchDown += ellipse_TouchDown;
                //强制同步
                this.btnAsyn.TouchDown += btnAsyn_TouchDown;
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
                //启动小组会话
                this.btnStartGourpConversation.Click -= btnStartGourpConversation_Click;

                //开启音频
                this.btnAudio.Click -= btnAudio_Click;
                //开启视频
                this.btnVideo.Click -= btnVideo_Click;
                //会话最大化
                this.btnMax.Click -= btnMax_Click;
                //会话最小化
                this.btnMin.Click -= btnMin_Click;
                //会话全屏
                this.btnFullScreen.Click -= btnFullScreen_Click;
                //会话还原
                this.btnReduction.Click -= btnReduction_Click;
                //悬浮球点击事件
                this.btnManage.MouseLeftButtonDown -= btnManage_MouseLeftButtonDown;
                //强制同步
                 this.btnAsyn.Click += btnAsyn_Click;
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
                //启动小组会话
                this.btnStartGourpConversation.TouchDown -= btnStartGourpConversation_TouchDown;

                //开启音频
                this.btnAudio.TouchDown -= btnAudio_TouchDown;
                //开启视频
                this.btnVideo.TouchDown -= btnVideo_TouchDown;
                //会话最大化
                this.btnMax.TouchDown -= btnMax_TouchDown;
                //会话最小化
                this.btnMin.TouchDown -= btnMin_TouchDown;
                //会话全屏
                this.btnFullScreen.TouchDown -= btnFullScreen_TouchDown;
                //会话还原
                this.btnReduction.TouchDown -= btnReduction_TouchDown;
                //悬浮窗体展开/闭合
                this.btnManage.TouchDown -= ellipse_TouchDown;
                //强制同步
                this.btnAsyn.TouchDown -= btnAsyn_TouchDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 事件集切换

        /// <summary>
        /// 切换为PAD模式
        /// </summary>
        public void ChangedToPAD()
        {
            try
            {
                //注销主窗体路由事件集
                this.DeleteRouteEventRegedit();
                //注销主窗体触摸事件集
                this.DeleteTouchEventRegedit();
                //添加主窗体触摸事件集
                this.TouchEventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 切换为PC模式
        /// </summary>
        public void ChangedToPC()
        {
            try
            {
                //注销主窗体路由事件集
                this.DeleteRouteEventRegedit();
                //注销主窗体触摸事件集
                this.DeleteTouchEventRegedit();
                //添加主窗体路由事件集
                this.RouteEventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 启动小组会话

        /// <summary>
        /// 启动小组会话(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnStartGourpConversation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //启动小组会话
                this.StartGourpConversation();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 启动小组会话(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnStartGourpConversation_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                //启动小组会话
                this.StartGourpConversation();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 启动小组会话
        /// </summary>
        public void StartGourpConversation()
        {
            try
            {
                if (ContactCustomCard.ContactCustomCardSourceList.Count > 0)
                {
                    //创建小组会话
                    this.StartConference_H(ContactCustomCard.ContactCustomCardSourceList, AutomationModalities.InstantMessage);
                }
                else
                {
                    this.StartConferenceVisibility = System.Windows.Visibility.Visible;

                    TimerJob.StartRun(new Action(() =>
                    {
                        this.StartConferenceVisibility = System.Windows.Visibility.Collapsed;
                    }), 1500);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 会话开启通用方法

        /// <summary>
        /// 会话开启通用方法
        /// </summary>
        /// <param name="participantList">参会人</param>
        /// <param name="automationModeality">会话类型</param>
        void StartConference_H(List<string> participantList, AutomationModalities automationModeality)
        {
            try
            {
                Dictionary<AutomationModalitySettings, object> dic = new Dictionary<AutomationModalitySettings, object>();
                dic.Add(AutomationModalitySettings.FirstInstantMessage, Constant.ConferenceName + this.conversationImFirstInfo);
                dic.Add(AutomationModalitySettings.SendFirstInstantMessageImmediately, true);

                LyncClient.GetAutomation().BeginStartConversation(
                   automationModeality,
                    participantList,
                    dic,
                     (ar) =>
                     {
                         try
                         {
                             ////获取主会话窗
                             //ConversationWindow window = LyncClient.GetAutomation().EndStartConversation(ar);
                             /////注册会话更改事件
                             //window.StateChanged += window_StateChanged;
                         }
                         catch (OperationException oe)
                         {
                             System.Windows.MessageBox.Show("开启会话出现异常" + oe.Message);
                         };
                     },
                     null);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 添加会话卡片

        /// <summary>
        /// 添加会话卡片
        /// </summary>
        public void ConversationCardItemAdd(ConversationCard conversationCard)
        {
            try
            {
                //添加会话卡片
                if (!stackPanel2.Children.Contains(conversationCard))
                {
                    this.stackPanel2.Children.Add(conversationCard);
                }

                //会话卡片对应会话窗体的字典集合添加子项
                this.DicCardAndConversationWidow.Add(conversationCard.ConversationWindow, conversationCard);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 删除会话卡片

        /// <summary>
        /// 删除会话卡片
        /// </summary>
        /// <param name="conversationCard"></param>
        public void ConversationCardItemRemove(ConversationCard conversationCard)
        {
            try
            {
                //删除会话卡片
                if (stackPanel2.Children.Contains(conversationCard))
                {
                    this.stackPanel2.Children.Remove(conversationCard);
                }

                //会话卡片对应会话窗体的字典集合删除子项
                if (this.DicCardAndConversationWidow.ContainsKey(conversationCard.ConversationWindow))
                {
                    this.DicCardAndConversationWidow.Remove(conversationCard.ConversationWindow);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 开启音频

        /// <summary>
        /// 开启音频(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StartAudio();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 开启音频(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAudio_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.StartAudio();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 开启音频
        /// </summary>
        public void StartAudio()
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    //((AVModality)(ConversationCard.conversationCard.ConversationWindow.Conversation.Modalities[Microsoft.Lync.Model.Conversation.ModalityTypes.AudioVideo])).BeginConnect(null, null);

                    (ConversationCard.currentConversationCard.ConversationWindow.Conversation.Modalities[ModalityTypes.ContentSharing]).BeginConnect(null, null);

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 开启视频

        /// <summary>
        /// 开启视频(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.StartVideo();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 开启视频(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVideo_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.StartVideo();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        public void StartVideo()
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    //给这些参与者发送指定信息
                    ((AVModality)ConversationCard.currentConversationCard.ConversationWindow.Conversation.Modalities[ModalityTypes.AudioVideo]).VideoChannel.BeginStart(null, null);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 会话最大化

        /// <summary>
        /// 会话最大化(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMax_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 会话最大化(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMax_TouchDown(object sender, TouchEventArgs e)
        {

        }

        #endregion

        #region 会话最小化

        /// <summary>
        /// 会话最小化(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMin_Click(object sender, RoutedEventArgs e)
        {

        }
        /// <summary>
        /// 会话最小化(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMin_TouchDown(object sender, TouchEventArgs e)
        {

        }

        #endregion

        #region 会话全屏

        /// <summary>
        /// 会话全屏(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.FullScreen();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
        /// <summary>
        /// 会话全屏(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnFullScreen_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.FullScreen();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        public void FullScreen()
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    ConversationCard.currentConversationCard.ConversationWindow.ShowFullScreen(0);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 会话还原

        /// <summary>
        /// 会话还原(pc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduction_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 会话还原(pad)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduction_TouchDown(object sender, TouchEventArgs e)
        {

        }

        #endregion

        #region 会话关闭

        /// <summary>
        /// 会话关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //会话关闭
                this.ConversationClose();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 会话关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                //会话关闭
                this.ConversationClose();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 会话关闭
        /// </summary>
        public void ConversationClose()
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    ConversationCard.currentConversationCard.ConversationWindow.Close();
                    ConversationCard.currentConversationCard = null;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 悬浮窗体展开/闭合

        /// <summary>
        /// 悬浮窗体展开/闭合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ellipse_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DisplayContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 悬浮窗体展开/闭合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ellipse_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.DisplayContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 悬浮窗体展开/闭合
        /// </summary>
        public void DisplayContent()
        {
            try
            {
                this.SetDisplayContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 设置隐藏

        /// <summary>
        /// 设置隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetHidenContent()
        {
            try
            {
                this.Height = 45;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 设置隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SetDisplayContent()
        {
            try
            {
                this.Height = 500;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 鼠标离开事件

        /// <summary>
        /// 鼠标离开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationManageWindow_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.SetHidenContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 鼠标进入事件

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationManageWindow_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                this.SetDisplayContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 悬浮球点击事件

        /// <summary>
        /// 悬浮球点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnManage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.SetDisplayContent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 登陆窗体移动

        /// <summary>
        /// 登陆窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    base.DragMove();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 强制同步

        /// <summary>
        /// 强制同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
         void btnAsyn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    this.ParticalsSynchronous(ConversationCard.currentConversationCard.ConversationWindow);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 强制同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAsyn_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (ConversationCard.currentConversationCard != null)
                {
                    this.ParticalsSynchronous(ConversationCard.currentConversationCard.ConversationWindow);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        ///  联系人同步（研讨客户端与lync同步）
        /// </summary>
        public void ParticalsSynchronous(ConversationWindow conversationWindow)
        {
            try
            {
                if (conversationWindow != null)
                {
                    //清除主会话联系人列表（实际）
                    List<string> list = new List<string>();
                    //重新填充主会话联系人列表（实际）
                    foreach (var partical in conversationWindow.Conversation.Participants)
                    {
                        list.Add(partical.Contact.Uri);
                    }
                    //遍历所有参会人
                    foreach (var item in Constant.ParticipantList)
                    {
                        //获取参会人
                        Contact contact = Constant.lyncClient.ContactManager.GetContactByUri(item);
                        //获取参会人状态
                        double s = Convert.ToDouble(contact.GetContactInformation(ContactInformationType.Availability));
                        //状态3500,对方不在线
                        if (s == 3500)
                        {
                            //呼叫在线参会人加入会话（添加参会人）
                            if (conversationWindow.Conversation.CanInvoke(ConversationAction.AddParticipant))
                            {
                                //添加除自己之外的参会人
                                if (!contact.Equals(Constant.lyncClient.Self.Contact))
                                {
                                    //对应实际参会人列表
                                    if (!list.Contains(contact.Uri))
                                    {
                                        //添加在线的参会人
                                        conversationWindow.Conversation.AddParticipant(contact);
                                    }
                                }
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
    }
}
