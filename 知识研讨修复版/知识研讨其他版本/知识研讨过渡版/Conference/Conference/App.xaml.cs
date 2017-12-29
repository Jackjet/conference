using Conference.Common;
using Conference.Page;
using Conference.View.Chair;
using Conference.View.MyConference;
using ConferenceCommon.CertificateHelper;
using ConferenceCommon.DetectionHelper;
using ConferenceCommon.FileHelper;
using ConferenceCommon.FireWallHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LocalServiceHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.RefreshSystemTrayHelper;
using ConferenceCommon.RegeditHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.VersionHelper;
using ConferenceModel;
using ConferenceModel.Enum;
using Microsoft.Lync.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Conference
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        #region 字段

        /// <summary>
        /// 登录窗体
        /// </summary>
        LoginWindow loginWindow = null;

        /// <summary>
        /// 环境检测完成计时器（服务连接）
        /// </summary>
        DispatcherTimer CheckLoginInitCompleatetimer = null;

        #endregion

        #region 构造函数

        public App()
        {
            try
            {
                //登录初始化配置1
                LoginEnviromentInit1();

                //创建登陆窗体
                this.loginWindow = new LoginWindow();
                //显示登陆窗体
                loginWindow.Show();

                //登录初始化配置2
                this.LoginEnviromentInit2();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(App), ex);
            }
        }

        

        #endregion

        #region 登录初始化配置

        /// <summary>
        /// 登录初始化配置1
        /// </summary>
        private void LoginEnviromentInit1()
        {
            try
            {
                //关闭指定后台进程
                ProcessManage.KillProcess("Lync");
                //lync临时显示
                WindowHide.SetTrayIconAllDsiplay("Lync");
                //消除死亡托盘图标    
                SysTray.Refresh();
                //判断当前进程是否为单例
                ProcessManage.CheckCurrentProcessIsSingleInstance(new Action(() =>
                {
                    MessageBox.Show("该程序已经在运行中", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                    //关闭程序
                    Application.Current.Shutdown(0);
                }));

                //获取lync进程
                Process[] processs = Process.GetProcessesByName("Lync");
                if (processs.Count() > 0)
                {
                    //清除lyncApp缓存文件
                    LyncManage.ClearLyncAppData();
                }

                //打开lync实例
                RegeditManage.OpenAplicationByRegedit2("Lync.exe");

                //日志初始化
                ConferenceCommon.LogHelper.LogManage.LogInit();

                //程序退出时释放lync实例
                Application.Current.Exit += Current_Exit;

                #region old solution

                //验证是否能够访问AD  (adpppds)
                //if (!DetectionManage.TestNetConnectity(Constant.TreeServiceIP))
                //{
                //    MessageBox.Show("服务器连接失败,请及时联系管理员", "操作提示", MessageBoxButton.OK, MessageBoxImage.Information);
                //    //关闭程序
                //    Application.Current.Shutdown(0);
                //}



                ////关闭指定后台进程
                //ProcessManage.KillProcess("Lync");

                ////lync临时显示
                //WindowHide.SetTrayIconAllDsiplay("Lync");

                #endregion
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
        /// 登录初始化配置2
        /// </summary>
        public void LoginEnviromentInit2()
        {
            //消除死亡托盘图标    
            //SysTray.Refresh();             
            ThreadPool.QueueUserWorkItem((o) =>
            {
                try
                {
                    //知识树服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceTreeServiceWebName, ClientModelType.ConferenceTree, null, null, null);
                    //版本更新服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceVersionWebName, ClientModelType.ConferenceVersion, null, null, null);
                    //IMM服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceAudioServiceWebName, ClientModelType.ConferenceAudio, null, null, null);
                    //文件同步服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.FileSyncWebName, ClientModelType.FileSync, null, null, null);
                    //word同步服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceWordAsyncWebName, ClientModelType.Spacesync, null, null, null);
                    //信息化同步服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceInfoWebName, ClientModelType.ConferenceInfo, null, null, null);

                    //信息化同步服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceMatrixWebName, ClientModelType.MaxtriSync, null, null, null);
                    //会话同步服务配置
                    ModelManage.ClientInit(Constant.TreeServiceAddressFront + Constant.ConferenceLyncConversationWebName, ClientModelType.LyncConversationSync, null, null, null);
                    //环境配置2（获取会议信息,持久层信息绑定【config】）
                    this.LoginEnviromentInit3();
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                };
            });
        }

        /// <summary>
        /// 环境配置2（获取会议信息,持久层信息绑定【config】）
        /// </summary>
        public void LoginEnviromentInit3()
        {
            try
            {
                if (DetectionManage.TestNetConnectity(Constant.TreeServiceIP) )
                {
                    //验证是否能够访问LYNC扩展web服务（研讨服务）
                    if (DetectionManage.IsWebServiceAvaiable(Constant.TreeServiceAddressFront + Constant.ConferenceTreeServiceWebName))
                    {
                        if (CheckLoginInitCompleatetimer!= null)
                        {
                            this.CheckLoginInitCompleatetimer.Stop();
                            this.CheckLoginInitCompleatetimer = null;
                        }
                        //获取客户端配置信息
                        ModelManage.ConferenceInfo.GetClientAppConfig(new Action<bool, ConferenceModel.ConferenceInfoWebService.ClientConfigEntity>((successed, result) =>
                        {
                            if (successed)
                            {
                                #region 配置本地环境

                                //客户端配置信息加载
                                this.AppconfigSetting(result);
                                //客户端配置信息加载2
                                this.AppconfigSetting2(result);

                                #region old solution
                                
                                ////防火墙规则添加（研讨客户端）
                                //FireManage.OpenFireWall(Constant.FireName_Conference, Constant.ApplicationFullName);
                                ////防火墙规则添加（版本更新）
                                //FireManage.OpenFireWall(Constant.FireName_ConferenceUpdate, Constant.ConferenceVersionUpdateExe);

                                ////验证并安装lync相关证书
                                //CertificationManage.SetUpCertification(Constant.CertificationSerial, Constant.Certification); 

                                //检测并开启rpc本地服务
                                //LocalServiceManage.CheckRunRpcService();

                                //设置证书（不检查发行商的证书是否吊销，不检查服务器证书吊销）
                                //RegeditManage.SetNoCheckCertificationIsRevoke();

                                //指定webbrowser版本(9.0)
                                //RegeditManage.SetWebBrowserVersion(Constant.ApplicationSingleName);

                                #endregion

                                //设置DNS
                                NetWorkAdapter.SetNetworkAdapter(Constant.DNS1);
                                //更改lync注册表
                                RegeditManage.UpdateLyncRegedit();                              
                                //确保PainFramework.dll存在系统目录
                                FileManage.CheckDebugHasTheFile(Constant.PaintFileName, Constant.PaintFileRoot);

                                #endregion

                                //获取信息成功
                                GetClientAppConfigSuccessedDealWidth();
                            }
                        }));
                    }
                }
                else
                {
                    //获取信息失败
                    GetClientAppConfigFailedDealWidth();
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
        /// 获取信息失败
        /// </summary>
        private void GetClientAppConfigFailedDealWidth()
        {
             try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.loginWindow != null)
                    {
                        this.loginWindow.txtInitializtionColor.Foreground = new SolidColorBrush(Colors.Red);
                        this.loginWindow.LoginEnviromentTip = "无法连接到服务器。。。";


                        #region 重置（先判断计时器是否已经在监测）

                        if (CheckLoginInitCompleatetimer == null)
                        {
                            TimerJob.StartRun(new Action(() =>
                            {
                                this.LoginEnviromentInit3();
                            }), 2500, out CheckLoginInitCompleatetimer);
                        }

                        #endregion
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

        /// <summary>
        /// 获取信息成功
        /// </summary>
        private void GetClientAppConfigSuccessedDealWidth()
        {
             try
            {
                this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (this.loginWindow != null)
                    {
                        //版本更新
                        ModelManage.ConferenceVersion.NeedVersionUpdate(Constant.CurrentVersion, new Action<bool, Exception>((needUpdate, error) =>
                        {
                            //调用版本更新服务无异常
                            if (error == null)
                            {
                                //是否需要更新（由服务端去判断）
                                if (needUpdate)
                                {
                                    //通过这种方式一样可以关闭程序
                                    this.loginWindow.Visibility = System.Windows.Visibility.Hidden;
                                    VersionUpdateManage.VersionUpdate(Constant.ConferenceVersionUpdateExe);
                                }
                            }
                        }));

                        this.loginWindow.InitializtionVisibility = Visibility.Collapsed;
                        this.loginWindow.CanThrow = true;
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

        #region 配置信息加载

        /// <summary>
        /// 配置信息加载2
        /// </summary>
        public void AppconfigSetting(ConferenceModel.ConferenceInfoWebService.ClientConfigEntity clientConfigEntity)
        {
            try
            {
                //首用dns
                Constant.DNS1 = clientConfigEntity.DNS1;
                //备用dns
                Constant.DNS2 = clientConfigEntity.DNS2;
                //lync名称
                Constant.LyncName = clientConfigEntity.LyncName;
                //lync证书文件名称
                Constant.Certification = Environment.CurrentDirectory + "\\" + clientConfigEntity.Certification;
                //lync证书秘钥
                Constant.CertificationSerial = clientConfigEntity.CertificationSerial;
                //域名
                Constant.UserDomain = clientConfigEntity.UserDomain;
                //域名前名称
                Constant.UserDoaminPart1Name = clientConfigEntity.UserDoaminPart1Name;
                //lync服务1IP地址
                Constant.LyncIP1 = clientConfigEntity.LyncIP1;
                //lync服务2IP地址
                Constant.LyncIP2 = clientConfigEntity.LyncIP2;
                //Sharepoint服务IP地址
                Constant.SPSiteAddressFront = clientConfigEntity.SPSiteAddressFront;
                //智慧树文件保存名称
                Constant.TreeXmlFileName = clientConfigEntity.TreeXmlFileName;
                //研讨图片文件保存名称
                Constant.TreeJpgFileName = clientConfigEntity.TreeJpgFileName;
                //会议空间文件夹 
                Constant.MeetingFolderName = clientConfigEntity.MeetingFolderName;
                //个人空间文件夹
                Constant.PesonalFolderName = clientConfigEntity.PesonalFolderName;
                //智存空间网站集
                Constant.SpaceWebSiteUri = clientConfigEntity.SpaceWebSiteUri;

                //分辨率宽度
                Constant.ScreenResulotionWidth = clientConfigEntity.ScreenResulotionWidth;
                //分辨率高度
                Constant.ScreenResulotionHeight = clientConfigEntity.ScreenResulotionHeight;
                //大屏幕名称
                //Constant.BigScreenName = clientConfigEntity.BigScreenName;              
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
        /// 配置信息加载2(包含本地信息)
        /// </summary>
        /// <param name="clientConfigEntity"></param>
        private  void AppconfigSetting2(ConferenceModel.ConferenceInfoWebService.ClientConfigEntity clientConfigEntity)
        {
            try
            {
                //本地个人笔记名称
                Constant.LocalPersonalNoteFile = clientConfigEntity.LocalPersonalNoteFile;
                //ftp服务地址
                Constant.ConferenceFtpWebAddressFront = clientConfigEntity.ConferenceFtpWebAddressFront;
                //语音文件上传目录
                Constant.FtpServerceAudioName = clientConfigEntity.FtpServerceAudioName;
                //用户头像上传目录
                Constant.FtpServercePersonImgName = clientConfigEntity.FtpServercePersonImgName;
                //智慧树新节点默认名称
                Constant.TreeItemEmptyName = clientConfigEntity.TreeItemEmptyName;
                //录播文件存放地址
                Constant.RecordFolderName = clientConfigEntity.RecordFolderName;
                //录播文件扩展名
                Constant.RecordExtention = clientConfigEntity.RecordExtention;
                //上传的录制视频名称
                Constant.ReacordUploadFileName = clientConfigEntity.ReacordUploadFileName;
                //触摸键盘设置区域（64）
                Constant.KeyboardSettingFile_64 = clientConfigEntity.KeyboardSettingFile_64;
                //触摸键盘设置区域（32）
                Constant.KeyboardSettingFile_32 = clientConfigEntity.KeyboardSettingFile_32;
                //ftp用户
                Constant.FtpUserName = clientConfigEntity.FtpUserName;
                //ftp用户密码
                Constant.FtpPassword = clientConfigEntity.FtpPassword;

                #region 本地完成（不依赖服务获取的信息）

                string strHostName = Dns.GetHostName();
                Constant.LocalIp = System.Net.Dns.GetHostAddresses(strHostName).GetValue(1).ToString();

                //获取屏幕分辨率
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.PrimaryScreen;
                //获取宽度
                Constant.FirstRunScreenWidth = screen.Bounds.Width;
                //获取高度
                Constant.FirstRunScreenHeight = screen.Bounds.Height;

                #endregion
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

        #region 应用程序关闭事件

        /// <summary>
        /// 当前应用程序关闭时，注销该用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Current_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (this.loginWindow != null)
                {
                    this.loginWindow.Close();
                }
                //设置DNS
                NetWorkAdapter.SetNetworkAdapter(Constant.RouteIp);

                LyncHelper.LyncCloseHelper(new Action(() =>
                    {
                        if (LyncHelper.MainConversation != null)
                        {
                            ModelManage.ConferenceLyncConversation.RemoveConversation(Constant.ConferenceName, new Action<bool>((successed) =>
                            {
                            }));
                            //设置DNS
                            //NetWorkAdapter.EnableDHCP2();

                            //Conference.View.Tree.ConferenceTreeView.TittleEditControlTimer.Stop();
                            //Conference.View.IMM.ConferenceAudio_View.TittleEditControlTimer.Stop();

                            //LyncClient.GetAutomation().EndMeetNow(null);
                            LyncHelper.MainConversation.Close();
                        }
                    }));
                //系统关闭辅助
               Current_Exit_Help();               
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(App), ex);
            }
            finally
            {
                //关闭指定后台进程
                ProcessManage.KillProcess("Lync");
                //lync临时显示
                WindowHide.SetTrayIconAllDsiplay("Lync");
                //消除死亡托盘图标    
                SysTray.Refresh();
            }
        }

        /// <summary>
        /// 系统关闭辅助
        /// </summary>
        private  void Current_Exit_Help()
        {
             try
            {
                if (Conference.MainWindow.MainPageInstance != null)
                {
                    //离开坐席
                    //ChairView.LeaveSeat();
                    //离开坐席
                    MyConferenceView.LeaveSeat();
                    //离开会话
                    ModelManage.ConferenceLyncConversation.LeaveConversation(Constant.ConferenceName, Constant.SelfUri, new Action<bool>((successed) =>
                    {

                    }));

                    Conference.MainWindow.MainPageInstance.DisPoseServerSocketArray(Constant.ConferenceName, new Action<bool>((isSuccessed) =>
                    {
                    }));

                    //关闭二维码窗体
                    if (Conference.MainWindow.MainPageInstance.QRWindow != null)
                    {
                        Conference.MainWindow.MainPageInstance.QRWindow.Close();
                    }
                    if (Conference.MainWindow.MainPageInstance.ToolCmWindow != null)
                    {
                        Conference.MainWindow.MainPageInstance.ToolCmWindow.Close();
                    }
                }
                //Thread.Sleep(1000);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                //结束透明键盘程序
                this.DisposeTouchKeyBoardOpacityApplication();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(App), ex);
            }
            finally
            {

            }           
        }

        #endregion

        #region 设置键盘透明度

        /// <summary>
        /// 设置键盘透明度
        /// </summary>
        public void TouchKeyBoardOpacitySetting()
        {
            try
            {
                if (File.Exists(Constant.KeyboardSettingFile_32))
                {
                    //设置触摸键盘透明
                    ProcessManage.OpenFileByLocalAddress(Constant.KeyboardSettingFile_32);
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
        /// 结束透明键盘程序
        /// </summary>
        public void DisposeTouchKeyBoardOpacityApplication()
        {
            try
            {
                ProcessManage.KillProcess("TabTip");
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
