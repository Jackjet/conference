using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SP = Microsoft.SharePoint.Client;


namespace Conference_Space
{
    /// <summary>
    /// UCBookFrame.xaml 的交互逻辑
    /// </summary>
    public partial class UCBook : Grid, INotifyPropertyChanged
    {
        #region 自定义委托事件(回调)
      
        /// <summary>
        /// 文件删除回调
        /// </summary>
        public Action<UCBook> FileDeleteCallBack = null;
        
        /// <summary>
        /// 文件推送回调
        /// </summary>
        public Action<string, FileType> FileSendCallBack = null;
    
        /// <summary>
        /// 文件共享回调
        /// </summary>
        public Action<string, FileType> FileShareCallBack = null;
   
        /// <summary>
        /// 文件下载回调
        /// </summary>
        public Action<string> FileDownLoadCallBack = null;

        #endregion

        #region 内部字段

        #endregion

        #region 一般属性

        string date;
        /// <summary>
        /// 书本的发布日期
        /// </summary>
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        BookType bookType = BookType.Folder;
        /// <summary>
        /// 书本类型【文件、文件夹】
        /// </summary>
        public BookType BookType
        {
            get { return bookType; }
            set
            {
                switch (value)
                {
                    case BookType.File:

                        break;
                    case BookType.Folder:
                        //删除一些不属于文件夹的快捷菜单
                        //this.context_Menu.Items.Remove(this.menuItem_Download);
                        //this.context_Menu.Items.Remove(this.menuItem_Share);
                        break;
                    default:
                        break;
                }
                bookType = value;
            }
        }

        FileType fileType;
        /// <summary>
        /// 文件类型
        /// </summary>
        public FileType FileType
        {
            get { return fileType; }
            set
            {
                fileType = value;
            }
        }

        string uri;
        /// <summary>
        /// 路径
        /// </summary>
        public string Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        //SP.Folder _folder;
        ///// <summary>
        ///// 该容器所包含的文件夹（属于文件夹的时候才有）
        ///// </summary>
        //public SP.Folder Folder
        //{
        //    get { return _folder; }
        //    set { _folder = value; }
        //}

        BreadLine breadLine = null;
        /// <summary>
        /// 书本关联的面包线（属于文件夹的时候才有）
        /// </summary>
        public BreadLine BreadLine
        {
            get { return breadLine; }
            set { breadLine = value; }
        }


        //SP.File _file;
        ///// <summary>
        ///// 该容器所包含的文档
        ///// </summary>
        //public SP.File File
        //{
        //    get { return _file; }
        //    set { _file = value; }
        //}


        //SP.List _list;
        ///// <summary>
        ///// 该容器所包含的列表
        ///// </summary>
        //public SP.List List
        //{
        //    get { return _list; }
        //    set { _list = value; }
        //}

        #endregion

        #region 实时更新

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region 绑定属性

        string book_Title;
        /// <summary>
        /// 书本的标题
        /// </summary>
        public string Book_Title
        {
            get { return book_Title; }
            set
            {
                if (value != this.book_Title)
                {
                    book_Title = value;
                    this.OnPropertyChanged("Book_Title");
                }
            }
        }

        //double path_Opacity = 0.8;
        ///// <summary>
        ///// 装饰带的透明度设置
        ///// </summary>
        //public double Path_Opacity
        //{
        //    get { return path_Opacity; }
        //    set
        //    {
        //        if (value != this.path_Opacity)
        //        {
        //            path_Opacity = value;
        //            this.OnPropertyChanged("Path_Opacity");
        //        }
        //    }
        //}

        Visibility arrowVisibility = Visibility.Collapsed;
        /// <summary>
        /// 显示箭头
        /// </summary>
        public Visibility ArrowVisibility
        {
            get { return arrowVisibility; }
            set
            {
                if (value != arrowVisibility)
                {
                    arrowVisibility = value;
                    this.OnPropertyChanged("ArrowVisibility");
                }
            }
        }

        double row1Height = 50;
        /// <summary>
        /// 第一行的高度
        /// </summary>
        public double Row1Height
        {
            get { return row1Height; }
            set
            {
                if (value != this.row1Height)
                {
                    row1Height = value;
                    this.OnPropertyChanged("Row1Height");
                }
            }
        }

