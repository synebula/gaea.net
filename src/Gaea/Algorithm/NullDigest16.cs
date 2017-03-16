namespace Gaea.Algorithm
{
    /// <summary>
    /// CRC16计算类。
    /// </summary>
    public class NullDigest16 : IDigest<ushort>
    {
        /// <summary>
        /// 计算一个byte数组的CRC16值。
        /// </summary>
        /// <param name="data">需要计算CRC16的byte数组。</param>
        /// <returns>返回CRC16值</returns>
        public ushort Calculate(byte[] bytes)
        {
            return 0;
        }

        /// <summary>
        /// 计算一个byte数组的CRC16值。
        /// </summary>
        /// <param name="bytes">需要计算CRC16的byte数组。</param>
        /// <param name="offset">开始位置。</param>
        /// <param name="length">长度。</param>
        /// <returns>返回CRC16值。</returns>
        public ushort Calculate(byte[] bytes, int offset, int length)
        {
            return 0;
        }
    }
}
