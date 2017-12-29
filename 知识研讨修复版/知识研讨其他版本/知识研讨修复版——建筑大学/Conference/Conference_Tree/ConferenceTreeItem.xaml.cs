using ConferenceCommon.TimerHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.KeyBoard;
using ConferenceCommon.LogHelper;
using ConferenceModel;
using ConferenceModel.ConferenceTreeWebService;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Controls = System.Windows.Controls;
using ConferenceCommon.WPFHelper;
using Conference_Tree;

namespace Conference_Tree
{
    /// <summary>
    /// AcademicReviewItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeItem : UserControlBase
    {
        #region 自定义事件

        //public delegate void VoteChangedEventHandle(string tittle, int YesVoteCount, int NoVoteCount, int guid);
        ///// <summary>
        ///// 投票更改事件
        ///// </summary>
        //public static event VoteChangedEventHandle VoteChangedEvent = null;

        ///// <summary>
        ///// 投票的相关节点添加事件
        ///// </summary>
        //public static event VoteChangedEventHandle VoteTreeItemAddEvent = null;

        //public delegate void VOteCHnagedEventHandle2(int guid);
        ///// <summary>
        ///// 投票的相关节点删除事件
        ///// </summary>
        //public static event VOteCHnagedEventHandle2 VoteTreeItemRemoveEvent = null;


        public delegate void RefleshCompleateEventHandle();
        /// <summary>
        /// 更新整棵研讨树完成事件
        /// </summary>
        public static event RefleshCompleateEventHandle RefleshCompleateEvent = null;

        #endregion

        #region 字段

        /// <summary>
        /// 当前选择的链接
        /// </summary>
        TextBlock selectedLink = null;

        /// <summary>
        /// 判断是否可以拖动
        /// </summary>
        public bool isDrag = false;

        /// <summary>
        /// 拖动起始位置
        /// </summary>
        public Point StartPoint;

        /// <summary>
        /// 操作显示计时器
        /// </summary>
        DispatcherTimer OperationVisibilityTimer = null;

        /// <summary>
        /// 是否可以显示编辑提示
        /// </summary>
        bool CanSettingEditCollpased = true;

        #endregion

        #region 绑定属性

        string aCA_Tittle ;
        /// <summary>
        /// 显示名称
        /// </summary>
        public string ACA_Tittle
        {
            get { return aCA_Tittle; }
            set
            {
                if (aCA_Tittle != value)
                {
                    aCA_Tittle = value;
                    this.OnPropertyChanged("ACA_Tittle");
                }
            }
        }

        int aCA_Guid = 0;
        /// <summary>
        /// 每个节点的唯一标示
        /// </summary>
        public int ACA_Guid
        {
            get { return aCA_Guid; }
            set
            {
                if (aCA_Guid != value)
                {
                    aCA_Guid = value;
                    this.OnPropertyChanged("ACA_Guid");
                }
            }
        }

        //string aCA_Introduction;
        ///// <summary>
        ///// 评论简介
        ///// </summary>
        //public string ACA_Introduction
        //{
        //    get { return aCA_Introduction; }
        //    set
        //    {
        //        if (aCA_Introduction != value)
        //        {
        //            aCA_Introduction = value;
        //        }
        //    }
        //}

        string aCA_Comment;
        /// <summary>
        /// 评论
        /// </summary>
        public string ACA_Comment
        {
            get { return aCA_Comment; }
            set
            {
                if (aCA_Comment != value)
                {
                    aCA_Comment = value;
                    this.OnPropertyChanged("ACA_Comment");
                }
            }
        }

        //int aCA_YesVoteCount;
        ///// <summary>
        ///// 投票赞成积分
        ///// </summary>
        //public int ACA_YesVoteCount
        //{
        //    get { return aCA_YesVoteCount; }
        //    set
        //    {
        //        if (aCA_YesVoteCount != value)
        //        {
        //            aCA_YesVoteCount = value;
        //            this.OnPropertyChanged("ACA_YesVoteCount");
        //        }
        //    }
        //}

        //int aCA_NoVoteCount;
        ///// <summary>
        ///// 投票反对积分
        ///// </summary>
        //public int ACA_NoVoteCount
        //{
        //    get { return aCA_NoVoteCount; }
        //    set
        //    {
        //        if (aCA_NoVoteCount != value)
        //        {
        //            aCA_NoVoteCount = value;
        //            this.OnPropertyChanged("ACA_NoVoteCount");
        //        }
        //    }
        //}

        Visibility selectedVisibility = Visibility.Collapsed;
        /// <summary>
        /// 选择的节点显示
        /// </summary>
        public Visibility SelectedVisibility
        {
            get { return selectedVisibility; }
            set
            {
                if (selectedVisibility != value)
                {
                    selectedVisibility = value;
                    this.OnPropertyChanged("SelectedVisibility");
                }
            }
        }

        Visibility borVisibility = Visibility.Collapsed;
        /// <summary>
        /// 筛选出的节点
        /// </summary>
        public Visibility BorVisibility
        {
            get { return borVisibility; }
            set
            {
                if (borVisibility != value)
                {
                    borVisibility = value;
                    this.OnPropertyChanged("BorVisibility");
                }
            }
        }

        private bool isReadOnly = false;
        /// <summary>
        /// 是否为只读权限
        /// </summary>
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set
            {
                if (isReadOnly != value)
                {
                    isReadOnly = value;
                    this.OnPropertyChanged("IsReadOnly");
                }
            }
        }

        //double yesVoteLength;
        ///// <summary>
        ///// 赞成票的显示的长度（只在客户端使用，服务器端协议实体无对应）
        ///// </summary>
        //public double YesVoteLength
        //{
        //    get { return yesVoteLength; }
        //    set
        //    {
        //        if (this.yesVoteLength != value)
        //        {
        //            yesVoteLength = value;
        //            this.OnPropertyChanged("YesVoteLength");
        //        }
        //    }
        //}

        //double noVoteLength;
        ///// <summary>
        ///// 反对票的显示的长度（只在客户端使用，服务器端协议实体无对应）
        ///// </summary>
        //public double NoVoteLength
        //{
        //    get { return noVoteLength; }
        //    set
        //    {
        //        if (this.noVoteLength != value)
        //        {
        //            noVoteLength = value;
        //            this.OnPropertyChanged("NoVoteLength");
        //        }
        //    }
        //}

