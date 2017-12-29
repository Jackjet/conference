using ConferenceCommon.SharePointHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_Space.Common
{
   public static  class SpaceCodeEnterEntity
    {


        /// <summary>
        /// 会议信息
        /// </summary>
        public static string ConferenceName { get; set; }
      
     
        /// <summary>
        /// 用户登录名
        /// </summary>
        public static string LoginUserName { get; set; }
       
        /// <summary>
        /// 用户名称
        /// </summary>
        public static string SelfName { get; set; }
       
        /// <summary>
        /// 会议根目录
        /// </summary>
        public static string MeetingFolderName { get; set; }

        /// <summary>
        /// 客户端上下文管理
        /// </summary>
        public static ClientContextManage ClientContextManage { get; set; }

      
        /// <summary>
        /// PaintFileRoot路径
        /// </summary>
        public static string LocalTempRoot { get; set; }

        /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName { get; set; }


        /// <summary>
        /// 智存空间登陆密码
        /// </summary>
        public static string WebLoginPassword { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public static string UserDomain { get; set; }

        /// <summary>
        /// SharePoint服务IP地址
        /// </summary>
        public static string SPSiteAddressFront { get; set; }
      
     
       /// <summary>
       /// 智存空间地址
       /// </summary>
        public static string SpaceWebSiteUri { get; set; }

       /// <summary>
       /// 会话
       /// </summary>
        public static Microsoft.Lync.Model.Extensibility.ConversationWindow MainConversation { get; set; }

         /// <summary>
        /// Tree服务IP
        /// </summary>
        public static  string TreeServiceIP { get; set; }

               /// <summary>
        /// 服务区缓存文件夹
        /// </summary>
        public static  string ServicePPTTempFile { get; set; }

        /// <summary>
        /// 个人存储文件夹名称
        /// </summary>
        public static string PesonalFolderName { get; set; }
    }
}
