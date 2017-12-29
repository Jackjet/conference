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
        #region 自定义委托事件回调

        /// <summary>
        /// 上传文件回调
        /// </summary>
        public Action UploadFileEventCallBack = null;

        /// <summary>
        /// 打开文件回调
        /// </summary>
        public Action<UCBook> OpenFileEventCallBack = null;

        /// <summary>
        /// 新建文件夹回调
        /// </summary>
        public Action FolderCreateEventCallBack = null;

        /// <summary>
        /// 当前用户导航到指定页面回调
        /// </summary>
        public Action<Action<bool>> ShareInConversationSelfNavicateCallBack = null;

        ///// <summary>
        ///// 同步其他参会人到指定页面回调
        ///// </summary>
        //public Action ShareInConversationOtherNavicateCallBack = null;

        /// <summary>
        /// 是否可以进行共享回调
        /// </summary>
        //public Action<Action<bool>> CanBeginSharingCallBack = null;

        /// <summary>
        /// 文件共享回调
        /// </summary>
        public Action<string, fileType> FileShareCallBack = null;

        ///// <summary>
        ///// ppt演示共享回调
        ///// </summary>
        //public Action<string> PPtShareCallBack = null;

        ///// <summary>
        ///// 普通文件共享回调
        ///// </summary>
        //public Action<string> CommonFileShareCallBack = null;

        /// <summary>
        /// 文件推送回调
        /// </summary>
        public Action<string, fileType> SendFileCallBack = null;

        /// <summary>
        /// 展开、收缩事件回调
        /// </summary>
        public Action<bool> ViewChangeCallBack = null;

        #endregion

        #region UI元素映射

        /// <summary>
        /// 切换控件
        /// </summary>
        //protected Button BtnPanelVisibilityControl = null;

        ///// <summary>
        ///// 书架
        ///// </summary>
        //protected UCBookShell Book = null;

        /// <summary>
        /// 等待提示
        /// </summary>
        protected Loading Loading = null;

        /// <summary>
        /// 第一列
        /// </summary>
        //protected ColumnDefinition Column1 = null;

        /// <summary>
        /// 第二列
        /// </summary>
        //protected ColumnDefinition Column2 = null;

        // <summary>
        // 第三列
        // </summary>
        //protected ColumnDefinition Column3 = null;

        /// <summary>
        /// 面板（存储本地应用）
        /// </summary>
        protected System.Windows.Forms.Panel Panel = null;

        /// <summary>
        /// winform控件的宿主
        /// </summary>
        protected WindowsFormsHost Host = null;

        /// <summary>
        /// 装饰
        /// </summary>
        protected Border BorDecorate = null;

        /// <summary>
        /// 视频
        /// </summary>
        protected Border BorContent = null;

        /// <summary>
        /// 遮挡控件
        /// </summary>
        //protected Grid GridOfficePanel = null;

        ///// <summary>
        ///// oneNote笔记模糊词
        ///// </summary>
        //protected string OneNoteFuzzy = "OneNote";

        /// <summary>
        /// 类型图片存储文件夹名称
        /// </summary>
        protected string extetionImageFolderName = "Image/TypeImage";

        #endregion

        #region 控件映射

        /// <summary>
        /// 面包屑根目录
        /// </summary>
        protected BreadLine BreadLineRoot = null;

        /// <summary>
        /// 书架面板
        /// </summary>
        protected Grid GridBook = null;

        /// <summary>
        /// 书架主题面板
        /// </summary>
        protected Grid GridBookParent = null;

        /// <summary>
        /// 左箭头按钮
        /// </summary>
        protected Button BtnArrowLeft = null;

        /// <summary>
        /// 右箭头按钮
        /// </summary>
        protected Button BtnArrowRight = null;

        /// <summary>
        /// 当前目录(自定义)
        /// </summary>
        BreadLine currentBreadLine = null;

        /// <summary>
        /// 资源推送按钮
        /// </summary>
        protected Button BtnResourceSend = null;

        /// <summary>
        /// 资源演示按钮
        /// </summary>
        protected Button BtnResourceShare = null;

        /// <summary>
        /// 资源下载按钮
        /// </summary>
        protected Button BtnResourceDownLoad = null;

        /// <summary>
        /// 资源删除按钮
        /// </summary>
        protected Button BtnResourceDelete = null;

        /// <summary>
        /// 资源移动按钮
        /// </summary>
        protected Button BtnResourceMove = null;

        /// <summary>
        /// 资源重命名按钮
        /// </summary>
        protected Button BtnResourceReName = null;

        /// <summary>
        /// 上传按钮
        /// </summary>
        protected Button BtnResourceUpload = null;

        /// <summary>
        /// 左侧面板
        /// </summary>
        protected Grid GridLeft = null;

        /// <summary>
        /// 收缩、展示按钮
        /// </summary>
        protected Button BtnViewChange = null;

        #endregion

        #region 内部字段

        ///// <summary>
        ///// 打开的某个文件的url地址
        ///// </summary>
        //string fileNavicateUrl = string.Empty;

        ///// <summary>
        ///// 打开本地应用程序的句柄
        ///// </summary>
        //IntPtr intptr = default(IntPtr);

        /// <summary>
        /// 内部浏览器,用于存储验证
        /// </summary>
        System.Windows.Forms.WebBrowser webBrowser = new System.Windows.Forms.WebBrowser();

        /// <summary>
        /// 列表名称
        /// </summary>
        protected string listName = string.Empty;

        /// <summary>
        /// 文件夹显示名称
        /// </summary>
        protected string folderName = string.Empty;

        /// <summary>
        /// 文件操作管理
        /// </summary>
        FileOpenManage fileOpenManage = null;

        #endregion

        #region 绑定属性

        //double disPlayAngle = 90;
        ///// <summary>
        ///// 底部隐藏按钮旋转方向
        ///// </summary>
        //public double DisPlayAngle
        //{
        //    get { return disPlayAngle; }
        //    set
        //    {
        //        if (value != this.disPlayAngle)
        //        {
        //            disPlayAngle = value;
        //            this.OnPropertyChanged("DisPlayAngle");
        //        }
        //    }
        //}
        vy resourcePartOperationVisibility;
        /// <summary>
        /// 部分资源操作显示
        /// </summary>
        public vy ResourcePartOperationVisibility
        {
            get { return resourcePartOperationVisibility; }
            set
            {
                if (value != this.resourcePartOperationVisibility)
                {
                    resourcePartOperationVisibility = value;
                    this.OnPropertyChanged("ResourcePartOperationVisibility");
                }
            }
        }

        #endregion

        #region 一般属性

        WebCredentialManage webCManage = null;
        /// <summary>
        /// web凭据验证管理模型
        /// </summary>
        public WebCredentialManage WebCManage
        {
            get { return webCManage; }
            set { webCManage = value; }
        }

        #endregion

        #region 初始化智存空间

        /// <summary>
        /// 智存空间入口点
        /// </summary>
        public void SpaceBaseMainStart()
        {
            try
            {            
                //智存空间基累数据初始化
                this.SpaceBaseInit();
                //浏览器初始化
                this.WebInit();
                //生成导航
                this.DaoHangInit();
                // 事件注册
                this.EventRegedit();
                //填充数据
                this.SpaceLineInit();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 浏览器初始化

        /// <summary>
        /// 浏览器初始化
        /// </summary>
        private void WebInit()
        {
            try
            {
                this.fileOpenManage = new FileOpenManage(SpaceCodeEnterEntity.WebLoginUserName, SpaceCodeEnterEntity.WebLoginPassword, SpaceCodeEnterEntity.LocalTempRoot, SpaceCodeEnterEntity.LoginUserName, SpaceCodeEnterEntity.UserDomain, false);
                //生成凭据通过智存空间验证
                this.WebCManage = new WebCredentialManage(this.webBrowser, SpaceCodeEnterEntity.WebLoginUserName, SpaceCodeEnterEntity.WebLoginPassword);
                //导航到个人空间
                this.WebCManage.Navicate(SpaceCodeEnterEntity.SpaceWebSiteUri);
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

        #region 智存空间基类数据初始化

        /// <summary>
        /// 智存空间基类数据初始化
        /// </summary>
        private void SpaceBaseInit()
        {
            try
            {
                //创建目录
                if (!Directory.Exists(SpaceCodeEnterEntity.LocalTempRoot))
                {
                    Directory.CreateDirectory(SpaceCodeEnterEntity.LocalTempRoot);
                }

                //设置智存空间当前用户名
                this.CurrentUserName = SpaceCodeEnterEntity.SelfName;
                ////设置根目录名称
                //this.Root1 = this.root1;
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
        /// 面包线初始化
        /// </summary>
        public void SpaceLineInit()
        {
            try
            {
                //等待提示
                this.ShowTip();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    try
                    {
                        //获取会议根目录
                        var mettingList = SpaceCodeEnterEntity.ClientContextManage.GetFolders(listName);
                        //获取相关会议的所有文档
                        var mettingF = mettingList.Where(Item => Item.Name.Equals(folderName)).ToList<SP.Folder>();
                        //跨线程（异步委托）
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                if (mettingF.Count > 0)
                                {
                                    //设置根目录(第一层面包线,文件夹为第一层文件夹【会议文件夹】)
                                    this.BreadLineRoot.Folder = mettingF[0];
                                    //设置当前所处的目录
                                    this.currentBreadLine = this.BreadLineRoot;
                                    //清除之前的那根线【第一个面包线是不需要那根线的】
                                    this.BreadLineRoot.ClearBeforeLine();
                                    //面包线点击事件
                                    this.BreadLineRoot.LineClickEventCallBack = breadLine_LineClickEvent;
                                    //刷新当前页面
                                    this.RefleshSpaceData(this.currentBreadLine);
                                }
                                else
                                {
                                    //刷新当前页面
                                    this.RefleshSpaceData(this.currentBreadLine);
                                    //等待提示
                                    this.HidenTip();
                                }
                            }
                            catch (Exception ex)
                            {
                                LogManage.WriteLog(this.GetType(), ex);
                            };
                        }));
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                });
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        /// <param name="folderList"></param>
        /// <param name="fileList"></param>
        private void DataLoad_All(List<SP.Folder> folderList, List<SP.File> fileList)
        {
            #region 数据加载

            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    //列1的高
                    double row1Height = 30;
                    //文件高
                    double fileHeigth = 200;
                    //文件宽
                    double fileWidth = 100;
                    //生成文件夹
                    this.DataLoad_Directory(folderList, row1Height, fileHeigth, fileWidth);
                    //生成文件
                    this.DataLoad_File(fileList, row1Height, fileHeigth, fileWidth);
                    //整体书架刷新
                    this.ReFlush_BookShell();
                    //等待提示
                    this.HidenTip();
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                };
            }));


            #endregion
        }

        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="folderList">文件夹目录</param>
        /// <param name="row1Height">行1高</param>
        /// <param name="fileHeigth">文件高</param>
        /// <param name="fileWidth">文件宽</param>
        private void DataLoad_File(List<SP.File> fileList, double row1Height, double fileHeigth, double fileWidth)
        {
            try
            {
                //生成文件（根据文件类型去匹配）
                foreach (var item in fileList)
                {
                    if (item.Name.Contains("."))
                    {
                        //文件类型
                        var extention = System.IO.Path.GetExtension(item.ServerRelativeUrl);
                        //文件名称
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(item.ServerRelativeUrl);
                        //去掉点
                        extention = extention.Replace(".", string.Empty);
                        //文件类型
                        fileType fileType = default(fileType);
                        //转换为枚举
                        Enum.TryParse(extention, true, out fileType);
                        //图片
                        string imageUri = "/" + this.extetionImageFolderName + "/" + extention + ".png";

                        //文件具体地址
                        string uri = SpaceCodeEnterEntity.SPSiteAddressFront + item.ServerRelativeUrl;

                        //生成书
                        UCBook ucBook = new UCBook(fileName, imageUri) { BookType = BookType.File, Uri = uri, FileType = fileType, Width = fileWidth, Height = fileHeigth, Row1Height = row1Height };
                        //添加书
                        this.Items_Add(ucBook);
                        //删除文件
                        ucBook.FileDeleteCallBack = ucBook_DeleteFile;
                        //下载文件
                        ucBook.FileDownLoadCallBack = ucBook_DownLoadFile;
                        //推送文件
                        ucBook.FileSendCallBack = ucBook_SendFile;
                        //实时共享
                        ucBook.FileShareCallBack = ucBook_RealShareFile;
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

        /// <summary>
        /// 生成文件夹
        /// </summary>
        /// <param name="folderList">文件夹目录</param>
        /// <param name="row1Height">行1高</param>
        /// <param name="fileHeigth">文件高</param>
        /// <param name="fileWidth">文件宽</param>
        private void DataLoad_Directory(List<SP.Folder> folderList, double row1Height, double fileHeigth, double fileWidth)
        {
            try
            {
                //生成书本形式的文件夹
                foreach (var folderItem in folderList)
                {
                    UCBook ucBook = new UCBook(folderItem.Name, "/Image/TypeImage/folder.png") { Date = DateTime.Now.ToShortTimeString(), BookType = BookType.Folder, Width = fileWidth, Height = fileHeigth, Row1Height = row1Height };
                    this.Items_Add(ucBook);

                    //创建一个面包线（目前在这里创建和根面包线【书架自带】）
                    BreadLine breadLine = new BreadLine() { Folder = folderItem, Title = folderItem.Name };
                    //关联指定生成的面包线（）
                    ucBook.BreadLine = breadLine;
                    //删除文件
                    ucBook.FileDeleteCallBack = ucBook_DeleteFile;
                    //面包线点击事件
                    breadLine.LineClickEventCallBack = breadLine_LineClickEvent;
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

        #region 事件注册

        /// <summary>
        /// 事件注册
        /// </summary>
        private void EventRegedit()
        {
            try
            {
                //尺寸更改事件
                //this.SizeChanged += MainWindow_SizeChanged;

                //UI加载回调
                this.fileOpenManage.LoadUICallBack = this.LoadUICallBack;
                //文件上传
                this.UploadFileEventCallBack = book_UploadFileEvent;
                //打开文件事件
                this.OpenFileEventCallBack = book_OpenFileEvent;
                //新建文件夹
                this.FolderCreateEventCallBack = book_FolderCreateEvent;

                #region 书架区域

                //资源推送
                this.BtnResourceSend.Click += btnResourceSend_Click;
                //资源演示
                this.BtnResourceShare.Click += btnResourceDisplay_Click;
                //资源下载
                this.BtnResourceDownLoad.Click += btnResourceDownLoad_Click;
                //资源删除
                this.BtnResourceDelete.Click += btnResourceDelete_Click;
                //资源移动
                this.BtnResourceMove.Click += btnResourceMove_Click;
                //资源重命名
                this.BtnResourceReName.Click += btnResourceReName_Click;
                //文件上传
                this.BtnResourceUpload.Click += BtnResourceUpload_Click;
                //收缩、展示按钮
                this.BtnViewChange.Click += BtnViewChange_Click;

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

        #region 返回加载视图

        /// <summary>
        /// 返回加载视图
        /// </summary>
        /// <param name="element">返回的元素</param>
        public void LoadUICallBack(FrameworkElement element)
        {
            try
            {
                //隐藏装饰UI
                this.BorDecorate.Visibility = vy.Collapsed;
                //显示视频UI
                this.BorContent.Visibility = vy.Visible;
                //加载视频元素
                this.BorContent.Child = element;
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

        #region 数据刷新

        /// <summary>
        /// 数据刷新
        /// </summary>
        /// <param name="folder"></param>
        public void RefleshSpaceData(BreadLine bread_Line)
        {
            try
            {
                //面包线是否为null
                if (bread_Line == null)
                {
                    return;
                }
                if (bread_Line.Folder != null)
                {
                    //等待提示
                    this.ShowTip();

                    TimerJob.StartRun(new Action(() =>
                        {
                            //清除书架
                            this.Items_Clear();
                            //获取文件夹
                            SP.Folder folder = bread_Line.Folder;

                            ThreadPool.QueueUserWorkItem((o) =>
                       {
                           try
                           {
                               //加载当前会议文件夹
                               SpaceCodeEnterEntity.ClientContextManage.LoadMethod(folder.Folders);
                               //获取当会议文件夹
                               List<SP.Folder> folderList = folder.Folders.ToList<SP.Folder>();

                               //加载当前所有文件
                               SpaceCodeEnterEntity.ClientContextManage.LoadMethod(folder.Files);
                               //获取当前所有文件
                               List<SP.File> fileList = folder.Files.ToList<SP.File>();

                               DataLoad_All(folderList, fileList);
                           }
                           catch (Exception ex)
                           {
                               LogManage.WriteLog(this.GetType(), ex);
                           }

                       });

                        }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 保证嵌入的应用程序View填充

        ///// <summary>
        ///// 保证嵌入的应用程序View填充
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //public void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (intptr != null)
        //        {
        //            Win32API.MoveWindow(intptr, 0, 0, this.Panel.Width, this.Panel.Height, false);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(this.GetType(), ex);
        //    }

        //}

        #endregion

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.WebCManage.SessionClear();
                //清空数据
                this.Panel.Controls.Clear();
                //睡眠300
                Thread.Sleep(300);
                //SharePoint数据清除
                ClientContextManage.ClientContext.Dispose();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 显示隐藏等待提示

        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowTip()
        {
            try
            {
                if (this.Loading.Visibility == vy.Collapsed)
                {
                    //等待提示
                    this.Loading.Visibility = vy.Visible;
                }
                if (this.BreadLineRoot.IsEnabled)
                {
                    this.BreadLineRoot.IsEnabled = false;
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
        /// 隐藏提示
        /// </summary>
        public void HidenTip()
        {
            try
            {
                if (this.Loading.Visibility == vy.Visible)
                {
                    //等待提示
                    this.Loading.Visibility = vy.Collapsed;
                }
                if (!this.BreadLineRoot.IsEnabled)
                {
                    this.BreadLineRoot.IsEnabled = true;
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

        #region 展開、收縮

        /// <summary>
        /// 收缩、展示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BtnViewChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ViewChange();
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
        /// 展開、收縮
        /// </summary>
        public void ViewChange()
        {
            try
            {
                int intColumn = Grid.GetColumn(this.GridLeft);
                bool isExpander = true;
                if (intColumn == 0)
                {
                    Grid.SetColumn(this.GridLeft, 1);
                    this.BtnViewChange.Content = "展开";
                    this.ResourcePartOperationVisibility = vy.Visible;              
                    isExpander = true;
                }
                else
                {
                    Grid.SetColumn(this.GridLeft, 0);
                    this.BtnViewChange.Content = "收缩";
                    this.ResourcePartOperationVisibility = vy.Collapsed;                 
                    isExpander = false;
                }
                if (this.ViewChangeCallBack != null)
                {
                    this.ViewChangeCallBack(isExpander);
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
