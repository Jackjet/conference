using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model.Conversation.Sharing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference_Conversation
{
    /// <summary>
    /// ConversationResourceItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationResourceItem : UserControlBase
    {
        #region 静态字段

        /// <summary>
        /// 当前选择的共享资源项
        /// </summary>
        public static ConversationResourceItem currentConversationResourceItem = null;

        #endregion

        #region 一般属性

        ShareableContent shareableContent;
        /// <summary>
        /// 关联的资源共享对象
        /// </summary>
        public ShareableContent ShareableContent
        {
            get { return shareableContent; }
            set { shareableContent = value; }
        }

        #endregion

        #region 绑定属性

        string shareResouceItemTitle;
        /// <summary>
        /// 资源共享的名称
        /// </summary>
        public string ShareResouceItemTitle
        {
            get { return shareResouceItemTitle; }
            set
            {
                if (value != shareResouceItemTitle)
                {
                    shareResouceItemTitle = value;
                    this.OnPropertyChanged("ShareResouceItemTitle");
                }
            }
        }

        string strPresenter;
        /// <summary>
        /// 当前演示者
        /// </summary>
        public string StrPresenter
        {
            get { return strPresenter; }
            set
            {
                if (value != strPresenter)
                {
                    strPresenter = value;
                    this.OnPropertyChanged("StrPresenter");
                }
            }
        }

        string strOwner;
        /// <summary>
        /// 当前资源拥有者
        /// </summary>
        public string StrOwner
        {
            get { return strOwner; }
            set
            {
                if (value != strOwner)
                {
                    strOwner = value;
                    this.OnPropertyChanged("StrOwner");
                }
            }
        }


        #endregion
       
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConversationResourceItem()
        {
            try
            {
                InitializeComponent();
               
                this.EventRegedit();

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
        
        #region 注册事件区域

        /// <summary>
        /// 注册事件区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.PreviewMouseLeftButtonDown += ConversationCard_PreviewMouseLeftButtonDown;
                //释放共享资源
                this.btnDispose.Click += btnDispose_Click;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
    
        #endregion
              
        #region 点击选择自己填充静态实例

        /// <summary>
        /// 点击选择自己填充静态实例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ConversationResourceItemPoint();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
       
        /// <summary>
        /// 点击选择自己填充静态实例
        /// </summary>
        public void ConversationResourceItemPoint()
        {
            try
            {
                if (ConversationResourceItem.currentConversationResourceItem != null)
                {
                    //将之前的会话卡片恢复为默认的背景色
                    ConversationResourceItem.currentConversationResourceItem.bor.Background = new SolidColorBrush(Colors.Transparent);
                }
                //点击选择自己填充静态实例
                ConversationResourceItem.currentConversationResourceItem = this;

                //设置选择后的背景色
                this.SetCardBackColor();

                TimerJob.StartRun(new Action(() =>
                    {
                        //资源属性刷新
                        //MainWindow.conversationResourceWindow.RefleshJustView();
                    }), 700);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 设置选择后的背景色

        /// <summary>
        /// 设置选择后的背景色
        /// </summary>
        public void SetCardBackColor()
        {
            try
            {
                //将选择的会话卡片设置新的背景
                this.bor.Background = new SolidColorBrush(Colors.SkyBlue) { Opacity = 0.3 };
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDispose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //移除
                this.ShareableContent.Remove();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

    }
}
