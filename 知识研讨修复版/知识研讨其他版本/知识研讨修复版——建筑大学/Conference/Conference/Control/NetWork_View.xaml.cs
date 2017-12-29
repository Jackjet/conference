using ConferenceModel.LogHelper;
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

namespace Conference.Control
{
    /// <summary>
    /// NetWork_View.xaml 的交互逻辑
    /// </summary>
    public partial class NetWork_View : UserControl
    {
        public NetWork_View()
        {
             try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        public void SetMessage(string message)
        {
             try
            {
                this.txtMessage.Text = message;
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
