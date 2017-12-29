using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.FileHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WPFHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using vy = System.Windows.Visibility;
using wpfHelperFileType = ConferenceCommon.WPFControl.FileType;
using IOOperation = System.IO.Path;

namespace Conference_Tree
{
    public class TreeView_ContentBase : UserControlBase
    {
        #region 预览更改

        /// <summary>
        /// 预览更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmbFileSizeChanged_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = e.AddedItems[0];
                if (item is ComboBoxItem)
                {
                    ComboBoxItem comboBoxItem = item as ComboBoxItem;
                    string content = Convert.ToString(comboBoxItem.Content);
                    content = content.Replace(" %", string.Empty);
                    int contentInt = 0;
                    bool result = int.TryParse(content, out contentInt);
                    if (result)
                    {
                        //double widthChange = ((double)contentInt / 100);

                        //double heightChange = ((double)contentInt / 100);

                        TreeCodeEnterEntity.webView.WebBrowser.Zoom(contentInt);

                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 下载

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="downLoadProgress">下载进度</param>
        protected void DownLoad_File(ProgressBar downLoadProgress)
        {
            try
            {
                if (!string.IsNullOrEmpty(TreeCodeEnterEntity.currentFileUri))
                {
                    //打开文件存储对话框
                    FileManage.OpenDialogThenDoing(TreeCodeEnterEntity.currentFileUri, new Action<string>((fileName) =>
                    {
                        downLoadProgress.Value = 0;
                        downLoadProgress.Visibility = vy.Visible;
                        //创建一个下载管理实例
                        WebClientManage webClientManage = new WebClientManage();
                        webClientManage.FileDown(TreeCodeEnterEntity.currentFileUri, fileName, TreeCodeEnterEntity.LoginUserName, TreeCodeEnterEntity.WebLoginPassword, TreeCodeEnterEntity.UserDomain, new Action<int>((intProcess) =>
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //设置进度
                                downLoadProgress.Value = intProcess;
                            }));

                        }), new Action<Exception, bool>((ex, IsSuccessed) =>
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                //设置进度
                                downLoadProgress.Value = downLoadProgress.Maximum;
                                downLoadProgress.Visibility = vy.Collapsed;
                            }));
                        }));
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

        #region 展開、收縮

        /// <summary>
        /// 展開、收縮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnViewChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConferenceTreeView.conferenceTreeView.ViewChange();
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

        #region 文件共享

        /// <summary>
        /// 文件共享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFileShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = IOOperation.GetFileName(TreeCodeEnterEntity.currentFileUri);
                //本地地址
                var loaclF = TreeCodeEnterEntity.LocalTempRoot + "\\" + fileName;

                //创建一个下载管理实例
                WebClientManage webClientManage = new WebClientManage();
                webClientManage.FileDown(TreeCodeEnterEntity.currentFileUri, loaclF, TreeCodeEnterEntity.LoginUserName, TreeCodeEnterEntity.WebLoginPassword, TreeCodeEnterEntity.UserDomain, new Action<int>((intProcess) =>
                {

                }), new Action<Exception, bool>((ex, IsSuccessed) =>
                {
                    if (IsSuccessed)
                    {
                        if (ConferenceTreeView.conferenceTreeView.FileShareCallBack != null)
                        {
                            ConferenceTreeView.conferenceTreeView.FileShareCallBack(loaclF, TreeCodeEnterEntity.currentFileType);
                        }
                        //if (TreeCodeEnterEntity.currentFileType == wpfHelperFileType.pptx || TreeCodeEnterEntity.currentFileType == wpfHelperFileType.ppt)
                        //{
                        //    if (ConferenceTreeView.conferenceTreeView.PPtShareCallBack != null)
                        //    {
                        //        ConferenceTreeView.conferenceTreeView.PPtShareCallBack(loaclF);
                        //    }
                        //}
                        //else
                        //{
                        //    if (ConferenceTreeView.conferenceTreeView.CommonFileShareCallBack != null)
                        //    {
                        //        ConferenceTreeView.conferenceTreeView.CommonFileShareCallBack(loaclF);
                        //    }
                        //}
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

        #endregion

        #region 文件推送

        /// <summary>
        /// 文件推送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnFileSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeView.conferenceTreeView.SendFileCallBack != null)
                {
                    ConferenceTreeView.conferenceTreeView.SendFileCallBack(TreeCodeEnterEntity.currentFileUri, TreeCodeEnterEntity.currentFileType);
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
