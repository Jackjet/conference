using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.Common
{
    [Serializable]
    public class PortTypeEntity
    {      
        /// <summary>
        /// 服务类型
        /// </summary>
        public ConferenceClientAcceptType conferenceClientAcceptType { get; set; }

        /// <summary>
        /// 开放的服务端口
        /// </summary>
        public int ServerPoint { get; set; }
    }
}
