using Conference.TimerControl;
using ConferenceCommon.LogHelper;
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

namespace Conference.View.Resource
{
    /// <summary>
    /// ConversationResourceItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConversationResourceItem : UserControl, INotifyPropertyChanged
    {
        #region 静态字段

        /// <summary>
        /// 单例模式
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

        #region 实时更新

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region 构造函数

        public ConversationResourceItem()
        {
            try
            {
                InitializeComponent();
                //绑定当前上下文
                this.DataContext = this;

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

        /// <summary>
        /// 注册事件
        /// </summary>
        public void TouchEventRegedit()
        {
            try
            {
                //点击选择自己填充静态实例
                this.TouchDown += ConversationCard_TouchDown;
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
                //点击选择自己填充静态实例
                this.PreviewMouseLeftButtonDown -= ConversationCard_PreviewMouseLeftButtonDown;
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
                //点击选择自己填充静态实例
                this.TouchDown -= ConversationCard_TouchDown;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 事件集切换

        /// <summary>
        /// 切换为PAD模式
        /// </summary>
        public void ChangedToPAD()
        {
            try
            {
                //注销主窗体路由事件集
                this.DeleteRouteEventRegedit();
                //注销主窗体触摸事件集
                this.DeleteTouchEventRegedit();
                //添加主窗体触摸事件集
                this.TouchEventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 切换为PC模式
        /// </summary>
        public void ChangedToPC()
        {
            try
            {
                //注销主窗体路由事件集
                this.DeleteRouteEventRegedit();
                //注销主窗体触摸事件集
                this.DeleteTouchEventRegedit();
                //添加主窗体路由事件集
                this.RouteEventRegedit();
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConversationCard_TouchDown(object sender, TouchEventArgs e)
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
