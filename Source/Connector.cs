using System;
using System.Net;
using System.Net.Sockets;

namespace Cellnet
{
    public class Connector : Peer
    {
        Socket _socket;        
        Session _session;

        public int AutoReconnectSec
        {
            get;
            set;
        }

        public bool _recvConnected;

        public Connector(EventQueue q, Codec c)
            : base( q, c )
        {

        }

        public override Peer Start(string address)
        {
            if (_session != null)
                return this;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);         

            _session = AddSession(_socket, _queue);            

            var addr = new NetworkAddress();
            addr.Resolve(address, ResolveDNS, delegate
            {
                BeginConnect(addr.AddressList, addr.Port);

            });

            return this;
        }

        void BeginConnect(IPAddress[] host, int port)
        {         
            try
            {
                _socket.BeginConnect(host, port, new AsyncCallback(HandleConnected), null);
            }
            catch (SocketException ex)
            {
                _session.PostError(SessionEvent.ConnectError, ex);
                _session.Close();
            }
        }

        void HandleConnected(IAsyncResult ar)
        {

            // 当多个连接过来时, 只有第一个
            if (_recvConnected )
                return;

            try
            {
                _socket.EndConnect(ar);
            }
            catch (Exception ex)
            {
                _session.PostError(SessionEvent.ConnectError, ex);
                _session.Close();
                return;
            }

            _recvConnected = true;

            _session.ReadHeader();


            _session.PostEvent(SessionEvent.Connected);            

        }

        public override void Stop()
        {
            if ( _session != null )
            {
                _session.Close();
            }
        }
    }
}
