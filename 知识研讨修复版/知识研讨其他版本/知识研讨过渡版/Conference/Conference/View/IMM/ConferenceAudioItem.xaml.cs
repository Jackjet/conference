using ConferenceCommon.TimerHelper;
using ConferenceCommon.LogHelper;
using ConferenceModel;
//using ConferenceWebCommon.EntityHelper.ConferenceDiscuss;
using ConferenceModel.ConferenceAudioWebservice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using Conference.Common;
using ConferenceCommon.WPFHelper;

namespace Conference.View.IMM
{
    /// <summary>
    /// 语音讨论
    /// </summary>
    public partial class ConferenceAudioItem : UserControlBase
    {
        #region 自定义委托事件

        public delegate void ItemRemoveClickEventHandle(ConferenceAudioItem item);
        /// <summary>
        /// 语音讨论子节点删除事件
        /// </summary>
        public event ItemRemoveClickEventHandle ItemRemoveClick = null;

        public delegate void ItemPlayClickEventHandle(ConferenceAudioItem item);
        /// <summary>
        /// 语音讨论子节点播放事件
        /// </summary>
        public event ItemPlayClickEventHandle ItemPlayClick = null;

        #endregion

        #region 字段

        #endregion

        #region 属性

        string audioUrl = string.Empty;
        /// <summary>
        /// 音乐文件地址
        /// </summary>
        public string AudioUrl
        {
            get { return audioUrl; }
            set { audioUrl = value; }
        }

        bool isEditNow = false;
        /// <summary>
        /// 是否正在转译
        /// </summary>
        public bool IsEditNow
        {
            get { return isEditNow; }
            set { isEditNow = value; }
        }

        #endregion

        #region 绑定属性

        string message;
        /// <summary>
        /// 语音信息
        /// </summary>
        public string Message
        {
            get { return message; }
            set
            {
                if (message != value)
                {
                    message = value;
                    this.OnPropertyChanged("Message");
                }
            }
        }

        int aCA_Guid = 0;
        /// <summary>
        /// 每个节点的唯一标示
        /// </summary>
        public int ACA_Guid
        {
            get { return aCA_Guid; }
            set
            {
                if (aCA_Guid != value)
                {
                    aCA_Guid = value;
                    this.OnPropertyChanged("ACA_Guid");
                }
            }
        }

        //Visibility audioPlayVisibility = Visibility.Visible;
        ///// <summary>
        ///// 语音播放显示
        ///// </summary>
        //public Visibility AudioPlayVisibility
        //{
        //    get { return audioPlayVisibility; }
        //    set
        //    {
        //        if (audioPlayVisibility != value)
        //        {
        //            audioPlayVisibility = value;
        //            this.OnPropertyChanged("AudioPlayVisibility");
        //        }

        //    }
        //}


        bool deleteEnable;
        /// <summary>
        ///  删除按钮是否可用
        /// </summary>
        public bool DeleteEnable
        {
            get { return deleteEnable; }
            set
            {
                if (deleteEnable != value)
                {
                    deleteEnable = value;
                    this.OnPropertyChanged("DeleteEnable");
                }
            }
        }

        string header;
        /// <summary>
        /// 信息头部
        /// </summary>
        public string Header
        {
            get { return header; }
            set
            {
                if (header != value)
                {
                    header = value;
                    this.OnPropertyChanged("Header");
                }
            }
        }

        string personalImg;
        /// <summary>
        /// 信息头部
        /// </summary>
        public string PersonalImg
        {
            get { return personalImg; }
            set
            {
                if (personalImg != value)
                {
                    personalImg = value;
                    this.OnPropertyChanged("PersonalImg");
                    //加载头像
                    this.imgPerson.Source = new BitmapImage(new Uri(PersonalImg, UriKind.Absolute));
                }
            }
        }

        string personName;
        /// <summary>
        /// 语音发起人的名称
        /// </summary>
        public string PersonName
        {
            get { return personName; }
            set
            {
                if (personName != value)
                {
                    personName = value;
                    this.OnPropertyChanged("PersonName");
                }
            }
        }

        Visibility uploadTip = Visibility.Collapsed;
        /// <summary>
        /// 上传提示
        /// </summary>
        public Visibility UploadTip
        {
            get { return uploadTip; }
            set
            {
                if (uploadTip != value)
                {
                    uploadTip = value;
                    this.OnPropertyChanged("UploadTip");
                }
            }
        }

        #endregion

        #region 协议属性（用于传输和xml序列化使用）

        ConferenceAudioItemTransferEntity academicReviewItemTransferEntity;
        /// <summary>
        /// 协议实体
        /// </summary>
        internal ConferenceAudioItemTransferEntity AcademicReviewItemTransferEntity
        {
            get { return academicReviewItemTransferEntity; }
            set { academicReviewItemTransferEntity = value; }
        }

        #endregion

        #region 构造函数

