using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_Space.Common
{
    class Oldsolution
    {
        #region old solution

        #region 移动分割

        ///// <summary>
        ///// 移动分割
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void btnPanelVisibilityControl_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        if (this.Column1.ActualWidth == 0)
        //        {
        //            //两列平分
        //            this.ColumnAllDisplay();
        //        }
        //        else
        //        {
        //            // 显示1列（左侧工作区）
        //            this.Column2Display();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #region 两列平分

        ///// <summary>
        ///// 两列平分
        ///// </summary>
        //public void ColumnAllDisplay()
        //{
        //    try
        //    {
        //        //平方两列
        //        this.Column1.Width = new GridLength((this.ActualWidth - Column2.ActualWidth) / 2);
        //        //显示书本
        //        this.gridBook.Visibility = System.Windows.Visibility.Visible;
        //        //隐藏
        //        //this.borerChange.Visibility = System.Windows.Visibility.Collapsed;
        //        //设置标示
        //        this.DisPlayAngle = 90;
        //        //清除签入的本地应用
        //        this.Panel.Controls.Clear();
        //        //设置
        //        //AppContainer.SetParent(this.intptr, panel.Handle);
        //        Win32API.MoveWindow(intptr, 0, 0, this.Panel.Width, this.Panel.Height, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

        #region 显示1列（左侧工作区）

        ///// <summary>
        ///// 显示1列（左侧工作区）
        ///// </summary>
        //public void Column2Display()
        //{
        //    try
        //    {
        //        //设置为1列
        //        this.Column1.Width = new GridLength(0);
        //        //隐藏书本
        //        this.gridBook.Visibility = System.Windows.Visibility.Collapsed;
        //        //隐藏
        //        //this.borerChange.Visibility = System.Windows.Visibility.Visible;
        //        //设置标示
        //        this.DisPlayAngle = 270;
        //        //设置
        //        //AppContainer.SetParent(this.intptr, panel.Handle);
        //        //Win32API.MoveWindow(intptr, 0, 0, (int)(this.Column1.ActualWidth + this.Column3.ActualWidth), this.Panel.Height, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

        #endregion

        #endregion

        #region 加载UI事件（比如多媒体播放器）

        ///// <summary>
        ///// 加载UI事件（比如多媒体播放器）
        ///// </summary>
        ///// <param name="element"></param>
        //public void book_LoadUI(FrameworkElement element)
        //{
        //    try
        //    {
        //        //隐藏本地officeUI
        //        this.Host.Visibility = System.Windows.Visibility.Collapsed;
        //        //隐藏装饰UI
        //        this.BorDecorate.Visibility = System.Windows.Visibility.Collapsed;
        //        //隐藏offcie遮挡面板
        //        //this.GridOfficePanel.Visibility = System.Windows.Visibility.Collapsed;
        //        //显示视频UI
        //        this.BorContent.Visibility = System.Windows.Visibility.Visible;
        //        //加载视频元素
        //        this.BorContent.Child = element;
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }

        //}

        #endregion

        #region 清除所有office文件

        ////未清除的话进行清除
        //     if (!SpaceBase.isOfficeClear)
        //     {                   
        //         //已清除
        //         SpaceBase.isOfficeClear = true;
        //     }

        ///// <summary>
        ///// 清除所有office文件
        ///// </summary>
        //public void OfficeDocumentsClear()
        //{
        //    try
        //    {
        //        ////移除Excel文件
        //        //Process[] pp = Process.GetProcessesByName("Excel");

        //        ////清除ppt文件
        //        //Process[] pp1 = Process.GetProcessesByName("POWERPNT");

        //        ////清除word文件
        //        //Process[] pp3 = Process.GetProcessesByName("WINWORD");

        //        //if (pp.Count() > 0 || pp1.Count() > 0 || pp3.Count() > 0)
        //        //{

        //        //    var result = MessageBox.Show("为了不影响使用，打开会议空间之前系统会将所有Excel文件,ppt文件，word文件关闭,请做好保存", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
        //        //    if (result == MessageBoxResult.OK)
        //        //    {
        //        //        foreach (var item in pp)
        //        //        {
        //        //            item.Kill();
        //        //        }

        //        //        foreach (var item in pp1)
        //        //        {
        //        //            item.Kill();
        //        //        }

        //        //        foreach (var item in pp3)
        //        //        {
        //        //            item.Kill();
        //        //        }
        //        //    }
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

        #region 根据句柄加载本地应用程序

        ///// <summary>
        ///// 根据句柄加载本地应用程序
        ///// </summary>
        ///// <param name="handle"></param>
        //public void book_OpenFileCompleate(IntPtr handle)
        //{
        //    try
        //    {
        //        this.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            try
        //            {
        //                //隐藏视频UI
        //                this.BorContent.Visibility = System.Windows.Visibility.Collapsed;
        //                //隐藏装饰UI
        //                this.BorDecorate.Visibility = System.Windows.Visibility.Collapsed;
        //                //显示offcie遮挡面板
        //                //this.GridOfficePanel.Visibility = System.Windows.Visibility.Visible;
        //                //显示本地officeUI
        //                this.Host.Visibility = System.Windows.Visibility.Visible;
        //                //置空视频UI
        //                this.BorContent.Child = null;

        //                //临时存储句柄
        //                intptr = handle;

        //                TimerJob.StartRun(new Action(() =>
        //                {
        //                    //本地office嵌入
        //                    AppContainer.SetParent(handle, Panel.Handle);
        //                    //设置合适尺寸
        //                    Win32API.MoveWindow(handle, 0, 0, this.Panel.Width, this.Panel.Height, false);
        //                }));
        //            }
        //            catch (Exception ex)
        //            {
        //                LogManage.WriteLog(this.GetType(), ex);
        //            };
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //}

        #endregion

          

    }
}
