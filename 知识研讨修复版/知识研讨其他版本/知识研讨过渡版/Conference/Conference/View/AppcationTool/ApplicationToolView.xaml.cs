using ConferenceCommon.ApplicationHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.RegeditHelper;
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

namespace Conference.View.AppcationTool
{
    /// <summary>
    /// 工具窗体
    /// </summary>
    public partial class ApplicationToolView : UserControl
    {
        #region 构造函数

        public ApplicationToolView()
        {
            try
            {
                //UI加载
                InitializeComponent();
              
                #region 注册事件

                //同步应用（甩屏）
                this.btnSyncApp.Click += btnSyncApp_Click;
                //打开IE浏览器
                this.btn_IE.Click += btn_IE_Click;
                //打开记事本
                this.btnNotePad.Click += NotePad_Click;
                //打开计算器
                this.btnCalcula.Click += btnCalcula_Click;

                #endregion
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 打开同步应用（甩屏同步）

        /// <summary>
        /// 打开同步应用（甩屏同步）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSyncApp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.SyncApp();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 同步应用（甩屏）
        /// </summary>
        private void SyncApp()
        {
            try
            {
                //加入会议后才可进行甩屏操作
                if (!string.IsNullOrEmpty(Constant.ConferenceName))
                {
                    if (!Constant.IsMeetingPresenter)
                    {
                        //创建甩屏窗体并进行相应显示
                        AppSyncWindow AppSyncWindow = new AppSyncWindow();
                        AppSyncWindow.Show();
                    }
                    else
                    {
                        MessageBox.Show("参会人可使用甩屏", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("在使用甩屏之前,请先进入一个会议", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 打开IE浏览器

        /// <summary>
        /// 打开IE浏览器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_IE_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplicationManage.OpenApp(AppType.IE);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 打开记事本

        /// <summary>
        /// 打开记事本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NotePad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplicationManage.OpenApp(AppType.Notepad);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 打开计算器

        /// <summary>
        /// 打开计算器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCalcula_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ApplicationManage.OpenApp(AppType.calc);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
