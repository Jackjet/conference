using Conference.Common;
using Conference.View.AppcationTool;
using Conference.View.Chair;
using Conference.View.Setting;
using Conference.View.Space;
using Conference.View.Tool;
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
using vy = System.Windows.Visibility;

namespace Conference.Page
{
    public partial class MainPageBase : UserControlBase
    {
        #region 主持人结束甩屏窗体

        /// <summary>
        /// 主持人结束甩屏窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AppSyncDataAccept_Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                //设置刷屏窗体为null
                this.AppSyncDataAcceptWindow = null;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
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
                if (this.ToolCmWindow != null && this.ToolCmWindow.Visibility == vy.Visible)
                {
                    //工具箱弹出框显示
                    this.ToolCmWindow.Visibility = vy.Collapsed;
                }

                this.ConversationM.HidenAllResourceWindow();
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
