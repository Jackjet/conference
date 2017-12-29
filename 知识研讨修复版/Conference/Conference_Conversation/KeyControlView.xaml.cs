using ConferenceCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference_Conversation
{
    /// <summary>
    /// KeyControlView.xaml 的交互逻辑
    /// </summary>
    public partial class KeyControlView : System.Windows.Controls.UserControl
    {
        #region 构造函数
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public KeyControlView()
        {
            try
            {
                //UI加载
                InitializeComponent();

                //左按钮点击
                this.arrowLeft.PreviewMouseLeftButtonDown += arrowLeft_MouseLeftButtonDown;
                //右按钮点击
                this.arrowRight.PreviewMouseLeftButtonDown += arrowRight_MouseLeftButtonDown;
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

        #region 左按钮点击
        
        /// <summary>
        /// 左按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void arrowRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            try
            {               
                SendKeys.SendWait("{LEFT}");
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

        #region 右按钮点击
        
        /// <summary>
        /// 右按钮点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void arrowLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {              
                SendKeys.SendWait("{RIGHT}");               
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
