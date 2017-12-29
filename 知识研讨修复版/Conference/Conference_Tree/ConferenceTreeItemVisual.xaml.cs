
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

namespace Conference_Tree
{
    /// <summary>
    /// AcademicReviewItem.xaml 的交互逻辑
    /// </summary>
    public partial class ConferenceTreeItemVisual : UserControlBase
    {

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

                //注册移动事件
                this.MouseMove += ConferenceTreeView_PreviewMouseMove;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeItem), ex);
            }
        }        

        #endregion

        #region 子项相应鼠标移动

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
                    ConferenceTreeView treeView = ConferenceTreeView.conferenceTreeView;

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
