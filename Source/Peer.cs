
namespace Cellnet
{
    public class Peer
    {
        public int MaxPacketSize
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }
        public bool ResolveDNS
        {
            get;
            set;
        }


        protected EventQueue _queue;

        public EventQueue Queue
        {
            get { return _queue; }
        }

        public Peer( EventQueue q )
        {
            _queue = q;
        }

        public virtual Peer Start(string address) 
        {
            return this;
        }

        public virtual void Stop( )
        {

        }
    }
}
