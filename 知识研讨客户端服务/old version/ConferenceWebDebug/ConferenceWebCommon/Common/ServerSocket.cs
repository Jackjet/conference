
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConferenceWebCommon.Common
{
    public class ServerSocket
    {
        /// <summary>
        /// 
        /// </summary>
        private const int BUFFER_SIZE = 1024 * 10;

        #region 变量

        /// <summary>
        /// 服务器端是否启动侦听的标识
        /// </summary>
        private volatile bool _start = false;
        /// <summary>
        /// 启动获取客户端向服务器发送连接的线程
        /// </summary>
        private Thread _tcpServerListenThread;

        /// <summary>
        /// 启动获取客户端连接数据的线程
        /// </summary>
        private Thread _connectDataThread;

        /// <summary>
        /// 服务器端开始侦听的Socket
        /// </summary>
        private Socket _tcpServerSocket;

        #endregion

        #region 委托 && 事件

        public delegate void TCPDataArrivalEventHandler(TcpDateEvent args, SocketClientType socketClientType);

        public event TCPDataArrivalEventHandler TCPDataArrival = null;


        public delegate void TCPDataAccroidingEventHandler(string data, Action<bool, string, string> isMobileSend);

        public event TCPDataAccroidingEventHandler TCPDataAccroiding = null;

        #endregion

        #region 开启侦听

        /// <summary>
        /// 监听
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口号</param>
        public void Listen(IPAddress ipAddress, int port)
        {
            try
            {
                //创建套接字
                this._tcpServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //将该套接字绑定到指定端口
                this._tcpServerSocket.Bind(new IPEndPoint(ipAddress, port));
                //标示为已侦听状态
                this._start = true;
                this._tcpServerSocket.ReceiveTimeout = 3000;
                this._tcpServerSocket.SendTimeout = 3000;
                //开始侦听
                this._tcpServerSocket.Listen(10);
                //开启一个新的子线程
                this._tcpServerListenThread = new Thread(new ThreadStart(this.GetClientConnectSocket));
                //指定该线程为后台线程
                this._tcpServerListenThread.IsBackground = true;
                //启动该子线程
                this._tcpServerListenThread.Start();
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

        #region 获取客户端连接

        /// <summary>
        /// 获取客户端的连接（）
        /// </summary>
        private void GetClientConnectSocket()
        {
            //标示为已侦听状态
            this._start = true;
            do
            {
                try
                {
                    //套接字已进行了侦听
                    if (this._tcpServerSocket != null)
                    {
                        //挂起套接字,一旦有远程去连接这个套接字的端口，将执行后续操作
                        Socket client = this._tcpServerSocket.Accept();//挂起操作
                        client.ReceiveTimeout = 3000;
                        client.SendTimeout = 3000;
                        //判断套接字与客户端的连接状态
                        if (client.Connected)
                        {
                            //获取客户端发送过来的数据
                            this._connectDataThread = new Thread(new ParameterizedThreadStart(this.GetRecieveData));
                            //指定该线程为后台线程
                            this._connectDataThread.IsBackground = true;
                            //启动该子线程
                            this._connectDataThread.Start(client);//parameter
                        }
                    }
                }
                catch (SocketException ex)
                {
                    if (ex.SocketErrorCode != SocketError.Interrupted)
                    {
                        //服务器关闭侦听
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                }
                catch (Exception ex)
                {
                    LogManage.WriteLog(this.GetType(), ex);
                }
            } while (this._start);
        }

        #endregion

        #region 获取客户端信息

        /// <summary>
        /// 获取客户端发送过来的数据
        /// </summary>
        /// <param name="obj"></param>
        private void GetRecieveData(object obj)
        {
            try
            {
                //客户端套接字
                if (obj is Socket)
                {
                    Socket clientSocket = obj as Socket;
                    //连接状态
                    while (clientSocket != null
                        && clientSocket.Connected)
                    {
                        //IPEndPoint remoteEndPoint = (IPEndPoint)client.RemoteEndPoint;//服务器对客户端操作的地址加端口
                        //PC客户端所传数据
                        ConferenceClientAccept clientIncordingEntity = null;

                        List<byte> lists = new List<byte>();
                    callBack:
                        byte[] buffer = new byte[BUFFER_SIZE];

                        int count = clientSocket.Receive(buffer);//挂起操作


                        if (count == 0)
                        {
                            //客户端与服务器端断开连接
                            break;
                        }
                        if (count == BUFFER_SIZE)
                        {
                            lists.AddRange(buffer);
                        }

                        else if (count < BUFFER_SIZE)
                        {
                            byte[] dataless = new byte[count];
                            Array.Copy(buffer, dataless, count);
                            lists.AddRange(dataless);
                        }
                        if (clientSocket.Available != 0)
                        {
                            goto callBack;
                        }
                        byte[] data = lists.ToArray();
                        lists.Clear();
                        lists = null;

                        if (TCPDataAccroiding != null)
                        {
                            string MobilePhoneIM = Encoding.UTF8.GetString(buffer, 0, count);

                            this.TCPDataAccroiding(MobilePhoneIM, new Action<bool, string, string>((successed, conferenceName, selfUri) =>
                                {
                                    if (successed)
                                    {
                                        clientIncordingEntity = new ConferenceClientAccept() { ConferenceClientAcceptType = ConferenceClientAcceptType.ConferenceAudio, ConferenceName = conferenceName, SelfUri = "_" + selfUri };

                                        if (this.TCPDataArrival != null)
                                        {
                                            this.TCPDataArrival(new TcpDateEvent(clientSocket, clientIncordingEntity), SocketClientType.Android);
                                        }
                                    }
                                    else
                                    {
                                        using (MemoryStream stream = new MemoryStream(data))
                                        {
                                            stream.Position = 0;
                                            BinaryFormatter formatter = new BinaryFormatter();
                                            formatter.Binder = new UBinder();
                                            var result = formatter.Deserialize(stream);
                                            Type type = result.GetType();
                                            if (type.Equals(typeof(ConferenceClientAccept)))
                                            {
                                                clientIncordingEntity = result as ConferenceClientAccept;
                                                if (this.TCPDataArrival != null)
                                                {
                                                    this.TCPDataArrival(new TcpDateEvent(clientSocket, clientIncordingEntity), SocketClientType.PC);
                                                }
                                            }
                                            stream.Flush();
                                        }
                                    }
                                }));
                        }
                        else
                        {
                            using (MemoryStream stream = new MemoryStream(data))
                            {
                                stream.Position = 0;
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Binder = new UBinder();
                                var result = formatter.Deserialize(stream);
                                Type type = result.GetType();
                                if (type.Equals(typeof(ConferenceClientAccept)))
                                {
                                    clientIncordingEntity = result as ConferenceClientAccept;
                                    if (this.TCPDataArrival != null)
                                    {
                                        this.TCPDataArrival(new TcpDateEvent(clientSocket, clientIncordingEntity), SocketClientType.PC);
                                    }
                                }
                                stream.Flush();
                            }
                        }
                        Array.Clear(data, 0, data.Length);
                        data = null;
                    }
                    try
                    {
                        if (clientSocket != null)
                        {
                            //if (this.TCPDataArrival != null)
                            //{
                            //    this.TCPDataArrival(new TcpDateEvent(clientSocket, new PackageBase() { head = EPackageHead.Discon }));
                            //}
                            //服务器端释放已经断开的客户端连接Socket对象资源
                            Thread.Sleep(300);
                            if (clientSocket.Poll(10, SelectMode.SelectRead))
                            {
                                clientSocket.Shutdown(SocketShutdown.Both);
                            }
                            clientSocket.Close();
                            clientSocket = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManage.WriteLog(this.GetType(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                //LogManage.WriteLog(this.GetType(), ex);
            }

        }

        #endregion

        #region 服务端往客户端发送信息

        /// <summary>
        /// 服务端往客户端发送消息
        /// </summary>
        /// <param name="socket">目标Socket</param>
        /// <param name="packageBase">需要发送的包</param>
        public void ServerSendData(Socket socket, PackageBase packageBase, Action<bool> callback)
        {
            try
            {
                if (socket != null
                    && socket.Connected)
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ms.Position = 0;
                        formatter.Serialize(ms, packageBase);//通过序列化的方式将数据发送到指定的客户端
                        byte[] data = ms.ToArray();
                        int count = socket.Send(data, data.Length, SocketFlags.None);
                        //Thread.Sleep(10);
                        ms.Flush();
                    }

                    callback(true);
                }
                else
                {
                    callback(false);
                }
            }
            catch (Exception ex)
            {
                callback(false);
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        /// <summary>
        /// 服务端往客户端发送消息
        /// </summary>
        /// <param name="socket">目标Socket</param>
        /// <param name="packageBase">需要发送的包</param>
        public  void ServerSendData(Socket socket, string json, Action<bool> callback)
        {
            try
            {
                if (socket != null
                    && socket.Connected)
                {                   
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                    int count = socket.Send(data, data.Length, SocketFlags.None);

                    callback(true);
                }
            }
            catch (Exception ex)
            {
                callback(false);
                LogManage.WriteLog(typeof(ServerSocket), ex);
            }
        }      

        #endregion

        #region 关闭服务器侦听

        /// <summary>
        /// 关闭监听
        /// </summary>
        public void CloseListen()
        {
            try
            {
                this._start = false;
                if (this._tcpServerSocket != null)
                {
                    if (this._tcpServerSocket.Connected)
                    {
                        this._tcpServerSocket.Shutdown(SocketShutdown.Both);
                    }
                    this._tcpServerSocket.Close();
                    this._tcpServerSocket = null;
                }
                if (this._connectDataThread != null)
                {
                    if (this._connectDataThread.ThreadState == System.Threading.ThreadState.Running)
                    {
                        this._connectDataThread.Abort();
                        this._connectDataThread = null;
                    }
                }
                if (this._tcpServerListenThread != null)
                {
                    if (this._tcpServerListenThread.ThreadState == System.Threading.ThreadState.Running)
                    {
                        this._tcpServerListenThread.Abort();
                        this._tcpServerListenThread = null;
                    }
                }
            }
            catch (Exception ex)
            {
                LogManage.WriteLog(this.GetType(), ex);
            }
        }

        #endregion

    }

}
