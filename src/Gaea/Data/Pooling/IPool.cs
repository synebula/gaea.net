using System;

namespace Gaea.Data.Pooling
{
    /// <summary>
    /// 池接口。
    /// </summary>
    public interface IPool : IDisposable
    {
        /// <summary>
        /// 获取池的容量。
        /// </summary>
        int Capacity { get; }

        /// <summary>
        /// 获取有效元素数量。
        /// </summary>
        int Available { get; }
    }

    /// <summary>
    /// 泛型池接口。
    /// </summary>
    /// <typeparam name="TElement">池中的元素。</typeparam>
    public interface IPool<TElement> : IPool
        where TElement : IDisposable
    {
        /// <summary>
        /// 获取池中元素。
        /// </summary>
        /// <returns></returns>
        TElement Pop();

        /// <summary>
        /// 回收一个池元素。
        /// </summary>
        /// <param name="item">池中的元素。</param>
        void Recycle(TElement item);
    }
}
