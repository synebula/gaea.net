using System;
using System.Collections.Generic;
using System.IO;

namespace Gaea.Data.Streaming
{
    /// <summary>
    /// 定义一个类似队列的内存流类型。
    /// 读取出的数据进行出队操作，写入的进行入队操作追加到尾部。
    /// </summary>
    public class QueueStream : Stream
    {
        /// <summary>
        /// 实际数据存储池。
        /// 使用链表可以避免长数据的内存分配性能问题。
        /// </summary>
        private LinkedList<byte[]> data = new LinkedList<byte[]>();

        /// <summary>
        /// 实例的读写锁。
        /// </summary>
        private object _lock = new object();

        public override bool CanRead { get { return true; } }

        public override bool CanSeek { get { return false; } }

        public override bool CanWrite { get { return true; } }

        long length;
        public override long Length { get { return length; } }

        long position;
        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = (int)value;
            }
        }


        /// <summary>
        /// 清空数据存储池。
        /// </summary>
        public override void Flush()
        {
            this.data.Clear();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (this._lock)
            {
                if (count > length)
                    return 0;

                int dataOffset = (int)this.position;//记下数据的偏移索引
                int destOffset = 0;//目标数据需要写入的偏移索引
                int wantLength = count;//记录下需要的长度
                int realLength;//数据段除去偏移量的长度
                byte[] segment;//数据段
                while (destOffset < count)
                {
                    segment = this.data.First.Value;
                    realLength = segment.Length - dataOffset;
                    //取出数据有两种策略：
                    //需要取出的数据长度大于数据段长度，按数据段长度取
                    //需要取出的数据长度小于数据段长度，按需要取的长度取
                    if (wantLength > realLength)
                    {
                        Buffer.BlockCopy(segment, dataOffset, buffer, destOffset, realLength);
                        this.length -= realLength;
                        dataOffset = 0;//完整取出后偏移量重置

                        destOffset += realLength;
                        wantLength -= realLength;

                        this.data.RemoveFirst();
                    }
                    else
                    {
                        Buffer.BlockCopy(segment, dataOffset, buffer, destOffset, wantLength);
                        this.length -= wantLength;
                        dataOffset += wantLength;//数据取完，记录下offset

                        destOffset += wantLength;

                        if (dataOffset == segment.Length)//offset等于长度说明本条已经取完了
                        {
                            dataOffset = 0;
                            this.data.RemoveFirst();
                        }
                    }
                }
                this.position = dataOffset;//记住偏移量
                return count;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new Exception("seek stream is not allow!");
        }

        public override void SetLength(long value)
        {
            throw new Exception("set stream length is not allow!");
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] segment = new byte[count];
            Buffer.BlockCopy(buffer, offset, segment, 0, count);
            this.data.AddLast(segment);
            this.length += count;
        }
    }
}