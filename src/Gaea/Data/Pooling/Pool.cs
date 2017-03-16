using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Gaea.Data.Pooling;

namespace Gaea.Data.Pooling
{
    /// <summary>
    /// 池。
    /// <para>Auth:reize</para>
    /// <para>Date:2016/5/4 16:29:12</para>
    /// <para>Memo:可以重复利用对象。</para>
    /// </summary>
    public abstract class Pool<TElement> : IPool<TElement>
         where TElement : IDisposable
    {
        #region Variable

        /// <summary>
        /// 空闲元素栈。
        /// </summary>
        protected ConcurrentStack<TElement> idle;

        /// <summary>
        /// 下限, 这里存储的是可用数和容量的比例。
        /// </summary>
        float floor;

        /// <summary>
        /// 最大数量。
        /// </summary>
        int maxCount;

        #endregion

        #region Property

        /// <summary>
        /// 获取有效元素数量。
        /// </summary>
        public int Available
        {
            get
            {
                return this.idle.Count;
            }
        }

        protected int capacity;
        /// <summary>
        /// 获取池的容量。
        /// </summary>
        public int Capacity
        {
            get
            {
                return this.capacity;
            }
        }

        #endregion

        #region Event

        #endregion

        #region Method

        #region Ctor
        /// <summary>
        /// 构造方法。
        /// </summary>
        public Pool() : this(16)
        {

        }


        /// <summary>
        /// 构造方法。
        /// </summary>
        public Pool(int initCount) : this(initCount, -1)
        {

        }

        /// <summary>
        /// 构造方法。默认不做初始化，如若构造时初始化请调用<see cref="Init"/>方法。
        /// </summary>
        public Pool(int initCount, int maxCount)
        {
            idle = new ConcurrentStack<TElement>();
            this.capacity = initCount;
            this.maxCount = maxCount;
            this.floor = initCount == 1 ? 1 : 2f / capacity;//最少数量下限
        }

        #endregion

        /// <summary>
        /// 初始化数据。
        /// </summary>
        public void Init()
        {
            //初始化
            TElement element;
            for (int i = 0; i < this.capacity; i++)
            {
                element = NewElement();
                idle.Push(element);
            }
        }

        /// <summary>
        /// 释放一个池元素。
        /// </summary>
        /// <param name="item"></param>
        public void Recycle(TElement item)
        {
            if (this.Available == this.capacity)
                Interlocked.Increment(ref this.capacity);
            this.idle.Push(item);
        }

        /// <summary>
        /// 获取池中元素。
        /// </summary>
        /// <returns></returns>
        public TElement Pop()
        {
            TElement item;
            item = default(TElement);
            if (idle.TryPop(out item))
            {
                if (((float)this.Available / this.capacity) <= floor)//小于下限则扩容
                {
                    this.ExpandScale();
                }
            }
            return item;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            this.capacity = 0;
            TElement element;
            while (this.idle.Count > 0)
            {
                this.idle.TryPop(out element);
                element.Dispose();
            }
        }

        #region Private

        /// <summary>
        /// 扩大容量。
        /// </summary>
        protected virtual void ExpandScale()
        {
            int scale = 0;
            if (this.maxCount == -1)
                scale = this.capacity;//容量扩大一倍
            else
            {
                if (this.maxCount < this.capacity << 1)//扩大到最大容量
                    scale = this.maxCount - this.capacity;
                else
                    scale = this.capacity;
            }

            TElement element;
            for (int i = 0; i < scale; i++)
            {
                element = this.NewElement();
                idle.Push(element);
                Interlocked.Increment(ref capacity);
            }
        }

        /// <summary>
        /// 收缩容量。
        /// </summary>
        protected virtual void ShrinkScale()
        {
            int shrink = this.capacity / 3;//收缩1/3
            TElement element;
            for (int i = 0; i < shrink; i++)
            {
                this.idle.TryPop(out element);
                element.Dispose();
            }
        }

        /// <summary>
        /// 创建一个新实例。
        /// </summary>
        /// <returns></returns>
        protected abstract TElement NewElement();

        #endregion

        #endregion
    }

    /// <summary>
    /// 默认的智能池。
    /// </summary>
    /// <typeparam name="TElement"></typeparam>
    public class IntelliPool<TElement> : Pool<TElement>
        where TElement : IDisposable, new()
    {
        /// <summary>
        /// 构造方法。
        /// </summary>
        public IntelliPool() : this(16)
        {

        }


        /// <summary>
        /// 构造方法。
        /// </summary>
        public IntelliPool(int initCount) : this(initCount, -1)
        {

        }

        /// <summary>
        /// 构造方法。
        /// </summary>
        public IntelliPool(int initCount, int maxCount) : base(initCount, maxCount)
        {
            this.Init();
        }

        /// <summary>
        /// 创建新的实例。
        /// </summary>
        /// <returns></returns>
        protected override TElement NewElement()
        {
            return new TElement();
        }
    }
}
