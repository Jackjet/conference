using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;
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

        /// <summary>
        /// 线程锁辅助对象(共享应用程序通知)
        /// </summary>
        static private object objShareApplicationResourceNotify = new object();

        #endregion

        #region 静态资源

        /// <summary>
        /// 会话资源
        /// </summary>
        static private Dictionary<int, string> dicConversationList = new Dictionary<int, string>();

        /// <summary>
        /// 会话开启总闸
        /// </summary>
        static private Dictionary<int, bool> dicConversationCanInitList = new Dictionary<int, bool>();

        /// <summary>
        /// lync应用程序共享资源
        /// </summary>
        static private Dictionary<int, LyncResourceNotify> dicLyncResourceNotify = new Dictionary<int, LyncResourceNotify>();

        #endregion

        #region lync会话参会（）

        /// <summary>
        /// 填充服务器的数据（需要甩屏同步的数据）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>      
        [WebMethod]
        public void JoinConferenceMainLyncConversation(int conferenceID, LyncConversationEntity lyncConversationEntity)
        {
            //上锁,达到线程互斥作用
            lock (objLyncConversationSync)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID==0) return;

                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID, LyncConversationFlgType.InviteContact, lyncConversationEntity);
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
        public void InterBigScreen(int conferenceID, string sharer)
        {
            lock (objEnterBigScreen)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID==0) return;
                    BigScreenEnterEntity bigScreenEnterEntity = new BigScreenEnterEntity();
                    bigScreenEnterEntity.Sharer = sharer;
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID, LyncConversationFlgType.EnterBigScreen, bigScreenEnterEntity);
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
        public void PPTControl(int conferenceID, string controler)
        {
            lock (objPPTControl)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID==0) return;
                    PPTControlEntity pPTControlEntity = new PPTControlEntity();
                    pPTControlEntity.Controler = controler;
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID, LyncConversationFlgType.PPTControl, pPTControlEntity);
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
        public string ContainConversation(int conferenceID)
        {
            lock (objContainConversation)
            {
                string meetAddress = null;
                try
                {
                    //会议名称不为空
                    if (conferenceID!=0)
                    {
                        if (dicConversationList.ContainsKey(conferenceID))
                        {
                            meetAddress = dicConversationList[conferenceID];
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
        public void FillConversation(int conferenceID, string meetAddress)
        {
            lock (objFillConversation)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID!=0)
                    {
                        if (!dicConversationList.ContainsKey(conferenceID))
                        {
                            dicConversationList.Add(conferenceID, meetAddress);
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
        public void ForceRemoveConversation(int conferenceID)
        {
            lock (objFillConversation)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID!=0)
                    {
                        if (dicConversationList.ContainsKey(conferenceID))
                        {
                            dicConversationList.Remove(conferenceID);
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
        public void RemoveConversation(int conferenceID)
        {
            lock (objRemoveConversation)
            {
                int trueCount = 0;
                bool canRemove = false;
                try
                {
                    //会议名称不为空
                    if (conferenceID!=0)
                    {
                        for (int i = Constant.DicInfoMeetServerSocket.Count - 1; i > -1; i--)
                        {
                            KeyValuePair<int, MeetServerSocket> itemMeet = Constant.DicInfoMeetServerSocket.ElementAt(i);
                            if (itemMeet.Key.Equals(conferenceID))
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
                                        if (Constant.ServerSendData(socketModel.Socket, 10))
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
                        if (dicConversationList.ContainsKey(conferenceID))
                        {
                            dicConversationList.Remove(conferenceID);
                        }
                        if (dicConversationCanInitList.ContainsKey(conferenceID))
                        {
                            dicConversationCanInitList.Remove(conferenceID);
                        }
                    }
                }
            }
        }

        #endregion

        #region 离开会话
        [WebMethod]
        public void LeaveConversation(int conferenceID, string contactUri)
        {
            lock (objLeaveConversation)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID!=0)
                    {
                        LeaveConversationEntity leaveConversationEntity = new LeaveConversationEntity();
                        leaveConversationEntity.ContactUri = contactUri;
                        this.InformClient(conferenceID, LyncConversationFlgType.LeaveConversation, leaveConversationEntity);
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
        public void ForbiddenConversationInit(int conferenceID)
        {
            lock (objForbiddenConversationInit)
            {
                try
                {
                    //会议名称为null则不执行
                    if (conferenceID==0) return;
                    if (!dicConversationCanInitList.ContainsKey(conferenceID))
                    {
                        dicConversationCanInitList.Add(conferenceID, false);
                    }
                    else
                    {
                        dicConversationCanInitList[conferenceID] = false;
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
        public void AllowConversationInit(int conferenceID)
        {
            lock (objAllowConversationInit)
            {
                try
                {
                    //会议名称为null则不执行
                    if (conferenceID==0) return;
                    if (!dicConversationCanInitList.ContainsKey(conferenceID))
                    {
                        dicConversationCanInitList.Add(conferenceID, true);
                    }
                    else
                    {
                        dicConversationCanInitList[conferenceID] = true;
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
        public bool CheckConversationInit(int conferenceID)
        {
            lock (objCheckConversationInit)
            {
                bool result = false;
                try
                {
                    //会议名称为null则不执行
                    if (conferenceID==0) return false;

                    if (dicConversationCanInitList.ContainsKey(conferenceID))
                    {
                        result = dicConversationCanInitList[conferenceID];
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

        #region 共享文件通知

        /// <summary>
        /// 共享文件通知
        /// </summary>
        [WebMethod]
        public void ShareApplicationResourceNotify(int conferenceID, LyncResourceNotify lyncResourceNotify)
        {
            lock (objShareApplicationResourceNotify)
            {
                try
                {
                    if (dicLyncResourceNotify.ContainsKey(conferenceID))
                    {
                        dicLyncResourceNotify[conferenceID] = lyncResourceNotify;
                    }
                    else
                    {
                        dicLyncResourceNotify.Add(conferenceID, lyncResourceNotify);
                    }
                    this.InformClient(conferenceID, LyncConversationFlgType.LyncResourceNotify, lyncResourceNotify);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)
                     
        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(int conferenceID, LyncConversationFlgType lyncConversationFlgType, LyncEntityBase lyncEntityBase)
        {
            try
            {
                //会议名称不为空
                if (conferenceID!=0)
                {
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase();
                    pack.ConferenceClientAcceptType = ConferenceClientAcceptType.LyncConversationSync;
                    pack.LyncConversationFlg.LyncConversationFlgType = lyncConversationFlgType;
                    switch (lyncConversationFlgType)
                    {
                        case LyncConversationFlgType.InviteContact:
                            pack.LyncConversationFlg.LyncConversationEntity = lyncEntityBase as LyncConversationEntity;
                            break;
                        case LyncConversationFlgType.EnterBigScreen:
                            pack.LyncConversationFlg.BigScreenEnterEntity = lyncEntityBase as BigScreenEnterEntity;
                            break;
                        case LyncConversationFlgType.LeaveConversation:
                            pack.LyncConversationFlg.LeaveConversationEntity = lyncEntityBase as LeaveConversationEntity;
                            break;
                        case LyncConversationFlgType.PPTControl:
                            pack.LyncConversationFlg.PPTControlEntity = lyncEntityBase as PPTControlEntity;
                            break;
                        case LyncConversationFlgType.LyncResourceNotify:
                            pack.LyncConversationFlg.LyncResourceNotify = lyncEntityBase as LyncResourceNotify;
                            break;

                        default:
                            break;
                    }
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicLyncMeetServerSocket, conferenceID, pack);
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
