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
using System.Windows.Shapes;

namespace Conference_Space
{
    /// <summary>
    /// InputMessageWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputMessageWindow : Window
    {
        #region 自定义委托事件(回调)
       
        /// <summary>
        /// 按钮确认事件回调
        /// </summary>
        public Action<string> OkEventCallBack = null;

        #endregion

        #region 构造函数

        public InputMessageWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region 确认事件

        /// <summary>
        /// 确认事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(this.OkEventCallBack!= null)
                {
                    this.OkEventCallBack(this.txtInput.Text.Trim());
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 取消事件

        /// <summary>
        /// 取消事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
