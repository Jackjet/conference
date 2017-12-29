using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.FileHelper;
using ConferenceCommon.SharePointHelper;
using ConferenceModel.ConferenceInfoWebService;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using config = System.Configuration.ConfigurationManager;


namespace Conference
{
    class Constant
    {
        /// <summary>
        /// DNS首选服务器
        /// </summary>
        public static string DNS1;

        /// <summary>
        /// DNS备用服务器
        /// </summary>
        public static string DNS2;

        /// <summary>
        /// lync名称
        /// </summary>
        public static string LyncName;

        /// <summary>
        /// lync证书所在地
        /// </summary>
        public static string Certification;

        /// <summary>
        /// lync证书序列号
        /// </summary>
        public static string CertificationSerial;

        /// <summary>
        /// 域名
        /// </summary>
        public static string UserDomain;

        /// <summary>
        /// 域名（前一部分名称）
        /// </summary>
        public static string UserDoaminPart1Name;

        /// <summary>
        /// 用户实体路径
        /// </summary>
        public static string UserFilePath = Environment.CurrentDirectory + "\\" + config.AppSettings["UserFilePath"].ToString();

        /// <summary>
        /// 设备实体路径
        /// </summary>
        public static string SettingFilePath = Environment.CurrentDirectory + "\\" + config.AppSettings["SettingFilePath"].ToString();

        /// <summary>
        /// PaintFileName文件名称
        /// </summary>
        public static string PaintFileName = config.AppSettings["PaintFileName"].ToString();

        /// <summary>
        /// PaintFileRoot路径
        /// </summary>
        public static string FileRoot = Environment.CurrentDirectory + "\\" + config.AppSettings["FileRoot"].ToString();

        /// <summary>
        /// 防火墙添加规则名称（研讨客户端）
        /// </summary>
        public static string FireName_Conference = config.AppSettings["FireName_Conference"].ToString();

        /// <summary>
        /// 防火墙添加规则名称（研讨客户端更新程序）
        /// </summary>
        public static string FireName_ConferenceUpdate = config.AppSettings["FireName_ConferenceUpdate"].ToString();

        /// <summary>
        /// lync1的IP
        /// </summary>
        public static string LyncIP1;

        /// <summary>
        /// lync的IP
        /// </summary>
        public static string LyncIP2;


        /// <summary>
        /// 应用程序名称(全名称)
        /// </summary>
        public static string ApplicationFullName = Environment.CurrentDirectory + "\\" + config.AppSettings["ApplicationFullName"].ToString();

        /// <summary>
        /// 研讨终端更新程序
        /// </summary>
        public static string ConferenceVersionUpdateExe = AppDomain.CurrentDomain.BaseDirectory + config.AppSettings["ConferenceVersionUpdateExe"];

        /// <summary>
        /// SharePoint服务IP地址
        /// </summary>
        public static string SPSiteAddressFront;

        /// <summary>
        /// 会议xml文件存储名称
        /// </summary>
        public static string TreeXmlFileName;

        /// <summary>
        /// 会议jpg文件存储名称
        /// </summary>
        public static string TreeJpgFileName;

        /// <summary>
        /// 会议存储文件夹名称
        /// </summary>
        public static string MeetingFolderName;

        /// <summary>
        /// 个人存储文件夹名称
        /// </summary>
        public static string PesonalFolderName;

        /// <summary>
        /// rpc服务名称
        /// </summary>
        public static string RpcLocatorName = config.AppSettings["RpcLocatorName"];

        /// <summary>
        /// 智存空间地址
        /// </summary>
        public static string SpaceWebSiteUri;

        /// <summary>
        /// 分辨率宽度
        /// </summary>

        public static string ScreenResulotionWidth;

        /// <summary>
        /// 分变率高度
        /// </summary>
        public static string ScreenResulotionHeight;

        /// <summary>
        /// 大屏幕名称
        /// </summary>
        public static string BigScreenName;


        /// <summary>
        /// 本地个人笔记名称
        /// </summary>
        public static string LocalPersonalNoteFile;

        /// <summary>
        /// Tree服务IP
        /// </summary>
        public static readonly string TreeServiceIP = config.AppSettings["TreeServiceIP"];

        /// <summary>
        /// Tree服务web服务地址
        /// </summary>
        public static readonly string TreeServiceAddressFront = config.AppSettings["TreeServiceAddressFront"];

        ///// <summary>
        ///// audio服务web服务地址
        ///// </summary>
        //public static readonly string AudioServiceAddressFront = config.AppSettings["AudioServiceAddressFront"];

        ///// <summary>
        ///// 其他服务web服务地址
        ///// </summary>
        //public static readonly string OtherServiceAddressFront = config.AppSettings["OtherServiceAddressFront"];



