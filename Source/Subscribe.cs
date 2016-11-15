using System;
using System.Threading;

namespace Cellnet
{
    public class Subscribe
    {
        public static MessageMeta RegisterMessage<T>( EventDispatcher ed, Action<T, Session> callback ) where T:class
        {
            var meta = MessageMetaSet.GetByType<T>();

            if (meta.Equals(MessageMeta.Empty))
            {
                throw new Exception("Register message failed: " + typeof(T).FullName );
            }

            ed.Add(meta.id, (obj) =>
            {
                var ev = (SessionEvent)obj;

                if ( ev.Stream != null)
                {
                    T msg;
                    if (ev.Ses.Peer.Codec.Decode<T>(meta, ev.Stream.ToStream(), out msg))
                    {
                        callback(msg, ev.Ses);
                    }
                }
                else
                {
                    callback(null, ev.Ses);
                }
            });

            return meta;
        }

        public static Timer RegisterTimer(EventDispatcher ed, int durationMS, Action callback)
        {
            return new Timer( (sender) =>{

                ed.Queue.Post(callback);                

            }, null, 0, durationMS);            
        }

    }
}
