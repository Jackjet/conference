using Conference_MyConference.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
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
using studiom = Studiom_Model.Pro_KnowledgeService;
using maxtrix = ConferenceModel.ConferenceMatrixWebservice;
using webCommonMaxtrix = ConferenceWebCommon.EntityHelper.ConferenceMatrix;

namespace Conference_MyConference
{
    /// <summary>
    /// SeatPanel.xaml 的交互逻辑
    /// </summary>
    public partial class SeatPanel_Ten : SeatPanelBase
    {
        #region 内部字段       

        /// <summary>
        /// 输出类型【0为输出1，1为输出2】
        /// </summary>
        int outputType = 0;

        #endregion

        #region 静态字段

        /// <summary>
        /// 天蓝画刷
        /// </summary>
        public SolidColorBrush AliceBlueBrush = new SolidColorBrush(Colors.AliceBlue);

        /// <summary>
        /// 浅绿画刷
        /// </summary>
        public SolidColorBrush LightGreenBrush = new SolidColorBrush(Colors.LightGreen);

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SeatPanel_Ten()
        {
            try
            {
                InitializeComponent();

                base.GridSeatPanel = this.gridSeatPanel;
                base.BorScreen1 = this.borScreen1;
                base.BorScreen2 = this.borScreen2;

                //事件注册
                this.EventRegedit();
                //座位初始化
                this.SeatDataFill();

               
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
                //投影座位1
                this.btnSetting1.Click += btnSetting_Click;
                this.btnSetting2.Click += btnSetting_Click;
                this.btnSetting3.Click += btnSetting_Click;
                this.btnSetting4.Click += btnSetting_Click;
                this.btnSetting5.Click += btnSetting_Click;
                this.btnSetting6.Click += btnSetting_Click;
                this.btnSetting7.Click += btnSetting_Click;
                this.btnSetting8.Click += btnSetting_Click;
                this.btnSetting9.Click += btnSetting_Click;
                this.btnSetting10.Click += btnSetting_Click;
                this.borScreen1.MouseLeftButtonDown += borScreen_MouseLeftButtonDown;
                this.borScreen2.MouseLeftButtonDown += borScreen_MouseLeftButtonDown;
                this.btnOther.Click += btnOther_Click;
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

        #region 屏幕选择

        /// <summary>
        /// 屏幕选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void borScreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (sender.Equals(this.borScreen1))
                {
                    this.borScreen1.Background = AliceBlueBrush;
                    this.borScreen2.Background = LightGreenBrush;
                    this.outputType = 0;
                }
                else if (sender.Equals(this.borScreen2))
                {
                    this.borScreen1.Background = LightGreenBrush;
                    this.borScreen2.Background = AliceBlueBrush;
                    this.outputType = 1;
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
         
        #region 投影座位

        /// <summary>
        /// 投影座位1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = (sender as NavicateButton);
                    studiom.MatrixChangeType matrixChangeType = default(studiom.MatrixChangeType);
                    if(this.outputType ==0)
                    {
                        matrixChangeType=(studiom.MatrixChangeType)(navicateButton.IntType * 2 - 2);
                    }
                    else if(this.outputType ==1)
                    {
                        matrixChangeType = (studiom.MatrixChangeType)(navicateButton.IntType * 2 - 1);
                    }
                    this.MaxtriChangeCenter(matrixChangeType);
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

        #region 切屏中心辅助

        /// <summary>
        /// 切屏中心辅助
        /// </summary>
        /// <param name="matrixChangeType"></param>
        public void MaxtriChangeCenter(studiom.MatrixChangeType matrixChangeType)
        {
            try
            {
                Studiom_Model.Constant.StudiomDataInstance.MatrixChange_AboutTen(matrixChangeType, new Action<string, string>((st1, st2) =>
            {
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

        #region 切换到外接设备

        /// <summary>
        /// 切换到外接设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnOther_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                studiom.MatrixChangeType matrixChangeType = (studiom.MatrixChangeType)((int)studiom.MatrixChangeType.ElevenToTwo - this.outputType);
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
    }
}
