using ConferenceCommon.AppContainHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.ProcessHelper;
using Microsoft.Lync.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Conference_Conversation.Common
{
    public partial class LyncHelper
    {
        #region 隐藏托盘图标

        /// <summary>
        /// lync程序环境设置（事件、状态、原生态界面抑制、注册表、标示）
        /// </summary>
        public static void SetLyncAplicationEnviroment(Action containCompleateCallBack)
        {
            try
            {
                //获取lync进程
                Process[] processes = Process.GetProcessesByName("Lync");
                if (processes.Count() > 0)
                {
                    IntPtr handle = ProcessManage.GetMainWindowHandle(processes[0]);
                    if (handle.ToInt32() > 0)
                    {
                        //设置lync常用对象
                        SettingLyncObject();

                        if (ConversationCodeEnterEntity.lyncClient != null)
                        {
                            #region 注册Lync事件

                            //lync状态更改事件
                            ConversationCodeEnterEntity.lyncClient.StateChanged -= lyncClient_StateChanged;
                            //lync状态更改事件
                            ConversationCodeEnterEntity.lyncClient.StateChanged += lyncClient_StateChanged;

                            #endregion

                            #region lync初始化

                            //首先将捕获到的lync实例进行一次初始化加载
                            if (ConversationCodeEnterEntity.lyncClient.State == ClientState.Uninitialized)
                            {
                                //lync客户端初始化
                                ConversationCodeEnterEntity.lyncClient.BeginInitialize(null, null);
                            }

                            //签入
                            if (ConversationCodeEnterEntity.lyncClient.State == ClientState.SigningIn || ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedIn)
                            {
                                //先签出（lync控制）
                                ConversationCodeEnterEntity.lyncClient.BeginSignOut(null, null);
                            }
                            if (ConversationCodeEnterEntity.lyncClient.State == ClientState.SignedOut)
                            {             
                                //是否可以签入回调事件
                                if (CanSiginedCallBack != null)
                                {
                                    CanSiginedCallBack();
                                }
                            }

                            #endregion

                            #region 程序相关设置

                            WindowHide.SetTrayIconVisible("Lync", false);

                            ////将lync的原生态的主窗体封装起来
                            APPContainManage.APP_Conatain(handle);

                            containCompleateCallBack();

                            #endregion
                        }
                    }
                    else
                    {
                        LyncHelper.SetLyncAplicationEnviroment(containCompleateCallBack);
                    }
                }
                else
                {
                    LyncHelper.SetLyncAplicationEnviroment(containCompleateCallBack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion

        #region 设置lync常用对象

        /// <summary>
        /// 设置lync常用对象
        /// </summary>
        public static void SettingLyncObject()
        {
            try
            {
                //获取LYNC客户端
                ConversationCodeEnterEntity.lyncClient = LyncClient.GetClient();
                //联系人管理
                ConversationCodeEnterEntity.contactManager = ConversationCodeEnterEntity.lyncClient.ContactManager;
                //会话管理
                ConversationCodeEnterEntity.conversationManager = ConversationCodeEnterEntity.lyncClient.ConversationManager;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(LyncHelper), ex);
            }
            finally
            {

            }
        }

        #endregion
    }
}
