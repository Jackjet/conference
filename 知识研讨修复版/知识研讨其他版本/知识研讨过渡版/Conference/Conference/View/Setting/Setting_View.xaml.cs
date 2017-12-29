using Conference.Common;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.FileDownAndUp;
using ConferenceCommon.FileHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.RegeditHelper;
using ConferenceModel;
using ESBasic;
using Microsoft.Win32;
using OMCS;
using OMCS.Passive;
using OMCS.Passive.Audio;
using OMCS.Passive.Video;
using OMCS.Server;
using OMCS.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using ConferenceCommon.VersionHelper;
using ConferenceCommon.WPFHelper;
using Microsoft.Lync.Model;

namespace Conference.View.Setting
{
    /// <summary>
    /// 参数设置视图
    /// </summary>
    public partial class Setting_View : UserControlBase
    {
        #region 字段

        /// <summary>
        /// 需要上传的图片
        /// </summary>
        //string strUploadImg = string.Empty;

        #endregion
       
        #region 绑定属性

        Visibility messageVisibility = Visibility.Collapsed;
        /// <summary>
        /// 消息提示状态
        /// </summary>
        public Visibility MessageVisibility
        {
            get { return messageVisibility; }
            set
            {
                messageVisibility = value;
                this.OnPropertyChanged("MessageVisibility");
            }
        }

        int tabControlIndex;
        /// <summary>
        /// 选项卡索引
        /// </summary>
        public int TabControlIndex
        {
            get { return tabControlIndex; }
            set
            {
                tabControlIndex = value;
                this.OnPropertyChanged("TabControlIndex");
            }
        }

        Visibility autoLogin_Visibility = Visibility.Visible;
        /// <summary>
        /// 记住密码是否显示
        /// </summary>
        public Visibility AutoLogin_Visibility
        {
            get { return autoLogin_Visibility; }
            set
            {
                autoLogin_Visibility = value;
                this.OnPropertyChanged("AutoLogin_Visibility");
            }
        }


        #endregion

        #region 构造函数

