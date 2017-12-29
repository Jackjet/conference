using ConferenceCommon.LogHelper;
using ConferenceCommon.PictureHelper;
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

namespace Conference.View.AppcationTool
{
    /// <summary>
    /// 接受到了参会人甩屏给的数据,并弹出该窗体
    /// </summary>
    public partial class AppSyncDataAcceptWindow : Window
    {
        #region 构造函数

        public AppSyncDataAcceptWindow()
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

        #endregion

        #region 填充数据

        /// <summary>
        /// 视图填充
        /// </summary>
        /// <param name="bytes"></param>
        public void FillData(byte[] bytes)
        {
            try
            {
                //指定字节数组形式的图片
                BitmapImage image = PictureManage.ByteArrayToBitmapImage(bytes);
                //图片显示
                this.img.Source = image;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
