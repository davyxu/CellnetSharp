using System.IO;

namespace Cellnet
{
    public class Codec
    {
        public virtual bool Encode<T>(T obj, out MessageMeta meta, out MemoryStream stream) where T : class
        {
            meta = MessageMeta.Empty;
            stream = null;

            return false;
        }

        public virtual bool Decode<T>(MessageMeta meta, MemoryStream stream, out T obj) where T : class
        {
            obj = default(T);

            return false;
        }
    }
}
