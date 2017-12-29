using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;
using ConferenceWebCommon.EntityHelper.FileSyncAppPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Services;

namespace ConferenceWeb
{
    /// <summary>
    /// FileSyncAppPoolWebservice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class FileSyncAppPoolWebservice : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 甩屏同步缓存数据
        /// </summary>
        static Dictionary<int, FileSyncAppEntity> FileSyncAppDic = new Dictionary<int, FileSyncAppEntity>();

        /// <summary>
        /// 线程锁辅助对象（填充服务器的数据）
        /// </summary>
        static private object objFillSyncServiceData = new object();

        /// <summary>
        /// 线程锁辅助对象（获取服务器的数据）
        /// </summary>
        static private object objGetSyncServiceData = new object();

        #endregion

        #region 填充服务器的数据（需要甩屏同步的数据）

        /// <summary>
        /// 填充服务器的数据（需要甩屏同步的数据）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="bytesStr">刷屏填充数据</param>
        [WebMethod]
        public void FillSyncServiceData(int conferenceID, byte[] bytesStr)
        {
              //上锁,达到线程互斥作用
            lock (objFillSyncServiceData)
            {
                try
                {
                     //会议名称不为空
                    if (conferenceID==0) return;

                    //字节数组不为空【图片文件】
                    if (bytesStr == null) return;
                    //查看缓存中是否包含该会议名称
                    if (FileSyncAppDic.ContainsKey(conferenceID))
                    {
                        //有则取出该会议临时存储的甩屏数据
                        FileSyncAppDic[conferenceID].ImgBytes = bytesStr;
                    }
                    else
                    {
                        //若没有，则记录会议并绑定相应的刷屏数据
                        FileSyncAppDic.Add(conferenceID, new FileSyncAppEntity() { ImgBytes = bytesStr });
                    }
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID);
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            }
        }

        #endregion

        #region 获取服务器的数据（需要甩屏同步的数据）

        /// <summary>
        /// 获取服务器的数据（需要甩屏同步的数据）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回甩屏同步数据</returns>
        [WebMethod]
        public FileSyncAppEntity GetSyncServiceData(int conferenceID)
        {
              //上锁,达到线程互斥作用
            lock (objGetSyncServiceData)
            {
                //会议名称不为空
                    if (conferenceID==0) return null;
                //获取到的数据
                FileSyncAppEntity fileSyncAppEntity = null;
                try
                {
                    //若缓存中有该会议,则获取该会议的刷屏数据
                    if (FileSyncAppDic.ContainsKey(conferenceID))
                    {
                        fileSyncAppEntity = FileSyncAppDic[conferenceID];
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return fileSyncAppEntity;
            }
        }

        #endregion

        #region 通讯机制（服务端给客户端发送信息）

        #region 实时同步(发送信息给客户端)

        /// <summary>
        /// 实时同步(发送信息给客户端)
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        public void InformClient(int conferenceID)
        {
            try
            {            
                //会议名称不为空
                if (conferenceID!=0)
                {
                    //临时存储（用于校验的实例，没有父节点和子节点，一对一） 
                    if (FileSyncAppDic.ContainsKey(conferenceID))
                    {
                        //生成一个数据包（文件甩屏）
                        PackageBase pack = new PackageBase() { ConferenceClientAcceptType = ConferenceClientAcceptType.ConferenceFileSync };
                        //会议通讯节点信息发送管理中心
                        Constant.SendClientCenterManage(Constant.DicFileMeetServerSocket, conferenceID, pack);
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
