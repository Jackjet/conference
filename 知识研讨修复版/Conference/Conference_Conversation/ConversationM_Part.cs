using ConferenceCommon.TimerHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.WindowHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using Microsoft.Win32;
using uc = Microsoft.Office.Uc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.OfficeHelper;
using ConferenceCommon.FileDownAndUp;
using System.IO;
using ConferenceCommon.WPFHelper;
using System.Windows.Forms.Integration;
using ConferenceCommon.WPFControl;
using Conference_Conversation.Common;
using Conference_Conversation.Control;
using ConferenceCommon.RegeditHelper;
using vy = System.Windows.Visibility;


namespace Conference_Conversation
{
    public partial class ConversationM : UserControlBase
    {
        #region 启动视频

        /// <summary>
        /// 启动视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVideo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //启动视频
                    LyncHelper.StartVideo(LyncHelper.MainConversation);
                }
                else
                {
                    MessageBox.Show("开启视频之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 启动音频

        /// <summary>
        /// 启动音频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //启动音频
                    LyncHelper.StartAudio(LyncHelper.MainConversation);
                }
                else
                {
                    MessageBox.Show("开启视频之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 关闭音视频

        /// <summary>
        /// 关闭音视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAVClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //启动音频
                    LyncHelper.Close_AV(LyncHelper.MainConversation);
                }
                else
                {
                    MessageBox.Show("没有可操作的对象", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 电子白板

        /// <summary>
        /// 电子白板
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnWhiteboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //共享前释放资源
                    this.ShareBeforeDisposeResrouce();
                    //共享电子白板
                    LyncHelper.ShareWhiteboard(LyncHelper.MainConversation, ConversationCodeEnterEntity.SelfName,null);
                }
                else
                {
                    MessageBox.Show("使用电子白板共享之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 显示批注

        /// <summary>
        /// 显示批注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPostil_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.CurrentShowType == ShowType.ConversationView)
                {
                    this.SetConversationAreaShow(ShowType.HidenView, false);
                    LyncHelper.HidenWindowContent();
                    bool result = this.SetConversationArea_Conversation2();
                    if (result)
                    {
                        this.CurrentShowType = ShowType.ConversationView;
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

        #endregion

        #region 桌面共享

        /// <summary>
        /// 桌面共享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDeskShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //桌面共享
                LyncHelper.ShareDesk();
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

        #region 手机投影

        /// <summary>
        /// 手机投影
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnprojection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

        #region ppt共享

        /// <summary>
        /// ppt共享
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnPPT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (LyncHelper.MainConversation != null)
                {
                    //打开选项对话框
                    OpenFileDialog dialog = new OpenFileDialog();
                    //指定显示的文件类型
                    dialog.Filter = "PPT文件(*.ppt,*.pptx)|*.ppt;*.pptx;";
                    //设置为多选
                    dialog.Multiselect = false;
                    if (dialog.ShowDialog() == true)
                    {
                        //共享前释放资源
                        this.ShareBeforeDisposeResrouce();
                        //打开ppt共享辅助
                        LyncHelper.PPtShareHelper(dialog.FileName);
                    }
                }
                else
                {
                    MessageBox.Show("共享ppt之前先选择一个会话", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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

        #region 共享本地文件
        
        /// <summary>
        /// 打开本地文件
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="fileNameWithoutExtension"></param>
        public void OpenLocalFileHelper(string fileName)
        {
            //获取文件名称（不包含扩展名称）
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);

            ProcessManage.OpenFileByLocalAddressReturnHandel(fileName, new Action<int, IntPtr>((processID, intptr) =>
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

        /// <summary>
        /// 共享并同步
        /// </summary>
        /// <param name="intptr"></param>
        /// <param name="shareModality"></param>
        /// <param name="item"></param>
        public void ShareAndSync(IntPtr intptr, ApplicationSharingModality shareModality, SharingResource item)
        {
            try
            {
                //共享程序置顶
                Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                //开始共享该程序
                shareModality.BeginShareResources(item, null, null);

                if (this.ShareAndSyncCallBack != null)
                {
                    this.ShareAndSyncCallBack();
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

        #region 共享浏览器

        /// <summary>
        /// 共享浏览器
        /// </summary>
        /// <param name="uri"></param>
        public void ShareWebBrowser(Uri uri)
        {
            try
            {
                RegeditManage.OpenAplicationByRegedit2("IEXPLORE.EXE", uri.OriginalString, new Action<int, IntPtr>((processID, intptr) =>
                {
                    //获取共享模型
                    var shareModality = ((ApplicationSharingModality)(LyncHelper.MainConversation.Conversation.Modalities[ModalityTypes.ApplicationSharing]));

                    //遍历可以共享的资源
                    foreach (var item in shareModality.ShareableResources)
                    {
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
                        //else if (item.Name.Contains(""))
                        //{

                        //    //判断是否可以进行共享该程序
                        //    if (shareModality.CanShare(item.Type))
                        //    {
                        //        共享程序置顶
                        //        Win32API.SetWindowPos(intptr, -1, 615, 110, 0, 0, 1 | 2);

                        //        //开始共享该程序
                        //        shareModality.BeginShareResources(item, null, null);
                        //        //同步页面
                        //        Conference.MainWindow.MainPageInstance.ChairView.SyncPageHelper(ConferenceCommon.EnumHelper.ViewSelectedItemEnum.Resource);
                        //        break;
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

        #region 接任演示

        /// <summary>
        /// 接任演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDemonstration_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var resourceList = this.ShareableContentList;
                if (LyncHelper.MainConversation != null && resourceList != null)
                {
                    foreach (var item in resourceList)
                    {
                        if (item.State == ShareableContentState.Active)
                        {
                            item.Present();
                        }
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

        #endregion

        #region 共享前释放资源

        /// <summary>
        /// 共享前释放资源
        /// </summary>
        public void ShareBeforeDisposeResrouce()
        {
            try
            {
                //int reson = 0;
                //foreach (var item in MainWindow.MainPageInstance.ShareableContentList)
                //{
                //    //状态为连接
                //    if (item.IsActive && item.CanInvoke(ShareableContentAction.StopPresenting, out reson))
                //    {
                //        item.StopPresenting();
                //        break;
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

        #region 推送区域

        /// <summary>
        /// 文件推送
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="uri"></param>
        public void FileOpenByExtension(FileType fileType, string uri)
        {
            try
            {
                if (this.FileOpenManage == null)
                {
                    this.FileOpenManage = new FileOpenManage(ConversationCodeEnterEntity.WebLoginUserName, ConversationCodeEnterEntity.WebLoginPassword, ConversationCodeEnterEntity.LocalTempRoot, ConversationCodeEnterEntity.LoginUserName, ConversationCodeEnterEntity.UserDomain, false);
                    this.FileOpenManage.LoadUICallBack = LoadUICallBack;
                }
                this.FileOpenManage.FileOpenByExtension(fileType, uri);
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
        /// UI加载回调
        /// </summary>
        /// <param name="obj"></param>
        private void LoadUICallBack(FrameworkElement obj)
        {
            try
            {
                //隐藏装饰UI
                this.borDecorate.Visibility = vy.Collapsed;
                //显示视频UI
                this.borContent.Visibility = vy.Visible;
                //加载视频元素
                this.borContent.Child = obj;
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

        #region 修复会话

        /// <summary>
        /// 修复会话
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnConversationRepair_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.RepairConversationCallBack != null)
                {
                    this.RepairConversationCallBack();
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
