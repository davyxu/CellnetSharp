
namespace gamedef
{
    
    public class SessionConnected
    {

    }

    public class SessionDisconnected
    {

    }

    public class SessionConnectError
    {

    }

    public class SessionRecvError
    {

    }

    public class SessionSendError
    {

    }
}

namespace Cellnet
{


    public struct SessionEvent
    {
        public readonly static uint Connected = MessageMetaSet.Register(typeof(gamedef.SessionConnected)).id;
        public readonly static uint Disconnected = MessageMetaSet.Register(typeof(gamedef.SessionDisconnected)).id;
        public readonly static uint ConnectError = MessageMetaSet.Register(typeof(gamedef.SessionConnectError)).id;
        public readonly static uint RecvError = MessageMetaSet.Register(typeof(gamedef.SessionRecvError)).id;
        public readonly static uint SendError = MessageMetaSet.Register(typeof(gamedef.SessionSendError)).id;

        public uint ID;
        public Session Ses;
        public PacketStream Stream;

        public SessionEvent(Session s, uint id, PacketStream stream)
        {
            ID = id;
            Ses = s;
            Stream = stream;
        }
    }
}
