namespace Gaea.Extension.System
{
    public static class UInt16Extension
    {
        /// <summary>
        /// 转化为byte数组。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this ushort obj)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)(obj >> 8 & 0xff);
            bytes[1] = (byte)(obj & 0xff);
            return bytes;
        }

        /// <summary>
        /// 转化为byte数组。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ToBytesBuffer(this ushort obj, byte[] buffer, int offset)
        {
            buffer[offset + 0] = (byte)(obj >> 8 & 0xff);
            buffer[offset + 1] = (byte)(obj & 0xff);
        }
    }
}
