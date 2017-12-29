using Conference_IMM.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.QRHelper;
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

namespace Conference_IMM
{
    /// <summary>
    /// QRWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QRWindow : Window
    {
        public QRWindow()
        {
            try
            {
                //加载UI
                InitializeComponent();

                //生成二维码图片
                System.Drawing.Image img = QRManage.CreateQRImg(IMMCodeEnterEntity.ConferenceWebAppAddress + "?username=" + IMMCodeEnterEntity.LoginUserName + "&ConferenceName=" + IMMCodeEnterEntity.ConferenceName);
                //绑定图片
                this.picture.Image = img;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }
    }
}
