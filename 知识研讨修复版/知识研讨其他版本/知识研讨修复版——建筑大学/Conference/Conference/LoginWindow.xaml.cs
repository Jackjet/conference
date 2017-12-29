using Conference.Common;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.DetectionHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.FileHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.KeyBoard;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.RefreshSystemTrayHelper;
using ConferenceCommon.RegeditHelper;
using ConferenceCommon.WindowHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceModel;
using ConferenceCommon.VersionHelper;
using ConferenceCommon.WPFHelper;

namespace Conference
{
    /// <summary>
    /// 研讨客户端登陆窗体
    /// </summary>
    public partial class LoginWindow : WindowBase
    {
        #region 静态字段

        /// <summary>
        /// 个人用户信息（本地存储）
        /// </summary>
        static MeetingUser user;

        #endregion

        #region 绑定属性

        bool isPwdRemmber;
        /// <summary>
        /// 是否记住用户名密码
        /// </summary>
        public bool IsPwdRemmber
        {
            get { return isPwdRemmber; }
            set
            {
                if (this.isPwdRemmber != value)
                {
                    this.isPwdRemmber = value;
                    this.OnPropertyChanged("IsPwdRemmber");
                }
            }
        }

        int stateIndex;
        /// <summary>
        /// 状态下标
        /// </summary>
        public int StateIndex
        {
            get { return stateIndex; }
            set
            {
                if (this.stateIndex != value)
                {
                    this.stateIndex = value;
                    this.OnPropertyChanged("StateIndex");
                }
            }
        }

        Visibility isLogining = Visibility.Hidden;
        /// <summary>
        /// 显示登陆提示
        /// </summary>
        public Visibility IsLogining
        {
            get { return isLogining; }
            set
            {
                if (this.isLogining != value)
                {
                    this.isLogining = value;
                    this.OnPropertyChanged("IsLogining");
                }
            }
        }


        bool loginPanelIsEnable = false;
        /// <summary>
        /// 登陆面板是否可用
        /// </summary>
        public bool LoginPanelIsEnable
        {
            get { return loginPanelIsEnable; }
            set
            {
                if (this.loginPanelIsEnable != value)
                {
                    this.loginPanelIsEnable = value;
                    this.OnPropertyChanged("LoginPanelIsEnable");
                }
            }
        }

        bool isAutoLogin;
        /// <summary>
        /// 是否自动登陆
        /// </summary>
        public bool IsAutoLogin
        {
            get { return isAutoLogin; }
            set
            {
                isAutoLogin = value;
                this.OnPropertyChanged("IsAutoLogin");
            }
        }

        Visibility loginAddition_Visibility = Visibility.Visible;
        /// <summary>
        /// 自动登陆、登陆状态、记住密码区域（显示设置）
        /// </summary>
        public Visibility LoginAddition_Visibility
        {
            get { return loginAddition_Visibility; }
            set
            {
                if (this.loginAddition_Visibility != value)
                {
                    this.loginAddition_Visibility = value;
                    this.OnPropertyChanged("RemmberCode_Visibility");
                }
            }
        }

        Thickness editPanelMargin;
        /// <summary>
        /// 编辑区域位置设置
        /// </summary>
        public Thickness EditPanelMargin
        {
            get { return editPanelMargin; }
            set
            {
                if (value != this.editPanelMargin)
                {
                    editPanelMargin = value;
                    this.OnPropertyChanged("EditPanelMargin");
                }
            }
        }

        string errorTip;
        /// <summary>
        /// 登陆错误提示
        /// </summary>
        public string ErrorTip
        {
            get { return errorTip; }
            set
            {
                if (value != this.errorTip)
                {
                    errorTip = value;
                    this.OnPropertyChanged("ErrorTip");
                }
            }
        }

        Visibility errorTipVisibility = Visibility.Collapsed;
        /// <summary>
        /// 提示显示
        /// </summary>
        public Visibility ErrorTipVisibility
        {
            get { return errorTipVisibility; }
            set
            {
                if (value != this.errorTipVisibility)
                {
                    errorTipVisibility = value;
                    this.OnPropertyChanged("ErrorTipVisibility");
                }
            }
        }

        Visibility initializtionVisibility = Visibility.Visible;
        /// <summary>
        /// 显示初始化
        /// </summary>
        public Visibility InitializtionVisibility
        {
            get { return initializtionVisibility; }
            set
            {
                if (value != this.initializtionVisibility)
                {
                    initializtionVisibility = value;
                    this.OnPropertyChanged("InitializtionVisibility");
                }
            }
        }

