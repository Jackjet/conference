using Conference_Conversation.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.Sharing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using vy = System.Windows.Visibility;

namespace Conference_Conversation
{
    /// <summary>
    /// ApplicationShareWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ApplicationWindow : WindowBase
    {
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ApplicationWindow()
        {
            try
            {
                InitializeComponent();
                //初始化加载
                this.Loaded += ApplicationWindow_Loaded;
                //关闭事件
                this.btnClose.Click += btnClose_Click;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 初始化加载

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ApplicationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.FillData();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 数据加载

       /// <summary>
        /// 数据加载
       /// </summary>
        public void FillData()
        {
             try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //获取共享模型
                    var shareModality = ((ApplicationSharingModality)(LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing]));
                    if (shareModality != null)
                    {
                        this.stackPanel.Children.Clear();
                        foreach (var item in shareModality.ShareableResources)
                        {
                            //生成共享程序子项
                            ApplicationItem applicationItem = new ApplicationItem();
                            //设置标题
                            applicationItem.ApplicationItemTitle = item.Name;
                            //共享程序映射
                            applicationItem.SharingResource = item;

                            //共享程序子项添加
                            this.stackPanel.Children.Add(applicationItem);
                            //共享程序子项点击事件
                            applicationItem.PreviewMouseLeftButtonDown += (object sender1, MouseButtonEventArgs e1) =>
                            {
                                try
                                {
                                    if (shareModality.CanShare(item.Type))
                                    {
                                        shareModality.BeginShareResources(item, null, null);
                                        Process process = Process.GetProcessById(item.Id);

                                        if (process != null && process.MainWindowHandle.ToInt32() > 0)
                                        {
                                            //设置会话区域显示内容
                                            ConversationM.conversationM.SetConversationAreaShow(ShowType.SelfDeskTopShowView, true);
                                        }
                                        else
                                        {
                                            if (this.stackPanel.Children.Contains(applicationItem))
                                            {
                                                this.stackPanel.Children.Remove(applicationItem);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    LogManage.WriteLog(this.GetType(), ex);
                                }
                            };
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

        #endregion

        #region 显示隐藏该窗体

        /// <summary>
        /// 显示窗体
        /// </summary>
        public void ShowTheView()
        {
            try
            {
                this.Visibility = vy.Visible;
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
        /// 隐藏窗体
        /// </summary>
        public void HiddenTheView()
        {
            try
            {
                this.Visibility = vy.Hidden;
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
        /// 根据当前状态显示或隐藏
        /// </summary>
        public void ShowOrHiddenTheView()
        {
            try
            {
                if (this.Visibility == vy.Hidden)
                {
                    this.Activate();
                    this.Visibility = vy.Visible;
                    this.FillData();
                }
                else
                {
                    this.Visibility = vy.Hidden;
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

        #region 窗体关闭

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.HiddenTheView();
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
    }
}
