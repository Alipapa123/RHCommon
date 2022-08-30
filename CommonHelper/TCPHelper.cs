using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonHelper
{

    #region 接口
    public interface ISocket
    {
        void InitSocket(IPAddress ip, int port);
        void InitSocket(string ip, int port);
        void InitSocket(int port);
        void Start();
        void Stop();
    }
    #endregion


    #region 枚举定义
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        Normal,
        warning,
        Error,
    }

    #endregion



    #region 事件参数
    public class ReceiveDataEventArgs : EventArgs
    {
        public IPEndPoint IP;
        public byte[] ReceiveData;
        public DateTime Time { get; set; }

        public ReceiveDataEventArgs(IPEndPoint IP, byte[] ReceiveData)
        {
            this.ReceiveData = ReceiveData;
            this.IP = IP;
            Time = DateTime.Now;
        }
    }

    public class ClientOnLineOrOffLineEventArgs : EventArgs
    {
        public IPEndPoint IP { get; set; }
        public DateTime Time { get; set; }
        public ClientOnLineOrOffLineEventArgs(IPEndPoint IP)
        {
            this.IP = IP;
            Time = DateTime.Now;
        }
    }

    public class TcpMessageEventArgs
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType EventType { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 消息发生的时间
        /// </summary>
        public DateTime Time { get; set; }
        public TcpMessageEventArgs(MessageType EventType, string Message)
        {
            this.EventType = EventType;
            this.Message = Message;
            Time = DateTime.Now;
        }
    }

    #endregion

    #region 服务端
    public class CLTcpServer : ISocket
    {
        public event EventHandler<ReceiveDataEventArgs> ReceiveDataEvent;
        public event EventHandler<ClientOnLineOrOffLineEventArgs> ClientOnLineEvent;
        public event EventHandler<ClientOnLineOrOffLineEventArgs> ClientOffLineEvent;
        public event EventHandler<TcpMessageEventArgs> MessageEvent;

        /// <summary>
        /// 标识服务器是否已经停止！
        /// </summary>
        bool _IsStop = true;
        object obj = new object();
        /// <summary>
        /// 信号量
        /// </summary>
        Semaphore semap = new Semaphore(5, 5000);
        /// <summary>
        /// 客户端队列集合
        /// </summary>
        public List<ConnectedClient> ClientList = new List<ConnectedClient>();
        /// <summary>
        /// 服务端监听连接的TcpListener
        /// </summary>
        TcpListener listener;


        private IPEndPoint _IP;

        public IPEndPoint IP
        {
            get { return _IP; }
        }

        /// <summary>
        /// 初始化服务端对象
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">监听端口</param>
        public void InitSocket(IPAddress iP, int port)
        {
            _IP = new IPEndPoint(iP, port);
            listener = new TcpListener(_IP);
        }
        /// <summary>
        /// 初始化服务端对象 监听Any即所有网卡
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">监听端口</param>
        public void InitSocket(int port)
        {
            _IP = new IPEndPoint(IPAddress.Any, port);
            listener = new TcpListener(_IP);
        }
        /// <summary>
        /// 初始化服务端对象
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">监听端口</param>
        public void InitSocket(string ipaddress, int port)
        {
            _IP = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            listener = new TcpListener(_IP);
        }
        /// <summary>
        /// 启动监听,并处理连接
        /// </summary>
        public void Start()
        {
            try
            {
                if (_IsStop == true)
                {
                    listener.Start();
                    _IsStop = false;
                    Task.Run(new Action(() =>
                    {
                        while (true)
                        {
                            if (_IsStop == true)
                            {
                                break;
                            }
                            GetAcceptTcpClient();
                            Thread.Sleep(1);
                        }
                    }));
                }
            }
            catch (SocketException e)
            {
                listener.Stop();
                _IsStop = true;
                throw e;
            }
        }
        /// <summary>
        /// 等待处理新的连接
        /// </summary>
        private void GetAcceptTcpClient()
        {
            try
            {
                semap.WaitOne();
                //获取连接的客户端；
                TcpClient tclient = listener.AcceptTcpClient();//此语句会死锁当前线程，直到有客户端连接；
                         
                

                ConnectedClient sks = new ConnectedClient() { Client = tclient };//保存连接的客户端；
                //加入客户端集合.
                AddClientList(sks);//将连接的客户端添加到客户端列表；



                ClientOnLineOrOffLineEventArgs e = new ClientOnLineOrOffLineEventArgs(sks.Ip); //通知客户端上线；
                if (ClientOnLineEvent != null)
                {
                    ClientOnLineEvent.Invoke(this, e);
                }



                //上线的客户端异步接收数据；
                tclient.Client.BeginReceive(sks.RecBuffer, 0, sks.RecBuffer.Length, 0, new AsyncCallback(EndReader), sks);// 
                semap.Release();
            }
            catch (Exception exs)
            {
                semap.Release();



                TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Error, exs.ToString());
                if (MessageEvent != null)
                {
                    MessageEvent.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// 异步接收信息.数据接收完成后的回调函数；
        /// </summary>
        /// <param name="ir"></param>
        private void EndReader(IAsyncResult ir)
        {
            ConnectedClient sks = ir.AsyncState as ConnectedClient;
            if (sks != null && listener != null)
            {
                try
                {
                    int ReceiveDataLengh = sks.Client.GetStream().EndRead(ir);//endreader 停止异步接收并返回接收到的字节数；
                    if (ReceiveDataLengh != 0)//长度不为零，说明不是下线通知，而是正常的数据
                    {
                        byte[] DataTemp = new byte[ReceiveDataLengh];
                        Array.Copy(sks.RecBuffer, DataTemp, ReceiveDataLengh);
                        ReceiveDataEventArgs e = new ReceiveDataEventArgs(sks.Ip, DataTemp);
                        if (ReceiveDataEvent != null)
                        {
                            ReceiveDataEvent.Invoke(this, e);
                        }
                        //继续下一次的接收；
                        sks.Client.Client.BeginReceive(sks.RecBuffer, 0, sks.RecBuffer.Length, 0, new AsyncCallback(EndReader), sks);
                    }
                    else//客户端下线，接收的数据长度为零，为下线通知；
                    {
                        ClientList.Remove(sks);
                        ClientOnLineOrOffLineEventArgs e = new ClientOnLineOrOffLineEventArgs(sks.Ip);
                        if (ClientOffLineEvent != null)
                        {
                            ClientOffLineEvent.Invoke(this, e);
                        }
                    }
                }
                catch (Exception skex)
                {
                    lock (obj)
                    {
                        TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Error, skex.ToString());
                        if (MessageEvent != null)
                        {
                            MessageEvent.Invoke(this, e);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 加入队列.
        /// </summary>
        /// <param name="sk"></param>
        private void AddClientList(ConnectedClient sk)
        {
            //判断现有的客户端列表中是否已经包含要添加的对象；
            ConnectedClient sockets = ClientList.Find(o => { return o.Ip == sk.Ip; });
            //如果不存在则添加,否则更新
            if (sockets == null)
            {
                ClientList.Add(sk);
            }
            else
            {
                ClientList.Remove(sockets);
                ClientList.Add(sk);
            }
        }
        public void Stop()
        {
            if (listener != null)
            {
                SendToAll("");
                //关闭列表中的所有客户端连接；
                for (int i = 0; i < ClientList.Count; i++)
                {
                    ClientList[i].Client.Close();
                }
                ClientList.Clear();
                listener.Stop();
                listener = null;
                _IsStop = true;
            }
        }
        /// <summary>
        /// 向所有在线的客户端发送信息.
        /// </summary>
        /// <param name="SendData">发送的文本</param>
        public void SendToAll(string SendData)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                SendToClient(ClientList[i].Ip, SendData);
            }
        }
        /// <summary>
        /// 向所有在线的客户端发送信息.
        /// </summary>
        /// <param name="SendData">发送的数据</param>
        public void SendToAll(byte[] SendDataBuffer)
        {
            for (int i = 0; i < ClientList.Count; i++)
            {
                SendToClient(ClientList[i].Ip, SendDataBuffer);
            }
        }
        /// <summary>
        /// 向某一位客户端发送信息
        /// </summary>
        /// <param name="ip">客户端IP+端口地址</param>
        /// <param name="SendData">发送的数据包</param>
        public void SendToClient(IPEndPoint ip, byte[] SendDataBuffer)
        {
            try
            {
                //判断客户端列表中是否存在要发送消息的客户端；
                ConnectedClient sks = ClientList.Find(o => { return (o.Ip.Address.ToString() == ip.Address.ToString()) && (o.Ip.Port == ip.Port); });
                if (sks != null)
                {
                    if (sks.Client.Connected)
                    {
                        sks.SendBuffer = SendDataBuffer;
                        sks.Client.Client.Send(sks.SendBuffer, sks.SendBuffer.Length, 0);
                    }
                }
            }
            catch (Exception skex)
            {
                TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Error, skex.ToString());
                if (MessageEvent != null)
                {
                    MessageEvent.Invoke(this, e);
                }
            }
        }

        /// <summary>
        /// 向某一位客户端发送信息
        /// </summary>
        /// <param name="ip">客户端IP+端口地址</param>
        /// <param name="SendData">发送的数据包</param>
        public void SendToClient(IPEndPoint ip, string SendData)
        {
            try
            {
                ConnectedClient sks = ClientList.Find(o => { return (o.Ip.Address.ToString() == ip.Address.ToString()) && (o.Ip.Port == ip.Port); });
                if (sks != null)
                {
                    if (sks.Client.Connected)
                    {
                        sks.SendBuffer = Encoding.GetEncoding("utf-8").GetBytes(SendData);
                        sks.Client.Client.Send(sks.SendBuffer, sks.SendBuffer.Length, 0);
                    }
                }
            }
            catch (Exception skex)
            {
                TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Error, skex.ToString());
                if (MessageEvent != null)
                {
                    MessageEvent.Invoke(this, e);
                }
            }
        }
    }

    /// <summary>
    /// 表示服务器端连接的客户端
    /// </summary>
    public class ConnectedClient : IEquatable<ConnectedClient>
    {
        /// <summary>
        /// 接收缓冲区
        /// </summary>
        public byte[] RecBuffer = new byte[4096000];
        /// <summary>
        /// 发送缓冲区
        /// </summary>
        public byte[] SendBuffer = new byte[4096000];

        public IPEndPoint Ip;

        private TcpClient _Client;

        public TcpClient Client
        {
            get { return _Client; }
            set
            {
                _Client = value;
                Ip = value.Client.RemoteEndPoint as IPEndPoint;
            }
        }

        public bool Equals(ConnectedClient other)
        {
            if ((this.Ip.Address.ToString() == other.Ip.Address.ToString()) && (this.Ip.Port == other.Ip.Port))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion



    #region 客户端

    public class CLTcpClient : ISocket
    {
        public event EventHandler<ReceiveDataEventArgs> ReceiveDataEvent;
        public event EventHandler<TcpMessageEventArgs> MessageEvent;

        private byte[] _ReceiveBuffer = new byte[4096000];
        public bool IsOpen { get; set; }
        private TcpClient client;
        IPEndPoint ip;

        public void InitSocket(string ipaddress, int port)
        {
            ip = new IPEndPoint(IPAddress.Parse(ipaddress), port);
            client = new TcpClient();
        }
        public void InitSocket(int port)
        {
            ip = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            client = new TcpClient();
        }

        public void InitSocket(IPAddress IP, int port)
        {
            ip = new IPEndPoint(IP, port);
            client = new TcpClient();
        }

        public void SendData(string SendData)
        {
            byte[] buffer = Encoding.GetEncoding("utf-8").GetBytes(SendData);
            this.SendData(buffer);
        }
        public void SendData(byte[] SendData)
        {
            byte[] buffer = SendData;
            client.Client.Send(buffer, buffer.Length, 0);
        }
        private void Connect()
        {
            client.Connect(ip);
            IsOpen = client.Connected;
            client.Client.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, 0, new AsyncCallback(EndReader), client);
            //推送连接成功.
            TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Normal, "客户端连接成功.");
            if (MessageEvent != null)
            {
                MessageEvent.Invoke(this, e);
            }
        }
        private void EndReader(IAsyncResult ir)
        {
            TcpClient Client = ir.AsyncState as TcpClient;
            try
            {
                if (IsOpen == true)
                {
                    int ReceiveDataLengh = Client.GetStream().EndRead(ir);
                    if (ReceiveDataLengh != 0)
                    {
                        byte[] DataTemp = new byte[ReceiveDataLengh];
                        Array.Copy(_ReceiveBuffer, DataTemp, ReceiveDataLengh);
                        ReceiveDataEventArgs ex = new ReceiveDataEventArgs(ip, DataTemp);
                        if (ReceiveDataEvent != null)
                        {
                            ReceiveDataEvent.Invoke(this, ex);
                        }
                        Client.Client.BeginReceive(_ReceiveBuffer, 0, _ReceiveBuffer.Length, 0, new AsyncCallback(EndReader), Client);
                    }
                    else//客户端下线
                    {
                        TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Normal, "服务端已关闭，因此客户端将关闭与服务端的连接");
                        if (MessageEvent != null)
                        {
                            MessageEvent.Invoke(this, e);
                        }
                        Stop();
                    }
                }
            }
            catch (Exception skex)
            {
                TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Error, skex.Message);
                if (MessageEvent != null)
                {
                    MessageEvent.Invoke(this, e);
                }
            }
        }

        public void Start()
        {
            Connect();
        }
        public void Stop()
        {
            if (!client.Connected)
            {
                IsOpen = client.Connected;
                TcpMessageEventArgs e = new TcpMessageEventArgs(MessageType.Normal, "客户端已经处于离线状态，无需断开连接");
                if (MessageEvent != null)
                {
                    MessageEvent.Invoke(this, e);
                }
                return;
            }
            client.Client.Shutdown(SocketShutdown.Both);
            Thread.Sleep(10);
            IsOpen = client.Connected;
            client.Close();
            TcpMessageEventArgs ex = new TcpMessageEventArgs(MessageType.Normal, "客户端已断开连接");
            if (MessageEvent != null)
            {
                MessageEvent.Invoke(this, ex);
            }
        }
    }
    #endregion
}