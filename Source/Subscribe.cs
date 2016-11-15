using System;

namespace Cellnet
{
    public class Subscribe
    {
        public static MessageMeta RegisterMessage<T>( EventDispatcher ed, Action<T, Session> callback ) where T:class
        {
            var meta = MessageMetaSet.GetByType<T>();
            if (meta.Equals(MessageMeta.Empty))
                return MessageMeta.Empty;

            ed.Add(meta.id, (obj) =>
            {
                var ev = (SessionEvent)obj;                

                callback(null, ev.Ses);

            });

            return meta;
        }


        public static MessageMeta RegisterMessage( EventDispatcher ed, string msgName, Action<object, Session> callback )
        {
            var meta = MessageMetaSet.GetByName(msgName);
            if (meta.Equals( MessageMeta.Empty) )
                return MessageMeta.Empty;

            ed.Add(meta.id, (obj) =>
            {
                var ev = (SessionEvent)obj;

                callback( obj, ev.Ses);

            });

            return meta;
        }
    }
}
