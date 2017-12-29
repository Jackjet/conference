using ConferenceWebCommon.EntityHelper.ConferenceVersion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
    public class ConferenceVersionWebservice : System.Web.Services.WebService
    {
        #region 静态字段

        /// <summary>
        /// 线程锁辅助对象
        /// </summary>
        static private object obj = new object();

        #endregion

        #region 获取更新文件
        
        /// <summary>
        /// 获取更新文件
        /// </summary>
        /// <returns>返回更新文件实体</returns>
        [WebMethod]
        public ConferenceVersionUpdateEntity GetUpDateFile()
        {
            //上锁,达到线程互斥作用
            lock (obj)
            {
                //实例化更新文件实体
                ConferenceVersionUpdateEntity conferenceVersionUpdateEntity = new ConferenceVersionUpdateEntity();
                try
                {
                    // 需要更新的文件
                    conferenceVersionUpdateEntity.UpdateFile = Constant.UpdateFile;
                    // 需要更新的文件（包含目录）
                    conferenceVersionUpdateEntity.UpdateRootFile = Constant.UpdateRootFile;
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return conferenceVersionUpdateEntity;
            }
        }

        #endregion

        #region 获取最新客户端版本号
        
        /// <summary>
        /// 获取最新客户端版本号
        /// </summary>
        /// <param name="PC_Version">传入版本号</param>
        /// <returns>返回是否需要进行版本更新的标示</returns>
        [WebMethod]
        public bool NeedVersionUpdate(string PC_Version)
        {
            //上锁,达到线程互斥作用
            lock (obj)
            {
                bool needUpdate = false;
                try
                {
                    //知识研讨客户端当前版本
                    if (!PC_Version.Equals(Constant.ConferencePCVersion))
                    {
                        needUpdate = true;
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
                return needUpdate;
            }
        }

        #endregion
    }
}