        /// <summary>
        /// 服务区缓存文件夹
        /// </summary>
        public static readonly string ServicePPTTempFile = config.AppSettings["ServicePPTTempFile"];

        /// <summary>
        /// 本地缓存目录
        /// </summary>
        public static string LocalTempRoot = Environment.CurrentDirectory + "\\" + config.AppSettings["LocalTempRoot"].ToString();

        /// <summary>
        /// 邀请文件
        /// </summary>
        public static string InviteFile = config.AppSettings["InviteFile"].ToString();

        /// <summary>
        /// 研讨同步服务目录（树）
        /// </summary>
        public static readonly string ConferenceTreeServiceWebName = config.AppSettings["ConferenceTreeServiceWebName"];

        /// <summary>
        /// 研讨同步服务目录(语音)
        /// </summary>
        public static readonly string ConferenceAudioServiceWebName = config.AppSettings["ConferenceAudioServiceWebName"];

        /// <summary>
        /// 版本更新目录
        /// </summary>
        public static readonly string ConferenceVersionWebName = config.AppSettings["ConferenceVersionWebName"];

        /// <summary>
        /// 研讨同步服务目录(文件同步)
        /// </summary>
        public static readonly string FileSyncWebName = config.AppSettings["FileSyncWebName"];

        /// <summary>
        /// 获取word同步服务地址
        /// </summary>
        public static readonly string ConferenceWordAsyncWebName = config.AppSettings["ConferenceWordAsyncWebName"];

        /// <summary>
        /// 研讨同步服务目录(lync会话)
        /// </summary>
        public static readonly string ConferenceLyncConversationWebName = config.AppSettings["ConferenceLyncConversationWebName"];

        /// <summary>
        /// 获取信息同步服务地址
        /// </summary>
        public static readonly string ConferenceInfoWebName = config.AppSettings["ConferenceInfoWebName"];

        /// <summary>
        /// 获取矩阵同步服务地址
        /// </summary>
        public static readonly string ConferenceMatrixWebName = config.AppSettings["ConferenceMatrixWebName"];

        /// <summary>
        /// 搜索服务
        /// </summary>
        public static readonly string SpsSearchWebName = config.AppSettings["SpsSearchWebName"];

        /// <summary>
        /// 网页客户端地址
        /// </summary>
        public static string ConferenceWebAppAddress = config.AppSettings["ConferenceWebAppAddress"];

        /// <summary>
        /// 网页客户端地址分隔符
        /// </summary>
        public static string ConferenceWebAppAddress_Split = config.AppSettings["ConferenceWebAppAddress_Split"];

        /// <summary>
        /// 默认头像
        /// </summary>
        public static string DefaultPersonImg = config.AppSettings["DefaultPersonImg"];


        /// <summary>
        /// 研讨ftp服务地址
        /// </summary>
        public static string ConferenceFtpWebAddressFront;

        /// <summary>
        /// ftp音频文件目录（）
        /// </summary>
        public static string FtpServerceAudioName;

        /// <summary>
        /// 用户头像文件目录（）
        /// </summary>
        public static string FtpServercePersonImgName;



        /// <summary>
        /// 语音文件目录
        /// </summary>
        public static string AudioFile_Root = AppDomain.CurrentDomain.BaseDirectory + config.AppSettings["AudioFile_Root"];
       
        /// <summary>
        /// 语音文件名称
        /// </summary>
        public static string AudioFile_Name = config.AppSettings["AudioFile_Name"];

        /// <summary>
        /// 语音文件类型
        /// </summary>
        public static string AudioFile_Extention = config.AppSettings["AudioFile_Extention"];

        /// <summary>
        /// pdf转换应用程序名称
        /// </summary>
        public static string PdfTransferAppName = Environment.CurrentDirectory +  config.AppSettings["PdfTransferAppName"];

        /// <summary>
        /// 场景模式
        /// </summary>
        public static ContextualModelType ContextualModel = (ContextualModelType)Convert.ToInt32(config.AppSettings["ContextualModel"]);

        /// <summary>
        /// 路由器ip
        /// </summary>
        public static string RouteIp = null;

        /// <summary>
        /// 当前终端端版本
        /// </summary>
        public static string CurrentVersion = config.AppSettings["CurrentVersion"];

        /// <summary>
        /// 智慧树新节点默认名称
        /// </summary>
        public static string TreeItemEmptyName;

        /// <summary>
        /// 录播文件存放地址
        /// </summary>
        public static string RecordFolderName;

        /// <summary>
        /// 录播文件扩展名
        /// </summary>
        public static string RecordExtention;

        /// <summary>
        /// 上传的录制视频名称
        /// </summary>
        public static string ReacordUploadFileName;

        /// <summary>
        /// 触摸键盘设置区域（64）
        /// </summary>
        public static string KeyboardSettingFile_64;

