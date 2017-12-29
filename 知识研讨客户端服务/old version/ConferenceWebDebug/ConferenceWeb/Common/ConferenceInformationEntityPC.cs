using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConferenceWeb.Common
{
    [Serializable]
    public class ConferenceInformationEntityPC
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
                meetingID = value;
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
                applyPeople = value;
            }
        }

        //Uri imgurl;
        ///// <summary>
        ///// 会议室图片
        ///// </summary>       
        //public Uri Imgurl
        //{
        //    get { return imgurl; }
        //    set { imgurl = value; }
        //}

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

        bool simpleMode = true;
        /// <summary>
        /// 精简模式
        /// </summary>
        public bool SimpleMode
        {
            get { return simpleMode; }
            set { simpleMode = value; }
        }

        bool educationMode = false;
        /// <summary>
        /// 教学模式
        /// </summary>
        public bool EducationMode
        {
            get { return educationMode; }
            set { educationMode = value; }
        }

        string settingIpList;
        /// <summary>
        /// ip座位号
        /// </summary>
        public string SettingIpList
        {
            get { return settingIpList; }
            set { settingIpList = value; }
        }


        string webUri;
        /// <summary>
        /// 网络浏览地址
        /// </summary>
        public string WebUri
        {
            get { return webUri; }
            set { webUri = value; }
        }

        string bigScreenName;
        /// <summary>
        /// 投影大屏幕
        /// </summary>
        public string BigScreenName
        {
            get { return bigScreenName; }
            set { bigScreenName = value; }
        }

        bool runCKOAPP;
        /// <summary>
        /// 是否运行投影应用程序
        /// </summary>
        public bool RunCKOAPP
        {
            get { return runCKOAPP; }
            set { runCKOAPP = value; }
        }
    }
}