        public Setting_View()
        {
            try
            {
                //UI加载
                InitializeComponent();

                //当前上下文设置
                this.DataContext = this;

                //加载常规参数(操作模式)
                this.CommonParameter_Load();

                #region 事件注册

                //保存事件(保存到本地)
                //this.btnSaved.Click += btnSaved_Click;
                //版本更新
                this.btnVersionUpdate.Click += btnVersionUpdate_Click;
                //切换到桌面
                this.ChangedToDesk.Click += ChangedToDesk_Click;
                ////上传文件
                //this.imgUPload.Click += imgUPload_Click;
                //选择图片
                this.btnUploadImg.Click += imgSelect_Click;

                #endregion

                if (Constant.IsMeetingPresenter)
                {
                    //显示模式切换面板
                    this.gridModelChangedPanel.Visibility = System.Windows.Visibility.Visible;
                    //显示客户端命令控制面板
                    this.gridClientControlPanel.Visibility = System.Windows.Visibility.Visible;

                    #region 事件注册（模式切换）

                    this.btnSimpleModel.Click += btnSimpleModel_Click;
                    this.btnEducationModel.Click += btnEducationModel_Click;
                    this.btnSrtandModel.Click += btnSrtandModel_Click;

                    #endregion

                    #region 事件注册(客户端控制)

                    //关闭所有参会人
                    this.btnAllClose.Click += btnAllClose_Click;
                    //强制所有参会人更新客户端
                    this.btnAllVersionUpdate.Click += btnAllVersionUpdate_Click;

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 加载常规参数

        /// <summary>
        /// 加载常规参数
        /// </summary>
        public void CommonParameter_Load()
        {
            try
            {
                //设置为不可见（上传提示）
                this.txtUPloadTip.Visibility = System.Windows.Visibility.Collapsed;
                //当前用户头像设置
                this.imgPerson.Source = new BitmapImage(new Uri(Constant.TreeServiceAddressFront + Constant.FtpServercePersonImgName + Constant.LoginUserName + ".png", UriKind.RelativeOrAbsolute));
               //当前用户名称显示
                this.txtSelfName.Text = Constant.SelfName;

                Constant.SelfCompony = Convert.ToString( Constant.lyncClient.Self.Contact .GetContactInformation(ContactInformationType.Company));
                Constant.SelfPosition = Convert.ToString(Constant.lyncClient.Self.Contact.GetContactInformation(ContactInformationType.Title));

                //当前用户公司名称显示
                this.txtCompony.Text = Constant.SelfCompony;
                //当前用户职位显示
                this.txtPosition.Text = Constant.SelfPosition;

               
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 选择图片

        /// <summary>
        /// 选择图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void imgSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                //打开选项对话框
                dialog.Filter = "图片文件(*.jpg,*.jpeg,*.png,*.gif)|*.jpeg;*.gif;*.png;*.jpg;)";
                //设置为多选
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == true)
                {
                    //需要上传的头像
                    //var file = dialog.FileName;
                    //显示待上传的图片
                    //this.localImg.Source = new BitmapImage(new Uri(file, UriKind.Absolute));

                    //绑定需要上传的文件名称
                    //this.strUploadImg = file;

                    //上传图片
                    this.imgUPload(dialog.FileName);
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

        #region 上传图片

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void imgUPload(string UploadImg)
        {
            try
            {
                if (string.IsNullOrEmpty(UploadImg)) return;
                //生成一个ftp辅助类（）
                FtpManage ftpHelper = new FtpManage();
                //头像地址
                string personImgUri = Constant.ConferenceFtpWebAddressFront + Constant.FtpServercePersonImgName;
                //删除文件
                FtpManage.DeleteFile(personImgUri, Constant.FtpUserName, Constant.FtpPassword);
                //上传头像
                ftpHelper.UploadFtp(UploadImg, Constant.LoginUserName + ".png", Constant.ConferenceFtpWebAddressFront + Constant.FtpServercePersonImgName, "/", Constant.FtpUserName, Constant.FtpPassword, delegate(long Length, double progress)
                {
                }, delegate(System.Exception error, bool result)
                {
                    if (result)
                    {
                        //使用异步委托
                        this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    //显示状态
                                    this.txtUPloadTip.Visibility = System.Windows.Visibility.Visible;
                                    //过渡一端时间隐藏提示状态
                                    TimerJob.StartRun(new Action(() =>
                                    {
                                        this.txtUPloadTip.Visibility = System.Windows.Visibility.Collapsed;
                                    }), 2000);
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

        #region 切换到桌面

        /// <summary>
        /// 切换到桌面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ChangedToDesk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //设置主窗体为最小化
                MainWindow.mainWindow.WindowState = WindowState.Minimized;
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

        #region 版本更新

        /// <summary>
        /// 版本更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnVersionUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //版本检测更新
                VersionUpdateManage.VersionUpdate(Constant.ConferenceVersionUpdateExe);
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

        #region 模式切换

        #region 切换为标准模式

        void btnSrtandModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModelManage.ConferenceInfo.ChangeClientModel(Constant.ConferenceName, false, false, new Action<bool>((successed) =>
                      {

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

        #region 切换为教育模式

        void btnEducationModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModelManage.ConferenceInfo.ChangeClientModel(Constant.ConferenceName, false, true, new Action<bool>((successed) =>
                 {

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

        #region 切换为精简模式

        void btnSimpleModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModelManage.ConferenceInfo.ChangeClientModel(Constant.ConferenceName, true, false, new Action<bool>((successed) =>
                 {

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

        #endregion

        #region 客户端统一控制区域

        #region 强制所有参会人更新客户端

        /// <summary>
        /// 强制所有参会人更新客户端
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAllVersionUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModelManage.ConferenceInfo.ClientControl(Constant.ConferenceName, Constant.SelfUri, ConferenceModel.ConferenceInfoWebService.ClientControlType.VersionUpdate, new Action<bool>((successed) =>
                    {

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

        #region 关闭所有参会人

        /// <summary>
        /// 关闭所有参会人
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnAllClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ModelManage.ConferenceInfo.ClientControl(Constant.ConferenceName, Constant.SelfUri, ConferenceModel.ConferenceInfoWebService.ClientControlType.Close, new Action<bool>((successed) =>
                {

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

        #endregion

        #region 保存事件

        /// <summary>
        /// 保存事件(保存到本地)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnSaved_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //保存到本地
                FileManage.Save_EntityInXml(Constant.SettingEntity, Constant.SettingFilePath);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion
    }
}