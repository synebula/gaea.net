using System;

namespace Gaea.Data
{
    /// <summary>
    /// 缓冲区。
    /// </summary>
    public interface IBuffer<T> : IDisposable
    {

        /// <summary>
        /// 获取实际数据。
        /// </summary>
        T[] Data
        {
            get;
        }

        /// <summary>
        /// 获取缓冲区容量。
        /// </summary>
        int Capacity
        {
            get;
        }

        /// <summary>
        /// 获取或设置实际数据长度。
        /// </summary>
        int Length
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置实际数据偏移地址。
        /// </summary>
        int Offset
        {
            get;
            set;
        }

        /// <summary>
        /// 获取当前缓冲区是否为空。
        /// </summary>
        bool IsEmpty
        {
            get;
        }

        /// <summary>
        /// 索引器。
        /// </summary>
        /// <param name="index">索引。</param>
        /// <returns></returns>
        T this[int index]
        {
            get;
        }
    }
}
