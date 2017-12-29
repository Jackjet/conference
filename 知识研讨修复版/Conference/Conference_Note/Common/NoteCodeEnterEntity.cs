using ConferenceCommon.SharePointHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_Note.Common
{
    public static class NoteCodeEnterEntity
    {
        /// <summary>
        /// 个人存储文件夹名称
        /// </summary>
        public static string PesonalFolderName{ get; set; }

        /// <summary>
        /// 登陆用户名（single_Left）
        /// </summary>
        public static string LoginUserName { get; set; }

          /// <summary>
        /// 当前用户名称
        /// </summary>
        public static string SelfName { get; set; }

         /// <summary>
        /// SharePoint客户端对象模型管理
        /// </summary>
        public static ClientContextManage clientContextManage { get; set; }

        
        /// <summary>
        /// PaintFileRoot路径
        /// </summary>
        public static string FileRoot { get; set; }

        /// <summary>
        /// 本地个人笔记名称
        /// </summary>
        public static string LocalPersonalNoteFile { get; set; }


        /// <summary>
        /// SharePoint服务IP地址
        /// </summary>
        public static string SPSiteAddressFront { get; set; }

         /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName { get; set; }

           /// <summary>
        /// 智存空间登陆密码
        /// </summary>
        public static string WebLoginPassword { get; set; }
    }
}
