using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ConferenceWebCommon.Common
{
    public class TcpDateEvent
    {
        /// <summary>
        /// 客户端套接字
        /// </summary>
        public Socket Socket;

        /// <summary>
        /// 判断客户端的实体类（会议名称,当前用户uri名称,客户端连接服务器时响应）
        /// </summary>
        public ConferenceClientAccept ClientIncordingEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="conferenceName"></param>
        /// <param name="selfUri"></param>
        public TcpDateEvent(Socket socket, ConferenceClientAccept clientIncordingEntity)
        {
            try
            {
            this.Socket = socket;
            this.ClientIncordingEntity = clientIncordingEntity;
                 }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }
    }
}
