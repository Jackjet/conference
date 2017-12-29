using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Lync.Model;
using ConferenceCommon.EntityHelper;
using Microsoft.Lync.Model.Extensibility;
using Microsoft.Lync.Model.Conversation;

namespace Conference_Conversation.Common
{
    public static class ConversationCodeEnterEntity
    {
      
        /// <summary>
        /// lync(automation)
        /// </summary>
        public static Automation lyncAutomation { get; set; }

      
        private static List<string> participantList = new List<string>();
        /// <summary>
        /// 参会人列表
        /// </summary>
        public static List<string> ParticipantList
        {
            get { return ConversationCodeEnterEntity.participantList; }
            set { ConversationCodeEnterEntity.participantList = value; }
        }


        private static Dictionary<string, string> dicParticipant = new Dictionary<string, string>();
        /// <summary>
        /// 参会人信息字典(邮件地址对应姓名)
        /// </summary>
        public static Dictionary<string, string> DicParticipant
        {
            get { return ConversationCodeEnterEntity.dicParticipant; }
            set { ConversationCodeEnterEntity.dicParticipant = value; }
        }

        /// <summary>
        /// Tree服务web服务地址
        /// </summary>
        public static string TreeServiceAddressFront { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public static string UserDomain { get; set; }

        /// <summary>
        /// 用户头像文件目录（）
        /// </summary>
        public static string FtpServercePersonImgName { get; set; }

        /// <summary>
        /// 当前用户名称
        /// </summary>
        public static string SelfName { get; set; }

        /// <summary>
        /// 当前用户Url地址
        /// </summary>
        public static string SelfUri { get; set; }

        /// <summary>
        /// 大屏幕名称
        /// </summary>
        public static string BigScreenName { get; set; }

        /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName { get; set; }

        /// <summary>
        /// 智存空间登陆密码
        /// </summary>
        public static string WebLoginPassword { get; set; }

        /// <summary>
        /// PaintFileRoot路径
        /// </summary>
        public static string LocalTempRoot { get; set; }

        /// <summary>
        /// 登陆用户名（single_Left）
        /// </summary>
        public static string LoginUserName { get; set; }

        /// <summary>
        /// lync1的IP
        /// </summary>
        public static string LyncIP1 { get; set; }

        /// <summary>
        /// 服务区缓存文件夹
        /// </summary>
        public static string ServicePPTTempFile { get; set; }


        #region 内部使用

        /// <summary>
        /// lync客户端
        /// </summary>
        internal static LyncClient lyncClient { get; set; }

        /// <summary>
        /// 会话管理者
        /// </summary>
        internal static ContactManager contactManager { get; set; }

        /// <summary>
        /// 会话管理
        /// </summary>
        internal static ConversationManager conversationManager { get; set; }


        #endregion
    }
}
