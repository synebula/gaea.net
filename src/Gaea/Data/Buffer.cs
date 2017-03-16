using System;

namespace Gaea.Data
{
    /// <summary>
    /// 缓冲区。
    /// </summary>
    public class Buffer<T> : IBuffer<T>
    {

        #region Property

        private T[] data;
        /// <summary>
        /// 获取实际数据。
        /// </summary>
        public T[] Data
        {
            get { return data; }
        }

        private int capacity;
        /// <summary>
        /// 获取缓冲区容量。
        /// </summary>
        public int Capacity
        {
            get { return capacity; }
        }

        private int length;
        /// <summary>
        /// 获取或设置实际数据长度。
        /// </summary>
        public int Length
        {
            get { return length; }
            set
            {
                length = value;
                if (length > 0)
                {
                    if (length > capacity)
                        throw new ArgumentOutOfRangeException("数据长度超过缓冲区容量范围！");
                    this.isEmpty = false;
                }
                else
                    this.isEmpty = true;
            }
        }

        private int offset;
        /// <summary>
        /// 获取或设置实际数据偏移地址。
        /// </summary>
        public int Offset
        {
            get { return offset; }
            set
            {
                offset = value;
                if (offset + length > capacity)
                    throw new ArgumentOutOfRangeException("当前偏移量下该数据长度超过缓冲区容量范围！");
                if (offset == capacity)//偏移量等于总长度说明已经为空
                    this.isEmpty = true;
            }
        }

        private bool isEmpty;
        /// <summary>
        /// 获取当前缓冲区是否为空。
        /// </summary>
        public bool IsEmpty
        {
            get { return isEmpty; }
        }

        /// <summary>
        /// 索引器。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                if (index >= offset + length)
                    throw new IndexOutOfRangeException("索引超出数据界限！");
                return data[index];
            }
        }

        #endregion

        #region Method

        /// <summary>
        /// 构造方法。
        /// </summary>
        /// <param name="capacity">缓冲区容量。</param>
        public Buffer(int capacity)
        {
            this.capacity = capacity;
            this.data = new T[capacity];
        }


        /// <summary>
        /// 释放Buffer。
        /// </summary>
        public void Dispose()
        {
            this.length = this.offset = 0;
        }

        /// <summary>
        /// 返回Buffer的备份。
        /// </summary>
        /// <returns></returns>
        public void CopyTo(Buffer<T> buffer)
        {
            buffer.offset = this.offset;
            buffer.length = this.length;
            Array.Copy(this.data, this.offset, buffer.data, this.offset, this.length);
        }

        #endregion
    }
}
