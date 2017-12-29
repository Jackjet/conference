using ConferenceCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ConferenceCommon.AppContainHelper
{
    public class APPContainManage
    {

        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
      
        static Form form = new Form();
        public static void APP_Conatain(IntPtr handle)
        {
            try
            {
                AppContainer appBox = new AppContainer();
                AppContainer.SetParent(handle, form.Handle);
                AppContainer.SetWindowLong(new HandleRef(appBox, handle), GWL_STYLE, WS_VISIBLE);
                form.ShowInTaskbar = true;
                form.Width = 0;
                form.Height = 0;
                form.Show();
                form.Visible = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(APPContainManage), ex);
            }
        }

        static Form form2 = new Form();
        public static void APP_Conatain3(IntPtr handle)
        {
            try
            {
                AppContainer appBox = new AppContainer();
                AppContainer.SetParent(handle, form2.Handle);
                AppContainer.SetWindowLong(new HandleRef(appBox, handle), GWL_STYLE, WS_VISIBLE);
                form.ShowInTaskbar = false;
                form.Width = 0;
                form.Height = 0;
                form.Show();
                form.Visible = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(APPContainManage), ex);
            }
        }

        public static void APP_Close()
        {
            try
            {
                form.Close();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(APPContainManage), ex);
            }
        }


        public static Panel APP_Conatain2(IntPtr handle)
        {
            Panel panel = new Panel();
            try
            {            
                AppContainer appBox = new AppContainer();               
                AppContainer.SetParent(handle, panel.Handle);
                AppContainer.SetWindowLong(new HandleRef(appBox, handle), GWL_STYLE, WS_VISIBLE);               
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(APPContainManage), ex);
            }
            return panel;
        }
    }
}
