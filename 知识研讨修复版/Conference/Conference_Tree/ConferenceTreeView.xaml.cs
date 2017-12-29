using ConferenceCommon.TimerHelper;
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
using System.Threading;
using ConferenceCommon.SharePointHelper;
using ConferenceCommon.WPFHelper;
using System.Windows.Threading;
using ConferenceCommon.FileHelper;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using form = System.Windows.Forms;
using ConferenceCommon.WPFControl;
using wpfHelperFileType = ConferenceCommon.WPFControl.FileType;
using Conference_Tree.Common;
using vy = System.Windows.Visibility;

namespace Conference_Tree
{
    /// <summary>
    /// TreeView.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeView : UserControlBase
    {
        #region 事件回调

        /// <summary>
        /// 文件推送回调
        /// </summary>
        public Action<string, wpfHelperFileType> SendFileCallBack = null;

        /// <summary>
        /// 文件共享回调
        /// </summary>
        public Action<string, wpfHelperFileType> FileShareCallBack = null;

        #endregion

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



        ///// <summary>
        ///// 标题参照物
        ///// </summary>
        //string displayNumber = "1";

        ///// <summary>
        /////  编辑控制计时器2
        ///// </summary>
        //public DispatcherTimer CommentEditControlTimer = null;

        /// <summary>
        /// 虚拟拖拽知识树
        /// </summary>
        public ConferenceTreeItemVisual conferenceTreeItemVisual = new ConferenceTreeItemVisual() { Visibility = Visibility.Collapsed, HorizontalAlignment = HorizontalAlignment.Left, VerticalAlignment = VerticalAlignment.Top };

        /// <summary>
        /// 文件显示
        /// </summary>
        internal OfficeFile officeFile = null;

        /// <summary>
        /// 会议纪要显示
        /// </summary>
        internal MeetSummary meetSummary = null;

        /// <summary>
        /// 搜索视图
        /// </summary>
        internal SearchFileView searchFileView = null;

        #endregion

        #region 绑定属性

        //string meetingName = string.Empty;
        ///// <summary>
        ///// 研讨会议名称
        ///// </summary>
        //public string MeetingName
        //{
        //    get { return meetingName; }
        //    set
        //    {
        //        if (this.meetingName != value)
        //        {
        //            this.meetingName = value;
        //            this.OnPropertyChanged("MeetingName");
        //        }
        //    }
        //}


        //Visibility loadingVisibility = Visibility.Visible;
        ///// <summary>
        ///// 加载提示显示状态
        ///// </summary>
        //public Visibility LoadingVisibility
        //{
        //    get { return loadingVisibility; }
        //    set
        //    {
        //        if (loadingVisibility != value)
        //        {
        //            loadingVisibility = value;
        //            this.OnPropertyChanged("LoadingVisibility");
        //        }
        //    }
        //}

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

        #endregion

        #region 静态字段

        /// <summary>
        /// 标示静态事件是否注册，启动程序一次只允许注册一次（图表相关联相对较复杂）
        /// </summary>
        static bool StaticEventRegeditIsRegedit = false;

        /// <summary>
        /// 自我绑定
        /// </summary>
        public static Conference_Tree.ConferenceTreeView conferenceTreeView = null;

        /// <summary>
        ///  编辑控制计时器
        /// </summary>
        public static DispatcherTimer TittleEditControlTimer = null;

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
                //自我绑定
                conferenceTreeView = this;
                //注册事件
                this.EventRegedit();
                //注册静态事件（全局只进行一次）
                this.StaticEventRegedit();
                //参数初始化
                this.Parameters_Init();
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
                //保存（xml文件保存,研讨语音，树研讨）
                this.btnXMLSaved.Click += btnXMLSaved_Click;
                //打开图表视图
                this.btnChart.Click += btnChart_Click;


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

                //子项拖动完成事件
                this.MouseLeftButtonUp += ConferenceTreeView_PreviewMouseRightButtonUp;
                //子项相应鼠标移动
                this.borDiscussTheme.MouseMove += ConferenceTreeView_PreviewMouseMove;
                ////放大预览文件视图
                //this.btnBigger.Click += btnBigger_Click;
                ////缩小预览文件视图
                //this.btnSmaller.Click += btnSmaller_Click;
                ////设置预览文件视图默认大小
                //this.btnDefault.Click += btnDefault_Click;
                //更改视图比例
                this.cmbTreeDisplay.SelectionChanged += cmbTreeDisplay_SelectionChanged;
                //智存空间文件搜索显示
                this.btnSpSearch.Click += btnSpSearch_Click;
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

        #region 展開、收縮

        /// <summary>
        /// 展開、收縮
        /// </summary>     
        public void ViewChange()
        {
            try
            {
                int intColumn = Grid.GetColumn(this.borLeftMain);
                if (intColumn == 0)
                {
                    Grid.SetColumn(this.borLeftMain, 1);
                    if (this.officeFile != null)
                    {
                        this.officeFile.CollapsedVisibility = vy.Collapsed;
                        this.officeFile.ExpanderVisibility = vy.Visible;
                    }
                    if (this.meetSummary != null)
                    {
                        this.meetSummary.CollapsedVisibility = vy.Collapsed;
                        this.meetSummary.ExpanderVisibility = vy.Visible;
                    }
                    if (this.searchFileView != null)
                    {
                        this.searchFileView.CollapsedVisibility = vy.Collapsed;
                        this.searchFileView.ExpanderVisibility = vy.Visible;
                    }
                }
                else
                {
                    Grid.SetColumn(this.borLeftMain, 0);
                    if (this.officeFile != null)
                    {
                        this.officeFile.CollapsedVisibility = vy.Visible;
                        this.officeFile.ExpanderVisibility = vy.Collapsed;
                    }
                    if (this.meetSummary != null)
                    {
                        this.meetSummary.CollapsedVisibility = vy.Visible;
                        this.meetSummary.ExpanderVisibility = vy.Collapsed;
                    }
                    if (this.searchFileView != null)
                    {
                        this.searchFileView.CollapsedVisibility = vy.Visible;
                        this.searchFileView.ExpanderVisibility = vy.Collapsed;
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

        #region 智存空间文件搜索显示

        /// <summary>
        /// 智存空间文件搜索显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSpSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeCodeEnterEntity.Tree_LeftContentType = Tree_LeftContentType.SearchFile;
                if (this.searchFileView == null)
                {
                    this.searchFileView = new SearchFileView();
                }
                this.borLeftMain.Child = this.searchFileView;
                this.searchFileView.Show_Data_View();
                //選擇事件回調
                this.conferenceTreeItem.SelectedItemCallEvent();
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

        #region 参数初始化

        /// <summary>
        /// 参数初始化
        /// </summary>
        private void Parameters_Init()
        {
            try
            {
                //创建文件操作管理
                TreeCodeEnterEntity.fileOpenManage = new FileOpenManage(TreeCodeEnterEntity.WebLoginUserName, TreeCodeEnterEntity.WebLoginPassword, TreeCodeEnterEntity.LocalTempRoot, TreeCodeEnterEntity.LoginUserName, TreeCodeEnterEntity.UserDomain, true);
                //知识树加载
                this.borDiscussTheme.Child = this.ConferenceTreeItem;
                //添加虚拟知识树节点
                this.gridDiscussTheme.Children.Add(conferenceTreeItemVisual);
                //会议纪要显示
                this.MeetSummaryShow();

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    //获取会议根目录
                    List<SP.Folder> mettingList = TreeCodeEnterEntity.ClientContextManage.GetFolders(TreeCodeEnterEntity.MeetingFolderName);
                    //获取相关会议的所有文档
                    TreeCodeEnterEntity.mettingF = mettingList.Where(Item => Item.Name.Equals(TreeCodeEnterEntity.ConferenceName)).ToList<SP.Folder>();
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

        #region 会议纪要显示

        private void btnMeetSummary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.MeetSummaryShow();
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
        /// 会议纪要显示
        /// </summary>
        void MeetSummaryShow()
        {
            try
            {
                //会议纪要类型
                TreeCodeEnterEntity.Tree_LeftContentType = Common.Tree_LeftContentType.Summary;
                if (this.meetSummary == null)
                {
                    this.meetSummary = new MeetSummary();
                }
                this.borLeftMain.Child = this.meetSummary;
                //创建会议纪要
                this.meetSummary.MeetSummary_DisPlay();
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
                            ConferenceTreeView.conferenceTreeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Collapsed;

                            return;
                        }
                        else
                        {
                            //显示虚拟拖拽节点
                            ConferenceTreeView.conferenceTreeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Collapsed;
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
                            ModelManage.ConferenceTree.InsteadElement(TreeCodeEnterEntity.ConferenceName, currentItem.ACA_Guid, conferenceTreeItem.ACA_Guid, new Action<bool>((successed) =>
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
                    this.ConferenceChart_View.ChartTittlesAdd(TreeCodeEnterEntity.VoteChatTittleList);

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
    }
}