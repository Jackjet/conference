using ConferenceCommon.TimerHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WebHelper;
using ConferenceModel.ConferenceInfoWebService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceCommon.WPFHelper;

namespace Conference_MyConference
{
    /// <summary>
    /// ConferenceRoomItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceRoomItem : UserControlBase
    {

        #region 自定义事件回调

        /// <summary>
        /// 子集点击事件
        /// </summary>
        public Action ItemClick = null;

        /// <summary>
        /// 设置主持人回调
        /// </summary>
        public Action<ConferenceInformationEntityPC> SettingMeetPrensterCallBack = null;

        #endregion

        #region 字段

        /// <summary>
        /// 当前会议信息
        /// </summary>
        public ConferenceInformationEntityPC conferenceInformationEntityPC = null;

        #endregion

        #region 静态字段



        #endregion

        #region 绑定属性



        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConferenceRoomItem()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        public ConferenceRoomItem(ConferenceInformationEntityPC conferenceInformationEntity)
            : this()
        {
            try
            {
                //绑定当前上下文
                this.DataContext = conferenceInformationEntity;

                conferenceInformationEntityPC = conferenceInformationEntity;

                if (conferenceInformationEntity.EndTime > DateTime.Now)
                {
                    this.borImg.Background = this.Resources["brush_Room2"] as ImageBrush;
                }
                else
                {
                    this.borImg.Background = this.Resources["brush_Room"] as ImageBrush;
                }


                //鼠标进入启动动画
                this.MouseEnter += ConferenceRoomItem_MouseEnter;
                //鼠标移除启动反向动画
                this.MouseLeave += ConferenceRoomItem_MouseLeave;

                #region 注册事件

                //图例点击
                this.MouseLeftButtonDown += ConferenceRoomItem_MouseLeftButtonDown;

                #endregion

                if (this.SettingMeetPrensterCallBack != null)
                {
                    this.SettingMeetPrensterCallBack(conferenceInformationEntity);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #region 图例点击

        /// <summary>
        /// 图例点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceRoomItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.ClickCount == 2 && conferenceInformationEntityPC != null)
                {
                    if (conferenceInformationEntityPC.BeginTime < DateTime.Now.AddMinutes(-30))
                    {
                        ConferenceRoom_View.conferenceRoom_View.RoomCardInit(this, conferenceInformationEntityPC);
                    }
                    else
                    {
                        this.gridTip.Visibility = System.Windows.Visibility.Visible;
                        TimerJob.StartRun(new Action(() =>
                            {
                                this.gridTip.Visibility = System.Windows.Visibility.Collapsed;
                            }), 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        ///// <summary>
        ///// 图例点击
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //void ConferenceRoomItem_TouchDown(object sender, TouchEventArgs e)
        //{
        //    try
        //    {
        //        if (this.ItemClick != null)
        //        {
        //            this.ItemClick();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

        #endregion

        #region 样式更改

        /// <summary>
        /// 会议室卡片选择样式设置
        /// </summary>
        public void StyleChangeToSelected()
        {
            try
            {
                this.borImg.Background = this.Resources["brush_RoomSelect"] as ImageBrush;
                this.borImg.BorderThickness = new Thickness(2, 2, 2, 0);
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
        /// 会议室卡片取消选择样式设置
        /// </summary>
        public void StyleChangeToNoSelected()
        {
            try
            {
                this.borImg.Background = this.Resources["brush_Room"] as ImageBrush;
                this.borImg.BorderThickness = new Thickness(0, 0, 0, 2);
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

        #region 信息显示隐藏事件

        /// <summary>
        /// 信息显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceRoomItem_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                //(this.Resources["Storyboard1"] as Storyboard).Begin();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 信息隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceRoomItem_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                //(this.Resources["Storyboard2"] as Storyboard).Begin();
                //设置为不可直接进入
                //this.canEnter = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
