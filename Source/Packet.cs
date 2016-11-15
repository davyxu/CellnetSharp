using System;
using System.IO;

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



    class PacketSerializer
    {
        public static PacketHeader ReadHeader( PacketStream ps, UInt16 tag )
        {
            PacketHeader header;

            using (MemoryStream stream = new MemoryStream(ps.Buff, 0, ps.Size))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    header.MsgID = reader.ReadUInt32();
                    header.Tag = reader.ReadUInt16();
                    header.TotalSize = reader.ReadUInt16();

                    // 检查包头
                    if (header.Tag == tag )
                    {
                        return header;
                    }
                }
            }

            return default(PacketHeader);
        }

        public static PacketStream WriteFull(PacketHeader header, byte[] payload)
        {
            PacketStream ps = new PacketStream(header.TotalSize, null, null);

            using (MemoryStream stream = new MemoryStream(ps.Buff, 0, ps.Size))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(header.MsgID);
                    writer.Write(header.Tag);
                    writer.Write(header.TotalSize);
                    writer.Write(payload, 0, payload.Length);
                    writer.Flush();
                }
            }

            return ps;
        }
    }
}
