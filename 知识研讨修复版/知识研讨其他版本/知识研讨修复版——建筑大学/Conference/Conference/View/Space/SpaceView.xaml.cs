using ConferenceCommon.LogHelper;
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

namespace Conference.View.Space
{
    /// <summary>
    /// SpaceView.xaml 的交互逻辑
    /// </summary>
    public partial class SpaceView : UserControl
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

        #region 构造函数

        public SpaceView()
        {
            try
            {
                InitializeComponent();

                #region 事件注册

                //会议空间选择
                this.btnMeetSpace.Click += btnMeetSpace_Click;
                //个人空间选择
                this.btnPersonalSpace.Click += btnPersonalSpace_Click;

                #endregion

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

        #region 选择空间类型

        /// <summary>
        /// 选择空间类型
        /// </summary>
        public void SpaceSelctet()
        {

        }

        #endregion

        #region 个人空间选择

        void btnPersonalSpace_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.borMain.Child = this.PersonalSpace;

                this.btnPersonalSpace.Style = this.Resources["btnSpaceStyleSelected"] as Style;
                this.btnMeetSpace.Style = this.Resources["btnSpaceStyle"] as Style;
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

        void InterMeetSpace()
        {
            try
            {
                this.borMain.Child = this.MeetingSpace;

                this.btnPersonalSpace.Style = this.Resources["btnSpaceStyle"] as Style;
                this.btnMeetSpace.Style = this.Resources["btnSpaceStyleSelected"] as Style;
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

        #region 是否空间资源

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
