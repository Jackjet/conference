using ConferenceCommon.TimerHelper;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.OfficeHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.SharePointHelper;
using ConferenceCommon.WebHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;
using SP = Microsoft.SharePoint.Client;
using uc = Microsoft.Office.Uc;
using ConferenceCommon.WPFHelper;
using ConferenceCommon.WPFControl;
using Conference_Space.Common;
using vy = System.Windows.Visibility;
using fileType = ConferenceCommon.WPFControl.FileType;

namespace Conference_Space
{
    public partial class SpaceBase : UserControlBase
    {      
        #region 数据变量

        /// <summary>
        /// 临时打开的书本集
        /// </summary>
        public List<UCBook> BookTempList;

        /// <summary>
        /// 书本集合
        /// </summary>
        public List<UCBook> BookList = new List<UCBook>();

        int rowCount = 4;
        /// <summary>
        /// 行的数量
        /// </summary>
        public int RowCount
        {
            get { return rowCount; }
            set
            {
                if (value != rowCount)
                {
                    rowCount = value;
                    //如果行的数量发生改变，那就重新调整书架
                    //this.ReFlush(GetListStart(this.PageNow), this.BookTempList);
                }
            }
        }

        int pageBookCount;
        /// <summary>
        /// 当前状态所能承载的书的数量
        /// </summary>
        public int PageBookCount
        {
            get { return pageBookCount; }
            set
            {
                if (value != pageBookCount)
                {
                    if (value > 0)
                    {
                        //如果当前页的数量大于页的总数量，那就将当前页设置为最后一页
                        this.PageCount = (int)Math.Ceiling(this.BookList.Count / Convert.ToDouble(value));
                    }
                    else
                    {
                        //如果当前页的数量大于页的总数量，那就将当前页设置为最后一页
                        this.PageCount = (int)Math.Ceiling(this.BookList.Count / Convert.ToDouble(1));
                    }
                    pageBookCount = value;
                }
            }
        }

        UCBook selectedUCbook;
        /// <summary>
        /// 当前选定的书本
        /// </summary>
        public UCBook SelectedUCbook
        {
            get { return selectedUCbook; }
            set { selectedUCbook = value; }
        }

        #endregion

        #region 绑定属性

        int pageNow = 1;
        /// <summary>
        /// 当前浏览的页
        /// </summary>
        public int PageNow
        {
            get { return pageNow; }
            set
            {
                if (value != pageNow)
                {
                    pageNow = value;
                    //重新调整
                    this.ReFlush_BookShell(GetListStart(value), this.BookTempList);
                    this.OnPropertyChanged("PageNow");
                }
            }
        }

        int pageCount;
        /// <summary>
        /// 页的数量
        /// </summary>
        public int PageCount
        {
            get { return pageCount; }
            set
            {
                if (value != pageCount && value > 0)
                {
                    //先赋值后刷新，因为刷新的方法里已经有了对页数量的更改，避免走入死循环
                    pageCount = value;
                    //重新调整
                    this.ReFlush_BookShell(this.GetListStart(this.PageNow), this.BookTempList);
                    this.OnPropertyChanged("PageCount");
                }
            }
        }

        //string serchText = string.Empty;
        ///// <summary>
        ///// 查询的字符
        ///// </summary>
        //public string SerchText
        //{
        //    get { return serchText; }
        //    set
        //    {
        //        if (value != this.serchText)
        //        {
        //            serchText = value;
        //            this.OnPropertyChanged("SerchText");
        //        }
        //    }
        //}

        string currentUserName;
        /// <summary>
        /// 当前用户名
        /// </summary>
        public string CurrentUserName
        {
            get { return currentUserName; }
            set
            {
                if (value != currentUserName)
                {
                    currentUserName = value;
                    this.OnPropertyChanged("CurrentUserName");
                }
            }
        }

        protected string root1;
        /// <summary>
        /// 根目录名称
        /// </summary>
        public string Root1
        {
            get { return root1; }
            set
            {
                //if (value != root1)
                //{
                root1 = value;
                this.BreadLineRoot.Title = value;
                //}
            }
        }

        #endregion

        #region 书架初始化

