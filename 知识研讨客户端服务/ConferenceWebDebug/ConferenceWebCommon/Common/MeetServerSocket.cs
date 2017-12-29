using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConferenceWebCommon.Common;
using System.Net.Sockets;
using ConferenceWebCommon.EntityCommon;

namespace ConferenceWebCommon.Common
{
    public class MeetServerSocket
    {       
        /// <summary>
        /// 会议通讯节点
        /// </summary>
        public ServerSocket ServerSocket { get; set; }


        /// <summary>
        /// 会议端口
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// 客户端通讯节点字典集合（）
        /// </summary>
        public Dictionary<string, SocketModel> DicClientSocket = new Dictionary<string, SocketModel>();

        /// <summary>
        /// 服务处理类型
        /// </summary>
        public ConferenceClientAcceptType ConferenceClientAcceptType;

        ///// <summary>
        ///// 判断实例是否为同一个
        ///// </summary>
        ///// <param name="obj">指定实例</param>
        ///// <returns>返回结果</returns>
        //public override bool Equals(object obj)
        //{
        //    //初始化返回结果
        //    bool result = false;
        //    //类型判断
        //    if (obj is MeetServerSocket)
        //    {
        //        //还原类型
        //        MeetServerSocket meetServerSocket = obj as MeetServerSocket;
        //        //判断实例是否为同一个
        //        if(this.ConferenceName.Equals(meetServerSocket.ConferenceName))
        //        {
        //            //名称相同则认定为同一个实例
        //            result = true;
        //        }
        //    }
        //    //返回结果
        //    return result;
        //}
    }

    public class SocketModel
    {
        Socket socket;
        /// <summary>
        /// 套接字
        /// </summary>
        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }

        SocketClientType socketClientType;
        /// <summary>
        /// 套接字对应类型
        /// </summary>
        public SocketClientType SocketClientType
        {
            get { return socketClientType; }
            set { socketClientType = value; }
        }


    }

    public enum SocketClientType
    {
        PC,
        Android,
    }
}
