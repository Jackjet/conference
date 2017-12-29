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
using Conference_IMM.Common;

namespace Conference_IMM
{
    /// <summary>
    /// 研讨语音
    /// </summary>
    public partial class ConferenceAudio_View : UserControl
    {
        #region 自定义委托事件(回调)

        /// <summary>
        /// 得到消息提醒（闪烁）
        /// </summary>
        public Action IMMFalshCallBack = null;

        #endregion

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

        /// <summary>
        /// 是否延迟
        /// </summary>
        bool delay = false;

        /// <summary>
        /// 转译模式
        /// </summary>
        TransferModeType transferModeType = TransferModeType.AudioAndTextMode;

        /// <summary>
        /// 标题介绍
        /// </summary>
        string title_introduce = "_信息交流记录";

        #endregion

        #region 拼接字符（html）

        /// <summary>
        /// 导出文件前半部分
        /// </summary>
        string htmlPart1 = "";

        /// 导出文件后半部分
        /// </summary>
        string htmlPart2 = "";

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
        static private object ItemAddObject = new object();

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

        /// <summary>
        /// 自我绑定
        /// </summary>
        public static ConferenceAudio_View conferenceAudio_View = null;

        #endregion

        #region 一般属性

        //bool isStart = false;
        ///// <summary>
        ///// 启动实例标示
        ///// </summary>
        //public bool IsStart
        //{
        //    get { return isStart; }
        //    set { isStart = value; }
        //}

        #endregion

        #region 静态属性

        /// <summary>
        /// 语音转译进行时样式
        /// </summary>
        static Style btnAudioRunStyle = null;

        /// <summary>
        /// 语音常规样式
        /// </summary>
        static Style btnAudioStyle = null;

        #endregion

        #region 构造函数

        /// <summary>
        /// 构造函数
        /// </summary>
        public ConferenceAudio_View()
        {
            try
            {
                InitializeComponent();

                //自我绑定
                conferenceAudio_View = this;
                //研讨语音初始化
                this.AudioControlInit();
                //文本框编辑权限控制器
                this.TittleLimitControlCenter();
                //样式收集
                this.StyleCollection();
                //事件注册
                this.EventRegedit();
                //导出文件前半部分初始化
                this.HtmlPart1Init();
                //导出文件后半部分初始化
                this.htmlPart2Init();
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
                //提交编辑信息
                this.btnSubmitAudio.Click += btnSubmitAudio_Click;
                //语音转文字操作
                this.btnAudioTransfer.Click += btnAudioTransfer_Click;
                //弹出二维码
                //this.btnQR.Click += btnQR_Click;
                //转译模式选择
                this.radioBtnJustText.Checked += radioBtn_Checked;
                //转译模式选择
                this.radioBtnAudioAndText.Checked += radioBtn_Checked;
                //清除文本
                this.btnTextClear.Click += btnTextClear_Click;
                //导出Word
                this.btnExportWord.Click += btnExportWord_Click;
                //导出PDF
                this.btnExportPDF.Click += btnExportPDF_Click;
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

        #region 清除文本

        /// <summary>
        /// 清除文本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnTextClear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.txtInput.Text = string.Empty;
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

        #region 转译模式选择

        /// <summary>
        /// 转译模式选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void radioBtn_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.Equals(this.radioBtnAudioAndText))
                {
                    this.transferModeType = TransferModeType.AudioAndTextMode;
                }
                else if (sender.Equals(this.radioBtnJustText))
                {
                    this.transferModeType = TransferModeType.JustTextMode;
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

        #region 样式收集

        /// <summary>
        /// 样式收集
        /// </summary>
        public void StyleCollection()
        {
            try
            {
                if (btnAudioRunStyle == null)
                {
                    btnAudioRunStyle = (base.Resources["btnAudioRunStyle"] as Style);
                }
                if (btnAudioStyle == null)
                {
                    btnAudioStyle = (base.Resources["btnAudioStyle"] as Style);
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

        #region 初始化UI

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
                        if (transferItem.AddAuthor.Equals(IMMCodeEnterEntity.LoginUserName))
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
                        academicReviewItem.ItemPlayClick = academicReviewItem_ItemPlayClick;
                        //研讨语音移除
                        academicReviewItem.ItemRemoveClick = academicReviewItem_ItemRemoveClick;

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
        /// <param name="result">语音协议实体</param>
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

                            //得到消息提醒
                            if (this.IMMFalshCallBack != null)
                            {
                                this.IMMFalshCallBack();
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
                    //不可再次进入,除非执行完相关操作
                    GoIntoInit = false;

                    #region 语音初始化

                    //录播设置
                    this.wave = new Wave();
                    //异常捕获事件
                    this.wave.ErrorEvent += new ErrorEventHandle(wave_ErrorEvent);

                    #endregion

                    //获取所有语音研讨信息
                    if (ConferenceAudio_View.RootCount == 0)
                    {
                        //从服务器获取所有语音节点
                        ModelManage.ConferenceAudio.GetAll(IMMCodeEnterEntity.ConferenceName, new Action<ConferenceAudioInitRefleshEntity>((result) =>
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

        #region 获取语音视图宽度

        /// <summary>
        /// 获取语音视图宽度
        /// </summary>
        public double GetIMMView_ActualWidth()
        {
            double actualWidth = 0;
            try
            {
                //获取实际宽度
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
                //信息交流释放滚动条是否可以滚动
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
                //信息交流释放滚动条是否可以滚动
                ConferenceAudio_View.CanScroll = false;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 控件调整处理中心

        /// <summary>
        /// 控件调整处理中心
        /// </summary>
        /// <param name="conferenceAudioItem">语音信息子项</param>
        /// <param name="isReflesh">是否进行更新</param>
        public static void ControlChangeDealWidthCenter(ConferenceAudioItem conferenceAudioItem, bool isReflesh)
        {
            try
            {
                //语音信息类型
                AudioItemType audioItemType = default(AudioItemType);
                //是否为主持人
                if (IMMCodeEnterEntity.IsMeetingPresenter)
                {
                    //是否包含音频文件
                    if (!string.IsNullOrEmpty(conferenceAudioItem.AudioUrl))
                    {
                        //包含语音
                        audioItemType = AudioItemType.LimitHasAudio;
                    }
                    else
                    {
                        //不包含语音
                        audioItemType = AudioItemType.LimitNoAudio;
                    }
                }
                else
                {
                    //是否包含音频文件
                    if (!string.IsNullOrEmpty(conferenceAudioItem.AudioUrl))
                    {
                        //包含语音
                        audioItemType = AudioItemType.NoLimitHasAudio;
                    }
                    else
                    {
                        //不包含语音
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
   
    }
}
