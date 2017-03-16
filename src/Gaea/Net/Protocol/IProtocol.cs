namespace Gaea.Net.Protocol
{
    /// <summary>
    /// 该接口规定了协议必须实现的方法。
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 数据部分长度。
        /// </summary>
        /// <returns></returns>
        ushort Length { get; }

        /// <summary>
        /// 协议数据部分。
        /// </summary>
        /// <returns></returns>
        byte[] Data { get; set; }
        
        /// <summary>
        /// 序列化成二进制。
        /// </summary>
        byte[] ToBytes();
    }
}