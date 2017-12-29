using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;
using ConferenceWebCommon.EntityHelper.ConferenceOffice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace ConferenceWeb
{
    /// <summary>
    /// ConferenceOfficeAsyncWebservice 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class ConferenceSpaceAsyncWebservice : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 智存空间缓存数据
        /// </summary>
        static Dictionary<int, ConferenceSpaceAsyncEntity> SpaceSyncAppDic = new Dictionary<int, ConferenceSpaceAsyncEntity>();

        /// <summary>
        /// 线程锁辅助对象(填充服务器的数据)
        /// </summary>
        static private object objFillSyncServiceData = new object();

        #endregion

        #region 填充服务器的数据（智存空间）

        /// <summary>
        /// 填充服务器的数据（智存空间）
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="sharer">共享人</param>
        /// <param name="uri">文件地址</param>
        /// <param name="fileType">文件类型</param>
        [WebMethod]
        public void FillSyncServiceData(int conferenceID, string sharer, string uri, FileType fileType)
        {
            //上锁,达到线程互斥作用
            lock (objFillSyncServiceData)
            {
                try
                {
                      //会议名称不为空
                    if (conferenceID==0) return;

                    //查看缓存中是否包含该会议名称
                    if (SpaceSyncAppDic.ContainsKey(conferenceID))
                    {
                        //有则取出该会议临时存储的word路径
                        SpaceSyncAppDic[conferenceID].Uri = uri;
                        SpaceSyncAppDic[conferenceID].Sharer = sharer;
                        SpaceSyncAppDic[conferenceID].FileType = fileType;
                    }
                    else
                    {
                        //若没有，则记录会议并绑定相应的刷屏数据
                        SpaceSyncAppDic.Add(conferenceID, new ConferenceSpaceAsyncEntity() { Uri = uri, Sharer = sharer, FileType = fileType });
                    }
                    //实时同步(发送信息给客户端)
                    this.InformClient(conferenceID, SpaceSyncAppDic[conferenceID]);
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
        public void InformClient(int conferenceID, ConferenceSpaceAsyncEntity conferenceOfficeAsyncEntity)
        {
            try
            {
                //会议名称不为空
                if (conferenceID!=0)
                {
                    //缓存数据包含该会议
                    if (SpaceSyncAppDic.ContainsKey(conferenceID))
                    {
                        //生成一个数据包（文件甩屏）
                        PackageBase pack = new PackageBase()
                        {
                            ConferenceClientAcceptType = ConferenceClientAcceptType.ConferenceSpaceSync,
                            ConferenceSpaceAsyncEntity = conferenceOfficeAsyncEntity
                        };
                        //会议通讯节点信息发送管理中心
                        Constant.SendClientCenterManage(Constant.DicSpaceMeetServerSocket, conferenceID, pack);
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
