using Conference.Page;
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
using Conference.Common;
using ConferenceCommon.WPFHelper;

namespace Conference.View.Tree
{
    /// <summary>
    /// AcademicReviewItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeItemVisual : UserControlBase
    {

        #region 绑定属性

        string aCA_Tittle = Constant.TreeItemEmptyName;
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
                if (this.commentCommandVisibility != value)
                {
                    commentCommandVisibility = value;
                    this.OnPropertyChanged("CommentCommandVisibility");
                }
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
            set { isTittleEditNow = value; }
        }

        bool isCommentEditNow = false;
        /// <summary>
        /// 是否正在编辑评论
        /// </summary>
        public bool IsCommentEditNow
        {
            get { return isCommentEditNow; }
            set { isCommentEditNow = value; }
        }

        #endregion

        #region 静态字段

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object obj1 = new object();

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="NeedInit"></param>
        public ConferenceTreeItemVisual()
        {
            try
            {
                InitializeComponent();

                //设置当前上下文
                this.DataContext = this;


                this.MouseMove += ConferenceTreeView_PreviewMouseMove;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }

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
                    ConferenceTreeView treeView = MainWindow.MainPageInstance.ConferenceTreeView;

                    EndPoint = e.GetPosition(treeView.borDiscussTheme);

                    //计算X、Y轴起始点与终止点之间的相对偏移量
                    double y = EndPoint.Y - item.StartPoint.Y;
                    double x = EndPoint.X - item.StartPoint.X;

                    Thickness margin = treeView.conferenceTreeItemVisual.Margin;

                    //计算新的Margin
                    Thickness newMargin = new Thickness()
                    {
                        Left = margin.Left + x,
                        Top = margin.Top + y,
                        Bottom = margin.Bottom,
                        Right = margin.Right
                    };
                    if (treeView.conferenceTreeItemVisual.Visibility != System.Windows.Visibility.Visible)
                    {
                        //显示虚拟拖拽节点
                        treeView.conferenceTreeItemVisual.Visibility = System.Windows.Visibility.Visible;
                    }
                    //设置移动效果
                    treeView.conferenceTreeItemVisual.Margin = newMargin;

                    //开始位置变为最终的位置
                    item.StartPoint = EndPoint;
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
