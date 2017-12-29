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

using fileType = ConferenceCommon.WPFControl.FileType;

namespace Conference_Space
{
    public partial class SpaceBase : UserControlBase
    {
        #region 实时共享

        /// <summary>
        /// 实时共享
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="fileType"></param>
        void ucBook_RealShareFile(string uri, fileType fileType)
        {
            try
            {               
                if (ShareInConversationSelfNavicateCallBack != null)
                {
                    this.ShareInConversationSelfNavicateCallBack(new Action<bool>((canSharing) =>
        {
            if (canSharing)
            {
                //获取文件名称（包含扩展名称）
                var fileName = System.IO.Path.GetFileName(uri);

                ////获取文件名称（不包含扩展名称）
                //var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(uri);

                //if (fileType == fileType.one)
                //{
                //    fileNameWithoutExtension = this.OneNoteFuzzy;
                //}
                //本地地址
                var loaclF = SpaceCodeEnterEntity.LocalTempRoot + "\\" + fileName;

                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();
                webClientManage.FileDown(uri, loaclF, SpaceCodeEnterEntity.LoginUserName, SpaceCodeEnterEntity.WebLoginPassword, SpaceCodeEnterEntity.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    if (IsSuccessed)
                    {
                        if(this.FileShareCallBack!= null)
                        {
                            this.FileShareCallBack(loaclF, fileType);
                        }                       
                    }
                }));
            }
            else
            {
                MessageBox.Show("共享之前请先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }));
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
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
                    webClientManage.FileDown(uri, saveFileDialog.FileName, SpaceCodeEnterEntity.LoginUserName, SpaceCodeEnterEntity.WebLoginPassword, SpaceCodeEnterEntity.UserDomain, new Action<int>((intProcess) =>
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
                        this.fileOpenManage.FileOpenByExtension((ConferenceCommon.WPFControl.FileType)ucBook.FileType, ucBook.Uri);
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
                    this.RefleshSpaceData(this.currentBreadLine);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


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
                this.RefleshSpaceData(this.currentBreadLine);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 文件上传

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    SpaceCodeEnterEntity.ClientContextManage.UploadFileToFolder(this.currentBreadLine.Folder, new Action(() =>
                    {
                        this.ShowTip();

                    }), new Action(() =>
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //填充数据(刷新)
                            this.RefleshSpaceData(this.currentBreadLine);
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

                    inputMessageWindow.OkEventCallBack = new Action<string>((folderName) =>
                    {
                        //等待提示
                        this.ShowTip();
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            SpaceCodeEnterEntity.ClientContextManage.CreateFolder(this.currentBreadLine.Folder, folderName);

                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //填充数据(刷新)
                                this.RefleshSpaceData(this.currentBreadLine);
                            }));
                        });
                    });
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
        /// 文件推送
        /// </summary>
        /// <param name="uri">文件地址</param>
        /// <param name="fileType">文件类型</param>
        public void ucBook_SendFile(string uri, fileType fileType)
        {
            try
            {
                //使用前先判断
                if (SpaceCodeEnterEntity.ConferenceName != null)
                {
                    if (this.SendFileCallBack != null)
                    {
                        this.SendFileCallBack(uri, fileType);
                    }
                    //填充word服务缓存数据
                    //ModelManage.ConferenceWordAsync.FillConferenceOfficeServiceData(SpaceCodeEnterEntity.ConferenceName, SpaceCodeEnterEntity.SelfName, uri, (ConferenceModel.ConferenceSpaceAsyncWebservice.FileType)fileType, new Action<bool>((isSuccessed) =>
                    //{

                    //}));
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
                            //等待提示
                            this.ShowTip();
                            //获取要删除的文件名称
                            var fileN = System.IO.Path.GetFileName(ucbook.Uri);
                            //this.BorContent.Child = null;
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                //删除文件
                                SpaceCodeEnterEntity.ClientContextManage.DeleFile(this.currentBreadLine.Folder, fileN);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //填充数据(刷新)
                                    this.RefleshSpaceData(this.currentBreadLine);

                                }));
                            });
                        }

                        break;
                    case BookType.Folder:
                        if (this.currentBreadLine != null && this.currentBreadLine.Folder != null)
                        {
                            //等待提示
                            this.ShowTip();
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                //删除文件夹
                                SpaceCodeEnterEntity.ClientContextManage.DeleteFolder(this.currentBreadLine.Folder, ucbook.Book_Title);
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //填充数据(刷新)
                                    this.RefleshSpaceData(this.currentBreadLine);
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

    }
}
