using Conference.Common;
using Conference.View.AppcationTool;
using Conference.View.Chair;
using Conference.View.ConferenceRoom;
using Conference.View.IMM;
using Conference.View.MyConference;
using Conference.View.Note;
using Conference.View.Resource;
using Conference.View.Setting;
using Conference.View.Space;
using Conference.View.Studiom;
using Conference.View.Tool;
using Conference.View.U_Disk;
using Conference.View.WebBrowser;
using Conference_Tree;
using ConferenceCommon.AppContainHelper;
using ConferenceCommon.ApplicationHelper;
using ConferenceCommon.DetectionHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.VersionHelper;
using ConferenceCommon.WebHelper;
using ConferenceCommon.WPFHelper;
using ConferenceModel;
using ConferenceModel.ConferenceInfoWebService;
using ConferenceModel.FileSyncAppPoolWebService;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Threading;

namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 释放之前使用的通讯节点

        /// <summary>
        /// 释放之前使用的通讯节点
        /// </summary>
        public void DisPoseServerSocketArray(string conferenceName, Action<bool> compleateCallback)
        {
            try
            {
                //关闭本地套接字的连接
                this.Communication_Server_Client_Disopose();

                #region old solution

                //int i = 0;

                ////移除知识树通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceTree, Constant.SelfUri, null);

                ////禁止进行通讯检测
                ////this.needCheckSocekt = false;

                ////移除语音通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceAudio, Constant.SelfUri, null);
                ////移除消息通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceInfoSync, Constant.SelfUri, null);
                ////移除甩屏通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceFileSync, Constant.SelfUri, null);
                ////移除lync会话通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.LyncConversationSync, Constant.SelfUri, null);
                ////移除智存空间通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceSpaceSync, Constant.SelfUri, null);
                ////移除矩阵应用通讯节点
                //ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, ConferenceModel.ConferenceInfoWebService.ConferenceClientAcceptType.ConferenceMatrixSync, Constant.SelfUri, new Action<bool>((successed) =>
                //{
                //    i++;
                //    if (i == 6)
                //    {
                //        if (compleateCallback != null)
                //        {
                //            //函数回调
                //            compleateCallback(true);
                //        }
                //    }
                //}));

                #endregion

                //移除矩阵应用通讯节点
                ModelManage.ConferenceInfo.RemoveCurrentUser_AllClientSocket(conferenceName, Constant.SelfUri, new Action<bool>((successed) =>
                {
                    if (successed)
                    {
                        if (compleateCallback != null)
                        {
                            //函数回调
                            compleateCallback(true);
                        }
                    }
                }));

            }
            catch (Exception ex)
            {
                if (compleateCallback != null)
                {
                    //函数回调
                    compleateCallback(false);
                }
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 主页面点击事件

        /// <summary>
        /// 主页面点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void MainPage_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //判断工具箱弹出框
                if (this.ToolCmWindow != null && this.ToolCmWindow.Visibility == System.Windows.Visibility.Visible)
                {
                    //工具箱弹出框显示
                    this.ToolCmWindow.Visibility = System.Windows.Visibility.Collapsed;
                }

                if (this.QRWindow != null && this.QRWindow.Visibility == System.Windows.Visibility.Visible)
                {
                    //工具箱弹出框显示
                    this.QRWindow.Visibility = System.Windows.Visibility.Collapsed;
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

        #region 检测通讯连接

        /// <summary>
        /// 检测通讯连接
        /// </summary>
        protected void CheckAndRepairClientSocekt(Action<NetWorkErrTipType> CallBack)
        {
            try
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(2.5);
                timer.Tick += (object sender, EventArgs e) =>
                {
                    ThreadPool.QueueUserWorkItem((o) =>
          {
              try
              {
                  if (DetectionManage.TestNetConnectity(Constant.RouteIp))
                  {
                      if (DetectionManage.TestNetConnectity(Constant.TreeServiceIP))
                      {
                          if (DetectionManage.IsWebServiceAvaiable(Constant.TreeServiceAddressFront + Constant.ConferenceTreeServiceWebName))
                          {
                              this.Dispatcher.BeginInvoke(new Action(() =>
                                  {
                                      CallBack(NetWorkErrTipType.Normal);
                                  }));

                              //知识树通讯修复
                              this.Repair_ClientSocket(this.TreeClientSocket, ConferenceClientAcceptType.ConferenceTree, ViewSelectedItemEnum.Tree);
                              //信息交流通讯修复
                              this.Repair_ClientSocket(this.AudioClientSocket, ConferenceClientAcceptType.ConferenceAudio, ViewSelectedItemEnum.IMM);
                              //文件通讯修复
                              this.Repair_ClientSocket(this.FileClientSocket, ConferenceClientAcceptType.ConferenceFileSync, ViewSelectedItemEnum.None);
                              //信息通讯修复
                              this.Repair_ClientSocket(this.InfoClientSocket, ConferenceClientAcceptType.ConferenceInfoSync, ViewSelectedItemEnum.None);
                              //矩阵通讯修复
                              this.Repair_ClientSocket(this.MatrixClientSocket, ConferenceClientAcceptType.ConferenceMatrixSync, ViewSelectedItemEnum.None);
                              //lync通讯修复
                              this.Repair_ClientSocket(this.LyncClientSocket, ConferenceClientAcceptType.LyncConversationSync, ViewSelectedItemEnum.None);
                              //office通讯修复
                              this.Repair_ClientSocket(this.SpaceClientSocket, ConferenceClientAcceptType.ConferenceSpaceSync, ViewSelectedItemEnum.None);

                              if (this.isDisconnectedBefore)
                              {
                                  this.Dispatcher.BeginInvoke(new Action(() =>
                                      {
                                          if (MainWindow.mainWindow.Top != 0)
                                          {
                                              MainWindow.mainWindow.Top = 0;
                                          }
                                          if (MainWindow.mainWindow.Left != 0)
                                          {
                                              MainWindow.mainWindow.Left = 0;
                                          }

                                          //强制导航到资源共享
                                          MainWindow.MainPageInstance.ForceToNavicate(this.ViewSelectedItemEnum);
                                      }));
                              }
                              this.isDisconnectedBefore = false;
                          }
                          else
                          {
                              this.Dispatcher.BeginInvoke(new Action(() =>
                                 {
                                     CallBack(NetWorkErrTipType.ConnectedWebServiceFailed);
                                     this.isDisconnectedBefore = true;
                                 }));
                          }
                      }
                      else
                      {
                          this.Dispatcher.BeginInvoke(new Action(() =>
                          {
                              CallBack(NetWorkErrTipType.ConnectedServiceFailed);
                              this.isDisconnectedBefore = true;
                          }));
                      }
                  }
                  else
                  {
                      this.Dispatcher.BeginInvoke(new Action(() =>
                      {
                          CallBack(NetWorkErrTipType.ConnectedRouteFailed);
                          this.isDisconnectedBefore = true;
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
          });
                };
                timer.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
        }

        public void Repair_ClientSocket(ClientSocket clientSocket, ConferenceClientAcceptType conferenceClientAcceptType, ViewSelectedItemEnum viewSelectedItemEnum)
        {
            try
            {
                if (clientSocket != null && clientSocket._clientSocket != null)
                {
                    if (clientSocket._clientSocket.Poll(10, SelectMode.SelectRead))
                    {
                        //移除知识树通讯节点
                        ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, conferenceClientAcceptType, Constant.SelfUri, null);
                        //获取知识树服务端口
                        ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, conferenceClientAcceptType, new Action<bool, PortTypeEntity>((Successed, portTypeEntity) =>
                        {
                            //通知服务端进行套接字的收集
                            this.Communication_Server_Client(clientSocket, portTypeEntity.ServerPoint);
                            if (viewSelectedItemEnum != ConferenceCommon.EnumHelper.ViewSelectedItemEnum.None)
                            {
                                this.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //刷新页面
                                    MainWindow.MainPageInstance.PageReflesh(viewSelectedItemEnum);
                                }));
                            }
                        }));
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

        public void RefleshBlockConnect(ConferenceClientAcceptType conferenceClientAcceptType)
        {
            try
            {
                ClientSocket clientSocket = default(ClientSocket);
                switch (conferenceClientAcceptType)
                {
                    case ConferenceClientAcceptType.ConferenceTree:
                        clientSocket = this.TreeClientSocket;
                        this.RefleshBlockConnectHelper(clientSocket, conferenceClientAcceptType);
                        break;
                    case ConferenceClientAcceptType.ConferenceAudio:
                        clientSocket = this.AudioClientSocket;
                        this.RefleshBlockConnectHelper(clientSocket, conferenceClientAcceptType);
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

        public void RefleshBlockConnectHelper(ClientSocket clientSocket, ConferenceClientAcceptType conferenceClientAcceptType)
        {
            try
            {
                //移除知识树通讯节点
                ModelManage.ConferenceInfo.RemoveSelfClientSocket(Constant.ConferenceName, conferenceClientAcceptType, Constant.SelfUri, null);
                //获取知识树服务端口
                ModelManage.ConferenceInfo.GetMeetPort(Constant.ConferenceName, conferenceClientAcceptType, new Action<bool, PortTypeEntity>((Successed, portTypeEntity) =>
                {
                    //通知服务端进行套接字的收集
                    this.Communication_Server_Client(clientSocket, portTypeEntity.ServerPoint);
                    //callBack();
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

        #region 释放资源

        /// <summary>
        /// 释放资源
        /// </summary>
        protected void UIAndSourceDispose()
        {
            try
            {
                if (this.chairView != null)
                {
                    //释放主持人页面
                    this.chairView = null;
                }

                //释放IMM页面   
                if (this.conferenceAudio_View != null)
                {
                    this.conferenceAudio_View.SessionClear();
                }
                this.conferenceAudio_View = null;

                if (this.conferenceTreeView != null)
                {
                    //释放知识树页面         
                    this.conferenceTreeView.ConferenceTreeItem = null;
                    this.conferenceTreeView = null;
                }
                ConferenceTreeItem.SessionClear();

                this.conversationM = null;

                //是否会议投票页面
                this.webBrowserView = null;

                //释放会议空间
                this.spaceView = null;

                //释放U盘传输页面
                this.u_DiskView = null;

                //二维码释放
                this.qRWindow = null;
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
