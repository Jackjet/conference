using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityHelper.LyncConversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Services;

namespace ConferenceWeb
{
    /// <summary>
    /// ConferenceLyncConversationWebservice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceLyncConversationWebservice : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 线程锁辅助对象(填充服务器的数据)
        /// </summary>
        static private object objLyncConversationSync = new object();

        /// <summary>
        /// 线程锁辅助对象(投影大屏幕)
        /// </summary>
        static private object objEnterBigScreen = new object();

        /// <summary>
        /// 线程锁辅助对象(包含会话)
        /// </summary>
        static private object objContainConversation = new object();

        /// <summary>
        /// 线程锁辅助对象(填充会话)
        /// </summary>
        static private object objFillConversation = new object();

        /// <summary>
        /// 线程锁辅助对象(移除会话)
        /// </summary>
        static private object objRemoveConversation = new object();

        /// <summary>
        /// 线程锁辅助对象(离开会话)
        /// </summary>
        static private object objLeaveConversation = new object();

        /// <summary>
        /// 线程锁辅助对象(PPT演示控制)
        /// </summary>
        static private object objPPTControl = new object();

        /// <summary>
        /// 线程锁辅助对象(禁止会话初始化)
        /// </summary>
        static private object objForbiddenConversationInit = new object();

        /// <summary>
        /// 线程锁辅助对象(允许会话初始化)
        /// </summary>
        static private object objAllowConversationInit = new object();


        /// <summary>
        /// 线程锁辅助对象(查看是否允许会话初始化)
        /// </summary>
        static private object objCheckConversationInit = new object();

        #endregion

        #region 静态资源

        /// <summary>
        /// 会话资源
        /// </summary>
        static private Dictionary<string, string> dicConversationList = new Dictionary<string, string>();

        /// <summary>
        /// 会话开启总闸
        /// </summary>
        static private Dictionary<string, bool> dicConversationCanInitList = new Dictionary<string, bool>();

        #endregion

        #region lync会话参会（）

