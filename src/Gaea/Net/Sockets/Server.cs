using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Gaea.Net.Sockets
{
    /// <summary>
    /// 服务器。
    /// </summary>
    /// <typeparam name="TElement">服务器接收的元素。</typeparam>
    public class Server : IStartable, IDisposable
    {
        #region Variable

        /// <summary>
        /// 服务端套接字。
        /// </summary>
        Socket socket;

        /// <summary>
        /// 正在连接的RemotedSocket列表。
        /// </summary>
        LinkedList<Socket> connectionList;

        /// <summary>
        /// 最大排队数。
        /// </summary>
        int backlog;

        #endregion


        #region Method


        /// <summary>
        /// 接收到客户端连接。
        /// </summary>
        public event EventHandler<Socket> OnConnected;

        #region Ctor

        /// <summary>
        /// 构造服务器。
        /// <para>调用该构造方法，请调用<see cref="Listen(IPAddress, int, int)"/>方法启动监听。</para>
        /// </summary>
        public Server(IPAddress address, int port) : this(address, port, 1 << 8)
        {
        }

        /// <summary>
        /// 构造服务器。
        /// <para>调用该构造方法，请调用<see cref="Listen(IPAddress, int, int)"/>方法启动监听。</para>
        /// </summary>
        /// <param name="backlog">最大等待数量。</param>
        /// <param name="bufferCapacity">缓冲区长度。</param>
        public Server(IPAddress address, int port, int backlog)
        {
            this.backlog = backlog;
            this.connectionList = new LinkedList<Socket>();
            this.Listen(address, port);
        }

        #endregion

        /// <summary>
        /// 在制定的IP地址和端口监听。
        /// </summary>
        /// <param name="address">IP地址。</param>
        /// <param name="port">端口。</param>
        /// <param name="backlog">最大链接数量。</param>
        private void Listen(IPAddress address, int port)
        {
            try
            {
                socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(address, port));
                socket.Listen(backlog);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 服务器启动。
        /// </summary>
        public void Start()
        {
            if (socket == null)
                throw new InvalidOperationException("当前服务器未在任何端口监听，无法启动！");
            StartAccept(null);
        }

        /// <summary>
        /// 开始监听准备接收连接。
        /// </summary>
        /// <param name="e"></param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += AcceptCompleted;
            }
            else
                e.AcceptSocket = null;
            if (!socket.AcceptAsync(e))
                ProcessAccept(e);
        }

        /// <summary>
        /// 接收IO操作完成。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        /// 处理接收方法。
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            this.connectionList.AddLast(e.AcceptSocket);//加入连接列表
            if (OnConnected != null)
                OnConnected(this, e.AcceptSocket);
            StartAccept(e);
        }

        #region Private

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in connectionList)
                    {
                        item.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

        #endregion
    }
}
