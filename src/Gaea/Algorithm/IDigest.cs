namespace Gaea.Algorithm
{
    /// <summary>
    /// 校验值计算接口。
    /// </summary>
    public interface IDigest<T>
    {
        /// <summary>
        /// 计算校验值。
        /// </summary>
        /// <param name="bytes">需要计算校验值的byte数组。</param>
        /// <returns>返回校验值。</returns>
        T Calculate(byte[] bytes);

        /// <summary>
        /// 计算校验值。
        /// </summary>
        /// <param name="bytes">需要计算校验值的byte数组。</param>
        /// <param name="offset">偏移位置。</param>
        /// <param name="length">长度。</param>
        /// <returns>返回校验值。</returns>
        T Calculate(byte[] bytes, int offset, int length);
    }
}
