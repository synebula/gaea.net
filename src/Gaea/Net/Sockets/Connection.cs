using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Gaea.Algorithm;
using Gaea.Data;
using Gaea.Data.Streaming;
using Gaea.Net.Protocol;

namespace Gaea.Net.Sockets
{
    public class Connection : IDisposable
    {

        #region Variable

        #region Socket
        /// <summary>
        /// 远程Socket。
        /// </summary>
        Socket remote;

        /// <summary>
        /// 接收参数。
        /// </summary>
        SocketAsyncEventArgs receiveArgs;

        /// <summary>
        /// 发送参数。
        /// </summary>
        SocketAsyncEventArgs sendArgs;
        #endregion

        #region Send&Recv
        /// <summary>
        /// 协议解析信号量，仅能一个线程解析。
        /// </summary>
        Semaphore resolveSemaphore = new Semaphore(1, 1);

        /// <summary>
        /// 缓冲区管理器。
        /// </summary>
        BufferManager bufferManager;

        /// <summary>
        /// 接收队列。
        /// </summary>
        ConcurrentQueue<Buffer<byte>> receiveQueue;

        /// <summary>
        /// 发送队列。
        /// </summary>
        ConcurrentQueue<byte[]> sendQueue;

        #endregion

        #region Trans

        /// <summary>
        /// 待解析数据队列。
        /// </summary>
        QueueStream stream;

        /// <summary>
        /// 数字摘要计算。
        /// </summary>
        IDigest<ushort> digestor;

        /// <summary>
        /// 接收数据帧。
        /// </summary>
        TransferFrame receiveFrame;

        /// <summary>
        /// 发送数据帧。
        /// </summary>
        TransferFrame sendFrame;

        /// <summary>
        /// 发送标志，1正在发送，0未发送。
        /// </summary>
        int sending = 0;
        #endregion

        #endregion

        #region Event
        /// <summary>
        /// 接收到数据时触发。
        /// </summary>
        public event Action<object, byte[]> OnReceived;

        /// <summary>
        /// 发送完数据时触发。
        /// </summary>
        public event Action<object, byte[]> OnSended;
        #endregion

        public Connection(Socket remote) : this(remote, 1 << 14)
        {
        }

        public Connection(Socket remote, int bufferCapacity) : this(remote, 1 << 14, new NullDigest16())
        {
        }

        public Connection(Socket remote, int bufferCapacity, IDigest<ushort> digestor)
        {
            this.remote = remote;
            this.digestor = digestor;
            this.Init(bufferCapacity);
        }

        /// <summary>
        /// 初始化参数信息。
        /// </summary>
        private void Init(int bufferCapacity)
        {
            //初始化接收发送参数
            this.receiveArgs = new SocketAsyncEventArgs();
            this.receiveArgs.Completed += this.ProcessReceive;
            this.sendArgs = new SocketAsyncEventArgs();

            //初始化接收发送队列
            this.receiveQueue = new ConcurrentQueue<Buffer<byte>>();
            this.sendQueue = new ConcurrentQueue<byte[]>();

            //Buffer管理器
            this.bufferManager = new BufferManager(bufferCapacity);

            //解析数据的队列
            this.stream = new QueueStream();
            this.receiveFrame = new TransferFrame(digestor);
            this.sendFrame = new TransferFrame(digestor);
        }

        /// <summary>
        /// 异步接收方法。
        /// </summary>
        public void ReceiveAsync()
        {
            var receiveBuffer = this.bufferManager.Pop();
            while (receiveBuffer == null)
            {
                receiveBuffer = this.bufferManager.Pop();
            }
            this.receiveArgs.UserToken = receiveBuffer;
            this.receiveArgs.SetBuffer(receiveBuffer.Data, receiveBuffer.Offset, receiveBuffer.Capacity);
            var willRaiseEvent = this.remote.ReceiveAsync(this.receiveArgs);
            if (!willRaiseEvent)
                ProcessReceive(this.remote, this.receiveArgs);
        }

