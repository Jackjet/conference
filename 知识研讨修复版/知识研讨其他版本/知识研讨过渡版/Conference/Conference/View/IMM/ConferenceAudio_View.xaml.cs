using Conference.Page;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.AudioRecord;
//using ConferenceCommon.AudioRecord;
using ConferenceCommon.AudioTransferHelper;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.KeyBoard;
using ConferenceCommon.LogHelper;
using ConferenceModel;
//using ConferenceWebCommon.EntityHelper.ConferenceDiscuss;
using ConferenceModel.ConferenceAudioWebservice;
using Microsoft.Lync.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
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
using Vlc.DotNet.Wpf;
using Vlc.DotNet.Core;
using Vlc.DotNet.Core.Medias;
using ConferenceCommon.VlcHelper;
using ConferenceCommon.IcoFlash;

namespace Conference.View.IMM
{
    /// <summary>
    /// 研讨语音
    /// </summary>
    public partial class ConferenceAudio_View : UserControl
    {
        #region 字段

        /// <summary>
        /// 录音设备
        /// </summary>
        IWaveControl wave;

        /// <summary>
        /// 声音播放器
        /// </summary>
        //SoundPlayer player = new SoundPlayer();

        /// <summary>
        /// 正在进行语音识别
        /// </summary>
        bool isInTransfer = false;

        /// <summary>
        /// 语音讨论点对点映射集合
        /// </summary>
        public List<int> retunList = new List<int>();

        /// <summary>
        /// 流媒体播放器
        /// </summary>
        VlcPlayer vlcPlayer = null;

        /// <summary>
        /// 当前时间
        /// </summary>
        string dateNow = null;

        #endregion

        #region 静态字段

        /// <summary>
        /// 根记录
        /// </summary>
        public static int RootCount = 0;

        /// <summary>
        /// 记录所有语音研讨的记录
        /// </summary>
        public static List<ConferenceAudioItem> ConferenceAudioItemList = new List<ConferenceAudioItem>();

        /// <summary>
        /// 会中，语音研讨是否可以滚动
        /// </summary>
        static bool CanScroll = true;

        /// <summary>
        /// 互斥辅助对象
        /// </summary>
        static private object obj1 = new object();

        ///// <summary>
        ///// 流媒体播放器
        ///// </summary>
        //VlcControl vlcPlayer = null;

        /// <summary>
        /// 是否可以进入初始化
        /// </summary>
        static bool GoIntoInit = true;
      
        /// <summary>
        ///  编辑控制计时器
        /// </summary>
       public static DispatcherTimer TittleEditControlTimer = null;

        #endregion

        #region 一般属性

        bool isStart = false;
        /// <summary>
        /// 启动实例标示
        /// </summary>
        public bool IsStart
        {
            get { return isStart; }
            set { isStart = value; }
        }

        #endregion

        #region 静态属性

        //static MediaElement mediaElement = new MediaElement();
        ///// <summary>
        ///// 音频播放器
        ///// </summary>
        //public static MediaElement MediaElement
        //{
        //    get { return mediaElement; }
        //    set { mediaElement = value; }
        //}

        #endregion

        #region 构造函数

        public ConferenceAudio_View()
        {
            InitializeComponent();

            //研讨语音初始化
            this.AudioControlInit();

            //文本框编辑权限控制器
            this.TittleLimitControlCenter();

            #region 注册事件

            //提交编辑信息
            this.btnSubmitAudio.Click += btnSubmitAudio_Click;
            //语音转文字操作
            this.btnAudioTransfer.Click += btnAudioTransfer_Click;

            this.btnQR.Click += btnQR_Click;

            #endregion
        }

        #endregion

        #region 文本框编辑权限控制器

