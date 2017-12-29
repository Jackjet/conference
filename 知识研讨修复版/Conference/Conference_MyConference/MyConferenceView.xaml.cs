using Conference_MyConference.Common;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.FileHelper;
using ConferenceCommon.ImageHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using ConferenceModel.ConferenceInfoWebService;
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


namespace Conference_MyConference
{
    /// <summary>
    /// MyConferenceView.xaml 的交互逻辑
    /// </summary>
    public partial class MyConferenceView : UserControlBase
    {
        #region 事件回调

        /// <summary>
        /// 填充数据回调
        /// </summary>
        public Action<DataGrid> FillDataGridCallBack = null;

        #endregion

        #region 绑定属性

        string presenterInfo;
        /// <summary>
        /// 会议主持人信息
        /// </summary>
        public string PresenterInfo
        {
            get { return presenterInfo; }
            set
            {
                if (presenterInfo != value)
                {
                    presenterInfo = value;
                    this.OnPropertyChanged("PresenterInfo");
                }
            }
        } 

        #endregion

        #region 静态字段

        /// <summary>
        /// 自我绑定
        /// </summary>
        public static MyConferenceView myConferenceView = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MyConferenceView()
        {
            try
            {
                //UI加载
                this.InitializeComponent();
              
                myConferenceView = this;

                //填充参会人列表
                this.FillDataSource();
                //初始化
                this.ParametersInit();
                //UI加载事件
                this.Loaded += MyConferenceView_Loaded;
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

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        private void ParametersInit()
        {
            try
            {
                if (ConferenceRoom_View.conferenceRoom_View.Parent != null && ConferenceRoom_View.conferenceRoom_View.Parent.GetType() == typeof(Border))
                {
                    Border bor1 = ConferenceRoom_View.conferenceRoom_View.Parent as Border;
                    bor1.Child = null;
                }

                //加载会议室
                this.bor1.Child = ConferenceRoom_View.conferenceRoom_View;
                
               
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

        /// <summary>
        /// UI加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MyConferenceView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
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

        #region 填充参会人列表

        /// <summary>
        /// 填充参会人列表
        /// </summary>
        public void FillDataSource()
        {
            try
            {
                ////会议名称
                //this.ConferenceName = MyConferenceCodeEnterEntity.ConferenceName;

                ////会议地址
                //this.ConferenceRoomName = MyConferenceCodeEnterEntity.ConferenceRoomName;
               
                if (this.FillDataGridCallBack != null)
                {
                    this.FillDataGridCallBack(this.datagrid);
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

        /// <summary>
        /// 子项更改事件
        /// </summary>
        public void DataGridItemChangeCallBack(ObservableCollection<ParticipantsEntity> currentParticipantsEntityList)
        {
            try
            {
                this.datagrid.ItemsSource = currentParticipantsEntityList.OrderBy(p => p.LoginState.Equals("未登录"));

                ////查看在线的情况
                //IEnumerable<ParticipantsEntity> list = currentParticipantsEntityList.Where(p => p.LoginState.Equals("在线"));
                //if(list.Count() ==0)
                //{

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

        #region 离开席位

        /// <summary>
        /// 离开席位
        /// </summary>
        public static void LeaveSeat()
        {
            try
            {
                //离开座位
                ModelManage.ConferenceMatrix.LeaveOneSeat(MyConferenceCodeEnterEntity.ConferenceName, MyConferenceCodeEnterEntity.SelfName, MyConferenceCodeEnterEntity.LocalIp, new Action<bool>((successed) =>
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

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="isShow"></param>
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

        #region 加入当前参与的一个会议

        /// <summary>
        /// 加入当前参与的一个会议
        /// </summary>
        public void JoinConfereince(Action<ConferenceInformationEntityPC> ClickCallBack)
        {
            try
            {

                ConferenceRoom_View.conferenceRoom_View.ConferenceRoomFlesh(new Action(() =>
                {
                    //会议开始时间和当前时间的间距
                    double timeSpace = 0;

                    //遍历所有会议信息（获取最接近当前时间的会议）
                    foreach (var item in ConferenceRoom_View.conferenceRoom_View.ConferenceInformationEntityList)
                    {
                        if (item.BeginTime < DateTime.Now.AddMinutes(-30))
                        {
                            //获取当前会议开始时间与当前时间的时间差
                            var timeSpace1 = (DateTime.Now - item.BeginTime).TotalSeconds;

                            if (timeSpace == 0 && item.JoinPeople.Contains(MyConferenceCodeEnterEntity.SelfUri))
                            {
                                timeSpace = timeSpace1;
                                MyConferenceCodeEnterEntity.TempConferenceInformationEntity = item;
                            }
                            //将更接近当前时间的会议做一个标示
                            else if (timeSpace1 < timeSpace && item.JoinPeople.Contains(MyConferenceCodeEnterEntity.SelfUri))
                            {
                                timeSpace = timeSpace1;
                                //设置为临时存储的会议
                                MyConferenceCodeEnterEntity.TempConferenceInformationEntity = item;
                            }
                        }
                    }

                    MyConferenceView.myConferenceView.TipShow(true);
                    //会议进入事件
                    ClickCallBack(MyConferenceCodeEnterEntity.TempConferenceInformationEntity);
                }));

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

        #region 进入一个桌位

        /// <summary>
        /// 进入一个桌位
        /// </summary>
        public void IntoOneSeat_AboutMyConference()
        {
            try
            {
                ModelManage.ConferenceMatrix.IntoOneSeat(MyConferenceCodeEnterEntity.ConferenceName, MyConferenceCodeEnterEntity.TempConferenceInformationEntity.SettingIpList, MyConferenceCodeEnterEntity.SelfName,
                    MyConferenceCodeEnterEntity.LocalIp, new Action<bool, List<ConferenceModel.ConferenceMatrixWebservice.SeatEntity>>((successed, seatEntityList) =>
                    {
                        if (successed)
                        {
                            //刷新座位分布情况
                            this.seatPanel.Reflesh(seatEntityList);

                            MyConferenceView.myConferenceView.TipShow(false);
                            //填充参会人信息
                            MyConferenceView.myConferenceView.FillDataSource();

                            //强制更改样式
                            ConferenceRoom_View.conferenceRoom_View.RoomItemForceToSelectedStyle(MyConferenceCodeEnterEntity.TempConferenceInformationEntity);
                        }
                    }));
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
                this.seatPanel.UpdateOneSeat(seatEntity);
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

        #region 主持人信息设置

        /// <summary>
        /// 主持人信息设置
        /// </summary>
        public void PresenterInfoSetting(string info)
        {
              try
            {
                //主持人信息
                this.PresenterInfo = info;
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
