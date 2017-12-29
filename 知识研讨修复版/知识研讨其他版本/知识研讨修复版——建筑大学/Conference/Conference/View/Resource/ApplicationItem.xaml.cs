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
    /// ApplicationItem.xaml 的交互逻辑
    /// </summary>
    public partial class ApplicationItem : UserControl, INotifyPropertyChanged
    {
        #region 静态字段

        /// <summary>
        /// 单例模式
        /// </summary>
        public static ApplicationItem currentApplicationItem = null;

        #endregion

        #region 一般属性

        SharingResource sharingResource;
        /// <summary>
        /// 映射的程序（可共享的）
        /// </summary>
        public SharingResource SharingResource
        {
            get { return sharingResource; }
            set { sharingResource = value; }
        }

        #endregion

        #region 绑定属性

        string applicationItemTitle;
        /// <summary>
        /// 资源共享的名称
        /// </summary>
        public string ApplicationItemTitle
        {
            get { return applicationItemTitle; }
            set
            {
                if (value != applicationItemTitle)
                {
                    applicationItemTitle = value;
                    this.OnPropertyChanged("ShareResouceItemTitle");
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

        public ApplicationItem()
        {
            InitializeComponent();
            //绑定当前上下文
            this.DataContext = this;

            //点击选择自己填充静态实例
            this.PreviewMouseLeftButtonDown += ConversationCard_PreviewMouseLeftButtonDown;
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
                if (ApplicationItem.currentApplicationItem != null)
                {
                    //将之前的会话卡片恢复为默认的背景色
                    ApplicationItem.currentApplicationItem.bor.Background = new SolidColorBrush(Colors.Transparent);
                }
                //点击选择自己填充静态实例
                ApplicationItem.currentApplicationItem = this;

                //设置选择后的背景色
                this.SetCardBackColor();
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

        #region 鼠标进入事件

        /// <summary>
        /// 鼠标进入事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bor_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                
                this.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 鼠标移出事件

        /// <summary>
        /// 鼠标移出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bor_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.BorderBrush = new SolidColorBrush(Colors.Silver);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
