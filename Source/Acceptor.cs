using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Cellnet
{
    public class Acceptor : Peer
    {
        Socket _socket;        

        public Acceptor(EventQueue q, Codec c)
            : base( q, c )
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public override Peer Start(string address)
        {
            var addr = new NetworkAddress();
            addr.Resolve(address, ResolveDNS, delegate
            {
                foreach( var a in addr.AddressList)
                {
                    var ipp = new IPEndPoint(a, addr.Port);

                    try
                    {
                        _socket.Bind(ipp);
                        _socket.Listen(5000);
                    }
                    catch( Exception )
                    {
                        PostEvent(SessionEvent.ListenError);
                        _socket.Close();

                        return;
                    }

                    

                    BeginAccept();
                }
                
            });

            return this;
        }

        void PostEvent(uint msgid)
        {
            _queue.Post(new SessionEvent(null, msgid, null));
        }


        void BeginAccept()
        {         
            try
            {
                _socket.BeginAccept(new AsyncCallback(HandleAccepted), null);
            }
            catch (SocketException)
            {
                PostEvent(SessionEvent.AcceptError);
                _socket.Close();
            }
        }

        void HandleAccepted(IAsyncResult ar)
        {

            try
            {
                var socket = _socket.EndAccept(ar);

                var ses = AddSession(socket, _queue);

                ses.ReadHeader();

                _queue.Post(new SessionEvent(ses, SessionEvent.Accepted, null));            
            }
            catch (Exception)
            {
                PostEvent(SessionEvent.AcceptError);
                _socket.Close();
                return;
            }

            BeginAccept();
        }

        public override void Stop()
        {
            _socket.Close();
        }

      
    }
}