        //double voteAllLength = 80;
        ///// <summary>
        ///// 投票对比控件的总长度
        ///// </summary>
        //public double VoteAllLength
        //{
        //    get { return voteAllLength; }
        //    set
        //    {
        //        if (this.voteAllLength != value)
        //        {
        //            voteAllLength = value;
        //            this.OnPropertyChanged("VoteAllLength");
        //        }
        //    }
        //}

        //Visibility voteTipVisibility = Visibility.Collapsed;
        ///// <summary>
        ///// 投票提示
        ///// </summary>
        //public Visibility VoteTipVisibility
        //{
        //    get { return voteTipVisibility; }
        //    set
        //    {
        //        if (this.voteTipVisibility != value)
        //        {
        //            voteTipVisibility = value;
        //            this.OnPropertyChanged("VoteTipVisibility");
        //        }
        //    }
        //}

        Visibility commentVisibility = Visibility.Collapsed;
        /// <summary>
        /// 评论提示
        /// </summary>
        public Visibility CommentVisibility
        {
            get { return commentVisibility; }
            set
            {
                if (this.commentVisibility != value)
                {
                    commentVisibility = value;
                    this.OnPropertyChanged("CommentVisibility");
                }
            }
        }

        Visibility commentCommandVisibility = Visibility.Collapsed;
        /// <summary>
        /// 备注控制显示
        /// </summary>
        public Visibility CommentCommandVisibility
        {
            get { return commentCommandVisibility; }
            set
            {
                //if (this.commentCommandVisibility != value)
                //{
                commentCommandVisibility = value;
                //this.OnPropertyChanged("CommentCommandVisibility");
                this.btnComment.Visibility = value;
                //}
            }
        }

        Visibility linkCommandVisibility = Visibility.Collapsed;
        /// <summary>
        /// 超链接命令显示
        /// </summary>
        public Visibility LinkCommandVisibility
        {
            get { return linkCommandVisibility; }
            set
            {
                if (this.linkCommandVisibility != value)
                {
                    linkCommandVisibility = value;
                    this.OnPropertyChanged("LinkCommandVisibility");
                }
            }
        }

        Visibility linkListVisibility = Visibility.Collapsed;
        /// <summary>
        /// 超链接列表显示
        /// </summary>
        public Visibility LinkListVisibility
        {
            get { return linkListVisibility; }
            set
            {
                if (this.linkListVisibility != value)
                {
                    linkListVisibility = value;
                    this.OnPropertyChanged("LinkListVisibility");
                }
            }
        }

        Visibility uploadFileTipVisibility = Visibility.Collapsed;
        /// <summary>
        /// 超链接列表显示
        /// </summary>
        public Visibility UploadFileTipVisibility
        {
            get { return uploadFileTipVisibility; }
            set
            {
                if (this.uploadFileTipVisibility != value)
                {
                    uploadFileTipVisibility = value;
                    this.OnPropertyChanged("UploadFileTipVisibility");
                }
            }
        }

        string titleOperationer;
        /// <summary>
        /// 标题操作人
        /// </summary>
        public string TitleOperationer
        {
            get { return titleOperationer; }
            set
            {
                if (this.titleOperationer != value)
                {
                    titleOperationer = value;
                    this.OnPropertyChanged("TitleOperationer");
                }
            }
        }

        //bool isOperating = false;
        ///// <summary>
        ///// 是否正在编辑
        ///// </summary>
        //public bool IsOperating
        //{
        //    get { return isOperating; }
        //    set { isOperating = value; }
        //}

        Visibility operationVisibility = Visibility.Collapsed;
        /// <summary>
        /// 操作提示
        /// </summary>
        public Visibility OperationVisibility
        {
            get { return operationVisibility; }
            set
            {
                if (this.operationVisibility != value)
                {
                    operationVisibility = value;
                    this.OnPropertyChanged("OperationVisibility");
                }
            }
        }

        Visibility contentVisibility = Visibility.Visible;
        /// <summary>
        /// 扩展内容显示
        /// </summary>
        public Visibility ContentVisibility
        {
            get { return contentVisibility; }
            set
            {
                if (this.contentVisibility != value)
                {
                    contentVisibility = value;
                    this.OnPropertyChanged("ContentVisibility");
                }
            }
        }

        Visibility verticalLineVisibility = Visibility.Visible;
        /// <summary>
        /// 右侧垂直线条显示
        /// </summary>
        public Visibility VerticalLineVisibility
        {
            get { return verticalLineVisibility; }
            set
            {
                if (this.verticalLineVisibility != value)
                {
                    verticalLineVisibility = value;
                    this.OnPropertyChanged("VerticalLineVisibility");
                }
            }
        }

        Visibility expanderVisibility;
        /// <summary>
        /// 扩展、收缩按钮显示
        /// </summary>
        public Visibility ExpanderVisibility
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

        bool isLocked = true;
        /// <summary>
        /// 是否被锁定
        /// </summary>
        public bool IsLocked
        {
            get { return isLocked; }
            set
            {
                if (this.isLocked != value)
                {
                    isLocked = value;
                    this.OnPropertyChanged("IsLocked");
                }
            }
        }

        #endregion
       
        #region 一般属性

        ConferenceTreeItem aCA_Parent = null;
        /// <summary>
        /// 父节点
        /// </summary>
        public ConferenceTreeItem ACA_Parent
        {
            get { return aCA_Parent; }
            set { aCA_Parent = value; }
        }

        List<ConferenceTreeItem> aCA_ChildList = new List<ConferenceTreeItem>();
        /// <summary>
        /// 知识树节点集合
        /// </summary>
        public List<ConferenceTreeItem> ACA_ChildList
        {
            get { return aCA_ChildList; }
            set { aCA_ChildList = value; }
        }

        int parentGuid;
        /// <summary>
        /// 父类GUID标示
        /// </summary>
        public int ParentGuid
        {
            get { return this.parentGuid; }
            set { this.parentGuid = value; }
        }

        bool isTittleEditNow = false;
        /// <summary>
        /// 是否正在编辑标题
        /// </summary>
        public bool IsTittleEditNow
        {
            get { return isTittleEditNow; }
            set
            {

                if (isTittleEditNow != value)
                {
                    isTittleEditNow = value;
                }
            }
        }

        bool isCommentEditNow = false;
        /// <summary>
        /// 是否正在编辑评论
        /// </summary>
        public bool IsCommentEditNow
        {
            get { return isCommentEditNow; }
            set
            {
                if (isCommentEditNow != value)
                {
                    isCommentEditNow = value;
                }
            }
        }

