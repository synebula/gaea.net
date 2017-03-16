using Gaea.Data;
using Gaea.Data.Pooling;

namespace Gaea.Net.Sockets
{
    /// <summary>
    /// 缓冲区管理器。
    /// <para>Auth:reize</para>
    /// <para>Date:2016/5/12 9:31:05</para>
    /// <para>Memo:</para>
    /// </summary>
    public class BufferManager : Pool<Buffer<byte>>
    {
        #region Variable

        /// <summary>
        /// 缓冲区容量。
        /// </summary>
        int bufferCapacity;

        #endregion

        #region Method

        /// <summary>
        /// 构造方法。
        /// </summary>
        public BufferManager(int bufferCapacity) : base()
        {
            this.bufferCapacity = bufferCapacity;
            this.Init();
        }

        #region Private

        /// <summary>
        /// 创建新的buffer。
        /// </summary>
        /// <returns></returns>
        protected override Buffer<byte> NewElement()
        {
            return new Buffer<byte>(this.bufferCapacity);
        }

        #endregion

        #endregion
    }
}