        string loginEnviromentTip = "正在初始化中.....";
        /// <summary>
        /// 登录环境提示
        /// </summary>
        public string LoginEnviromentTip
        {
            get { return loginEnviromentTip; }
            set
            {
                if (value != this.loginEnviromentTip)
                {
                    loginEnviromentTip = value;
                    this.OnPropertyChanged("LoginEnviromentTip");
                }
            }
        }

        //SolidColorBrush initializtionColor  ;
        ///// <summary>
        ///// 初始化字体颜色
        ///// </summary>
        //public SolidColorBrush InitializtionColor
        //{
        //    get { return initializtionColor; }
        //    set
        //    {
        //        if (value != this.initializtionColor)
        //        {
        //            initializtionColor = value;
        //            this.OnPropertyChanged("InitializtionColor");
        //        }
        //    }
        //}

        #endregion

        #region 一般属性

        bool isLoginVerify = false;
        /// <summary>
        /// 是否经过验证
        /// </summary>
        public bool IsLoginVerify
        {
            get { return isLoginVerify; }
            set { isLoginVerify = value; }
        }


        /// <summary>
        /// 通知接受计时器
        /// </summary>
        public DispatcherTimer timerAcept = null;

        bool canThrow = false;
        /// <summary>
        /// 可以通过
        /// </summary>
        public bool CanThrow
        {
            get { return canThrow; }
            set { canThrow = value; }
        }

        #endregion

        #region 字段

        /// <summary>
        /// 打断登陆
        /// </summary>
        //bool isInterruptLogin = false;

        #endregion

        #region 自定义事件

        #endregion

        #region 构造函数

