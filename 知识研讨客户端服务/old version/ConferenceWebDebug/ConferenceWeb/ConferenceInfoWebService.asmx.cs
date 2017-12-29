using ConferenceWeb.Common;
using ConferenceWeb.MobilePhoneEntity;
using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityHelper.ConferenceInfo;
using ConferenceWebModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace ConferenceWeb
{
    /// <summary>
    /// ConferenceVersionWebservice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceInfoWebService : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 刷屏同步缓存数据
        /// </summary>
        static Dictionary<string, ConferenceInfoEntity> InfoSyncAppDic = new Dictionary<string, ConferenceInfoEntity>();

        /// <summary>
        /// 线程锁辅助对象（添加临时会议信息）
        /// </summary>
        static private object objGetTempInformation = new object();

        /// <summary>
        /// 线程锁辅助对象（更改临时会议信息）
        /// </summary>
        static private object objUpdateTempInformation = new object();

        /// <summary>
        /// 客户端控制
        /// </summary>
        static private object objClientControl = new object();

        /// <summary>
        /// 线程锁辅助对象（启动服务器通讯机制）
        /// </summary>
        static private object objRunServerSocket = new object();

        /// <summary>
        /// 线程锁辅助对象（填充服务器的数据）
        /// </summary>
        static private object objFillSyncServiceData = new object();

        /// <summary>
        /// 线程锁辅助对象（移除某个通讯点）
        /// </summary>
        static private object objRemoveClientSocket = new object();

        /// <summary>
        /// 线程锁辅助对象（移除某人所有通讯点）
        /// </summary>
        static private object objRemoveAllClientSocketBySomeOne = new object();


        /// <summary>
        /// 线程锁辅助对象（查看用户是否在线）
        /// </summary>
        static private object objCheckUserIsOnline = new object();

        /// <summary>
        /// 研讨客户端配置信息
        /// </summary>
        static ClientConfigEntity ClientConfigEntity = new ClientConfigEntity();

        /// <summary>
        /// 会议信息
        /// </summary>
        static string meetingInfo = string.Empty;

        /// <summary>
        /// 会议信息实体集合
        /// </summary>
        static List<ConferenceInformationEntityPC> ConferenceInformationEntityPCList = new List<ConferenceInformationEntityPC>();

        /// <summary>
        /// 预定会议信息实体集合
        /// </summary>
        static List<ConferenceInformationEntityPC> ReservationConferenceInformationEntityPCList = null;

        ///// <summary>
        ///// 保持服务运行状态
        ///// </summary>
        //static System.Timers.Timer KeepAliveTimer =null;

        #endregion

        #region 获取研讨客户端配置信息

        [WebMethod]
        /// <summary>
        /// 获取研讨客户端配置信息
        /// </summary>
        public ClientConfigEntity GetClientConfigInformation()
        {
            ClientConfigEntity clientConfigEntity = null;
            try
            {
                if (ClientConfigEntity != null)
                {
                    //获取客户端配置信息
                    clientConfigEntity = ClientConfigEntity;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            return clientConfigEntity;
        }

        #endregion

        #region 添加临时会议信息

        /// <summary>
        /// 添加临时会议信息
        /// </summary>
        /// <returns>返回所有会议信息</returns>
        [WebMethod]
        public List<ConferenceInformationEntityPC> GetTempInformation()
        {
            //上锁,达到线程互斥作用
            lock (objGetTempInformation)
            {
                try
                {
                    if (Constant.IsNeedReservationInfo && ReservationConferenceInformationEntityPCList == null)
                    {
                        //bool getReservationSuccessed = false;
                        ModelManage.ClientInit(Constant.RevertWebServiceUri, Constant.UserDoaminPart1Name, Constant.ReservationLoginUser, Constant.ReservationLoginPwd);

                        string info = ModelManage.ConferenceWebInfo.GetReservationConferenceInfo();
                        if (!string.IsNullOrEmpty(info))
                        {
                            ReservationConferenceInformationEntityPCList = JsonManage.JsonToEntity<ConferenceInformationEntityPC>(info, ',');
                            if (ReservationConferenceInformationEntityPCList != null && ReservationConferenceInformationEntityPCList.Count > 0)
                            {
                                //添加预定会议信息
                                ConferenceInformationEntityPCList.AddRange(ReservationConferenceInformationEntityPCList);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(meetingInfo))
                    {
                        //获取webservice路径
                        string strLocal = this.Server.MapPath(".");

                        //通过文件流将音频文件转为字节数组
                        using (System.IO.FileStream fileStream = new System.IO.FileStream(strLocal + @"\" + Constant.MeetFileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.Delete))
                        {
                            //流读取器
                            StreamReader sr = new StreamReader(fileStream);
                            //读取流中的信息
                            meetingInfo = sr.ReadToEnd();
                            //去掉换行字符
                            meetingInfo = meetingInfo.Replace("\r\n", string.Empty);
                        }
                        ConferenceInformationEntityPCList.AddRange(JsonManage.JsonToEntity<ConferenceInformationEntityPC>(meetingInfo, ','));
                    }

                    //if(KeepAliveTimer == null)
                    //{
                    //    KeepAliveTimer = new System.Timers.Timer();
                    //    KeepAliveTimer.Elapsed += KeepAliveTimer_Elapsed;
                    //    KeepAliveTimer.Interval = 30000;
                    //    KeepAliveTimer.Start();
                    //}
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {

                }
                return ConferenceInformationEntityPCList;
            }
        }

        //void KeepAliveTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{

        //}

        #endregion

        #region 清除缓存（会议信息）

        [WebMethod]
        public void ClearTempConfig(int type)
        {
            try
            {
                if (type == 0)
                {
                    ConferenceInformationEntityPCList = null;
                }
                else if (type == 1)
                {
                    ReservationConferenceInformationEntityPCList = null;
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

        #region 更改临时会议信息

        /// <summary>
        /// 更改临时会议信息
        /// </summary>
        [WebMethod]
        public void UpdateTempInformation(string conferenceName, bool isSimpleModel, bool isEducationModel)
        {
            //上锁,达到线程互斥作用
            lock (objUpdateTempInformation)
            {
                try
                {
                    if (ConferenceInformationEntityPCList != null)
                    {
                        List<ConferenceInformationEntityPC> ConferenceInfoEntityPCList = ConferenceInformationEntityPCList.Where(Item => Item.MeetingName.Equals(conferenceName)).ToList<ConferenceInformationEntityPC>();
                        if (ConferenceInfoEntityPCList != null && ConferenceInfoEntityPCList.Count > 0)
                        {
                            ConferenceInfoEntityPCList[0].SimpleMode = isSimpleModel;
                            ConferenceInfoEntityPCList[0].EducationMode = isEducationModel;

                            ConferenceInfoTypeChangeEntity conferenceInfoTypeChangeEntity = new ConferenceInfoTypeChangeEntity()
                                {
                                    IsEducationModel = isEducationModel,
                                    IsSimpleModel = isSimpleModel,
                                };

                            this.InformClient2(conferenceName, conferenceInfoTypeChangeEntity);
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
        }

        #endregion

        #region 客户端命令

        [WebMethod]
        public void ClientControl(string conferenceName, string commander, ClientControlType ClientControlType)
        {
            //上锁,达到线程互斥作用
            lock (objClientControl)
            {
                try
                {
                    ConferenceClientControlEntity ConferenceClientControlEntity = new ConferenceClientControlEntity();
                    ConferenceClientControlEntity.Sharer = commander;
                    ConferenceClientControlEntity.ClientControlType = ClientControlType;

                    this.InformClient3(conferenceName, ConferenceClientControlEntity);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {

                }
            }
        }

        #endregion

        #region 启动服务器通讯机制
        /// <summary>
        /// 启动服务器通讯机制
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回会议的通讯端口</returns>        
        [WebMethod]
        public PortTypeEntity RunServerSocket(string conferenceName, ConferenceClientAcceptType conferenceClientAcceptType)
        {
            //上锁,达到线程互斥作用
            lock (objRunServerSocket)
            {
                //返回会议端口
                PortTypeEntity portTypeEntity = new PortTypeEntity();
                try
                {
                    //服务器通讯机制启动
                    int MeetPort = Constant.ServerSocketInitAndRetunPort(conferenceName, conferenceClientAcceptType);
                    //服务端口
                    portTypeEntity.ServerPoint = MeetPort;
                    //服务类型
                    portTypeEntity.conferenceClientAcceptType = conferenceClientAcceptType;
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {

                }
                return portTypeEntity;
            }
        }

        #endregion

        #region 填充服务器的数据（需要刷屏同步的数据）

        /// <summary>
        /// 填充服务器的数据（需要刷屏同步的数据）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="sharer">共享人</param>
        /// <param name="pageType">会议页面类型</param>
        [WebMethod]
        public void FillSyncServiceData(string conferenceName, string sharer, ConferencePageType pageType)
        {
            //上锁,达到线程互斥作用
            lock (objFillSyncServiceData)
            {
                try
                {
                    //会议名称不为空
                    if (string.IsNullOrEmpty(conferenceName)) return;

                    //查看缓存中是否包含该会议名称
                    if (InfoSyncAppDic.ContainsKey(conferenceName))
                    {
                        //有则取出该会议临时存储的word路径                  
                        InfoSyncAppDic[conferenceName].Sharer = sharer;
                        InfoSyncAppDic[conferenceName].ConferencePageType = pageType;
                    }
                    else
                    {
                        //若没有，则记录会议并绑定相应的刷屏数据
                        InfoSyncAppDic.Add(conferenceName, new ConferenceInfoEntity() { Sharer = sharer, ConferencePageType = pageType });
                    }
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceName, InfoSyncAppDic[conferenceName]);
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
        public void InformClient(string conferenceName, ConferenceInfoEntity conferenceInfoEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //缓存数据包含该会议
                    if (InfoSyncAppDic.ContainsKey(conferenceName))
                    {
                        ConferenceInfoFlg conferenceInfoFlg = new ConferenceInfoFlg();
                        conferenceInfoFlg.ConferenceInfoEntity = conferenceInfoEntity;
                        conferenceInfoFlg.ConferenceInfoFlgType = ConferenceInfoFlgType.ConferenceInfoEntity;
                        //生成一个数据包（文件甩屏）
                        PackageBase pack = new PackageBase()
                        {
                            ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceInfoSync,
                            ConferenceInfoFlg = conferenceInfoFlg
                        };
                        //会议通讯节点信息发送管理中心
                        Constant.SendClientCenterManage(Constant.DicInfoMeetServerSocket, conferenceName, pack);
                    }
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
        public void InformClient2(string conferenceName, ConferenceInfoTypeChangeEntity confecrenceInfoTypeChangeEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    ConferenceInfoFlg conferenceInfoFlg = new ConferenceInfoFlg();
                    conferenceInfoFlg.ConferenceInfoTypeChangeEntity = confecrenceInfoTypeChangeEntity;
                    conferenceInfoFlg.ConferenceInfoFlgType = ConferenceInfoFlgType.ConferenceInfoTypeChangeEntity;
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceInfoSync,
                        ConferenceInfoFlg = conferenceInfoFlg
                    };
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicInfoMeetServerSocket, conferenceName, pack);
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
        public void InformClient3(string conferenceName, ConferenceClientControlEntity conferenceClientControlEntity)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    ConferenceInfoFlg conferenceInfoFlg = new ConferenceInfoFlg();
                    conferenceInfoFlg.ConferenceClientControlEntity = conferenceClientControlEntity;
                    conferenceInfoFlg.ConferenceInfoFlgType = ConferenceInfoFlgType.ConferenceClientControlEntity;
                    //生成一个数据包（文件甩屏）
                    PackageBase pack = new PackageBase()
                    {
                        ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceInfoSync,
                        ConferenceInfoFlg = conferenceInfoFlg
                    };
                    //会议通讯节点信息发送管理中心
                    Constant.SendClientCenterManage(Constant.DicInfoMeetServerSocket, conferenceName, pack);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #endregion

        #region 保持会议持续性

        /// <summary>
        /// 保持会议持续性
        /// </summary>
        [WebMethod]
        public bool SetKeepAlive()
        {
            bool result = true;
            return result;
        }

        #endregion

        #region 移除某个通讯点

        /// <summary>
        /// 移除某个通讯点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="contactUrl">联系人地址【唯一值】</param>
        [WebMethod]
        public void RemoveClientSocket(string conferenceName, ConferenceClientAcceptType conferenceClientAcceptType, string contactUrl)
        {
            //上锁,达到线程互斥作用
            lock (objRemoveClientSocket)
            {
                try
                {
                    //声明一个服务集合
                    Dictionary<string, MeetServerSocket> DicmeetServerSocket = null;
                    switch (conferenceClientAcceptType)
                    {
                        case ConferenceClientAcceptType.ConferenceTree:
                            //知识树服务
                            DicmeetServerSocket = Constant.DicTreeMeetServerSocket;
                            break;
                        case ConferenceClientAcceptType.ConferenceAudio:
                            //语音服务
                            DicmeetServerSocket = Constant.DicAudioMeetServerSocket;
                            break;
                        case ConferenceClientAcceptType.ConferenceFileSync:
                            //甩屏服务
                            DicmeetServerSocket = Constant.DicFileMeetServerSocket;
                            break;
                        case ConferenceClientAcceptType.ConferenceSpaceSync:
                            //office服务
                            DicmeetServerSocket = Constant.DicSpaceMeetServerSocket;
                            break;
                        case ConferenceClientAcceptType.ConferenceInfoSync:
                            //消息服务
                            DicmeetServerSocket = Constant.DicInfoMeetServerSocket;
                            break;
                        case ConferenceClientAcceptType.LyncConversationSync:
                            //lync通讯服务
                            DicmeetServerSocket = Constant.DicLyncMeetServerSocket;
                            break;

                        case ConferenceClientAcceptType.ConferenceMatrixSync:
                            //矩阵应用通讯服务
                            DicmeetServerSocket = Constant.DicMatrixMeetServerSocket;
                            break;
                        default:
                            break;
                    }
                    //会议通讯节点集合是否包含该项
                    if (DicmeetServerSocket.ContainsKey(conferenceName))
                    {
                        //获取客户端通讯节点字典集合
                        Dictionary<string, SocketModel> dicSocket = DicmeetServerSocket[conferenceName].DicClientSocket;
                        //获取指定客户端通讯节点
                        if (dicSocket.ContainsKey(contactUrl))
                        {
                            //移除指定客户端通讯节点
                            dicSocket.Remove(contactUrl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        /// <summary>
        /// 移除某个人的所有节点
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <param name="contactUrl"></param>
        [WebMethod]
        public void RemoveAllClientSocketBySomeOne(string conferenceName, string contactUrl)
        {
            //上锁,达到线程互斥作用
            lock (objRemoveClientSocket)
            {
                try
                {
                    //会议名称不为空
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceAudio, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceFileSync, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceInfoSync, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceMatrixSync, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceSpaceSync, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceTree, contactUrl);
                        RemoveClientSocket(conferenceName, ConferenceClientAcceptType.LyncConversationSync, contactUrl);

                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 判断用户是否存在

        [WebMethod]
        /// <summary>
        /// 判断用户是否存在
        /// </summary>
        public bool CheckUserIsOnline(string Uri)
        {
            lock (objCheckUserIsOnline)
            {
                bool isOnline = false;
                try
                {
                    for (int i = Constant.DicInfoMeetServerSocket.Count - 1; i > -1; i--)
                    {
                        KeyValuePair<string, MeetServerSocket> itemMeet = Constant.DicTreeMeetServerSocket.ElementAt(i);

                        MeetServerSocket meetServerSocket = itemMeet.Value;
                        Dictionary<string, SocketModel> dicSocket = meetServerSocket.DicClientSocket;

                        if (dicSocket.ContainsKey(Uri))
                        {
                            Socket socket = dicSocket[Uri].Socket;
                            
                            if (socket.Connected && Constant.ServerSendData(socket,10))
                            {
                                isOnline = true;
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

                }
                return isOnline;
            }
        }

        #endregion

        #region 客户端实体下载辅助

        /// <summary>
        /// 客户端实体下载辅助
        /// </summary>
        [WebMethod]
        public void ClientHelper(ConferenceInfoEntity entity)
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
        }

        #endregion

        #region 手机应用

        /// <summary>
        /// 获取会议信息
        /// </summary>
        [WebMethod]
        public string GetConferenceInfoByMobile(string loginName)
        {
            string json = string.Empty;

            MessageInfo mi = new MessageInfo();
            mi.State = "0000";
            mi.Message = "";

            ConferenceInformationEntityM ConferenceInformationEntityM = new ConferenceInformationEntityM();
            try
            {
                string userName = loginName + "@" + Constant.UserDomain;
                if (!string.IsNullOrEmpty(meetingInfo))
                {
                    List<ConferenceInformationEntity> list = JsonManage.JsonToEntity<ConferenceInformationEntity>(meetingInfo, ',');

                    List<ConferenceInformationEntity> list2 = list.Where(Item => Item.JoinPeople.Contains(userName)).ToList<ConferenceInformationEntity>();

                    if (list2.Count > 0)
                    {
                        if (list2[0].JoinPeople.Contains(userName))
                        {
                            int index = list2[0].JoinPeople.IndexOf(userName);
                            //list2[0].JoinPeopleName
                            ConferenceInformationEntityM.SelfRealName = list2[0].JoinPeopleName[index];
                        }
                        ConferenceInformationEntityM.ConferenceInformationEntityList = list2;
                        ConferenceInformationEntityM.SelfAddress = userName;

                        json = CommonMethod.Serialize(ConferenceInformationEntityM);
                        mi.Result = json;
                    }
                    else
                    {
                        mi.State = "0001";
                        mi.Message = "当前用户没有所有参加的会议";
                    }

                }
                else
                {
                    mi.State = "0001";
                    mi.Message = "获取会议信息失败";
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            finally
            {

            }
            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();

            return json;
        }

        /// <summary>
        /// 移除某个通讯点
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="contactUrl">联系人地址【唯一值】</param>
        [WebMethod]
        public string RemoveClientSocketByIMM(string conferenceName, string contactUrl)
        {
            string json = string.Empty;

            MessageInfo mi = new MessageInfo();
            mi.State = "0000";
            mi.Message = "";

            //移除某个通讯节点
            this.RemoveClientSocket(conferenceName, ConferenceClientAcceptType.ConferenceAudio, "_" + contactUrl);

            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();
            return json;
        }

        /// <summary>
        /// 获取信息交流服务端口
        /// </summary>
        /// <param name="conferenceName"></param>
        /// <returns></returns>
        [WebMethod]
        public string GetIMMServerPort(string conferenceName)
        {
            string json = string.Empty;

            MessageInfo mi = new MessageInfo();
            mi.State = "0000";
            mi.Message = "";

            PortTypeEntity porTypeEntity = this.RunServerSocket(conferenceName, ConferenceClientAcceptType.ConferenceAudio);
            mi.Result = Convert.ToString(porTypeEntity.ServerPoint);

            Context.Response.ContentType = "application/json";
            Context.Response.Charset = Encoding.UTF8.ToString(); //设置字符集类型
            Context.Response.ContentEncoding = System.Text.Encoding.UTF8;
            Context.Response.Write(CommonMethod.Serialize(mi));
            Context.Response.End();
            return json;
        }

        #endregion
    }
}
