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
using ConferenceCommon.FileHelper;


namespace Conference_IMM
{
    public partial class ConferenceAudio_View : UserControl
    {
        #region 添加节点

        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="message">信息内容</param>      
        public void AddDiscussInformation(string message)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                //语音研讨协议节点
                ConferenceAudioItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceAudioItemTransferEntity()
                {
                    AddAuthor = IMMCodeEnterEntity.LoginUserName,
                    IMMType = "Text",
                    AudioMessage = message,
                    MessageSendName = IMMCodeEnterEntity.SelfName,
                    MessageSendTime = dateTime.ToLongTimeString(),
                    MessageSendDate = dateTime.ToLongDateString(),
                    AudioUrl = string.Empty,
                    PersonalImg = IMMCodeEnterEntity.TreeServiceAddressFront + IMMCodeEnterEntity.FtpServercePersonImgName + IMMCodeEnterEntity.LoginUserName + ".png"
                };

                //向服务器添加一个子节点          
                ModelManage.ConferenceAudio.Add(IMMCodeEnterEntity.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool, int>((result, guid) =>
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
        /// <param name="message">消息</param>
        /// <param name="audioUrl">语音地址</param>
        /// <param name="audioFileName">语音文件</param>
        /// <param name="callBack">事件回调</param>
        public void AddDiscussInformation(string message, string audioUrl, string audioFileName, Action<int> callBack)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                //语音研讨协议节点
                ConferenceAudioItemTransferEntity AcademicReviewItemTransferEntityChild = new ConferenceAudioItemTransferEntity()
                {
                    AddAuthor = IMMCodeEnterEntity.LoginUserName,
                    IMMType = "Audio",
                    AudioMessage = message,
                    MessageSendName = IMMCodeEnterEntity.SelfName,
                    MessageSendTime = dateTime.ToLongTimeString(),
                    MessageSendDate = dateTime.ToLongDateString(),
                    AudioUrl = audioUrl,
                    AudioFileName = audioFileName,
                    PersonalImg = IMMCodeEnterEntity.TreeServiceAddressFront + IMMCodeEnterEntity.FtpServercePersonImgName + IMMCodeEnterEntity.LoginUserName + ".png"
                };
                //向服务器添加一个子节点          
                ModelManage.ConferenceAudio.Add(IMMCodeEnterEntity.ConferenceName, AcademicReviewItemTransferEntityChild, new Action<bool, int>((result, guid) =>
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
        /// <param name="academicReviewItemTransferEntity">语音协议实体</param>
        public void Add_Item(ConferenceAudioItemTransferEntity academicReviewItemTransferEntity)
        {
            try
            {
                lock (ItemAddObject)
                {

                    if (!this.retunList.Contains(academicReviewItemTransferEntity.Guid))
                    {
                        //添加索引
                        this.retunList.Add(academicReviewItemTransferEntity.Guid);
                        //本地语音节点
                        ConferenceAudioItem academicReviewItem = null;
                        //控件加载（关联）Header = academicReviewItemTransferEntity.MessageHeader,
                        academicReviewItem = new ConferenceAudioItem()
                        {
                            ACA_Guid = academicReviewItemTransferEntity.Guid,
                            Message = academicReviewItemTransferEntity.AudioMessage,
                            AudioUrl = academicReviewItemTransferEntity.AudioUrl,
                            PersonalImg = academicReviewItemTransferEntity.PersonalImg,
                            PersonName = academicReviewItemTransferEntity.MessageSendName
                        };

                        //如果是自己发的则在左侧(验证该子节点是否为自己所创建)
                        if (academicReviewItemTransferEntity.AddAuthor.Equals(IMMCodeEnterEntity.LoginUserName))
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
                        academicReviewItem.ItemPlayClick = academicReviewItem_ItemPlayClick;
                        //语音讨论子节点删除事件
                        academicReviewItem.ItemRemoveClick = academicReviewItem_ItemRemoveClick;

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
                //提交信息
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

        #region 删除节点

        /// <summary>
        /// 当前用户手动删除节点
        /// </summary>
        /// <param name="academicReviewItem">语音实体</param>
        public void DeleteDiscussInformation(ConferenceAudioItem academicReviewItem)
        {
            try
            {
                //把需要删除的语音几点交给服务器，让服务器去通知其他终端进行删除的操作              
                ModelManage.ConferenceAudio.Delete(IMMCodeEnterEntity.ConferenceName, academicReviewItem.AcademicReviewItemTransferEntity, new Action<bool>((result) =>
                {
                    if (result)
                    {
                        //验证是否包含语音
                        if (!String.IsNullOrEmpty(academicReviewItem.AudioUrl))
                        {
                            string url = string.Empty;
                            //将其转为ftp的地址，才可以进行ftp的移除（将保存到服务器的语音文件进行删除）
                            if (academicReviewItem.AudioUrl.Contains(IMMCodeEnterEntity.TreeServiceAddressFront))
                            {
                                //htp路径转成ftp路径
                                url = academicReviewItem.AudioUrl.Replace(IMMCodeEnterEntity.TreeServiceAddressFront, IMMCodeEnterEntity.ConferenceFtpWebAddressFront);
                            }
                            ThreadPool.QueueUserWorkItem((o) =>
                            {
                                //删除子节点所包含的语音文件
                                FtpManage.DeleteFile(url, IMMCodeEnterEntity.FtpUserName, IMMCodeEnterEntity.FtpPassword);
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
        /// <param name="academicReviewItemTransferEntity">语音信息协议实体</param>
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
        /// <param name="item">语音信息实体</param>
        void academicReviewItem_ItemRemoveClick(ConferenceAudioItem item)
        {
            try
            {
                //手动删除节点
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
        /// <param name="academicReviewItemTransferEntity">语音信息协议实体</param>
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
                        //给vlc指定组件路径
                        string pluginPath = System.Environment.CurrentDirectory + "\\plugins\\";
                        //初始化vlc播放器
                        this.vlcPlayer = new VlcPlayer(pluginPath);
                    }
                    //vlc播放器播放音频文件
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
                    //鼠标按下录音开始
                    this.AudioTransfer_Dwon();
                }
                else
                {
                    //鼠标释放录音结束
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
        public void AudioTransfer_Dwon()
        {
            try
            {
                if (!this.delay)
                {
                    this.delay = true;
                    //如果当前正在进行语音识别，不可马上执行录音（）
                    if (!this.isInTransfer)
                    {
                        //标示为正在进行语音识别
                        this.isInTransfer = true;
                        //语音转换按钮设置为转换的样式（旋转）
                        this.btnAudioTransfer.Style = btnAudioRunStyle;
                        //  获取当前时间
                        dateNow = System.DateTime.Now.ToLongTimeString();
                        dateNow = dateNow.Replace(":", "_");
                        //设置文件保存的名称（默认为系统输出目录）
                        this.wave.SavedFile = IMMCodeEnterEntity.AudioFile_Root + IMMCodeEnterEntity.AudioFile_Name + dateNow + IMMCodeEnterEntity.AudioFile_Extention;
                        //开始录音
                        this.wave.Start(new Action<bool>((successed) =>
                            {
                                if (!successed)
                                {

                                }
                                this.delay = false;
                            }));
                    }
                    else
                    {
                        MessageBox.Show("当前正在执行语音转换,请稍等", "操作提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManage.WriteLog(base.GetType(), ex);
            }
        }

        /// <summary>
        /// 鼠标释放结束录音
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AudioTransfer_Up()
        {
            try
            {
                if (!this.delay)
                {
                    if (this.wave != null && this.wave.IsRecord)
                    {
                        //停止录音
                        this.wave.Stop();
                        //语音转文字按钮样式恢复默认
                        this.btnAudioTransfer.Style = btnAudioStyle;
                        //后台队列执行
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            try
                            {
                                //判断录音文件是否存在
                                if (System.IO.File.Exists(this.wave.SavedFile))
                                {
                                    //录音检测处理
                                    this.RecodeAudioCheckDealWidth();
                                    //录音释放
                                    this.wave.StreamDispose();

                                    switch (this.transferModeType)
                                    {
                                        case TransferModeType.JustTextMode:
                                            this.JustTextModel();
                                            break;

                                        case TransferModeType.AudioAndTextMode:
                                            //语音加文本模式
                                            this.AudioAndTextModel();

                                            break;
                                        default:
                                            break;
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
            }
            catch (System.Exception ex)
            {
                //可以再次执行录音、语音转换
                this.isInTransfer = false;
                LogManage.WriteLog(base.GetType(), ex);
            }
        }

        /// <summary>
        /// 录音检测处理
        /// </summary>
        public void RecodeAudioCheckDealWidth()
        {
            try
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
                    //创建文件操作实例
                    FileInfo fileInfo = new FileInfo(this.wave.RecordTmpFile);
                    if (fileInfo.Length > 1024 * 1024 * 20)
                    {
                        //永久删除文件
                        fileInfo.Delete();
                        //可以再次执行录音、语音转换
                        this.isInTransfer = false;
                        return;
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

        /// <summary>
        /// 纯文本模式
        /// </summary>
        public void JustTextModel()
        {
            try
            {
                //可以再次执行录音、语音转换
                this.isInTransfer = false;
                //指定文件名称进行转译
                string message = AudioTransfer.AudioToText(this.wave.SavedFile);
                //异步委托
                this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        string beforeText = this.txtInput.Text;
                        this.txtInput.Text = beforeText + " " + message;
                    }));


                //this.Dispatcher.BeginInvoke(new Action(() =>
                //    {
                //        string beforeText = this.txtInput.Text;
                //        this.txtInput.Text = beforeText + " " + "转译成功";
                //    }));
                ////可以再次执行录音、语音转换
                //this.isInTransfer = false;
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
        /// 语音加文本模式
        /// </summary>
        public void AudioAndTextModel()
        {
            try
            {
                //生成一个ftp辅助类（）
                FtpManage ftpHelper = new FtpManage();
                //获取文件名称
                string newFileName = System.IO.Path.GetFileName(this.wave.SavedFile);
                //设置ftp文件地址
                string newFileFullName = IMMCodeEnterEntity.ConferenceFtpWebAddressFront + IMMCodeEnterEntity.FtpServerceAudioName + newFileName;
                //ftp文件地址包含ftp的前部分地址
                if (newFileFullName.Contains(IMMCodeEnterEntity.ConferenceFtpWebAddressFront))
                {
                    //使用异步委托
                    base.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        //添加语音节点
                        this.AddDiscussInformation(string.Empty, newFileFullName.Replace(IMMCodeEnterEntity.ConferenceFtpWebAddressFront, IMMCodeEnterEntity.TreeServiceAddressFront), newFileName, new Action<int>((guid) =>
                        {
                            this.BeginUpload(ftpHelper, this.wave.SavedFile, newFileName, guid);
                        }));

                    }), new object[0]);
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

        /// <summary>
        /// 开始上传文件
        /// </summary>
        /// <param name="ftpHelper">ftp辅助对象(针对ftp的相关操作)</param>
        /// <param name="amrAudioFullName">语音文件地址</param>
        /// <param name="amrAudioName">语音名称</param>
        /// <param name="guid">序列号</param>
        public void BeginUpload(FtpManage ftpHelper, string amrAudioFullName, string amrAudioName, int guid)
        {
            //将语音文件上传到服务器
            ftpHelper.UploadFtp(amrAudioFullName, amrAudioName, IMMCodeEnterEntity.ConferenceFtpWebAddressFront + IMMCodeEnterEntity.FtpServerceAudioName, "/",
                IMMCodeEnterEntity.FtpUserName, IMMCodeEnterEntity.FtpPassword, delegate(long Length, double progress)
                {
                }, delegate(System.Exception error, bool result)
                {
                    //this.wave.StreamDispose();

                    //可以再次执行录音、语音转换
                    this.isInTransfer = false;

                    if (File.Exists(this.wave.SavedFile))
                    {
                        File.Delete(this.wave.SavedFile);
                    }
                    ModelManage.ConferenceAudio.UploadAudioCompleate(IMMCodeEnterEntity.ConferenceName, new ConferenceAudioItemTransferEntity() { Guid = guid }, new Action<bool>((isSuccessed) =>
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
                //this.wave.StreamDispose();
                //this.wave.Stop();
                //this.isInTransfer = false;
                ////语音转换按钮设置为转换的样式（旋转）
                //this.btnAudioTransfer.Style = btnAudioStyle;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 导出文件

        /// <summary>
        /// 导出PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(IMMCodeEnterEntity.PdfTransferAppName))
                {
                    string fileTitle = IMMCodeEnterEntity.ConferenceName + this.title_introduce;
                    string fileNameWord = fileTitle + ".html";
                    string FileFullName = IMMCodeEnterEntity.FileRoot + "\\" + fileNameWord;
                    string htmlData = this.BuildHtml(fileTitle);
                    FileManage.CreateWPFile_Html(FileFullName, htmlData);

                    string fileNamePDF = fileTitle + ".PDF";
                    FileManage.CreatePDF(FileFullName, fileNamePDF, IMMCodeEnterEntity.PdfTransferAppName);
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

        /// <summary>
        /// 导出Word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnExportWord_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileTitle = IMMCodeEnterEntity.ConferenceName + this.title_introduce;
                string fileName = fileTitle + ".doc";

                string htmlData = this.BuildHtml(fileTitle);
                FileManage.CreateWord(fileName, htmlData);
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
        /// 创建文件所需字符集
        /// </summary>
        public string BuildHtml(string fileTitle)
        {
            string htmlData = string.Empty;
            try
            {
                string title = "<p class=MsoNormal><span style='font-family:宋体;font-size:30px;line-height:200%'>" +
                    fileTitle + "</span></p>";

                DateTime dateTime = default(DateTime);

                StringBuilder builder = new StringBuilder();
                foreach (var item in ConferenceAudioItemList)
                {
                    ConferenceAudioItemTransferEntity tansferItem = item.AcademicReviewItemTransferEntity;
                    DateTime time = Convert.ToDateTime(tansferItem.MessageSendDate);
                    if (time > dateTime)
                    {
                        builder.Append("<p class=MsoNormal><span style='font-family:宋体;font-size:20px;text-align:center;line-height:150%'>");
                        builder.Append(tansferItem.MessageSendDate);
                        builder.Append("</span></p>");
                        dateTime = time;
                    }
                    builder.Append("<p class=MsoNormal>");
                    builder.Append("<span style='font-family:宋体;color:#00B0F0'>");
                    builder.Append(tansferItem.MessageSendName + "(");
                    builder.Append("</span>");
                    builder.Append("<span lang=EN-US style='color:#00B0F0'>");
                    builder.Append(tansferItem.AddAuthor);
                    builder.Append("</span>");
                    builder.Append("<span style='font-family:宋体;color:#00B0F0'>");
                    builder.Append("):");
                    builder.Append("</span>");
                    builder.Append("<span lang=EN-US style='color:#00B0F0'>");
                    builder.Append(tansferItem.MessageSendTime);
                    builder.Append("</span></p>");
                    builder.Append("<p class=MsoNormal style='text-indent:10.5pt;line-height:150%'>");
                    builder.Append("<span style='font-family:宋体'>");
                    builder.Append(tansferItem.AudioMessage);
                    builder.Append("</span></p>");
                }
                htmlData = this.htmlPart1 + title + builder.ToString() + this.htmlPart2;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {
            }
            return htmlData;
        }

        /// <summary>
        /// 导出文件前半部分初始化
        /// </summary>
        public void HtmlPart1Init()
        {
            try
            {
                if (string.IsNullOrEmpty(this.htmlPart1))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("<html>");
                    builder.Append("<head>");
                    builder.Append("<meta http-equiv=Content-Type content='text/html; charset=utf-8'>");
                    builder.Append("<meta name=Generator content='Microsoft Word 15 (filtered)'>");
                    builder.Append("<style>");
                    builder.Append("<!--");
                    builder.Append("/* Font Definitions */");
                    builder.Append(" @font-face");
                    builder.Append("	{font-family:宋体;");
                    builder.Append("	panose-1:2 1 6 0 3 1 1 1 1 1;}");
                    builder.Append("@font-face");
                    builder.Append("	{font-family:'Cambria Math';");
                    builder.Append("	panose-1:2 4 5 3 5 4 6 3 2 4;}");
                    builder.Append("@font-face");
                    builder.Append("	{font-family:Calibri;");
                    builder.Append("	panose-1:2 15 5 2 2 2 4 3 2 4;}");
                    builder.Append("@font-face");
                    builder.Append("	{font-family:'@宋体';");
                    builder.Append("	panose-1:2 1 6 0 3 1 1 1 1 1;}");
                    builder.Append(" /* Style Definitions */");
                    builder.Append(" p.MsoNormal, li.MsoNormal, div.MsoNormal");
                    builder.Append("	{margin:0cm;");
                    builder.Append("	margin-bottom:.0001pt;");
                    builder.Append("	text-align:justify;");
                    builder.Append("	text-justify:inter-ideograph;");
                    builder.Append("	font-size:10.5pt;");
                    builder.Append("	font-family:'Calibri',sans-serif;}");
                    builder.Append(".MsoChpDefault");
                    builder.Append("	{font-family:'Calibri',sans-serif;}");
                    builder.Append(" /* Page Definitions */");
                    builder.Append(" @page WordSection1");
                    builder.Append("	{size:595.3pt 841.9pt;");
                    builder.Append("	margin:72.0pt 90.0pt 72.0pt 90.0pt;");
                    builder.Append("	layout-grid:15.6pt;}");
                    builder.Append("div.WordSection1");
                    builder.Append("	{page:WordSection1;}");
                    builder.Append("-->");
                    builder.Append("</style>");
                    builder.Append("</head>");
                    builder.Append("<body lang=ZH-CN style='text-justify-trim:punctuation'>");
                    builder.Append("<div class=WordSection1 style='layout-grid:15.6pt'>");
                    htmlPart1 = builder.ToString();
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

        /// <summary>
        /// 导出文件后半部分初始化
        /// </summary>
        public void htmlPart2Init()
        {
            try
            {
                if (string.IsNullOrEmpty(this.htmlPart1))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("</div>");
                    builder.Append("</body>");
                    builder.Append("</html>");
                    htmlPart2 = builder.ToString();
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
                //if (Conference.MainWindow.MainPageInstance.QRWindow.Visibility == System.Windows.Visibility.Collapsed)
                //{
                //    Conference.MainWindow.MainPageInstance.QRWindow.Visibility = System.Windows.Visibility.Visible;
                //    //显示二维码窗体
                //    Conference.MainWindow.MainPageInstance.QRWindow.Show();
                //}
                //else
                //{
                //    Conference.MainWindow.MainPageInstance.QRWindow.Visibility = System.Windows.Visibility.Collapsed;
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
                //是否允许滚动
                ConferenceAudio_View.CanScroll = true;
                //是否可以进行初始化
                ConferenceAudio_View.GoIntoInit = true;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}
