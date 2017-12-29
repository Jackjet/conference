using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ConferenceWeb.Common
{
    //并非需要对应所有属性
    [Serializable]
    public class ConferenceInformationEntity
    {       
        int meetingID;
        /// <summary>
        /// 会议ID
        /// </summary>
        public int MeetingID
        {
            get { return meetingID; }
            set
            {
                if (value != meetingID)
                {
                    meetingID = value;                   
                }
            }
        }

        string meetingName;
        /// <summary>
        /// 会议室名称
        /// </summary>
        public string MeetingName
        {
            get { return meetingName; }
            set { meetingName = value; }
        }

        string roomName;
        /// <summary>
        /// 会议主题
        /// </summary>
        public string RoomName
        {
            get { return roomName; }
            set { roomName = value; }
        }

        List<string> joinPeople;
        /// <summary>
        /// 会议相关参会人列表
        /// </summary>
        public List<string> JoinPeople
        {
            get { return joinPeople; }
            set { joinPeople = value; }
        }

        List<string> joinPeopleName;
        /// <summary>
        /// 会议相关参会人列表
        /// </summary>
        public List<string> JoinPeopleName
        {
            get { return joinPeopleName; }
            set { joinPeopleName = value; }
        }
        

        DateTime beginTime;
        /// <summary>
        /// 会议开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get { return beginTime; }
            set { beginTime = value; }
        }

        DateTime endTime;
        /// <summary>
        /// 会议结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        string applyPeople;
        /// <summary>
        /// zhuhuiren
        /// </summary>
        public string ApplyPeople
        {
            get { return applyPeople; }
            set
            {
                if (value != applyPeople)
                {
                    applyPeople = value;                  
                }
            }
        }
       
        string pTaskName;
        /// <summary>
        /// 主课题
        /// </summary>
        public string PTaskName
        {
            get { return pTaskName; }
            set { pTaskName = value; }
        }

        string sTaskName;
        /// <summary>
        /// 子课题
        /// </summary>
        public string STaskName
        {
            get { return sTaskName; }
            set { sTaskName = value; }
        }               
    }
}
