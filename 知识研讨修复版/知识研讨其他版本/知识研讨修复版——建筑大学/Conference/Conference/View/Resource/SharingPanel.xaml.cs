using Conference.Common;
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

namespace Conference.View.Resource
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

        #endregion

        #region 绑定字段

        Visibility netWork_ViewVisibility = Visibility.Collapsed;
        /// <summary>
        /// 网络连接状况视图显示
        /// </summary>
        public Visibility NetWork_ViewVisibility
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

                //绑定当前上下文
                this.DataContext = this;

                base.Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
                this.btnConnect.Click += new RoutedEventHandler(this.btnConnect_Click);
                this.btnDisconnect.Click += new RoutedEventHandler(this.btnDisconnect_Click);
                this.btnPause.Click += new RoutedEventHandler(this.btnPause_Click);
                this.btnMini.Click += new RoutedEventHandler(this.btnMini_Click);
                this.btnClose.Click += new RoutedEventHandler(this.btnClose_Click);
                //this.txtName.Text = Constant.SelfName;
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

        #region 关闭程序

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        #region 最小化

        private void btnMini_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        #region 断开连接

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

        #region 开始连接

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                switch (this.bigEnterScreen)
                {
                    case BigEnterScreen.Enter:
                        MainWindow.MainPageInstance.ConversationM.DisConnectDeskShare();
                        break;
                    case BigEnterScreen.NoEnter:
                        MainWindow.MainPageInstance.ConversationM.ShareDesk();
                        break;
                    default:
                        break;
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

        #region 窗体初始化（设置默认尺寸、位置）

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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

        private void WindowMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
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
        public void UpdateState(string presenter)
        {
            try
            {
                if (string.IsNullOrEmpty(presenter))
                {
                    this.txtName.Text = "未 投 影";
                    this.btnConnect.Content = "我 要 投 影";
                    this.bigEnterScreen = BigEnterScreen.NoEnter;
                    //设置会话区域显示内容
                    MainWindow.MainPageInstance.ConversationM.SetConversationAreaShow(ShowType.ConversationView, true);
                }
                else if (Constant.SelfName.Equals(presenter))
                {
                    this.txtName.Text = string.Format("当前（{0}）正在投影", presenter);
                    this.btnConnect.Content = "退 出 投 影";
                    this.bigEnterScreen = BigEnterScreen.Enter;
                    //设置会话区域显示内容
                    MainWindow.MainPageInstance.ConversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);
                }
                else
                {
                    this.txtName.Text = string.Format("当前（{0}）正在投影", presenter);
                    this.btnConnect.Content = "我 要 投 影";
                    this.bigEnterScreen = BigEnterScreen.NoEnter;
                    //设置会话区域显示内容
                    MainWindow.MainPageInstance.ConversationM.SetConversationAreaShow(ShowType.ConversationView, true);
                }

                #region old solution

                //if (MainWindow.mainWindow.WindowState == System.Windows.WindowState.Maximized)
                //{
                //    if (LyncHelper.CanShowContent)
                //    {
                //        LyncHelper.HidenWindowContent();
                //    }
                //    else if (MainWindow.MainPageInstance.ViewSelectedItemEnum == ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource && MainWindow.MainPageInstance.ConversationM.PageIndex == ResourceType.Share)
                //    {
                //        LyncHelper.ShowWindowContent();
                //    }
                //}

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

        #region 网络连接异常提示

        public void SettingNetWorkConnectErrorTip(string message)
        {
            try
            {
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
    }

    enum BigEnterScreen
    {
        Enter,
        NoEnter
    }
}
