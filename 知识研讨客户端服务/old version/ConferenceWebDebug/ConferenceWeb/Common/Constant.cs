
using ConferenceWeb.Common;
using ConferenceWebCommon.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Web;

namespace ConferenceWeb
{
    public class Constant
    {
        #region 静态资源

        /// <summary>
        /// 研讨客户端当前版本
        /// </summary>
        public static readonly string ConferencePCVersion = System.Configuration.ConfigurationManager.AppSettings["ConferencePCVersion"];
        /// <summary>
        /// 需要更新的文件（dll,exe,config,cer）
        /// </summary>
        public static readonly string UpdateFile = System.Configuration.ConfigurationManager.AppSettings["UpdateFile"];
        /// <summary>
        /// 更新根目录
        /// </summary>
        public static readonly string UpdateRootFile = System.Configuration.ConfigurationManager.AppSettings["UpdateRootFile"];
        /// <summary>
        /// 本地IP
        /// </summary>
        public static readonly string ServerAddress = System.Configuration.ConfigurationManager.AppSettings["LocalIP"];
        /// <summary>
        /// 服务端口
        /// </summary>
        //public static int ServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ServerPoint"]);

        /// <summary>
        /// 知识树服务端口
        /// </summary>
        public static int TreeServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["TreeServerPoint"]);

        /// <summary>
        /// 语音服务端口
        /// </summary>
        public static int AudioServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AudioServerPoint"]);

