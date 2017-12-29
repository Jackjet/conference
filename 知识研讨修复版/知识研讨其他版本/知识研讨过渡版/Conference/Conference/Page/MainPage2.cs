using Conference_Tree;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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
        public void PageReflesh(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.Meet:

                        #region old solution
                        
                        ////释放我的会议页面
                        //base.myConferenceView = null;
                        ////进入我的会议
                        //this.borMain.Child = this.MyConferenceView;

                        //#region 位置信息获取并设置

                        //ModelManage.ConferenceMatrix.IntoOneSeat(Constant.ConferenceName, TempConferenceInformationEntity.SettingIpList, Constant.SelfName, Constant.LocalIp, new Action<bool, List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity>>((successed, seatEntityList) =>
                        //{
                        //    if (successed)
                        //    {
                        //        //刷新座位分布情况
                        //        this.MyConferenceView.Reflesh(seatEntityList);
                        //    }
                        //}));

                        //#endregion
                        
                        #endregion

                        break;

                    case ViewSelectedItemEnum.Tree:
                        if (base.conferenceTreeView != null)
                        {
                            //释放知识树页面         
                            base.conferenceTreeView.ConferenceTreeItem = null;
                            base.conferenceTreeView = null;
                        }
                        ConferenceTreeItem.SessionClear();
                        //加载树视图
                        this.borMain.Child = this.ConferenceTreeView;
                        break;

                    case ViewSelectedItemEnum.Space:
                        //清空智存空间
                        base.spaceView = null;
                        //加载智存空间
                        this.borMain.Child = this.SpaceView;
                        break;

                    case ViewSelectedItemEnum.Resource:


                        break;

                    case ViewSelectedItemEnum.IMM:
                        //this.RefleshBlockConnect(ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceAudio, new Action(() =>
                        //    {
                                //释放IMM页面   
                                if (base.conferenceAudio_View != null)
                                {
                                    base.conferenceAudio_View.SessionClear();
                                }
                                base.conferenceAudio_View = null;
                                //加载IMM
                                this.borMain.Child = this.ConferenceAudio_View;
                            //}));

                        break;

                    case ViewSelectedItemEnum.PersonNote:
                        //清空个人笔记
                        base.personalNote = null;
                        //加载个人笔记
                        this.borMain.Child = this.PersonalNote;
                        break;


                    case ViewSelectedItemEnum.WebBrowserView:
                        //是否会议投票页面
                        base.webBrowserView = null;
                        //加载会议投票
                        this.borMain.Child = this.WebBrowserView;
                        break;

                    case ViewSelectedItemEnum.U_Disk:
                        //释放U盘传输页面
                        this.u_DiskView = null;
                        //加载U盘传输
                        this.borMain.Child = this.U_DiskView;
                        break;

                    case ViewSelectedItemEnum.Meet_Change:
                        break;

                    case ViewSelectedItemEnum.Chair:
                        if (this.chairView != null)
                        {
                            //释放主持人页面
                            this.chairView = null;
                        }
                        //加载主持人功能
                        this.borMain.Child = this.ChairView;

                        #region 位置信息获取并设置

                        //ModelManage.ConferenceMatrix.IntoOneSeat(Constant.ConferenceName, TempConferenceInformationEntity.SettingIpList, Constant.SelfName, Constant.LocalIp, new Action<bool, List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity>>((successed, seatEntityList) =>
                        //{
                        //    if (successed)
                        //    {
                        //        //刷新座位分布情况
                        //        this.ChairView.Reflesh(seatEntityList);
                        //    }
                        //}));

                        #endregion


                        break;

                    case ViewSelectedItemEnum.Studiom:
                        //加载中控功能
                        this.borMain.Child = this.StudiomView;
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
                //导航样式更改
                //this.ButtonStyleChanged(this.btnTool);

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
        /// <param name="viewSelectedItemEnum"></param>
        void ToolCmWindow_SelectItemChanged(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //判断工具箱右上角的图标状态
                if (this.borToolMain.Visibility == System.Windows.Visibility.Collapsed)
                {
                    //右上角图标显示（工具箱）
                    this.borToolMain.Visibility = System.Windows.Visibility.Visible;
                }
                switch (viewSelectedItemEnum)
                {
                    case ViewSelectedItemEnum.ToolUsing:
                        //工具箱图标切换为U盘传输
                        this.borTool.Background = Application.Current.Resources["brush_Tool2"] as ImageBrush;
                        //图标换为U盘传输
                        this.btnTool.Background = Application.Current.Resources["brush_U_Disk1"] as ImageBrush;
                        //标题更改
                        this.btnTool.Content = " 工 具 应 用 ";
                        break;

                    case ViewSelectedItemEnum.Meet_Change:
                        //工具箱图标切换为会议切换
                        //this.borTool.Background = Application.Current.Resources["brush_Meet_Change2"] as ImageBrush;
                        this.borTool.Background = Application.Current.Resources["brush_Tool2"] as ImageBrush;
                        //图标换为会议切换
                        this.btnTool.Background = Application.Current.Resources["brush_Meet_Change1"] as ImageBrush;
                        //标题更改
                        this.btnTool.Content = "会 议 切 换";
                        break;

                    case ViewSelectedItemEnum.Chair:
                        //工具箱图标切换为主持人功能
                        //this.borTool.Background = Application.Current.Resources["brush_Chair2"] as ImageBrush;
                        this.borTool.Background = Application.Current.Resources["brush_Tool2"] as ImageBrush;
                        //图标换为主持人功能
                        this.btnTool.Background = Application.Current.Resources["brush_Chair1"] as ImageBrush;
                        //标题更改
                        this.btnTool.Content = "主 持 功 能";
                        break;

                    case ViewSelectedItemEnum.Studiom:
                        //工具箱图标切换为中控功能
                        //this.borTool.Background = Application.Current.Resources["brush_Studiom2"] as ImageBrush;
                        this.borTool.Background = Application.Current.Resources["brush_Tool2"] as ImageBrush;
                        //图标换为中控功能
                        this.btnTool.Background = Application.Current.Resources["brush_Studiom1"] as ImageBrush;
                        //标题更改
                        this.btnTool.Content = "中 控 功 能";
                        break;

                    case ViewSelectedItemEnum.SystemSetting:
                        //工具箱图标切换为系统设置
                        //this.borTool.Background = Application.Current.Resources["brush_Setting2"] as ImageBrush;
                        this.borTool.Background = Application.Current.Resources["brush_Tool2"] as ImageBrush;
                        //图标换为系统设置
                        this.btnTool.Background = Application.Current.Resources["brush_Setting1"] as ImageBrush;
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
                    if (!TempConferenceInformationEntity.SimpleMode)
                    {
                        //显示状态转为隐藏
                        if (this.borImmTip.Visibility == System.Windows.Visibility.Visible)
                        {
                            this.borImmTip.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            this.borImmTip.Visibility = System.Windows.Visibility.Visible;
                        }
                        if (flashCount >= 8)
                        {
                            this.borImmTip.Visibility = System.Windows.Visibility.Visible;
                            //停止计时器工作
                            this.IMMFlashTimer.Stop();
                        }
                    }
                    else
                    {
                        //显示状态转为隐藏
                        if (this.borImmTip2.Visibility == System.Windows.Visibility.Visible)
                        {
                            this.borImmTip2.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            this.borImmTip2.Visibility = System.Windows.Visibility.Visible;
                        }
                        if (flashCount >= 8)
                        {
                            this.borImmTip2.Visibility = System.Windows.Visibility.Visible;
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

                    if (!TempConferenceInformationEntity.EducationMode)
                    {

                        //标准模式
                        if (!TempConferenceInformationEntity.SimpleMode)
                        {
                            //显示状态转为隐藏
                            if (this.borResourceTip.Visibility == System.Windows.Visibility.Visible)
                            {
                                this.borResourceTip.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                this.borResourceTip.Visibility = System.Windows.Visibility.Visible;
                            }
                            if (flashCount >= 8)
                            {
                                this.borResourceTip.Visibility = System.Windows.Visibility.Visible;
                                //停止计时器工作
                                this.ResourceFlashTimer.Stop();
                            }
                        }
                        else
                        {
                            //精简模式
                            //显示状态转为隐藏
                            if (this.borResourceTip2.Visibility == System.Windows.Visibility.Visible)
                            {
                                this.borResourceTip2.Visibility = System.Windows.Visibility.Hidden;
                            }
                            else
                            {
                                this.borResourceTip2.Visibility = System.Windows.Visibility.Visible;
                            }
                            if (flashCount >= 8)
                            {
                                this.borResourceTip2.Visibility = System.Windows.Visibility.Visible;
                                //停止计时器工作
                                this.ResourceFlashTimer.Stop();
                            }
                        }
                    }
                    else
                    {
                        //教学模式
                        //显示状态转为隐藏
                        if (this.borResourceTipEducation.Visibility == System.Windows.Visibility.Visible)
                        {
                            this.borResourceTipEducation.Visibility = System.Windows.Visibility.Hidden;
                        }
                        else
                        {
                            this.borResourceTipEducation.Visibility = System.Windows.Visibility.Visible;
                        }
                        if (flashCount >= 8)
                        {
                            this.borResourceTipEducation.Visibility = System.Windows.Visibility.Visible;
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
        protected void ButtonStyleChanged(Button btn)
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
                    //将当前导航按钮进行样式更改
                    if (btn.Tag != null)
                    {
                        //类型转换
                        int intTag = Convert.ToInt32(btn.Tag);
                        //当前按钮为导航区域类型1
                        if (intTag == 1)
                        {
                            //样式更改1
                            btn.Style = this.Resources["btnSyle1_Selected"] as Style;
                        }
                        //当前按钮为导航区域类型2
                        else if (intTag == 2)
                        {
                            //样式更改2
                            btn.Style = this.Resources["btnSyle2_Selected"] as Style;
                        }
                    }


                    //将之前选中的导航按钮进行样式恢复
                    if (base.beforeSelectedButton != null && base.beforeSelectedButton.Tag != null)
                    {
                        //类型转换
                        int intTag = Convert.ToInt32(base.beforeSelectedButton.Tag);
                        //之前选中的按钮样式还原
                        if (intTag == 1)
                        {
                            //样式更改1
                            base.beforeSelectedButton.Style = this.Resources["btnSyle1"] as Style;
                        }
                        //之前选中的按钮样式还原
                        else if (intTag == 2)
                        {
                            //样式更改2
                            base.beforeSelectedButton.Style = this.Resources["btnSyle2"] as Style;
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
        public void ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum viewSelectedItemEnum)
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
                //width = (int)System.Windows.SystemParameters.PrimaryScreenWidth;  
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
        public void ViewModelChangedSimple(bool isSampleModel)
        {
            try
            {
                if (isSampleModel)
                {
                    this.gridSampleModelView.Visibility = System.Windows.Visibility.Visible;
                    this.gridStandardModelView.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.gridSampleModelView.Visibility = System.Windows.Visibility.Collapsed;
                    this.gridStandardModelView.Visibility = System.Windows.Visibility.Visible;
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
        /// 是否为教育模式
        /// </summary>
        /// <param name="isEducationModel"></param>
        public void ViewModelChangedEducation(bool isEducationModel)
        {
            try
            {
                if (isEducationModel)
                {
                    this.gridEducationModelView.Visibility = System.Windows.Visibility.Visible;
                    //主窗体标题2显示
                    MainWindow.mainWindow.MainWindowHeader2 = Constant.EducationTittle1 + Constant.ConferenceName;
                    //设置主窗体标题2
                    MainWindow.mainWindow.MainWindowHeader3 = "     " + Constant.EducationTittle2 + Constant.SelfName;
                }
                else
                {
                    this.gridEducationModelView.Visibility = System.Windows.Visibility.Collapsed;
                    //主窗体标题2显示
                    MainWindow.mainWindow.MainWindowHeader2 = Constant.DiscussTittle1 + Constant.ConferenceName;
                    //设置主窗体标题2
                    MainWindow.mainWindow.MainWindowHeader3 = "     " + Constant.DiscussTittle2 + Constant.SelfName;
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
