using Conference.Common;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.FileHelper;
using ConferenceCommon.ImageHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using Microsoft.Lync.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference.View.MyConference
{
    /// <summary>
    /// MyConferenceView.xaml 的交互逻辑
    /// </summary>
    public partial class MyConferenceView : UserControlBase
    {
        #region 内部字段

        /// <summary>
        /// 座位字典集
        /// </summary>
        Dictionary<int, Button> dicSeatButton1 = new Dictionary<int, Button>();

        /// <summary>
        /// 座位字典集
        /// </summary>
        Dictionary<int, Button> dicSeatButton2 = new Dictionary<int, Button>();

        /// <summary>
        /// 自己的座位
        /// </summary>
        ConferenceModel.ConferenceMatrixWebservice.SeatEntity selfSeatEntity = null;

        #endregion

        #region 绑定属性

        string conferenceName;
        /// <summary>
        /// 会议名称
        /// </summary>
        public string ConferenceName
        {
            get { return conferenceName; }
            set
            {
                if (this.conferenceName != value)
                {
                    this.conferenceName = value;
                    this.OnPropertyChanged("ConferenceName");
                }
            }
        }

        string conferenceRoomName;
        /// <summary>
        /// 会议室名称
        /// </summary>
        public string ConferenceRoomName
        {
            get { return conferenceRoomName; }
            set
            {
                if (this.conferenceRoomName != value)
                {
                    this.conferenceRoomName = value;
                    this.OnPropertyChanged("ConferenceRoomName");
                }
            }
        }

        string conferenceHost;
        /// <summary>
        /// 会议主持人
        /// </summary>
        public string ConferenceHost
        {
            get { return conferenceHost; }
            set
            {
                if (this.conferenceHost != value)
                {
                    this.conferenceHost = value;
                    this.OnPropertyChanged("ConferenceHost");
                }
            }
        }

        ObservableCollection<ParticipantsEntity> participantsEntityList = new ObservableCollection<ParticipantsEntity>();
        /// <summary>
        /// 参会人列表
        /// </summary>
        internal ObservableCollection<ParticipantsEntity> ParticipantsEntityList
        {
            get { return participantsEntityList; }
            set { participantsEntityList = value; }
        }

        #endregion

        #region 构造函数

        public MyConferenceView()
        {
            try
            {
                //UI加载
                this.InitializeComponent();
                //绑定当前上下文
                this.DataContext = this;

                //this.datagrid.ItemsSource = currentParticipantsEntityList;

                //填充参会人列表
                this.FillDataSource();

                if (MainWindow.MainPageInstance.ConferenceRoom_View.Parent != null && MainWindow.MainPageInstance.ConferenceRoom_View.Parent.GetType() == typeof(Border))
                {
                    Border bor1 = MainWindow.MainPageInstance.ConferenceRoom_View.Parent as Border;
                    bor1.Child = null;
                }

                //加载会议室
                this.bor1.Child = MainWindow.MainPageInstance.ConferenceRoom_View;

                //座位初始化
                this.SeatDataFill();

                //UI加载事件
                this.Loaded += MyConferenceView_Loaded;

                //指定回调
                LyncHelper.BeginRefleshDataGridCallBack = DataGridItemChangeCallBack;


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

        #region UI加载事件

        void MyConferenceView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                #region old solution

                //double allColumnWidth = 0;

                //foreach (var item in this.datagrid.Columns)
                //{
                //    allColumnWidth += item.ActualWidth;
                //}
                //if (allColumnWidth < this.borTittle.ActualWidth)
                //{
                //    this.columnState.Width = this.borTittle.ActualWidth - allColumnWidth;
                //}

                #endregion

                this.columnState.Width = 105;
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

        #region 座位初始化

        /// <summary>
        /// 座位初始化
        /// </summary>
        public void SeatDataFill()
        {
            try
            {
                //座位数量统计
                int count = this.gridSeatPanel.Children.Count;

                //遍历同一面板的所有座位并进行加载
                for (int i = 0; i < count; i++)
                {
                    //获取元素
                    var element = this.gridSeatPanel.Children[i];
                    if (element.GetType() == typeof(Button))
                    {
                        //类型还原
                        Button btn = element as Button;

                        if (i < count / 2)
                        {
                            //记录
                            this.dicSeatButton1.Add(i + 1, btn);
                        }
                        else
                        {
                            this.dicSeatButton2.Add(i + 1, btn);
                        }
                    }
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

        #region 填充参会人列表

        /// <summary>
        /// 填充参会人列表
        /// </summary>
        public void FillDataSource()
        {
            try
            {
                //会议名称
                this.ConferenceName = Constant.ConferenceName;

                //会议地址
                this.ConferenceRoomName = Constant.ConferenceRoomName;


                if (Constant.DicParticipant.ContainsKey(Constant.ConferenceHost))
                {
                    //会议主持人
                    this.ConferenceHost = Constant.DicParticipant[Constant.ConferenceHost];
                }
                //填充参会人列表
                LyncHelper.FillLyncOnlineInfo(this.datagrid, this.ParticipantsEntityList);
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
        /// 子项更改事件
        /// </summary>
        public void DataGridItemChangeCallBack(ObservableCollection<ParticipantsEntity> currentParticipantsEntityList)
        {
            try
            {
                //this.datagrid.ItemsSource = currentParticipantsEntityList;
                //this.datagrid.Items.Clear();

                this.datagrid.ItemsSource = currentParticipantsEntityList.OrderBy(p => p.LoginState.Equals("未登录"));
               //foreach (var item in items)
               //{
               //    this.datagrid.Items.Add(item);
               //}
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

        #region 列宽度定义

        /// <summary>
        /// 列宽度定义
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //(sender as Grid).Width = this.ActualWidth - 160;
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

        #region 刷新座位分布情况

        /// <summary>
        /// 刷新座位分布情况
        /// </summary>
        public void Reflesh(List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity> seatEntity)
        {
            try
            {
                //座位数量统计
                int count = this.gridSeatPanel.Children.Count;

                List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity> selfSeatEntityList = seatEntity.Where(Item => Item.UserName != null &&
                      Item.UserName.Equals(Constant.SelfName)).ToList();
                if (selfSeatEntityList.Count > 0)
                {
                    this.selfSeatEntity = selfSeatEntityList[0];
                }
                int halfCount = count / 2;
                if (this.selfSeatEntity != null && this.selfSeatEntity.SettingNummber <= halfCount)
                {
                    //遍历设置座位
                    foreach (var item in seatEntity)
                    {
                        int number = item.SettingNummber;
                        string content = "座位" + number;
                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            content = item.UserName;
                        }
                        if (number <= halfCount)
                        {
                            if (this.dicSeatButton2.ContainsKey(number + halfCount))
                            {
                                Button btnd = this.dicSeatButton2[number + halfCount];
                                btnd.Content = content;
                                //btnd.Foreground = new SolidColorBrush(Colors.Red);
                                btnd.Tag = number;
                            }
                        }
                        else
                        {
                            if (this.dicSeatButton1.ContainsKey(number - halfCount))
                            {
                                Button btnd = this.dicSeatButton1[number - halfCount];
                                btnd.Content = content;
                                //btnd.Foreground = new SolidColorBrush(Colors.Red);
                                btnd.Tag = number;
                            }
                        }

                    }
                }
                else
                {
                    //遍历设置座位
                    foreach (var item in seatEntity)
                    {
                        int number = item.SettingNummber;
                        string content = "座位" + number;
                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            content = item.UserName;
                        }
                        if (number <= count / 2)
                        {
                            if (this.dicSeatButton1.ContainsKey(number))
                            {
                                Button btnd = this.dicSeatButton1[number];
                                btnd.Content = content;
                                //btnd.Foreground = new SolidColorBrush(Colors.Red);
                                btnd.Tag = number;
                            }
                        }
                        else
                        {
                            if (this.dicSeatButton2.ContainsKey(number))
                            {
                                Button btnd = this.dicSeatButton2[number];
                                btnd.Content = content;
                                //btnd.Foreground = new SolidColorBrush(Colors.Red);
                                btnd.Tag = number;
                            }
                        }

                    }
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

        #region 更改一个座位信息（自己或其他参会人）

        /// <summary>
        /// 更改一个座位信息（自己或其他参会人）
        /// </summary>
        public void UpdateOneSeat(ConferenceWebCommon.EntityHelper.ConferenceMatrix.SeatEntity seatEntity)
        {
            try
            {
                //座位数量统计
                int count = this.gridSeatPanel.Children.Count;

                if (this.selfSeatEntity != null)
                {
                    int number = seatEntity.SettingNummber;
                    if (this.selfSeatEntity.SettingNummber <= count / 2)
                    {
                        if (number <= count / 2)
                        {
                            //座位对应名称设置
                            this.dicSeatButton2[number + count / 2].Content = seatEntity.UserName;
                        }
                        else
                        {
                            //座位对应名称设置
                            this.dicSeatButton1[number - count / 2].Content = seatEntity.UserName;
                        }
                    }
                    else
                    {
                        if (number <= count / 2)
                        {
                            this.dicSeatButton1[number].Content = seatEntity.UserName;
                        }
                        else
                        {
                            this.dicSeatButton2[number].Content = seatEntity.UserName;
                        }
                    }
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

        #region 离开席位

        /// <summary>
        /// 离开席位
        /// </summary>
        public static void LeaveSeat()
        {
            try
            {
                //离开座位
                ModelManage.ConferenceMatrix.LeaveOneSeat(Constant.ConferenceName, Constant.SelfName, Constant.LocalIp, new Action<bool>((successed) =>
                {
                    if (successed)
                    {
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(MyConferenceView), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 显示提示

        public void TipShow(bool isShow)
        {
            try
            {
                if (isShow)
                {
                    this.Loading.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    this.Loading.Visibility = System.Windows.Visibility.Collapsed;
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
