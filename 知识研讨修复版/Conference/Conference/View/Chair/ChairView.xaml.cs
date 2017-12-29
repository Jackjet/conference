using Conference.Page;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.ScreenSetting;
using ConferenceModel;
using Studiom_Model.Model;
using studiom = Studiom_Model.Pro_KnowledgeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using ConferenceCommon.VlcHelper;
using Vlc.DotNet.Core.Medias;
using Vlc.DotNet.Core;
using Vlc.DotNet.Wpf;
using ConferenceCommon.WPFHelper;

namespace Conference.View.Chair
{
    /// <summary>
    /// ChairView.xaml 的交互逻辑
    /// </summary>
    public partial class ChairView : UserControl
    {
        #region 内部字段
              
        //  <summary>
        //  流媒体播放器
        //  </summary>
        /// <summary>
        /// vlc流媒体播放器
        /// </summary>
        public static VlcControl vlcPlayer = null;
     
        #endregion

        #region 静态字段

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ChairView()
        {
            try
            {
                //UI加载
                InitializeComponent();

                //加载会议现场
                this.BeginPlay();
                //事件注册
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
                //同步我的会议页面
                this.btnMeet.Click += btnNavicate;
                //同步U盘传输页面
                this.btn_U_disk.Click += btnNavicate;
                //同步主持人功能页面
                this.btnChair.Click += btnNavicate;
                //同步信息交流页面
                this.btnIMM.Click += btnNavicate;
                //同步个人笔记页面
                this.btnNote.Click += btnNavicate;
                //同步资源共享页面
                this.btnResource.Click += btnNavicate;
                //同步智存空间页面
                this.btnSpace.Click += btnNavicate;
                //同步中控功能页面
                this.btnStudiom.Click += btnNavicate;
                //同步系统设置页面
                this.btnSystemSetting.Click += btnNavicate;
                //同步知识树页面
                this.btnTree.Click += btnNavicate;
                //同步投票页面
                this.btnWebBrowser.Click += btnNavicate;
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

        #region 开始现场播放

        /// <summary>
        /// 开始现场播放
        /// </summary>
        private void BeginPlay()
        {
            try
            {
                if (vlcPlayer == null)
                {
                    //初始化流媒体播放器
                    this.VlcMediaPlayerInit();
                }
                //播放视频流
                this.Play(Constant.ConferenceRoomCameraAddress);
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
      
        #region 初始化流媒体播放器

        /// <summary>
        /// 初始化流媒体播放器
        /// </summary>m
        public void VlcMediaPlayerInit()
        {
            try
            {
                //vlc配置参数
                VlcContext.StartupOptions.IgnoreConfig = true;
                VlcContext.StartupOptions.LogOptions.LogInFile = false;
                VlcContext.StartupOptions.LogOptions.ShowLoggerConsole = false;
                VlcContext.StartupOptions.LogOptions.Verbosity = VlcLogVerbosities.None;
                VlcContext.LibVlcPluginsPath = Environment.CurrentDirectory + "\\plugins";
                VlcContext.LibVlcDllsPath = Environment.CurrentDirectory;
                //流媒体播放器初始化
                VlcContext.Initialize();
                //播放器实例生成
                vlcPlayer = new VlcControl();


                // 创建绑定，绑定Image
                Binding bing = new Binding();
                bing.Source = vlcPlayer;
                bing.Path = new PropertyPath("VideoSource");
                this.img.SetBinding(Image.SourceProperty, bing);
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

        #region 流媒体播放器开始播放

        /// <summary>
        /// 流媒体播放器开始播放
        /// </summary>
        /// <param name="uri">播放地址</param>
        public void Play(string uri)
        {
            try
            {
                if (vlcPlayer != null)
                {
                    //设置播放地址
                    LocationMedia media = new LocationMedia(uri);
                    //PathMedia media = new PathMedia(uri);
                    //播放
                    vlcPlayer.Play(media);
                }

                //if (this.vlcPlayer == null)
                //{
                //    string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
                //    this.vlcPlayer = new VlcPlayer(pluginPath);
                //}
                //this.vlcPlayer.PlayFile(uri);
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
      
        #region 页面同步

        /// <summary>
        /// 页面同步
        /// </summary>
        void btnNavicate(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is NavicateButton)
                {
                    NavicateButton navicateButton = sender as NavicateButton;
                    //首页子项选择事件
                    this.SyncPageHelper(navicateButton.ViewSelectedItemEnum);
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

        #region 页面同步辅助

        /// <summary>
        /// 页面同步辅助
        /// </summary>
        /// <param name="viewSelectedItemEnum">选择的子项</param>
        public void SyncPageHelper(ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                //页面同步
                ModelManage.ConferenceInfo.FillConferenceOfficeServiceData(Constant.ConferenceName, Constant.SelfName, (ConferenceModel.ConferenceInfoWebService.ConferencePageType)viewSelectedItemEnum, new Action<bool>((successed) =>
                {
                    if (successed)
                    {

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
       
        #region vlc流媒体资源释放

        #endregion
    }
}
