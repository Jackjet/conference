using ConferenceCommon.LogHelper;
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
    /// ApplicationShareWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ApplicationWindow : Window
    {
        #region 构造函数

        public ApplicationWindow()
        {
            try
            {
                InitializeComponent();
                //初始化加载
                this.Loaded += ApplicationWindow_Loaded;
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
                if (ConversationCard.currentConversationCard != null)
                {
                    //获取当前会话卡片
                    var conversationCard = ConversationCard.currentConversationCard;
                    //获取共享模型
                    var shareModality = ((ApplicationSharingModality)(conversationCard.ConversationWindow.Conversation.Modalities[ModalityTypes.ApplicationSharing]));

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
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

    }
}
