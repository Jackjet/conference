using Conference.Page;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.ScreenSetting;
using ConferenceModel;
using Studiom_Model.Model;
using studiom = Studiom_Model.Pro_KnowledgeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using ConferenceCommon.VlcHelper;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using ConferenceCommon.WPFHelper;

namespace Conference.View.Chair
{
    /// <summary>
    /// ChairView.xaml 的交互逻辑
    /// </summary>
    public partial class ChairView : UserControl
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

        /// <summary>
        /// 是否投影到大屏
        /// </summary>
        bool IsLargeScreen = true;

        ///// <summary>
        ///// vlc流媒体管理器
        ///// </summary>
        //VlcManage vlcManage = new VlcManage();

        //  <summary>
        //  流媒体播放器
        //  </summary>
        public static VlcControl vlcPlayer = null;

        /// <summary>
        /// 流媒体播放器
        /// </summary>
        //VlcPlayer vlcPlayer = null;


        #endregion

        #region 静态字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChairView()
        {
            try
            {
                //UI加载
                InitializeComponent();

                #region 加载会议现场
                if (vlcPlayer == null)
                {
                    //初始化流媒体播放器
                    this.VlcMediaPlayerInit();
                }
                //播放视频流
                this.Play(Constant.ConferenceRoomCameraAddress);


                #endregion

                #region 事件注册

                //投影座位1
                this.btnSetting1.Click += btnSetting_Click;
                this.btnSetting2.Click += btnSetting_Click;
                this.btnSetting3.Click += btnSetting_Click;
                this.btnSetting4.Click += btnSetting_Click;
                this.btnSetting5.Click += btnSetting_Click;
                this.btnSetting6.Click += btnSetting_Click;
                this.btnSetting7.Click += btnSetting_Click;
                this.btnSetting8.Click += btnSetting_Click;
                //this.btnSetting11.Click += btnSetting_Click;
                //this.btnSetting12.Click += btnSetting_Click;
                //录播投影
                this.btnRecord.Click += btnRecord_Click;
                //选择大屏投影
                this.borScreen1.PreviewMouseLeftButtonDown += borScreen1_PreviewMouseLeftButtonDown;
                //选择小屏投影
                this.borScreen2.PreviewMouseLeftButtonDown += borScreen2_PreviewMouseLeftButtonDown;

                //同步我的会议页面
                this.btnMeet.Click += btnNavicate;
                //同步U盘传输页面
                this.btn_U_disk.Click += btnNavicate;
                //同步主持人功能页面
                this.btnChair.Click += btnNavicate;
                //同步信息交流页面
                this.btnIMM.Click += btnNavicate;
                //同步个人笔记页面
                this.btnNote.Click += btnNavicate;
                //同步资源共享页面
                this.btnResource.Click += btnNavicate;
                //同步智存空间页面
                this.btnSpace.Click += btnNavicate;
                //同步中控功能页面
                this.btnStudiom.Click += btnNavicate;
                //同步系统设置页面
                this.btnSystemSetting.Click += btnNavicate;
                //同步知识树页面
                this.btnTree.Click += btnNavicate;
                //同步投票页面
                this.btnWebBrowser.Click += btnNavicate;

                #endregion

                #region 座位统计

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

                #endregion



                //this.Play("UDP://@234.1.2.3:1234");
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

        #region 初始化流媒体播放器

        /// <summary>
        /// 初始化流媒体播放器
        /// </summary>m
        public void VlcMediaPlayerInit()
        {
            try
            {
                //vlc配置参数
                VlcContext.StartupOptions.IgnoreConfig = true;
                VlcContext.StartupOptions.LogOptions.LogInFile = false;
                VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
                VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.None;
                VlcContext.LibVlcPluginsPath = Environment.CurrentDirectory + "\\plugins";
                VlcContext.LibVlcDllsPath = Environment.CurrentDirectory;
                //流媒体播放器初始化
                VlcContext.Initialize();
                //播放器实例生成
                vlcPlayer = new VlcControl();


                // 创建绑定，绑定Image
                Binding bing = new Binding();
                bing.Source = vlcPlayer;
                bing.Path = new PropertyPath("VideoSource");
                this.img.SetBinding(Image.SourceProperty, bing);
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

        #region 流媒体播放器开始播放

        /// <summary>
        /// 流媒体播放器开始播放
        /// </summary>
        /// <param name="uri">播放地址</param>
        public void Play(string uri)
        {
            try
            {
                if (vlcPlayer != null)
                {
                    //设置播放地址
                    LocationMedia media = new LocationMedia(uri);
                    //PathMedia media = new PathMedia(uri);
                    //播放
                    vlcPlayer.Play(media);
                }

                //if (this.vlcPlayer == null)
                //{
                //    string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
                //    this.vlcPlayer = new VlcPlayer(pluginPath);
                //}
                //this.vlcPlayer.PlayFile(uri);
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

        #region 大小屏投影选择

        /// <summary>
        /// 选择小屏投影
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void borScreen2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsLargeScreen = false;
                //小屏样式更换
                this.borScreen2.Background = new SolidColorBrush(Colors.LightGreen);
                //大屏还原
                this.borScreen1.Background = new SolidColorBrush(Colors.AliceBlue);
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
        /// 选择大屏投影
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void borScreen1_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.IsLargeScreen = true;
                //小屏样式更换
                this.borScreen2.Background = new SolidColorBrush(Colors.AliceBlue);
                //大屏还原
                this.borScreen1.Background = new SolidColorBrush(Colors.LightGreen);
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

        #region 投影座位1

        /// <summary>
        /// 投影座位1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = (sender as Button);
                string UserName = Convert.ToString(btn.Content);
                ConferenceModel.ConferenceMatrixWebservice.ConferenceMatrixOutPut outputType = ConferenceModel.ConferenceMatrixWebservice.ConferenceMatrixOutPut.OutPut1;
                if (this.IsLargeScreen)
                {
                    outputType = ConferenceModel.ConferenceMatrixWebservice.ConferenceMatrixOutPut.OutPut1;
                }
                else
                {
                    outputType = ConferenceModel.ConferenceMatrixWebservice.ConferenceMatrixOutPut.OutPut2;
                }
                //矩阵应用
                ModelManage.ConferenceMatrix.MatrixSetting(Constant.ConferenceName, UserName, outputType, new Action<bool>((successed) =>
                {

                }));
                studiom.MatrixChangeType matrixChangeType = studiom.MatrixChangeType.OneToOne;
                if (btn.Tag != null)
                {
                    if (this.IsLargeScreen)
                    {
                        matrixChangeType = GetSeatMatrixChangeType(Convert.ToInt32(btn.Tag), 0);
                    }
                    else
                    {
                        matrixChangeType = GetSeatMatrixChangeType(Convert.ToInt32(btn.Tag), 1);
                    }
                }
                this.MaxtriChangeCenter(matrixChangeType);
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

        #region 切屏中心辅助

        public void MaxtriChangeCenter(studiom.MatrixChangeType matrixChangeType)
        {
            Studiom_Model.Constant.StudiomDataInstance.MatrixChange(matrixChangeType, new
                    Action<string, string>((innerError, serverError) =>
                    {

                    }));
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

                var selfSeatEntityList = seatEntity.Where(Item => Item.UserName != null &&
                      Item.UserName.Equals(Constant.SelfName)).ToList();
                if (selfSeatEntityList.Count > 0)
                {
                    this.selfSeatEntity = selfSeatEntityList[0];
                }
                if (selfSeatEntityList.Count > 0 && selfSeatEntityList[0].SettingNummber <= count / 2)
                {

                    //遍历设置座位
                    foreach (var item in seatEntity)
                    {
                        string content = "座位" + item.SettingNummber;
                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            content = item.UserName;
                        }
                        if (item.SettingNummber <= count / 2)
                        {
                            if (this.dicSeatButton2.ContainsKey(item.SettingNummber + count / 2))
                            {
                                this.dicSeatButton2[item.SettingNummber + count / 2].Content = content;
                                this.dicSeatButton2[item.SettingNummber + count / 2].Tag = item.SettingNummber;
                            }
                        }
                        else
                        {
                            if (this.dicSeatButton1.ContainsKey(item.SettingNummber - count / 2))
                            {
                                this.dicSeatButton1[item.SettingNummber - count / 2].Content = content;
                                this.dicSeatButton1[item.SettingNummber - count / 2].Tag = item.SettingNummber;
                            }
                        }

                    }
                }
                else
                {
                    //遍历设置座位
                    foreach (var item in seatEntity)
                    {
                        string content = "座位" + item.SettingNummber;
                        if (!string.IsNullOrEmpty(item.UserName))
                        {
                            content = item.UserName;
                        }
                        if (item.SettingNummber <= count / 2)
                        {
                            if (this.dicSeatButton1.ContainsKey(item.SettingNummber))
                            {
                                this.dicSeatButton1[item.SettingNummber].Content = content;
                                this.dicSeatButton1[item.SettingNummber].Tag = item.SettingNummber;
                            }
                        }
                        else
                        {
                            if (this.dicSeatButton2.ContainsKey(item.SettingNummber))
                            {
                                this.dicSeatButton2[item.SettingNummber].Content = content;
                                this.dicSeatButton2[item.SettingNummber].Tag = item.SettingNummber;
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

                    if (this.selfSeatEntity.SettingNummber <= count / 2)
                    {
                        if (seatEntity.SettingNummber <= count / 2)
                        {
                            //座位对应名称设置
                            this.dicSeatButton2[seatEntity.SettingNummber + count / 2].Content = seatEntity.UserName;
                        }
                        else
                        {
                            //座位对应名称设置
                            this.dicSeatButton1[seatEntity.SettingNummber - count / 2].Content = seatEntity.UserName;
                        }
                    }
                    else
                    {
                        if (seatEntity.SettingNummber <= count / 2)
                        {
                            this.dicSeatButton1[seatEntity.SettingNummber].Content = seatEntity.UserName;
                        }
                        else
                        {
                            this.dicSeatButton2[seatEntity.SettingNummber].Content = seatEntity.UserName;
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
                ////离开座位
                //ModelManage.ConferenceMatrix.LeaveOneSeat(Constant.ConferenceName, Constant.SelfName, Constant.LocalIp, new Action<bool>((successed) =>
                //    {
                //        if (successed)
                //        {
                //        }
                //    }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ChairView), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 页面同步

        void btnNavicate(object sender, RoutedEventArgs e)
        {
             try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.SyncPageHelper(navicateButton.ViewSelectedItemEnum);
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

        #region 页面同步辅助

        /// <summary>
        /// 页面同步辅助
        /// </summary>
        public void SyncPageHelper(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //页面同步
                ModelManage.ConferenceInfo.FillConferenceOfficeServiceData(Constant.ConferenceName, Constant.SelfName, (ConferenceModel.ConferenceInfoWebService.ConferencePageType)viewSelectedItemEnum, new Action<bool>((successed) =>
                {
                    if (successed)
                    {

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

        #region 接受到投影通知

        /// <summary>
        /// 接受到投影通知
        /// </summary>
        public void RecieveSilexNotification(string sharer, int outputType)
        {
            try
            {
                this.txtVoteTip.Visibility = System.Windows.Visibility.Visible;
                //当前投影人设置
                this.txtVotePersonName.Text = sharer;

                if (!string.IsNullOrEmpty(sharer))
                {
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

        #region 获取矩阵切换类型

        /// <summary>
        /// 获取矩阵切换类型
        /// </summary>
        /// <param name="seatNumber"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public studiom.MatrixChangeType GetSeatMatrixChangeType(int seatNumber, int outputType)
        {
            studiom.MatrixChangeType matrixChangeType = studiom.MatrixChangeType.OneToOne;
            try
            {
                if (seatNumber == 1 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.TwoToOne;
                }
                else if (seatNumber == 1 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.TwoToTwo;
                }
                else if (seatNumber == 2 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.ElevenToOne;
                }
                else if (seatNumber == 2 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.ElevenToTwo;
                }
                else if (seatNumber == 3 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.TenToOne;
                }
                else if (seatNumber == 3 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.TenToTwo;
                }
                else if (seatNumber == 4 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.NineToOne;
                }
                else if (seatNumber == 4 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.NineToTwo;
                }
                else if (seatNumber == 5 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.EightToOne;
                }
                else if (seatNumber == 5 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.EightToTwo;
                }
                else if (seatNumber == 6 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.SevenToOne;
                }
                else if (seatNumber == 6 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.SevenToTwo;
                }
                else if (seatNumber == 7 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.SixToOne;
                }
                else if (seatNumber == 7 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.SixToTwo;
                }
                else if (seatNumber == 8 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.FiveToOne;
                }
                else if (seatNumber == 8 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.FiveToTwo;
                }
                else if (seatNumber == 9 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.FourToOne;
                }
                else if (seatNumber == 9 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.FourToTwo;
                }
                else if (seatNumber == 10 && outputType == 0)
                {
                    matrixChangeType = studiom.MatrixChangeType.ThreeToOne;
                }
                else if (seatNumber == 10 && outputType == 1)
                {
                    matrixChangeType = studiom.MatrixChangeType.ThreeToTwo;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return matrixChangeType;
        }

        #endregion

        #region 切换到录播

        /// <summary>
        /// 启动录播
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                studiom.MatrixChangeType matrixChangeType = studiom.MatrixChangeType.OneToOne;
                if (this.IsLargeScreen)
                {
                    matrixChangeType = studiom.MatrixChangeType.OneToOne;
                }
                else
                {
                    matrixChangeType = studiom.MatrixChangeType.OneToTwo;
                }

                this.MaxtriChangeCenter(matrixChangeType);
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

        #region vlc流媒体资源释放

        #endregion
    }
}
