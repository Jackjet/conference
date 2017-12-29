using Conference.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using SP = Microsoft.SharePoint.Client;

namespace Conference.View.Space
{
    /// <summary>
    /// 面包线（第一层是没有关联ucBook的，第一层是根）
    /// </summary>
    public partial class BreadLine :UserControlBase
    {
        #region 自定义委托事件

        public delegate void LineTitleClickEventHandle(BreadLine breadLine);
        /// <summary>
        /// 面包线点击事件（禁止直接使用该实例去注册【因为有子项】）
        /// </summary>
        public event LineTitleClickEventHandle LineClickEvent = null;

        #endregion

        #region 一般属性

        SP.Folder _folder;
        /// <summary>
        /// 该面包线所关联的文件夹
        /// </summary>
        public SP.Folder Folder
        {
            get { return _folder; }
            set { _folder = value; }
        }

        BreadLine breadLineChild;
        /// <summary>
        /// 面包线子节点
        /// </summary>
        public BreadLine BreadLineChild
        {
            get { return breadLineChild; }
            set
            {
                if (value != breadLineChild)
                {
                    this.borPanel.Child = value;
                    breadLineChild = value;
                }
            }
        }

        #endregion

        #region 绑定属性

        string title;
        /// <summary>
        /// 面包线标题
        /// </summary>
        public string Title
        {
            get { return title; }
            set
            {
                if (value != title)
                {
                    title = value;
                    this.OnPropertyChanged("Title");
                }
            }
        }



        #endregion

        #region 依赖项属性

        //public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BreadLine), new PropertyMetadata(new PropertyChangedCallback((obj,e)=>
        //    {
        //    })));

        //public string Title
        //{
        //    get
        //    {
        //        return (string)base.GetValue(BreadLine.TitleProperty);
        //    }
        //    set
        //    {
        //        base.SetValue(BreadLine.TitleProperty, value);
        //        this.txt2.Text = value;
        //    }
        //}

        #endregion
       
        #region 构造函数

        public BreadLine()
        {
            InitializeComponent();

            //绑定当前上下文
            this.DataContext = this;
        }

        #endregion

        #region 鼠标进入事件

        private void breadLineEnter(object sender, MouseEventArgs e)
        {
            try
            {
                this.txtTitle.Foreground = new SolidColorBrush(Colors.Red);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 鼠标移除事件

        private void breadLineLeave(object sender, MouseEventArgs e)
        {
            try
            {
                this.txtTitle.Foreground = new SolidColorBrush(Colors.Black);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 选择面包线

        /// <summary>
        /// 选择面包线
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breadLineMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        #region 面包线点击
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             try
            {
                 if(this.LineClickEvent!= null)
                 {
                     this.LineClickEvent(this);
                 }
            }
            catch (Exception ex)
            {
               LogManage.WriteLog(this.GetType(),ex);
            }
        }

        #endregion

        #region 清除之前的那根线【第一个面包线是不需要那根线的】

        /// <summary>
        /// 清除之前的那根线【第一个面包线是不需要那根线的】
        /// </summary>
         public void ClearBeforeLine()
        {
              try
            {
                this.txtLine.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
               LogManage.WriteLog(this.GetType(),ex);
            }
        }

        #endregion
    }
}
