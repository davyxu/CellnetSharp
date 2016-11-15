using System;
using System.IO;

namespace Cellnet
{
    public class ProtobufCodec : Codec
    {
        public override bool Encode<T>(T obj, out MessageMeta meta, out MemoryStream stream)
        {
            stream = new MemoryStream();

            meta = MessageMetaSet.GetByType(typeof(T));            

            try
            {
                ProtoBuf.Serializer.Serialize(stream, obj);
            }
            catch (Exception)
            {

                return false;
            }


            return true;
        }

        public override bool Decode<T>(MessageMeta meta, MemoryStream stream, out T obj)
        {
            obj = default(T);

            try
            {
                obj = ProtoBuf.Serializer.NonGeneric.Deserialize(meta.type, stream) as T;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

}
