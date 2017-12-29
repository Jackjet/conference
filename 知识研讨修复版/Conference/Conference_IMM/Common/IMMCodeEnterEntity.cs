using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Conference_IMM.Common
{
    public static class IMMCodeEnterEntity
    {

        /// <summary>
        /// 网页客户端地址
        /// </summary>
        public static string ConferenceWebAppAddress { get; set; }

        ///// <summary>
        ///// 智存空间登陆用户名
        ///// </summary>
        //public static string WebLoginUserName { get; set; }

        /// <summary>
        /// 登陆用户名（single_Left）
        /// </summary>
        public static string LoginUserName { get; set; }

        /// <summary>
        /// 当前参加的研讨会议名称
        /// </summary>
        public static string ConferenceName { get; set; }

        /// <summary>
        /// 当前用户名称
        /// </summary>
        public static string SelfName { get; set; }

        /// <summary>
        /// Tree服务web服务地址
        /// </summary>
        public static  string TreeServiceAddressFront { get; set; }

        /// <summary>
        /// 用户头像文件目录（）
        /// </summary>
        public static string FtpServercePersonImgName { get; set; }

        /// <summary>
        /// 研讨ftp服务地址
        /// </summary>
        public static string ConferenceFtpWebAddressFront { get; set; }

        /// <summary>
        /// ftp用户
        /// </summary>
        public static string FtpUserName { get; set; }

        /// <summary>
        /// ftp用户密码
        /// </summary>
        public static string FtpPassword { get; set; }

        /// <summary>
        /// 语音文件本地目录
        /// </summary>
        public static string AudioFile_Root { get; set; }

        /// <summary>
        /// 语音文件名称
        /// </summary>
        public static string AudioFile_Name { get; set; }

        /// <summary>
        /// 语音文件类型
        /// </summary>
        public static string AudioFile_Extention { get; set; }

        /// <summary>
        /// ftp音频文件目录（）
        /// </summary>
        public static string FtpServerceAudioName { get; set; }

        /// <summary>
        /// 是否为会议主持人
        /// </summary>
        public static bool IsMeetingPresenter { get; set; }

        /// <summary>
        /// pdf转换应用程序名称
        /// </summary>
        public static string PdfTransferAppName { get; set; }

         /// <summary>
        /// pdf转换应用程序名称
        /// </summary>
        public static string FileRoot { get; set; }
        
    }
}
