using Conference.Control;
using Conference.Page;
using ConferenceCommon.TimerHelper;
using Conference.View.Resource;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.OfficeHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.SharePointHelper;
using ConferenceCommon.WebHelper;
using ConferenceModel;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.Sharing;
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
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using Conference.Common;
using ConferenceCommon.WPFHelper;

namespace Conference.View.Space
{
    public class SpaceBase : UserControlBase
    {
        #region 自定义委托事件

        public delegate void UploadFileEventHandle();
        /// <summary>
        /// 上传文件事件
        /// </summary>
        public event UploadFileEventHandle UploadFileEvent = null;

        public delegate void OpenFileEventHandle(UCBook ucBook);
        /// <summary>
        /// 打开文件事件
        /// </summary>
        public event OpenFileEventHandle OpenFileEvent = null;

        public delegate void FolderCreateEventHandle();
        /// <summary>
        /// 新建文件夹
        /// </summary>
        public event FolderCreateEventHandle FolderCreateEvent = null;


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

        /// <summary>
        /// oneNote笔记模糊词
        /// </summary>
        protected string OneNoteFuzzy = "OneNote";

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
        /// 书架
        /// </summary>
        protected Grid GridBook = null;

        /// <summary>
        /// 书架主题
        /// </summary>
        protected Grid GridBookParent = null;

        /// <summary>
        /// 左箭头
        /// </summary>
        protected Button BtnArrowLeft = null;

        /// <summary>
        /// 右箭头
        /// </summary>
        protected Button BtnArrowRight = null;

        /// <summary>
        /// 当前目录(自定义)
        /// </summary>
        BreadLine currentBreadLine = null;

        /// <summary>
        /// 资源推送
        /// </summary>
        protected Button BtnResourceSend = null;

        /// <summary>
        /// 资源演示
        /// </summary>
        protected Button BtnResourceShare = null;

        /// <summary>
        /// 资源下载
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
        /// 资源重命名
        /// </summary>
        protected Button BtnResourceReName = null;

        protected Button BtnResourceUpload = null;

        #endregion

        #region 内部字段

        /// <summary>
        /// 打开的某个文件的url地址
        /// </summary>
        string fileNavicateUrl = string.Empty;

        /// <summary>
        /// 打开本地应用程序的句柄
        /// </summary>
        IntPtr intptr;

        /// <summary>
        /// 内部浏览器,用于存储验证
        /// </summary>
        System.Windows.Forms.WebBrowser webBrowser = new System.Windows.Forms.WebBrowser();

        /// <summary>
        /// 视频播放器
        /// </summary>
        MediaPlayerView mediaPlayerView = null;

        /// <summary>
        /// 浏览器
        /// </summary>
        WebView webView = null;

        /// <summary>
        /// 图片编辑器
        /// </summary>
        PictureView pictureView = null;

        /// <summary>
        /// 列表名称
        /// </summary>
        protected string listName = string.Empty;

        /// <summary>
        /// 文件夹名称
        /// </summary>
        protected string folderName = string.Empty;

        ///// <summary>
        ///// 根目录名称
        ///// </summary>
        //protected string root1 = string.Empty;

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


        #endregion

        #region 绑定属性

        double disPlayAngle = 90;
        /// <summary>
        /// 底部隐藏按钮旋转方向
        /// </summary>
        public double DisPlayAngle
        {
            get { return disPlayAngle; }
            set
            {
                if (value != this.disPlayAngle)
                {
                    disPlayAngle = value;
                    this.OnPropertyChanged("DisPlayAngle");
                }
            }
        }

        #endregion

        #region 静态字段

        /// <summary>
        /// 在启用智存空间是否已清除office本地应用
        /// </summary>
        public static bool isOfficeClear = false;

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
                //未清除的话进行清除
                if (!SpaceBase.isOfficeClear)
                {
                    //清除所有office文件
                    this.OfficeDocumentsClear();
                    //已清除
                    SpaceBase.isOfficeClear = true;
                }

