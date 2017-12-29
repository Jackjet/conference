using ConferenceCommon.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ConferenceCommon.TimerHelper
{
    public static class TimerDisplayManage
    {
        /// <summary>
        /// 时间显示
        /// </summary>
        public static void TimerDisplay(TextBlock txtNowTime)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += (object sender, EventArgs e) =>
                {
                    //显示当前时间
                    var nowTimer = DateTime.Now.ToString("   yyyy/MM/dd HH:mm:ss");
                    //
                    txtNowTime.Text = nowTimer;
                };
                //设置为每一秒更新一次
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerDisplayManage), ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 时间显示
        /// </summary>
        public static void TimerDisplay(TextBlock txtNowTime,Action callBack)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += (object sender, EventArgs e) =>
                {
                    //显示当前时间
                    var nowTimer = DateTime.Now.ToString("   yyyy年MM月dd日 HH:mm:ss");
                    //
                    txtNowTime.Text = nowTimer;

                    callBack();
                };
                //设置为每一秒更新一次
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerDisplayManage), ex);
            }
            finally
            {

            }
        }
    }
}
