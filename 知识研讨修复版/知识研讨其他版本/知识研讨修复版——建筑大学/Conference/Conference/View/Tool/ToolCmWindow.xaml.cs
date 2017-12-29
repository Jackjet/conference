using ConferenceCommon.EnumHelper;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Conference.View.Tool
{
    /// <summary>
    /// ToolCmWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ToolCmWindow : Window
    {
        #region 自定义委托事件

        public delegate void SelectItemChangedEventHandle(ViewSelectedItemEnum viewSelectedItemEnum);
        /// <summary>
        /// 子项选择事件
        /// </summary>
        public event SelectItemChangedEventHandle SelectItemChanged = null;

        #endregion

        #region 属性

        bool firstSelected = true;
        /// <summary>
        /// 第一次选择
        /// </summary>
        public bool FirstSelected
        {
            get { return firstSelected; }
            set { firstSelected = value; }
        }

        ViewSelectedItemEnum viewSelectedItemEnum;
        /// <summary>
        /// 工具箱选择项
        /// </summary>
        public ViewSelectedItemEnum ViewSelectedItemEnum
        {
            get { return viewSelectedItemEnum; }
            set { viewSelectedItemEnum = value; }
        }

        #endregion

        #region 字段

        static double windowTop = SystemParameters.WorkArea.Height - 188 + 40;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ToolCmWindow()
        {
            try
            {
                //UI加载
                InitializeComponent();

                #region 事件注册

                //U盘传输
                this.btn_U_Disk.Click += btn_Navicate;
                //会议切换
                this.btn_Meet_Changed.Click += btn_Navicate;
                //系统设置
                this.btn_Setting.Click += btn_Navicate;
                //中控功能
                this.btn_Studiom.Click += btn_Navicate;
                //主持人功能
                this.btn_Chair.Click += btn_Navicate;

                this.ContentRendered += ToolCmWindow_ContentRendered;

                #endregion

                //初始化y坐标位置
                this.Top = SystemParameters.WorkArea.Height;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
                //System.Windows.Forms.Integration.h
            }
        }

        #endregion

        #region 窗体显示事件

        /// <summary>
        /// 窗体显示事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ToolCmWindow_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                //窗体位置设置
                this.WindowPositionSetting();
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

        #region 窗体位置设置

        /// <summary>
        /// 窗体位置设置
        /// </summary>
        public void WindowPositionSetting()
        {
            try
            {
                //设置y坐标
                this.Top = windowTop;

                //设置x坐标
                this.Left = 100;
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

        #region 统一导航

        public void btn_Navicate(object sender, RoutedEventArgs e)
        {
             try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //切换控制中心
                    this.NavicateChangeCenter(navicateButton.ViewSelectedItemEnum);
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

        #region 切换控制中心

        /// <summary>
        /// 切换控制中心
        /// </summary>
        public void NavicateChangeCenter(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //当前面板设置为隐藏
                this.Visibility = System.Windows.Visibility.Collapsed;
                //工具箱选择项
                this.ViewSelectedItemEnum = viewSelectedItemEnum;

                //若为第一次选择,则变换样式
                if (this.firstSelected)
                {
                    //面板背景色更改
                    this.gridMain.Background = this.Resources["bgColor2"] as SolidColorBrush;


                    //主持人功能按钮背景更改
                    this.btn_Chair.Background = Application.Current.Resources["brush_Chair2"] as ImageBrush;
                    //会议切换按钮背景更改
                    this.btn_Meet_Changed.Background = Application.Current.Resources["brush_Meet_Change2"] as ImageBrush;
                    //系统设置按钮背景更改
                    this.btn_Setting.Background = Application.Current.Resources["brush_Setting2"] as ImageBrush;
                    //中控功能按钮背景更改
                    this.btn_Studiom.Background = Application.Current.Resources["brush_Studiom2"] as ImageBrush;
                    //U盘传输按钮背景更改
                    this.btn_U_Disk.Background = Application.Current.Resources["brush_U_Disk2"] as ImageBrush;

                    //字体颜色2
                    SolidColorBrush txtForeColor2 = this.Resources["txtForeColor2"] as SolidColorBrush;

                    //主持人功能按钮字体色更改
                    this.btn_Chair.Foreground = txtForeColor2;
                    //会议切换按钮字体色更改
                    this.btn_Meet_Changed.Foreground = txtForeColor2;
                    //系统设置按钮字体色更改
                    this.btn_Setting.Foreground = txtForeColor2;
                    //中控功能按钮字体色更改
                    this.btn_Studiom.Foreground = txtForeColor2;
                    //U盘传输按钮字体色更改
                    this.btn_U_Disk.Foreground = txtForeColor2;

                    //更改面板边距值
                    this.BorderThickness = new Thickness(0, 1, 1, 1);

                    //第一次标示改为false
                    this.FirstSelected = false;
                }

                //子项选择更改事件执行
                if (this.SelectItemChanged != null)
                {
                    this.SelectItemChanged(viewSelectedItemEnum);
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
