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
using Conference.Common;
using ConferenceCommon.WPFHelper;

namespace Conference.View.ConferenceRoom
{
    /// <summary>
    /// ConferenceRoomItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceRoomItem : UserControlBase
    {

        #region 自定义事件

        public delegate void ItemClickEnventHandle();
        /// <summary>
        /// 子集点击事件
        /// </summary>
        public event ItemClickEnventHandle ItemClick = null;

        #endregion

        #region 字段

        /// <summary>
        /// 当前会议信息
        /// </summary>
       public ConferenceInformationEntityPC ConferenceInformationEntity = null;
       
        #endregion

        #region 静态字段

      

        #endregion
      
        #region 绑定属性



        #endregion

        #region 构造函数

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

                ConferenceInformationEntity = conferenceInformationEntity;

                if (conferenceInformationEntity.EndTime > DateTime.Now)
                {
                    this.borImg.Background = Application.Current.Resources["brush_Room2"] as ImageBrush;
                }
                else
                {
                    this.borImg.Background = Application.Current.Resources["brush_Room"] as ImageBrush;
                }


                //鼠标进入启动动画
                this.MouseEnter += ConferenceRoomItem_MouseEnter;
                //鼠标移除启动反向动画
                this.MouseLeave += ConferenceRoomItem_MouseLeave;

                #region 注册事件

                //图例点击
                this.MouseLeftButtonDown += ConferenceRoomItem_MouseLeftButtonDown;

                #endregion

                DispatcherTimer timer = null;
                TimerJob.StartRun(new Action(() =>
                {
                    if (Constant.DicParticipant.Count > 0)
                    {
                        //参会人绑定
                        if (conferenceInformationEntity.ApplyPeople != null && Constant.DicParticipant.ContainsKey(conferenceInformationEntity.ApplyPeople))
                        {
                            conferenceInformationEntity.ApplyPeople = Constant.DicParticipant[conferenceInformationEntity.ApplyPeople];

                        }
                        timer.Stop();
                    }
                }), 600, out timer);
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
                //if (!canEnter)
                //{
                //    this.borImg.Background = Application.Current.Resources[""] as ImageBrush;
                //    //设置为可直接进入
                //    //this.canEnter = true;
                //}
                //else
                //{
                //    if (this.ItemClick != null)
                //    {
                //        this.ItemClick();
                //        //设置为不可直接进入
                //        //this.canEnter = false;
                //    }
                //}
                if (e.ClickCount == 2 && ConferenceInformationEntity != null)
                {
                    if (ConferenceInformationEntity.BeginTime < DateTime.Now.AddMinutes(-30))
                    {

                        if (this.ItemClick != null)
                        {
                            this.ItemClick();                           
                        }
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

        /// <summary>
        /// 图例点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceRoomItem_TouchDown(object sender, TouchEventArgs e)
        {
            try
            {
                if (this.ItemClick != null)
                {
                    this.ItemClick();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #endregion

        #region 样式更改

        public void StyleChangeToSelected()
        {
            try
            {
                this.borImg.Background = Application.Current.Resources["brush_RoomSelect"] as ImageBrush;
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

        public void StyleChangeToNoSelected()
        {
            try
            {
                this.borImg.Background = Application.Current.Resources["brush_Room"] as ImageBrush;
                this.borImg.BorderThickness = new Thickness(0,0,0,2);
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
