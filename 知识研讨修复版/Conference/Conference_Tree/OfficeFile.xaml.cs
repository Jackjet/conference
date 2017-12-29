using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.FileHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
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
using vy = System.Windows.Visibility;
using office_File = ConferenceWebCommon.EntityHelper.ConferenceOffice;
using IOOperation = System.IO.Path;

namespace Conference_Tree
{
    /// <summary>
    /// OfficeFile.xaml 的交互逻辑
    /// </summary>
    public partial class OfficeFile : TreeView_ContentBase
    {
        #region 绑定属性

        vy collapsedVisibility = vy.Collapsed;
        /// <summary>
        /// 收缩按钮显示
        /// </summary>
        public vy CollapsedVisibility
        {
            get { return collapsedVisibility; }
            set
            {
                if (this.collapsedVisibility != value)
                {
                    collapsedVisibility = value;
                    this.OnPropertyChanged("CollapsedVisibility");
                }
            }
        }

        vy expanderVisibility = vy.Visible;
        /// <summary>
        /// 展开按钮显示
        /// </summary>
        public vy ExpanderVisibility
        {
            get { return expanderVisibility; }
            set
            {
                if (this.expanderVisibility != value)
                {
                    expanderVisibility = value;
                    this.OnPropertyChanged("ExpanderVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public OfficeFile()
        {
            try
            {
                //UI初始化
                InitializeComponent();
                //注册事件
                this.EventRegedit();
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

        #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //加载文件UI
                TreeCodeEnterEntity.fileOpenManage.LoadUICallBack += this.LoadUICallBack;
                //附加下载
                this.btnDownLoad.Click += btnDownLoad_Click;
                //收缩
                this.btnViewChange.Click += btnViewChange_Click;
                //视图查看
                this.cmbFileSizeChanged.SelectionChanged += cmbFileSizeChanged_SelectionChanged;
                //文件推送
                this.btnFileSend.Click += btnFileSend_Click;
                //文件共享
                this.btnFileShare.Click += btnFileShare_Click;
                //展开按钮
                this.btnViewChange_Expander.Click += btnViewChange_Click;
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

        #region 下载

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDownLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.DownLoad_File(this.downLoadProgress);
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

        #region 加载UI事件（比如多媒体播放器）

        /// <summary>
        /// 加载UI事件（比如多媒体播放器）
        /// </summary>
        /// <param name="element"></param>
        public void LoadUICallBack(FrameworkElement element)
        {
            try
            {
                ////隐藏本地officeUI
                //this.host.Visibility = System.Windows.Visibility.Collapsed;
                ////隐藏装饰UI
                //this.borDecorate.Visibility = System.Windows.Visibility.Collapsed;
                if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.OfficeFile)
                {
                    if (element.Parent is Border)
                    {
                        Border borParent = element.Parent as Border;
                        borParent.Child = null;
                    }
                    //加载视频元素
                    this.borContent.Child = element;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

       
    }
}
