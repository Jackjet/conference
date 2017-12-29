using ConferenceModel.ConferenceInfoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_MyConference.Common
{
   public  class MyConferenceCodeEnterEntity
    {
        /// <summary>
        /// 当前参加的研讨会议名称
        /// </summary>
       public static string ConferenceName { get; set; }

       /// <summary>
       /// 当前用户名称
       /// </summary>
        public static string SelfName { get; set; }

        /// <summary>
        /// 当前机器的IP地址
        /// </summary>
        public static string LocalIp { get; set; }

        ///// <summary>
        ///// 当前参加的研讨会议室名称
        ///// </summary>
        //public static string ConferenceRoomName { get; set; }

        /// <summary>
        /// 当前参加的研讨会主持人
        /// </summary>
        //public static string ConferenceHost { get; set; }

        /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName { get; set; }

        /// <summary>
        /// 当前用户Url地址
        /// </summary>
        public static string SelfUri { get; set; }

        /// <summary>
        /// 临时存储的会议信息
        /// </summary>
        public static ConferenceInformationEntityPC TempConferenceInformationEntity { get; set; }
    }
}
