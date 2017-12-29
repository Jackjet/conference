using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWeb.Common
{
    class ConferenceInformationEntityM
    {      
        List<ConferenceInformationEntity> conferenceInformationEntityList = new List<ConferenceInformationEntity>();
        /// <summary>
        /// 会议信息
        /// </summary>
        public List<ConferenceInformationEntity> ConferenceInformationEntityList
        {
            get { return conferenceInformationEntityList; }
            set { conferenceInformationEntityList = value; }
        }

        string selfRealName;
        /// <summary>
        /// 当前用户名称
        /// </summary>
        public string SelfRealName
        {
            get { return selfRealName; }
            set { selfRealName = value; }
        }

        string selfAddress;
        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string SelfAddress
        {
            get { return selfAddress; }
            set { selfAddress = value; }
        }
    }
}