        /// <summary>
        /// 导航初始化
        /// </summary>        
        void DaoHangInit()
        {
            try
            {
                //临时的书本集合
                this.BookTempList = BookList;


                //左箭头点击事件
                BtnArrowLeft.Click += (object sender, RoutedEventArgs e) =>
                {
                    //前提是当前页不是第一页
                    if (this.PageNow > 1)
                    {
                        //当前页自减
                        this.PageNow--;
                    }
                };

                //右箭头的点击事件
                BtnArrowRight.Click += (object sender, RoutedEventArgs e) =>
                {
                    //前提是当前页不是最后一页
                    if (this.PageNow < Convert.ToInt32(this.PageCount))
                    {
                        //当前页自增
                        this.PageNow++;
                    }
                };
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

        #region 子项管理（添加、删除子项）

        /// <summary>
        /// 添加子项
        /// </summary>
        /// <param name="book">书本的实例</param>
        public void Items_Add(UCBook ucBook)
        {
            try
            {
                //给书本集合添加书本
                this.BookList.Add(ucBook);
                ucBook.PreviewMouseLeftButtonDown += ucBook_PreviewMouseLeftButtonDown;
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
        /// 添加多本书籍
        /// </summary>
        /// <param name="ucBookList">书本集合</param>
        public void Items_Add(List<UCBook> ucBookList)
        {
            try
            {
                foreach (var ucBook in ucBookList)
                {
                    Items_Add(ucBook);
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
        /// 通过索引删除某子项
        /// </summary>
        /// <param name="index">索引</param>
        public void Items_Remove(int index)
        {
            try
            {
                //索引必须必需在有效范围内才可以使用
                if (this.BookList.Count > index && index > -1)
                {
                    this.BookList.RemoveAt(index);
                }
                //刷新
                this.ReFlush_BookShell();
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
        /// 删除指定子项
        /// </summary>
        /// <param name="ucBook">书本</param>
        public void Items_Remove(UCBook ucBook)
        {
            try
            {
                //删除子项
                this.BookList.Remove(ucBook);
                //刷新
                this.ReFlush_BookShell();
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
        /// 清除数据
        /// </summary>
        public void Items_Clear()
        {
            try
            {
                this.BookList.Clear();
                //刷新
                this.ReFlush_BookShell();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 书本点击事件

        /// <summary>
        /// 书本点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucBook_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is UCBook)
            {
                if (this.SelectedUCbook != null)
                {
                    this.SelectedUCbook.SelectedVisibility = vy.Collapsed;
                }
                this.SelectedUCbook = sender as UCBook;
                this.SelectedUCbook.SelectedVisibility = vy.Visible;
                //打开文件或文件夹事件
                if (this.OpenFileEventCallBack != null)
                {
                    this.OpenFileEventCallBack(this.SelectedUCbook);
                }

            }
        }

        #endregion

        #region 书架刷新

        /// <summary>
        /// 通过指定的书本集合位置和书本集合来给书架添加书本
        /// </summary>
        /// <param name="RowPosition">书本集合的指定位置</param>
        /// <param name="bookListSelf">书本集合</param>
        public void ReFlush_BookShell(int RowPosition, List<UCBook> bookListSelf) 
        {
            try
            {
                //清除书架上的书本
                this.GridBookParent.Children.Clear();

                //列的数量
                int columnCount = this.GridBookParent.ColumnDefinitions.Count;


                int k = 0;
                //先使用第一行所拥有的空位
                for (; k < columnCount; RowPosition++, k++)
                {
                    //索引必须小于书本集的数量
                    if (RowPosition < bookListSelf.Count && BookList.Count > 0)
                    {
                        //添加子项（先决条件：定位）
                        this.ItemsAddByPosition(bookListSelf[RowPosition], 0, k);
                    }
                }

                //开始添加第二行
                for (int j = 0; j < columnCount; j++, RowPosition++)
                {
                    //索引必须小于书本集的数量
                    if (RowPosition < bookListSelf.Count && BookList.Count > 0)
                    {
                        //添加子项（先决条件：定位）
                        this.ItemsAddByPosition(bookListSelf[RowPosition], 1, j);
                    }
                }

                //开始添加第三行
                for (int f = 0; f < columnCount; f++, RowPosition++)
                {
                    //索引必须小于书本集的数量
                    if (RowPosition < bookListSelf.Count && BookList.Count > 0)
                    {
                        //添加子项（先决条件：定位）
                        this.ItemsAddByPosition(bookListSelf[RowPosition], 2, f);
                    }
                }

                //开始添加第三行
                for (int h = 0; h < columnCount; h++, RowPosition++)
                {
                    //索引必须小于书本集的数量
                    if (RowPosition < bookListSelf.Count && BookList.Count > 0)
                    {
                        //添加子项（先决条件：定位）
                        this.ItemsAddByPosition(bookListSelf[RowPosition], 3, h);
                    }
                }

                //书架当前所能承载的书本的数量
                this.PageBookCount = columnCount * this.RowCount;
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
        ///刷新（更改子项数量时发生）
        /// </summary>
        public void ReFlush_BookShell()
        {
            try
            {
                this.ReFlush_BookShell(GetListStart(this.PageNow), this.BookTempList);

                if (this.PageBookCount > 0)
                {
                    //如果当前页的数量大于页的总数量，那就将当前页设置为最后一页
                    this.PageCount = (int)Math.Ceiling(this.BookList.Count / Convert.ToDouble(this.PageBookCount));
                }
                else
                {
                    //如果当前页的数量大于页的总数量，那就将当前页设置为最后一页
                    this.PageCount = (int)Math.Ceiling(this.BookList.Count / Convert.ToDouble(1));
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

        #region 添加子项（先决条件：定位）

        /// <summary>
        /// 添加子项（先决条件：定位）
        /// </summary>
        public void ItemsAddByPosition(UCBook ucBook, int rowPosition, int columnPosition)
        {
            //【给元素定位行】
            Grid.SetRow(ucBook, rowPosition);
            //【给元素定位列】
            Grid.SetColumn(ucBook, columnPosition);
            //容器添加子项【添加】
            this.GridBookParent.Children.Add(ucBook);
        }

        #endregion

        #region 通过当前页来指定书本集合的起始位置

        /// <summary>
        /// 通过当前页来指定书本集合的起始位置
        /// </summary>
        /// <param name="pageNN">当前页</param>
        public int GetListStart(int pageNN)
        {
            int result = 0;
            try
            {
                //返回一个起始的位置（第一页为0）
                if (pageNN > 0)
                {
                    result = (pageNN * this.PageBookCount) - this.PageBookCount;
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return result;
        }

        #endregion


        #region 搜索文本更改

        /// <summary>
        /// 搜索文本更改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void txtSearchChanged(object sender, TextChangedEventArgs e)
        {
            //创建一个临时储存符合关键字的书本集合
            //List<UCBook> bookListLinShi = new List<UCBook>();
            //通过关键字来给临时书本集合添加符合的书本
            foreach (var item in BookList)
            {
                var txt = (sender as TextBox).Text.Trim();
                //通过标题来判断
                if (item.Book_Title.Contains(txt) && !string.IsNullOrEmpty(txt))
                {
                    item.ArrowVisibility = vy.Visible;
                    //启动箭头跳动动画
                    item.RunArrowSkipAnimation();
                }
                else
                {
                    item.ArrowVisibility = vy.Collapsed;
                }
            }
        }

        #endregion

        #region 上传文件

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void menuItemUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.UploadFileEventCallBack != null)
                {
                    this.UploadFileEventCallBack();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 新建文件夹

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void menuItemFolderCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.FolderCreateEventCallBack != null)
                {
                    this.FolderCreateEventCallBack();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 资源下载

        /// <summary>
        /// 资源下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResourceDownLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedUCbook != null)
                {
                    this.SelectedUCbook.menuItemDownload();
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

        #region 资源演示

        /// <summary>
        /// 资源演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResourceDisplay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedUCbook != null)
                {
                    this.SelectedUCbook.menuItemRealTimeShare();
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

        #region 资源推送

        /// <summary>
        /// 资源推送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResourceSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedUCbook != null)
                {
                    this.SelectedUCbook.menuItemFileSend();
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

        #region 资源重命名

        /// <summary>
        /// 资源重命名
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResourceReName_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 资源移动

        /// <summary>
        /// 资源移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnResourceMove_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 资源删除

        /// <summary>
        /// 资源删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnResourceDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedUCbook != null)
                {                    
                    this.SelectedUCbook.menuItemDelete();
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
        /// 删除文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void menuItemFolderDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.SelectedUCbook != null)
                {
                    this.SelectedUCbook.menuItemDelete();
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
    }
}