        /// <summary>
        /// 处理异步接收数据。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                var buffer = e.UserToken as Buffer<byte>;
                buffer.Offset = e.Offset;
                buffer.Length = e.BytesTransferred;
                this.receiveQueue.Enqueue(buffer);

                this.ResolveAsync();//异步解析接受数据
                this.ReceiveAsync();//继续下次接收
            }
            else
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 异步解析接收数据。
        /// </summary>
        protected async void ResolveAsync()
        {
            await Task.Run(() =>
            {
                resolveSemaphore.WaitOne();
                Buffer<byte> buffer;
                while (this.receiveQueue.TryDequeue(out buffer))
                {
                    this.stream.Write(buffer.Data, buffer.Offset, buffer.Length);
                    this.bufferManager.Recycle(buffer);//回收buffer
                }
                byte[] bytes;//协议数据,每次接收都会重新实例化。
                //开始按照协议解析g
                while (this.stream.Length > 0)
                {
                    bytes = new byte[4];
                    if (this.receiveFrame.Length == 0)//协议头未解析
                    {
                        if (this.stream.Read(bytes, 0, TransferFrame.HeadLength) > 0)
                        {
                            this.receiveFrame.LoadHead(bytes);
                        }
                        else
                            break;//数据不足以解析头部跳出等待下次接收
                    }
                    else//已经解析了头部还未接收数据
                    {
                        bytes = new byte[this.receiveFrame.Length];
                        if (this.stream.Read(bytes, 0, bytes.Length) > 0)//尝试获取内容数据
                        {
                            this.receiveFrame.Data = bytes;
                            if (this.receiveFrame.Validate())
                            {
                                if (this.OnReceived != null)//同步通知客户端接收数据
                                    this.OnReceived(this, this.receiveFrame.Data);
                            }
                            this.receiveFrame.Clear();
                        }
                        else
                            break;//数据不足以解析头部跳出等待下次接收
                    }
                }

                resolveSemaphore.Release();
            });
        }

        /// <summary>
        /// 异步发送数据。
        /// </summary>
        public void SendAsync(byte[] buffer)
        {
            //封装数据帧并压入发送栈。
            this.sendFrame.Data = buffer;
            this.sendFrame.CalcDigest();
            this.sendQueue.Enqueue(this.sendFrame.ToBytes());
            this.sendFrame.Clear();
            this.Send();
        }

        /// <summary>
        /// 调用socket实际发送。
        /// </summary>
        protected void Send()
        {
            if (!IsSending())
            {
                Interlocked.CompareExchange(ref this.sending, 1, 0);
                byte[] buffer;
                while (this.sendQueue.TryDequeue(out buffer))
                {
                    this.sendArgs.SetBuffer(buffer, 0, buffer.Length);
                    if (this.remote.SendAsync(this.sendArgs))
                        this.ProcessSend(this, this.sendArgs);
                }
                Interlocked.CompareExchange(ref this.sending, 0, 1);//设置未发送
            }
        }

        /// <summary>
        /// 处理发送完毕事件。
        /// </summary>
        protected void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                if (this.OnSended != null)
                    this.OnSended(this, e.Buffer);
            }
            else
            {
                this.Dispose();
            }
        }

        /// <summary>
        /// 获取是否正在发送。
        /// </summary>
        protected bool IsSending()
        {
            return this.sending == 1;
        }

        /// <summary>
        /// 释放本对象。
        /// </summary>
        public void Dispose()
        {
            try
            {
                this.remote.Shutdown(SocketShutdown.Both);
            }
            catch { }

            //清除参数
            this.receiveArgs.Completed -= this.ProcessReceive;
            this.receiveArgs = null;
            this.sendArgs.Completed -= this.ProcessSend;
            this.sendArgs = null;

            //清除事件
            this.OnReceived = null;
            this.OnSended = null;

            //清除socket
            this.remote = null;
        }
    }
}