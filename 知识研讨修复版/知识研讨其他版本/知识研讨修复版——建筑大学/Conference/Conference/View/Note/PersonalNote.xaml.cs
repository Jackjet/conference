using Conference.Common;
using ConferenceCommon.LogHelper;
using ConferenceCommon.SharePointHelper;
using ConferenceCommon.WebHelper;
using ConferenceCommon.WPFHelper;
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
using SP = Microsoft.SharePoint.Client;

namespace Conference.View.Note
{
    /// <summary>
    /// PersonalNote.xaml 的交互逻辑
    /// </summary>
    public partial class PersonalNote : UserControlBase
    {
        #region 字段
       
        /// <summary>
        /// 列表名称
        /// </summary>
        protected string listName = Constant.PesonalFolderName;

        /// <summary>
        /// 文件夹名称
        /// </summary>
        protected string folderName = Constant.LoginUserName;

        /// <summary>
        /// 个人笔记名称
        /// </summary>
        string personalNoteName = Constant.SelfName + "个人笔记";

        /// <summary>
        /// 个人笔记文件类型
        /// </summary>
        string personNoteFileExtention = ".One";

        /// <summary>
        /// 通过owa打开的文件uri后缀名
        /// </summary>
        string owaWebExtentionName = "?Web=1";

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
      
        #region 绑定属性

        Visibility loadingVisibility = Visibility.Collapsed;
        /// <summary>
        /// 提示
        /// </summary>
        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set
            {
                if (loadingVisibility != value)
                {
                    loadingVisibility = value;
                    this.OnPropertyChanged("LoadingVisibility");
                }
            }
        }

        Visibility personNoteVisibility = Visibility.Collapsed;
        /// <summary>
        /// 日记加载
        /// </summary>
        public Visibility PersonNoteVisibility
        {
            get { return personNoteVisibility; }
            set
            {
                if (personNoteVisibility != value)
                {
                    personNoteVisibility = value;
                    this.OnPropertyChanged("PersonNoteVisibility");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public PersonalNote()
        {
            try
            {
                //UI加载
                InitializeComponent();

                this.DataContext = this;

              

                //加载个人笔记
                this.AddInPersonalNote();
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

        #region 加载个人笔记

        /// <summary>
        /// 加载个人笔记
        /// </summary>
        public void AddInPersonalNote()
        {
            try
            {
                this.LoadingVisibility = System.Windows.Visibility.Visible;
                ThreadPool.QueueUserWorkItem((o) =>
                      {                        
                           //获取会议根目录
                           var mettingList = Constant.clientContextManage.GetFolders(listName);

                           //获取相关会议的所有文档
                           var mettingF = mettingList.Where(Item => Item.Name.Equals(folderName)).ToList<SP.Folder>();
                           //if (mettingF.Count <1)
                           //{
                           //    //新建文件夹（根目录下的文件夹）
                           //    this.clientContextManage.CreateFolder(listName, folderName);
                           //}
                           if (mettingF.Count > 0)
                           {
                               //获取文件夹
                               SP.Folder folder = mettingF[0];
                               //个人笔记文件完全限定名
                               string PersonalFile = this.personalNoteName + this.personNoteFileExtention;

                               //加载当前所有文件
                               Constant.clientContextManage.LoadMethod(folder.Files);
                               //获取当前所有文件
                               List<SP.File> fileList = folder.Files.ToList<SP.File>();
                               //获取个人笔记
                               var intPersonFileListCount = fileList.Count(Item => Item.Name.Equals(PersonalFile));

                               //个人笔记不存在
                               if (intPersonFileListCount < 1)
                               {
                                   //创建个人笔记
                                   string returnStr = Constant.clientContextManage.CreateFile(folder, Constant.PaintFileRoot + "\\" + Constant.LocalPersonalNoteFile, PersonalFile);
                               }

                               //获取文件路径
                               string fileUri = Constant.SPSiteAddressFront + folder.ServerRelativeUrl + "/" + PersonalFile + this.owaWebExtentionName;

                               this.Dispatcher.BeginInvoke(new Action(() =>
                                   {
                                       //生成凭据通过智存空间验证
                                       this.WebCManage = new WebCredentialManage(this.webBrowser, Constant.WebLoginUserName, Constant.WebLoginPassword);
                                       //导航到个人空间
                                       this.WebCManage.Navicate(fileUri);
                                       this.LoadingVisibility = System.Windows.Visibility.Collapsed;
                                       this.PersonNoteVisibility = System.Windows.Visibility.Visible;
                                   }));

                           }
                       });
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
