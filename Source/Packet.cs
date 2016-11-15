using System;

namespace Cellnet
{
    /// <summary>
    /// 消息头
    /// </summary>
    public struct PacketHeader
    {
        /// <summary>
        /// 消息头长度
        /// </summary>
        public const Int32 HeaderSize = 4 + 2 + 2;

        //消息ID
        public UInt32 MsgID;
        //Tag
        public UInt16 Tag;
        //消息长度 （包含消息头）
        public UInt16 TotalSize;        

        public Int32 PayloadSize
        {
            get { return (Int32)(TotalSize - PacketHeader.HeaderSize); }
        }

        public bool Valid
        {
            get { return MsgID != 0; }
        }        

    }
}
