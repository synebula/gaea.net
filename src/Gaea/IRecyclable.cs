namespace Gaea
{
    /// <summary>
    /// 区别于<see cref="IDisposable"/>接口，继承本接口表明实现类可以释放后重复使用。
    /// </summary>
    public interface IRecyclable
    {
        /// <summary>
        /// 回收实例。
        /// </summary>
        void Recycle();
    }
}
