
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
using vy = System.Windows.Visibility;

namespace Conference_Tree
{
    /// <summary>
    /// AcademicReviewItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeItem : UserControlBase
    {
        #region 自定义事件回调

        /// <summary>
        /// 更新整棵研讨树完成事件回调
        /// </summary>
        public static Action RefleshCompleateEvent = null;

        /// <summary>
        /// 节点选择事件回调
        /// </summary>
        public static Action<string> SelectedItemCallBack = null;

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

        string aCA_Tittle = TreeCodeEnterEntity.TreeItemEmptyName;
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

        vy selectedVisibility = vy.Collapsed;
        /// <summary>
        /// 选择的节点显示
        /// </summary>
        public vy SelectedVisibility
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

        vy borVisibility = Visibility.Collapsed;
        /// <summary>
        /// 筛选出的节点
        /// </summary>
        public vy BorVisibility
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


        vy commentVisibility = vy.Collapsed;
        /// <summary>
        /// 评论提示
        /// </summary>
        public vy CommentVisibility
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

        vy commentCommandVisibility = vy.Collapsed;
        /// <summary>
        /// 备注控制显示
        /// </summary>
        public vy CommentCommandVisibility
        {
            get { return commentCommandVisibility; }
            set
            {
                commentCommandVisibility = value;
                this.btnComment.Visibility = value;

            }
        }

        vy linkCommandVisibility = vy.Collapsed;
        /// <summary>
        /// 超链接命令显示
        /// </summary>
        public vy LinkCommandVisibility
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

        vy linkListVisibility = vy.Collapsed;
        /// <summary>
        /// 超链接列表显示
        /// </summary>
        public vy LinkListVisibility
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

        vy uploadFileTipVisibility = vy.Collapsed;
        /// <summary>
        /// 文件上传提示
        /// </summary>
        public vy UploadFileTipVisibility
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

        vy operationVisibility = vy.Collapsed;
        /// <summary>
        /// 操作提示
        /// </summary>
        public vy OperationVisibility
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

        vy contentVisibility = vy.Visible;
        /// <summary>
        /// 扩展内容显示
        /// </summary>
        public vy ContentVisibility
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

        vy verticalLineVisibility = vy.Visible;
        /// <summary>
        /// 右侧垂直线条显示
        /// </summary>
        public vy VerticalLineVisibility
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

        vy expanderVisibility;
        /// <summary>
        /// 扩展、收缩按钮显示
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

        int lineColumnSpan;
        /// <summary>
        /// 线条位置
        /// </summary>
        public int LineColumnSpan
        {
            get { return lineColumnSpan; }
            set
            {
                if (this.lineColumnSpan != value)
                {
                    lineColumnSpan = value;
                    this.OnPropertyChanged("LineColumnSpan");
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
        internal static List<ConferenceTreeItem> AcademicReviewItemList = new List<ConferenceTreeItem>();

        /// <summary>
        /// 语音讨论点对点映射集合
        /// </summary>
        internal static List<int> retunList = new List<int>();

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object AddItem_Object = new object();

        /// <summary>
        /// 当前树节点
        /// </summary>
        internal static ConferenceTreeItem currentConferenceTreeItem = null;

        /// <summary>
        /// 是否可以进行刷新
        /// </summary>
        internal static bool goIntoReflesh = true;

        /// <summary>
        /// 展开样式
        /// </summary>
        static ImageBrush brush_unfold = null;

        /// <summary>
        /// 收缩样式
        /// </summary>
        static ImageBrush brush_fold = null;

        #endregion

        #region 协议属性

        ConferenceTreeItemTransferEntity academicReviewItemTransferEntity;
        /// <summary>
        /// 协议实体（用于传输和xml序列化使用）
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
               
                //是否需要通过这些初始化
                if (NeedInit)
                {
                    //初始化()
                    this.ParametersInit();

                    //隐藏弹出论点按钮
                    this.btnComment.Visibility = vy.Collapsed;

                    //第一次事件注册
                    this.First_EventRegedit();
                }
                //样式收集
                this.StyleCollection();

                //常规事件注册
                this.Normal_EventRegedit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

        #endregion

        #region 样式收集

        /// <summary>
        /// 样式收集
        /// </summary>
        public void StyleCollection()
        {
            try
            {
                if (brush_unfold != null && this.Resources.Contains("brush_unfold"))
                {
                    brush_unfold = this.Resources["brush_unfold"] as ImageBrush;
                }
                if (brush_fold != null && this.Resources.Contains("brush_fold"))
                {
                    brush_fold = this.Resources["brush_fold"] as ImageBrush;
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


        #region 事件注册区域

        /// <summary>
        /// 第一次加载事件
        /// </summary>
        public void First_EventRegedit()
        {
            try
            {
                //通过主窗体点击事件去捕获点击到的元素
                TreeCodeEnterEntity.MainWindow.PreviewMouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
              
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
        /// 常规事件注册区域
        /// </summary>
        public void Normal_EventRegedit()
        {
            try
            {
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
                //this.borMain.SizeChanged += borMain_SizeChanged;

                this.btnExpander.Click += btnExpander_Click;

                //节点选择事件
                this.borTreeMain.PreviewMouseLeftButtonDown += ConferenceTreeItem_PreviewMouseLeftButtonDown;
                //拖拽事件
                this.borSelected.PreviewMouseLeftButtonDown += borSelected_PreviewMouseLeftButtonDown;

                #region old solution

                ////会议主持人有添加和删除节点的权限
                //if (TreeCodeEnterEntity.TreeCodeEnterEntityInstance.IsMeetPresenter)
                //{
                //    //添加节点
                //    this.imgAdd.MouseLeftButtonDown += imgAdd_MouseLeftButtonDown;
                //    //删除节点
                //    this.menuItemDelete.Click += menuItemDelete_Click;
                //    //清除当前节点的所有投票
                //    //this.menuItemAllVoteClear.Click += menuItemAllVoteClear_Click;
                //}

                //投票赞成
                //this.imgYesVote.MouseLeftButtonDown += imgVote_MouseLeftButtonDown;
                //this.txtYesVote.MouseLeftButtonDown += imgVote_MouseLeftButtonDown;

                //投票反对
                //this.imgNoVote.MouseLeftButtonDown += imgNoVote_MouseLeftButtonDown;
                //this.txtNoVote.MouseLeftButtonDown += imgNoVote_MouseLeftButtonDown;

                //点击弹出评论窗体
                //this.imgComment.PreviewMouseLeftButtonDown += imgComment_PreviewMouseLeftButtonDown;

                #endregion
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

        #region 树节点子集展开、隐藏事件

        /// <summary>
        /// 树节点子集展开、隐藏事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExpander_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //扩展内容显示
                if (this.ContentVisibility == vy.Visible)
                {
                    this.ContentVisibility = vy.Collapsed;
                    this.VerticalLineVisibility = vy.Collapsed;
                    this.btnExpander.Background = brush_unfold;
                }
                else
                {
                    this.ContentVisibility = vy.Visible;
                    if (this.ACA_ChildList.Count > 1)
                    {
                        this.VerticalLineVisibility = vy.Visible;
                    }
                    this.btnExpander.Background = brush_fold;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 节点选择(包含拖拽)

        /// <summary>
        /// 节点选择
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
                    currentConferenceTreeItem.SelectedVisibility = vy.Collapsed;
                }
                //绑定节点
                currentConferenceTreeItem = this;
                //显示选择
                currentConferenceTreeItem.SelectedVisibility = vy.Visible;
                //選擇事件回調
                this.SelectedItemCallEvent();
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
        /// 選擇事件回調
        /// </summary>
        public void SelectedItemCallEvent()
        {
            try
            {
                //子项选择回调（携带标题）
                if (ConferenceTreeItem.SelectedItemCallBack != null && currentConferenceTreeItem != null)
                {
                    ConferenceTreeItem.SelectedItemCallBack(currentConferenceTreeItem.ACA_Tittle);
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

                Border borDicuss = ConferenceTreeView.conferenceTreeView.borDiscussTheme;

                ConferenceTreeItemVisual visulItem = ConferenceTreeView.conferenceTreeView.conferenceTreeItemVisual;

                //获取选择的树节点的相对容器位置
                Point p = this.TranslatePoint(new Point(0, 0), borDicuss);

                //设置虚拟拖拽节点的坐标
                visulItem.Margin = new Thickness(p.X, p.Y, 0, 0);
                visulItem.Height = this.ActualHeight;
                //显示拖拽节点的标题
                visulItem.ACA_Tittle = this.ACA_Tittle;

                isDrag = true;

                visulItem.Visibility = vy.Visible;

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
        /// 子项拖动完成（鼠标点击弹起）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConferenceTreeView_PreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //鼠标点击释放了，不可再拖动
                    ConferenceTreeItem.currentConferenceTreeItem.isDrag = false;
                }

                //显示虚拟拖拽节点
                ConferenceTreeView.conferenceTreeView.conferenceTreeItemVisual.Visibility = vy.Collapsed;
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

        #region 评论弹出

        /// <summary>
        /// 评论弹出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnComment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.LinkListVisibility = vy.Collapsed;
                //论点面板显示设置
                if (this.CommentVisibility == vy.Visible)
                {
                    this.CommentVisibility = vy.Collapsed;
                    this.LineColumnSpan = 1;
                }
                else
                {
                    this.CommentVisibility = vy.Visible;
                    this.LineColumnSpan = 2;
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

        #region 超链接列表弹出

        /// <summary>
        /// 超链接列表弹出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLinkList_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.CommentVisibility = vy.Collapsed;
                //论点面板显示设置
                if (this.LinkListVisibility == vy.Visible)
                {
                    this.LinkListVisibility = vy.Collapsed;
                    this.LineColumnSpan = 1;
                }
                else
                {
                    this.LinkListVisibility = vy.Visible;
                    this.LineColumnSpan = 2;
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

        #region 焦点获取

        /// <summary>
        /// 标题焦点获取
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
                    if (this.ACA_Tittle.Equals(TreeCodeEnterEntity.TreeItemEmptyName))
                    {
                        this.ACA_Tittle = string.Empty;
                    }
                }
                if (this.AcademicReviewItemTransferEntity != null)
                {
                    //设置占用者
                    this.AcademicReviewItemTransferEntity.FocusAuthor = TreeCodeEnterEntity.LoginUserName;
                    //占用标题
                    this.AcademicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.FocusType1;
                }
                //强行占用焦点
                ModelManage.ConferenceTree.ForceOccupyFocus(TreeCodeEnterEntity.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
                {

                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        /// <summary>
        /// 评论区焦点获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtComment_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //设置占用者
                this.AcademicReviewItemTransferEntity.FocusAuthor = TreeCodeEnterEntity.LoginUserName;
                //占用评论
                this.AcademicReviewItemTransferEntity.Operation = ConferenceTreeOperationType.FocusType2;
                //强行占用焦点
                ModelManage.ConferenceTree.ForceOccupyFocus(TreeCodeEnterEntity.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
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

        #region 键盘弹出

        /// <summary>
        /// 键盘弹出
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
    }
}
