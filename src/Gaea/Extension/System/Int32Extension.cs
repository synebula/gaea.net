namespace Gaea.Extension.System
{
    public static class Int32Extension
    {
        /// <summary>
        /// 转化为byte数组。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this int obj)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)(obj >> 24 & 0xff);
            bytes[1] = (byte)(obj >> 16 & 0xff);
            bytes[2] = (byte)(obj >> 8 & 0xff);
            bytes[3] = (byte)(obj & 0xff);
            return bytes;
        }

        /// <summary>
        /// 转化为byte数组。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ToBytesBuffer(this int obj, byte[] buffer, int offset)
        {
            buffer[offset + 0] = (byte)(obj >> 24 & 0xff);
            buffer[offset + 1] = (byte)(obj >> 16 & 0xff);
            buffer[offset + 2] = (byte)(obj >> 8 & 0xff);
            buffer[offset + 3] = (byte)(obj & 0xff);
        }
    }
}