        double row2Height = 70;
        /// <summary>
        /// 第二行的高度
        /// </summary>
        public double Row2Height
        {
            get { return row2Height; }
            set
            {
                if (value != this.row2Height)
                {
                    row2Height = value;
                    this.OnPropertyChanged("Row2Height");
                }
            }
        }

        Visibility selectedVisibility = Visibility.Collapsed;
        /// <summary>
        /// 文件选择显示
        /// </summary>
        public Visibility SelectedVisibility
        {
            get { return selectedVisibility; }
            set
            {
                if (value != this.selectedVisibility)
                {
                    selectedVisibility = value;
                    this.OnPropertyChanged("SelectedVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 书本的构造函数（无参数）
        /// </summary>
        public UCBook()
        {
            try
            {
                this.InitializeComponent();

                //绑定当前上下文
                this.DataContext = this;
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
        /// 书本的构造函数(普通元素构造者)
        /// </summary>
        /// <param name="title">书本的标题</param>             
        /// <param name="imageUri">书的背景图</param>
        public UCBook(string title, string imageName)
            : this()
        {
            try
            {
                Uri uri = default(Uri);

                if (imageName.Contains("http://"))
                {
                    uri = new Uri(imageName);
                }
                else
                {
                    uri = new Uri("pack://application:,,," + imageName, UriKind.RelativeOrAbsolute);
                }
                BitmapImage imageSource = new BitmapImage(uri);
                this.TomBook_Loaded(title, imageSource);
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
        /// 书本的构造函数(普通元素构造者【供内部使用】)
        /// </summary>
        /// <param name="title">书本的标题</param>             
        /// <param name="imageUri">书的背景图</param>
        public UCBook(string title, ImageSource imageSource)
            : this()
        {
            try
            {
                this.TomBook_Loaded(title, imageSource);
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

        #region 书本初始化加载

        /// <summary>
        /// 加载书本时发生的事件（比如说赋予给父容器）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="title">书本的标题</param>
        /// <param name="data">书本的发布日期</param>
        /// <param name="note">书本的备注</param>
        /// <param name="imageUri">书本的背景图</param>
        void TomBook_Loaded(string title, ImageSource imageSource)
        {
            try
            {
                this.Book_Title = title;

                this.borBook.Background = new ImageBrush(imageSource);

                ////当鼠标进入设置为显示（文本，束带）
                //this.MouseEnter += (object sender1, MouseEventArgs e1) =>
                //    {
                //        this.Path_Opacity = 0.3;
                //    };
                ////当鼠标进入设置为隐藏（文本，束带）
                //this.MouseLeave += (object sender1, MouseEventArgs e1) =>
                //{
                //    this.Path_Opacity = 0.8;
                //};
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

        #region 启动箭头跳动动画

        /// <summary>
        /// 启动箭头跳动动画
        /// </summary>
        public void RunArrowSkipAnimation()
        {
            try
            {
                var storyBoard = this.Resources["Storyboard1"] as Storyboard;
                storyBoard.Begin();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 删除文件

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void menuItemDelete()
        {
            try
            {
                //激发删除文件事件
                if (this.FileDeleteCallBack != null)
                {
                    this.FileDeleteCallBack(this);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 文件推送

       /// <summary>
        /// 文件推送
       /// </summary>
        public void menuItemFileSend()
        {
            try
            {
                //激发文件共享事件
                if (this.FileSendCallBack != null)
                {
                    this.FileSendCallBack(this.Uri, this.FileType);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 实时共享
       
        /// <summary>
        /// 实时共享
        /// </summary>
        public void menuItemRealTimeShare()
        {
            try
            {
                //激发文件共享事件
                if (this.FileShareCallBack != null)
                {
                    this.FileShareCallBack(this.Uri, this.FileType);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
        #endregion

        #region 文件下载
        
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void menuItemDownload()
        {
            try
            {
                //激发文件下载事件
                if (this.FileDownLoadCallBack != null)
                {
                    this.FileDownLoadCallBack(this.Uri);
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
