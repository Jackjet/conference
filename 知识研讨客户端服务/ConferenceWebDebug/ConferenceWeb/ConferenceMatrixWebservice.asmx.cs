using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ConferenceWebCommon;
using ConferenceWebCommon.EntityHelper.ConferenceMatrix;
using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;

namespace ConferenceWeb
{
    /// <summary>
    /// ConferenceMatrixWebservice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceMatrixWebservice : System.Web.Services.WebService
    {

        #region 静态字段

        /// <summary>
        /// 矩阵数据应用
        /// </summary>
        static Dictionary<int, ConferenceMatrixEntity> dicConferenceMatrixEntity = new Dictionary<int, ConferenceMatrixEntity>();

        /// <summary>
        /// 座位信息
        /// </summary>
        static Dictionary<int, List<SeatEntity>> dicSeatEntity = new Dictionary<int, List<SeatEntity>>();

        /// <summary>
        /// 矩阵切换模型
        /// </summary>
        static Dictionary<int, MaxtrixModeType> dicMaxtrixModeType = new Dictionary<int, MaxtrixModeType>();

        /// <summary>
        /// 线程锁辅助对象(设置投影者)
        /// </summary>
        static private object objSetMatrixEntity = new object();

        /// <summary>
        /// 线程锁辅助对象(获取投影者)
        /// </summary>
        static private object objGetMatrixEntity = new object();

        /// <summary>
        /// 线程锁辅助对象(申请投影)
        /// </summary>
        static private object objApplyMatrixEntity = new object();

        /// <summary>
        /// 线程锁辅助对象(投影命令)
        /// </summary>
        static private object objSendMaxtrixControlCode = new object();

        /// <summary>
        /// 线程锁辅助对象(进入座位)
        /// </summary>
        static private object objInToOneSeat = new object();

        /// <summary>
        /// 线程锁辅助对象(离开座位)
        /// </summary>
        static private object objLeaveOneSeat = new object();

        #endregion

        #region 设置投影者（）

        /// <summary>
        /// 设置投影者（矩阵应用）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="conferenceMatrixEntity">投影者</param>       
        [WebMethod]
        public void SetMatrixEntity(int conferenceID, ConferenceMatrixEntity conferenceMatrixEntity)
        {
            //上锁,达到线程互斥作用
            lock (objSetMatrixEntity)
            {
                try
                {
                    //会议名称不为空
                    if (conferenceID == 0) return;

                    //查看缓存中是否包含该会议名称
                    if (dicConferenceMatrixEntity.ContainsKey(conferenceID))
                    {
                        //取出临时矩阵投影人                      
                        dicConferenceMatrixEntity[conferenceID] = conferenceMatrixEntity;
                    }
                    else
                    {
                        //若没有，则记录会议并绑定相应的矩阵数据
                        dicConferenceMatrixEntity.Add(conferenceID, conferenceMatrixEntity);
                    }
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID, conferenceMatrixEntity);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }




        #endregion

        #region 获取投影者

        /// <summary>
        /// 获取投影者
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回投影者</returns>
        [WebMethod]
        public ConferenceMatrixEntity GetMatrixEntity(int conferenceID)
        {
            ConferenceMatrixEntity conferenceMatrixEntity = null;
            try
            {
                //上锁,达到线程互斥作用
                lock (objGetMatrixEntity)
                {
                    try
                    {
                        //会议名称不为空
                        if (conferenceID == 0) return null;

                        //查看缓存中是否包含该会议名称
                        if (dicConferenceMatrixEntity.ContainsKey(conferenceID))
                        {
                            //取出临时矩阵投影人                      
                            conferenceMatrixEntity = dicConferenceMatrixEntity[conferenceID];
                            SeatEntity seatEntity = conferenceMatrixEntity.SeatEntity;
                            string strIP = seatEntity.SettingIP;
                            if (dicSeatEntity.ContainsKey(conferenceID) && !string.IsNullOrEmpty(strIP))
                            {
                                List<SeatEntity> seatList = dicSeatEntity[conferenceID].Where(Item => Item.SettingIP.Equals(strIP)).ToList<SeatEntity>();
                                if (seatList.Count > 0 && seatList[0].SettingIP.Equals(strIP))
                                {
                                    if (Constant.DicMatrixMeetServerSocket.ContainsKey(conferenceID))
                                    {
                                        string uri = seatList[0].SharerUri;
                                        conferenceMatrixEntity.SharerUri = uri;
                                        MeetServerSocket meetSocekt = Constant.DicMatrixMeetServerSocket[conferenceID];
                                        if (meetSocekt != null && meetSocekt.DicClientSocket != null &&
                                            meetSocekt.DicClientSocket.Count > 0 && !string.IsNullOrEmpty(uri) && meetSocekt.DicClientSocket.ContainsKey(uri))
                                        {
                                            SocketModel socketModel = meetSocekt.DicClientSocket[uri];
                                            bool isOnline = Constant.ServerSendData(socketModel.Socket, 10);
                                            //判断是否在线
                                            conferenceMatrixEntity.IsOnline = isOnline;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
            return conferenceMatrixEntity;
        }

        #endregion

        #region 申请投影

        /// <summary>
        /// 申请投影
        /// </summary>
        /// <param name="conferenceName">会议名称</param>        
        [WebMethod]
        public void ApplyMatrixEntity(int conferenceID, ConferenceMaxtrixApplyEntity conferenceMaxtrixApplyEntity)
        {
            try
            {
                //上锁,达到线程互斥作用
                lock (objApplyMatrixEntity)
                {
                    try
                    {
                         //会议名称不为空
                    if (conferenceID==0) return;
                    ConferenceMatrixEntity conferenceMatrixEntity = this.GetMatrixEntity(conferenceID);
                        if (conferenceMatrixEntity != null)
                        {
                            conferenceMaxtrixApplyEntity.BeforeSeatEntity = conferenceMatrixEntity.SeatEntity;
                        }
                        //实时同步(发送信息给客户端)
                        this.InformClient(conferenceID, conferenceMaxtrixApplyEntity);
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 投影命令

        /// <summary>
        /// 投影命令
        /// </summary>
        [WebMethod]
        public void SendMaxtrixControlCode(int conferenceID, ConferenceMatrixThrowCode conferenceMatrixThrowCode)
        {
            lock (objSendMaxtrixControlCode)
            {
                try
                {
                    this.InformClient(conferenceID, conferenceMatrixThrowCode);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 获取座位信息

        /// <summary>
        /// 获取座位信息
        /// </summary>
        [WebMethod]
        public List<SeatEntity> InToOneSeat(int conferenceID, string seatList, string selfUri, string selfName, string selfIP)
        {
            lock (objInToOneSeat)
            {
                //座位信息集合
                List<SeatEntity> settingList = null;
                try
                {
                    if (conferenceID!=0)
                    {

                        //座位加载人
                        SeatEntity settingAddEntity = null;
                        //座位信息
                        if (!dicSeatEntity.ContainsKey(conferenceID))
                        {
                            //座位信息集合
                            settingList = new List<SeatEntity>();

                            //分割获取座位IP集
                            string[] settingIpList = seatList.Split(new char[] { '*' });

                            //加载位置信息
                            for (int i = 0; i < settingIpList.Count(); i++)
                            {
                                //创建一个座位实体
                                SeatEntity settingEntity = new ConferenceWebCommon.EntityHelper.ConferenceMatrix.SeatEntity();
                                //设置座位IP
                                settingEntity.SettingIP = settingIpList[i];
                                //设置座位序列号
                                settingEntity.SettingNummber = i + 1;
                                //添加座位实体
                                settingList.Add(settingEntity);

                                //设置当前用户名称
                                if (selfIP.Equals(settingIpList[i]))
                                {
                                    settingEntity.UserName = selfName;
                                    settingAddEntity = settingEntity;
                                    settingAddEntity.SharerUri = selfUri;
                                }
                            }
                            //针对会议的座位信息
                            dicSeatEntity.Add(conferenceID, settingList);
                        }
                        else
                        {
                            List<SeatEntity> sList = dicSeatEntity[conferenceID];
                            //加载位置信息
                            for (int i = 0; i < sList.Count(); i++)
                            {
                                //设置当前用户名称
                                if (selfIP.Equals(sList[i].SettingIP))
                                {
                                    settingAddEntity = sList[i];
                                    settingAddEntity.SharerUri = selfUri;
                                    sList[i].UserName = selfName;
                                    break;
                                }
                            }
                            //获取当前会议的座位信息
                            settingList = dicSeatEntity[conferenceID];
                        }

                        //实时同步(发送信息给客户端)
                        this.InformClient(conferenceID, settingAddEntity);
                    }

                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                finally
                {

                }
                return settingList;
            }
        }

        #endregion

        #region 离开位置

        /// <summary>
        /// 离开位置
        /// </summary>
        [WebMethod]
        public void LeaveOneSeat(int conferenceID, string selfName, string selfIP)
        {
            lock (objLeaveOneSeat)
            {
                try
                {
                    if (conferenceID!=0)
                    {
                        //座位信息
                        if (dicSeatEntity.ContainsKey(conferenceID))
                        {
                            List<SeatEntity> settingIpList = dicSeatEntity[conferenceID];
                            //加载位置信息
                            for (int i = 0; i < settingIpList.Count(); i++)
                            {
                                //设置当前用户名称
                                if (selfIP.Equals(settingIpList[i].SettingIP))
                                {
                                    settingIpList[i].UserName = string.Empty;

                                    //实时同步(发送信息给客户端)
                                    this.InformClient(conferenceID, settingIpList[i]);
                                    break;
                                }
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
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(int conferenceID, ConferenceMatrixBase conferenceMatrixBase)
        {
            try
            {
                //会议名称不为空
                if (conferenceID!=0)
                {
                    //缓存数据包含该会议
                    if (dicSeatEntity.ContainsKey(conferenceID))
                    {
                        //生成一个数据包（文件甩屏）
                        PackageBase pack = new PackageBase()
                        {
                            ConferenceClientAcceptType = ConferenceClientAcceptType.ConferenceMatrixSync,
                            ConferenceMatrixBase = conferenceMatrixBase
                        };
                        //会议通讯节点信息发送管理中心
                        Constant.SendClientCenterManage(Constant.DicMatrixMeetServerSocket, conferenceID, pack);
                    }
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
