using ConferenceCommon.TimerHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.PictureHelper;
using ConferenceModel;
using ConferenceModel.FileSyncAppPoolWebService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;
using Controls = System.Windows.Controls;
using ConferenceCommon.WPFControl.DragView;

namespace Conference.View.AppcationTool
{
    /// <summary>
    /// 甩屏窗体
    /// </summary>
    public partial class AppSyncWindow : Window
    {
        #region 属性

        bool canSilding = true;
        /// <summary>
        /// 是否可以滑动,可以动态设置（当前程序正在运行时可以直接设置）
        /// </summary>
        public bool CanSilding
        {
            get { return canSilding; }
            set { canSilding = value; }
        }

        #endregion

        #region 字段

        ///// <summary>
        ///// 甩屏同步发送计时器
        ///// </summary>
        //DispatcherTimer syncSendTimer = null;

        ///// <summary>
        ///// 鼠标点击开始坐标
        ///// </summary>
        //Point starPosition;

        ///// <summary>
        ///// 鼠标点击释放结束坐标
        ///// </summary>
        //Point endPosition;

        ///// <summary>
        ///// 鼠标点击开始坐标
        ///// </summary>
        //TouchPoint starPosition1;

        ///// <summary>
        ///// 鼠标点击释放结束坐标
        ///// </summary>
        //TouchPoint endPosition1;

        ///// <summary>
        ///// 坐标X滑动参数
        ///// </summary>
        //double HeightY = -80;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public AppSyncWindow()
        {
            try
            {
                //UI加载
                InitializeComponent();
                //事件注册
                this.EventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
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
                //窗体关闭事件
                this.Closing += AppSyncWindow_Closing;
                //打开文件对话框
                this.btnOpenFile.PreviewMouseLeftButtonDown += btnOpenFile_PreviewMouseLeftButtonDown;
                //拖动完成事件
                this.tb.DragViewCompleateEventCallBack = tb_DragViewCompleateEvent;
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

        #region 关闭事件

        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        void AppSyncWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion


        #region 开启一个文件

        /// <summary>
        /// 开启一个文件
        /// </summary>
        void btnOpenFile_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.OpenFile();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开文件框并选择图片格式的文件进行甩屏操作
        /// </summary>
        private void OpenFile()
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                //打开选项对话框
                dialog.Filter = "图片文件(*.jpg,*.jpeg,*.png,*.gif)|*.jpeg;*.gif;*.png;*.jpg;)";
                //设置为多选
                dialog.Multiselect = true;
                if (dialog.ShowDialog() == true)
                {
                    var files = dialog.FileNames;

                    foreach (var file in files)
                    {
                        tb.AddItem(new DragTabItem() { Element = new Controls.Image() { Source = new BitmapImage(new Uri(file, UriKind.RelativeOrAbsolute)) } });
                    }
                    TimerJob.StartRun(new Action(() =>
                      {
                          //投影到大屏幕
                          this.CutToOtherClient();
                      }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 投影到大屏幕

        /// <summary>
        /// 拖动完成事件
        /// </summary>
        void tb_DragViewCompleateEvent()
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                       {
                           //投影到大屏幕
                           this.CutToOtherClient();
                       }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 投影到大屏幕
        /// </summary>
        public void CutToOtherClient()
        {
            try
            {
                FrameworkElement element = this.tb.GetElement();
                //主持人默认不可以使用甩屏操作（参会人进行甩屏，会议主持人进行接收）
                //if (!Constant.IsMeetingPresenter)
                //{
                PictureManage.ELementToArray((int)element.ActualWidth, (int)element.ActualHeight, element, new Action<byte[]>((bytes) =>
                     {
                         //将图片以字节数字的方式上传到服务器
                         ModelManage.FileSyncAppPool.FillSyncServiceData(Constant.ConferenceName, bytes, new Action<bool>((successed2) =>
                         {
                             if (successed2)
                             {

                             }
                         }));
                     }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
