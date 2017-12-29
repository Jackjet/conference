using Conference_Conversation.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.Sharing;
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
using vy = System.Windows.Visibility;

namespace Conference_Conversation
{
    /// <summary>
    /// ConversationResourceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationResourceWindow : WindowBase
    {
        #region 内部字段


        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConversationResourceWindow()
        {
            try
            {
                InitializeComponent();
                //事件注册
                this.EventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

         #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //初始化加载
                this.Loaded += ConversationResourceWindow_Loaded;
                //关闭事件
                this.btnClose.Click += btnClose_Click;
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

        #region 初始化加载

        /// <summary>
        /// 初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationResourceWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //资源呈现刷新
                this.RefleshViewByMesuare();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    
        #region 共享资源管理（添加，删除）

        /// <summary>
        /// 共享资源View添加
        /// </summary>
        /// <param name="item"></param>
        public void ShareResourceItemAdd(ConversationResourceItem item)
        {
            try
            {
                if (!this.stackPanel.Children.Contains(item))
                {
                    this.stackPanel.Children.Add(item);
                    //点击进行演示
                    item.PreviewMouseLeftButtonDown += item_PreviewMouseLeftButtonDown;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        /// <summary>
        /// 共享资源View删除
        /// </summary>
        /// <param name="item"></param>
        public void ShareResourceItemRemove(ConversationResourceItem item)
        {
            try
            {
                if (this.stackPanel.Children.Contains(item))
                {
                    this.stackPanel.Children.Remove(item);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 演示当前选择的共享资源

        /// <summary>
        /// 演示当前选择的共享资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void item_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.PresentSelectedResourceItem();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 演示当前选择的共享资源
        /// </summary>
        public void PresentSelectedResourceItem()
        {
            try
            {
                if (ConversationResourceItem.currentConversationResourceItem != null)
                {
                    var item = ConversationResourceItem.currentConversationResourceItem;
                    if (!item.ShareableContent.IsActive)
                    {
                        //立即演示
                        item.ShareableContent.Present();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 资源呈现刷新

        /// <summary>
        /// 资源呈现刷新
        /// </summary>
        public void RefleshViewByMesuare()
        {
            try
            {
                //清空数据
                this.stackPanel.Children.Clear();

                //遍历共享资源
                foreach (var item in LyncHelper.ShareableContentList)
                {
                    //共享资源View子项生成
                    ConversationResourceItem conversationResourceItem = new ConversationResourceItem();
                    //设置标题
                    conversationResourceItem.ShareResouceItemTitle = item.Title;

                    if (item.Presenter != null)
                    {
                        //获取当前演示者
                        conversationResourceItem.StrPresenter = Convert.ToString(item.Presenter.Contact.GetContactInformation(ContactInformationType.DisplayName));
                    }
                    else
                    {
                        conversationResourceItem.StrPresenter = "无";
                    }
                    if (item.Owner != null)
                    {
                        //获取当前共享资源拥有者
                        conversationResourceItem.StrOwner = Convert.ToString(item.Owner.GetContactInformation(ContactInformationType.DisplayName));
                    }
                    //关联共享资源
                    conversationResourceItem.ShareableContent = item;
                    //共享资源View添加
                    this.ShareResourceItemAdd(conversationResourceItem);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 资源属性刷新
        /// </summary>
        public void RefleshJustView()
        {
            try
            {
                //遍历子项
                foreach (var item in stackPanel.Children)
                {
                    if (item is ConversationResourceItem)
                    {
                        //类型转换
                        var conversationResourceItem = item as ConversationResourceItem;
                        //获取映射的共享资源
                        var shareContent = conversationResourceItem.ShareableContent;

                        if (shareContent.Presenter != null)
                        {
                            //获取当前演示者
                            conversationResourceItem.StrPresenter = Convert.ToString(shareContent.Presenter.Contact.GetContactInformation(ContactInformationType.DisplayName));
                        }
                        else
                        {
                            conversationResourceItem.StrPresenter = "无";
                        }
                        if (shareContent.Owner != null)
                        {
                            //获取当前共享资源拥有者
                            conversationResourceItem.StrOwner = Convert.ToString(shareContent.Owner.GetContactInformation(ContactInformationType.DisplayName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 显示、隐藏该窗体

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
        /// 根据当前窗体的状态来显示或隐藏
        /// </summary>
        public void ShowOrHiddenTheView()
        {
              try
            {
                if (this.Visibility == vy.Hidden)
                {
                    this.Activate();
                    this.Visibility = vy.Visible;
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

        #region 关闭窗体

        /// <summary>
        /// 关闭窗体
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