        /// <summary>
        /// 触摸键盘设置区域（32）
        /// </summary>
        public static string KeyboardSettingFile_32;

        /// <summary>
        /// 申明客户端
        /// </summary>
        public static LyncClient lyncClient;

        /// <summary>
        /// lync(automation)
        /// </summary>
        public static Automation lyncAutomation;

        /// <summary>
        /// ftp用户
        /// </summary>
        public static string FtpUserName = string.Empty;

        /// <summary>
        /// ftp用户密码
        /// </summary>
        public static string FtpPassword = string.Empty;

        /// <summary>
        /// 是否为会议主持人
        /// </summary>
        public static bool IsMeetingPresenter = false;

        /// <summary>
        /// 参会人列表
        /// </summary>
        public static List<string> ParticipantList = new List<string>();
       
        /// <summary>
        /// 当前参加的研讨会议名称
        /// </summary>
        public static string ConferenceName = string.Empty;

        /// <summary>
        /// 当前参加的研讨会议室名称
        /// </summary>
        public static string ConferenceRoomName = string.Empty;

        /// <summary>
        /// 当前参加的研讨会主持人
        /// </summary>
        public static string ConferenceHost = string.Empty;

        /// <summary>
        /// 会议ID
        /// </summary>
        public static int MeetingID;

        /// <summary>
        /// 当前用户名称
        /// </summary>
        public static string SelfName = string.Empty;

        /// <summary>
        /// 智存空间登陆用户名
        /// </summary>
        public static string WebLoginUserName = string.Empty;

        /// <summary>
        /// 登陆用户名（single_Left）
        /// </summary>
        public static string LoginUserName = string.Empty;

        /// <summary>
        /// 智存空间登陆密码
        /// </summary>
        public static string WebLoginPassword = string.Empty;

        /// <summary>
        /// 当前用户Url地址
        /// </summary>
        public static string SelfUri = string.Empty;

        /// <summary>
        /// 投票图表对比标题
        /// </summary>
        public static List<string> VoteChatTittleList = new List<string>() { "赞成", "反对" };      

        /// <summary>
        /// 用户设置实体
        /// </summary>
        public static SettingEntity SettingEntity = FileManage.Load_EntityInXml<SettingEntity>(Constant.SettingFilePath);

        /// <summary>
        /// 登陆窗体（未使用,待议）
        /// </summary>
        //public static LoginWindow loginWindow = null;

        /// <summary>
        /// 当前机器的IP地址
        /// </summary>
        public static string LocalIp;

        /// <summary>
        /// 第一次运行时的屏幕宽度
        /// </summary>
        public static int FirstRunScreenWidth;

        /// <summary>
        /// 第一次运行时的屏幕高度
        /// </summary>
        public static int FirstRunScreenHeight;

        /// <summary>
        /// lync会话是否为全屏
        /// </summary>
        public static bool IsMainConversationWindowFullScreen = false;

        /// <summary>
        /// 网络浏览地址
        /// </summary>
        public static string WebUri;

        #region 正则表达式【old solution】

        ///// <summary>
        ///// http
        ///// </summary>
        //public static string HttpRegexPattern = @"^http://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$";

        #endregion

        /// <summary>
        /// 教育模式标题1
        /// </summary>
        public static string EducationTittle1 = "当前课堂：";
        /// <summary>
        /// 教育模式标题2
        /// </summary>
        public static string EducationTittle2 = "当前老师：";

        /// <summary>
        /// 研讨模式标题1
        /// </summary>
        public static string DiscussTittle1 = "当前会议：";     
        /// <summary>
        /// 研讨模式标题2
        /// </summary>
        public static string DiscussTittle2 = "当前参会人：";

        /// <summary>
        /// 会议纪要模板名称
        /// </summary>
        public static string ConferenceCommentHtmlTemp = config.AppSettings["ConferenceCommentHtmlTemp"];

        /// <summary>
        /// 会议室摄像头地址
        /// </summary>
        public static string ConferenceRoomCameraAddress = "UDP://@234.1.2.3:1234";

        /// <summary>
        /// 大屏投影应用程序文件名称
        /// </summary>
        public static string CkoAirFileName =  "CkoAir.exe";

        /// <summary>
        /// url文件名称
        /// </summary>
        public static string urlStoreFileName = "urlData.bin";      

        /// <summary>
        /// SharePoint客户端对象模型管理
        /// </summary>
        public static ClientContextManage clientContextManage = new ClientContextManage();      

        /// <summary>
        /// 当前用户公司
        /// </summary>
        public static string SelfCompony = string.Empty;

        /// <summary>
        /// 当前用户职位
        /// </summary>
        public static string SelfPosition = string.Empty;

        /// <summary>
        /// 临时存储的会议信息
        /// </summary>
        public static ConferenceInformationEntityPC TempConferenceInformationEntity = null;
    }
}
