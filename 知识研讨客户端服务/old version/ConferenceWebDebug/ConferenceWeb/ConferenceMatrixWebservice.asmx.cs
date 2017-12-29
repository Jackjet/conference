using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using ConferenceWebCommon;
using ConferenceWebCommon.EntityHelper.ConferenceMatrix;
using ConferenceWebCommon.Common;

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
        static Dictionary<string, ConferenceMatrixEntity> MatrixSyncAppDic = new Dictionary<string, ConferenceMatrixEntity>();

        /// <summary>
        /// 座位信息
        /// </summary>
        static Dictionary<string, List<SeatEntity>> SeatEntityList = new Dictionary<string, List<SeatEntity>>();

        /// <summary>
        /// 线程锁辅助对象(填充服务器的数据)
        /// </summary>
        static private object objMatrixSyncServiceData = new object();

        /// <summary>
        /// 线程锁辅助对象(进入座位)
        /// </summary>
        static private object objInToOneSeat = new object();

        /// <summary>
        /// 线程锁辅助对象(离开座位)
        /// </summary>
        static private object objLeaveOneSeat = new object();

        #endregion

        #region 填充服务器的数据（）

        /// <summary>
        /// 填充服务器的数据（矩阵应用）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="sharer">共享人</param>
        /// <param name="uri">文件地址</param>
        /// <param name="fileType">文件类型</param>
        [WebMethod]
        public void SetMatrixEntity(string conferenceName, string sharer, ConferenceMatrixOutPut conferenceMatrixOutPut)
        {
            //上锁,达到线程互斥作用
            lock (objMatrixSyncServiceData)
            {

                try
                {
                    //会议名称不为空
                    if (string.IsNullOrEmpty(conferenceName)) return;

                    //查看缓存中是否包含该会议名称
                    if (MatrixSyncAppDic.ContainsKey(conferenceName))
                    {
                        //取出临时矩阵投影人                      
                        MatrixSyncAppDic[conferenceName].Sharer = sharer;
                        //输出（判断是哪个屏幕）
                        MatrixSyncAppDic[conferenceName].ConferenceMatrixOutPut = conferenceMatrixOutPut;
                    }
                    else
                    {
                        //若没有，则记录会议并绑定相应的矩阵数据
                        MatrixSyncAppDic.Add(conferenceName, new ConferenceMatrixEntity() { Sharer = sharer, ConferenceMatrixOutPut = conferenceMatrixOutPut });
                    }
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceName, MatrixSyncAppDic[conferenceName]);
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
        public List<SeatEntity> InToOneSeat(string conferenceName, string seatList, string selfName, string selfIP)
        {
            lock (objInToOneSeat)
            {
                //座位信息集合
                List<SeatEntity> settingList = null;
                try
                {
                    if (!string.IsNullOrEmpty(conferenceName))
                    {

                        //座位加载人
                        SeatEntity settingAddEntity = null;
                        //座位信息
                        if (!SeatEntityList.ContainsKey(conferenceName))
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
                                }
                            }
                            //针对会议的座位信息
                            SeatEntityList.Add(conferenceName, settingList);
                        }
                        else
                        {
                            List<SeatEntity> sList = SeatEntityList[conferenceName];
                            //加载位置信息
                            for (int i = 0; i < sList.Count(); i++)
                            {
                                //设置当前用户名称
                                if (selfIP.Equals(sList[i].SettingIP))
                                {
                                    settingAddEntity = sList[i];
                                    sList[i].UserName = selfName;
                                    break;
                                }
                            }
                            //获取当前会议的座位信息
                            settingList = SeatEntityList[conferenceName];
                        }

                        //实时同步(发送信息给客户端)
                        this.InformClient(conferenceName, settingAddEntity);
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
        public void LeaveOneSeat(string conferenceName, string selfName, string selfIP)
        {
            lock (objLeaveOneSeat)
            {
                try
                {
                    if (!string.IsNullOrEmpty(conferenceName))
                    {
                        //座位信息
                        if (SeatEntityList.ContainsKey(conferenceName))
                        {
                            List<SeatEntity> settingIpList = SeatEntityList[conferenceName];
                            //加载位置信息
                            for (int i = 0; i < settingIpList.Count(); i++)
                            {
                                //设置当前用户名称
                                if (selfIP.Equals(settingIpList[i].SettingIP))
                                {
                                    settingIpList[i].UserName = string.Empty;

                                    //实时同步(发送信息给客户端)
                                    this.InformClient(conferenceName, settingIpList[i]);
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
        public void InformClient(string conferenceName, ConferenceMatrixBase conferenceMatrixBase)
        {
            try
            {
                //会议名称不为空
                if (!string.IsNullOrEmpty(conferenceName))
                {
                    //缓存数据包含该会议
                    if (SeatEntityList.ContainsKey(conferenceName))
                    {
                        //生成一个数据包（文件甩屏）
                        PackageBase pack = new PackageBase()
                        {
                            ConferenceClientAcceptType = ConferenceWebCommon.Common.ConferenceClientAcceptType.ConferenceMatrixSync,
                            ConferenceMatrixBase = conferenceMatrixBase
                        };
                        //会议通讯节点信息发送管理中心
                        Constant.SendClientCenterManage(Constant.DicMatrixMeetServerSocket, conferenceName, pack);
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
