using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConferenceWeb.Common
{
    /// <summary>
    /// 客户端配置信息实体
    /// </summary>
    [Serializable]
    public class ClientConfigEntity
    {
        string dNS1 = System.Configuration.ConfigurationManager.AppSettings["DNS1"];
        /// <summary>
        /// 首用dns
        /// </summary>
        public string DNS1
        {
            get { return dNS1; }
            set { }
        }

        string dNS2 = System.Configuration.ConfigurationManager.AppSettings["dNS2"];
        /// <summary>
        /// 备用dns
        /// </summary>
        public string DNS2
        {
            get { return dNS2; }
            set { }
        }

        string lyncName = System.Configuration.ConfigurationManager.AppSettings["LyncName"];
        /// <summary>
        /// lync名称
        /// </summary>
        public string LyncName
        {
            get { return lyncName; }
            set { }
        }

        string certification = System.Configuration.ConfigurationManager.AppSettings["Certification"];
        /// <summary>
        /// lync证书文件名称
        /// </summary>
        public string Certification
        {
            get { return certification; }
            set { }
        }

        string certificationSerial = System.Configuration.ConfigurationManager.AppSettings["CertificationSerial"];
        /// <summary>
        /// lync证书秘钥
        /// </summary>
        public string CertificationSerial
        {
            get { return certificationSerial; }
            set { }
        }

        string userDomain = System.Configuration.ConfigurationManager.AppSettings["UserDomain"];
        /// <summary>
        /// 域名
        /// </summary>
        public string UserDomain
        {
            get { return userDomain; }
            set { }
        }

        string userDoaminPart1Name = System.Configuration.ConfigurationManager.AppSettings["UserDoaminPart1Name"];
        /// <summary>
        /// 域名前名称
        /// </summary>
        public string UserDoaminPart1Name
        {
            get { return userDoaminPart1Name; }
            set { }
        }

        string lyncIP1 = System.Configuration.ConfigurationManager.AppSettings["LyncIP1"];
        /// <summary>
        /// lync1服务器的IP
        /// </summary>
        public string LyncIP1
        {
            get { return lyncIP1; }
            set { }
        }

        string lyncIP2 = System.Configuration.ConfigurationManager.AppSettings["LyncIP2"];
        /// <summary>
        /// lync2服务器的IP
        /// </summary>
        public string LyncIP2
        {
            get { return lyncIP2; }
            set { }
        }

        string sPSiteAddressFront = System.Configuration.ConfigurationManager.AppSettings["SPSiteAddressFront"];
        /// <summary>
        /// Sharepoint服务IP地址
        /// </summary>
        public string SPSiteAddressFront
        {
            get { return sPSiteAddressFront; }
            set { }
        }

        string treeXmlFileName = System.Configuration.ConfigurationManager.AppSettings["TreeXmlFileName"];
        /// <summary>
        /// 会议xml文件存储名称
        /// </summary>
        public string TreeXmlFileName
        {
            get { return treeXmlFileName; }
            set { }
        }

        string treeJpgFileName = System.Configuration.ConfigurationManager.AppSettings["TreeJpgFileName"];
        /// <summary>
        /// 会议jpg文件存储名称
        /// </summary>
        public string TreeJpgFileName
        {
            get { return treeJpgFileName; }
            set { }
        }

        string meetingFolderName = System.Configuration.ConfigurationManager.AppSettings["MeetingFolderName"];
        /// <summary>
        /// 会议存储文件夹名称
        /// </summary>
        public string MeetingFolderName
        {
            get { return meetingFolderName; }
            set { }
        }

        string pesonalFolderName = System.Configuration.ConfigurationManager.AppSettings["PesonalFolderName"];
        /// <summary>
        /// 个人存储文件夹名称
        /// </summary>
        public string PesonalFolderName
        {
            get { return pesonalFolderName; }
            set { }
        }

        string spaceWebSiteUri = System.Configuration.ConfigurationManager.AppSettings["SpaceWebSiteUri"];
        /// <summary>
        /// 智存空间网站集
        /// </summary>
        public string SpaceWebSiteUri
        {
            get { return spaceWebSiteUri; }
            set { }
        }


        string screenResulotionWidth = System.Configuration.ConfigurationManager.AppSettings["ScreenResulotionWidth"];
        /// <summary>
        /// 投影分辨率宽度设置
        /// </summary>
        public string ScreenResulotionWidth
        {
            get { return screenResulotionWidth; }
            set { }
        }

        string screenResulotionHeight = System.Configuration.ConfigurationManager.AppSettings["ScreenResulotionHeight"];
        /// <summary>
        /// 投影分辨率高度设置
        /// </summary>
        public string ScreenResulotionHeight
        {
            get { return screenResulotionHeight; }
            set { screenResulotionHeight = value; }
        }


        string localPersonalNoteFile = System.Configuration.ConfigurationManager.AppSettings["LocalPersonalNoteFile"];
        /// <summary>
        /// 本地个人笔记名称
        /// </summary>
        public string LocalPersonalNoteFile
        {
            get { return localPersonalNoteFile; }
            set { }
        }

        string conferenceFtpWebAddressFront = System.Configuration.ConfigurationManager.AppSettings["ConferenceFtpWebAddressFront"];
        /// <summary>
        /// ftp服务地址
        /// </summary>
        public string ConferenceFtpWebAddressFront
        {
            get { return conferenceFtpWebAddressFront; }
            set { }
        }

        string ftpServerceAudioName = System.Configuration.ConfigurationManager.AppSettings["FtpServerceAudioName"];
        /// <summary>
        /// 语音文件上传目录
        /// </summary>
        public string FtpServerceAudioName
        {
            get { return ftpServerceAudioName; }
            set { }
        }

        string ftpServercePersonImgName = System.Configuration.ConfigurationManager.AppSettings["FtpServercePersonImgName"];
        /// <summary>
        /// 用户头像上传目录
        /// </summary>
        public string FtpServercePersonImgName
        {
            get { return ftpServercePersonImgName; }
            set { }
        }

        string treeItemEmptyName = System.Configuration.ConfigurationManager.AppSettings["TreeItemEmptyName"];
        /// <summary>
        /// 智慧树新节点默认名称
        /// </summary>
        public string TreeItemEmptyName
        {
            get { return treeItemEmptyName; }
            set { }
        }

        string recordFolderName = System.Configuration.ConfigurationManager.AppSettings["RecordFolderName"];
        /// <summary>
        /// 录播文件存放地址
        /// </summary>
        public string RecordFolderName
        {
            get { return recordFolderName; }
            set { }
        }

        string recordExtention = System.Configuration.ConfigurationManager.AppSettings["RecordExtention"];
        /// <summary>
        /// 录播文件扩展名
        /// </summary>
        public string RecordExtention
        {
            get { return recordExtention; }
            set { }
        }

        string reacordUploadFileName = System.Configuration.ConfigurationManager.AppSettings["ReacordUploadFileName"];
        /// <summary>
        /// 上传的录制视频名称
        /// </summary>
        public string ReacordUploadFileName
        {
            get { return reacordUploadFileName; }
            set { }
        }

        string keyboardSettingFile_64 = System.Configuration.ConfigurationManager.AppSettings["KeyboardSettingFile_64"];
        /// <summary>
        /// 触摸键盘设置区域（64）
        /// </summary>
        public string KeyboardSettingFile_64
        {
            get { return keyboardSettingFile_64; }
            set { }
        }

        string keyboardSettingFile_32 = System.Configuration.ConfigurationManager.AppSettings["KeyboardSettingFile_32"];
        /// <summary>
        /// 触摸键盘设置区域（32）
        /// </summary>
        public string KeyboardSettingFile_32
        {
            get { return keyboardSettingFile_32; }
            set { }
        }


        string ftpUserName = System.Configuration.ConfigurationManager.AppSettings["FtpUserName"];
        /// <summary>
        /// ftp用户
        /// </summary>
        public string FtpUserName
        {
            get { return ftpUserName; }
            set { }
        }

        string ftpPassword = System.Configuration.ConfigurationManager.AppSettings["FtpPassword"];
        /// <summary>
        /// ftp用户密码
        /// </summary>
        public string FtpPassword
        {
            get { return ftpPassword; }
            set { }
        }

        //string bigScreenName = System.Configuration.ConfigurationManager.AppSettings["BigScreenName"];
        ///// <summary>
        ///// 大屏名称
        ///// </summary>
        //public string BigScreenName
        //{
        //    get { return bigScreenName; }
        //    set {}
        //}            
    }
}
