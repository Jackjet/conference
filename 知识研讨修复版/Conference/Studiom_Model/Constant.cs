using Studiom_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Studiom_Model
{
    /// <summary>
    /// 持久类
    /// </summary>
    public static class Constant
    {
      
      
        static StudiomService studiomDataInstance;
        /// <summary>
        /// 电源时序器
        /// </summary>
        public static StudiomService StudiomDataInstance
        {
            get
            {
                if (studiomDataInstance == null)
                {
                    studiomDataInstance = new StudiomService();
                    studiomDataInstance.StudiomServiceInit();
                }
                return Constant.studiomDataInstance;
            }

        }

        /// <summary>
        /// 初始化矩阵服务模型
        /// </summary>
        public static void StudiomDataInstanceInit()
        {
            if (studiomDataInstance == null)
            {
                studiomDataInstance = new StudiomService();
                studiomDataInstance.StudiomServiceInit();
            }
        }
    }
}
