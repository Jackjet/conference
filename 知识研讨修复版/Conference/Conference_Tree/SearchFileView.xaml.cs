using ConferenceCommon.EntityHelper;
using ConferenceCommon.JsonHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using wpfHelperFileType = ConferenceCommon.WPFControl.FileType;
using vy = System.Windows.Visibility;

namespace Conference_Tree
{
    /// <summary>
    /// SearchFileView.xaml 的交互逻辑
    /// </summary>
    public partial class SearchFileView : TreeView_ContentBase
    {
        #region 字段

        /// <summary>
        /// 选中的搜索文档
        /// </summary>
        SpSearchEntity spSearchEntity_Selected = null;

        #endregion

        #region 绑定属性

        vy view_Data_ShowVisibility = vy.Visible;
        /// <summary>
        /// 数据页面展示
        /// </summary>
        public vy View_Data_ShowVisibility
        {
            get { return view_Data_ShowVisibility; }
            set
            {
                if (this.view_Data_ShowVisibility != value)
                {
                    this.view_Data_ShowVisibility = value;
                    this.OnPropertyChanged("View_Data_ShowVisibility");
                }
            }
        }

        vy view_File_ShowVisibility = vy.Collapsed;
        /// <summary>
        /// 文档页面展示
        /// </summary>
        public vy View_File_ShowVisibility
        {
            get { return view_File_ShowVisibility; }
            set
            {
                if (this.view_File_ShowVisibility != value)
                {
                    this.view_File_ShowVisibility = value;
                    this.OnPropertyChanged("View_File_ShowVisibility");
                }
            }
        }

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

        vy loadingVisibility = vy.Collapsed;
        /// <summary>
        /// 加載提示
        /// </summary>
        public vy LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                if (this.loadingVisibility != value)
                {
                    loadingVisibility = value;
                    this.OnPropertyChanged("LoadingVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public SearchFileView()
        {
            try
            {
                InitializeComponent();

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
                this.datagrid.SelectionChanged += datagrid_SelectionChanged;
                this.btnView.Click += btnView_Click;
                TreeCodeEnterEntity.fileOpenManage.LoadUICallBack += LoadUICallBack;
                TreeCodeEnterEntity.fileOpenManage.DocumentLoadCompleateCallBack = DocumentLoadCompleateCallBack;
                this.btnBack.Click += btnBack_Click;
                //收缩
                this.btnViewChange.Click += btnViewChange_Click;
                //附加下载
                this.btnDownLoad.Click += btnDownLoad_Click;
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

        #region 加载UI事件（比如多媒体播放器）

        /// <summary>
        /// 加载UI事件（比如多媒体播放器）
        /// </summary>
        /// <param name="element"></param>
        public void LoadUICallBack(FrameworkElement element)
        {
            try
            {
                if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.SearchFile)
                {
                    if (element.Parent is Border)
                    {
                        Border borParent = element.Parent as Border;
                        borParent.Child = null;
                    }
                    //加载视频元素
                    this.borContent.Child = element;
                    this.Show_File_View();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #region 展示数据

        /// <summary>
        /// 展示数据
        /// </summary>
        /// <param name="message">搜索信息</param>
        public void Display(string message)
        {
            try
            {
                List<SpSearchEntity> SpSearchEntityList = JsonManage.JsonToEntity<SpSearchEntity>(message, ',');
                SpSearchEntityList.ForEach(item =>
                {
                    string path = item.Path;
                    item.Title = System.IO.Path.GetFileName(item.Path);                   
                });
                //展示搜索结果
                this.datagrid.ItemsSource = SpSearchEntityList;
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

        #region 选择项更改

        /// <summary>
        /// 选择项更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void datagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                object selectObject = this.datagrid.SelectedItem;
                if (selectObject is SpSearchEntity)
                {
                    this.spSearchEntity_Selected = selectObject as SpSearchEntity;
                    //保存当前打开的文件地址
                    TreeCodeEnterEntity.currentFileUri = spSearchEntity_Selected.Path;
                }
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

        #region 文档预览

        /// <summary>
        /// 文档预览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.spSearchEntity_Selected!=null&&this.spSearchEntity_Selected.FileType != null)
                {
                    wpfHelperFileType fileType = (wpfHelperFileType)Enum.Parse(typeof(wpfHelperFileType), this.spSearchEntity_Selected.FileType);

                    //打开文件
                    ConferenceTreeView.conferenceTreeView.FileOpenByExtension(fileType, this.spSearchEntity_Selected.Path);
                }
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

        #region 文档加载完成

        /// <summary>
        /// 文档加载完成
        /// </summary>
        private void DocumentLoadCompleateCallBack()
        {
            try
            {
                if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.SearchFile && this.View_File_ShowVisibility == vy.Visible)
                {
                    //TimerJob.StartRun(new Action(() =>
                    //    {
                    SendKeys.SendWait("^f");
                    SendKeys.SendWait("^v");
                    SendKeys.SendWait("{ENTER}");
                    //}),600);                    
                }
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

        #region 返回到搜索区域

        /// <summary>
        /// 返回到搜索区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Show_Data_View();
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

        #region 展示控制

        /// <summary>
        /// 搜索数据显示
        /// </summary>
        public void Show_Data_View()
        {
            try
            {
                this.View_Data_ShowVisibility = vy.Visible;
                this.View_File_ShowVisibility = vy.Collapsed;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 文档区域显示
        /// </summary>
        public void Show_File_View()
        {
            try
            {
                this.View_File_ShowVisibility = vy.Visible;
                this.View_Data_ShowVisibility = vy.Collapsed;
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
    }
}