        /// <summary>
        /// 文本框编辑权限控制器（标题）
        /// </summary>
        public void TittleLimitControlCenter()
        {
            try
            {
                //一个节点对应一个计时器
                if (TittleEditControlTimer == null)
                {
                    TimerJob.StartRun(new Action(() =>
                    {
                        foreach (var item in ConferenceAudioItemList)
                        {
                            //完毕敲键盘
                            if (!item.txtAudio.IsKeyboardFocused)
                            {
                                //编辑完毕
                                item.IsEditNow = false;
                            }
                        }
                    }), 1000, out TittleEditControlTimer);
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

        #region 初始化

        /// <summary>
        /// 刷新语音所有节点
        /// </summary>
        /// <param name="result"></param>
        void Reflesh(ConferenceAudioInitRefleshEntity result)
        {
            try
            {
                //先判断获取到的信息
                if (result.AcademicReviewItemTransferEntity_ObserList != null && result.AcademicReviewItemTransferEntity_ObserList.Count() > 0)
                {
                    //进行后续的关联              

                    foreach (var transferItem in result.AcademicReviewItemTransferEntity_ObserList)
                    {
                        //添加索引
                        this.retunList.Add(transferItem.Guid);
                        //控件加载（关联）Header = transferItem.MessageHeader,
                        ConferenceAudioItem academicReviewItem = new ConferenceAudioItem() { Message = transferItem.AudioMessage, ACA_Guid = transferItem.Guid, AudioUrl = transferItem.AudioUrl, PersonalImg = transferItem.PersonalImg, PersonName = transferItem.MessageSendName };

                        //如果是自己发的则在左侧 (验证该子节点是否为自己所创建)
                        if (transferItem.AddAuthor.Equals(Constant.LoginUserName))
                        {
                            //设置为左侧
                            academicReviewItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            //若为自己所创建则设置为可编辑
                            academicReviewItem.txtAudio.IsReadOnly = false;
                            //整体布局更改
                            academicReviewItem.LayoutChange();
                        }
                        else
                        {
                            //设置为右侧
                            academicReviewItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                        }

                        //控件调整处理中心
                        ConferenceAudio_View.ControlChangeDealWidthCenter(academicReviewItem, true);

                        #region 注册事件

                        //研讨语音播放
                        academicReviewItem.ItemPlayClick += academicReviewItem_ItemPlayClick;
                        //研讨语音移除
                        academicReviewItem.ItemRemoveClick += academicReviewItem_ItemRemoveClick;

                        #endregion

                        //设置协议实体
                        academicReviewItem.AcademicReviewItemTransferEntity = transferItem;
                        //语音研讨节点集合（添加）
                        ConferenceAudioItemList.Add(academicReviewItem);
                        //添加到研讨语音容器里
                        this.stackDiscussContent.Children.Add(academicReviewItem);
                        //添加索引
                        //ConferenceAudio_View.retunList.Add(academicReviewItem.ACA_Guid);
                    }
                    //同步到最后一个节点
                    this.AudioTextScrollView.ScrollToEnd();
                    //设置节点
                    ConferenceAudio_View.RootCount = result.RootCount;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        /// <summary>
        /// 同步语音研讨（收到服务器通知）
        /// </summary>
        /// <param name="result"></param>
        public void Information_Sync(ConferenceAudioItemTransferEntity result)
        {
            try
            {
                if (result != null)
                {
                    //节点操作类型
                    switch (result.Operation)
                    {
                        //添加节点
                        case ConferenceAudioOperationType.AddType:
                            //添加子项
                            Add_Item(result);

                            //信息闪烁
                            WindowExtensions.FlashWindow(MainWindow.mainWindow, 10);

                            //已经处于信息交流页面则不进行闪烁
                            if (Conference.MainWindow.MainPageInstance.ViewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.IMM)
                            {
                                //消息闪烁
                                Conference.MainWindow.MainPageInstance.IMMReceivMessageFlash();
                            }
                            break;

                        //删除节点
                        case ConferenceAudioOperationType.DeleteType:
                            //删除子项
                            Delete_Item(result);
                            break;

                        //更新节点
                        case ConferenceAudioOperationType.UpdateType:
                            //更新子项
                            UpdateItem(result);
                            break;

                        case ConferenceAudioOperationType.UploadCompleateType:
                            //音频文件上传完成通知
                            this.NotifyUploadCompleate(result);
                            break;

                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        #endregion

        #region 添加节点

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="message"></param>
        /// <param name="audioUrl"></param>
        public void AddDiscussInformation(string message)
        {
            try
            {
                //语音研讨协议节点
                ConferenceAudioItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceAudioItemTransferEntity() { AddAuthor = Constant.LoginUserName, IMMType = "Text", AudioMessage = message, MessageSendName = Constant.SelfName, MessageSendTime = DateTime.Now.ToLongTimeString(), AudioUrl = string.Empty, PersonalImg = Constant.TreeServiceAddressFront + Constant.FtpServercePersonImgName + Constant.LoginUserName + ".png" };

                //向服务器添加一个子节点          
                ModelManage.ConferenceAudio.Add(Constant.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool, int>((result, guid) =>
                {
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="message"></param>
        /// <param name="audioUrl"></param>
        public void AddDiscussInformation(string message, string audioUrl, string audioFileName, Action<int> callBack)
        {
            try
            {
                //语音研讨协议节点
                ConferenceAudioItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceAudioItemTransferEntity() { AddAuthor = Constant.LoginUserName, IMMType = "Audio", AudioMessage = message, MessageSendName = Constant.SelfName, MessageSendTime = DateTime.Now.ToLongTimeString(), AudioUrl = audioUrl, AudioFileName = audioFileName, PersonalImg = Constant.TreeServiceAddressFront + Constant.FtpServercePersonImgName + Constant.LoginUserName + ".png" };
                //向服务器添加一个子节点          
                ModelManage.ConferenceAudio.Add(Constant.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool, int>((result, guid) =>
                {
                    if (result)
                    {
                        callBack(guid);
                    }
                }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 服务器同步添加节点
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public void Add_Item(ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                lock (obj1)
                {

                    if (!this.retunList.Contains(academicReviewItemTransferEntity.Guid))
                    {
                        //添加索引
                        this.retunList.Add(academicReviewItemTransferEntity.Guid);
                        //本地语音节点
                        ConferenceAudioItem academicReviewItem = null;
                        //控件加载（关联）Header = academicReviewItemTransferEntity.MessageHeader,
                        academicReviewItem = new ConferenceAudioItem() { ACA_Guid = academicReviewItemTransferEntity.Guid, Message = academicReviewItemTransferEntity.AudioMessage, AudioUrl = academicReviewItemTransferEntity.AudioUrl, PersonalImg = academicReviewItemTransferEntity.PersonalImg, PersonName = academicReviewItemTransferEntity.MessageSendName };

                        //如果是自己发的则在左侧(验证该子节点是否为自己所创建)
                        if (academicReviewItemTransferEntity.AddAuthor.Equals(Constant.LoginUserName))
                        {
                            //设置为左侧
                            academicReviewItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                            //若为自己所创建则设置为可编辑
                            academicReviewItem.txtAudio.IsReadOnly = false;
                            //整体布局更改
                            academicReviewItem.LayoutChange();
                        }
                        else
                        {
                            //设置为右侧
                            academicReviewItem.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                        }
                        //协议实体填充
                        academicReviewItem.AcademicReviewItemTransferEntity = academicReviewItemTransferEntity;
                        //具体语音文本信息
                        academicReviewItem.AcademicReviewItemTransferEntity.AudioMessage = academicReviewItemTransferEntity.AudioMessage;
                        //控件调整处理中心
                        ConferenceAudio_View.ControlChangeDealWidthCenter(academicReviewItem, false);

                        //语音控件添加子项
                        this.stackDiscussContent.Children.Add(academicReviewItem);

                        //滚动到末尾（判断是否可以执行该操作，比如正在翻页，不允许强制进行）
                        if (ConferenceAudio_View.CanScroll)
                        {
                            this.AudioTextScrollView.ScrollToEnd();
                        }

                        #region 注册事件

                        //语音讨论子节点播放事件
                        academicReviewItem.ItemPlayClick += academicReviewItem_ItemPlayClick;
                        //语音讨论子节点删除事件
                        academicReviewItem.ItemRemoveClick += academicReviewItem_ItemRemoveClick;

                        #endregion

                        //根记录进行自增1（其实就是GUID唯一标识符）
                        ConferenceAudio_View.RootCount++;

                        //记录所有语音研讨的记录添加语音节点
                        ConferenceAudioItemList.Add(academicReviewItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        #endregion

        #region 获取语音视图宽度

        /// <summary>
        /// 获取语音视图宽度
        /// </summary>
        public double GetIMMView_ActualWidth()
        {
            double actualWidth = 0;
            try
            {
                actualWidth = this.ActualWidth;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return actualWidth;
        }

        #endregion

        #region 语音讨论播放节点

        /// <summary>
        /// 语音讨论播放节点
        /// </summary>
        /// <param name="item"></param>
        void academicReviewItem_ItemPlayClick(ConferenceAudioItem item)
        {
            try
            {
                //如果该语音节点包含语音url，说明包含语音文件
                if (!string.IsNullOrEmpty(item.AudioUrl))
                {
                    if (this.vlcPlayer == null)
                    {
                        string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
                        this.vlcPlayer = new VlcPlayer(pluginPath);
                    }
                    this.vlcPlayer.PlayFile(item.AudioUrl);
                }
                else
                {
                    MessageBox.Show("播放的文件不存在或无效", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 删除节点

        /// <summary>
        /// 当前用户手动删除节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DeleteDiscussInformation(ConferenceAudioItem academicReviewItem)
        {
            try
            {
                //把需要删除的语音几点交给服务器，让服务器去通知其他终端进行删除的操作              
                ModelManage.ConferenceAudio.Delete(Constant.ConferenceName, academicReviewItem.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
                    {
                        if (result)
                        {
                            //验证是否包含语音
                            if (!String.IsNullOrEmpty(academicReviewItem.AudioUrl))
                            {
                                string url = string.Empty;
                                //将其转为ftp的地址，才可以进行ftp的移除（将保存到服务器的语音文件进行删除）
                                if (academicReviewItem.AudioUrl.Contains(Constant.TreeServiceAddressFront))
                                {
                                    //htp路径转成ftp路径
                                    url = academicReviewItem.AudioUrl.Replace(Constant.TreeServiceAddressFront, Constant.ConferenceFtpWebAddressFront);
                                }
                                ThreadPool.QueueUserWorkItem((o) =>
            {
                //删除子节点所包含的语音文件
                FtpManage.DeleteFile(url, Constant.FtpUserName, Constant.FtpPassword);
            });
                            }
                        }
                    }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        /// <summary>
        /// 服务器同步删除节点
        /// </summary>
        public void Delete_Item(ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                ConferenceAudioItem academicReviewItem = null;
                //记录所有语音研讨的记录清除指定语音节点(递减的方式)
                for (int i = ConferenceAudio_View.ConferenceAudioItemList.Count - 1; i > -1; i--)
                {
                    academicReviewItem = ConferenceAudio_View.ConferenceAudioItemList[i];
                    //协议实体与当前遍历到的实体
                    if (academicReviewItem.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //语音控件删除指定项
                        this.stackDiscussContent.Children.Remove(academicReviewItem);
                        //设置当前协议实体为null
                        ConferenceAudio_View.ConferenceAudioItemList[i].AcademicReviewItemTransferEntity = null;
                        //记录所有语音研讨的记录删除该语音节点
                        ConferenceAudio_View.ConferenceAudioItemList.Remove(academicReviewItem);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        /// <summary>
        /// 语音讨论删除节点
        /// </summary>
        /// <param name="item"></param>
        void academicReviewItem_ItemRemoveClick(ConferenceAudioItem item)
        {
            try
            {
                this.DeleteDiscussInformation(item);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 更改节点（服务器同步）

        /// <summary>
        /// 更改节点（服务器同步）
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public static void UpdateItem(ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历语音节点
                foreach (var item in ConferenceAudioItemList)
                {
                    //对应相应GUID
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //更改信息
                        if (item.Message != academicReviewItemTransferEntity.AudioMessage && !item.IsEditNow)
                        {
                            //语音转文字信息
                            item.Message = academicReviewItemTransferEntity.AudioMessage;
                            //设置信息
                            item.AcademicReviewItemTransferEntity.AudioMessage = item.Message;
                            //if(!string.IsNullOrEmpty(academicReviewItemTransferEntity.AudioUrl)&&string.IsNullOrEmpty( academicReviewItemTransferEntity.AudioMessage))
                            //{
                            //    item.UploadTip = Visibility.Collapsed;
                            //}
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
        }

        #endregion

        #region 音频文件上传完成通知

        /// <summary>
        /// 音频文件上传完成通知
        /// </summary>
        /// <param name="academicReviewItemTransferEntity"></param>
        public void NotifyUploadCompleate(ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                //遍历语音节点
                foreach (var item in ConferenceAudioItemList)
                {
                    //对应相应GUID
                    if (item.ACA_Guid.Equals(academicReviewItemTransferEntity.Guid))
                    {
                        //控件调整处理中心
                        ConferenceAudio_View.ControlChangeDealWidthCenter(item, true);
                        break;

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

        #region 语音研讨板块焦点控制（有焦点的情况下滚动轮进行相关设置）

        /// <summary>
        /// 语音研讨板块焦点控制（失去焦点，同步增加节点时禁止进行滚动末尾的操作）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void stackDiscussContent_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ConferenceAudio_View.CanScroll = true;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 语音研讨板块焦点控制（获得焦点，同步增加节点允许进行滚动末尾的操作）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void stackDiscussContent_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ConferenceAudio_View.CanScroll = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion        

        #region 初始化语音

        /// <summary>
        /// 语音研讨初始化
        /// </summary>
        public void AudioControlInit()
        {
            try
            {
                //做限制,防止重复
                if (GoIntoInit)
                {
                    GoIntoInit = false;

                    #region 语音初始化

                    //录播设置
                    this.wave = new Wave();
                    //异常捕获事件
                    this.wave.ErrorEvent += new ErrorEventHandle(wave_ErrorEvent);
                    //录音质量
                    //this.wave.RecordQuality = Quality.Height;

                    //this.wave.RecordQuality = Quality.Normal;


                    #endregion

                    //获取所有语音研讨信息
                    if (ConferenceAudio_View.RootCount == 0)
                    {
                        //从服务器获取所有语音节点
                        ModelManage.ConferenceAudio.GetAll(Constant.ConferenceName, new Action<ConferenceAudioInitRefleshEntity>((result) =>
                        {
                            //刷新(获取到的所有语音研讨信息)
                            this.Reflesh(result);

                            //语音研讨板块焦点控制（获得焦点，同步增加节点允许进行滚动末尾的操作）
                            this.stackDiscussContent.GotFocus += stackDiscussContent_GotFocus;
                            // 语音研讨板块焦点控制（失去焦点，同步增加节点时禁止进行滚动末尾的操作）
                            this.stackDiscussContent.LostFocus += stackDiscussContent_LostFocus;
                            //this.loading.Visibility = System.Windows.Visibility.Collapsed;
                            GoIntoInit = true;                          
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 语音转文字

        /// <summary>
        /// 语音转文字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAudioTransfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!this.isInTransfer)
                {
                    this.AudioTransfer_Dwon();
                }
                else
                {
                    this.AudioTransfer_Up();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 鼠标按下开始录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AudioTransfer_Dwon()
        {
            try
            {
                //如果当前正在进行语音识别，不可马上执行录音（）
                if (!this.isInTransfer)
                {
                    //标示为正在进行语音识别
                    this.isInTransfer = true;
                    //语音转换按钮设置为转换的样式（旋转）
                    this.btnAudioTransfer.Style = (base.Resources["btnAudioRunStyle"] as Style);
                    //  获取当前时间
                    dateNow = System.DateTime.Now.ToLongTimeString();
                    dateNow = dateNow.Replace(":", "_");
                    //设置文件保存的名称（默认为系统输出目录）
                    this.wave.SavedFile = Constant.AudioFile_Root + Constant.AudioFile_Name + dateNow + Constant.AudioFile_Extention;
                    //开始录音
                    this.wave.Start();
                }
                else
                {
                    MessageBox.Show("当前正在执行语音转换,请稍等", "操作提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
        }
        WavConvertToAmr toamr = new WavConvertToAmr();

        /// <summary>
        /// 鼠标释放结束录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AudioTransfer_Up()
        {
            try
            {
                if (this.wave.IsRecord)
                {
                    //停止录音
                    this.wave.Stop();
                    //语音转文字按钮样式恢复默认
                    this.btnAudioTransfer.Style = (base.Resources["btnAudioStyle"] as Style);

                    ThreadPool.QueueUserWorkItem((o) =>
           {
               try
               {
                   //判断录音文件是否存在
                   if (System.IO.File.Exists(this.wave.SavedFile))
                   {

                       //获取当前时间
                       //string dateNow = System.DateTime.Now.ToLongTimeString();
                       //查看存储文件，若大于20MB移除（录音故障）
                       if (File.Exists(this.wave.SavedFile))
                       {
                           FileInfo fileInfo = new FileInfo(this.wave.SavedFile);
                           if (fileInfo.Length > 1024 * 1024 * 20)
                           {
                               fileInfo.Delete();
                               //可以再次执行录音、语音转换
                               this.isInTransfer = false;
                               return;
                           }
                       }
                       //查看存储文件，若大于20MB移除（录音故障）
                       if (File.Exists(this.wave.RecordTmpFile))
                       {
                           FileInfo fileInfo = new FileInfo(this.wave.RecordTmpFile);
                           if (fileInfo.Length > 1024 * 1024 * 20)
                           {
                               fileInfo.Delete();
                               //可以再次执行录音、语音转换
                               this.isInTransfer = false;
                               return;
                           }

                       }
                       this.wave.StreamDispose();

                       //string AudioName = Constant.AudioFile_Name + dateNow + Constant.AudioFile_Extention;
                       //string amrAudioFullName = Constant.AudioFile_Root + amrAudioName;

                       //toamr.ConvertToAmr(System.Windows.Forms.Application.StartupPath, this.wave.SavedFile, amrAudioFullName);

                       //生成一个ftp辅助类（）
                       FtpManage ftpHelper = new FtpManage();
                       //获取文件名称
                       string newFileName = System.IO.Path.GetFileName(this.wave.SavedFile);
                       //设置ftp文件地址
                       string newFileFullName = Constant.ConferenceFtpWebAddressFront + Constant.FtpServerceAudioName + newFileName;
                       //ftp文件地址包含ftp的前部分地址
                       if (newFileFullName.Contains(Constant.ConferenceFtpWebAddressFront))
                       {
                           //使用异步委托
                           base.Dispatcher.BeginInvoke(new Action(() =>
                  {
                      //添加语音节点
                      this.AddDiscussInformation(string.Empty, newFileFullName.Replace(Constant.ConferenceFtpWebAddressFront, Constant.TreeServiceAddressFront), newFileName, new Action<int>((guid) =>
                          {
                              this.BeginUpload(ftpHelper, this.wave.SavedFile, newFileName, guid);
                          }));

                  }), new object[0]);
                       }

                   }
                   else
                   {
                       //可以再次执行录音、语音转换
                       this.isInTransfer = false;
                   }
               }
               catch (Exception ex)
               {
                   LogManage.WriteLog(this.GetType(), ex);
               };
           });
                }
            }
            catch (System.Exception ex)
            {
                //可以再次执行录音、语音转换
                this.isInTransfer = false;
                LogManage.WriteLog(base.GetType(), ex);
            }
        }

        public void BeginUpload(FtpManage ftpHelper, string amrAudioFullName, string amrAudioName, int guid)
        {
            //将语音文件上传到服务器
            ftpHelper.UploadFtp(amrAudioFullName, amrAudioName, Constant.ConferenceFtpWebAddressFront + Constant.FtpServerceAudioName, "/", Constant.FtpUserName, Constant.FtpPassword, delegate(long Length, double progress)
            {
            }, delegate(System.Exception error, bool result)
            {
                this.wave.StreamDispose();

                //可以再次执行录音、语音转换
                this.isInTransfer = false;

                if (File.Exists(this.wave.SavedFile))
                {
                    File.Delete(this.wave.SavedFile);
                }
                ModelManage.ConferenceAudio.UploadAudioCompleate(Constant.ConferenceName, new ConferenceAudioItemTransferEntity() { Guid = guid }, new Action<bool>((isSuccessed) =>
                    {

                    }));
            });
        }


        /// <summary>
        /// 录音异常监控事件
        /// </summary>
        /// <param name="e"></param>
        /// <param name="error"></param>
        private void wave_ErrorEvent(Exception e, string error)
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

        #region 提交信息

        /// <summary>
        /// 提交信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeySubMitAudioText(object sender, KeyEventArgs e)
        {
            try
            {
                //在文本有焦点的情况下，按ENter键可以将信息同步发送
                if (e.Key == Key.Enter)
                {
                    //文字为空不可发
                    if (!string.IsNullOrEmpty(this.txtInput.Text.Trim()))
                    {
                        ////当前时间
                        //var dateNow = DateTime.Now.ToLongTimeString();
                        ////生成的消息格式
                        //var header = dateNow + "  " + Constant.SelfName + "：";
                        //添加语音节点
                        this.AddDiscussInformation(this.txtInput.Text);
                        //清除当前的编辑文本框
                        this.txtInput.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 提交信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSubmitAudio_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.ImgSubmitAudioText();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 提交信息
        /// </summary>
        private void ImgSubmitAudioText()
        {
            try
            {
                //在文本有焦点的情况下，按ENter键可以将信息同步发送
                if (!string.IsNullOrEmpty(this.txtInput.Text.Trim()))
                {
                    //当前时间
                    //var dateNow = DateTime.Now.ToLongTimeString();
                    //生成的消息格式
                    //var header = dateNow + "  " + Constant.SelfName + "：";
                    //添加语音节点
                    this.AddDiscussInformation(this.txtInput.Text);
                    //清除当前的编辑文本框
                    this.txtInput.Clear();
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 生成二维码图片

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnQR_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Conference.MainWindow.MainPageInstance.QRWindow.Visibility == System.Windows.Visibility.Collapsed)
                {
                    Conference.MainWindow.MainPageInstance.QRWindow.Visibility = System.Windows.Visibility.Visible;
                    //显示二维码窗体
                    Conference.MainWindow.MainPageInstance.QRWindow.Show();
                }
                else
                {
                    Conference.MainWindow.MainPageInstance.QRWindow.Visibility = System.Windows.Visibility.Collapsed;
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

        #region 控件调整处理中心

        public static void ControlChangeDealWidthCenter(ConferenceAudioItem conferenceAudioItem, bool isReflesh)
        {
            try
            {
                AudioItemType audioItemType = default(AudioItemType);

                if (Constant.IsMeetingPresenter)
                {
                    //是否包含音频文件
                    if (!string.IsNullOrEmpty(conferenceAudioItem.AudioUrl))
                    {
                        audioItemType = AudioItemType.LimitHasAudio;
                    }
                    else
                    {
                        audioItemType = AudioItemType.LimitNoAudio;
                    }
                }
                else
                {
                    //是否包含音频文件
                    if (!string.IsNullOrEmpty(conferenceAudioItem.AudioUrl))
                    {
                        audioItemType = AudioItemType.NoLimitHasAudio;
                    }
                    else
                    {
                        audioItemType = AudioItemType.NoLimitNoAudio;
                    }
                }

                //控件调整
                conferenceAudioItem.MesuareAudioItem(audioItemType, isReflesh);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(ConferenceAudio_View), ex);
            }
            finally
            {
            }
        }

        #endregion

        #region 记住如果重新进入一个会议室,该实例弃用,把描述静态的实例释放

        /// <summary>
        /// 记住如果重新进入一个会议室,该实例弃用,把描述静态的实例释放
        /// </summary>
        public void SessionClear()
        {
            try
            {
                //语音讨论集清空
                this.stackDiscussContent.Children.Clear();
                //记录所有语音研讨的记录清空
                ConferenceAudio_View.ConferenceAudioItemList.Clear();
                //根记录恢复默认值
                ConferenceAudio_View.RootCount = 0;

                ConferenceAudio_View.CanScroll = true;

                ConferenceAudio_View.GoIntoInit = true;
               
                //ConferenceAudio_View.
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
