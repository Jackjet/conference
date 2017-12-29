using ConferenceCommon.FileHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.WPFControl;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using ConferenceModel.ConferenceTreeWebService;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using vy = System.Windows.Visibility;

namespace Conference_Tree
{
    /// <summary>
    /// MeetSummary.xaml 的交互逻辑
    /// </summary>
    public partial class MeetSummary : TreeView_ContentBase
    {        
        #region 字段

		// <summary>
        /// 标题参照物
        /// </summary>
        string displayNumber = "1";

        /// <summary>
        /// 标题介绍
        /// </summary>
        string title_introduce = "_会议纪要";

        #endregion

        #region 绑定属性

        vy uploadVisibility = vy.Hidden;
        /// <summary>
        /// 上传进行时显示
        /// </summary>
        public vy UploadVisibility
        {
            get { return uploadVisibility; }
            set
            {
                if (this.uploadVisibility != value)
                {
                    uploadVisibility = value;
                    this.OnPropertyChanged("UploadVisibility");
                }
            }
        }

        vy uploadFlgVisibility = vy.Collapsed;
        /// <summary>
        /// 上传完成提示
        /// </summary>
        public vy UploadFlgVisibility
        {
            get { return uploadFlgVisibility; }
            set
            {
                if (this.uploadFlgVisibility != value)
                {
                    uploadFlgVisibility = value;
                    this.OnPropertyChanged("UploadFlgVisibility");
                }
            }
        }

        vy summerUpdateVisibility = vy.Hidden;
        /// <summary>
        /// 会议纪要更改提示
        /// </summary>
        public vy SummerUpdateVisibility
        {
            get { return summerUpdateVisibility; }
            set
            {
                if (this.summerUpdateVisibility != value)
                {
                    summerUpdateVisibility = value;
                    this.OnPropertyChanged("SummerUpdateVisibility");
                }
            }
        }

        vy conferenceCommentCommandVisibility = vy.Collapsed;
        /// <summary>
        /// 会议纪要控制显示
        /// </summary>
        public vy ConferenceCommentCommandVisibility
        {
            get { return conferenceCommentCommandVisibility; }
            set
            {
                if (this.conferenceCommentCommandVisibility != value)
                {
                    conferenceCommentCommandVisibility = value;
                    this.OnPropertyChanged("ConferenceCommentCommandVisibility");
                }
            }
        }

        vy collapsedVisibility = vy.Collapsed;
        /// <summary>
        /// 收縮按鈕顯示
        /// </summary>
        public vy CollapsedVisibility
        {
            get { return collapsedVisibility; }
            set
            {
                if (this.collapsedVisibility != value)
                {
                    collapsedVisibility = value;
                    this.OnPropertyChanged("CollapsedVisibility");
                }
            }
        }

