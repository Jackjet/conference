using ConferenceCommon.TimerHelper;
using ConferenceCommon.LogHelper;
using ConferenceModel;
using ConferenceModel.ConferenceTreeWebService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Xml.Serialization;
using SP = Microsoft.SharePoint.Client;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.OfficeHelper;
using System.Threading;
using ConferenceCommon.SharePointHelper;
using ConferenceCommon.WPFHelper;
using System.Windows.Threading;
using ConferenceCommon.FileHelper;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using form = System.Windows.Forms;
using ConferenceCommon.WPFControl;
using wpfHelperFileType = ConferenceCommon.WPFControl.FileType;
using ConferenceCommon.JsonHelper;
using ConferenceCommon.EntityHelper;
using vy = System.Windows.Visibility;

namespace Conference_Tree
{
    public partial class ConferenceTreeView : UserControlBase
    {
        #region 保存（xml文件保存,研讨语音，树研讨）

        /// <summary>
        /// 保存（xml文件保存,研讨语音，树研讨）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnXMLSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.XMLSaved();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 保存（xml文件保存,研讨语音，树研讨）
        /// </summary>
        private void XMLSaved()
        {
            try
            {

                ModelManage.ConferenceTree.GetAll(TreeCodeEnterEntity.ConferenceName, new Action<ConferenceTreeInitRefleshEntity>((result) =>
                {
                    //研讨树综述窗体
                    SummarizeWindow window = new SummarizeWindow();

                    #region old solution

                    //设置窗体显示位置
                    //var left = (MainWindow.DisCuss_View.ActualWidth - MainWindow.DisCuss_View.Column1Width - window.Width) / 2;
                    //var top = 110 + 55 + (MainWindow.DisCuss_View.ActualHeight - 55 - window.Height) / 2;
                    //window.Left = left + 70 + 10 + MainWindow.DisCuss_View.Column1Width;
                    //window.Top = top;

                    #endregion

                    //获取会议根目录
                    var mettingList = TreeCodeEnterEntity.ClientContextManage.GetFolders(TreeCodeEnterEntity.MeetingFolderName);

                    //获取相关会议的所有文档
                    var mettingF = mettingList.Where(Item => Item.Name.Equals(TreeCodeEnterEntity.ConferenceName)).ToList<SP.Folder>();

                    window.IsOkEvent += (string message) =>
                    {

                        #region 上传会议知识树节点数据

                        //声明序列化对象实例serializer 
                        XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));

                        //研讨树序列化并进行存储（存储到SharePoint的服务器里）
                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Position = 0;
                            result.Summarize = message;
                            //序列化
                            serializer.Serialize(ms, result);

                            byte[] bytes = ms.ToArray();

                            if (mettingF.Count > 0)
                            {
                                TreeCodeEnterEntity.ClientContextManage.UploadFileToFolder(mettingF[0], TreeCodeEnterEntity.ConferenceName + ".xml", bytes);
                            }
                        }

                        #endregion

                        #region 上传会议知识树图片

                        //研讨树图片序列化并进行存储（存储到SharePoint的服务器里）
                        using (MemoryStream ms = new MemoryStream())
                        {
                            RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.borDiscussTheme.ActualWidth, (int)this.borDiscussTheme.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                            bmp.Render(this.borDiscussTheme);
                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                            encoder.Save(ms);

                            ms.Position = 0;
                            byte[] bytes = new byte[ms.Length];
                            ms.Read(bytes, 0, bytes.Length);

                            if (mettingF.Count > 0)
                            {
                                TreeCodeEnterEntity.ClientContextManage.UploadFileToFolder(mettingF[0], TreeCodeEnterEntity.ConferenceName + ".png", bytes);
                                window.Close();
                            }
                        }

                        #endregion

                        #region 上传会议视频链接

                        //全名称
                        var fullName = this.GetRecordFileFullName(TreeCodeEnterEntity.RecordFolderName, TreeCodeEnterEntity.RecordExtention);
                        //当前名称                                 
                        //判断是否存在该文件
                        if (File.Exists(fullName))
                        {
                            #region old solution

                            ////使用内存流
                            //using (MemoryStream ms = new MemoryStream())
                            //{
                            //    ms.Position = 0;
                            //    //将文件写入流【utf8】
                            //    StreamWriter sw = new StreamWriter(ms,Encoding.UTF8);

                            //    sw.Write(fullName);

                            //    byte[] bytes = ms.ToArray();

                            #endregion

                            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(fullName);

                            if (mettingF.Count > 0)
                            {
                                TreeCodeEnterEntity.ClientContextManage.UploadFileToFolder(mettingF[0], TreeCodeEnterEntity.ReacordUploadFileName, bytes);
                            }
                        }
                        //}
                        #endregion
                    };
                    window.Show();
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 获取录制视频文件
        /// </summary>
        /// <param name="recordVideoSavedPlace">录播视频存储地址</param>
        /// <param name="recordVideoExtention">录播视频格式</param>
        /// <returns></returns>
        public string GetRecordFileFullName(string recordVideoSavedPlace, string recordVideoExtention)
        {
            string fullName = string.Empty;
            try
            {
                //判断录播的路径
                if (Directory.Exists(recordVideoSavedPlace))
                {
                    //操作文件夹
                    DirectoryInfo directoryInfo = new DirectoryInfo(recordVideoSavedPlace);
                    //获取该文件夹下的所有文件夹
                    DirectoryInfo[] directoryInfoes = directoryInfo.GetDirectories();

                    DirectoryInfo directoryInfoRecently = null;

                    //判断创建时间
                    double longerCreatimer = 0;

                    //遍历文件夹获取时间最近时间创建的文件夹
                    foreach (var item in directoryInfoes)
                    {
                        if (longerCreatimer == 0)
                        {
                            longerCreatimer = (DateTime.Now - item.CreationTime).TotalSeconds;
                            directoryInfoRecently = item;
                        }
                        else if (item.CreationTime.Second < longerCreatimer)
                        {
                            longerCreatimer = (DateTime.Now - item.CreationTime).TotalSeconds;
                            directoryInfoRecently = item;
                        }
                    }

                    //获取到的视频文件
                    FileInfo fileInfo = null;

                    //获取到最近时间创建的文件夹
                    if (directoryInfoRecently != null)
                    {
                        var mp4File = directoryInfoRecently.GetFiles(recordVideoExtention);
                        if (mp4File.Length > 0)
                        {
                            fileInfo = mp4File[0];
                            //获取录制视频的全名称
                            fullName = fileInfo.FullName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

            return fullName;
        }

        #endregion

        #region 知识树显示

        /// <summary>
        /// 知识树显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnProTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //隐藏图表窗体
                this.borChart.Visibility = System.Windows.Visibility.Hidden;
                //设置知识树按钮激活颜色
                this.btnProTree.Background = new SolidColorBrush(Colors.LightGreen);
                //恢复图表按钮颜色
                this.btnChart.Background = new SolidColorBrush(Colors.White);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 视图尺寸更改（定制化【放大、还原、缩小】）

        #region 放大

        /// <summary>
        /// 树视图放大
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnEnLarge_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图放大
                this.EnLarge();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图放大
        /// </summary>
        public void EnLarge()
        {
            try
            {
                //按比例增加视图宽度
                this.ViewBoxWidth += 100 * this.constratParameterAboutWidth;
                // //按比例增加视图高度
                this.ViewBoxHeigth += 100 * this.constratParameterAboutHeight;
                //视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = this.ViewBoxWidth / this.borDiscussTheme.ActualWidth;
                //视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = this.ViewBoxHeigth / this.borDiscussTheme.ActualHeight;
                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #region 百分比显示

        /// <summary>
        /// 百分比显示
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public string PersentDisplay(double count)
        {
            string aa = string.Empty;
            try
            {
                double m = count * 100;
                aa = m.ToString("0") + "%";

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return aa;
        }

        #endregion

        #endregion

        #region 还原

        /// <summary>
        /// 树视图还原
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图还原
                this.Reduction();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图还原
        /// </summary>
        public void Reduction()
        {
            try
            {
                //还原视图宽度
                this.ViewBoxWidth = this.borDiscussTheme.ActualWidth;
                //还原视图高度
                this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight;
                //还原视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = 1;
                //还原视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = 1;

                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 缩小

        /// <summary>
        /// 树视图缩小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReduce_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //树视图缩小
                this.Reduce();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 树视图缩小
        /// </summary>
        public void Reduce()
        {
            try
            {
                //按比例减少视图宽度
                this.ViewBoxWidth -= 100 * this.constratParameterAboutWidth;
                //按比例减少视图高度
                this.ViewBoxHeigth -= 100 * this.constratParameterAboutHeight;

                //视图缩放的宽度倍数
                this.changedConstratParameterAboutWidth = this.ViewBoxWidth / this.borDiscussTheme.ActualWidth;
                //视图缩放的高度倍数
                this.changedConstratParameterAboutHeight = this.ViewBoxHeigth / this.borDiscussTheme.ActualHeight;
                //百分比显示
                this.txtPercent.Text = this.PersentDisplay(this.changedConstratParameterAboutWidth);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        /// <summary>
        /// 更改视图比例
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbTreeDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        double widthChange = ((double)contentInt / 100);

                        double heightChange = ((double)contentInt / 100);

                        //按比例减少视图宽度
                        this.ViewBoxWidth = this.borDiscussTheme.ActualWidth * widthChange;
                        //按比例减少视图高度
                        this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight * heightChange;

                        //视图缩放的宽度倍数
                        this.changedConstratParameterAboutWidth = widthChange;
                        //视图缩放的高度倍数
                        this.changedConstratParameterAboutHeight = heightChange;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 静态事件

        /// <summary>
        /// 注册静态事件（全局只进行一次）
        /// </summary>
        public void StaticEventRegedit()
        {
             try
            {
                if (!ConferenceTreeView.StaticEventRegeditIsRegedit)
                {
                    //投票更改事件
                    //ConferenceTreeItem.VoteChangedEvent += ConferenceTree_VoteChangedEvent;
                    //投票的相关节点添加事件
                    //ConferenceTreeItem.VoteTreeItemAddEvent += ConferenceTreeItem_VoteTreeItemAddEvent;
                    //投票的相关节点删除事件
                    //ConferenceTreeItem.VoteTreeItemRemoveEvent += ConferenceTreeItem_VoteTreeItemRemoveEvent;
                    //更新整棵研讨树完成事件
                    ConferenceTreeItem.RefleshCompleateEvent = ConferenceTreeItem_RefleshCompleateEvent;
                    //知识树里的静态事件标示为已注册
                    ConferenceTreeView.StaticEventRegeditIsRegedit = true;
                    //子项选择回调事件
                    ConferenceTreeItem.SelectedItemCallBack = SelectedItemCallBack;
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

        #region 知识树更新完成事件

        /// <summary>
        /// 知识树更新完成事件
        /// </summary>
        void ConferenceTreeItem_RefleshCompleateEvent()
        {
            try
            {
                if (this.ConferenceChart_View != null)
                {
                    this.ConferenceChart_View.ClearAllPoints();
                    /////填充图表
                    //foreach (var item in ConferenceTreeItem2.AcademicReviewItemList)
                    //{
                    //this.ConferenceChart_View.ChartItemsAdd(item.ACA_Tittle, item.ACA_YesVoteCount, item.ACA_NoVoteCount, item.ACA_Guid);
                    //}
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 研讨树节点搜索

        /// <summary>
        /// 研讨树节点搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //获取搜索文本
                this.SearchText = (sender as TextBox).Text;
                //搜索信息若为空，则清除阴影
                if (string.IsNullOrEmpty(this.SearchText))
                {
                    //清除阴影
                    foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                    {
                        if (item.BorVisibility == System.Windows.Visibility.Visible)
                        {
                            item.BorVisibility = System.Windows.Visibility.Hidden;
                        }
                    }
                }
                //记录所有知识树的记录（确保有记录）
                else if (ConferenceTreeItem.AcademicReviewItemList.Count > 0)
                {
                    //遍历记录所有知识树的记录
                    foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                    {
                        //标题不能为空（包含该关键字分为3块【1、标题 2、评论 3、简介】）
                        if ((!string.IsNullOrEmpty(item.ACA_Tittle) && item.ACA_Tittle.Contains(this.SearchText)) || (!string.IsNullOrEmpty(item.ACA_Comment) && item.ACA_Comment.Contains(this.SearchText)))
                        {
                            item.BorVisibility = System.Windows.Visibility.Visible;
                        }
                        else
                        {
                            item.BorVisibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }


        #endregion

        #region 初始化视图容器

        /// <summary>
        /// 初始化视图容器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void viewBox_Load(object sender, RoutedEventArgs e)
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

        #region 树容器尺寸更改

        /// <summary>
        /// 树容器尺寸更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void borDiscussTheme_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                //按比例设置视图的宽度
                this.ViewBoxWidth = this.borDiscussTheme.ActualWidth * this.changedConstratParameterAboutWidth;
                //按比例了设置视图的高度
                this.ViewBoxHeigth = this.borDiscussTheme.ActualHeight * this.changedConstratParameterAboutHeight;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 备注

        /// <summary>
        /// 备注
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNote_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //显示备注控制
                    ConferenceTreeItem.currentConferenceTreeItem.CommentCommandVisibility = System.Windows.Visibility.Visible;
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

        #region 链接

        /// <summary>
        /// 链接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    if (TreeCodeEnterEntity.mettingF.Count > 0)
                    {
                        //获取文件夹
                        SP.Folder folder = TreeCodeEnterEntity.mettingF[0];
                        //上传文件
                        TreeCodeEnterEntity.ClientContextManage.UploadFileToFolder(folder, new Action(() =>
                        {
                            if (ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility == System.Windows.Visibility.Collapsed)
                            {
                                ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility = System.Windows.Visibility.Visible;
                            }
                        }), new Action<SP.File>((uploadFile) =>
                        {
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {

                                //通知服务器更改链接
                                ConferenceTreeItem.currentConferenceTreeItem.LinkListItemToService(TreeCodeEnterEntity.SPSiteAddressFront + uploadFile.ServerRelativeUrl);
                                ConferenceTreeItem.currentConferenceTreeItem.UploadFileTipVisibility = System.Windows.Visibility.Collapsed;
                            }));
                        }));
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

        #region 删除节点

        /// <summary>
        /// 删除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //删除节点
                    ConferenceTreeItem.currentConferenceTreeItem.menuItem_Delete();
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

        #region 生成子层

        /// <summary>
        /// 生成子层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnChildlevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //添加子节点
                    ConferenceTreeItem.currentConferenceTreeItem.ItemAdd();
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

        #region 生成平行层

        /// <summary>
        /// 生成平行层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSamelevel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ConferenceTreeItem.currentConferenceTreeItem != null)
                {
                    //添加同一级节点
                    ConferenceTreeItem.currentConferenceTreeItem.ParentItemAdd();
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

        #region 打开文件

        /// <summary>
        /// 根据文件的类型使用相应的方式打开
        /// </summary>
        public void FileOpenByExtension(wpfHelperFileType fileType, string uri)
        {
            try
            {
                //保存当前打开的文件地址
                TreeCodeEnterEntity.currentFileUri = uri;
                //设置当前文件类型
                TreeCodeEnterEntity.currentFileType = fileType;

                if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.OfficeFile)
                {
                    if (this.officeFile == null)
                    {
                        this.officeFile = new OfficeFile();
                    }
                    this.borLeftMain.Child = this.officeFile;
                }
                //根据文件的类型使用相应的方式打开
                TreeCodeEnterEntity.fileOpenManage.FileOpenByExtension(fileType, uri);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 元素替换

        /// <summary>
        /// 判断是否可执行元素替换
        /// </summary>
        /// <param name="beforeConferenceTreeItem"></param>
        /// <param name="nowConferenceTreeItem"></param>
        /// <returns></returns>
        public bool CheckCanInsteadElement(ConferenceTreeItem beforeConferenceTreeItem, ConferenceTreeItem nowConferenceTreeItem)
        {
            bool result = true;
            try
            {
                if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                {

                    //查找拖动目标的子集（不允许父级拖到子级的动作）
                    List<ConferenceTreeItem> beforeItemList = WPFElementManage.FindVisualChildren<ConferenceTreeItem>(beforeConferenceTreeItem).ToList<ConferenceTreeItem>();
                    //不允许父级拖到子级的动作（返回）
                    if (beforeItemList != null && beforeItemList.Contains(nowConferenceTreeItem))
                    {
                        result = false;
                    }
                    //不为父子级关系
                    if (beforeConferenceTreeItem.ACA_Parent == null || beforeConferenceTreeItem.ACA_Parent.Equals(nowConferenceTreeItem))
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            return result;
        }

        /// <summary>
        /// 元素替换法
        /// </summary>
        public void InsteadElement(ConferenceTreeItem beforeConferenceTreeItem, ConferenceTreeItem nowConferenceTreeItem)
        {
            try
            {
                if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                {
                    bool result = this.CheckCanInsteadElement(beforeConferenceTreeItem, nowConferenceTreeItem);
                    if (result)
                    {
                        //获取目标节点的父节点
                        ConferenceTreeItem beforeConferenceTreeItemParent = beforeConferenceTreeItem.ACA_Parent;
                        if (beforeConferenceTreeItemParent.ACA_ChildList.Contains(beforeConferenceTreeItem))
                        {
                            //移除该子节点   
                            beforeConferenceTreeItemParent.ACA_ChildList.Remove(beforeConferenceTreeItem);
                        }
                        if (beforeConferenceTreeItemParent.StackPanel.Children.Contains(beforeConferenceTreeItem))
                        {
                            //UI显示
                            beforeConferenceTreeItemParent.StackPanel.Children.Remove(beforeConferenceTreeItem);
                        }
                        if (!nowConferenceTreeItem.ACA_ChildList.Contains(beforeConferenceTreeItem))
                        {
                            //拖拽到选中子节点添加节点
                            nowConferenceTreeItem.ACA_ChildList.Add(beforeConferenceTreeItem);
                        }
                        if (!nowConferenceTreeItem.StackPanel.Children.Contains(beforeConferenceTreeItem))
                        {
                            //UI显示
                            nowConferenceTreeItem.StackPanel.Children.Add(beforeConferenceTreeItem);
                        }
                        //父级更替
                        beforeConferenceTreeItem.ACA_Parent = nowConferenceTreeItem;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 元素替换法（服务器同步使用）
        /// </summary>
        public void InsteadElement(int beforeConferenceTreeItemGuid, int nowConferenceTreeItemGuid)
        {
            try
            {
                ///获取相对应的元素
                ConferenceTreeItem beforeConferenceTreeItem = null;
                ConferenceTreeItem nowConferenceTreeItem = null;

                //通过遍历获取相对应的子节点（判断GUID）
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {
                    if (beforeConferenceTreeItem != null && nowConferenceTreeItem != null)
                    {
                        break;
                    }
                    else if (item.ACA_Guid.Equals(beforeConferenceTreeItemGuid))
                    {
                        beforeConferenceTreeItem = item;
                    }
                    else if (item.ACA_Guid.Equals(nowConferenceTreeItemGuid))
                    {
                        nowConferenceTreeItem = item;
                    }
                }
                this.InsteadElement(beforeConferenceTreeItem, nowConferenceTreeItem);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 子节点扩展隐藏公共方法

        /// <summary>
        /// 子节点扩展隐藏公共方法
        /// </summary>
        /// <param name="item"></param>
        public static void ExpanderMethod()
        {
            try
            {
                foreach (var item in ConferenceTreeItem.AcademicReviewItemList)
                {                   
                    //完毕敲键盘
                    if (!item.txtTittle.IsKeyboardFocused)
                    {
                        //编辑完毕
                        item.IsTittleEditNow = false;
                    }

                    //完毕敲键盘
                    if (!item.txtComment.IsKeyboardFocused)
                    {
                        //编辑完毕
                        item.IsCommentEditNow = false;
                    }

                    if (item.ACA_ChildList.Count == 0)
                    {
                        item.ExpanderVisibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        item.ExpanderVisibility = System.Windows.Visibility.Visible;
                    }
                    if (item.ACA_ChildList.Count < 2)
                    {
                        item.VerticalLineVisibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        if (item.ExpanderVisibility == Visibility.Visible)
                        {
                            item.VerticalLineVisibility = System.Windows.Visibility.Visible;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceTreeView), ex);
            }
        }

        #endregion

        #region 子项选择进行搜索

        /// <summary>
        /// 子项选择进行搜索
        /// </summary>
        /// <param name="title">子项标题</param>
        private void SelectedItemCallBack(string title)
        {
            try
            {

                if (conferenceTreeView.searchFileView != null)
                {
                    if (!string.IsNullOrEmpty(title))
                    {
                        if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.SearchFile)
                        {
                            conferenceTreeView.searchFileView.LoadingVisibility = vy.Visible;
                            ModelManage.ConferenceSpSearch.SearchFiles(TreeCodeEnterEntity.SpaceWebSiteUri, TreeCodeEnterEntity.SpaceWebSiteUri + "/" + TreeCodeEnterEntity.MeetingFolderName, title, new Action<string>((message) =>
                                {
                                    if (!string.IsNullOrEmpty(message))
                                    {
                                        conferenceTreeView.searchFileView.Display(message);
                                        Clipboard.SetText(title);
                                        conferenceTreeView.searchFileView.LoadingVisibility = vy.Collapsed; ;
                                    }
                                }));
                        }
                    }
                    else
                    {
                        conferenceTreeView.searchFileView.datagrid.ItemsSource = null;
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
    }
}
