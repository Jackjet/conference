using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
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

namespace Conference_Tree
{
    /// <summary>
    /// SummarizeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SummarizeWindow : WindowBase
    {
        #region 自定义事件回调

        public delegate void IsOkEventHandle(string message);
        /// <summary>
        /// 设置
        /// </summary>
        public event IsOkEventHandle IsOkEvent;

        #endregion

        #region 绑定属性

        bool summarizeIsReadOnly;
        /// <summary>
        /// 综述只读
        /// </summary>
        public bool SummarizeIsReadOnly
        {
            get { return summarizeIsReadOnly; }
            set
            {
                if (summarizeIsReadOnly != value)
                {
                    summarizeIsReadOnly = value;
                    this.OnPropertyChanged("SummarizeIsReadOnly");
                }
            }
        }

        string summarizeText = string.Empty;
        /// <summary>
        /// 综述内容
        /// </summary>
        public string SummarizeText
        {
            get { return summarizeText; }
            set
            {
                if (summarizeText != value)
                {
                    summarizeText = value;
                    this.OnPropertyChanged("SummarizeText");
                }
            }
        }


        #endregion
       
        #region 构造函数

        /// <summary>
        /// 研讨树综述窗体
        /// </summary>
        public SummarizeWindow()
        {
            try
            {
                //UI初始化
                InitializeComponent();

                #region 客户端操作模式

                this.RouteEventRegedit();

                //switch (Constant.ControlMode)
                //{
                //    case ConferenceCommon.EnumHelper.ClientMode.PC:
                //        this.RouteEventRegedit();
                //        break;
                //    case ConferenceCommon.EnumHelper.ClientMode.PAD:
                //        this.TouchEventRegedit();
                //        break;

                //    default:
                //        break;
                //}
                #endregion

                if (TreeCodeEnterEntity.IsMeetPresenter)
                {
                    this.SummarizeIsReadOnly = false;
                }
                else
                {
                    this.SummarizeIsReadOnly = true;
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
                //确认（生成会议纪要）
                this.btnOK.Click += btnOK_Click;              
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
                //确认（生成会议纪要）
                this.btnOK.TouchUp += btnOK_TouchDown;             
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
                //确认（生成会议纪要）
                this.btnOK.Click -= btnOK_Click;
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
                //确认（生成会议纪要）
                this.btnOK.TouchDown -= btnOK_TouchDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 确定

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Ok();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnOK_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                this.Ok();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 确定
        /// </summary>
        public void Ok()
        {
            try
            {
                if (this.IsOkEvent != null)
                {
                    this.IsOkEvent(this.SummarizeText);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 取消

        //void btnNo_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        this.Cancel();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}
        //void btnNo_TouchUp(object sender, TouchEventArgs e)
        //{
        //    try
        //    {
        //        this.Cancel();
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        //public void Cancel()
        //{
        //    try
        //    {
        //        if (this.IsOkEvent != null)
        //        {
        //            this.IsOkEvent(null);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion
    }
}
