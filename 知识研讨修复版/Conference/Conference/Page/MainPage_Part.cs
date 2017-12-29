using Conference_Tree;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using vy = System.Windows.Visibility;

namespace Conference.Page
{
    public partial class MainPage : MainPageBase
    {
        #region 工具箱模块处理

        /// <summary>
        /// 工具箱模块处理
        /// </summary>
        public void DealWithTool()
        {
            try
            {
                //判断是否为第一次选择
                if (this.ToolCmWindow.FirstSelected)
                {
                    //工具箱高级面板显示设置
                    this.ToolPopupDisplay();

                    //防止反复注册
                    this.borTool.PreviewMouseLeftButtonDown -= borTool_PreviewMouseLeftButtonDown;
                    //工具箱右上角图标点击事件
                    this.borTool.PreviewMouseLeftButtonDown += borTool_PreviewMouseLeftButtonDown;

                }
                else
                {
                    //导航样式更改
                    this.ButtonStyleChanged(this.btnTool);
                    //导航到指定视图
                    this.NavicateView(this.ToolCmWindow.ViewSelectedItemEnum);
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

        #region 页面刷新

        /// <summary>
        /// 页面刷新
        /// </summary>
        /// <param name="viewSelectedItemEnum">页面选项</param>
        public void PageReflesh(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.Tree:
                        //知识树刷新
                        this.Tree_Reflesh();
                        break;

                    case ViewSelectedItemEnum.Space:
                        //智存空间刷新
                        this.Space_Reflesh();
                        break;

                    case ViewSelectedItemEnum.IMM:
                        //信息交流刷新
                        this.IMM_Reflesh();

                        break;

                    case ViewSelectedItemEnum.PersonNote:
                        //个人笔记刷新
                        this.Note_Reflesh();
                        break;


                    case ViewSelectedItemEnum.WebBrowserView:
                        //网络浏览刷新
                        this.WebBrowser_Reflesh();
                        break;

                    case ViewSelectedItemEnum.Chair:
                        //主持人页面刷新
                        this.Chair_Reflesh();

                        break;

                    case ViewSelectedItemEnum.SystemSetting:
                        //加载系统设置
                        this.borMain.Child = this.Setting_View;
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

        /// <summary>
        /// 主持人功能页面刷新
        /// </summary>
        private void Chair_Reflesh()
        {
            try
            {
                if (this.chairView != null)
                {
                    //释放主持人页面
                    this.chairView = null;
                }
                //加载主持人功能
                this.borMain.Child = this.ChairView;
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
        /// 网络浏览页面刷新
        /// </summary>
        private void WebBrowser_Reflesh()
        {
            try
            {
                //是否会议投票页面
                base.webBrowserView = null;
                //加载会议投票
                this.borMain.Child = this.WebBrowserView;
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
        /// 个人笔记页面刷新
        /// </summary>
        private void Note_Reflesh()
        {
            try
            {
                //清空个人笔记
                base.personalNote = null;
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

        /// <summary>
        /// 智存空间页面刷新
        /// </summary>
        private void Space_Reflesh()
        {
            try
            {
                //清空智存空间
                base.spaceView = null;
                //加载智存空间
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
        /// 信息交流页面刷新
        /// </summary>
        private void IMM_Reflesh()
        {
            try
            {
                //释放IMM页面   
                if (base.conferenceAudio_View != null)
                {
                    base.conferenceAudio_View.SessionClear();
                }
                base.conferenceAudio_View = null;
                //加载IMM
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
        /// 知识树刷新
        /// </summary>
        private void Tree_Reflesh()
        {
            try
            {
                if (base.conferenceTreeView != null)
                {
                    //释放知识树页面         
                    base.conferenceTreeView.ConferenceTreeItem = null;
                    base.conferenceTreeView = null;
                }
                ConferenceTreeItem.SessionClear();
                //加载树视图
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


        #endregion

        #region 工具箱右上角图标点击事件

        /// <summary>
        /// 工具箱右上角图标点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void borTool_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //工具箱高级面板显示设置
                this.ToolPopupDisplay();
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

        #region 工具箱子项更改事件

        /// <summary>
        /// 工具箱子项更改事件
        /// </summary>
        /// <param name="viewSelectedItemEnum">页面选项</param>
        void ToolCmWindow_SelectItemChanged(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //判断工具箱右上角的图标状态
                if (this.borToolMain.Visibility == vy.Collapsed)
                {
                    //右上角图标显示（工具箱）
                    this.borToolMain.Visibility = vy.Visible;
                }
                //工具箱图标
                this.borTool.Background = this.brush_Tool2;

                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.ToolUsing:
                        //图标换为U盘传输
                        this.btnTool.Background = this.brush_U_Disk1;
                        //标题更改
                        this.btnTool.Content = " 工 具 应 用 ";
                        break;

                    case ViewSelectedItemEnum.Meet_Change:
                        //图标换为会议切换
                        this.btnTool.Background = this.brush_Meet_Change1;
                        //标题更改
                        this.btnTool.Content = "会 议 切 换";
                        break;

                    case ViewSelectedItemEnum.Chair:
                        //图标换为主持人功能
                        this.btnTool.Background = this.brush_Chair1;
                        //标题更改
                        this.btnTool.Content = "主 持 功 能";
                        break;

                    case ViewSelectedItemEnum.Studiom:
                        //图标换为中控功能
                        this.btnTool.Background = this.brush_Studiom1;
                        //标题更改
                        this.btnTool.Content = "中 控 功 能";
                        break;

                    case ViewSelectedItemEnum.SystemSetting:
                        //图标换为系统设置
                        this.btnTool.Background = this.brush_Setting1;
                        //标题更改
                        this.btnTool.Content = "系 统 设 置";
                        break;

                    default:
                        break;
                }
                this.ButtonStyleChanged(this.btnTool);
                //导航到指定页面
                this.NavicateView(viewSelectedItemEnum);
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

        #region 消息提示（闪烁）

        /// <summary>
        /// 消息提示（闪烁）
        /// </summary>
        public void IMMReceivMessageFlash()
        {
            try
            {
                //停止计时器工作
                if (this.IMMFlashTimer != null)
                {
                    this.IMMFlashTimer.Stop();
                }

                //闪烁次数
                int flashCount = 0;

                TimerJob.StartRun(new Action(() =>
                {
                    //闪烁次数递增
                    flashCount++;

                    //标准模式
                    if (!this.isSimpleModel)
                    {
                        //显示状态转为隐藏                       
                        this.ShowOrDisplay(this.borImmTip);
                        if (flashCount >= 8)
                        {
                            this.borImmTip.Visibility = vy.Visible;
                            //停止计时器工作
                            this.IMMFlashTimer.Stop();
                        }
                    }
                    else
                    {
                        //显示状态转为隐藏                       
                        this.ShowOrDisplay(this.borImmTip2);
                        if (flashCount >= 8)
                        {
                            this.borImmTip2.Visibility = vy.Visible;
                            //停止计时器工作
                            this.IMMFlashTimer.Stop();
                        }
                    }


                }), 500, out this.IMMFlashTimer);
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
        /// 资源共享提示（闪烁）
        /// </summary>
        public void ResourceReceivMessageFlash()
        {
            try
            {
                //停止计时器工作
                if (this.ResourceFlashTimer != null)
                {
                    this.ResourceFlashTimer.Stop();
                }
                //闪烁次数
                int flashCount = 0;

                TimerJob.StartRun(new Action(() =>
                {
                    //闪烁次数递增
                    flashCount++;

                    //标准模式
                    if (this.isNormalModel)
                    {
                        //显示状态转为隐藏                       
                        this.ShowOrDisplay(this.borResourceTip);
                        if (flashCount >= 8)
                        {
                            this.borResourceTip.Visibility = vy.Visible;
                            //停止计时器工作
                            this.ResourceFlashTimer.Stop();
                        }
                    }
                    else if (this.isSimpleModel)
                    {
                        //精简模式
                        //显示状态转为隐藏                           
                        this.ShowOrDisplay(this.borResourceTip2);
                        if (flashCount >= 8)
                        {
                            this.borResourceTip2.Visibility = vy.Visible;
                            //停止计时器工作
                            this.ResourceFlashTimer.Stop();
                        }
                    }
                    else if (this.isEdacationModel)
                    {
                        //教学模式
                        //显示状态转为隐藏                       
                        this.ShowOrDisplay(this.borResourceTipEducation);
                        if (flashCount >= 8)
                        {
                            this.borResourceTipEducation.Visibility = vy.Visible;
                            //停止计时器工作
                            this.ResourceFlashTimer.Stop();
                        }
                    }

                }), 500, out this.ResourceFlashTimer);
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

        #region 导航样式更改

        /// <summary>
        /// 导航样式更改
        /// </summary>
        /// <param name="btn">导航按钮</param>
        protected void ButtonStyleChanged(NavicateButton btn)
        {
            try
            {
                if (btn != null)
                {
                    //若选中的为当前标记的则不进行样式变更
                    if (btn.Equals(base.beforeSelectedButton))
                    {
                        return;
                    }

                    if (btn.IntType == 1)
                    {
                        //样式更改1
                        btn.Style = this.btnSyle1_Selected;
                    }
                    //当前按钮为导航区域类型2
                    else if (btn.IntType == 2)
                    {
                        //样式更改2
                        btn.Style = this.btnSyle2_Selected;
                    }

                    //将之前选中的导航按钮进行样式恢复
                    if (base.beforeSelectedButton != null)
                    {
                        //类型转换
                        int intTag = base.beforeSelectedButton.IntType;
                        //之前选中的按钮样式还原
                        if (intTag == 1)
                        {
                            //样式更改1
                            base.beforeSelectedButton.Style = this.btnSyle1;
                        }
                        //之前选中的按钮样式还原
                        else if (intTag == 2)
                        {
                            //样式更改2
                            base.beforeSelectedButton.Style = this.btnSyle2;
                        }
                    }
                    //当前选中按钮变更
                    base.beforeSelectedButton = btn;
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

        #region 强制打开并显示某个导航

        /// <summary>
        ///  强制打开并显示某个导航
        /// </summary>
        /// <param name="viewSelectedItemEnum">页面选择类型</param>
        public void ForceToNavicate(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //首页切换(MainPage)
                MainWindow.mainWindow.IndexPageChangedToMainPage();
                //如果不为当前页面,则进行强制导航
                if (this.ViewSelectedItemEnum != viewSelectedItemEnum)
                {
                    //导航到指定页面
                    this.NavicateView(viewSelectedItemEnum);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 获取工作区域的尺寸

        /// <summary>
        /// 获取工作区域的宽度
        /// </summary>
        public int GetWorkingArea_Width()
        {
            int width = 0;
            try
            {
                //获取宽度
                width = (int)this.borMain.ActualWidth;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return width;
        }

        /// <summary>
        /// 获取工作区域的高度
        /// </summary>
        public int GetWorkingArea_Height()
        {
            int height = 0;
            try
            {
                //获取高度
                //height = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                height = (int)this.borMain.ActualHeight;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return height;
        }

        #endregion

        #region 模式切换

        /// <summary>
        /// 精简与标准模式
        /// </summary>
        /// <param name="isSampleModel">是否为精简模式</param>
        public void ViewModelChangedSimple(bool isSampleModel)
        {
            try
            {
                if (isSampleModel)
                {
                    //显示精简模式视图
                    this.gridSampleModelView.Visibility = vy.Visible;
                    //隐藏标准模式视图
                    this.gridStandardModelView.Visibility = vy.Collapsed;
                }
                else
                {
                    //显示精简模式视图
                    this.gridSampleModelView.Visibility = vy.Collapsed;
                    //隐藏标准模式视图
                    this.gridStandardModelView.Visibility = vy.Visible;
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
        /// 教育模式
        /// </summary>
        /// <param name="isEducationModel">是否为教育模式</param>
        public void ViewModelChangedEducation(bool isEducationModel)
        {
            try
            {
                MainWindow window = MainWindow.mainWindow;
                if (isEducationModel)
                {
                    this.gridEducationModelView.Visibility = vy.Visible;
                    //主窗体标题2显示
                    window.MainWindowHeader2 = Constant.EducationTittle1 + Constant.ConferenceName;
                    //设置主窗体标题2
                    window.MainWindowHeader3 = "     " + Constant.EducationTittle2 + Constant.SelfName;
                }
                else
                {
                    this.gridEducationModelView.Visibility = vy.Collapsed;
                    //主窗体标题2显示
                    window.MainWindowHeader2 = Constant.DiscussTittle1 + Constant.ConferenceName;
                    //设置主窗体标题2
                    window.MainWindowHeader3 = "     " + Constant.DiscussTittle2 + Constant.SelfName;
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

        #region 隐藏或显示

        /// <summary>
        /// 隐藏或显示
        /// </summary>
        public void ShowOrDisplay(FrameworkElement element)
        {
            try
            {
                //显示状态转为隐藏
                if (element.Visibility == vy.Visible)
                {
                    element.Visibility = vy.Hidden;
                }
                else
                {
                    element.Visibility = vy.Visible;
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
