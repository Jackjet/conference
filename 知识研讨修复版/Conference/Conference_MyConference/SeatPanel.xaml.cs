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
    public partial class SeatPanel : SeatPanelBase
    {       
        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SeatPanel()
        {
            try
            {
                InitializeComponent();

                base.GridSeatPanel = this.gridSeatPanel;
                base.BorScreen1 = this.borScreen1;      
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
                    studiom.MaxtrixType matrixChangeType = (studiom.MaxtrixType)(navicateButton.IntType - 1);                    
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
        public void MaxtriChangeCenter(studiom.MaxtrixType matrixChangeType)
        {
            try
            {
                Studiom_Model.Constant.StudiomDataInstance.MatrixChange(matrixChangeType, new Action<string, string>((st1, st2) =>
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
        
        #region 切换到外接设备（在此为录音设备）

        /// <summary>
        /// 切换到外接设备
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnOther_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
                studiom.MaxtrixType matrixChangeType = studiom.MaxtrixType.maxtrix9;
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
