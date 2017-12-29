using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWebCommon.EntityHelper.LyncConversation
{
    [Serializable]
    public class LyncConversationFlg
    {
        LyncConversationEntity lyncConversationEntity;
        /// <summary>
        /// 邀请参会
        /// </summary>
        public LyncConversationEntity LyncConversationEntity
        {
            get { return lyncConversationEntity; }
            set { lyncConversationEntity = value; }
        }

        BigScreenEnterEntity bigScreenEnterEntity;
        /// <summary>
        /// 大屏投影
        /// </summary>
        public BigScreenEnterEntity BigScreenEnterEntity
        {
            get { return bigScreenEnterEntity; }
            set { bigScreenEnterEntity = value; }
        }

        LeaveConversationEntity leaveConversationEntity;
        /// <summary>
        /// 离开会议实体
        /// </summary>
        public LeaveConversationEntity LeaveConversationEntity
        {
            get { return leaveConversationEntity; }
            set { leaveConversationEntity = value; }
        }

        PPTControlEntity pPTControlEntity;
        /// <summary>
        /// 离开会议实体
        /// </summary>
        public PPTControlEntity PPTControlEntity
        {
            get { return pPTControlEntity; }
            set { pPTControlEntity = value; }
        } 

        LyncConversationFlgType lyncConversationFlgType;
        /// <summary>
        /// 消息模式
        /// </summary>
        public LyncConversationFlgType LyncConversationFlgType
        {
            get { return lyncConversationFlgType; }
            set { lyncConversationFlgType = value; }
        }
    }
    [Serializable]
    public enum LyncConversationFlgType
    {
        InviteContact = 0,
        EnterBigScreen =1,
        LeaveConversation =2,
        PPTControl =3,
    }
}