        /// <summary>
        /// 填充服务器的数据（需要甩屏同步的数据）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>      
        [WebMethod]
        public void JoinConferenceMainLyncConversation(string conferenceName, LyncConversationEntity lyncConversationEntity)
        {
            //上锁,达到线程互斥作用
            lock (objLyncConversationSync)
            {
                try
                {
                    //会议名称不为空
                    if (string.IsNullOrEmpty(conferenceName)) return;

                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceName, lyncConversationEntity);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 大屏投影

        /// <summary>
        /// 投影大屏幕
        /// </summary>
        [WebMethod]
        public void InterBigScreen(string conferenceName, string sharer)
        {
            lock (objEnterBigScreen)
            {
                try
                {
                    //会议名称不为空
                    if (string.IsNullOrEmpty(conferenceName)) return;
                    BigScreenEnterEntity bigScreenEnterEntity = new BigScreenEnterEntity();
                    bigScreenEnterEntity.Sharer = sharer;
                    //实时同步(发送信息给客户端)
                    this.InformClient2(conferenceName, bigScreenEnterEntity);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region ppt演示控制

        /// <summary>
        /// ppt演示控制
        /// </summary>
        [WebMethod]
        public void PPTControl(string conferenceName, string controler)
        {
            lock (objPPTControl)
            {
                try
                {
                    //会议名称不为空
                    if (string.IsNullOrEmpty(conferenceName)) return;
                    PPTControlEntity pPTControlEntity = new PPTControlEntity();
                    pPTControlEntity.Controler = controler;
                    //实时同步(发送信息给客户端)
                    this.InformClient4(conferenceName, pPTControlEntity);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 是否包含会话

        [WebMethod]
        /// <summary>
        /// 是否包含会话
        /// </summary>
        public string ContainConversation(string conferenceName)
        {
            lock (objContainConversation)
            {
                string meetAddress = null;
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        if (dicConversationList.ContainsKey(conferenceName))
                        {
                            meetAddress = dicConversationList[conferenceName];
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return meetAddress;
            }
        }

        #endregion

        #region 填充会话

        [WebMethod]
        public void FillConversation(string conferenceName, string meetAddress)
        {
            lock (objFillConversation)
            {
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        if (!dicConversationList.ContainsKey(conferenceName))
                        {
                            dicConversationList.Add(conferenceName, meetAddress);
                        }
                    }
                }

                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 移除会话


        [WebMethod]
        public void ForceRemoveConversation(string conferenceName)
        {
            lock (objFillConversation)
            {
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        if (dicConversationList.ContainsKey(conferenceName))
                        {
                            dicConversationList.Remove(conferenceName);
                        }
                    }
                }

                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        [WebMethod]
        public void RemoveConversation(string conferenceName)
        {
            lock (objRemoveConversation)
            {
                int trueCount = 0;
                bool canRemove = false;
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        for (int i = Constant.DicInfoMeetServerSocket.Count - 1; i > -1; i--)
                        {
                            KeyValuePair<string, MeetServerSocket> itemMeet = Constant.DicInfoMeetServerSocket.ElementAt(i);
                            if (itemMeet.Key.Equals(conferenceName))
                            {
                                MeetServerSocket meetServerSocket = itemMeet.Value;

                                Dictionary<string, SocketModel> dicSocket = meetServerSocket.DicClientSocket;

                                for (int j = dicSocket.Count - 1; j > -1; j--)
                                {
                                    try
                                    {
                                        KeyValuePair<string, SocketModel> itemSocket = dicSocket.ElementAt(j);
                                        //即使有一个连着都说明还有参会人,不能去清除会话标识
                                        SocketModel socketModel = itemSocket.Value;
                                        if (Constant.ServerSendData(socketModel.Socket,10))
                                        {
                                            trueCount++;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                }
                                break;
                            }
                        }
                      
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {
                    if (trueCount > 0)
                    {
                        canRemove = false;
                    }
                    else if (trueCount == 0)
                    {
                        canRemove = true;
                    }
                    if (canRemove)
                    {
                        if (dicConversationList.ContainsKey(conferenceName))
                        {
                            dicConversationList.Remove(conferenceName);
                        }
                        if (dicConversationCanInitList.ContainsKey(conferenceName))
                        {
                            dicConversationCanInitList.Remove(conferenceName);
                        }
                    }
                }
            }
        }

        #endregion

        #region 离开会话
        [WebMethod]
        public void LeaveConversation(string conferenceName, string contactUri)
        {
            lock (objLeaveConversation)
            {
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        LeaveConversationEntity leaveConversationEntity = new LeaveConversationEntity();
                        leaveConversationEntity.ContactUri = contactUri;
                        this.InformClient3(conferenceName, leaveConversationEntity);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 禁止、允许会话初始化(进行延迟)

        /// <summary>
        /// 禁止会话初始化
        /// </summary>
        /// <param name="conferenceName"></param>
        [WebMethod]
        public void ForbiddenConversationInit(string conferenceName)
        {
            lock (objForbiddenConversationInit)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;
                    if (!dicConversationCanInitList.ContainsKey(conferenceName))
                    {
                        dicConversationCanInitList.Add(conferenceName, false);
                    }
                    else
                    {
                        dicConversationCanInitList[conferenceName] = false;
                    }

                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        /// <summary>
        /// 允许会话初始化
        /// </summary>
        /// <param name="conferenceName"></param>
        [WebMethod]
        public void AllowConversationInit(string conferenceName)
        {
            lock (objAllowConversationInit)
            {
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return;
                    if (!dicConversationCanInitList.ContainsKey(conferenceName))
                    {
                        dicConversationCanInitList.Add(conferenceName, true);
                    }
                    else
                    {
                        dicConversationCanInitList[conferenceName] = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        /// <summary>
        /// 查看是否允许会话初始化
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <returns></returns>
        [WebMethod]
        public bool CheckConversationInit(string conferenceName)
        {
            lock (objCheckConversationInit)
            {
                bool result = false;
                try
                {
                    //会议名称为null则不执行
                    if (string.IsNullOrEmpty(conferenceName)) return false;

                    if (dicConversationCanInitList.ContainsKey(conferenceName))
                    {
                        result = dicConversationCanInitList[conferenceName];
                    }
                    else
                    {
                        //它是会话参与的第一人（可以进行初始化执行）
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return result;
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(string conferenceName, LyncConversationEntity lyncConversationEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.LyncConversationSync,
                    };
                    pack.LyncConversationFlg.LyncConversationEntity = lyncConversationEntity;
                    pack.LyncConversationFlg.LyncConversationFlgType = LyncConversationFlgType.InviteContact;
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicLyncMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient2(string conferenceName, BigScreenEnterEntity bigScreenEnterEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.LyncConversationSync,
                    };
                    pack.LyncConversationFlg.BigScreenEnterEntity = bigScreenEnterEntity;
                    pack.LyncConversationFlg.LyncConversationFlgType = LyncConversationFlgType.EnterBigScreen;
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicLyncMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient3(string conferenceName, LeaveConversationEntity leaveConversationEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.LyncConversationSync,
                    };
                    pack.LyncConversationFlg.LeaveConversationEntity = leaveConversationEntity;
                    pack.LyncConversationFlg.LyncConversationFlgType = LyncConversationFlgType.LeaveConversation;
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicLyncMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient4(string conferenceName, PPTControlEntity pPTControlEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.LyncConversationSync,
                    };
                    pack.LyncConversationFlg.PPTControlEntity = pPTControlEntity;
                    pack.LyncConversationFlg.LyncConversationFlgType = LyncConversationFlgType.PPTControl;
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicLyncMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #endregion
    }
}
