
using ConferenceModel.LogHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace ConferenceCommon.TimerHelper
{
    public static class TimerModelJob
    {
        /// <summary>
        /// 自定义计时器（200毫秒触发一次，执行完指定的任务后结束计时器）
        /// </summary>
        /// <param name="action">执行的内容</param>
        public static void StartRun(Action action)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(200);
                timer.Tick += (object o, EventArgs e) =>
                {
                    try
                    {
                        action();
                        timer.Stop();
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        LogManage.WriteLog(typeof(TimerModelJob), ex);
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerModelJob), ex);
            }

        }
        /// <summary>
        /// 自定义计时器（指定时间内触发一次，执行完指定的任务后结束计时器）
        /// </summary>
        /// <param name="action">执行的内容</param>
        /// <param name="Milliseconds">指定时间</param>
        public static void StartRun(Action action, double Milliseconds)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(Milliseconds);
                timer.Tick += (object o, EventArgs e) =>
                {
                    try
                    {
                        action();
                        timer.Stop();
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        LogManage.WriteLog(typeof(TimerModelJob), ex);
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerModelJob), ex);
            }

        }

        /// <summary>
        /// 自定义计时器（指定时间内触发一次，执行完指定的任务后）
        /// </summary>
        /// <param name="action">执行的内容</param>
        /// <param name="Milliseconds">指定时间</param>
        public static void StartRunNoStop(Action action, double Milliseconds)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(Milliseconds);
                timer.Tick += (object o, EventArgs e) =>
                {
                    try
                    {
                        action();                       
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        LogManage.WriteLog(typeof(TimerModelJob), ex);
                    }
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerModelJob), ex);
            }

        }  

        /// <summary>
        /// 自定义计时器（指定时间内触发一次，在达到预期的目的之后，通过外部引用计时器来进行相应的操作（如结束计时器的生命周期））
        /// </summary>
        /// <param name="action">执行的内容</param>
        /// <param name="Milliseconds">指定时间</param>
        /// <param name="timer">向外引用计时器</param>
        public static void StartRun(Action action, double Milliseconds, out DispatcherTimer timer)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            timer = dispatcherTimer;
            try
            {
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Milliseconds);
                dispatcherTimer.Tick += (object o, EventArgs e) =>
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        dispatcherTimer.Stop();
                        LogManage.WriteLog(typeof(TimerModelJob), ex);
                    }
                };
                dispatcherTimer.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(TimerModelJob), ex);
            }
        }



        /// <summary>
        /// 自定义计时器（指定时间内触发一次，在达到预期的目的之后，通过外部引用计时器来进行相应的操作（如结束计时器的生命周期））
        /// </summary>
        /// <param name="action">执行的内容</param>
        /// <param name="Milliseconds">指定时间</param>
        /// <param name="timer">向外引用计时器</param>
        public static void StartRun(Action action, int actionCount, double Milliseconds)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();

            int count = 0;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Milliseconds);
            dispatcherTimer.Tick += (object o, EventArgs e) =>
            {
                //进行计时
                count++;
                //执行操作
                action();
                //达到指定操作数量，停止
                if (count.Equals(actionCount))
                {
                    dispatcherTimer.Stop();
                }
            };
            dispatcherTimer.Start();
        }


        /// <summary>
        /// 自定义计时器（指定时间内触发一次，在达到预期的目的之后，通过外部引用计时器来进行相应的操作（如结束计时器的生命周期））
        /// </summary>
        /// <param name="action">执行的内容</param>
        /// <param name="Milliseconds">指定时间</param>
        /// <param name="timer">向外引用计时器</param>
        public static void StartRun(Action action, int actionCount, double Milliseconds, out DispatcherTimer timer)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();

            int count = 0;

            timer = dispatcherTimer;

            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(Milliseconds);
            dispatcherTimer.Tick += (object o, EventArgs e) =>
            {
                //进行计时
                count++;
                //执行操作
                action();
                //达到指定操作数量，停止
                if (count.Equals(actionCount))
                {
                    dispatcherTimer.Stop();
                }
            };
            dispatcherTimer.Start();
        }


    }
}
