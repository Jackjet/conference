using Conference.Common;
using Conference.Page;
using ConferenceCommon.TimerHelper;
using Conference.View.Space;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.KeyBoard;
using ConferenceCommon.LogHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Threading;
using System.Diagnostics;
using ConferenceCommon.NetworkHelper;
using Conference.View.Resource;
using ConferenceCommon.IcoFlash;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model;

namespace Conference
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        #region 字段

        /// <summary>
        /// 会话窗体状态管理模型
        /// </summary>
        //ConferenceCommon.IconHelper.Win32API.ManagedWindowPlacement Placement = new Win32API.ManagedWindowPlacement() { showCmd = 2 };

        #endregion

        #region 静态字段

        /// <summary>
        /// 是否可以签入
        /// </summary>
        public static bool CanSigined;

        /// <summary>
        /// 左偏偏移会话窗体）
        /// </summary>
        public static int conversationLeft = 0;

        /// <summary>
        /// 顶部偏移（会话窗体）
        /// </summary>
        public static int conversationTop = 0;

        /// <summary>
        /// 当前主窗体
        /// </summary>
        public static MainWindow mainWindow = null;

        /// <summary>
        /// 当前主窗体
        /// </summary>
        public static Index IndexInstance = null;

        /// <summary>
        /// 单例（主页面）
        /// </summary>
        public static MainPage MainPageInstance = null;

        #endregion

        #region 绑定属性

        string mainWindowHeader2;
        /// <summary>
        /// 标题2
        /// </summary>
        public string MainWindowHeader2
        {
            get { return mainWindowHeader2; }
            set
            {
                if (this.mainWindowHeader2 != value)
                {
                    this.mainWindowHeader2 = value;
                    this.OnPropertyChanged("MainWindowHeader2");
                }
            }
        }

        string mainWindowHeader3;
        /// <summary>
        /// 标题3
        /// </summary>
        public string MainWindowHeader3
        {
            get { return mainWindowHeader3; }
            set
            {
                if (this.mainWindowHeader3 != value)
                {
                    this.mainWindowHeader3 = value;
                    this.OnPropertyChanged("MainWindowHeader3");
                }
            }
        }

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

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainWindow()
        {
            try
            {
                //加载UI
                InitializeComponent();

                //绑定当前上下文
                this.DataContext = this;

                #region 控件绑定

                //自我绑定
                MainWindow.mainWindow = this;
                //主页绑定
                MainWindow.MainPageInstance = this.mainPage;
                //首页绑定
                MainWindow.IndexInstance = this.index;

                #endregion

                #region 事件注册

                //退出
                this.btnExit.Click += btnExit_Click;
                //首页子项选择事件
                this.index.IndexItemSelected += index_IndexItemSelected;
                //主窗体关闭事件
                this.Closed += MainWindow_Closed;
                //弹出软键盘
                this.btnkeyBoard.Click += btnkeyBoard_Click;
                //刷新
                this.btnReflesh.Click += btnReflesh_Click;
                //通讯修复
                //this.btnCommunicationRepair.Click += btnCommunicationRepair_Click;
                //返回高级菜单
                this.btnBack.Click += btnBack_Click;
                //状态更改
                this.StateChanged += MainWindow_StateChanged;

                this.ChangedToDesk.Click += ChangedToDesk_Click;

                #endregion

                //获取automation
                Constant.lyncAutomation = LyncClient.GetAutomation();
                //时间显示
                TimerDisplayManage.TimerDisplay(this.txtNowTime);
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

        #region 返回高级菜单

        /// <summary>
        /// 返回高级菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //首页切换(Index)
                this.IndexPageChangedToIndexPage();
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

        #region 弹出软键盘


        Process notePadProcess = null;
        /// <summary>
        /// 弹出软键盘
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnkeyBoard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //switch (mainPage.ViewSelectedItemEnum)
                //{
                //    case ViewSelectedItemEnum.Meet:
                //        break;
                //    case ViewSelectedItemEnum.Tree:
                //        this.UsingTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.Space:
                //        this.OpenTheTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.Resource:
                //        this.OpenTheTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.IMM:
                //        this.UsingTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.PersonNote:
                //        this.OpenTheTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.MeetVote:
                //        this.OpenTheTouchKeyBoard();
                //        break;
                //    case ViewSelectedItemEnum.U_Disk:
                //        break;
                //    case ViewSelectedItemEnum.Meet_Change:
                //        break;
                //    case ViewSelectedItemEnum.Chair:
                //        break;
                //    case ViewSelectedItemEnum.Studiom:
                //        break;
                //    case ViewSelectedItemEnum.SystemSetting:
                //        break;
                //    case ViewSelectedItemEnum.Tool:
                //        break;
                //    default:
                //        break;
                //}
                this.OpenTheTouchKeyBoard();
                //使用计时器执行软键盘透明设置
                TimerJob.StartRun(new Action(() =>
                {
                    //设置键盘透明度
                    this.TouchKeyBoardOpacitySetting();
                }), 1000);
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
        /// 打开键盘
        /// </summary>
        public void OpenTheTouchKeyBoard()
        {
            try
            {
                TouchKeyBoard.ShowInputPanel();
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
        /// 引导键盘
        /// </summary>
        public void UsingTouchKeyBoard()
        {
            try
            {
                notePadProcess = ProcessManage.OpenAppByAppNameGetProcess("notepad.exe");
                //设置合适尺寸
                Win32API.MoveWindow(notePadProcess.Handle, 0, 0, 70, 70, false);
                TimerJob.StartRun(new Action(() =>
                {
                    notePadProcess.Kill();
                    notePadProcess = null;
                }), 700);
                //}             
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

        #region 设置键盘透明度

        /// <summary>
        /// 设置键盘透明度
        /// </summary>
        public void TouchKeyBoardOpacitySetting()
        {
            try
            {
                string keyFile = AppDomain.CurrentDomain.BaseDirectory + Constant.KeyboardSettingFile_32;
                if (File.Exists(keyFile))
                {
                    //设置触摸键盘透明
                    ProcessManage.OpenFileByLocalAddress(keyFile);
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
        /// 结束透明键盘程序
        /// </summary>
        public void DisposeTouchKeyBoardOpacityApplication()
        {
            try
            {
                ProcessManage.KillProcess("TabTip");
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

        #region 首页子项选择事件

        /// <summary>
        /// 首页子项选择事件
        /// </summary>
        void index_IndexItemSelected(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //首页切换(MainPage)
                this.IndexPageChangedToMainPage();
                //head样式更改
                this.borMain.Background = this.Resources["brush_Header2"] as ImageBrush;
                //导航到指定视图
                this.mainPage.NavicateView(viewSelectedItemEnum);
                //只要符合其中一条，进行相应样式切换
                if (viewSelectedItemEnum == ViewSelectedItemEnum.Studiom || viewSelectedItemEnum == ViewSelectedItemEnum.Meet_Change
                    || viewSelectedItemEnum == ViewSelectedItemEnum.SystemSetting || viewSelectedItemEnum == ViewSelectedItemEnum.U_Disk ||
                    viewSelectedItemEnum == ViewSelectedItemEnum.Chair)
                {
                    //工具箱模块处理
                    this.mainPage.DealWithTool();
                    //切换控制中心
                    this.mainPage.ToolCmWindow.NavicateChangeCenter(viewSelectedItemEnum);
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

        #region 首页切换（）

        /// <summary>
        /// 首页切换(Index)
        /// </summary>
        public void IndexPageChangedToIndexPage()
        {
            try
            {
                //首页隐藏
                this.index.Visibility = System.Windows.Visibility.Visible;
                //主页显示
                this.mainPage.Visibility = System.Windows.Visibility.Collapsed;
                //导航到首页
                //this.mainPage.ViewSelectedItemEnum = ViewSelectedItemEnum.Index;
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
        /// 首页切换(MainPage)
        /// </summary>
        public void IndexPageChangedToMainPage()
        {
            try
            {
                //首页隐藏
                this.index.Visibility = System.Windows.Visibility.Collapsed;
                //主页显示
                this.mainPage.Visibility = System.Windows.Visibility.Visible;
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

        #region 状态更改

        /// <summary>
        /// 状态更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_StateChanged(object sender, EventArgs e)
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                    {
                        if (this.WindowState == System.Windows.WindowState.Maximized)
                        {
                            MainWindowToMaxDealWidth();
                        }
                        else
                        {
                            MainWindowToMinDealWidth();
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
        ///  窗体最小化处理
        /// </summary>
        private void MainWindowToMinDealWidth()
        {
            try
            {
                //隐藏菜单
                mainPage.ToolCmWindow.Visibility = System.Windows.Visibility.Collapsed;

                //设置会话区域显示内容
                this.mainPage.ConversationM.SetConversationAreaShow(ShowType.HidenView, false);

                if (MainWindow.MainPageInstance.SharingPanel.Visibility == System.Windows.Visibility.Collapsed)
                {
                    //鼠标单击显示
                    MainWindow.MainPageInstance.SharingPanel.Visibility = System.Windows.Visibility.Visible;
                    //显示
                    MainWindow.MainPageInstance.SharingPanel.Show();
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
        /// 窗体最大化处理
        /// </summary>
        private void MainWindowToMaxDealWidth()
        {
            try
            {
                //隐藏闪烁
                WindowExtensions.StopFlashingWindow(this);

                //设置会话区域显示内容
                this.mainPage.ConversationM.SetConversationAreaShow(ShowType.ConversationView, false);

                this.Topmost = true;
                TimerJob.StartRun(new Action(() =>
                {
                    this.Topmost = false;
                }), 700);
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

        #region 系统退出

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //关闭当前进程
                Application.Current.Shutdown(0);
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

        #region 主窗体关闭事件

        /// <summary>
        /// 主窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Closed(object sender, EventArgs e)
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

        #region 刷新（刷新当前页面和重新加载通讯节点）

        /// <summary>
        /// 刷新（刷新当前页面）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReflesh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewSelectedItemEnum viewSelectedItemEnum = this.mainPage.ViewSelectedItemEnum;

                //刷新页面
                this.mainPage.PageReflesh(viewSelectedItemEnum);

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

        #region 切换到桌面

        /// <summary>
        /// 切换到桌面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangedToDesk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //设置主窗体为最小化
                this.WindowState = WindowState.Minimized;
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
}
