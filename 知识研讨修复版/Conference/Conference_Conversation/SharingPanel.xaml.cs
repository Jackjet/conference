using Conference_Conversation;
using Conference_Conversation.Common;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using vy = System.Windows.Visibility;

namespace Conference_Conversation
{
    /// <summary>
    /// SharingPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SharingPanel : WindowBase
    {
        #region 字段

        /// <summary>
        /// 是否投影
        /// </summary>
        BigEnterScreen bigEnterScreen = BigEnterScreen.NoEnter;

        /// <summary>
        /// 没有人投影
        /// </summary>
        string noEnter = "未投影";

        /// <summary>
        /// 某人正在投影
        /// </summary>
        string someOneEnter = "当前（{0}）正在投影";

        /// <summary>
        /// 我要投影
        /// </summary>
        string NeedEnter = "我要投影";

        /// <summary>
        /// 退出投影
        /// </summary>
        string ExitEnter = "退出投影";

        #endregion

        #region 绑定属性

        vy netWork_ViewVisibility = vy.Collapsed;
        /// <summary>
        /// 网络连接状况视图显示
        /// </summary>
        public vy NetWork_ViewVisibility
        {
            get { return netWork_ViewVisibility; }
            set
            {
                if (this.netWork_ViewVisibility != value)
                {
                    this.netWork_ViewVisibility = value;
                    this.OnPropertyChanged("NetWork_ViewVisibility");
                }
            }
        }


        string enterBigScreenName ="未投影";
        /// <summary>
        /// 投影人
        /// </summary>
        public string EnterBigScreenName
        {
            get { return enterBigScreenName; }
            set
            {
                if (this.enterBigScreenName != value)
                {
                    this.enterBigScreenName = value;
                    this.OnPropertyChanged("EnterBigScreenName");
                }
            }
        }

        string connectedState = "我要投影";
        /// <summary>
        /// 连接显示
        /// </summary>
        public string ConnectedState
        {
            get { return connectedState; }
            set
            {
                if (this.connectedState != value)
                {
                    this.connectedState = value;
                    this.OnPropertyChanged("ConnectedState");
                }
            }
        }
         

        #endregion

        #region 回调事件

        /// <summary>
        /// 共享桌面回调
        /// </summary>
        //Action ShareDeskCallBack = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SharingPanel()
        {
            try
            {
                this.InitializeComponent();
                //事件注册
                this.EventRegedit();
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }
        #endregion

         #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //加载事件
                base.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
                //共享桌面连接
                this.btnConnect.Click += new RoutedEventHandler(this.btnConnect_Click);
                //断开桌面共享连接
                this.btnDisconnect.Click += new RoutedEventHandler(this.btnDisconnect_Click);
                //暂停投影
                //this.btnPause.Click += new RoutedEventHandler(this.btnPause_Click);
                //最小化投影
                //this.btnMini.Click += new RoutedEventHandler(this.btnMini_Click);
                //关闭共享面板
                //this.btnClose.Click += new RoutedEventHandler(this.btnClose_Click);
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

        #region 关闭程序

        /// <summary>
        /// 关闭程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //关闭当前应用程序
                System.Windows.Application.Current.Shutdown(0);
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 当前窗体最小化

        /// <summary>
        /// 当前窗体最小化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMini_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //窗体最小化
                base.WindowState = WindowState.Minimized;
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 暂停投影

        /// <summary>
        /// 暂停投影
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //使用快捷键（Ctrl + P）【模拟】
                SendKeys.SendWait("^+{p}");
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 开始连接()

        /// <summary>
        /// 开始连接()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //判断共享协作页面是否加载
                if (ConversationM.conversationM != null)
                {
                    //是否进入大屏投影
                    switch (this.bigEnterScreen)
                    {
                        case BigEnterScreen.Enter:
                            //断开连接
                            bool canDisConnect = LyncHelper.DisConnectDeskShare();
                            if (!canDisConnect)
                            {
                                //如断开连接失败则及时更新状态
                                this.UpdateState(null);
                            }

                            break;
                        case BigEnterScreen.NoEnter:
                            //开始桌面共享
                            bool canConnect = LyncHelper.ShareDesk();
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion


        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisconnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion

       
        #region 窗体初始化（设置默认尺寸、位置）

        /// <summary>
        ///  窗体初始化（设置默认尺寸、位置）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
               //移动窗体到指定位置
                Win32API.MoveWindow(new WindowInteropHelper(this).Handle, (int)((double)Screen.PrimaryScreen.WorkingArea.Width - base.Width), (int)((double)Screen.PrimaryScreen.WorkingArea.Height - base.Height), (int)base.Width, (int)base.Height, true);
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 窗体移动

        /// <summary>
        /// 窗体移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ///左键设置是否可以拖动
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    //进行拖动
                    base.DragMove();
                }
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
        }

        #endregion

        #region 更改状态（名称）

        /// <summary>
        /// 更改状态（名称）
        /// </summary>
        /// <param name="presenter"></param>
        public void UpdateState(string presenter)
        {
            try
            {
                //共享协作不为null
                if (ConversationM.conversationM != null)
                {
                    if (string.IsNullOrEmpty(presenter))
                    {
                        //未投影
                        this.EnterBigScreenName = this.noEnter;
                        //我要投影
                        this.ConnectedState = this.NeedEnter;
                        //投影类型
                        this.bigEnterScreen = BigEnterScreen.NoEnter;
                        //设置会话区域显示内容
                        ConversationM.conversationM.SetConversationAreaShow(ShowType.ConversationView, true);
                    }
                    else if (ConversationCodeEnterEntity.SelfName.Equals(presenter))
                    {
                        //某参会人投影
                        this.EnterBigScreenName = string.Format(this.someOneEnter, presenter);
                        //离开投影
                        this.ConnectedState = this.ExitEnter;
                        //投影类型
                        this.bigEnterScreen = BigEnterScreen.Enter;
                        //设置会话区域显示内容
                        ConversationM.conversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);
                    }
                    else
                    {
                        //某参会人投影
                        this.EnterBigScreenName = string.Format(this.someOneEnter, presenter);
                        //我要投影
                        this.ConnectedState = this.NeedEnter;
                        //投影类型
                        this.bigEnterScreen = BigEnterScreen.NoEnter;
                        //设置会话区域显示内容
                        ConversationM.conversationM.SetConversationAreaShow(ShowType.ConversationView, true);
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

        #endregion

        #region 网络连接异常提示

        /// <summary>
        /// 网络连接异常提示
        /// </summary>
        /// <param name="message"></param>
        public void SettingNetWorkConnectErrorTip(string message)
        {
            try
            {
                //共享面板异常视图显示
                this.NetWork_ViewVisibility = vy.Visible;
                this.netWork_View.SetMessage(message);
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

        #region 最小化窗体时发生

        /// <summary>
        /// 最小化窗体时发生
        /// </summary>
        public void MinwindowDealWidth()
        {
              try
            {
                if (this.Visibility == vy.Collapsed)
                {
                    //鼠标单击显示
                    this.Visibility = vy.Visible;
                    //显示
                    this.Show();
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
