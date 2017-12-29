
using Conference_WebBrowser.Common;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.RegeditHelper;
using ConferenceCommon.WebHelper;
using ConferenceCommon.WPFHelper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Conference_WebBrowser
{
    /// <summary>
    /// MeetVoteView.xaml 的交互逻辑
    /// </summary>
    public partial class WebBrowserView : UserControlBase
    {
        #region 自定义委托事件(回调)

        /// <summary>
        /// 浏览器共享回调
        /// </summary>
        public Action<Uri> ShareWebBrowserCallBack = null;

        #endregion

        #region 字段

        /// <summary>
        /// 当前目录
        /// </summary>
        string currentDirectory = Environment.CurrentDirectory;

        /// <summary>
        /// 文件所处路径
        /// </summary>
        string uriFileName = string.Empty;

        /// <summary>
        /// 百度访问地址
        /// </summary>
        string baiduUri = "http://61.135.169.121";

        /// <summary>
        /// 验证数据注入
        /// </summary>
        WebCredentialManage WebCManage = null;

        #endregion

        #region 绑定属性

        ObservableCollection<UrlEntity> urlEntityList = new ObservableCollection<UrlEntity>();
        /// <summary>
        /// uri列表
        /// </summary>
        public ObservableCollection<UrlEntity> UrlEntityList
        {
            get { return urlEntityList; }
            set
            {
                if (this.urlEntityList != value)
                {
                    this.urlEntityList = value;
                    this.OnPropertyChanged("UrlEntityList");
                }
            }
        }

        UrlEntity selectedUlEntity;
        /// <summary>
        /// 当前选中的地址
        /// </summary>
        public UrlEntity SelectedUlEntity
        {
            get { return selectedUlEntity; }
            set
            {
                if (this.selectedUlEntity != value)
                {
                    this.selectedUlEntity = value;
                    this.OnPropertyChanged("SelectedUlEntity");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public WebBrowserView()
        {
            try
            {
                //加载UI
                InitializeComponent();

                //初始化加载
                this.ParametersInit();

                //注册事件
                this.EventRegedit();

                //加载uri列表
                this.LoadUrlStoreData();
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

        #region 初始化

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileUri"></param>
        private void ParametersInit()
        {
            try
            {
                //生成凭据通过智存空间验证
                this.WebCManage = new WebCredentialManage(this.webBrowser, WebBrowserCodeEnterEntity.WebLoginUserName, WebBrowserCodeEnterEntity.WebLoginPassword);
                //导航到个人空间
                this.WebCManage.Navicate(WebBrowserCodeEnterEntity.WebUri);

                //显示导航地址
                this.txtInput.Text = WebBrowserCodeEnterEntity.WebUri;               
                //当前输出目录
                currentDirectory = Environment.CurrentDirectory;
                //存储uri地址路径
                uriFileName = currentDirectory + "\\" + WebBrowserCodeEnterEntity.urlStoreFileName;
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
                //加载文档
                this.webBrowser.Navigated += webBrowser_Navigated;
                //新窗体加载事件
                this.webBrowser.NewWindow += new CancelEventHandler(this.webBrowser_NewWindow);
                //导航到指定地址
                this.btnNavicate.Click += btnNavicate_Click;
                //收藏地址
                this.btnCollection.Click += btnCollection_Click;
                //演示
                this.btnShare.Click += btnShare_Click;
                //url列表选中事件
                this.btnCmb.SelectionChanged += btnCmb_SelectionChanged;
                //url移除事件
                this.btnUrlRemove.Click += btnUrlRemove_Click;
                //上一页
                this.btnUp.Click += btnUp_Click;
                //下一页
                this.btnNext.Click += btnNext_Click;
                //进入主页
                this.btnHome.Click += btnHome_Click;
                //转到百度
                this.btnBaidu.Click += btnBaidu_Click;
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

        #region 转到百度

        /// <summary>
        /// 转到百度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBaidu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //转到百度
                this.webBrowser.Navigate(this.baiduUri);
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

        #region 进入主页

        /// <summary>
        /// 进入主页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnHome_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载默认页面
                this.webBrowser.Navigate(WebBrowserCodeEnterEntity.WebUri);
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

        #region 下一页

        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //是否可以返回下一页
                if (this.webBrowser.CanGoForward)
                {
                    //返回下一页
                    this.webBrowser.GoForward();
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

        #region 上一页

        /// <summary>
        /// 上一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnUp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //是否可以进行返回上一页
                if (this.webBrowser.CanGoBack)
                {
                    //返回上一页
                    this.webBrowser.GoBack();
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

        #region url移除事件

        /// <summary>
        /// url移除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnUrlRemove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //移除相关uri地址
                this.UrlEntityList.Remove(this.SelectedUlEntity);
                //重新加载存储记录
                this.ReSave();
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

        #region url列表选中事件

        /// <summary>
        /// url列表选中事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //导航到地址
                this.webBrowser.Navigate(this.SelectedUlEntity.Url);
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

        #region 开始演示

        /// <summary>
        /// 开始演示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnShare_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //获取当前地址
                Uri uri = this.webBrowser.Url;
                //浏览器共享回调
                if (this.ShareWebBrowserCallBack != null)
                {
                    this.ShareWebBrowserCallBack(uri);
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

        #region 浏览器打开之前（清除缓存）

        /// <summary>
        /// 浏览器打开之前（清除缓存）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser_NewWindow(object sender, CancelEventArgs e)
        {
            try
            {
                //给浏览器指定地址
                this.webBrowser.Url = new Uri(((System.Windows.Forms.WebBrowser)sender).StatusText);
                //确认已处理
                e.Cancel = true;
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

        #region 加载文档(浏览器)

        /// <summary>
        /// 加载文档(浏览器)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void webBrowser_Navigated(object sender, System.Windows.Forms.WebBrowserNavigatedEventArgs e)
        {
            try
            {
                //错误提示事件
                this.webBrowser.Document.Window.Error += new System.Windows.Forms.HtmlElementErrorEventHandler(this.Window_Error);
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
        /// 浏览器加载异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Error(object sender, System.Windows.Forms.HtmlElementErrorEventArgs e)
        {
            try
            {
                //标示已处理
                e.Handled = true;
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

        #region 收藏地址

        /// <summary>
        /// 收藏地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载新地址
                this.urlEntityList.Add(new UrlEntity(this.webBrowser.Document.Title, this.webBrowser.Url.AbsoluteUri));
                //重新加载存储
                this.ReSave();
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

        #region 加载uri列表库

        /// <summary>
        /// 加载uri列表库
        /// </summary>
        private void LoadUrlStoreData()
        {

            try
            {
                //若不存在该文件则创建
                if (!File.Exists(uriFileName))
                {
                    File.Create(uriFileName);
                }
                else
                {
                    //使用文件流的方式加载uri列表信息
                    using (FileStream fileStream = new FileStream(uriFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        //文件流长度大于0
                        if (fileStream.Length > 0)
                        {
                            //创建序列化类
                            BinaryFormatter binaryFormatter = new BinaryFormatter();
                            //反序列化
                            object uriEntityCollection = binaryFormatter.Deserialize(fileStream);
                            //还原成原来的实体集合
                            if (uriEntityCollection is ObservableCollection<UrlEntity>)
                            {
                                this.urlEntityList = (uriEntityCollection as ObservableCollection<UrlEntity>);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //出现异常删除文件
                File.Delete(uriFileName);
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }

        }

        #endregion

        #region 重新加载uri存储文件

        /// <summary>
        /// 重新加载uri存储文件
        /// </summary>
        private void ReSave()
        {
            try
            {
                //判断文件是否存在（不存在则进行创建）
                this.ChekFile();
                //通过文件流进行序列化
                using (FileStream fileStream = new FileStream(uriFileName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite))
                {
                    //序列化对象执行
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(fileStream, this.urlEntityList);
                    //删除文件流缓存
                    fileStream.Flush();
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

        #region 判断文件是否存在（不存在则进行创建）

        /// <summary>
        /// 判断文件是否存在（不存在则进行创建）
        /// </summary>
        private void ChekFile()
        {
            try
            {
                //若不包含该文件则创建
                if (!File.Exists(uriFileName))
                {
                    File.Create(uriFileName);
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

        #region 导航到指定地址

        /// <summary>
        /// 导航到指定地址
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnNavicate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //网址导航
                this.webBrowser.Navigate(this.txtInput.Text.Trim());
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
