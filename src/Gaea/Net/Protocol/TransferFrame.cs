using System;
using Gaea.Extension.System;
using Gaea.Algorithm;

namespace Gaea.Net.Protocol
{
    /// <summary>
    /// 帧协议。负责对TCP流区分帧。
    /// </summary>
    public class TransferFrame : IProtocol
    {
        /// <summary>
        /// 协议头长度。
        /// </summary>
        public static ushort HeadLength = 4;

        /// <summary>
        /// 本协议最大数据容量。
        /// </summary>
        public static ushort Capacity = ushort.MaxValue - 4;

        /// <summary>
        /// 摘要计算器。
        /// </summary>
        private IDigest<ushort> digestor;
        /// <summary>
        /// 协议最数据容量。
        /// </summary>
        public IDigest<ushort> Digestor
        {
            get { return this.digestor; }
            set { this.digestor = value; }
        }

        #region Head

        ushort length;
        /// <summary>
        /// 协议数据（除去头部部分）长度。
        /// </summary>
        public ushort Length
        {
            get { return this.length; }
        }

        ushort digest;
        /// <summary>
        /// 16位数据校验。
        /// </summary>
        public ushort Digest
        {
            get { return this.digest; }
        }

        #endregion

        byte[] data;
        public byte[] Data
        {
            get
            {
                return this.data;
            }
            set
            {
                if (value.Length > TransferFrame.Capacity)
                    throw new Exception("数据超出协议最大容量！");

                this.data = value;
                this.length = (ushort)value.Length;
            }
        }

        #region Ctor
        public TransferFrame() : this(new NullDigest16())
        {
        }

        public TransferFrame(IDigest<ushort> digestor)
        {
            this.digestor = digestor;
        }

        /// <summary>
        /// 计算摘要校验码。
        /// </summary>
        public void CalcDigest()
        {
            if (this.data != null)
                this.digest = this.digestor.Calculate(this.data);
        }

        /// <summary>
        /// 验证摘要校验码是否对应。
        /// </summary>
        public bool Validate()
        {
            if (this.data != null)
                return this.digestor.Calculate(this.data) == this.digest;
            return false;
        }
        #endregion

        /// <summary>
        /// 序列化成二进制。
        /// </summary>
        public byte[] ToBytes()
        {
            byte[] bytes = new byte[this.length + 4];
            this.length.ToBytesBuffer(bytes, 0);
            this.digest.ToBytesBuffer(bytes, 2);
            Array.Copy(this.data, 0, bytes, 4, this.data.Length);
            return bytes;
        }

        /// <summary>
        /// 解析头部数据。
        /// </summary>
        public void LoadHead(byte[] bytes)
        {
            this.length = bytes.ToUInt16(0);
            this.digest = bytes.ToUInt16(2);
        }

        /// <summary>
        /// 清空协议数据。
        /// </summary>
        public void Clear()
        {
            this.length = 0;
            this.digest = 0;
            this.data = null;
        }
    }
}