        public ConferenceAudioItem()
        {
            try
            {
                //UI加载
                InitializeComponent();
                //设置当前上下文
                this.DataContext = this;

                //当前为会议主持人可以进行节点删除
                if (Constant.IsMeetingPresenter)
                {
                    //删除按钮可用
                    this.DeleteEnable = true;
                }
                else
                {
                    //删除按钮是不可用
                    this.DeleteEnable = false;
                }

                #region 注册事件

                //播放
                this.btnItemPlay.PreviewMouseLeftButtonDown += btnItemPlay_PreviewMouseLeftButtonDown;
                //删除
                this.btnRemove.PreviewMouseLeftButtonDown += btnRemove_PreviewMouseLeftButtonDown;
                //语音转文字
                this.btnAudioTransfer.Click += btnAudioTransfer_Click;
                //文本更改
                this.txtAudio.TextChanged += txtAudio_TextChanged;

                #endregion

                this.Visibility = System.Windows.Visibility.Hidden;
                TimerJob.StartRun(new Action(() =>
                    {
                        double iMMActualWidth = MainWindow.MainPageInstance.ConferenceAudio_View.GetIMMView_ActualWidth();
                        //设置IMM信息的宽度最大值
                        var conversationWidth = iMMActualWidth - 260;
                        //不能为负值
                        if (conversationWidth > 0)
                        {
                            this.txtAudio.MaxWidth = conversationWidth;
                        }
                        this.Visibility = System.Windows.Visibility.Visible;
                    }));
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public ConferenceAudioItem(string message)
            : this()
        {
            try
            {
                //语音信息
                this.Message = message;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 整体布局更改

        /// <summary>
        /// 整体布局更改
        /// </summary>
        public void LayoutChange()
        {
            try
            {
                //信息框宽度自适应
                this.columnH1.Width = GridLength.Auto;
                //个人头像宽度设置
                this.columnH2.Width = new GridLength(55);
                //个人头像位置
                Grid.SetColumn(this.imgPerson, 1);
                //个人名称
                Grid.SetColumn(this.txtPersonName, 1);
                //信息框位置
                Grid.SetColumn(this.borMessage, 0);


                //内部信息框位置
                this.columnT1.Width = GridLength.Auto;
                //信息指引
                this.columnT2.Width = new GridLength(15);
                //信息指引位置
                Grid.SetColumn(this.pathTriangle, 1);
                //内部信息框位置
                Grid.SetColumn(this.borInnerMessage, 0);
                //旋转180度
                this.pathAngle.Angle = 180;
                //信息框margin设置
                this.borInnerMessage.Margin = new Thickness(10, 5, -2, 5);
                //设置阴影环绕位置
                //this.pathShadow.Direction = 172;
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

        #region 文本更改

        /// <summary>
        /// 文本更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtAudio_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //键盘是否有焦点
                if ((sender as TextBox).IsKeyboardFocused)
                {
                    //正在编辑
                    this.IsEditNow = true;
                    //绑定语音信息
                    this.AcademicReviewItemTransferEntity.AudioMessage = this.txtAudio.Text;
                    //研讨语音子节点更改
                    ModelManage.ConferenceAudio.UpdateItem(Constant.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
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
                //获取语音转文字信息
                ModelManage.ConferenceAudio.SettingAudioTransfer(Constant.ConferenceName, this.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
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

        #region 自我删除

        /// <summary>
        /// 自我删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRemove_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //删除
                this.ItemRemove();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 自我删除
        /// </summary>
        private void ItemRemove()
        {
            try
            {
                //语音讨论子节点删除事件
                if (this.ItemRemoveClick != null)
                {
                    this.ItemRemoveClick(this);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 控件调整

        public void MesuareAudioItem(AudioItemType audioItemType, bool isReflesh)
        {
            try
            {
                switch (audioItemType)
                {
                    case AudioItemType.LimitHasAudio:
                        this.LimitHasAudio(isReflesh);
                        break;
                    case AudioItemType.LimitNoAudio:
                        this.LimitNoAudio();
                        break;
                    case AudioItemType.NoLimitHasAudio:
                        this.NoLimitHasAudio(isReflesh);
                        break;
                    case AudioItemType.NoLimitNoAudio:
                        this.NoLimitNoAudio();
                        break;
                    case AudioItemType.DeleteTipControl:
                        this.DeleteTipControl(isReflesh);
                        break;
                    default:
                        break;
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

        public void LimitHasAudio(bool isReflesh)
        {
            try
            {
                if (isReflesh)
                {
                    this.stackPane2.Children.Remove(this.btnUploadTip);
                    this.columnH3.Width = new GridLength(110);
                }
                else
                {
                    //添加项时而且是主持人,无需调整
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

        public void LimitNoAudio()
        {
            try
            {
                //删除播放按钮
                this.stackPanel.Children.Remove(this.btnItemPlay);
                this.stackPane2.Children.Remove(this.btnUploadTip);
                //删除转译按钮
                this.stackPane2.Children.Remove(this.btnAudioTransfer);
                //设置为null
                this.columnH3.Width = new GridLength(60);

                //初始化和添加项操作,UI不变
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }

        public void NoLimitHasAudio(bool isReflesh)
        {
            try
            {
                //清除删除按钮
                this.stackPane2.Children.Remove(this.btnRemove);

                if (isReflesh)
                {
                    this.stackPane2.Children.Remove(this.btnUploadTip);
                    this.columnH3.Width = new GridLength(60);
                }
                else
                {
                    this.columnH3.Width = new GridLength(90);
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

        public void NoLimitNoAudio()
        {
            try
            {
                this.stackPane2.Children.Remove(this.btnUploadTip);
                //删除播放按钮
                this.stackPanel.Children.Remove(this.btnItemPlay);
                //清除删除按钮
                this.stackPane2.Children.Clear();
               
                this.columnH3.Width = new GridLength(10);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
        }       

        public void DeleteTipControl(bool isReflesh)
        {
            try
            {
                this.stackPane2.Children.Remove(this.btnUploadTip);
                this.columnH3.Width = new GridLength(110);
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

        #region 播放

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnItemPlay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.ItemPlay();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        private void ItemPlay()
        {
            try
            {
                //语音讨论子节点播放事件
                if (this.ItemPlayClick != null)
                {
                    this.ItemPlayClick(this);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

    }

    public enum AudioItemType
    {
        LimitHasAudio,
        LimitNoAudio,
        NoLimitHasAudio,
        NoLimitNoAudio,
        DeleteTipControl
    }
}
