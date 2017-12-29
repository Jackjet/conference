using ConferenceWebCommon.Common;
using ConferenceWebCommon.EntityCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConferenceWeb.Common
{
    [Serializable]
    public class ConferenceInformationEntityPC
    {
        /// <summary>
        /// 会议ID
        /// </summary>
        public int MeetingID { get; set; }

        /// <summary>
        /// 会议室名称
        /// </summary>
        public string MeetingName { get; set; }

        /// <summary>
        /// 会议主题
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// 会议相关参会人列表
        /// </summary>
        public List<string> JoinPeople { get; set; }

        /// <summary>
        /// 会议相关参会人列表
        /// </summary>       
        public List<string> JoinPeopleName { get; set; }

        /// <summary>
        /// 会议开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }

        /// <summary>
        /// 会议结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 主持人
        /// </summary>
        public string ApplyPeople { get; set; }
      
        /// <summary>
        /// 主课题
        /// </summary>
        public string PTaskName { get; set; }

        /// <summary>
        /// 子课题
        /// </summary>
        public string STaskName { get; set; }
      
        /// <summary>
        /// 场景模式(标准模式、精简模式、教学模式)
        /// </summary>
        public SceneModeType SceneModeType { get; set; }

        /// <summary>
        /// 矩阵投影模式（自由、传递、审批）
        /// </summary>
        public MaxtrixModeType MaxtrixModeType { get; set; }
        
        /// <summary>
        /// 默认投影桌位
        /// </summary>
        public int FirstProjectionSeat { get; set; }

        /// <summary>
        /// ip座位号
        /// </summary>
        public string SettingIpList { get; set; }

        /// <summary>
        /// 网络浏览地址
        /// </summary>
        public string WebUri { get; set; }

        /// <summary>
        /// 瘦客身份
        /// </summary>
        public string ThinClientUserName { get; set; }


         /// <summary>
        /// 是否包含瘦客
        /// </summary>
        public bool HasThinClient { get; set; }
        
     
    }


}