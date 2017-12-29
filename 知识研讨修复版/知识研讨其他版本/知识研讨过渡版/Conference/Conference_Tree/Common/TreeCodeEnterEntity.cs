using ConferenceCommon.SharePointHelper;
using ConferenceModel.ConferenceInfoWebService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;


namespace Conference_Tree
{
    public static class TreeCodeEnterEntity
    {
       
        /// <summary>
        /// 会议信息
        /// </summary>
        public static string ConferenceName { get; set; }

        /// <summary>
        /// 是否为会议主持人
        /// </summary>
        public static bool IsMeetPresenter { get; set; }

        /// <summary>
        /// 主窗体引用
        /// </summary>
        public static Window MainWindow { get; set; }

        /// <summary>
        /// 节点默认名称
        /// </summary>
        public static string TreeItemEmptyName { get; set; }

        /// <summary>
        /// 用户登录名
        /// </summary>
        public static string LoginUserName { get; set; }

        /// <summary>
        /// 当前用户的uri地址
        /// </summary>
        public static string SelfUri { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public static string SelfName { get; set; }

        /// <summary>
        /// 知识树xml文件名称
        /// </summary>
        public static string TreeXmlFileName { get; set; }

        /// <summary>
        /// 投票标题集合
        /// </summary>
        public static  List<string> VoteChatTittleList { get; set; }

        /// <summary>
        /// 会议根目录
        /// </summary>
        public static string MeetingFolderName { get; set; }

        /// <summary>
        /// 客户端上下文管理
        /// </summary>
        public static ClientContextManage ClientContextManage { get; set; }

        /// <summary>
        /// 录制文件夹名称
        /// </summary>
        public static string RecordFolderName { get; set; }

        /// <summary>
        /// 录制视频格式
        /// </summary>
        public static string RecordExtention { get; set; }

        /// <summary>
        /// 上传的录制视频名称
        /// </summary>
        public static string ReacordUploadFileName { get; set; }

        /// <summary>
        /// 参会人列表
        /// </summary>
        public static List<string> ParticipantList = new List<string>();

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
        /// PaintFileRoot路径
        /// </summary>
        public static string PaintFileRoot { get; set; }

         /// <summary>
        /// 会议纪要模板名称
        /// </summary>
        public static string ConferenceCommentHtmlTemp { get; set; }

        /// <summary>
        /// 临时存储的会议信息
        /// </summary>
        public static ConferenceInformationEntityPC TempConferenceInformationEntity { get; set; }
    }
}