                //设置当前上下文
                this.DataContext = this;

                //生成凭据通过智存空间验证
                this.WebCManage = new WebCredentialManage(this.webBrowser, Constant.WebLoginUserName, Constant.WebLoginPassword);
                //导航到个人空间
                this.WebCManage.Navicate(Constant.SpaceWebSiteUri);

                this.SizeChanged += MainWindow_SizeChanged;

                //启动隐藏/显示底部工具栏
                //this.BtnPanelVisibilityControl.Click += btnPanelVisibilityControl_Click;

                //文件上传
                this.UploadFileEvent += book_UploadFileEvent;
                //打开文件事件
                this.OpenFileEvent += book_OpenFileEvent;
                //新建文件夹
                this.FolderCreateEvent += book_FolderCreateEvent;

                //填充数据
                this.FillMeetingSpace();
                //设置会议空间当前用户名
                this.CurrentUserName = Constant.SelfName;
                //设置会议空间当前会议名称
                this.Root1 = this.root1;

                #region 书架区域

                //生成导航
                this.DaoHangInit();
                //绑定当前上下文
                this.DataContext = this;
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

                #endregion

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 清除所有office文件

        /// <summary>
        /// 清除所有office文件
        /// </summary>
        public void OfficeDocumentsClear()
        {
            try
            {
                ////移除Excel文件
                //Process[] pp = Process.GetProcessesByName("Excel");

                ////清除ppt文件
                //Process[] pp1 = Process.GetProcessesByName("POWERPNT");

                ////清除word文件
                //Process[] pp3 = Process.GetProcessesByName("WINWORD");

                //if (pp.Count() > 0 || pp1.Count() > 0 || pp3.Count() > 0)
                //{

                //    var result = MessageBox.Show("为了不影响使用，打开会议空间之前系统会将所有Excel文件,ppt文件，word文件关闭,请做好保存", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    if (result == MessageBoxResult.OK)
                //    {
                //        foreach (var item in pp)
                //        {
                //            item.Kill();
                //        }

                //        foreach (var item in pp1)
                //        {
                //            item.Kill();
                //        }

                //        foreach (var item in pp3)
                //        {
                //            item.Kill();
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 填充数据

        /// <summary>
        /// 填充数据
        /// </summary>
        public void FillMeetingSpace()
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
                        var mettingList = Constant.clientContextManage.GetFolders(listName);

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
                                    this.BreadLineRoot.LineClickEvent += breadLine_LineClickEvent;

                                    //刷新当前页面
                                    this.RefleshView(this.currentBreadLine);
                                }
                                else
                                {
                                    //刷新当前页面
                                    this.RefleshView(this.currentBreadLine);
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
        /// 刷新当前页面
        /// </summary>
        /// <param name="folder"></param>
        public void RefleshView(BreadLine bread_Line)
        {
            try
            {
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
                               Constant.clientContextManage.LoadMethod(folder.Folders);
                               //获取当会议文件夹
                               List<SP.Folder> folderList = folder.Folders.ToList<SP.Folder>();

                               //加载当前所有文件
                               Constant.clientContextManage.LoadMethod(folder.Files);
                               //获取当前所有文件
                               List<SP.File> fileList = folder.Files.ToList<SP.File>();

                               #region 数据加载

                               this.Dispatcher.BeginInvoke(new Action(() =>
                               {
                                   try
                                   {

                                       //偏移
                                       //Thickness margin = new Thickness(0, 0, 0, 8);
                                       //列1的高
                                       double row1Height = 30;
                                       //文件高
                                       double fileHeigth = 200;
                                       //文件宽
                                       double fileWidth = 100;

                                       //, Margin = margin
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
                                           ucBook.DeleteFile += ucBook_DeleteFile;

                                           //面包线点击事件
                                           breadLine.LineClickEvent += breadLine_LineClickEvent;
                                       }



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
                                               FileType fileType = default(FileType);
                                               //转换为枚举
                                               Enum.TryParse(extention, true, out fileType);
                                               //图片
                                               string imageUri = "/" + this.extetionImageFolderName + "/" + extention + ".png";

                                               //文件具体地址
                                               string uri = Constant.SPSiteAddressFront + item.ServerRelativeUrl;

                                               switch (fileType)
                                               {
                                                   case FileType.Jpg:
                                                       //imageUri = uri;
                                                       break;
                                                   case FileType.Png:
                                                       //imageUri = uri;
                                                       break;
                                                   case FileType.Ico:
                                                       //imageUri = uri;
                                                       break;

                                                   default:
                                                       break;
                                               }
                                               //,  Margin = margin
                                               //生成书
                                               UCBook ucBook = new UCBook(fileName, imageUri) { BookType = BookType.File, Uri = uri, FileType = fileType, Width = fileWidth, Height = fileHeigth, Row1Height = row1Height };
                                               //添加书
                                               this.Items_Add(ucBook);
                                               //删除文件
                                               ucBook.DeleteFile += ucBook_DeleteFile;
                                               //下载文件
                                               ucBook.DownLoadFile += ucBook_DownLoadFile;
                                               //共享文件
                                               ucBook.ShareFile += ucBook_ShareFile;
                                               //实时共享
                                               ucBook.RealShareFile += ucBook_RealShareFile;
                                           }
                                       }
                                       this.ReFlush();
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

        /// <summary>
        /// 保证嵌入的应用程序View填充
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                if (intptr != null)
                {
                    Win32API.MoveWindow(intptr, 0, 0, this.Panel.Width, this.Panel.Height, false);
                }
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
                this.Host.Visibility = System.Windows.Visibility.Collapsed;
                //隐藏装饰UI
                this.BorDecorate.Visibility = System.Windows.Visibility.Collapsed;
                //隐藏offcie遮挡面板
                //this.GridOfficePanel.Visibility = System.Windows.Visibility.Collapsed;
                //显示视频UI
                this.BorContent.Visibility = System.Windows.Visibility.Visible;
                //加载视频元素
                this.BorContent.Child = element;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #region 根据句柄加载本地应用程序

        /// <summary>
        /// 根据句柄加载本地应用程序
        /// </summary>
        /// <param name="handle"></param>
        public void book_OpenFileCompleate(IntPtr handle)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        //隐藏视频UI
                        this.BorContent.Visibility = System.Windows.Visibility.Collapsed;
                        //隐藏装饰UI
                        this.BorDecorate.Visibility = System.Windows.Visibility.Collapsed;
                        //显示offcie遮挡面板
                        //this.GridOfficePanel.Visibility = System.Windows.Visibility.Visible;
                        //显示本地officeUI
                        this.Host.Visibility = System.Windows.Visibility.Visible;
                        //置空视频UI
                        this.BorContent.Child = null;

                        //临时存储句柄
                        intptr = handle;

                        TimerJob.StartRun(new Action(() =>
                            {
                                //本地office嵌入
                                AppContainer.SetParent(handle, Panel.Handle);
                                //设置合适尺寸
                                Win32API.MoveWindow(handle, 0, 0, this.Panel.Width, this.Panel.Height, false);
                            }));
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
        }

        #endregion

        #region 打开文件（包括文件夹）

        /// <summary>
        /// 打开文件（包括文件夹）
        /// </summary>
        /// <param name="ucBook"></param>
        public void book_OpenFileEvent(UCBook ucBook)
        {
            try
            {
                switch (ucBook.BookType)
                {
                    case BookType.File:
                        //根据文件的类型使用相应的方式打开
                        this.FileOpenByExtension(ucBook.FileType, ucBook.Uri);
                        break;
                    case BookType.Folder:
                        //打开文件夹
                        this.FolderOpen(ucBook);
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

        /// <summary>
        /// 打开文件夹
        /// </summary>
        public void FolderOpen(UCBook ucbook)
        {
            try
            {
                if (this.currentBreadLine != null && ucbook.BreadLine != null)
                {
                    //设置当前面包线的子节点（子面包线）
                    this.currentBreadLine.BreadLineChild = ucbook.BreadLine;

                    //设置当前面包线
                    this.currentBreadLine = ucbook.BreadLine;

                    //刷新当前页面
                    this.RefleshView(this.currentBreadLine);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 根据文件的类型使用相应的方式打开
        /// </summary>
        public void FileOpenByExtension(FileType fileType, string uri)
        {
            try
            {
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
                    this.webView = new WebView(uri);
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
                    this.webView = new WebView(uri);
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
        /// 打开office文件
        /// </summary>
        /// <param name="uri"></param>
        public void OpenOfficeFile(string uri, FileType filetype)
        {
            try
            {
                IntPtr handle = default(IntPtr);

                switch (filetype)
                {
                    case FileType.docx:
                        excelManage.ClearDocuments();
                        pPTManage.ClearDocuments();

                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            try
                            {
                                //使用本地word打开文件
                                handle = wordManage.OpenWord(uri);
                                //根据句柄加载本地应用程序
                                this.book_OpenFileCompleate(handle);
                            }
                            catch (Exception ex)
                            {
                                LogManage.WriteLog(this.GetType(), ex);
                            };
                        });

                        break;
                    case FileType.xlsx:
                        wordManage.ClearDocuments();
                        pPTManage.ClearDocuments();
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            try
                            {
                                //使用本地Ecel打开文件
                                handle = excelManage.OpenExcel(uri);
                                //根据句柄加载本地应用程序
                                this.book_OpenFileCompleate(handle);
                            }
                            catch (Exception ex)
                            {
                                LogManage.WriteLog(this.GetType(), ex);
                            };
                        });

                        break;

                    case FileType.PPtx:
                        wordManage.ClearDocuments();
                        excelManage.ClearDocuments();
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            try
                            {
                                //使用本地Ecel打开文件
                                handle = pPTManage.OpenPPT(uri);
                                //根据句柄加载本地应用程序
                                this.book_OpenFileCompleate(handle);
                            }
                            catch (Exception ex)
                            {
                                LogManage.WriteLog(this.GetType(), ex);
                            };
                        });

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

        #endregion

        #region 面包线点击

        /// <summary>
        /// 根面包线点击事件
        /// </summary>
        /// <param name="breadLine"></param>
        void breadLine_LineClickEvent(BreadLine breadLine)
        {
            try
            {
                //清空面包线之后的子项
                breadLine.BreadLineChild = null;
                //设置当前的目录
                this.currentBreadLine = breadLine;
                //刷新当前页面
                this.RefleshView(this.currentBreadLine);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 文件上传

        void BtnResourceUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.book_UploadFileEvent();
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
        /// 文件上传
        /// </summary>
        /// <param name="fileName"></param>
        public void book_UploadFileEvent()
        {
            try
            {
                if (this.currentBreadLine != null && this.currentBreadLine.Folder != null)
                {

                    //上传文件
                    Constant.clientContextManage.UploadFileToFolder(this.currentBreadLine.Folder, new Action(() =>
                    {
                        this.ShowTip();

                    }), new Action(() =>
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //填充数据(刷新)
                            this.RefleshView(this.currentBreadLine);
                        }));
                        }));
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
        void book_FolderCreateEvent()
        {
            try
            {
                if (this.currentBreadLine != null && this.currentBreadLine.Folder != null)
                {
                    //文本输入窗体
                    InputMessageWindow inputMessageWindow = new InputMessageWindow();

                    inputMessageWindow.OkEvent += (folderName) =>
                    {
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            Constant.clientContextManage.CreateFolder(this.currentBreadLine.Folder, folderName);

                            this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //填充数据(刷新)
                                    this.RefleshView(this.currentBreadLine);
                                }));
                        });
                    };
                    inputMessageWindow.Show();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 协同共享

        /// <summary>
        /// 共享文件
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileType"></param>
        public void ucBook_ShareFile(string uri, FileType fileType)
        {
            try
            {
                //使用前先判断
                if (Constant.ConferenceName != null)
                {
                    ////根据文件的类型使用相应的方式打开
                    //this.FileOpenByExtension(fileType, uri);

                    //填充word服务缓存数据
                    ModelManage.ConferenceWordAsync.FillConferenceOfficeServiceData(Constant.ConferenceName, Constant.SelfName, uri, (ConferenceModel.ConferenceSpaceAsyncWebservice.FileType)fileType, new Action<bool>((isSuccessed) =>
                    {

                    }));
                }
                else
                {
                    MessageBox.Show("共享之前先进入一个会议", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
        /// <param name="uri"></param>
        /// <param name="fileType"></param>
        void ucBook_RealShareFile(string uri, FileType fileType)
        {
            try
            {
                if (fileType == FileType.PPtx || fileType == FileType.PPt || fileType == FileType.doc || fileType == FileType.docx || fileType == FileType.one)
                {
                    MainWindow.MainPageInstance.ConversationM.FileOpenByExtension(fileType, uri);
                }
                //使用前先判断
                if (LyncHelper.MainConversation != null)
                {
                    //强制跳转到会话管理页面
                    //强制导航到资源共享
                    MainWindow.MainPageInstance.ForceToNavicate(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                    MainWindow.MainPageInstance.ConversationM.PageIndex = ResourceType.Share;

                    //获取文件名称（包含扩展名称）
                    var fileName = System.IO.Path.GetFileName(uri);

                    //获取文件名称（不包含扩展名称）
                    var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(uri);

                    if (fileType == FileType.one)
                    {
                        fileNameWithoutExtension = this.OneNoteFuzzy;
                    }
                    //本地地址
                    var loaclF = Constant.LocalTempRoot + "\\" + fileName;

                    //创建一个下载管理实例
                    WebClientManage webClientManage = new WebClientManage();
                    webClientManage.FileDown(uri, loaclF, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                    {

                    }), new Action<Exception, bool>((ex, IsSuccessed) =>
                    {
                        if (IsSuccessed)
                        {
                            if (fileType == FileType.PPtx || fileType == FileType.PPt)
                            {
                                //封装的会话窗体内部实例
                                uc.ConversationWindowClass conversationWindowClass = LyncHelper.MainConversation.InnerObject as uc.ConversationWindowClass;

                                //开启ppt
                                conversationWindowClass.GetConversationWindowActions().AddOfficePowerPointToConversation(loaclF, Constant.TreeServiceIP + Constant.ServicePPTTempFile + fileName);
                            }
                            else
                            {
                                //通过进程打开一个文件
                                ProcessManage.OpenFileByLocalAddressReturnHandel(loaclF, new Action<int, IntPtr>((processID, intptr) =>
                                {
                                    //获取共享模型
                                    var shareModality = ((ApplicationSharingModality)(LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing]));

                                    //遍历可以共享的资源
                                    foreach (var item in shareModality.ShareableResources)
                                    {
                                        //&& intptr != new IntPtr(0)
                                        //指定要共享的程序名称
                                        if (item.Id.Equals(processID))
                                        {
                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                        else if (item.Id == intptr.ToInt32())
                                        {
                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                        else if (item.Name.Contains(fileNameWithoutExtension))
                                        {

                                            //判断是否可以进行共享该程序
                                            if (shareModality.CanShare(item.Type))
                                            {
                                                this.ShareAndSync(intptr, shareModality, item);
                                                break;
                                            }
                                        }
                                    }
                                }));
                            }
                        }
                    }));

                    ////根据文件的类型使用相应的方式打开
                    //this.FileOpenByExtension(fileType, uri);

                    ////显示1列（左侧工作区）
                    //this.Column2Display();


                }
                else
                {
                    MessageBox.Show("共享之前请先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        public void ShareAndSync(IntPtr intptr, ApplicationSharingModality shareModality, SharingResource item)
        {
            try
            {
                //共享程序置顶
                Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                //开始共享该程序
                shareModality.BeginShareResources(item, null, null);
                //同步页面
                Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
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

        #region 下载文件

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        public void ucBook_DownLoadFile(string uri)
        {
            try
            {
                //存储文件对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;

                //设置默认文件名（可以不设置）
                saveFileDialog.FileName = System.IO.Path.GetFileName(uri);

                if (saveFileDialog.ShowDialog() == true)
                {

                    //创建一个下载管理实例
                    WebClientManage webClientManage = new WebClientManage();
                    webClientManage.FileDown(uri, saveFileDialog.FileName, Constant.LoginUserName, Constant.WebLoginPassword, Constant.UserDomain, new Action<int>((intProcess) =>
                        {

                        }), new Action<Exception, bool>((ex, IsSuccessed) =>
                        {

                        }));
                }

                //webClientManage.FileDown(fileName)
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 文件删除

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="uri"></param>
        public void ucBook_DeleteFile(UCBook ucbook)
        {
            try
            {
                switch (ucbook.BookType)
                {
                    case BookType.File:

                        if (this.currentBreadLine != null && this.currentBreadLine.Folder != null)
                        {
                            //获取要删除的文件名称
                            var fileN = System.IO.Path.GetFileName(ucbook.Uri);
                            //this.BorContent.Child = null;
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                //删除文件
                                Constant.clientContextManage.DeleFile(this.currentBreadLine.Folder, fileN);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {                                     
                                    //填充数据(刷新)
                                    this.RefleshView(this.currentBreadLine);

                                }));
                            });
                        }

                        break;
                    case BookType.Folder:
                        if (this.currentBreadLine != null && this.currentBreadLine.Folder != null)
                        {
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                //删除文件夹
                                Constant.clientContextManage.DeleteFolder(this.currentBreadLine.Folder, ucbook.Book_Title);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //填充数据(刷新)
                                    this.RefleshView(this.currentBreadLine);
                                }));
                            });
                        }

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
                //excel数据清除
                excelManage.Dispose();
                //ppt数据清除
                pPTManage.Dispose();
                //word数据清除
                wordManage.Dispose();
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

        public void ShowTip()
        {
            try
            {
                if (this.Loading.Visibility == System.Windows.Visibility.Collapsed)
                {
                    //等待提示
                    this.Loading.Visibility = System.Windows.Visibility.Visible;
                }
                if(this.BreadLineRoot.IsEnabled)
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

        public void HidenTip()
        {
            try
            {
                if (this.Loading.Visibility == System.Windows.Visibility.Visible)
                {
                    //等待提示
                    this.Loading.Visibility = System.Windows.Visibility.Collapsed;
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

        #region 书架进行时区域

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
                    this.ReFlush(GetListStart(value), this.BookTempList);
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
                    this.ReFlush(this.GetListStart(this.PageNow), this.BookTempList);
                    this.OnPropertyChanged("PageCount");
                }
            }
        }

        string serchText = string.Empty;
        /// <summary>
        /// 查询的字符
        /// </summary>
        public string SerchText
        {
            get { return serchText; }
            set
            {
                if (value != this.serchText)
                {
                    serchText = value;
                    this.OnPropertyChanged("SerchText");
                }
            }
        }

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
        /// 当前会议名称
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
        /// 添加一本书
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
                this.ReFlush();
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
                this.ReFlush();
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
                this.ReFlush();
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
                    this.SelectedUCbook.SelectedVisibility = System.Windows.Visibility.Collapsed;
                }
                this.SelectedUCbook = sender as UCBook;
                this.SelectedUCbook.SelectedVisibility = System.Windows.Visibility.Visible;
                //打开文件或文件夹事件
                if (this.OpenFileEvent != null)
                {
                    this.OpenFileEvent(this.SelectedUCbook);
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
        public void ReFlush(int RowPosition, List<UCBook> bookListSelf)
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
        public void ReFlush()
        {
            try
            {
                this.ReFlush(GetListStart(this.PageNow), this.BookTempList);

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
                    item.ArrowVisibility = System.Windows.Visibility.Visible;
                    //启动箭头跳动动画
                    item.RunArrowSkipAnimation();
                }
                else
                {
                    item.ArrowVisibility = System.Windows.Visibility.Collapsed;
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
                this.UploadFile();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        public void UploadFile()
        {
            try
            {
                if (this.UploadFileEvent != null)
                {
                    this.UploadFileEvent();
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
                this.FolderCreate();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        public void FolderCreate()
        {
            try
            {
                if (this.FolderCreateEvent != null)
                {
                    this.FolderCreateEvent();
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
                    this.SelectedUCbook.menuItemShare();
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

        void btnResourceReName_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 资源移动

        void btnResourceMove_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        #region 资源删除

        void btnResourceDelete_Click(object sender, RoutedEventArgs e)
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

        #endregion
    }
}

#region old solution

#region 移动分割

///// <summary>
///// 移动分割
///// </summary>
///// <param name="sender"></param>
///// <param name="e"></param>
//public void btnPanelVisibilityControl_Click(object sender, RoutedEventArgs e)
//{
//    try
//    {
//        if (this.Column1.ActualWidth == 0)
//        {
//            //两列平分
//            this.ColumnAllDisplay();
//        }
//        else
//        {
//            // 显示1列（左侧工作区）
//            this.Column2Display();
//        }
//    }
//    catch (Exception ex)
//    {
//        LogManage.WriteLog(this.GetType(), ex);
//    }
//}

#region 两列平分

///// <summary>
///// 两列平分
///// </summary>
//public void ColumnAllDisplay()
//{
//    try
//    {
//        //平方两列
//        this.Column1.Width = new GridLength((this.ActualWidth - Column2.ActualWidth) / 2);
//        //显示书本
//        this.gridBook.Visibility = System.Windows.Visibility.Visible;
//        //隐藏
//        //this.borerChange.Visibility = System.Windows.Visibility.Collapsed;
//        //设置标示
//        this.DisPlayAngle = 90;
//        //清除签入的本地应用
//        this.Panel.Controls.Clear();
//        //设置
//        //AppContainer.SetParent(this.intptr, panel.Handle);
//        Win32API.MoveWindow(intptr, 0, 0, this.Panel.Width, this.Panel.Height, false);
//    }
//    catch (Exception ex)
//    {
//        LogManage.WriteLog(this.GetType(), ex);
//    }
//}

#endregion

#region 显示1列（左侧工作区）

///// <summary>
///// 显示1列（左侧工作区）
///// </summary>
//public void Column2Display()
//{
//    try
//    {
//        //设置为1列
//        this.Column1.Width = new GridLength(0);
//        //隐藏书本
//        this.gridBook.Visibility = System.Windows.Visibility.Collapsed;
//        //隐藏
//        //this.borerChange.Visibility = System.Windows.Visibility.Visible;
//        //设置标示
//        this.DisPlayAngle = 270;
//        //设置
//        //AppContainer.SetParent(this.intptr, panel.Handle);
//        //Win32API.MoveWindow(intptr, 0, 0, (int)(this.Column1.ActualWidth + this.Column3.ActualWidth), this.Panel.Height, false);
//    }
//    catch (Exception ex)
//    {
//        LogManage.WriteLog(this.GetType(), ex);
//    }
//}

#endregion

#endregion

#endregion