        /// <summary>
        /// 消息服务端口
        /// </summary>
        public static int InfoServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["InfoServerPoint"]);

        /// <summary>
        /// Lync服务端口
        /// </summary>
        public static int LyncServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["LyncServerPoint"]);

        /// <summary>
        /// 文件服务端口【甩屏】
        /// </summary>
        public static int FileServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FileServerPoint"]);

        /// <summary>
        /// 文件服务端口【甩屏】
        /// </summary>
        public static int SpaceServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["SpaceServerPoint"]);

        /// <summary>
        /// 矩阵应用端口
        /// </summary>
        public static int MatrixServerPoint = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MatrixServerPoint"]);

        /// <summary>
        /// 本地语音存储文件夹名称
        /// </summary>
        public static readonly string AudioLocalRootName = System.Configuration.ConfigurationManager.AppSettings["AudioLocalRootName"];
        /// <summary>
        /// 会议信息文本
        /// </summary>
        public static readonly string MeetFileName = System.Configuration.ConfigurationManager.AppSettings["MeetFileName"];

        /// <summary>
        /// 投影分辨率宽度设置
        /// </summary>
        public static readonly string ScreenResulotionWidth = System.Configuration.ConfigurationManager.AppSettings["ScreenResulotionWidth"];

        /// <summary>
        /// 投影分辨率高度设置
        /// </summary>
        public static readonly string ScreenResulotionHeight = System.Configuration.ConfigurationManager.AppSettings["ScreenResulotionHeight"];

        ///// <summary>
        ///// 座位IP集合
        ///// </summary>
        //public static readonly string SettingIpList = System.Configuration.ConfigurationManager.AppSettings["SettingIpList"];

        /// <summary>
        /// 个人头像存储位置
        /// </summary>
        public static readonly string PersonImgHttp = System.Configuration.ConfigurationManager.AppSettings["PersonImgHttp"];

        /// <summary>
        /// 用户域名
        /// </summary>
        public static readonly string UserDomain = System.Configuration.ConfigurationManager.AppSettings["UserDomain"];

        /// <summary>
        /// 音频文件存储实际目录
        /// </summary>
        public static readonly string AudioTempHttp = System.Configuration.ConfigurationManager.AppSettings["AudioTempHttp"];

        #region 会议预定信息登陆

        /// <summary>
        /// 域名
        /// </summary>
        public static readonly string UserDoaminPart1Name = System.Configuration.ConfigurationManager.AppSettings["UserDoaminPart1Name"];

        /// <summary>
        /// 会议预定信息登陆用户
        /// </summary>
        public static readonly string ReservationLoginUser = System.Configuration.ConfigurationManager.AppSettings["ReservationLoginUser"];

        /// <summary>
        /// 会议预定信息登陆密码
        /// </summary>
        public static readonly string ReservationLoginPwd = System.Configuration.ConfigurationManager.AppSettings["ReservationLoginPwd"];

        /// <summary>
        /// 是否需要预定会议信息
        /// </summary>
        public static readonly bool IsNeedReservationInfo = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["IsNeedReservationInfo"]);

        /// <summary>
        /// 预定会议信息服务地址
        /// </summary>
        public static readonly string RevertWebServiceUri = System.Configuration.ConfigurationManager.AppSettings["RevertWebServiceUri"];

        #endregion

        #region 服务对应处理区域集合

        private static Dictionary<string, MeetServerSocket> dicAudioMeetServerSocket;
        /// <summary>
        /// 语音服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicAudioMeetServerSocket
        {
            get
            {
                if (dicAudioMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicAudioMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicAudioMeetServerSocket;
            }
            set
            {
                Constant.dicAudioMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicTreeMeetServerSocket;
        /// <summary>
        /// 知识树服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicTreeMeetServerSocket
        {
            get
            {
                if (dicTreeMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicTreeMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicTreeMeetServerSocket;
            }
            set
            {
                Constant.dicTreeMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicInfoMeetServerSocket;
        /// <summary>
        /// 消息服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicInfoMeetServerSocket
        {
            get
            {
                if (dicInfoMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicInfoMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicInfoMeetServerSocket;
            }
            set
            {
                Constant.dicInfoMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicLyncMeetServerSocket;
        /// <summary>
        /// Lync服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicLyncMeetServerSocket
        {
            get
            {
                if (dicLyncMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicLyncMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicLyncMeetServerSocket;
            }
            set
            {
                Constant.dicLyncMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicFileMeetServerSocket;
        /// <summary>
        /// Lync服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicFileMeetServerSocket
        {
            get
            {
                if (dicFileMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicFileMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicFileMeetServerSocket;
            }
            set
            {
                Constant.dicFileMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicSpaceMeetServerSocket;
        /// <summary>
        /// Lync服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicSpaceMeetServerSocket
        {
            get
            {
                if (dicSpaceMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicSpaceMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicSpaceMeetServerSocket;
            }
            set
            {
                Constant.dicSpaceMeetServerSocket = value;
            }
        }

        private static Dictionary<string, MeetServerSocket> dicMatrixMeetServerSocket;
        /// <summary>
        /// 矩阵应用服务套接字集合
        /// </summary>
        public static Dictionary<string, MeetServerSocket> DicMatrixMeetServerSocket
        {
            get
            {
                if (dicMatrixMeetServerSocket == null)
                {
                    try
                    {
                        //服务器通讯节点字典集合
                        dicMatrixMeetServerSocket = new Dictionary<string, MeetServerSocket>();
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(typeof(Constant), ex);
                    }
                }
                return Constant.dicMatrixMeetServerSocket;
            }
            set
            {
                Constant.dicMatrixMeetServerSocket = value;
            }
        }

        #endregion

        /// <summary>
        /// 当前会议数量
        /// </summary>
        public static int MeetCount = 0;

        #endregion

        #region 通讯区域

        /// <summary>
        /// 服务器通讯节点初始化
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <returns>返回端口号</returns>
        public static int ServerSocketInitAndRetunPort(string conferenceName, ConferenceClientAcceptType conferenceClientAcceptType)
        {
            int ConferencePort = 0;
            try
            {
                //声明一个服务集合
                Dictionary<string, MeetServerSocket> DicmeetServerSocket = null;
                switch (conferenceClientAcceptType)
                {
                    case ConferenceClientAcceptType.ConferenceTree:
                        //知识树服务
                        DicmeetServerSocket = Constant.DicTreeMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.ConferenceAudio:
                        //语音服务
                        DicmeetServerSocket = Constant.DicAudioMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.ConferenceFileSync:
                        //甩屏服务
                        DicmeetServerSocket = Constant.DicFileMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.ConferenceSpaceSync:
                        //甩屏服务
                        DicmeetServerSocket = Constant.DicSpaceMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.ConferenceInfoSync:
                        //消息服务
                        DicmeetServerSocket = Constant.DicInfoMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.LyncConversationSync:
                        //lync通讯服务
                        DicmeetServerSocket = Constant.DicLyncMeetServerSocket;
                        break;
                    case ConferenceClientAcceptType.ConferenceMatrixSync:
                        //矩阵应用
                        DicmeetServerSocket = Constant.DicMatrixMeetServerSocket;
                        break;
                    default:
                        break;
                }
                //判断是否包含该会议通讯节点
                if (!DicmeetServerSocket.ContainsKey(conferenceName))
                {

                    //生成一个服务器通讯节点
                    MeetServerSocket meetServerSocket = new MeetServerSocket();
                    //服务处理类型
                    meetServerSocket.ConferenceClientAcceptType = conferenceClientAcceptType;

                    //加载一个新的会议节点
                    DicmeetServerSocket.Add(conferenceName, meetServerSocket);
                    //设置会议的数量
                    Constant.MeetCount = DicmeetServerSocket.Count;

                    //服务器通讯节点初始化辅助
                    Constant.DicServerSocketInitHelper(meetServerSocket);
                    //返回端口
                    ConferencePort = meetServerSocket.ServerPort;
                }
                else
                {
                    //返回端口
                    ConferencePort = DicmeetServerSocket[conferenceName].ServerPort;
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
            //返回指定端口
            return ConferencePort;
        }

        /// <summary>
        /// 服务器通讯节点集合初始化辅助
        /// </summary>
        public static void DicServerSocketInitHelper(MeetServerSocket meetServerSocket)
        {
            try
            {
                int point = 0;
                switch (meetServerSocket.ConferenceClientAcceptType)
                {
                    case ConferenceClientAcceptType.ConferenceTree:
                        //知识树服务端口绑定
                        point = Constant.TreeServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket TreeServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        TreeServerSocket.TCPDataArrival += TreeServerSocket_TCPDataArrival;
                        break;

                    case ConferenceClientAcceptType.ConferenceAudio:
                        //语音服务端口绑定
                        point = Constant.AudioServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket AudioServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        AudioServerSocket.TCPDataArrival += AudioServerSocket_TCPDataArrival;
                        //获取手机端信息
                        AudioServerSocket.TCPDataAccroiding += AudioServerSocket_TCPDataAccroiding;
                        break;

                    case ConferenceClientAcceptType.ConferenceFileSync:
                        //文件服务端口绑定【甩屏】
                        point = Constant.FileServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket FileServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        FileServerSocket.TCPDataArrival += FileServerSocket_TCPDataArrival;
                        break;

                    case ConferenceClientAcceptType.ConferenceSpaceSync:
                        //文件服务端口绑定【甩屏】
                        point = Constant.SpaceServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket OfficeServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        OfficeServerSocket.TCPDataArrival += SpaceServerSocket_TCPDataArrival;
                        break;

                    case ConferenceClientAcceptType.ConferenceInfoSync:
                        //消息服务端口绑定
                        point = Constant.InfoServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket InfoServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        InfoServerSocket.TCPDataArrival += InfoServerSocket_TCPDataArrival;
                        break;

                    case ConferenceClientAcceptType.LyncConversationSync:
                        //lync服务端口绑定
                        point = Constant.LyncServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket LyncServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        LyncServerSocket.TCPDataArrival += LyncServerSocket_TCPDataArrival;
                        break;

                    case ConferenceClientAcceptType.ConferenceMatrixSync:
                        //lync服务端口绑定
                        point = Constant.MatrixServerPoint;
                        if (Constant.MeetCount > 1)
                        {
                            //多个会议,合理分配端口
                            point += Constant.MeetCount * 7;
                        }
                        //服务器通讯节点初始化辅助
                        ServerSocket MatrixServerSocket = Constant.ServerSocketInitHelper(meetServerSocket, point);
                        //获取客户端信息
                        MatrixServerSocket.TCPDataArrival += MatrixServerSocket_TCPDataArrival;
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 判断是否为手机端还是pc端传来的数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="isMobileSend"></param>
        static void AudioServerSocket_TCPDataAccroiding(string data, Action<bool, string, string> CallBack)
        {
            try
            {
                if (data.Contains(InnerResourceHelper.MobileConnectCommondSplitChar))
                {
                    var list = data.Split(InnerResourceHelper.MobileConnectCommondSplitChar);
                    if (list.Count() == 2)
                    {
                        CallBack(true, list[0], list[1]);
                    }
                    else
                    {
                        CallBack(false, null, null);
                    }
                }
                else
                {
                    CallBack(false, null, null);
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
            finally
            {

            }
        }

        //public int PointDealWidth(int point)
        //{                 
        //    if (point > 1)
        //    {
        //        //多个会议,合理分配端口
        //        point += point * 5;
        //    }  

        //}

        /// <summary>
        /// 服务器通讯节点初始化辅助
        /// </summary>
        public static ServerSocket ServerSocketInitHelper(MeetServerSocket meetServerSocket, int point)
        {
            ServerSocket serverSocket = null;
            try
            {
                //指派一个端口
                meetServerSocket.ServerPort = point;
                //生成一个服务套接字
                meetServerSocket.ServerSocket = new ServerSocket();
                //开启侦听
                meetServerSocket.ServerSocket.Listen(IPAddress.Parse(Constant.ServerAddress), point);
                //返回的服务套接字
                serverSocket = meetServerSocket.ServerSocket;
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
            finally
            {

            }
            return serverSocket;
        }

        #endregion

        #region 获取客户端信息

        /// <summary>
        /// 知识树服务
        /// </summary>
        /// <param name="args"></param>
        static void TreeServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicTreeMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// 语音服务
        /// </summary>
        /// <param name="args"></param>
        static void AudioServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicAudioMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// 消息服务
        /// </summary>
        /// <param name="args"></param>
        static void InfoServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicInfoMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// 文件同步【甩屏】
        /// </summary>
        /// <param name="args"></param>
        static void FileServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicFileMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// lync服务
        /// </summary>
        /// <param name="args"></param>
        static void LyncServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicLyncMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// 智存空间
        /// </summary>
        /// <param name="args"></param>
        static void SpaceServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicSpaceMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        /// <summary>
        /// 矩阵应用
        /// </summary>
        /// <param name="args"></param>
        static void MatrixServerSocket_TCPDataArrival(ConferenceWebCommon.Common.TcpDateEvent args, SocketClientType socketClientType)
        {
            try
            {
                //填充客户端套接字
                FillsocDic(DicMatrixMeetServerSocket, args.ClientIncordingEntity.ConferenceName, args.ClientIncordingEntity.SelfUri, args.Socket, socketClientType);
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        #endregion

        #region 填充客户端套接字

        /// <summary>
        /// 填充客户端套接字
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="contactUrl">参会人</param>
        /// <param name="socket">套接字</param>
        public static void FillsocDic(Dictionary<string, MeetServerSocket> dicServierSocket, string conferenceName, string contactUrl, Socket socket, SocketClientType socketClientType)
        {
            try
            {
                //查看是否包含该会议通讯节点
                if (dicServierSocket.ContainsKey(conferenceName))
                {
                    SocketModel socketModel = new SocketModel() { Socket = socket, SocketClientType = socketClientType };
                    //会议的通信节点集合
                    Dictionary<string, SocketModel> dicSocketModels = dicServierSocket[conferenceName].DicClientSocket;

                    //该会议通讯节点是否包含该客户端通讯节点（处于一对多的映射关系）
                    if (!dicSocketModels.ContainsKey(contactUrl))
                    {
                        //该会议通讯节点添加该客户端的通讯节点
                        dicSocketModels.Add(contactUrl, socketModel);
                    }
                    else if (dicSocketModels.ContainsKey(contactUrl) && !dicSocketModels[contactUrl].Socket.Connected)
                    {
                        //该会议通讯节点添加该客户端的通讯节点
                        dicSocketModels[contactUrl] = socketModel;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        #endregion

        #region 会议通讯节点信息发送管理中心

        /// <summary>
        /// 会议通讯节点信息发送管理中心
        /// </summary>
        /// <param name="conferenceName">会议名称</param>
        /// <param name="pack">数据包</param>
        public static void SendClientCenterManage(Dictionary<string, MeetServerSocket> dicMeetServerSocket, string conferenceName, PackageBase pack)
        {
            try
            {
                //会议通讯节点字典集合是否包含此项
                if (dicMeetServerSocket.ContainsKey(conferenceName))
                {
                    MeetServerSocket meetServerSocket = dicMeetServerSocket[conferenceName];
                    //获取会议通讯节点
                    ServerSocket ServerSocket = meetServerSocket.ServerSocket;
                    //获取客户端通讯节点字典集合
                    Dictionary<string, SocketModel> dicSocket = meetServerSocket.DicClientSocket;
                    //Thread.Sleep(100);
                    //遍历客户端通讯节点字典集合
                    for (int i = dicSocket.Count - 1; i > -1; i--)
                    {
                        SocketModel socketModel = dicSocket.Values.ElementAt(i);
                        switch (socketModel.SocketClientType)
                        {
                            case SocketClientType.PC:

                                //会议通讯节点给客户端发送信息
                                ServerSocket.ServerSendData(socketModel.Socket, pack, new Action<bool>((successed) =>
                                {
                                     //socket.Poll(10, SelectMode.SelectRead) || !socket.Connected)
                                    Socket socket = socketModel.Socket;
                                    if (!successed ||ServerSendData(socket,10))
                                    {
                                        //错误返回标示，异常废弃的客户端通讯节点
                                        dicSocket.Remove(dicSocket.Keys.ElementAt(i));
                                    }
                                }));
                                break;

                            case SocketClientType.Android:

                                if (pack.ConferenceAudioItemTransferEntity != null && pack.ConferenceAudioItemTransferEntity.Operation == ConferenceWebCommon.EntityHelper.ConferenceDiscuss.ConferenceAudioOperationType.AddType)
                                {
                                    //传输json
                                    string json = CommonMethod.Serialize(pack);

                                    //会议通讯节点给手机端发送信息
                                    ServerSocket.ServerSendData(socketModel.Socket, json, new Action<bool>((successed) =>
                                     {
                                         Socket socket = socketModel.Socket;
                                         if (!successed || ServerSendData(socket,10))
                                         {
                                             //错误返回标示，异常废弃的客户端通讯节点
                                             dicSocket.Remove(dicSocket.Keys.ElementAt(i));
                                             //LogManage.WriteLog(typeof(Constant), "已作废一个节点，该节点为" + dicSocket.Keys.ElementAt(i));
                                         }
                                     }));
                                }
                                break;
                            default:
                                break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
            finally
            {

            }
        }

        #endregion

        //#region 获取本地IP

        //public static string GetLocalIP()
        //{
        //    string ip = string.Empty;
        //    try
        //    {
        //        System.Net.IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        //        for (int i = 0; i < addressList.Length; i++)
        //        {
        //            ip = addressList[i].ToString();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LogManage.WriteLog(typeof(Constant), ex);
        //    }
        //    return ip;
        //}

        //#endregion

        #region 获取版本号

        /// <summary>
        /// 获取版本号
        /// </summary>
        public static void GetPCVersionByFile()
        {
            try
            {
               

            }
            catch (Exception ex)
            {
                LogManage.WriteLog(typeof(Constant), ex);
            }
        }

        #endregion

        #region 心跳验证
     
        /// <summary>
        /// 服务端往客户端发送消息（心跳验证）
        /// </summary>
        /// <param name="socket">目标Socket</param>
         public static bool ServerSendData(Socket socket,int millseconds)
        {
            bool isConnected = true;
            try
            {
                if (socket != null
                   && socket.Connected)
                {
                    if (socket.Poll(millseconds, SelectMode.SelectRead))
                    {
                        if (socket.Available == 0)
                        {
                            isConnected = false;
                        }
                        else
                        {
                            isConnected = true;
                        }
                    }                   
                }
                else
                {
                    isConnected = false;
                }
            }
            catch (Exception )
            {
                isConnected = false;
                return isConnected;
            }

            return isConnected;
        }
       

        #endregion
    }
}