        #endregion

        #region 静态字段

        /// <summary>
        /// 根记录
        /// </summary>
        static int RootCount = 0;

        /// <summary>
        /// 记录所有知识树的记录
        /// </summary>
        public static List<ConferenceTreeItem> AcademicReviewItemList = new List<ConferenceTreeItem>();

        /// <summary>
        /// 语音讨论点对点映射集合
        /// </summary>
        public static List<int> retunList = new List<int>();

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object obj1 = new object();

        /// <summary>
        /// 当前树节点
        /// </summary>
        public static ConferenceTreeItem currentConferenceTreeItem = null;

        /// <summary>
        /// 是否可以进行刷新
        /// </summary>
        static bool goIntoReflesh = true;

        //static TreeCodeEnterEntity TreeCodeEnterEntity;

        #endregion

        #region 协议属性（用于传输和xml序列化使用）

        ConferenceTreeItemTransferEntity academicReviewItemTransferEntity;
        /// <summary>
        /// 协议实体
        /// </summary>
        internal ConferenceTreeItemTransferEntity AcademicReviewItemTransferEntity
        {
            get { return academicReviewItemTransferEntity; }
            set { academicReviewItemTransferEntity = value; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="NeedInit"></param>
        public ConferenceTreeItem(bool NeedInit)
        {
            try
            {
                InitializeComponent();

                //设置当前上下文
                this.DataContext = this;

                //是否需要通过这些初始化
                if (NeedInit)
                {
                    

                    //初始化()
                    this.TreeInit();
                    //隐藏弹出论点按钮
                    this.btnComment.Visibility = System.Windows.Visibility.Collapsed;

                    #region 事件注册

                    //通过主窗体点击事件去捕获点击到的元素
                    TreeCodeEnterEntity.TreeCodeEnterEntityInstance.MainWindow.PreviewMouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

                    //结束评论窗体
                    //TreeCodeEnterEntity.MainWindow.Closed += MainWindow_Closed;
                    //更改评论
                    //ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.TextChangedEvent += messagWindow_TextChangedEvent;

                    #endregion
                }

                #region 注册事件区域

                //更改标题
                this.txtTittle.TextChanged += txtTittle_TextChanged;
                //更改评论
                this.txtComment.TextChanged += txtComment_TextChanged;

                //标题获取焦点事件
                this.txtTittle.GotFocus += txtTittle_GotFocus;
                //评论获取焦点事件
                this.txtComment.GotFocus += txtComment_GotFocus;
                //评论弹出事件
                this.btnComment.Click += btnComment_Click;
                //超链接列表弹出事件
                this.btnLinkList.Click += btnLinkList_Click;
                //尺寸更改事件
                this.borMain.SizeChanged += borMain_SizeChanged;

                this.btnExpander.Click += btnExpander_Click;

                //节点选择事件
                this.borTreeMain.PreviewMouseLeftButtonDown += ConferenceTreeItem_PreviewMouseLeftButtonDown;
                //拖拽事件
                this.borSelected.PreviewMouseLeftButtonDown += borSelected_PreviewMouseLeftButtonDown;
                //会议主持人有添加和删除节点的权限
                if (TreeCodeEnterEntity.TreeCodeEnterEntityInstance.IsMeetPresenter)
                {
                    //添加节点
                    this.imgAdd.MouseLeftButtonDown += imgAdd_MouseLeftButtonDown;
                    //删除节点
                    this.menuItemDelete.Click += menuItemDelete_Click;
                    //清除当前节点的所有投票
                    //this.menuItemAllVoteClear.Click += menuItemAllVoteClear_Click;
                }
                
                #endregion

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 展开、隐藏事件

        void btnExpander_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ContentVisibility == System.Windows.Visibility.Visible)
                {
                    this.ContentVisibility = System.Windows.Visibility.Collapsed;
                    this.VerticalLineVisibility = System.Windows.Visibility.Collapsed;
                    this.btnExpander.Background = Application.Current.Resources["brush_unfold"] as ImageBrush;
                }
                else
                {
                    this.ContentVisibility = System.Windows.Visibility.Visible;
                    if (this.ACA_ChildList.Count > 1)
                    {
                        this.VerticalLineVisibility = System.Windows.Visibility.Visible;
                    }
                    this.btnExpander.Background = Application.Current.Resources["brush_fold"] as ImageBrush;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion


        #region 节点选择事件(包含拖拽事件)

        /// <summary>
        /// 节点选择事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ConferenceTreeItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //释放选择
                if (currentConferenceTreeItem != null)
                {
                    currentConferenceTreeItem.SelectedVisibility = System.Windows.Visibility.Collapsed;
                }
                //绑定节点
                currentConferenceTreeItem = this;
                //显示选择
                currentConferenceTreeItem.SelectedVisibility = System.Windows.Visibility.Visible;

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
        /// 拖拽选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void borSelected_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                #region 节点拖拽设置

                Border borDicuss = ConferenceTreeView.ConferenceTreeView.borDiscussTheme;

                ConferenceTreeItemVisual visulItem = MainWindow.MainPageInstance.ConferenceTreeView.conferenceTreeItemVisual;

                //获取选择的树节点的相对容器位置
                Point p = this.TranslatePoint(new Point(0, 0), borDicuss);

                //设置虚拟拖拽节点的坐标
                visulItem.Margin = new Thickness(p.X, p.Y, 0, 0);
                visulItem.Height = this.ActualHeight;
                //显示拖拽节点的标题
                visulItem.ACA_Tittle = this.ACA_Tittle;

                isDrag = true;

                visulItem.Visibility = System.Windows.Visibility.Visible;

                ////记录起始位置
                StartPoint = e.GetPosition(borDicuss);

                #endregion
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
            if (ConferenceTreeItem.currentConferenceTreeItem != null)
            {
                //鼠标点击释放了，不可再拖动
                ConferenceTreeItem.currentConferenceTreeItem.isDrag = false;
            }

            //显示虚拟拖拽节点
            MainWindow.MainPageInstance.ConferenceTreeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        #region 高度同步

        /// <summary>
        /// 高度同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void borMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                //var height = this.ActualHeight;
                //if(e.HeightChanged&&this.ACA_Parent!= null)
                //{
                //    foreach (var item in this.ACA_Parent.ACA_ChildList)
                //    {
                //        item.Height = height;
                //    }
                //}
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

        #region 评论弹出事件

        /// <summary>
        /// 评论弹出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LinkListVisibility = System.Windows.Visibility.Collapsed;
                //论点面板显示设置
                if (this.CommentVisibility == System.Windows.Visibility.Visible)
                {
                    this.CommentVisibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.CommentVisibility = System.Windows.Visibility.Visible;
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

        #region 超链接列表弹出事件

        void btnLinkList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CommentVisibility = System.Windows.Visibility.Collapsed;
                //论点面板显示设置
                if (this.LinkListVisibility == System.Windows.Visibility.Visible)
                {
                    this.LinkListVisibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    this.LinkListVisibility = System.Windows.Visibility.Visible;
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

        #region 焦点获取事件

        /// <summary>
        /// 标题默认转为null
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtTittle_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //判断标题不为null
                if (!string.IsNullOrEmpty(this.ACA_Tittle))
                {
                    if (this.ACA_Tittle.Equals(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.TreeItemEmptyName))
                    {
                        this.ACA_Tittle = string.Empty;
                    }
                }
                //设置占用者
                this.AcademicReviewItemTransferEntity.FocusAuthor = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.LoginUserName;
                //占用标题
                this.AcademicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.FocusType1;
                //强行占用焦点
                ModelManage.ConferenceTree.ForceOccupyFocus(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
                {

                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        /// <summary>
        /// 焦点获取事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtComment_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //设置占用者
                this.AcademicReviewItemTransferEntity.FocusAuthor = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.LoginUserName;
                //占用评论
                this.AcademicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.FocusType2;
                //强行占用焦点
                ModelManage.ConferenceTree.ForceOccupyFocus(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
                {

                }));
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

        #region 键盘弹出事件

        /// <summary>
        /// 键盘弹出事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KeyBoard_UP(object sender, TouchEventArgs e)
        {
            try
            {
                TimerJob.StartRun(new Action(() =>
                    {
                        //显示触摸键盘
                        TouchKeyBoard.ShowInputPanel();
                    }), 100);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 初始化(权限、)
        /// </summary>
        public void TreeInit()
        {
            try
            {
                if (goIntoReflesh)
                {
                    goIntoReflesh = false;
                    //同步协议对象（根节点）
                    this.AcademicReviewItemTransferEntity = new ConferenceTreeItemTransferEntity() { };
                    //根目录为0，说明刚启动
                    if (ConferenceTreeItem.RootCount == 0)
                    {
                        retunList.Add(0);
                        //获取服务器里缓存的知识树并刷新本地知识树（到达初始化同步）
                        ModelManage.ConferenceTree.GetAll(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, new Action<ConferenceTreeInitRefleshEntity>((result) =>
                            {
                                //刷新知识树
                                this.Reflesh(result);

                                //更新整棵研讨树完成事件
                                if (ConferenceTreeItem.RefleshCompleateEvent != null)
                                {
                                    ConferenceTreeItem.RefleshCompleateEvent();
                                }
                                goIntoReflesh = true;
                            }));
                        ConferenceTreeItem.AcademicReviewItemList.Add(this);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 刷新知识树
        /// </summary>
        /// <param name="result"></param>
        void Reflesh(ConferenceTreeInitRefleshEntity result)
        {
            try
            {
                //根记录同步
                ConferenceTreeItem.RootCount = result.RootCount;

                //rootParent(只有根节点是没有的，所有可以判断该节点为根节点)
                if (result != null && result.RootParent_AcademicReviewItemTransferEntity != null)
                {
                    //绑定根节点的协议实体
                    this.AcademicReviewItemTransferEntity = result.RootParent_AcademicReviewItemTransferEntity;
                    //设置根节点的标题
                    this.ACA_Tittle = result.RootParent_AcademicReviewItemTransferEntity.Title;
                    //设置根节点的评论
                    this.ACA_Comment = result.RootParent_AcademicReviewItemTransferEntity.Comment;
                }

                //根节点的衍生子节点
                if (result.AcademicReviewItemTransferEntity_ObserList != null && result.AcademicReviewItemTransferEntity_ObserList.Count() > 0)
                {

                    //进行后续的关联              
                    for (int i = 0; i < ConferenceTreeItem.AcademicReviewItemList.Count; i++)
                    {
                        //记录所有知识树的记录
                        ConferenceTreeItem entityItem = ConferenceTreeItem.AcademicReviewItemList[i];
                        foreach (var transferItem in result.AcademicReviewItemTransferEntity_ObserList)
                        {
                            //映射集合GUID
                            if (!ConferenceTreeItem.retunList.Contains(transferItem.Guid))
                            {
                                //添加GUID
                                ConferenceTreeItem.retunList.Add(transferItem.Guid);
                            }
                            //父节点加载子节点
                            if (transferItem.ParentGuid.Equals(entityItem.ACA_Guid))
                            {
                                //生成子节点
                                ConferenceTreeItem academicReviewItem = new ConferenceTreeItem(false,null) { ACA_Parent = entityItem, ParentGuid = entityItem.ACA_Guid, ACA_Guid = transferItem.Guid };

                                //节点标题
                                academicReviewItem.ACA_Tittle = transferItem.Title;

                                if (!string.IsNullOrEmpty(transferItem.Comment))
                                {
                                    academicReviewItem.CommentCommandVisibility = System.Windows.Visibility.Visible;
                                }
                                if (transferItem.LinkList != null && transferItem.LinkList.Count > 0)
                                {
                                    academicReviewItem.LinkCommandVisibility = System.Windows.Visibility.Visible;
                                }

                                foreach (var link in transferItem.LinkList)
                                {
                                    string fileName = System.IO.Path.GetFileName(link);
                                    academicReviewItem.LinkListItemAdd(fileName, link);
                                }

                                //评论
                                academicReviewItem.ACA_Comment = transferItem.Comment;

                                //简介                              
                                entityItem.StackPanel.Children.Add(academicReviewItem);
                                //父节点子节点集合添加该子项
                                entityItem.ACA_ChildList.Add(academicReviewItem);
                                //子节点协议实体绑定
                                academicReviewItem.AcademicReviewItemTransferEntity = transferItem;
                                //记录所有知识树的记录添加子节点
                                ConferenceTreeItem.AcademicReviewItemList.Add(academicReviewItem);
                            }
                        }

                    }
                }
                ConferenceTreeView.ExpanderMethod();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 知识树子节点点对点进行同步
        /// </summary>
        /// <param name="result"></param>
        public static void Information_Sync(ConferenceTreeItemTransferEntity result)
        {
            try
            {
                //树视图启动
                Conference.MainWindow.MainPageInstance.ConferenceTreeView.IsStart = true;
                switch (result.Operation)
                {
                    //添加节点
                    case ConferenceTreeOperationType.AddType:
                        Add_Item(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;
                    //删除节点
                    case ConferenceTreeOperationType.DeleteType:
                        Delete_Item(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;
                    //更新节点
                    case ConferenceTreeOperationType.UpdateType:
                        UpdateItem(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;
                    //刷新
                    case ConferenceTreeOperationType.RefleshAllType:
                        ////重新加载一个新实例
                        //MainPage.MainPageInstance.ConferenceTreeView.ConferenceTreeItem2 = new ConferenceTreeItem2(true);
                        ////跳转到树
                        //MainPage.MainPageInstance.NavicateView(ViewSelectedItemEnum.Tree);
                        break;

                    case ConferenceTreeOperationType.FocusType1:
                        //剔除标题焦点
                        RemoveTittleFocus(result);
                        break;

                    case ConferenceTreeOperationType.FocusType2:
                        //剔除评论焦点
                        RemoveCommentFocus(result);
                        break;

                    case ConferenceTreeOperationType.LinkAdd:
                        UpdateItemLink(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;

                    case ConferenceTreeOperationType.UpdateComment:
                        UpdateItemComment(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;

                    case ConferenceTreeOperationType.UpdateTittle:
                        UpdateItemTittle(result);
                        MainWindow.MainPageInstance.ConferenceTreeView.SummerUpdateVisibility = Visibility.Visible;
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 添加节点
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void imgAdd_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ItemAdd();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 当前用户手动添加节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ItemAdd()
        {
            try
            {
                //生成一个知识树协议实体对象
                ConferenceTreeItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceTreeItemTransferEntity() { ParentGuid = this.ACA_Guid };
                //向服务器添加一个子节点         
                ModelManage.ConferenceTree.Add(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool>(result =>
                {

                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        public void ParentItemAdd()
        {
            try
            {
                if (this.ACA_Parent != null)
                {
                    //生成一个知识树协议实体对象
                    ConferenceTreeItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceTreeItemTransferEntity() { ParentGuid = this.ACA_Parent.ACA_Guid };
                    //向服务器添加一个子节点         
                    ModelManage.ConferenceTree.Add(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool>(result =>
                    {

                    }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 服务器同步添加节点
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void Add_Item(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                lock (obj1)
                {
                    //映射集合GUID
                    if (!ConferenceTreeItem.retunList.Contains(academicReviewItemTransferEntity.Guid))
                    {
                        //添加GUID
                        ConferenceTreeItem.retunList.Add(academicReviewItemTransferEntity.Guid);
                        //声明一个新的知识树节点
                        ConferenceTreeItem academicReviewItem = null;
                        //指定父节点，通过关联记录所有知识树的记录中获取（根据服务器传过来的知识树协议实体的父节点的GUID来判断）
                        foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                        {
                            if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.ParentGuid))
                            {
                                //生成知识树节点（关联）（该节点的GUID为服务器传过来的知识树协议实体的GUID）
                                academicReviewItem = new ConferenceTreeItem(false,null) { VerticalLineVisibility = System.Windows.Visibility.Collapsed, ExpanderVisibility = System.Windows.Visibility.Collapsed, ACA_Parent = item, ParentGuid = academicReviewItemTransferEntity.ParentGuid, ACA_Guid = academicReviewItemTransferEntity.Guid };
                                //父节点容器将新生成的知识节点添加
                                item.StackPanel.Children.Add(academicReviewItem);
                                //父节点的子项集合添加该节点
                                item.ACA_ChildList.Add(academicReviewItem);
                                //该节点的协议实体绑定服务器传过来的知识树协议实体
                                academicReviewItem.AcademicReviewItemTransferEntity = academicReviewItemTransferEntity;
                                //根目录自增1
                                ConferenceTreeItem.RootCount++;
                                break;
                            }
                        }
                        if (academicReviewItem != null)
                        {
                            //记录所有知识树的记录添加节点
                            ConferenceTreeItem.AcademicReviewItemList.Add(academicReviewItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 删除节点

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.menuItem_Delete();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 当前用户手动删除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void menuItem_Delete()
        {
            try
            {
                if (currentConferenceTreeItem.Parent != null)
                {
                    this.AcademicReviewItemTransferEntity.Operationer = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfUri;
                    //向服务器提交一个需要删除的知识树节点              
                    ModelManage.ConferenceTree.Delete(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #region 使用递归方式删除子节点

        /// <summary>
        /// 使用递归方式删除子节点
        /// </summary>
        /// <param name="parentGuid"></param>
        public static void ItemDeleteRecursion(int parentGuid)
        {
            #region 相对应的子节点也得删除

            //遍历记录所有知识树的记录
            for (int i = ConferenceTreeItem.AcademicReviewItemList.Count - 1; i > -1; i--)
            {
                var item = ConferenceTreeItem.AcademicReviewItemList[i];
                //若该节点的GUID为当前遍历所指定的子节点的父节点GUID,则证明了该节点为遍历所之指定的节点的父节点，父几点删除，子节点一样得进行删除
                if (item.ParentGuid.Equals(parentGuid))
                {
                    //清空附加数据
                    ConferenceTreeItem.ClearAddData(item);

                    //使用递归的方式以同样是方式操作子节点
                    if (item != null)
                    {
                        ItemDeleteRecursion(item.ACA_Guid);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            #endregion
        }

        #endregion

        /// <summary>
        /// 服务器同步删除节点
        /// </summary>
        public static void Delete_Item(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //声明一个知识树节点
                ConferenceTreeItem academicReviewChild = null;
                //指定父节点，通过关联记录所有知识树的记录中获取（根据服务器传过来的知识树协议实体的父节点的GUID来判断）
                if (academicReviewItemTransferEntity.Guid != 0)
                {
                    for (int i = 0; i < ConferenceTreeItem.AcademicReviewItemList.Count; i++)
                    {
                        //记录所有知识树的记录里获取同协议实体相同GUID的子节点
                        academicReviewChild = ConferenceTreeItem.AcademicReviewItemList[i];
                        //对应协议实体的GUID
                        if (academicReviewChild.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                        {
                            //父节点容器删除该子节点
                            academicReviewChild.ACA_Parent.StackPanel.Children.Remove(academicReviewChild);
                            //父节点的子项集合删除该节点
                            academicReviewChild.ACA_Parent.ACA_ChildList.Remove(academicReviewChild);

                            //清空附加数据
                            ConferenceTreeItem.ClearAddData(academicReviewChild);

                            #region 相对应的子节点也得删除

                            //使用递归方式删除子节点
                            ConferenceTreeItem.ItemDeleteRecursion(academicReviewItemTransferEntity.Guid);

                            #endregion

                            break;
                        }
                    }
                }
                //该为根节点
                else
                {
                    for (int i = 0; i < ConferenceTreeItem.AcademicReviewItemList.Count; i++)
                    {
                        //记录所有知识树的记录里获取同协议实体相同GUID的子节点
                        academicReviewChild = ConferenceTreeItem.AcademicReviewItemList[i];
                        //对应协议实体的GUID
                        if (academicReviewChild.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                        {
                            //父节点容器删除所有子节点
                            academicReviewChild.StackPanel.Children.Clear();
                            //父节点的子项集合删除所有节点
                            academicReviewChild.ACA_ChildList.Clear();

                            //通过遍历去清空所有子节点
                            for (int j = ConferenceTreeItem.AcademicReviewItemList.Count - 1; j > -1; j--)
                            {
                                ConferenceTreeItem item = ConferenceTreeItem.AcademicReviewItemList[j];
                                //根节点没有父类
                                if (item.ACA_Parent != null)
                                {
                                    //记录所有知识树的记录删除该节点
                                    ConferenceTreeItem.AcademicReviewItemList.RemoveAt(j);
                                    //清空附加数据
                                    ConferenceTreeItem.ClearAddData(item);
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 清空附加数据
        /// </summary>
        /// <param name="academicReviewChild"></param>
        public static void ClearAddData(ConferenceTreeItem academicReviewChild)
        {
            try
            {
                #region 清空附加数据

                //树节点集合
                List<ConferenceTreeItem> treeItemList = ConferenceTreeItem.AcademicReviewItemList;
                //若包含则清除
                if (treeItemList.Contains(academicReviewChild))
                {
                    treeItemList.Remove(academicReviewChild);
                }
                List<int> intlist = ConferenceTreeItem.retunList;
                if (intlist.Contains(academicReviewChild.ACA_Guid))
                {
                    intlist.Remove(academicReviewChild.ACA_Guid);
                }

                //若为当前选择的子节点则清空
                if (currentConferenceTreeItem != null && currentConferenceTreeItem.Equals(academicReviewChild))
                {
                    currentConferenceTreeItem = null;
                }

                ////清空计时器
                //if (academicReviewChild.CommentEditControlTimer != null)
                //{
                //    academicReviewChild.CommentEditControlTimer.Stop();
                //}
                //if (academicReviewChild.TittleEditControlTimer != null)
                //{
                //    academicReviewChild.TittleEditControlTimer.Stop();
                //}

                #endregion
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion
      
        #region 更改文本

        /// <summary>
        /// 更改标题
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtTittle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                TextBox txtBox = sender as TextBox;
                if (txtBox != null && txtBox.IsKeyboardFocused)
                {
                    //正在编辑
                    this.IsTittleEditNow = true;
                    //获取当前更改的子节点标题
                    this.ACA_Tittle = this.txtTittle.Text;
                    //设置当前节点的协议实体的标题            
                    this.AcademicReviewItemTransferEntity.Title = this.ACA_Tittle;
                    //设置当前协议实体的操作人
                    this.AcademicReviewItemTransferEntity.Operationer = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfName;
                    //通知服务器节点更新
                    ModelManage.ConferenceTree.UpdateTitle(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<string>((result) =>
                        {
                            if (string.IsNullOrEmpty(result))
                            {

                            }
                        }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 更改评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtComment_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if ((sender as TextBox).IsKeyboardFocused)
                {
                    //正在编辑
                    this.IsCommentEditNow = true;
                    //获取当前更改的子节点标题
                    this.ACA_Comment = this.txtComment.Text;
                    //设置当前节点的协议实体的标题            
                    this.AcademicReviewItemTransferEntity.Comment = this.txtComment.Text;
                    //设置当前协议实体的操作人
                    this.AcademicReviewItemTransferEntity.Operationer = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfName;
                    //通知服务器节点更新
                    ModelManage.ConferenceTree.UpdateComment(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<string>((result) =>
                    {
                        if (string.IsNullOrEmpty(result))
                        {

                        }
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

        #region 更改节点（服务器同步）

        /// <summary>
        /// 更改标题
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void UpdateItemTittle(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //避免重复设置（标题）
                        if (item.ACA_Tittle != academicReviewItemTransferEntity.Title && !item.IsTittleEditNow)
                        {
                            //指定节点的标题设置
                            item.ACA_Tittle = academicReviewItemTransferEntity.Title;
                            //指定节点的协议实体的标题设置
                            item.AcademicReviewItemTransferEntity.Title = item.ACA_Tittle;
                            //指定节点的选择起始位置设置为文本的长度
                            item.txtTittle.SelectionStart = item.txtTittle.Text.Length;
                            //显示操作提示
                            ConferenceTreeItem.ShowOperationTip(item, academicReviewItemTransferEntity.Operationer);
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 更改评论
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void UpdateItemComment(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //避免重复设置（评论）
                        if (item.ACA_Comment != academicReviewItemTransferEntity.Comment && !item.IsCommentEditNow)
                        {
                            //指定节点的评论设置
                            item.ACA_Comment = academicReviewItemTransferEntity.Comment;
                            //指定节点的协议实体的评论设置
                            item.AcademicReviewItemTransferEntity.Comment = item.ACA_Comment;
                            item.CommentCommandVisibility = Visibility.Visible;

                            //显示操作提示
                            ConferenceTreeItem.ShowOperationTip(item, academicReviewItemTransferEntity.Operationer);
                        }

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 更改链接
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void UpdateItemLink(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {

                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        item.stackPanelLinkList.Children.Clear();
                        foreach (var link in academicReviewItemTransferEntity.LinkList)
                        {
                            string fileName = System.IO.Path.GetFileName(link);
                            item.LinkListItemAdd(fileName, link);
                        }
                        item.LinkCommandVisibility = Visibility.Visible;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
            finally
            {
            }
        }

        /// <summary>
        /// 更改节点（服务器同步）
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void UpdateItem(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //避免重复设置（标题）
                        if (item.ACA_Tittle != academicReviewItemTransferEntity.Title && !item.IsTittleEditNow)
                        {
                            //指定节点的标题设置
                            item.ACA_Tittle = academicReviewItemTransferEntity.Title;
                            //指定节点的协议实体的标题设置
                            item.AcademicReviewItemTransferEntity.Title = item.ACA_Tittle;
                            //指定节点的选择起始位置设置为文本的长度
                            item.txtTittle.SelectionStart = item.txtTittle.Text.Length;
                        }

                        //避免重复设置（评论）
                        if (item.ACA_Comment != academicReviewItemTransferEntity.Comment && !item.IsCommentEditNow)
                        {
                            //指定节点的评论设置
                            item.ACA_Comment = academicReviewItemTransferEntity.Comment;
                            //指定节点的协议实体的评论设置
                            item.AcademicReviewItemTransferEntity.Comment = item.ACA_Comment;
                            item.CommentCommandVisibility = Visibility.Visible;
                        }

                        if (string.IsNullOrEmpty(academicReviewItemTransferEntity.Comment))
                        {
                            item.CommentCommandVisibility = Visibility.Collapsed;
                            item.CommentVisibility = Visibility.Collapsed;
                        }

                        if (academicReviewItemTransferEntity.LinkList.Count > 0)
                        {
                            //更改链接
                            UpdateItemLink(academicReviewItemTransferEntity);
                        }
                        else
                        {
                            item.LinkCommandVisibility = Visibility.Collapsed;
                            item.LinkListVisibility = Visibility.Collapsed;
                        }

                        break;
                    }
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 焦点去除（被最后一个占用者剔除焦点）

        /// <summary>
        /// 焦点去除（被最后一个占用者剔除焦点）
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void RemoveTittleFocus(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid) && !academicReviewItemTransferEntity.FocusAuthor.Equals(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.LoginUserName))
                    {
                        if (item.txtTittle.IsFocused)
                        {
                            //踢掉标题焦点
                            item.txtHelper.Focus();
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 焦点去除（被最后一个占用者剔除焦点）
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void RemoveCommentFocus(ConferenceTreeItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历记录所有知识树的记录
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    //指定与服务器传过来的协议实体GUID相同的知识树节点
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid) && !academicReviewItemTransferEntity.FocusAuthor.Equals(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.LoginUserName))
                    {
                        if (item.txtComment.IsFocused)
                        {
                            //踢掉标题焦点
                            item.txtHelper.Focus();
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 显示评论

        /// <summary>
        /// 显示评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void imgComment_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ShowComment();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 显示评论
        /// </summary>
        void ShowComment()
        {
            try
            {
                //设置当前评论关联的子节点
                //ConferenceTreeItem2.CurrentAcademicReviewItem = this;
                //设置评论窗体的GUID
                ////ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.MessageGuid = ConferenceTreeItem2.CurrentAcademicReviewItem.ACA_Guid;
                //////设置评论窗体的标题
                ////ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.MessageTittle = this.ACA_Tittle;
                //////设置评论窗体的评论
                ////ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Comment = this.ACA_Comment;
                //////设置评论窗体的简介
                ////ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Introduction = this.ACA_Introduction;
                //////评论窗体显示
                ////ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Visibility = System.Windows.Visibility.Visible;
                ////查看当前用户是否有权限进行评论和简介的编辑
                //if (Constant.IsMeetingPresenter)
                //{
                //    ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.IsReadOnly = false;
                //}
                //else
                //{
                //    ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.IsReadOnly = true;
                //}
                ////显示评论窗体
                //ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Show();
                ////激活评论窗体
                //ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Activate();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        /// <summary>
        /// 通过主窗体点击事件去捕获点击到的元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {


                //获取源
                if (e.OriginalSource != null && e.OriginalSource is Controls.Image)
                {
                    //图片
                    Controls.Image img = e.OriginalSource as Controls.Image;
                    //tag值绑定的数据
                    if (img.Tag != null)
                    {
                        return;
                    }
                }
                //评论窗体显示
                //ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Visibility = System.Windows.Visibility.Hidden;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 更改评论

        /// <summary>
        /// 更改评论
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="comment"></param>
        /// <param name="introduction"></param>
        void messagWindow_TextChangedEvent(object sender, string comment, string introduction)
        {
            try
            {
                ////当前知识树选择子节点实例（通过打开评论窗体去获取）
                //if (ConferenceTreeItem.CurrentAcademicReviewItem != null)
                //{
                //    if ((sender as TextBox).IsKeyboardFocused)
                //    {
                //        //评论关联的子节点设置评论
                //        ConferenceTreeItem.CurrentAcademicReviewItem.ACA_Comment = comment;
                //        //评论关联的子节点设置简介
                //        //ConferenceTreeItem2.CurrentAcademicReviewItem.ACA_Introduction = introduction;

                //        //评论关联的子节点的协议实体的评论
                //        ConferenceTreeItem.CurrentAcademicReviewItem.AcademicReviewItemTransferEntity.Comment = comment;                       
                //        //评论关联的子节点的协议实体的操作人
                //        ConferenceTreeItem.CurrentAcademicReviewItem.AcademicReviewItemTransferEntity.Operationer = Constant.SelfUri;
                //        //向服务器提交一个更新的节点
                //        ModelManage.ConferenceTree.Update(Constant.ConferenceName, ConferenceTreeItem.CurrentAcademicReviewItem.AcademicReviewItemTransferEntity, new Action<string>((result) =>
                //        {
                //        }));
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 清除当前节点的所有投票

        /// <summary>
        /// 清除当前节点的所有投票
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemAllVoteClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //清除当前节点的所有投票
                this.AllVoteClear();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 清除当前节点的所有投票
        /// </summary>
        public void AllVoteClear()
        {
            try
            {
                //指定操作人
                this.AcademicReviewItemTransferEntity.Operationer = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfUri;
                //清除节点所有投票
                ModelManage.ConferenceTree.ClearItemAllVote(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((successed) =>
                    {
                        //调用成功，清除本地对应节点的所有投票
                        if (successed)
                        {
                            ////协议树节点赞成票为0
                            //this.AcademicReviewItemTransferEntity.YesVoteCount = 0;
                            ////协议树节点反对票为0
                            //this.AcademicReviewItemTransferEntity.NoVoteCount = 0;
                            ////清除所有投票相关人
                            //if (this.AcademicReviewItemTransferEntity.VotedParticipantList != null && this.AcademicReviewItemTransferEntity.VotedParticipantList.Count > 0)
                            //{
                            //    this.AcademicReviewItemTransferEntity.VotedParticipantList.Clear();
                            //}
                            ////计算赞成票与反对票显示长度的对比
                            //this.CalculateVoteDisplay(this, 0, 0);
                        }
                    }));

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 结束评论窗体

        /// <summary>
        /// 结束评论窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                //ConferenceTreeItem2.ConferenceTreeItem2CommentWindow.Close();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 清除缓存

        /// <summary>
        /// 记住如果重新进入一个会议室,该实例弃用,把描述静态的实例释放
        /// </summary>
        public static void SessionClear()
        {
            try
            {
                //清除所有节点附加数据
                for (int i = ConferenceTreeItem.AcademicReviewItemList.Count - 1; i > -1; i--)
                {
                    ConferenceTreeItem.ClearAddData(ConferenceTreeItem.AcademicReviewItemList[i]);
                }

                //记录所有知识树的记录清空
                ConferenceTreeItem.AcademicReviewItemList.Clear();
                //GUID集合清除
                ConferenceTreeItem.retunList.Clear();
                //根记录恢复为默认值
                ConferenceTreeItem.RootCount = 0;
                //评论关联的子节点恢复为默认值
                ConferenceTreeItem.currentConferenceTreeItem = null;


            }
            catch (Exception ex)
            {

                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 计算赞成票与反对票显示长度的对比

        /// <summary>
        /// 计算赞成票与反对票显示长度的对比
        /// </summary>
        /// <param name="YesVoteCount"></param>
        /// <param name="NoVoteCount"></param>
        public void CalculateVoteDisplay(ConferenceTreeItem item, int YesVoteCount, int NoVoteCount)
        {
            try
            {
                //赞成票的对比度显示值设置
                //item.YesVoteLength = ((double)YesVoteCount / ((double)YesVoteCount + (double)NoVoteCount)) * item.VoteAllLength;
                ////反对票的对比度显示值设置
                //item.NoVoteLength = ((double)NoVoteCount / ((double)YesVoteCount + (double)NoVoteCount)) * item.VoteAllLength;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 添加链接

        public void LinkListItemToService(string uri)
        {
            try
            {

                if (this.AcademicReviewItemTransferEntity.LinkList == null)
                {
                    this.AcademicReviewItemTransferEntity.LinkList = new List<string>();
                }
                this.AcademicReviewItemTransferEntity.LinkList.Add(uri);
                //设置当前协议实体的操作人
                this.AcademicReviewItemTransferEntity.Operationer = TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfUri;
                //通知服务器节点更新
                ModelManage.ConferenceTree.LinkAdd(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<string>((result) =>
                {
                    if (string.IsNullOrEmpty(result))
                    {

                    }
                }));
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
        /// 服务器通知添加链接
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="uri"></param>
        public void LinkListItemAdd(string fileName, string uri)
        {
            try
            {
                this.LinkCommandVisibility = System.Windows.Visibility.Visible;
                TextBlock txt = new TextBlock() { Text = fileName, Tag = uri };
                txt.PreviewMouseLeftButtonDown += txt_PreviewMouseLeftButtonDown;
                this.stackPanelLinkList.Children.Add(txt);
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
        /// 打开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txt_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //还原类型
                TextBlock txt = sender as TextBlock;
                //获取文件地址
                string uri = Convert.ToString(txt.Tag);
                if (this.selectedLink != null)
                {
                    this.selectedLink.Background = new SolidColorBrush(Colors.Transparent);
                }
                this.selectedLink = txt;
                this.selectedLink.Background = new SolidColorBrush(Colors.SkyBlue) { Opacity = 0.6 };
                //获取类型
                string extension = System.IO.Path.GetExtension(uri);
                //移除小数点
                extension = extension.Replace(".", string.Empty);
                //转为枚举类型
                ConferenceWebCommon.EntityHelper.ConferenceOffice.FileType fileType = (ConferenceWebCommon.EntityHelper.ConferenceOffice.FileType)Enum.Parse(typeof(ConferenceWebCommon.EntityHelper.ConferenceOffice.FileType), extension, true);
                //打开文件
                MainWindow.MainPageInstance.ConferenceTreeView.FileOpenByExtension(fileType, uri);

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

        #region 显示操作提示

        /// <summary>
        /// 显示操作提示
        /// </summary>
        /// <param name="item"></param>
        public static void ShowOperationTip(ConferenceTreeItem item, string operater)
        {
            try
            {
                if (!operater.Equals(TreeCodeEnterEntity.TreeCodeEnterEntityInstance.SelfName))
                {
                    #region 显示操作提示

                    if (!string.IsNullOrEmpty(item.TitleOperationer) && item.TitleOperationer.Equals(operater))
                    {
                        if (item.OperationVisibilityTimer != null)
                        {
                            item.CanSettingEditCollpased = false;
                            item.OperationVisibilityTimer.Stop();
                            item.OperationVisibilityTimer = null;

                            TimerJob.StartRun(new Action(() =>
                            {
                                item.OperationVisibility = Visibility.Collapsed;
                                item.IsLocked = true;
                                item.CanSettingEditCollpased = true;
                                if (item.OperationVisibilityTimer != null)
                                {
                                    item.OperationVisibilityTimer.Stop();
                                    item.OperationVisibilityTimer = null;
                                }
                            }), 3000, out item.OperationVisibilityTimer);
                        }
                        else
                        {
                            TimerJob.StartRun(new Action(() =>
                            {
                                if (item.CanSettingEditCollpased)
                                {
                                    item.OperationVisibility = Visibility.Collapsed;
                                    item.IsLocked = true;
                                    if (item.OperationVisibilityTimer != null)
                                    {
                                        item.OperationVisibilityTimer.Stop();
                                    }
                                }

                            }), 3000, out item.OperationVisibilityTimer);
                        }
                    }
                    else
                    {
                        TimerJob.StartRun(new Action(() =>
                        {
                            if (item.CanSettingEditCollpased)
                            {
                                item.OperationVisibility = Visibility.Collapsed;
                                item.IsLocked = true;
                                if (item.OperationVisibilityTimer != null)
                                {
                                    item.OperationVisibilityTimer.Stop();
                                }
                            }

                        }), 3000, out item.OperationVisibilityTimer);
                    }

                    item.OperationVisibility = Visibility.Visible;
                    //操作人
                    item.TitleOperationer = operater;
                    //只读
                    item.IsLocked = false;

                    //item.txtHelper.Focus();

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        public void ShowOperationTipHelper()
        {
            try
            {

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