        vy expanderVisibility = vy.Visible;
        /// <summary>
        /// 展开按鈕顯示
        /// </summary>
        public vy ExpanderVisibility
        {
            get { return expanderVisibility; }
            set
            {
                if (this.expanderVisibility != value)
                {
                    expanderVisibility = value;
                    this.OnPropertyChanged("ExpanderVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public MeetSummary()
        {
            try
            {
                InitializeComponent();
                //加载文件UI
                TreeCodeEnterEntity.fileOpenManage.LoadUICallBack += this.LoadUICallBack;

                this.EventRegedit();
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

        #region 事件注册区域

        /// <summary>
        /// 事件注册区域
        /// </summary>
        public void EventRegedit()
        {
            try
            {
                //生成pdf版会议纪要
                this.btnCreatePDF.Click += btnCreatePDF_Click;
                //生成word版会议纪要
                this.btnCreateWord.Click += btnCreateWord_Click;
                //上传会议纪要
                this.btnUpload.Click += btnUpload_Click;
                //刷新会议纪要
                this.btnReflesh.Click += btnReflesh_Click;
                //收缩
                this.btnViewChange.Click += btnViewChange_Click;
                //保存到本地
                this.btnLoaclSaved.Click += btnLoaclSaved_Click;
                //导入xml文件（知识树）
                this.btnLocalImport.Click += btnLocalImport_Click;
                //视图查看
                this.cmbFileSizeChanged.SelectionChanged +=cmbFileSizeChanged_SelectionChanged;
                //展开按钮
                this.btnViewChange_Expander.Click += btnViewChange_Click;
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

        #region 生成word版会议纪要

        /// <summary>
        /// 生成word版会议纪要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCreateWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = TreeCodeEnterEntity.ConferenceName + this.title_introduce + ".doc";
                FileManage.CreateWord(fileName, TreeCodeEnterEntity.webView.WebBrowser, "conferenceComment");
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

        #region 生成pdf版会议纪要

        /// <summary>
        /// 生成pdf版会议纪要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCreatePDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {              
                if (File.Exists(TreeCodeEnterEntity.PdfTransferAppName))
                {
                    string fileTitle = TreeCodeEnterEntity.ConferenceName + this.title_introduce;
                    string fileNameWord = fileTitle + ".html";
                    string FileFullName = TreeCodeEnterEntity.FileRoot + "\\" + fileNameWord;

                    FileManage.CreateWPFile_Web(FileFullName, TreeCodeEnterEntity.webView.WebBrowser, "conferenceComment");

                    string fileNamePDF = fileTitle + ".PDF";
                    FileManage.CreatePDF(FileFullName, fileNamePDF, TreeCodeEnterEntity.PdfTransferAppName);
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

        #region 将会议纪要上传到服务器

        /// <summary>
        /// 将会议纪要上传到服务器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //显示正在上传
                this.UploadVisibility = vy.Visible;
                MemoryStream ms = null;
                using (ms = new MemoryStream())
                {
                    ms.Position = 0;
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                    {
                        System.Windows.Forms.HtmlElement element = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("conferenceComment");
                        sw.Write(element.OuterHtml);
                    }
                }
                //个人文件夹
                List<SP.Folder> mettingF = TreeCodeEnterEntity.mettingF;
                if (mettingF.Count > 0 && ms != null)
                {
                    //获取文件夹
                    SP.Folder folder = mettingF[0];
                    //上传文件
                    TreeCodeEnterEntity.ClientContextManage.UploadFileToFolder(folder, TreeCodeEnterEntity.ConferenceName + "_会议纪要" + ".doc", ms.GetBuffer(), new Action<bool>((successed) =>
                    {
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.UploadVisibility = vy.Hidden;
                            //显示并在短时间内关闭
                            VisibilityManage.SetShowThanHidenVisibility(this.txtUpload, 1500);
                        }));
                    }));
                }
            }
            catch (Exception ex)
            {
                this.UploadVisibility = vy.Hidden;
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 生成会议纪要

        /// <summary>
        /// 生成会议纪要
        /// </summary>       
        internal void MeetSummary_DisPlay()
        {
            try
            {
                //显示生成会议纪要
                this.ConferenceCommentCommandVisibility = vy.Visible;

                this.BuildMeetSummary();
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
        /// 使用递归的方式填充会议纪要的内容
        /// </summary>
        /// <param name="treeParent"></param>
        /// <param name="number"></param>
        /// <param name="builder"></param>
        void SetNextLineAboutMeetSummary(ConferenceTreeItem treeParent, string number, ref StringBuilder builder)
        {
            try
            {

                if (treeParent.ACA_ChildList.Count > 0)
                {
                    int count = 0;
                    foreach (var item in treeParent.ACA_ChildList)
                    {
                        count++;
                        if (treeParent.ACA_Parent == null)
                        {
                            displayNumber = Convert.ToString(count);
                        }
                        else
                        {
                            displayNumber = number + "." + count;
                        }

                        this.SetLineValue(item, ref builder);

                        this.SetNextLineAboutMeetSummary(item, displayNumber, ref builder);
                    }

                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 填充一行内容
        /// </summary>
        /// <param name="item"></param>
        /// <param name="builder"></param>
        void SetLineValue(ConferenceTreeItem item, ref StringBuilder builder)
        {
            try
            {
                string pNormal = @"<p class=MsoNormal align=left style='text-align:left'><b><span lang=EN-US
  style='font-size:12pt'>" + (displayNumber) + "、" + item.ACA_Tittle + "</span></span></b></p>";
                builder.Append(pNormal);
                if (!string.IsNullOrEmpty(item.ACA_Comment))
                {
                    string pNormal2 = @"<p  align=left style='text-align:left'><b><span lang=EN-US
  style='font-size:12pt'>" + "   备注：" + item.ACA_Comment + "</span></span></b></p>";
                    builder.Append(pNormal2);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 构建会议纪要
        /// </summary>
        public void BuildMeetSummary()
        {
            try
            {
                //获取打开文件管理
                FileOpenManage fileOpenManage = TreeCodeEnterEntity.fileOpenManage;

                //通过浏览器的方式打开
                TreeCodeEnterEntity.webView = fileOpenManage.OpenFileByBrowser(TreeCodeEnterEntity.FileRoot + "/" + TreeCodeEnterEntity.ConferenceCommentHtmlTemp);

                TimerJob.StartRun(new Action(() =>
                {
                    if (TreeCodeEnterEntity.webView.WebBrowser.Document != null)
                    {
                        //获取当前的会议信息
                        ConferenceModel.ConferenceInfoWebService.ConferenceInformationEntityPC ConferenceInformationEntityPC = TreeCodeEnterEntity.TempConferenceInformationEntity;
                        //获取会议名称
                        System.Windows.Forms.HtmlElement htmlElement = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetName");
                        htmlElement.InnerHtml = ConferenceInformationEntityPC.MeetingName;
                        //获取开始时间
                        System.Windows.Forms.HtmlElement htmlElement2 = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetTime1");
                        htmlElement2.InnerHtml = ConferenceInformationEntityPC.BeginTime.ToString("yyyy-MM-dd hh:mm");
                        //获取结束时间
                        System.Windows.Forms.HtmlElement htmlElement3 = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetTime2");
                        htmlElement3.InnerHtml = ConferenceInformationEntityPC.EndTime.ToString("yyyy-MM-dd hh:mm");
                        //获取会议室名称
                        System.Windows.Forms.HtmlElement htmlElement4 = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetPlace");
                        htmlElement4.InnerHtml = ConferenceInformationEntityPC.RoomName;
                        //获取主持人
                        System.Windows.Forms.HtmlElement htmlElement5 = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetChair");
                        htmlElement5.InnerHtml = ConferenceInformationEntityPC.ApplyPeople;

                        StringBuilder builder = new StringBuilder();
                        foreach (var item in ConferenceInformationEntityPC.JoinPeopleName)
                        {
                            builder.Append(item + "   ");
                        }
                        //获取所有参会人
                        System.Windows.Forms.HtmlElement htmlElement6 = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("txtMeetPartical");
                        htmlElement6.InnerHtml = builder.ToString();

                        System.Windows.Forms.HtmlElement tdPanel = TreeCodeEnterEntity.webView.WebBrowser.Document.GetElementById("tdPanel");

                        var p1 = tdPanel.InnerHtml;

                        StringBuilder builder2 = new StringBuilder();
                        builder2.Append(p1);
                        string pParent = @"<p class=MsoNormal align=center style='text-align:center'><b><span lang=EN-US
  style='font-size:15pt'>" + ConferenceTreeView.conferenceTreeView.ConferenceTreeItem.ACA_Tittle + "</span></span></b></p>";
                        builder2.Append(pParent);

                        this.SetNextLineAboutMeetSummary(ConferenceTreeView.conferenceTreeView.ConferenceTreeItem, "1", ref builder2);

                        tdPanel.InnerHtml = builder2.ToString();
                    }
                }), 1000);

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 刷新会议纪要

        /// <summary>
        /// 刷新会议纪要
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnReflesh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //隐藏会议纪要更改提示
                this.SummerUpdateVisibility = vy.Hidden;
                //构建会议纪要
                this.BuildMeetSummary();
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
        public void LoadUICallBack(FrameworkElement element)
        {
            try
            {
                if (TreeCodeEnterEntity.Tree_LeftContentType == Common.Tree_LeftContentType.Summary)
                {
                    if (element.Parent is Border)
                    {
                        Border borParent = element.Parent as Border;
                        borParent.Child = null;
                    }
                    //加载视频元素
                    this.borContent.Child = element;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #region 保存到本地

        /// <summary>
        /// 保存到本地
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLoaclSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存到本地
                this.SevedXmlLocal();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 保存到本地
        /// </summary>
        public void SevedXmlLocal()
        {
            try
            {
                //存储文件对话框
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //设置文件类型
                //书写规则例如：txt files(*.txt)|*.txt
                saveFileDialog.Filter = "txt files(*.xml)|*.xml|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                //设置默认文件名（可以不设置）
                saveFileDialog.FileName = TreeCodeEnterEntity.TreeXmlFileName;
                //主设置默认文件extension（可以不设置）
                saveFileDialog.DefaultExt = "xml";
                //获取或设置一个值，该值指示如果用户省略扩展名，文件对话框是否自动在文件名中添加扩展名。（可以不设置）
                saveFileDialog.AddExtension = true;

                ////设置默认文件类型显示顺序（可以不设置）
                //saveFileDialog.FilterIndex = 0;

                //保存对话框是否记忆上次打开的目录
                saveFileDialog.RestoreDirectory = true;
                if (saveFileDialog.ShowDialog() == true)
                {
                    //获取当前服务器所缓存的知识树
                    ModelManage.ConferenceTree.GetAll(TreeCodeEnterEntity.ConferenceName, new Action<ConferenceTreeInitRefleshEntity>((result) =>
                    {
                        //声明序列化对象实例serializer 
                        XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));
                        //研讨树序列化并进行存储（存储到SharePoint的服务器里）
                        using (FileStream ms = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write))
                        {
                            //序列化
                            serializer.Serialize(ms, result);
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

        #region 导入xml文件（知识树）

        /// <summary>
        /// 导入xml文件（知识树）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnLocalImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.XmlImport();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 导入xml文件（知识树）
        /// </summary>
        public void XmlImport()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "txt files(*.xml)|*.xml|txt files(*.txt)|*.txt|All files(*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    //声明序列化对象实例serializer 
                    XmlSerializer serializer = new XmlSerializer(typeof(ConferenceTreeInitRefleshEntity));

                    //生成文件到指定路径
                    using (FileStream ms = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read))
                    {
                        //反序列化获取协议实体
                        var result = serializer.Deserialize(ms);
                        if (result is ConferenceTreeInitRefleshEntity)
                        {
                            //反序列化，并将反序列化结果值赋给变量i
                            ConferenceTreeInitRefleshEntity conferenceTreeInitRefleshEntity = result as ConferenceTreeInitRefleshEntity;
                            //协议实体参会人清空
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.ParticipantList.Clear();
                            //协议实体参会人重新添加为当前会议的参会人
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.ParticipantList.AddRange(TreeCodeEnterEntity.ParticipantList);
                            //清除协议实体操作人
                            conferenceTreeInitRefleshEntity.RootParent_AcademicReviewItemTransferEntity.Operationer = string.Empty;
                            //重新构造服务器中的研讨树
                            ModelManage.ConferenceTree.SetAll(TreeCodeEnterEntity.ConferenceName, conferenceTreeInitRefleshEntity, new Action<bool>((error2) =>
                            {

                            }));
                        }
                        else
                        {
                            MessageBox.Show("文件异常", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
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
    }
}
