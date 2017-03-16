using System;
using System.Linq;

namespace Gaea.Extension.System
{
    public static class ArrayExtension
    {
        /// <summary>
        /// 转化byte数组为<see cref="System.UInt16"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(this byte[] obj)
        {
            return ToUInt16(obj, 0);
        }

        /// <summary>
        /// 转化byte数组为<see cref="System.UInt16"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UInt16 ToUInt16(this byte[] obj, int offset)
        {
            if (obj == null)
                throw new NullReferenceException();
            var count = obj.Count();
            int high = 0;
            int low = 0;
            if (count - offset == 0)
                return 0;
            if (count - offset == 1)
                low = (UInt16)(obj[offset] & 0xff);
            if (count - offset > 1)
            {
                high = (obj[offset] & 0xff) << 8;
                low = obj[offset + 1] & 0xff;
            }
            return (UInt16)(high | low);
        }

        /// <summary>
        /// 转化byte数组为<see cref="System.UInt32"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static UInt32 ToUInt32(this byte[] obj)
        {
            return ToUInt32(obj, 0);
        }

        /// <summary>
        /// 转化byte数组为<see cref="System.UInt32"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>        
        public static UInt32 ToUInt32(this byte[] obj, int offset)
        {
            if (obj == null)
                throw new NullReferenceException();
            var count = obj.Count();
            int high = 0;
            int second = 0;
            int third = 0;
            int low = 0;
            if (count - offset == 0)
                return 0;
            if (count - offset == 1)
                low = obj[offset] & 0xff;
            if (count - offset == 2)
            {
                third = (obj[offset + 0] & 0xff) << 8;
                low = obj[offset + 1] & 0xff;
            }
            if (count - offset == 3)
            {
                second = (obj[offset + 0] & 0xff) << 16;
                third = (obj[offset + 1] & 0xff) << 8;
                low = obj[offset + 2] & 0xff;
            }
            if (count - offset > 3)
            {
                high = (obj[offset + 0] & 0xff) << 24;
                second = (obj[offset + 1] & 0xff) << 16;
                third = (obj[offset + 2] & 0xff) << 8;
                low = obj[offset + 3] & 0xff;
            }
            return (UInt32)(high | second | third | low);
        }

        /// <summary>
        /// 转化byte数组为<see cref="System.Int32"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int32 ToInt32(this byte[] obj)
        {
            return ToInt32(obj, 0);
        }

        /// <summary>
        /// 转化byte数组为<see cref="System.Int32"/>。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Int32 ToInt32(this byte[] obj, int offset)
        {
            if (obj == null)
                throw new NullReferenceException();
            var count = obj.Count();
            int high = 0;
            int second = 0;
            int third = 0;
            int low = 0;
            if (count - offset == 0)
                return 0;
            if (count - offset == 1)
                low = obj[offset + 0] & 0xff;
            if (count - offset == 2)
            {
                third = (obj[offset + 0] & 0xff) << 8;
                low = obj[offset + 1] & 0xff;
            }
            if (count - offset == 3)
            {
                second = (obj[offset + 0] & 0xff) << 16;
                third = (obj[offset + 1] & 0xff) << 8;
                low = obj[offset + 2] & 0xff;
            }
            if (count - offset > 3)
            {
                high = (obj[offset + 0] & 0xff) << 24;
                second = (obj[offset + 1] & 0xff) << 16;
                third = (obj[offset + 2] & 0xff) << 8;
                low = obj[offset + 3] & 0xff;
            }
            return (Int32)(high | second | third | low);
        }

        /// <summary>
        /// 比较当前byte数组与另一数组是否相等。
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="target">需要比较的数组。</param>
        /// <returns></returns>
        public static bool EqualsBytes(this byte[] obj, params byte[] target)
        {
            if (obj.Length != target.Length)
                return false;
            for (int i = 0; i < obj.Length; i++)
            {
                if (obj[i] != target[i])
                    return false;
            }
            return true;
        }

    }
}
