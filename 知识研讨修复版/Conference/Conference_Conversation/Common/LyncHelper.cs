using ConferenceCommon.AppContainHelper;
using ConferenceCommon.EntityHelper;
using ConferenceCommon.EnumHelper;
using ConferenceCommon.IconHelper;
using ConferenceCommon.ImageHelper;
using ConferenceCommon.LogHelper;
using ConferenceCommon.LyncHelper;
using ConferenceCommon.NetworkHelper;
using ConferenceCommon.ProcessHelper;
using ConferenceCommon.TimerHelper;
using ConferenceCommon.VersionHelper;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using Microsoft.Lync.Model.Conversation.AudioVideo;
using Microsoft.Lync.Model.Conversation.Sharing;
using Microsoft.Lync.Model.Extensibility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using win32 = ConferenceCommon.IconHelper.Win32API;

namespace Conference_Conversation.Common
{
    public partial class LyncHelper
    {
        #region 方法回调

        /// <summary>
        /// datagrid子项更改事件回调
        /// </summary>
        public static Action<ObservableCollection<ParticipantsEntity>> BeginRefleshDataGridCallBack = null;

        /// <summary>
        /// lync签入回调
        /// </summary>
        public static Action StateINCallBack = null;

        /// <summary>
        /// lync签出回调
        /// </summary>
        public static Action StateOutCallBack = null;

        /// <summary>
        /// lync正在签入时签出回调
        /// </summary>
        public static Action StateIN_OutCallBack = null;

        /// <summary>
        /// 查看是否包含会议回调
        /// </summary>
        public static Action<Action<bool>> HasConferenceCallBack = null;

        /// <summary>
        /// 输出atuomation回调
        /// </summary>
        public static Action<ConversationWindow> MainConversationOutCallBack = null;

        /// <summary>
        /// 会话窗体加载内容完成回调
        /// </summary>
        public static Action ConversationAddCompleateCallBack = null;

        /// <summary>
        /// 输入automation回调
        /// </summary>
        public static Action<Action<ConversationWindow>> MainConversationInCallBack = null;

        /// <summary>
        /// 会话加载完成回调
        /// </summary>
        public static Action<string> ContentAddCompleateCallBack = null;

        /// <summary>
        /// 共享内容回调
        /// </summary>
        public static Action Content_DeskRemoveCompleateCallBack = null;

        /// <summary>
        /// 共享窗体回调
        /// </summary>
        public static Action ShareDeskCallBack = null;

        /// <summary>
        /// 附加到新窗体回调
        /// </summary>
        public static Action DockNewWindowCallBack = null;       

        /// <summary>
        /// 返回演示人回调
        /// </summary>
        public static Action<string> PresentCallBack = null;

        /// <summary>
        /// 可签入回调
        /// </summary>
        public static Action CanSiginedCallBack = null;

        /// <summary>
        /// 没有参会人回调
        /// </summary>
        public static Action NoParticalCallBack = null;

        /// <summary>
        /// 加载共享类型回调
        /// </summary>
        public static Action<SharingType> AddContent_Type_CallBack = null;

        /// <summary>
        /// 移除共享类型回调
        /// </summary>
        public static Action<SharingType> RemoveContent_Type_CallBack = null;

        /// <summary>
        /// 添加会话联系人回调
        /// </summary>
        public static Action<Participant> Add_ConversationParticalCallBack = null;

        /// <summary>
        /// 移除会话联系人回调
        /// </summary>
        public static Action<Participant> Remove_ConversationParticalCallBack = null;

        #endregion

        #region 静态字段

        /// <summary>
        /// 共享白板的自定义名称
        /// </summary>
        static string whiteBoardShareName = "所共享的白板";

        /// <summary>
        /// 共享白板数量(默认为0,进行计数)
        /// </summary>
        static int whiteBoardShareCount = 0;

        /// <summary>
        /// 启动会话邀请词
        /// </summary>
        static string MainConversationAccrodingStr = "___邀请会话";

        /// <summary>
        /// 通知接受计时器
        /// </summary>
        static DispatcherTimer timerAcept = null;

        /// <summary>
        /// 参会人列表
        /// </summary>
       internal static ObservableCollection<ParticipantsEntity> currentParticipantsEntityList = new ObservableCollection<ParticipantsEntity>();

        /// <summary>
        /// 不在线(lync)
        /// </summary>
        static string notOnlineAboutLync = "脱机";

        /// <summary>
        /// 不在线(sky)
        /// </summary>
        static string notOnlineAboutSky = "离线";

        /// <summary>
        /// 在线显示
        /// </summary>
        static string onlineShow = "在线";

        /// <summary>
        /// 离线显示
        /// </summary>
        static string notOnlineShow = "未登录";

        /// <summary>
        /// （深灰色）字体颜色
        /// </summary>
        static SolidColorBrush NormalColorBrush = null;

        /// <summary>
        /// 绿色字体
        /// </summary>
        static SolidColorBrush GreenColorBrush = null;

        /// <summary>
        /// 灰色色条
        /// </summary>
        static SolidColorBrush GrayColor = null;

        /// <summary>
        /// 绿色色条
        /// </summary>
        static SolidColorBrush GreenColor = null;

        /// <summary>
        /// 会话窗体状态管理模型
        /// </summary>
        static win32.ManagedWindowPlacement Placement = new win32.ManagedWindowPlacement() { showCmd = 2 };

        /// <summary>
        /// 引导参会浏览器
        /// </summary>
        static System.Windows.Forms.WebBrowser conversationWebBrowser = null;

        /// <summary>
        /// lync人员辅助作用
        /// </summary>
        static LyncContactManage lyncContactManage = null;

        /// <summary>
        /// 主会话
        /// </summary>
        public static ConversationWindow MainConversation = null;
      
        #endregion       

        #region 属性

        static List<ShareableContent> shareableContentList = new List<ShareableContent>();
        /// <summary>
        /// 共享的资源集合
        /// </summary>
        internal static List<ShareableContent> ShareableContentList
        {
            get { return shareableContentList; }
            set { shareableContentList = value; }
        }
      
        #endregion
    }
}
