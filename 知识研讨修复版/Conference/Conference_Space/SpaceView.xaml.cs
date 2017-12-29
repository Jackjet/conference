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
using System.Windows.Navigation;
using System.Windows.Shapes;
using vy = System.Windows.Visibility;

namespace Conference_Space
{
    /// <summary>
    /// SpaceView.xaml 的交互逻辑
    /// </summary>
    public partial class SpaceView : UserControlBase
    {
        #region 字段

        MeetingSpace meetingSpace;
        /// <summary>
        /// 会议空间
        /// </summary>
        public MeetingSpace MeetingSpace
        {
            get
            {
                //创建单例
                if (this.meetingSpace == null)
                {
                    this.meetingSpace = new MeetingSpace();
                }
                return meetingSpace;
            }
        }

        PersonalSpace personalSpace;
        /// <summary>
        /// 个人空间
        /// </summary>
        public PersonalSpace PersonalSpace
        {
            get
            {
                //创建单例
                if (this.personalSpace == null)
                {
                    this.personalSpace = new PersonalSpace();
                }
                return personalSpace;
            }
        }

        #endregion

        #region 绑定属性

        vy changedPanelVisibility;
       /// <summary>
       /// 切换面板显示
       /// </summary>
        public vy ChangedPanelVisibility
        {
            get { return changedPanelVisibility; }
            set
            {
                if (changedPanelVisibility != value)
                {
                    changedPanelVisibility = value;
                    this.OnPropertyChanged("ChangedPanelVisibility");
                }
            }
        }
        
        #endregion

        #region 静态字段

        /// <summary>
        /// 空间选择样式
        /// </summary>
        Style btnSpaceStyleSelected = null;

        /// <summary>
        /// 空间未选择样式
        /// </summary>
        Style btnSpaceStyle = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SpaceView()
        {
            try
            {
                InitializeComponent();               
                //样式收集
                this.StyleCollection();
                //事件注册
                this.EventRegedit();
                //空间选择
                this.InterMeetSpace();
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

        #region 样式收集

        /// <summary>
        /// 样式收集
        /// </summary>
        public void StyleCollection()
        {
            try
            {

                if (this.btnSpaceStyleSelected == null && this.Resources.Contains("btnSpaceStyleSelected"))
                {
                    btnSpaceStyleSelected = this.Resources["btnSpaceStyleSelected"] as Style;
                }
                if (this.btnSpaceStyle == null && this.Resources.Contains("btnSpaceStyle"))
                {
                    btnSpaceStyle = this.Resources["btnSpaceStyle"] as Style;
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


        #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //会议空间选择
                this.btnMeetSpace.Click += btnMeetSpace_Click;
                //个人空间选择
                this.btnPersonalSpace.Click += btnPersonalSpace_Click;
                //视图切换回调
                this.MeetingSpace.ViewChangeCallBack = ViewChangeCallBack;
                //视图切换回调
                this.PersonalSpace.ViewChangeCallBack = ViewChangeCallBack;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        #region 视图切换回调

        /// <summary>
        /// 视图切换回调
        /// </summary>
        /// <param name="isExpander">是否为展开</param>
        private void ViewChangeCallBack(bool isExpander)
        {
            try
            {
                if (isExpander)
                {
                    this.ChangedPanelVisibility = vy.Visible;                    
                }
                else
                {
                    this.ChangedPanelVisibility = vy.Collapsed;
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

        #endregion

        #region 个人空间选择

        /// <summary>
        /// 个人空间选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPersonalSpace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.borMain.Child = this.PersonalSpace;
                this.btnPersonalSpace.Style = btnSpaceStyleSelected;
                this.btnMeetSpace.Style = btnSpaceStyle;
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

        #region 会议空间选择

        /// <summary>
        /// 会议空间选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMeetSpace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.InterMeetSpace();
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
        /// 会议空间选择
        /// </summary>
        void InterMeetSpace()
        {
            try
            {
                this.borMain.Child = this.MeetingSpace;
                this.btnPersonalSpace.Style = btnSpaceStyle;
                this.btnMeetSpace.Style = btnSpaceStyleSelected;
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

        #region 释放空间资源

        /// <summary>
        /// 释放空间资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (this.meetingSpace != null)
                {
                    this.meetingSpace.Dispose();
                }
                if (this.personalSpace != null)
                {
                    this.personalSpace.Dispose();
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
