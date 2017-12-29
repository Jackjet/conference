using Conference.Common;
using ConferenceCommon.TimerHelper;
using Conference.View.AppcationTool;
using Conference.View.Chair;
using Conference.View.Setting;
using Conference.View.Space;
using Conference.View.Tool;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceModel;
using ConferenceModel.ConferenceAudioWebservice;
using ConferenceModel.ConferenceInfoWebService;
using ConferenceModel.ConferenceTreeWebService;
using ConferenceModel.FileSyncAppPoolWebService;
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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.IcoFlash;
using ConferenceCommon.WPFHelper;
using Conference_Space.Common;
using ConferenceModel.Common;
using Conference_Conversation;
using Conference_Conversation.Common;
using vy = System.Windows.Visibility;

namespace Conference.Page
{
    /// <summary>
    /// MainPage.xaml 的交互逻辑
    /// </summary>
    public partial class MainPage : MainPageBase
    {
        #region 静态字段

        public static MainPage mainPage = null;

        #endregion

        #region 字段

        /// <summary>
        /// 教育模式
        /// </summary>
        bool isEdacationModel = false;

        /// <summary>
        /// 精简模式
        /// </summary>
        bool isSimpleModel = false;

        /// <summary>
        /// 标准模式
        /// </summary>
        bool isNormalModel = false;

        /// <summary>
        /// 按钮样式选择1
        /// </summary>
        Style btnSyle1_Selected = null;

        /// <summary>
        /// 按钮样式选择2
        /// </summary>
        Style btnSyle2_Selected = null;

        /// <summary>
        /// 按钮样式1
        /// </summary>
        Style btnSyle1 = null;

        /// <summary>
        /// 按钮样式2
        /// </summary>
        Style btnSyle2 = null;

        /// <summary>
        /// 工具画刷2
        /// </summary>
        ImageBrush brush_Tool2 = null;

        /// <summary>
        /// u盘传输画刷
        /// </summary>
        ImageBrush brush_U_Disk1 = null;

        /// <summary>
        /// 会议切换画刷
        /// </summary>
        ImageBrush brush_Meet_Change1 = null;

        /// <summary>
        /// 主持人画刷
        /// </summary>
        ImageBrush brush_Chair1 = null;

        /// <summary>
        /// 中控画刷
        /// </summary>
        ImageBrush brush_Studiom1 = null;

