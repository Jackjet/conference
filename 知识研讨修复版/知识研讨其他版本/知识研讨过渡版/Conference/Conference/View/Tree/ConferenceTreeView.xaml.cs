using ConferenceCommon.TimerHelper;
using Conference.View.Space;
using ConferenceCommon.LogHelper;
using ConferenceModel;
using ConferenceModel.ConferenceTreeWebService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using SP = Microsoft.SharePoint.Client;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.OfficeHelper;
using Conference.Control;
using System.Threading;
using ConferenceCommon.SharePointHelper;
using Conference.View.ConferenceRoom;
using ConferenceCommon.WPFHelper;
using System.Windows.Threading;
using ConferenceCommon.FileHelper;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using form = System.Windows.Forms;
using Conference.Common;
using ConferenceCommon.WPFControl;
//xmlns:Common="clr-namespace:ConferenceCommon.WPFHelper;assembly=ConferenceCommon"
//xmlns:Common="clr-namespace:ConferenceCommon.WPFHelper;assembly=ConferenceCommon"
namespace Conference.View.Tree
{
    /// <summary>
    /// TreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeView : UserControlBase
    {
        #region 内部字段

        /// <summary>
        /// 视图缩放的宽度比例
        /// </summary>
        double constratParameterAboutWidth = 0;

        /// <summary>
        /// 视图缩放的高度比例
        /// </summary>
        double constratParameterAboutHeight = 0;

        /// <summary>
        /// 视图缩放的宽度倍数
        /// </summary>
        double changedConstratParameterAboutWidth = 1;

        /// <summary>
        /// 视图缩放的高度倍数
        /// </summary>
        double changedConstratParameterAboutHeight = 1;

        /// <summary>
        /// 图表统计
        /// </summary>
        ConferenceChartView ConferenceChart_View = null;

        /// <summary>
        /// 通过owa打开的文件uri后缀名
        /// </summary>
        string owaWebExtentionName = "?Web=1";

        /// <summary>
        /// word
        /// </summary>
        WordManage wordManage = new WordManage();

        /// <summary>
        /// excel
        /// </summary>
        ExcelManage excelManage = new ExcelManage();

        /// <summary>
        /// excel
        /// </summary>
        PPTManage pPTManage = new PPTManage();

        /// <summary>
        /// 视频播放器
        /// </summary>
        MediaPlayerView mediaPlayerView = null;

        /// <summary>
        /// 浏览器
        /// </summary>
        WebView webView = null;

        /// <summary>
        /// 知识树使用的浏览器（有差别）
        /// </summary>
        //TreeWebView treeWebView = null;

        /// <summary>
        /// 图片编辑器
        /// </summary>
        PictureView pictureView = null;

        /// <summary>
        /// 列表名称
        /// </summary>
        protected string listName = Constant.MeetingFolderName;

        /// <summary>
        /// 文件夹名称
        /// </summary>
        protected string folderName = Constant.ConferenceName;

        /// <summary>
        /// 个人空间列表
        /// </summary>
        List<SP.Folder> mettingF = null;

        /// <summary>
        /// 当前打开的文件uri地址
        /// </summary>
        string currentFileUri = null;

        /// <summary>
        ///  编辑控制计时器
        /// </summary>
        public static DispatcherTimer TittleEditControlTimer = null;

        /// <summary>
        /// 标题参照物
        /// </summary>
        string displayNumber = "1";

        ///// <summary>
        /////  编辑控制计时器2
        ///// </summary>
        //public DispatcherTimer CommentEditControlTimer = null;

        #endregion

        #region 绑定属性

        string meetingName = string.Empty;
        /// <summary>
        /// 研讨会议名称
        /// </summary>
        public string MeetingName
        {
            get { return meetingName; }
            set
            {
                if (this.meetingName != value)
                {
                    this.meetingName = value;
                    this.OnPropertyChanged("MeetingName");
                }
            }
        }


        Visibility loadingVisibility = Visibility.Visible;
        /// <summary>
        /// 加载提示显示状态
        /// </summary>
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                if (loadingVisibility != value)
                {
                    loadingVisibility = value;
                    this.OnPropertyChanged("LoadingVisibility");
                }
            }
        }

        Visibility relate_Visibility = Visibility.Collapsed;
        /// <summary>
        /// 获取和关联会议是否显示
        /// </summary>
        public Visibility Relate_Visibility
        {
            get { return relate_Visibility; }
            set
            {
                if (this.relate_Visibility != value)
                {
                    relate_Visibility = value;
                    this.OnPropertyChanged("Relate_Visibility");
                }
            }
        }

        #region old solution

        //double borTreePanelWidth;
        ///// <summary>
        ///// 树panel容器的宽度
        ///// </summary>
        //public double BorTreePanelWidth
        //{
        //    get { return borTreePanelWidth; }
        //    set
        //    {
        //        if (this.borTreePanelWidth != value)
        //        {
        //            borTreePanelWidth = value;
        //            this.OnPropertyChanged("BorTreePanelWidth");
        //        }
        //    }
        //}

        //double borTreePanelHeigth ;
        ///// <summary>
        ///// 树panel容器的高度
        ///// </summary>
        //public double BorTreePanelHeigth
        //{
        //    get { return borTreePanelHeigth; }
        //    set
        //    {
        //        if (this.borTreePanelHeigth != value)
        //        {
        //            borTreePanelHeigth = value;
        //            this.OnPropertyChanged("BorTreePanelHeigth");
        //        }
        //    }
        //}

        #endregion

        double viewBoxWidth;
        /// <summary>
        /// 视图容器的宽度
        /// </summary>
        public double ViewBoxWidth
        {
            get { return viewBoxWidth; }
            set
            {
                if (this.viewBoxWidth != value)
                {
                    viewBoxWidth = value;
                    this.OnPropertyChanged("ViewBoxWidth");
                }

            }
        }

        double viewBoxHeigth;
        /// <summary>
        /// 视图容器的高度
        /// </summary>
        public double ViewBoxHeigth
        {
            get { return viewBoxHeigth; }
            set
            {
                if (this.viewBoxHeigth != value)
                {
                    viewBoxHeigth = value;
                    this.OnPropertyChanged("ViewBoxHeigth");
                }
            }
        }

        Visibility downloadCommandVisibility = Visibility.Collapsed;
        /// <summary>
        /// 下载控制显示
        /// </summary>
        public Visibility DownloadCommandVisibility
        {
            get { return downloadCommandVisibility; }
            set
            {
                if (this.downloadCommandVisibility != value)
                {
                    downloadCommandVisibility = value;
                    this.OnPropertyChanged("DownloadCommandVisibility");
                }
            }
        }

        Visibility conferenceCommentCommandVisibility = Visibility.Collapsed;
        /// <summary>
        /// 会议纪要控制显示
        /// </summary>
        public Visibility ConferenceCommentCommandVisibility
        {
            get { return conferenceCommentCommandVisibility; }
            set
            {
                if (this.conferenceCommentCommandVisibility != value)
                {
                    conferenceCommentCommandVisibility = value;
                    this.OnPropertyChanged("ConferenceCommentCommandVisibility");
                }
            }
        }

        Visibility downLoadingVisibility = Visibility.Collapsed;
        /// <summary>
        /// 下载进行时提示
        /// </summary>
        public Visibility DownLoadingVisibility
        {
            get { return downLoadingVisibility; }
            set
            {
                if (this.downLoadingVisibility != value)
                {
                    downLoadingVisibility = value;
                    this.OnPropertyChanged("DownLoadingVisibility");
                }
            }
        }

        Visibility uploadFlgVisibility = Visibility.Collapsed;
        /// <summary>
        /// 上传完成提示
        /// </summary>
        public Visibility UploadFlgVisibility
        {
            get { return uploadFlgVisibility; }
            set
            {
                if (this.uploadFlgVisibility != value)
                {
                    uploadFlgVisibility = value;
                    this.OnPropertyChanged("UploadFlgVisibility");
                }
            }
        }

        Visibility uploadVisibility = Visibility.Collapsed;
        /// <summary>
        /// 上传进行时显示
        /// </summary>
        public Visibility UploadVisibility
        {
            get { return uploadVisibility; }
            set
            {
                if (this.uploadVisibility != value)
                {
                    uploadVisibility = value;
                    this.OnPropertyChanged("UploadVisibility");
                }
            }
        }

        Visibility summerUpdateVisibility = Visibility.Collapsed;
        /// <summary>
        /// 会议纪要更改提示
        /// </summary>
        public Visibility SummerUpdateVisibility
        {
            get { return summerUpdateVisibility; }
            set
            {
                if (this.summerUpdateVisibility != value)
                {
                    summerUpdateVisibility = value;
                    this.OnPropertyChanged("SummerUpdateVisibility");
                }
            }
        }

        #endregion

        #region 静态字段

        /// <summary>
        /// 标示静态事件是否注册，启动程序一次只允许注册一次（图表相关联相对较复杂）
        /// </summary>
        static bool StaticEventRegeditIsRegedit = false;

        /// <summary>
        /// 虚拟拖拽知识树
        /// </summary>
        public ConferenceTreeItemVisual conferenceTreeItemVisual = new ConferenceTreeItemVisual() { Visibility = Visibility.Collapsed, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

        #endregion

        #region 一般属性

        ConferenceTreeItem conferenceTreeItem;
        /// <summary>
        /// 知识树
        /// </summary>
        public ConferenceTreeItem ConferenceTreeItem
        {
            get
            {   //创建单例
                if (this.conferenceTreeItem == null)
                {
                    this.conferenceTreeItem = new ConferenceTreeItem(true);
                }
                return conferenceTreeItem;
            }
            set
            {
                conferenceTreeItem = value;
            }
        }

        string searchText;
        /// <summary>
        /// 查询文本
        /// </summary>
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; }
        }

        bool isStart = false;
        /// <summary>
        /// 启动实例标示
        /// </summary>
        public bool IsStart
        {
            get { return isStart; }
            set { isStart = value; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConferenceTreeView()
        {
            try
            {
                //UI加载
                InitializeComponent();

                //绑定当前上下文
                this.DataContext = this;

                //未注册则进行注册
                if (!ConferenceTreeView.StaticEventRegeditIsRegedit)
                {
                    //注册静态事件（全局只进行一次）
                    this.StaticEventRegedit();
                }
                //知识树加载
                this.borDiscussTheme.Child = this.ConferenceTreeItem;

                //添加虚拟知识树节点
                this.gridDiscussTheme.Children.Add(conferenceTreeItemVisual);

                #region 注册事件


                //保存（xml文件保存,研讨语音，树研讨）
                this.btnXMLSaved.Click += btnXMLSaved_Click;
                //打开图表视图
                this.btnChart.Click += btnChart_Click;
                //保存到本地
                this.btnLoaclSaved.Click += btnLoaclSaved_Click;
                //导入xml文件（知识树）
                this.btnLocalImport.Click += btnLocalImport_Click;

                //知识树显示
                this.btnProTree.Click += btnProTree_Click;

                //树视图放大
                this.btnEnLarge.Click += btnEnLarge_Click;
                //树视图还原
                this.btnReduction.Click += btnReduction_Click;
                //树视图缩小
                this.btnReduce.Click += btnReduce_Click;

                //树视图加载事件
                this.Loaded += ConferenceTreeView_Loaded;
                //生成会议纪要
                this.btnMeetSummary.Click += btnMeetSummary_Click;

                //生成平行层
                this.btnSamelevel.Click += btnSamelevel_Click;
                //生成子层
                this.btnChildlevel.Click += btnChildlevel_Click;
                //链接
                this.btnLink.Click += btnLink_Click;
                //备注
                this.btnNote.Click += btnNote_Click;
                //删除节点
                this.btnDelete.Click += btnDelete_Click;
                //附加下载
                this.btnDownLoad.Click += btnDownLoad_Click;
                //生成pdf版会议纪要
                this.btnCreatePDF.Click += btnCreatePDF_Click;
                //生成word版会议纪要
                this.btnCreateWord.Click += btnCreateWord_Click;
                //子项拖动完成事件
                this.MouseLeftButtonUp += ConferenceTreeView_PreviewMouseRightButtonUp;
                //子项相应鼠标移动
                this.borDiscussTheme.MouseMove += ConferenceTreeView_PreviewMouseMove;
                //上传会议纪要
                this.btnUpload.Click += btnUpload_Click;
                //刷新会议纪要
                this.btnReflesh.Click += btnReflesh_Click;
                //放大预览文件视图
                this.btnBigger.Click += btnBigger_Click;
                //缩小预览文件视图
                this.btnSmaller.Click += btnSmaller_Click;
                //设置预览文件视图默认大小
                this.btnDefault.Click += btnDefault_Click;
                //更改视图比例
                this.cmbTreeDisplay.SelectionChanged += cmbTreeDisplay_SelectionChanged;
                //更改文件预览视图比例
                this.cmbFileSizeChanged.SelectionChanged += cmbFileSizeChanged_SelectionChanged;

                #endregion

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //获取会议根目录
                    List<SP.Folder> mettingList = Constant.clientContextManage.GetFolders(listName);

                    //获取相关会议的所有文档
                    this.mettingF = mettingList.Where(Item => Item.Name.Equals(folderName)).ToList<SP.Folder>();
                });

                //文本框编辑权限控制器(标题)
                this.TittleLimitControlCenter();

                //文本框编辑权限控制器(评论)
                //this.CommentLimitControlCenter();
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

        #region 预览大小比例调整(文件)

        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBigger_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSmaller_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 设置预览文件视图默认大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDefault_Click(object sender, RoutedEventArgs e)
        {

        }

        void cmbFileSizeChanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.AddedItems[0];
                if (item is ComboBoxItem)
                {
                    ComboBoxItem comboBoxItem = item as ComboBoxItem;
                    string content = Convert.ToString(comboBoxItem.Content);
                    content = content.Replace(" %", string.Empty);
                    int contentInt = 0;
                    bool result = int.TryParse(content, out contentInt);
                    if (result)
                    {
                        //double widthChange = ((double)contentInt / 100);

                        //double heightChange = ((double)contentInt / 100);

                        this.webView.WebBrowser.Zoom(contentInt);

                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 文本框编辑权限控制器

        /// <summary>
        /// 文本框编辑权限控制器（标题）
        /// </summary>
        public void TittleLimitControlCenter()
        {
            try
            {
                //一个节点对应一个计时器
                if (TittleEditControlTimer == null)
                {
                    TimerJob.StartRun(new Action(() =>
                    {
                        ExpanderMethod();

                    }), 1000, out TittleEditControlTimer);
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

        ///// <summary>
        ///// 文本框编辑权限控制器（标题）
        ///// </summary>
        //public void CommentLimitControlCenter()
        //{
        //    try
        //    {
        //        //一个节点对应一个计时器
        //        if (this.CommentEditControlTimer == null)
        //        {
        //            TimerJob.StartRun(new Action(() =>
        //            {
        //                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
        //                {

        //                }
        //            }), 1000, out this.CommentEditControlTimer);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }
        //    finally
        //    {

        //    }
        //}

        #endregion

        #region 拖拽事件

        /// <summary>
        /// 子项相应鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConferenceTreeView_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {

                // 拖动终止位置      
                Point EndPoint;
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    return;

                ConferenceTreeItem item = ConferenceTreeItem.currentConferenceTreeItem;
                if (item != null && item.isDrag)
                {
                    EndPoint = e.GetPosition(this.borDiscussTheme);

                    //计算X、Y轴起始点与终止点之间的相对偏移量
                    double y = EndPoint.Y - item.StartPoint.Y;
                    double x = EndPoint.X - item.StartPoint.X;

                    //if (x > 10 || y > 10 || x < -10 || y < -10)
                    //{
                    Thickness margin = this.conferenceTreeItemVisual.Margin;

                    //计算新的Margin
                    Thickness newMargin = new Thickness()
                    {
                        Left = margin.Left + x,
                        Top = margin.Top + y,
                        Bottom = margin.Bottom,
                        Right = margin.Right
                    };
                    if (this.conferenceTreeItemVisual.Visibility != System.Windows.Visibility.Visible)
                    {
                        //显示虚拟拖拽节点
                        this.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Visible;
                    }
                    //设置移动效果
                    this.conferenceTreeItemVisual.Margin = newMargin;

                    //开始位置变为最终的位置
                    item.StartPoint = EndPoint;
                    //}
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 子项拖动完成（鼠标点击弹起事件）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConferenceTreeView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {

                if (this.conferenceTreeItemVisual.Visibility == System.Windows.Visibility.Visible)
                {
                    ConferenceTreeItem currentItem = ConferenceTreeItem.currentConferenceTreeItem;
                    if (currentItem != null)
                    {
                        //鼠标点击释放了，不可再拖动
                        currentItem.isDrag = false;


                        Point pos = e.GetPosition(this.borDiscussTheme);
                        HitTestResult result = VisualTreeHelper.HitTest(this.borDiscussTheme, pos);

                        if (result == null)
                        {
                            //显示虚拟拖拽节点
                            MainWindow.MainPageInstance.ConferenceTreeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Collapsed;

                            return;
                        }
                        else
                        {
                            //显示虚拟拖拽节点
                            MainWindow.MainPageInstance.ConferenceTreeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Collapsed;
                        }

                        ConferenceTreeItem conferenceTreeItem = WPFElementManage.FindVisualParent<ConferenceTreeItem>(result.VisualHit); // Find your actual visual you want to drag
                        if (conferenceTreeItem == null)
                            return;
                        if (conferenceTreeItem == currentItem)
                            return;

                        #region old solution

                        //ConferenceTreeItemTransferEntity beforeTransferItem = currentItem.AcademicReviewItemTransferEntity;
                        //ConferenceTreeItemTransferEntity nowTransferItem = conferenceTreeItem.AcademicReviewItemTransferEntity;
                        ////存储选择协议实体和目标协议实体标题
                        //string title1 = beforeTransferItem.Title;
                        //string title2 = nowTransferItem.Title;
                        ////存储选择协议实体和目标协议实体评论
                        //string coment1 = beforeTransferItem.Comment;
                        //string coment2 = nowTransferItem.Comment;
                        ////存储选择协议实体和目标协议实体链接
                        //List<string> list1 = beforeTransferItem.LinkList.ToList<string>();
                        //List<string> list2 = nowTransferItem.LinkList.ToList<string>();



                        //beforeTransferItem.Title = title2;
                        //nowTransferItem.Title = title1;

                        //beforeTransferItem.Comment = coment2;
                        //nowTransferItem.Comment = coment1;

                        //beforeTransferItem.LinkList = list2;
                        //nowTransferItem.LinkList = list1;

                        #endregion

                        if (CheckCanInsteadElement(currentItem, conferenceTreeItem))
                        {
                            //conferenceTreeItem.ContentVisibility = System.Windows.Visibility.Visible;
                            ModelManage.ConferenceTree.InsteadElement(Constant.ConferenceName, currentItem.ACA_Guid, conferenceTreeItem.ACA_Guid, new Action<bool>((successed) =>
                                {
                                }));
                        }
                    }
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

        #region 树视图加载事件

        /// <summary>
        /// 树视图加载事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceTreeView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //若为主持人,可进行导入和保存到服务器的操作
                if (Constant.IsMeetingPresenter)
                {
                    this.Relate_Visibility = System.Windows.Visibility.Visible;
                }

                TimerJob.StartRun(new Action(() =>
                    {
                        //初始化视图容器的宽度
                        this.ViewBoxWidth = this.borDiscussTheme.ActualWidth;
                        //初始化视图容器的高度
                        this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight;

                        ///长加宽
                        var widthAddHeigth = this.ViewBoxWidth + this.ViewBoxHeigth;

                        //设置视图缩放的宽度比例
                        this.constratParameterAboutWidth = this.ViewBoxWidth / widthAddHeigth;
                        //设置视图缩放的高度比例
                        this.constratParameterAboutHeight = this.ViewBoxHeigth / widthAddHeigth;
                    }), 1000);
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

        #region 保存到本地

        /// <summary>
        /// 保存到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLoaclSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存到本地
                this.SevedXmlLocal();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        public void SevedXmlLocal()
        {
            try
            {
                //存储文件对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //设置文件类型
                //书写规则例如：txt files(*.txt)|*.txt
                saveFileDialog.Filter = "txt files(*.xml)|*.xml|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                //设置默认文件名（可以不设置）
                saveFileDialog.FileName = Constant.TreeXmlFileName;
                //主设置默认文件extension（可以不设置）
                saveFileDialog.DefaultExt = "xml";
                //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                saveFileDialog.AddExtension = true;

                ////设置默认文件类型显示顺序（可以不设置）
                //saveFileDialog.FilterIndex = 0;

                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == true)
                {
                    //获取当前服务器所缓存的知识树
                    ModelManage.ConferenceTree.GetAll(Constant.ConferenceName, new Action<ConferenceTreeInitRefleshEntity>((result) =>
                    {
                        //声明序列化对象实例serializer 
                        XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));
                        //研讨树序列化并进行存储（存储到SharePoint的服务器里）
                        using (FileStream ms = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                        {
                            //序列化
                            serializer.Serialize(ms, result);
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 打开图表视图

        /// <summary>
        /// 打开图表视图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnChart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Chart_Show();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 图表显示
        /// </summary>
        private void Chart_Show()
        {
            try
            {
                if (this.ConferenceChart_View == null)
                {
                    //创建图表
                    this.ConferenceChart_View = new ConferenceChartView();
                    this.borChart.Child = this.ConferenceChart_View;
                }

                if (this.borChart.Visibility == System.Windows.Visibility.Hidden || this.borChart.Visibility == System.Windows.Visibility.Collapsed)
                {
                    ///添加图表分类
                    this.ConferenceChart_View.ChartTittlesAdd(Constant.VoteChatTittleList);

                    /////填充图表
                    //foreach (var item in ConferenceTreeItem2.AcademicReviewItemList)
                    //{
                    //    this.ConferenceChart_View.ChartItemsAdd(item.ACA_Tittle, item.ACA_YesVoteCount, item.ACA_NoVoteCount, item.ACA_Guid);
                    //}

                    TimerJob.StartRun(new Action(() =>
                    {
                        //显示图表窗体
                        this.borChart.Visibility = System.Windows.Visibility.Visible;

                        //设置知识树按钮激活颜色
                        this.btnProTree.Background = new SolidColorBrush(Colors.White);
                        //恢复图表按钮颜色
                        this.btnChart.Background = new SolidColorBrush(Colors.LightGreen);

                    }), 100);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 投票树节点更改事件
        /// </summary>
        /// <param name="tittle"></param>
        /// <param name="YesVoteCount"></param>
        /// <param name="NoVoteCount"></param>
        void ConferenceTree_VoteChangedEvent(string tittle, int YesVoteCount, int NoVoteCount, int Guid)
        {
            try
            {
                //图表统计显示的情况下才进行同步
                if (this.ConferenceChart_View != null && this.ConferenceChart_View.Visibility == System.Windows.Visibility.Visible)
                {
                    //图表更改子项（投票）
                    this.ConferenceChart_View.ChartItemsUpdate(tittle, YesVoteCount, NoVoteCount, Guid);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 图标投票节点删除事件
        /// </summary>
        /// <param name="tittle"></param>
        void ConferenceTreeItem_VoteTreeItemRemoveEvent(int Guid)
        {
            try
            {
                //图表统计显示的情况下才进行同步
                if (this.ConferenceChart_View != null && this.ConferenceChart_View.Visibility == System.Windows.Visibility.Visible)
                {
                    //图表删除子项（投票）
                    this.ConferenceChart_View.VoteTreeItemRemove(Guid);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        //图表投票节点添加事件
        void ConferenceTreeItem_VoteTreeItemAddEvent(string tittle, int YesVoteCount, int NoVoteCount, int Guid)
        {
            try
            {
                //图表统计显示的情况下才进行同步
                if (this.ConferenceChart_View != null && this.ConferenceChart_View.Visibility == System.Windows.Visibility.Visible)
                {
                    //图表添加子项（投票）
                    this.ConferenceChart_View.ChartItemsAdd(tittle, YesVoteCount, NoVoteCount, Guid);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 保存（xml文件保存,研讨语音，树研讨）

        /// <summary>
        /// 保存（xml文件保存,研讨语音，树研讨）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnXMLSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.XMLSaved();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 保存（xml文件保存,研讨语音，树研讨）
        /// </summary>
        private void XMLSaved()
        {
            try
            {

                ModelManage.ConferenceTree.GetAll(Constant.ConferenceName, new Action<ConferenceTreeInitRefleshEntity>((result) =>
                {
                    //研讨树综述窗体
                    SummarizeWindow window = new SummarizeWindow();

                    #region old solution

                    //设置窗体显示位置
                    //var left = (MainWindow.DisCuss_View.ActualWidth - MainWindow.DisCuss_View.Column1Width - window.Width) / 2;
                    //var top = 110 + 55 + (MainWindow.DisCuss_View.ActualHeight - 55 - window.Height) / 2;
                    //window.Left = left + 70 + 10 + MainWindow.DisCuss_View.Column1Width;
                    //window.Top = top;

                    #endregion

                    //获取会议根目录
                    var mettingList = Constant.clientContextManage.GetFolders(Constant.MeetingFolderName);

                    //获取相关会议的所有文档
                    var mettingF = mettingList.Where(Item => Item.Name.Equals(Constant.ConferenceName)).ToList<SP.Folder>();

                    window.IsOkEvent += (string message) =>
                    {

                        #region 上传会议知识树节点数据

                        //声明序列化对象实例serializer 
                        XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));

                        //研讨树序列化并进行存储（存储到SharePoint的服务器里）
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Position = 0;
                            result.Summarize = message;
                            //序列化
                            serializer.Serialize(ms, result);

                            byte[] bytes = ms.ToArray();

                            if (mettingF.Count > 0)
                            {
                                Constant.clientContextManage.UploadFileToFolder(mettingF[0], Constant.ConferenceName + ".xml", bytes);
                            }
                        }

                        #endregion

                        #region 上传会议知识树图片

                        //研讨树图片序列化并进行存储（存储到SharePoint的服务器里）
                        using (MemoryStream ms = new MemoryStream())
                        {
                            RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.borDiscussTheme.ActualWidth, (int)this.borDiscussTheme.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                            bmp.Render(this.borDiscussTheme);
                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                            encoder.Save(ms);

                            ms.Position = 0;
                            byte[] bytes = new byte[ms.Length];
                            ms.Read(bytes, 0, bytes.Length);

                            if (mettingF.Count > 0)
                            {
                                Constant.clientContextManage.UploadFileToFolder(mettingF[0], Constant.ConferenceName + ".png", bytes);
                                window.Close();
                            }
                        }

                        #endregion

                        #region 上传会议视频链接

                        //全名称
                        var fullName = this.GetRecordFileFullName(Constant.RecordFolderName, Constant.RecordExtention);
                        //当前名称                                 
                        //判断是否存在该文件
                        if (File.Exists(fullName))
                        {
                            #region old solution

                            ////使用内存流
                            //using (MemoryStream ms = new MemoryStream())
                            //{
                            //    ms.Position = 0;
                            //    //将文件写入流【utf8】
                            //    StreamWriter sw = new StreamWriter(ms,Encoding.UTF8);

                            //    sw.Write(fullName);

                            //    byte[] bytes = ms.ToArray();

                            #endregion

                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(fullName);

                            if (mettingF.Count > 0)
                            {
                                Constant.clientContextManage.UploadFileToFolder(mettingF[0], Constant.ReacordUploadFileName, bytes);
                            }
                        }
                        //}
                        #endregion
                    };
                    window.Show();
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 获取录制视频文件
        /// </summary>
        /// <param name="recordVideoSavedPlace">录播视频存储地址</param>
        /// <param name="recordVideoExtention">录播视频格式</param>
        /// <returns></returns>
        public string GetRecordFileFullName(string recordVideoSavedPlace, string recordVideoExtention)
        {
            string fullName = string.Empty;
            try
            {
                //判断录播的路径
                if (Directory.Exists(recordVideoSavedPlace))
                {
                    //操作文件夹
                    DirectoryInfo directoryInfo = new DirectoryInfo(recordVideoSavedPlace);
                    //获取该文件夹下的所有文件夹
                    DirectoryInfo[] directoryInfoes = directoryInfo.GetDirectories();

                    DirectoryInfo directoryInfoRecently = null;

                    //判断创建时间
                    double longerCreatimer = 0;

                    //遍历文件夹获取时间最近时间创建的文件夹
                    foreach (var item in directoryInfoes)
                    {
                        if (longerCreatimer == 0)
                        {
                            longerCreatimer = (DateTime.Now - item.CreationTime).TotalSeconds;
                            directoryInfoRecently = item;
                        }
                        else if (item.CreationTime.Second < longerCreatimer)
                        {
                            longerCreatimer = (DateTime.Now - item.CreationTime).TotalSeconds;
                            directoryInfoRecently = item;
                        }
                    }

                    //获取到的视频文件
                    FileInfo fileInfo = null;

                    //获取到最近时间创建的文件夹
                    if (directoryInfoRecently != null)
                    {
                        var mp4File = directoryInfoRecently.GetFiles(recordVideoExtention);
                        if (mp4File.Length > 0)
                        {
                            fileInfo = mp4File[0];
                            //获取录制视频的全名称
                            fullName = fileInfo.FullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

            return fullName;
        }

        #endregion

        #region 导入xml文件（知识树）

        /// <summary>
        /// 导入xml文件（知识树）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLocalImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.XmlImport();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 导入xml文件（知识树）
        /// </summary>
        public void XmlImport()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "txt files(*.xml)|*.xml|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    //声明序列化对象实例serializer 
                    XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));

                    //生成文件到指定路径
                    using (FileStream ms = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        //反序列化获取协议实体
                        var result = serializer.Deserialize(ms);
                        if (result is ConferenceTreeInitRefleshEntity)
                        {
                            //反序列化，并将反序列化结果值赋给变量i
                            ConferenceTreeInitRefleshEntity conferenceTreeInitRefleshEntity = result as ConferenceTreeInitRefleshEntity;
                            //协议实体参会人清空
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.ParticipantList.Clear();
                            //协议实体参会人重新添加为当前会议的参会人
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.ParticipantList.AddRange(Constant.ParticipantList);
                            //清除协议实体操作人
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.Operationer = string.Empty;
                            //重新构造服务器中的研讨树
                            ModelManage.ConferenceTree.SetAll(Constant.ConferenceName, conferenceTreeInitRefleshEntity, new Action<bool>((error2) =>
                            {

                            }));
                        }
                        else
                        {
                            MessageBox.Show("文件异常", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 知识树显示

        /// <summary>
        /// 知识树显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnProTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //隐藏图表窗体
                this.borChart.Visibility = System.Windows.Visibility.Hidden;
                //设置知识树按钮激活颜色
                this.btnProTree.Background = new SolidColorBrush(Colors.LightGreen);
                //恢复图表按钮颜色
                this.btnChart.Background = new SolidColorBrush(Colors.White);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 视图尺寸更改（定制化【放大、还原、缩小】）

        #region 放大

        /// <summary>
        /// 树视图放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEnLarge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图放大
                this.EnLarge();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图放大
        /// </summary>
        public void EnLarge()
        {
            try
            {
                //按比例增加视图宽度
                this.ViewBoxWidth += 100 * this.constratParameterAboutWidth;
                // //按比例增加视图高度
                this.ViewBoxHeigth += 100 * this.constratParameterAboutHeight;
                //视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = this.ViewBoxWidth / this.borDiscussTheme.ActualWidth;
                //视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = this.ViewBoxHeigth / this.borDiscussTheme.ActualHeight;
                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #region 百分比显示

        /// <summary>
        /// 百分比显示
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string PersentDisplay(double count)
        {
            string aa = string.Empty;
            try
            {
                double m = count * 100;
                aa = m.ToString("0") + "%";

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return aa;
        }

        #endregion

        #endregion

        #region 还原

        /// <summary>
        /// 树视图还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图还原
                this.Reduction();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图还原
        /// </summary>
        public void Reduction()
        {
            try
            {
                //还原视图宽度
                this.ViewBoxWidth = this.borDiscussTheme.ActualWidth;
                //还原视图高度
                this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight;
                //还原视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = 1;
                //还原视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = 1;

                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 缩小

        /// <summary>
        /// 树视图缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduce_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图缩小
                this.Reduce();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图缩小
        /// </summary>
        public void Reduce()
        {
            try
            {
                //按比例减少视图宽度
                this.ViewBoxWidth -= 100 * this.constratParameterAboutWidth;
                //按比例减少视图高度
                this.ViewBoxHeigth -= 100 * this.constratParameterAboutHeight;

                //视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = this.ViewBoxWidth / this.borDiscussTheme.ActualWidth;
                //视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = this.ViewBoxHeigth / this.borDiscussTheme.ActualHeight;
                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        /// <summary>
        /// 更改视图比例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbTreeDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.AddedItems[0];
                if (item is ComboBoxItem)
                {
                    ComboBoxItem comboBoxItem = item as ComboBoxItem;
                    string content = Convert.ToString(comboBoxItem.Content);
                    content = content.Replace(" %", string.Empty);
                    int contentInt = 0;
                    bool result = int.TryParse(content, out contentInt);
                    if (result)
                    {
                        double widthChange = ((double)contentInt / 100);

                        double heightChange = ((double)contentInt / 100);

                        //按比例减少视图宽度
                        this.ViewBoxWidth = this.borDiscussTheme.ActualWidth * widthChange;
                        //按比例减少视图高度
                        this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight * heightChange;

                        //视图缩放的宽度倍数
                        this.changedConstratParameterAboutWidth = widthChange;
                        //视图缩放的高度倍数
                        this.changedConstratParameterAboutHeight = heightChange;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 静态事件

        /// <summary>
        /// 注册静态事件（全局只进行一次）
        /// </summary>
        public void StaticEventRegedit()
        {
            //投票更改事件
            //ConferenceTreeItem.VoteChangedEvent += ConferenceTree_VoteChangedEvent;
            //投票的相关节点添加事件
            //ConferenceTreeItem.VoteTreeItemAddEvent += ConferenceTreeItem_VoteTreeItemAddEvent;
            //投票的相关节点删除事件
            //ConferenceTreeItem.VoteTreeItemRemoveEvent += ConferenceTreeItem_VoteTreeItemRemoveEvent;
            //更新整棵研讨树完成事件
            ConferenceTreeItem.RefleshCompleateEvent += ConferenceTreeItem_RefleshCompleateEvent;

            //知识树里的静态事件标示为已注册
            ConferenceTreeView.StaticEventRegeditIsRegedit = true;
        }

        #endregion

        #region 知识树更新完成事件

        /// <summary>
        /// 知识树更新完成事件
        /// </summary>
        void ConferenceTreeItem_RefleshCompleateEvent()
        {
            try
            {
                if (this.ConferenceChart_View != null)
                {
                    this.ConferenceChart_View.ClearAllPoints();
                    /////填充图表
                    //foreach (var item in ConferenceTreeItem2.AcademicReviewItemList)
                    //{
                    //this.ConferenceChart_View.ChartItemsAdd(item.ACA_Tittle, item.ACA_YesVoteCount, item.ACA_NoVoteCount, item.ACA_Guid);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 研讨树节点搜索

        /// <summary>
        /// 研讨树节点搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //获取搜索文本
                this.SearchText = (sender as TextBox).Text;
                //搜索信息若为空，则清除阴影
                if (string.IsNullOrEmpty(this.SearchText))
                {
                    //清除阴影
                    foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                    {
                        if (item.BorVisibility == System.Windows.Visibility.Visible)
                        {
                            item.BorVisibility = System.Windows.Visibility.Hidden;
                        }
                    }
                }
                //记录所有知识树的记录（确保有记录）
                else if (ConferenceTreeItem.AcademicReviewItemList.Count > 0)
                {
                    //遍历记录所有知识树的记录
                    foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                    {
                        //标题不能为空（包含该关键字分为3块【1、标题 2、评论 3、简介】）
                        if ((!string.IsNullOrEmpty(item.ACA_Tittle) && item.ACA_Tittle.Contains(this.SearchText)) || (!string.IsNullOrEmpty(item.ACA_Comment) && item.ACA_Comment.Contains(this.SearchText)))
                        {
                            item.BorVisibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            item.BorVisibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 初始化视图容器

        /// <summary>
        /// 初始化视图容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewBox_Load(object sender, RoutedEventArgs e)
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

        #region 树容器尺寸更改事件

        /// <summary>
        /// 树容器尺寸更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void borDiscussTheme_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                //按比例设置视图的宽度
                this.ViewBoxWidth = this.borDiscussTheme.ActualWidth * this.changedConstratParameterAboutWidth;
                //按比例了设置视图的高度
                this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight * this.changedConstratParameterAboutHeight;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 生成会议纪要

        /// <summary>
        /// 生成会议纪要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnMeetSummary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //显示生成会议纪要
                this.ConferenceCommentCommandVisibility = System.Windows.Visibility.Visible;
                //隐藏下载控制
                this.DownloadCommandVisibility = System.Windows.Visibility.Collapsed;
                //隐藏会议纪要更改提示
                this.SummerUpdateVisibility = System.Windows.Visibility.Collapsed;

                this.BuildMeetSummary();
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
        /// 使用递归的方式填充会议纪要的内容
        /// </summary>
        void SetNextLineAboutMeetSummary(ConferenceTreeItem treeParent, string number, ref StringBuilder builder)
        {
            try
            {

                if (treeParent.ACA_ChildList.Count > 0)
                {
                    int count = 0;
                    foreach (var item in treeParent.ACA_ChildList)
                    {
                        count++;
                        if (treeParent.ACA_Parent == null)
                        {
                            displayNumber = Convert.ToString(count);
                        }
                        else
                        {
                            displayNumber = number + "." + count;
                        }

                        this.SetLineValue(item, ref builder);

                        this.SetNextLineAboutMeetSummary(item, displayNumber, ref builder);
                    }

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 填充一行内容
        /// </summary>
        /// <param name="item"></param>
        /// <param name="builder"></param>
        void SetLineValue(ConferenceTreeItem item, ref StringBuilder builder)
        {
            try
            {
                string pNormal = @"<p class=MsoNormal align=left style='text-align:left'><b><span lang=EN-US
  style='font-size:12pt'>" + (displayNumber) + "、" + item.ACA_Tittle + "</span></span></b></p>";
                builder.Append(pNormal);
                if (!string.IsNullOrEmpty(item.ACA_Comment))
                {
                    string pNormal2 = @"<p  align=left style='text-align:left'><b><span lang=EN-US
  style='font-size:12pt'>" + "   备注：" + item.ACA_Comment + "</span></span></b></p>";
                    builder.Append(pNormal2);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 构建会议纪要
        /// </summary>
        public void BuildMeetSummary()
        {
            try
            {
                //通过浏览器的方式打开
                this.OpenFileByBrowser(Constant.PaintFileRoot + "/" + Constant.ConferenceCommentHtmlTemp);

                TimerJob.StartRun(new Action(() =>
                {
                    if (this.webView.WebBrowser.Document != null)
                    {
                        //获取当前的会议信息
                        ConferenceModel.ConferenceInfoWebService.ConferenceInformationEntityPC ConferenceInformationEntityPC = MainWindow.MainPageInstance.TempConferenceInformationEntity;
                        //获取会议名称
                        System.Windows.Forms.HtmlElement htmlElement = this.webView.WebBrowser.Document.GetElementById("txtMeetName");
                        htmlElement.InnerHtml = ConferenceInformationEntityPC.MeetingName;
                        //获取开始时间
                        System.Windows.Forms.HtmlElement htmlElement2 = this.webView.WebBrowser.Document.GetElementById("txtMeetTime1");
                        htmlElement2.InnerHtml = ConferenceInformationEntityPC.BeginTime.ToString("yyyy-MM-dd hh:mm");
                        //获取结束时间
                        System.Windows.Forms.HtmlElement htmlElement3 = this.webView.WebBrowser.Document.GetElementById("txtMeetTime2");
                        htmlElement3.InnerHtml = ConferenceInformationEntityPC.EndTime.ToString("yyyy-MM-dd hh:mm");
                        //获取会议室名称
                        System.Windows.Forms.HtmlElement htmlElement4 = this.webView.WebBrowser.Document.GetElementById("txtMeetPlace");
                        htmlElement4.InnerHtml = ConferenceInformationEntityPC.RoomName;
                        //获取主持人
                        System.Windows.Forms.HtmlElement htmlElement5 = this.webView.WebBrowser.Document.GetElementById("txtMeetChair");
                        htmlElement5.InnerHtml = ConferenceInformationEntityPC.ApplyPeople;

                        StringBuilder builder = new StringBuilder();
                        foreach (var item in ConferenceInformationEntityPC.JoinPeopleName)
                        {
                            builder.Append(item + "   ");
                        }
                        //获取所有参会人
                        System.Windows.Forms.HtmlElement htmlElement6 = this.webView.WebBrowser.Document.GetElementById("txtMeetPartical");
                        htmlElement6.InnerHtml = builder.ToString();

                        System.Windows.Forms.HtmlElement tdPanel = this.webView.WebBrowser.Document.GetElementById("tdPanel");

                        var p1 = tdPanel.InnerHtml;

                        StringBuilder builder2 = new StringBuilder();
                        builder2.Append(p1);
                        string pParent = @"<p class=MsoNormal align=center style='text-align:center'><b><span lang=EN-US
  style='font-size:15pt'>" + this.ConferenceTreeItem.ACA_Tittle + "</span></span></b></p>";
                        builder2.Append(pParent);

                        this.SetNextLineAboutMeetSummary(this.ConferenceTreeItem, "1", ref builder2);

                        tdPanel.InnerHtml = builder2.ToString();
                    }
                }), 1000);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 刷新会议纪要

        void btnReflesh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //隐藏会议纪要更改提示
                this.SummerUpdateVisibility = System.Windows.Visibility.Collapsed;
                //构建会议纪要
                this.BuildMeetSummary();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 生成word版会议纪要

        void btnCreateWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileManage.CreateWPFile(Constant.ConferenceName + "_会议纪要" + ".doc", this.webView.WebBrowser, "conferenceComment");
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

        #region 生成pdf版会议纪要

        void btnCreatePDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileManage.CreateWPFile(Constant.ConferenceName + "_会议纪要" + ".pdf", this.webView.WebBrowser, "conferenceComment");
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

        #region 将会议纪要上传到服务器

        /// <summary>
        /// 将会议纪要上传到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //显示正在上传
                this.UploadVisibility = System.Windows.Visibility.Visible;
                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    ms.Position = 0;
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                    {
                        System.Windows.Forms.HtmlElement element = this.webView.WebBrowser.Document.GetElementById("conferenceComment");
                        sw.Write(element.OuterHtml);
                    }
                }

                if (this.mettingF.Count > 0 && ms != null)
                {
                    //获取文件夹
                    SP.Folder folder = this.mettingF[0];
                    //上传文件
                    Constant.clientContextManage.UploadFileToFolder(folder, Constant.ConferenceName + "_会议纪要" + ".doc", ms.GetBuffer(), new Action<bool>((successed) =>
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.UploadVisibility = System.Windows.Visibility.Collapsed;
                            this.UploadFlgVisibility = System.Windows.Visibility.Visible;
                            TimerJob.StartRun(new Action(() =>
                                {
                                    this.UploadFlgVisibility = System.Windows.Visibility.Collapsed;
                                }), 1500);
                        }));
                    }));
                }
            }
            catch (Exception ex)
            {
                this.UploadVisibility = System.Windows.Visibility.Collapsed;
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 备注

        /// <summary>
        /// 备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //显示备注控制
                    ConferenceTreeItem.currentConferenceTreeItem.CommentCommandVisibility = System.Windows.Visibility.Visible;
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

        #region 链接

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    if (this.mettingF.Count > 0)
                    {
                        //获取文件夹
                        SP.Folder folder = this.mettingF[0];
                        //上传文件
                        Constant.clientContextManage.UploadFileToFolder(folder, new Action(() =>
                          {
                              if (ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility == System.Windows.Visibility.Collapsed)
                              {
                                  ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility = System.Windows.Visibility.Visible;
                              }
                          }), new Action<SP.File>((uploadFile) =>
                          {
                              this.Dispatcher.BeginInvoke(new Action(() =>
                              {

                                  //通知服务器更改链接
                                  ConferenceTreeItem.currentConferenceTreeItem.LinkListItemToService(Constant.SPSiteAddressFront + uploadFile.ServerRelativeUrl);
                                  ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility = System.Windows.Visibility.Collapsed;
                              }));
                          }));
                    }
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

        #region 删除节点

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //删除节点
                    ConferenceTreeItem.currentConferenceTreeItem.menuItem_Delete();
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

        #region 生成子层

        /// <summary>
        /// 生成子层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnChildlevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //添加子节点
                    ConferenceTreeItem.currentConferenceTreeItem.ItemAdd();
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

        #region 生成平行层

        /// <summary>
        /// 生成平行层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSamelevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //添加同一级节点
                    ConferenceTreeItem.currentConferenceTreeItem.ParentItemAdd();
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

        #region 打开文件

        /// <summary>
        /// 根据文件的类型使用相应的方式打开
        /// </summary>
        public void FileOpenByExtension(ConferenceWebCommon.EntityHelper.ConferenceOffice.FileType fileType, string uri)
        {
            try
            {
                //隐藏生成会议纪要
                this.ConferenceCommentCommandVisibility = System.Windows.Visibility.Collapsed;
                //显示下载控制
                this.DownloadCommandVisibility = System.Windows.Visibility.Visible;
                //保存当前打开的文件地址
                this.currentFileUri = uri;

                switch (fileType)
                {
                    case FileType.docx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.doc:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.xlsx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.Txt:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.PPtx:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.PPt:
                        //this.OpenOfficeFile(uri, fileType);
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.one:
                        this.OpenOfficeFile(uri);

                        break;
                    case FileType.Mp4:
                        this.OpenVideoAudioFile(uri);

                        break;
                    case FileType.avi:
                        this.OpenVideoAudioFile(uri);

                        break;
                    case FileType.mp3:
                        this.OpenVideoAudioFile(uri);

                        break;

                    case FileType.Jpg:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Png:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Ico:
                        this.OpenPictureFile(uri);
                        break;

                    case FileType.Xml:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.txt:
                        this.OpenFileByBrowser(uri);

                        break;
                    case FileType.record:
                        this.OpenRecordFile(uri);

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #region 文件类型

        /// <summary>
        /// 通过浏览器打开文件
        /// </summary>
        public void OpenFileByBrowser(string uri)
        {
            try
            {

                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();
                if (this.webView == null)
                {
                    this.webView = new WebView(uri, Constant.WebLoginUserName, Constant.WebLoginPassword);
                }
                else
                {
                    this.webView.OpenUri(uri);
                }
                //加载UI事件（比如多媒体播放器,浏览器）
                this.book_LoadUI(this.webView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 通过浏览器打开文件
        /// </summary>
        public void OpenTreeByBrowser(string uri)
        {
            try
            {

                //wordManage.ClearDocuments();
                //pPTManage.ClearDocuments();
                //excelManage.ClearDocuments();
                //if (this.webView == null)
                //{
                //    this.treeWebView = new TreeWebView(uri);
                //}
                //else
                //{
                //    this.treeWebView.OpenUri(uri);
                //}
                ////加载UI事件（比如多媒体播放器,浏览器）
                //this.book_LoadUI(this.treeWebView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开图片文件
        /// </summary>
        /// <param name="uri"></param>
        public void OpenPictureFile(string uri)
        {
            try
            {
                //获取文件名称（包含扩展名称）
                var fileName = System.IO.Path.GetFileName(uri);
                //本地地址
                var loaclF = Constant.LocalTempRoot + "\\" + fileName;
                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();

                //文件下载
                webClientManage.FileDown(uri, loaclF, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    try
                    {
                        if (IsSuccessed)
                        {
                            if (File.Exists(loaclF))
                            {
                                wordManage.ClearDocuments();
                                pPTManage.ClearDocuments();
                                excelManage.ClearDocuments();
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    try
                                    {
                                        if (this.pictureView == null)
                                        {
                                            this.pictureView = new PictureView(loaclF);
                                        }
                                        else
                                        {
                                            this.pictureView.OpenUri(loaclF);
                                        }
                                        //加载UI事件（比如多媒体播放器,浏览器）
                                        this.book_LoadUI(this.pictureView);
                                    }
                                    catch (Exception ex2)
                                    {
                                        LogManage.WriteLog(this.GetType(), ex2);
                                    };
                                }));
                            }
                        }
                    }
                    catch (Exception ex2)
                    {
                        LogManage.WriteLog(this.GetType(), ex2);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开office文件
        /// </summary>
        /// <param name="uri"></param>
        void OpenOfficeFile(string uri)
        {
            try
            {
                uri = uri + this.owaWebExtentionName;
                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();
                if (this.webView == null)
                {
                    this.webView = new WebView(uri, Constant.WebLoginUserName, Constant.WebLoginPassword);
                }
                else
                {
                    this.webView.OpenUri(uri);
                }
                //加载UI事件（比如多媒体播放器,浏览器）
                this.book_LoadUI(this.webView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        /// <summary>
        /// 打开视频文件
        /// </summary>
        public void OpenVideoAudioFile(string uri)
        {
            try
            {
                wordManage.ClearDocuments();
                pPTManage.ClearDocuments();
                excelManage.ClearDocuments();

                if (this.mediaPlayerView == null)
                {
                    this.mediaPlayerView = new MediaPlayerView(uri);
                }
                else
                {
                    this.mediaPlayerView.OpenVideo(uri);
                }
                //加载UI事件（比如多媒体播放器）
                this.book_LoadUI(this.mediaPlayerView);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 打开record文件
        /// </summary>
        /// <param name="uri"></param>
        public void OpenRecordFile(string uri)
        {
            try
            {
                //获取文件名称（包含扩展名称）
                var fileName = System.IO.Path.GetFileName(uri);
                //本地地址
                var loaclF = Constant.LocalTempRoot + "\\" + fileName;

                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();

                //文件下载
                webClientManage.FileDown(uri, loaclF, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    if (IsSuccessed)
                    {
                        try
                        {
                            if (File.Exists(loaclF))
                            {
                                //通过流去获取文件字符串
                                using (FileStream fs = new FileStream(loaclF, FileMode.Open, FileAccess.Read))
                                {
                                    StreamReader reader = new StreamReader(fs, Encoding.UTF8);
                                    string recordUri = reader.ReadToEnd();
                                    this.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        try
                                        {
                                            //打开视频文件
                                            this.OpenVideoAudioFile(recordUri);
                                        }
                                        catch (Exception ex2)
                                        {
                                            LogManage.WriteLog(this.GetType(), ex2);
                                        };
                                    }));
                                }
                            }
                        }
                        catch (Exception ex2)
                        {
                            LogManage.WriteLog(this.GetType(), ex2);
                        }
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 加载UI事件（比如多媒体播放器）

        /// <summary>
        /// 加载UI事件（比如多媒体播放器）
        /// </summary>
        /// <param name="element"></param>
        public void book_LoadUI(FrameworkElement element)
        {
            try
            {
                //隐藏本地officeUI
                this.host.Visibility = System.Windows.Visibility.Collapsed;
                //隐藏装饰UI
                this.borDecorate.Visibility = System.Windows.Visibility.Collapsed;

                //显示视频UI
                this.borContent.Visibility = System.Windows.Visibility.Visible;
                //加载视频元素
                this.borContent.Child = element;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #endregion

        #region 下载

        void btnDownLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(this.currentFileUri))
                {
                    //打开文件存储对话框
                    FileManage.OpenDialogThenDoing(this.currentFileUri, new Action<string>((fileName) =>
                        {
                            this.downLoadProgress.Value = 0;
                            this.DownLoadingVisibility = System.Windows.Visibility.Visible;
                            //创建一个下载管理实例
                            WebClientManage webClientManage = new WebClientManage();
                            webClientManage.FileDown(this.currentFileUri, fileName, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //设置进度
                                    this.downLoadProgress.Value = intProcess;
                                }));

                            }), new Action<Exception, bool>((ex, IsSuccessed) =>
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //设置进度
                                    this.downLoadProgress.Value = this.downLoadProgress.Maximum;
                                    this.DownLoadingVisibility = System.Windows.Visibility.Collapsed;
                                }));
                            }));
                        }));

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

        #region 元素替换法

        /// <summary>
        /// 判断是否可执行
        /// </summary>
        /// <param name="beforeConferenceTreeItem"></param>
        /// <param name="nowConferenceTreeItem"></param>
        /// <returns></returns>
        public bool CheckCanInsteadElement(ConferenceTreeItem beforeConferenceTreeItem, ConferenceTreeItem nowConferenceTreeItem)
        {
            bool result = true;
            try
            {
                if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                {

                    //查找拖动目标的子集（不允许父级拖到子级的动作）
                    List<ConferenceTreeItem> beforeItemList = WPFElementManage.FindVisualChildren<ConferenceTreeItem>(beforeConferenceTreeItem).ToList<ConferenceTreeItem>();
                    //不允许父级拖到子级的动作（返回）
                    if (beforeItemList != null && beforeItemList.Contains(nowConferenceTreeItem))
                    {
                        result = false;
                    }
                    //不为父子级关系
                    if (beforeConferenceTreeItem.ACA_Parent == null || beforeConferenceTreeItem.ACA_Parent.Equals(nowConferenceTreeItem))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            return result;
        }

        /// <summary>
        /// 元素替换法
        /// </summary>
        public void InsteadElement(ConferenceTreeItem beforeConferenceTreeItem, ConferenceTreeItem nowConferenceTreeItem)
        {
            try
            {
                if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                {
                    bool result = this.CheckCanInsteadElement(beforeConferenceTreeItem, nowConferenceTreeItem);
                    if (result)
                    {
                        //获取目标节点的父节点
                        ConferenceTreeItem beforeConferenceTreeItemParent = beforeConferenceTreeItem.ACA_Parent;
                        if (beforeConferenceTreeItemParent.ACA_ChildList.Contains(beforeConferenceTreeItem))
                        {
                            //移除该子节点   
                            beforeConferenceTreeItemParent.ACA_ChildList.Remove(beforeConferenceTreeItem);
                        }
                        if (beforeConferenceTreeItemParent.StackPanel.Children.Contains(beforeConferenceTreeItem))
                        {
                            //UI显示
                            beforeConferenceTreeItemParent.StackPanel.Children.Remove(beforeConferenceTreeItem);
                        }
                        if (!nowConferenceTreeItem.ACA_ChildList.Contains(beforeConferenceTreeItem))
                        {
                            //拖拽到选中子节点添加节点
                            nowConferenceTreeItem.ACA_ChildList.Add(beforeConferenceTreeItem);
                        }
                        if (!nowConferenceTreeItem.StackPanel.Children.Contains(beforeConferenceTreeItem))
                        {
                            //UI显示
                            nowConferenceTreeItem.StackPanel.Children.Add(beforeConferenceTreeItem);
                        }
                        //父级更替
                        beforeConferenceTreeItem.ACA_Parent = nowConferenceTreeItem;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 元素替换法（服务器同步使用）
        /// </summary>
        public void InsteadElement(int beforeConferenceTreeItemGuid, int nowConferenceTreeItemGuid)
        {
            try
            {
                ///获取相对应的元素
                ConferenceTreeItem beforeConferenceTreeItem = null;
                ConferenceTreeItem nowConferenceTreeItem = null;

                //通过遍历获取相对应的子节点（判断GUID）
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                    {
                        break;
                    }
                    else if (item.ACA_Guid.Equals(beforeConferenceTreeItemGuid))
                    {
                        beforeConferenceTreeItem = item;
                    }
                    else if (item.ACA_Guid.Equals(nowConferenceTreeItemGuid))
                    {
                        nowConferenceTreeItem = item;
                    }
                }
                this.InsteadElement(beforeConferenceTreeItem, nowConferenceTreeItem);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 子节点扩展隐藏公共方法

        /// <summary>
        /// 子节点扩展隐藏公共方法
        /// </summary>
        /// <param name="item"></param>
        public static void ExpanderMethod()
        {
            try
            {
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    if (item.ACA_Parent == null)
                    {


                    }
                    //完毕敲键盘
                    if (!item.txtTittle.IsKeyboardFocused)
                    {
                        //编辑完毕
                        item.IsTittleEditNow = false;
                    }

                    //完毕敲键盘
                    if (!item.txtComment.IsKeyboardFocused)
                    {
                        //编辑完毕
                        item.IsCommentEditNow = false;
                    }

                    if (item.ACA_ChildList.Count == 0)
                    {
                        item.ExpanderVisibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        item.ExpanderVisibility = System.Windows.Visibility.Visible;
                    }
                    if (item.ACA_ChildList.Count < 2)
                    {
                        item.VerticalLineVisibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        if (item.ExpanderVisibility == Visibility.Visible)
                        {
                            item.VerticalLineVisibility = System.Windows.Visibility.Visible;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeView), ex);
            }
        }

        #endregion

    }
}


#region old solution

////生成一个ftp辅助类（）
//FtpManage ftpHelper = new FtpManage();
////文件地址
//string fileUri = Constant.ConferenceFtpWebAddressFront + Constant.ServicePPTTempFile;
////上传文件
//ftpHelper.UploadFtp(file, fileName, fileUri, "/", Constant.FtpUserName, Constant.FtpPassword, delegate(long Length, double progress)
//{
//}, delegate(System.Exception error, bool result)
//{
//    if (result)
//    {
//        //使用异步委托
//        this.Dispatcher.BeginInvoke(new Action(() =>
//        {
//            try
//            {
//                string httpFileUri = fileUri.Replace(Constant.ConferenceFtpWebAddressFront, Constant.TreeServiceAddressFront);
//                //通知服务器更改链接
//                ConferenceTreeItem.currentConferenceTreeItem.LinkListItemToService(httpFileUri + fileName);
//                ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility = System.Windows.Visibility.Collapsed;
//            }
//            catch (Exception ex)
//            {
//                LogManage.WriteLog(this.GetType(), ex);
//            }
//            finally
//            {

//            }
//        }));

//    }
//});
#endregion