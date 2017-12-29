using ConferenceCommon.LogHelper;
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

namespace Conference.View.Resource
{
    /// <summary>
    /// ConversationResourceWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationResourceWindow : Window
    {
        #region 内部字段


        #endregion

        #region 构造函数

        public ConversationResourceWindow()
        {
            try
            {

                InitializeComponent();
                //初始化加载
                this.Loaded += ConversationResourceWindow_Loaded;               
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

        #region 移动应用窗体

        /// <summary>
        /// 移动应用窗体
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
                //设置当前选择的资源选项
                var currentConversationCard = ConversationCard.currentConversationCard;
                //遍历共享资源
                foreach (var item in currentConversationCard.ShareableContentList)
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
    }
}