        /// <summary>
        /// 系统设置画刷
        /// </summary>
        ImageBrush brush_Setting1 = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MainPage()
        {
            try
            {
                //UI加载
                InitializeComponent();
                mainPage = this;
                //资源样式收集
                this.StyleCollection();
                //基本事件注册
                this.EventRegedit_Normal();
                //事件注册（精简模式）
                this.EventRegedit_Simple();
                //事件注册(教学模式)
                this.EventRegedit_Education();
                //加入会议并处理资源共享
                this.MyConferenceView.JoinConfereince(this.ItemClick_DisposeSourceAndInitAgain);
                //注册父类事件
                this.EventRegedit_Parent();
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

        #region 样式收集

        /// <summary>
        /// 样式收集
        /// </summary>
        public void StyleCollection()
        {
            try
            {
                if (this.Resources.Contains("btnSyle1_Selected"))
                {
                    //按钮样式选择1
                    this.btnSyle1_Selected = this.Resources["btnSyle1_Selected"] as Style;
                }
                if (this.Resources.Contains("btnSyle2_Selected"))
                {
                    //按钮样式选择2
                    this.btnSyle2_Selected = this.Resources["btnSyle2_Selected"] as Style;
                }
                if (this.Resources.Contains("btnSyle1"))
                {
                    //按钮样式1
                    this.btnSyle1 = this.Resources["btnSyle1"] as Style;
                }
                if (this.Resources.Contains("btnSyle2"))
                {
                    //按钮样式2
                    this.btnSyle2 = this.Resources["btnSyle2"] as Style;
                }

                //当前应该程序资源
                ResourceDictionary resource = Application.Current.Resources;
                if (resource.Contains("brush_Tool2"))
                {
                    //工具画刷2
                    this.brush_Tool2 = resource["brush_Tool2"] as ImageBrush;
                }
                if (resource.Contains("brush_U_Disk1"))
                {
                    // u盘传输画刷
                    this.brush_U_Disk1 = resource["brush_U_Disk1"] as ImageBrush;
                }
                if (resource.Contains("brush_Meet_Change1"))
                {
                    //会议切换画刷
                    this.brush_Meet_Change1 = resource["brush_Meet_Change1"] as ImageBrush;
                }
                if (resource.Contains("brush_Chair1"))
                {
                    //主持人画刷
                    this.brush_Chair1 = resource["brush_Chair1"] as ImageBrush;
                }
                if (resource.Contains("brush_Studiom1"))
                {
                    //中控画刷
                    this.brush_Studiom1 = resource["brush_Studiom1"] as ImageBrush;
                }
                if (resource.Contains("brush_Setting1"))
                {
                    //系统设置画刷
                    this.brush_Setting1 = resource["brush_Setting1"] as ImageBrush;
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

       
        #region 事件注册区域

        /// <summary>
        /// 基本事件注册
        /// </summary>
        private void EventRegedit_Normal()
        {
            try
            {
                //会议信息
                this.btnMeet.Click += Navacite;
                //信息交流
                this.btnIMM.Click += Navacite;
                //共享资源
                this.btnResource.Click += Navacite;
                //智存空间
                this.btnSpace.Click += Navacite;

                //知识树
                this.btnTree.Click += Navacite;
                //会议投票
                this.btnVote.Click += Navacite;
                //个人笔记
                this.btnNote.Click += Navacite;
                //工具应用
                this.btnTool.Click += Navacite;

                //主页面点击事件
                this.PreviewMouseLeftButtonDown += MainPage_PreviewMouseLeftButtonDown;
                //会议室图例点击事件
                this.ConferenceRoom_View.ItemClickSendItemCallBack = ItemClick_DisposeSourceAndInitAgain;
                //工具箱子项更改事件
                this.ToolCmWindow.SelectItemChangedCallBack = ToolCmWindow_SelectItemChanged;
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
        /// 父类事件注册
        /// </summary>
        private void EventRegedit_Parent()
        {
            try
            {
                //教育模式
                base.ViewModelChangedEducationCallBack = MainPage_ViewModelChangedEducationEvent;
                //精简模式
                base.ViewModelChangedSimpleCallBack = MainPage_ViewModelChangedSimpleEvent;
                //页面切换
                base.ForceToNavicateCallBack = MainPage_ForceToNavicateEvent;
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
        /// 精简模式事件注册
        /// </summary>
        private void EventRegedit_Simple()
        {
            try
            {
                //会议信息（精简模式）
                this.btnMeet2.Click += Navacite;
                //信息交流
                this.btnIMM2.Click += Navacite;
                //共享资源
                this.btnResource2.Click += Navacite;
                //智存空间
                this.btnSpace2.Click += Navacite;
                //系统设置（精简模式有）
                this.btnSetting.Click += Navacite;
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
        /// 教育模式事件注册
        /// </summary>
        private void EventRegedit_Education()
        {
            try
            {
                //我的课堂
                this.btnMeetEducation.Click += Navacite;
                //互动课堂
                this.btnResourceEducation.Click += Navacite;
                //教学资料
                this.btnSpaceEducation.Click += Navacite;
                //课程大纲
                this.btnTreeEducation.Click += Navacite;
                //系统设置（教学模式）
                this.btnSettingEducation.Click += Navacite;
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

        #region 父类事件

        /// <summary>
        /// 页面切换
        /// </summary>
        /// <param name="selectedItem">页面选择类型</param>
        void MainPage_ForceToNavicateEvent(ViewSelectedItemEnum selectedItem)
        {
            try
            {
                //强制切换到指定视图
                this.ForceToNavicate(selectedItem);
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
        /// 精简模式
        /// </summary>
        /// <param name="result">是否为精简模式</param>
        void MainPage_ViewModelChangedSimpleEvent(bool result)
        {
            try
            {
                //更改为精简视图
                this.ViewModelChangedSimple(result);
                //首页设置为精简视图
                Index.index.ViewModelChangedSimple(result);
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
        /// 教育模式
        /// </summary>
        /// <param name="result">是否为教育模式</param>
        void MainPage_ViewModelChangedEducationEvent(bool result)
        {
            try
            {
                //显示为教育视图
                this.ViewModelChangedEducation(result);
                //首页设置为教育视图
                Index.index.ViewModelChangedEducation(result);
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

        #region 导航到指定视图

        /// <summary>
        /// 导航到指定视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Navacite(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.NavicateView(navicateButton.ViewSelectedItemEnum);
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
        /// 导航到指定视图
        /// </summary>
        /// <param name="viewSelectedItemEnum">页面选择类型</param>
        public void NavicateView(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //前期处理
                this.DealWithBefore(viewSelectedItemEnum);

                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.Meet:
                        this.DealWithMeet();
                        break;

                    case ViewSelectedItemEnum.Tree:
                        this.DealWithTree();
                        break;

                    case ViewSelectedItemEnum.Space:
                        this.DealWidthSpace();
                        break;

                    case ViewSelectedItemEnum.Resource:
                        this.DealWidthResource();
                        break;

                    case ViewSelectedItemEnum.IMM:
                        this.DealWidthIMM();
                        break;

                    case ViewSelectedItemEnum.PersonNote:
                        this.DealWidthPersonNote();
                        break;

                    case ViewSelectedItemEnum.WebBrowserView:
                        //导航样式更改
                        this.ButtonStyleChanged(this.btnVote);
                        //加载会议投票
                        this.borMain.Child = this.WebBrowserView;
                        break;

                    case ViewSelectedItemEnum.U_Disk:
                        //U盘传输
                        //this.borMain.Child = this.U_DiskView;
                        break;

                    case ViewSelectedItemEnum.Meet_Change:
                        break;

                    case ViewSelectedItemEnum.Chair:
                        //加载主持人功能
                        this.borMain.Child = this.ChairView;
                        break;

                    case ViewSelectedItemEnum.Studiom:
                        //中控设置
                        //this.borMain.Child = this.StudiomView;
                        break;

                    case ViewSelectedItemEnum.SystemSetting:
                        this.DealWidthSystemSetting();
                        break;

                    case ViewSelectedItemEnum.Tool:
                        //工具箱模块处理
                        this.DealWithTool();
                        break;

                    case ViewSelectedItemEnum.ToolUsing:
                        //工具应用
                        this.borMain.Child = this.ApplicationToolView;
                        break;

                    default:
                        break;
                }
                //绑定选择项
                this.ViewSelectedItemEnum = viewSelectedItemEnum;
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
        /// 前期处理
        /// </summary>
        /// <param name="viewSelectedItemEnum">页面选择类型</param>
        public void DealWithBefore(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //若为工具箱或者第一选中,则返回
                if (viewSelectedItemEnum == ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Tool && this.ToolCmWindow.FirstSelected)
                {
                    return;
                }
                else
                {
                    if (LyncHelper.MainConversation != null)
                    {
                        //避免批注之类的影响视图，进行显示的切换
                        if (viewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource)
                        {
                            //设置会话区域显示内容
                            this.ConversationM.SetConversationAreaShow(ShowType.HidenView, false);
                        }
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

        /// <summary>
        /// 我的会议处理
        /// </summary>
        public void DealWithMeet()
        {
            try
            {
                //导航按钮
                NavicateButton navicateButton = null;

                if (this.isEdacationModel)
                {
                    navicateButton = this.btnMeetEducation;
                }
                else if (this.isSimpleModel)
                {
                    //导航样式更改
                    navicateButton = this.btnMeet2;
                }
                else if (this.isNormalModel)
                {
                    //导航样式更改
                    navicateButton = this.btnMeet;
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //加载我的会议
                this.borMain.Child = this.MyConferenceView;
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
        /// 知识树处理
        /// </summary>
        public void DealWithTree()
        {
            try
            {
                //导航按钮
                NavicateButton navicateButton = null;
                if (this.isEdacationModel)
                {
                    //导航样式更改
                    navicateButton = this.btnTreeEducation;
                }
                else
                {
                    //导航样式更改
                    navicateButton = this.btnTree;
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //跳转到知识树
                this.borMain.Child = this.ConferenceTreeView;
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
        /// 智存空间处理
        /// </summary>
        public void DealWidthSpace()
        {
            try
            {
                //导航按钮
                NavicateButton navicateButton = null;
                if (this.isEdacationModel)
                {
                    //导航样式更改
                    navicateButton = this.btnSpaceEducation;
                }
                else if (this.isSimpleModel)
                {
                    //导航样式更改
                    navicateButton = this.btnSpace2;
                }
                else if (this.isNormalModel)
                {
                    //导航样式更改
                    navicateButton = this.btnSpace;
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //跳转到会议空间
                this.borMain.Child = this.SpaceView;
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
        /// 共享协作处理
        /// </summary>
        public void DealWidthResource()
        {
            try
            {
                //导航样式更改
                NavicateButton navicateButton = null;
                if (this.isEdacationModel)
                {
                    //导航按钮
                    navicateButton = this.btnResourceEducation;
                    //隐藏IMM提示
                    this.borResourceTipEducation.Visibility = vy.Hidden;
                }
                else if (this.isSimpleModel)
                {
                    //导航样式更改
                    navicateButton = this.btnResource2;
                    //隐藏IMM提示
                    this.borResourceTip2.Visibility = vy.Hidden;
                }
                else if (this.isNormalModel)
                {
                    //导航样式更改
                    navicateButton = this.btnResource;
                    //隐藏IMM提示
                    this.borResourceTip.Visibility = vy.Hidden;
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //跳转到资源共享
                this.borMain.Child = this.ConversationM;
                //设置会话区域显示内容
                this.ConversationM.SetConversationAreaShow(ShowType.ConversationView, false);
                //停止计时器工作
                if (base.ResourceFlashTimer != null)
                {
                    base.ResourceFlashTimer.Stop();
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
        /// 信息交流处理
        /// </summary>
        public void DealWidthIMM()
        {
            try
            {
                NavicateButton navicateButton = null;
                if (this.isSimpleModel)
                {
                    //导航样式更改
                    navicateButton = this.btnIMM2;
                    //隐藏IMM提示
                    this.borImmTip2.Visibility = vy.Hidden;
                }
                else
                {
                    //导航样式更改
                    navicateButton = this.btnIMM;
                    //隐藏IMM提示
                    this.borImmTip.Visibility = vy.Hidden;
                }
                //停止计时器工作
                if (base.IMMFlashTimer != null)
                {
                    base.IMMFlashTimer.Stop();
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //跳转到信息交流
                this.borMain.Child = this.ConferenceAudio_View;
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
        /// 系统设置处理
        /// </summary>
        public void DealWidthSystemSetting()
        {
            try
            {
                NavicateButton navicateButton = null;
                if (this.isEdacationModel)
                {
                    //导航样式更改
                    navicateButton = this.btnSettingEducation;
                }
                else if (this.isSimpleModel)
                {
                    //导航样式更改
                    navicateButton = this.btnSetting;
                }
                //导航样式更改
                this.ButtonStyleChanged(navicateButton);
                //跳转到系统设置
                this.borMain.Child = this.Setting_View;
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
        /// 个人笔记处理
        /// </summary>
        public void DealWidthPersonNote()
        {
            try
            {
                //导航样式更改
                this.ButtonStyleChanged(this.btnNote);
                //加载个人笔记
                this.borMain.Child = this.PersonalNote;
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

        #region 模式判断

        /// <summary>
        /// 查看是否为教育模式（持久缓存）
        /// </summary>
        /// <returns></returns>
        public bool IsEdacationModel()
        {
            try
            {
                //查看是否为教育模式（持久缓存）
                if (Constant.TempConferenceInformationEntity != null && Constant.TempConferenceInformationEntity.EducationMode)
                {
                    isEdacationModel = true;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return isEdacationModel;
        }

        /// <summary>
        /// 查看是否为精简模式（持久缓存）
        /// </summary>
        /// <returns></returns>
        public bool IsSimpleModel()
        {
            try
            {
                //查看是否为精简模式（持久缓存）
                if (Constant.TempConferenceInformationEntity != null && !Constant.TempConferenceInformationEntity.EducationMode)
                {
                    if (Constant.TempConferenceInformationEntity.SimpleMode)
                    {
                        this.isSimpleModel = true;
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
            return isSimpleModel;
        }

        /// <summary>
        /// 查看是否为标准模式（持久缓存）
        /// </summary>
        /// <returns></returns>
        public bool IsNormalModel()
        {
            try
            {
                //查看是否为标准模式（持久缓存）
                if (Constant.TempConferenceInformationEntity != null && !Constant.TempConferenceInformationEntity.EducationMode)
                {
                    if (!Constant.TempConferenceInformationEntity.SimpleMode)
                    {
                        isNormalModel = true;
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
            return isNormalModel;
        }

        #endregion

        #region 检测网络异常回调

        /// <summary>
        /// 检测网络异常回调
        /// </summary>
        /// <param name="netWorkErrTipType">异常类型</param>
        public void CheckNetWorkCallBack(NetWorkErrTipType netWorkErrTipType)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //信息提示
                        string message = string.Empty;
                        if (this.currentNetWorkState != netWorkErrTipType)
                        {
                            switch (netWorkErrTipType)
                            {
                                case NetWorkErrTipType.Normal:
                                    //网络连接标准
                                    this.NetWork_Normal();
                                    break;

                                case NetWorkErrTipType.ConnectedRouteFailed:
                                    message = "无法连接到当前网络";
                                    //连接失败
                                    this.NetWork_ConnectedFailed(message);

                                    break;
                                case NetWorkErrTipType.ConnectedServiceFailed:
                                    message = "无法连接到服务器";
                                    //连接失败
                                    this.NetWork_ConnectedFailed(message);

                                    break;
                                case NetWorkErrTipType.ConnectedWebServiceFailed:
                                    message = "无法访问web服务";
                                    //连接失败
                                    this.NetWork_ConnectedFailed(message);

                                    break;
                                default:
                                    break;
                            }
                        }
                        //当前网络状态
                        this.currentNetWorkState = netWorkErrTipType;
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
        /// web服务连接失败
        /// </summary>
        /// <param name="message">连接异常具体信息</param>
        /// <returns></returns>
        private void NetWork_ConnectedFailed(string message)
        {
            try
            {
                //共享面板
                SharingPanel panel2 = MainPage.mainPage.SharingPanel;
                //导航到首页
                MainWindow.mainWindow.IndexPageChangedToIndexPage();
                //显示网络异常视图
                MainWindow.mainWindow.NetWork_ViewVisibility = vy.Visible;
                //提示信息设置
                MainWindow.mainWindow.SettingNetWorkConnectErrorTip(message);
             
                //共享漫步异常信息设置
                panel2.SettingNetWorkConnectErrorTip(message);
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
        /// 网络连接视图——标准
        /// </summary>
        private void NetWork_Normal()
        {
            try
            {
                //主界面 网络异常隐藏
                MainWindow.mainWindow.NetWork_ViewVisibility = vy.Collapsed;
                //共享面板 网络异常隐藏
                MainPage.mainPage.SharingPanel.NetWork_ViewVisibility = vy.Collapsed;
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
