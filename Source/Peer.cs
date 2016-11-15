
using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
        Codec _codec;

        public EventQueue Queue
        {
            get { return _queue; }
        }

        public Codec Codec
        {
            get { return _codec; }
        }

        public Peer( EventQueue q, Codec c )
        {
            _queue = q;
            _codec = c;
        }

        public virtual Peer Start(string address) 
        {
            return this;
        }

        public virtual void Stop( )
        {

        }

        Dictionary<Int64, Session> _sessionByID = new Dictionary<long, Session>();
        object _sessionByIDGuard = new object();


        Int64 _sessionIDAcc = 0;
        object _sessionIDAccGuard = new object();

        const int TotalTryCount = 100;

        protected Session AddSession(Socket s, EventQueue q)
        {
            var tryCount = TotalTryCount;

            Int64 sesid = 0;


            while (tryCount > 0)
            {

                lock (_sessionIDAccGuard)
                {
                    _sessionIDAcc++;

                    sesid = _sessionIDAcc;
                }                

                if (GetSession(sesid) == null)
                    break;

                tryCount--;
            }

            var ses = new Session(this, s, q, sesid );

            lock (_sessionByIDGuard)
            {
                _sessionByID.Add(sesid, ses);
            }

            return ses;

        }

        public Session GetSession(Int64 id)
        {
            lock (_sessionByIDGuard)
            {
                Session s;
                if (_sessionByID.TryGetValue(id, out s))
                {
                    return s;
                }

            }

            return null;
        }

        internal void RemoveSession(Int64 id)
        {
            lock (_sessionByIDGuard)
            {
                _sessionByID.Remove(id);
            }

        }
    }
}