        public LoginWindow()
        {
            try
            {
                //UI加载
                InitializeComponent();
                //绑定当前上下文
                this.DataContext = this;
                //状态设置
                StateSetting();
                //注册事件
                EventRegedit();
                //lync程序环境设置（事件、状态、原生态界面抑制、注册表、标示）
                LyncEnviromentPrepare();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 注册事件

        private void EventRegedit()
        {
            try
            {
                //登陆
                this.btnLogin.Click += btnLogin_Click;
                //取消登陆
                this.btnCancel.Click += btnCancel_Click;
                //点击enter键进行登陆
                this.KeyDown += LoginWindow_KeyDown;

                this.chkAutoLogin.Unchecked += chkAutoLogin_Unchecked;

                //状态1
                LyncHelper.State1CallBack = this.State1CallBackCompleate;
                //状态2
                LyncHelper.State2CallBack = this.State2CallBackCompleate;
                //状态3
                LyncHelper.State3CallBack = this.State2CallBackCompleate;
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

        #region lync状态设置

        /// <summary>
        /// lync状态设置
        /// </summary>
        private void StateSetting()
        {
            try
            {
                switch (Constant.ContextualModel)
                {
                    //正常模式(私人模式)
                    case ContextualModelType.Normal:
                        //获取本地个人用户存储信息
                        LoginWindow.user = FileManage.Load_Entity<MeetingUser>(Constant.UserFilePath);
                        //绑定本存储的用户信息
                        this.txtUser.Text = LoginWindow.user.UserName;
                        //登陆密码
                        this.pwd.Password = LoginWindow.user.PassWord;
                        //记住密码
                        this.IsPwdRemmber = LoginWindow.user.IsPwdRemmber;
                        //自动登陆
                        this.IsAutoLogin = LoginWindow.user.IsAutoLogin;
                        //登陆状态
                        this.StateIndex = Convert.ToInt32(LoginWindow.user.State);
                        //自动登陆（登陆窗体显示事件）
                        this.ContentRendered += LoginWindow_ContentRendered;
                        this.EditPanelMargin = new Thickness(0);
                        break;

                    //会议模式
                    case ContextualModelType.Conference:
                        //自动登陆、登陆状态、记住密码区域（显示设置）
                        this.LoginAddition_Visibility = System.Windows.Visibility.Hidden;
                        this.EditPanelMargin = new Thickness(50, 0, 0, 0);
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
        }

        #endregion

        #region lync程序环境设置（事件、状态、原生态界面抑制、注册表、标示）

        /// <summary>
        /// lync程序环境设置（事件、状态、原生态界面抑制、注册表、标示）
        /// </summary>
        private void LyncEnviromentPrepare()
        {
            try
            {
                //初始化加载
                LyncHelper.SetLyncAplicationEnviroment(new Action(() =>
                {
                    if (this.Topmost == true || this.LoginPanelIsEnable == false)
                    {
                        //设置主窗体为最顶层
                        this.Topmost = false;

                        //登陆编辑区域恢复可用状态
                        this.LoginButtonIsEnable();
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

        #region 状态更改事件(进入主界面)

        public void State1CallBackCompleate()
        {
            try
            {
                //是否进行过登陆验证
                if (this.IsLoginVerify)
                {
                    //(登陆窗体、登陆提示、开始菜单隐藏)                           
                    //登陆窗体隐藏
                    this.Visibility = System.Windows.Visibility.Hidden;
                    //登陆提示隐藏
                    this.IsLogining = System.Windows.Visibility.Hidden;

                    //设置当前用户名
                    LyncHelper.SetCurrentUser();

                    #region 进入主界面

                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        //创建客户端对象模型实例(并通过验证)
                        Constant.clientContextManage.CreateClient(Constant.SpaceWebSiteUri, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDoaminPart1Name);

                        //设置DNS
                        //NetWorkAdapter.EnableDHCP2();
                    });

                    //创建主界面
                    MainWindow mainWindow = new MainWindow();

                    //显示主界面
                    mainWindow.Show();

                    #endregion
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

        public void State2CallBackCompleate()
        {
            try
            {
                //登陆验证设置为未验证
                this.IsLoginVerify = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        public void State3CallBackCompleate()
        {
            try
            {
                //登陆面板设置为可用
                //this.LoginPanelIsEnable = true;
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

        #region 快捷键事件

        /// <summary>
        /// 快捷键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginWindow_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                //使用Enter键进行登陆
                if (e.Key == Key.Enter)
                {
                    TimerJob.StartRun(new Action(() =>
                    {
                        //显示登陆提示(旋转)
                        this.IsLogining = System.Windows.Visibility.Visible;
                    }), 100);

                    TimerJob.StartRun(new Action(() =>
                    {
                        this.Login();
                    }), 100);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 窗体显示事件

        /// <summary>
        /// 自动登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LoginWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                //是否为自动登陆（私人模式启用）
                if (this.IsAutoLogin)
                {
                    //是否自动登陆
                    DispatcherTimer timer = null;

                    TimerJob.StartRun(new Action(() =>
                    {
                        if (this.IsLogining == System.Windows.Visibility.Hidden)
                        {
                            //开始登陆
                            this.Login();
                        }
                        else
                        {
                            timer.Stop();
                        }
                    }), 50, out timer);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 登陆事件

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                  {
                      //显示登陆提示(旋转)
                      this.IsLogining = System.Windows.Visibility.Visible;
                  }), 100);

                TimerJob.StartRun(new Action(() =>
                {
                    this.Login();
                }), 100);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 登陆
        /// </summary>
        private void Login()
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtUser.Text))
                {
                    this.CodeOrUsingIsNull("请输入用户名");
                    return;
                }

                if (string.IsNullOrEmpty(this.pwd.Password))
                {
                    this.CodeOrUsingIsNull("请输入密码");
                    return;
                }
                LyncHelper.IsSignedOutDoSomeThing(new Action(() =>
                    {
                        MainWindow.CanSigined = true;
                    }));
                //嵌入条件判断（lync实例是否存在）
                if (MainWindow.CanSigined)
                {
                    #region 登陆前先进行判断（是否连接网络,是否能够连接服务器）

                    //验证是否能够访问AD
                    if (!DetectionManage.TestNetConnectity(Constant.DNS1))
                    {
                        this.ShowNetWorkIsNotThrow("AD服务器连接失败,请及时联系管理员");
                        return;
                    }

                    bool lyncServiceCanVisit = false;

                    if (DetectionManage.TestNetConnectity(Constant.LyncIP1) || DetectionManage.TestNetConnectity(Constant.LyncIP2))
                    {
                        lyncServiceCanVisit = true;
                    }

                    //验证是否能够访问LYNC服务器
                    if (!lyncServiceCanVisit)
                    {
                        this.ShowNetWorkIsNotThrow("Lync服务器连接失败,请及时联系管理员");
                        return;
                    }

                    //验证是否能够访问外围服务器
                    if (!DetectionManage.TestNetConnectity(Constant.TreeServiceIP))
                    {
                        this.ShowNetWorkIsNotThrow("会议服务器连接失败,请及时联系管理员");
                        return;
                    }

                    //验证是否能够访问LYNC扩展web服务（研讨服务）
                    if (!DetectionManage.IsWebServiceAvaiable(Constant.TreeServiceAddressFront + Constant.ConferenceTreeServiceWebName))
                    {
                        this.ShowNetWorkIsNotThrow("会议服务器访问失败,请及时联系管理员");
                        return;
                    }

                    #endregion

                    
                    //邮件地址
                    var email = this.txtUser.Text.Trim() + "@" + Constant.UserDomain;

                    ModelManage.ConferenceInfo.CheckUserIsOnline(email, new Action<bool>((isOnline) =>
                    {
                        if (isOnline)
                        {
                            this.CodeOrUsingIsNull("当前用户已在线");
                            return;
                        }
                        else
                        {
                            //登陆编辑区域设置为不可用
                            this.LoginPanelIsEnable = false;
                            //lync状态设置
                            LyncHelper.LyncStateSetting(this.StateIndex);
                            //lync嵌入
                            LyncHelper.LyncSignning(email, this.pwd.Password, StartSignIn);
                        }
                    }));

                   
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                //出现异常,关闭登陆提示
                this.IsLogining = System.Windows.Visibility.Collapsed;
                //将登陆编辑区域恢复为可用状态
                this.LoginButtonIsEnable();
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        public void CodeOrUsingIsNull(string tip)
        {
            try
            {
                this.IsLogining = System.Windows.Visibility.Hidden;
                this.ErrorTip = tip;
                if (this.ErrorTipVisibility == System.Windows.Visibility.Collapsed)
                {
                    this.ErrorTipVisibility = System.Windows.Visibility.Visible;
                    TimerJob.StartRun(new Action(() =>
                    {
                        this.ErrorTipVisibility = System.Windows.Visibility.Collapsed;
                    }), 1000);
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
        /// 显示提示（网络不通）
        /// </summary>
        public void ShowNetWorkIsNotThrow(string tip)
        {
            try
            {
                this.IsLogining = System.Windows.Visibility.Hidden;
                this.ErrorTip = tip;
                if (ErrorTipVisibility == System.Windows.Visibility.Collapsed)
                {
                    this.ErrorTipVisibility = System.Windows.Visibility.Visible;
                    TimerJob.StartRun(new Action(() =>
                    {
                        this.ErrorTipVisibility = System.Windows.Visibility.Collapsed;
                    }), 1000);
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

        #region 取消事件

        void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LyncHelper.CancelLyncSigned(new Action(() =>
                    {
                        //隐藏登陆提示
                        this.IsLogining = System.Windows.Visibility.Hidden;
                        //取消登陆
                        this.Visibility = System.Windows.Visibility.Hidden;
                    }));
                Application.Current.Shutdown(0);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region Lync嵌入

        /// <summary>
        /// 登陆Lync
        /// </summary>
        /// <param name="UserUri">用户Uri（用户邮箱）</param>
        /// <param name="Domain">用户域信息</param>
        /// <param name="Password">密码</param>
        public void StartSignIn()
        {
            try
            {
                //个人模式
                if (Constant.ContextualModel == ContextualModelType.Normal)
                {
                    //存储用户信息
                    LoginWindow.user.UserName = this.txtUser.Text.Trim();
                    //是否记住密码
                    LoginWindow.user.IsPwdRemmber = this.IsPwdRemmber;
                    //是否自动登陆
                    LoginWindow.user.IsAutoLogin = this.IsAutoLogin;
                    //是否为记住密码
                    if (this.IsPwdRemmber)
                    {
                        LoginWindow.user.PassWord = this.pwd.Password;
                    }
                    else
                    {
                        //不为记住密码则将密码框清空
                        LoginWindow.user.PassWord = string.Empty;
                    }
                    //状态索引
                    LoginWindow.user.State = (UserLoginState)this.StateIndex;
                    //存储个人用户信息
                    FileManage.Save_Entity(LoginWindow.user, Constant.UserFilePath);

                }
                //登陆用户名（如：tbg）
                Constant.LoginUserName = this.txtUser.Text.Trim();

                //智存空间登陆用户名
                Constant.WebLoginUserName = Constant.UserDoaminPart1Name + @"\" + Constant.LoginUserName;
                //智存空间登陆密码
                Constant.WebLoginPassword = this.pwd.Password;
                //通过验证
                this.IsLoginVerify = true;
            }
            catch (Exception ex)
            {
                //出现异常,登陆编辑区域恢复为可用
                this.LoginButtonIsEnable();
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 登录按钮可用

        public void LoginButtonIsEnable()
        {
            try
            {
                if (!this.LoginPanelIsEnable)
                {
                    DispatcherTimer timer = null;
                    TimerJob.StartRun(new Action(() =>
                        {
                            if (this.CanThrow)
                            {
                                this.LoginPanelIsEnable = true;
                                timer.Stop();
                            }
                        }), 800, out timer);
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

        #region 获取登陆编辑区域焦点（方便交互）
        /// <summary>
        /// 获取登陆编辑区域焦点（方便交互）
        /// </summary>
        public void FocusEditPanel()
        {
            try
            {
                //this.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                //   (Action)(() => { Keyboard.Focus(txtUser); }));

                Keyboard.Focus(txtUser);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 取消自动登陆

        void chkAutoLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.isInterruptLogin = true;
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