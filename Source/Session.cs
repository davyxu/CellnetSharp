using System.Net.Sockets;
using System.IO;
using System;

namespace Cellnet
{
    public class Session
    {
        /// <summary>
        /// socket
        /// </summary>
        Socket _socket;

        /// <summary>
        /// 包验证
        /// </summary>
        UInt16 _sendTag = 1;
        UInt16 _recvTag = 1;
        object _sendTagGuard = new object();

        EventQueue _queue;

        /// <summary>
        /// 超时时长
        /// </summary>
        public int _timeoutSecond = 5000;

        /// <summary>
        /// 是否连接上
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_socket == null)
                    return false;

                return _socket.Connected;
            }
        }


        public Session(Socket s, EventQueue q )
        {
            _socket = s;
            _queue = q;
            _sendTag = 1;
            _recvTag = 1;
        }
        void ReadHeader()
        {
            ReadPacket(new PacketStream( PacketHeader.HeaderSize, HandleReadHeader, null));        
        }

        /// <summary>
        /// 读取包头
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        void HandleReadHeader(PacketStream ps, object obj)
        {
            PacketHeader header = PacketSerializer.ReadHeader(ps, _recvTag);

            if ( header.Valid)
            {
                var size = (Int32)(header.TotalSize - PacketHeader.HeaderSize);

                // 还有包体
                if (size > 0)
                {
                    ReadPacket(new PacketStream( size, HandleReadHeader, header));
                }
                else
                {                    
                    
                    PostPacket(header, null);

                    // 包体没有, 继续读下一个包
                    ReadHeader();
                }

            }
            else
            {                
                PostEvent(SessionEvent.RecvError);
                Close();                
            }
        }

        void HandleReadBody(PacketStream ps, object obj)
        {
            var header = (PacketHeader)obj;

            PostPacket(header, ps);
        }

        void PostPacket(PacketHeader header, PacketStream stream)
        {            
            _queue.Post( new SessionEvent(this, header.MsgID, stream));
            _recvTag++;
        }

        public void PostEvent( uint msgid )
        {
            _queue.Post(new SessionEvent(this, msgid, null));
        }


        void ReadPacket(PacketStream ps)
        {
            try
            {
                _socket.BeginReceive(ps.Buff, 
                    ps.Offset, 
                    ps.BytesToOperate, 
                    SocketFlags.None, 
                    new AsyncCallback(HandleRecvData), ps);
            }
            catch( SocketException )
            {                
                PostEvent(SessionEvent.Disconnected);
            }

        }

        void HandleRecvData(IAsyncResult ar)
        {
            var ps = (PacketStream)ar.AsyncState;

            try
            {
                int recvSize = _socket.EndReceive(ar);

                if ( ps.OperateDone(recvSize) )
                {
                    ps.Notify();
                }
                else
                {
                    ReadPacket(ps);
                }
            }
            catch(Exception)
            {                
                PostEvent(SessionEvent.Disconnected);
                Close();
            }            
        }
        

        public void SendPacket(UInt32 msgID, byte[] payload)
        {
            if (_socket == null || !_socket.Connected || payload == null)
            {
                return;
            }

            var header = new PacketHeader();
            header.Tag = GenSendTag();
            header.MsgID = msgID;
            header.TotalSize = (UInt16)(PacketHeader.HeaderSize + payload.Length);

            var ps = PacketSerializer.WriteFull(header, payload);

            try
            {
                if (_socket != null && _socket.Connected)
                {
                    SendStream(ps);
                }


            }
            catch (Exception)
            {                
                PostEvent(SessionEvent.SendError);
            }

        }

        void SendStream(PacketStream ps)
        {
            _socket.BeginSend(ps.Buff,
                ps.Offset,
                ps.BytesToOperate,
                SocketFlags.None,
                new AsyncCallback(HandleSend),
                ps);
        }

        void HandleSend(IAsyncResult ar)
        {
            var ps = ar as PacketStream;

            try
            {
                int sendSize = _socket.EndSend(ar);

                if ( !ps.OperateDone(sendSize) )
                {
                    SendStream(ps);
                }                
            }
            catch( Exception )
            {                
                PostEvent(SessionEvent.SendError);
            }
           
        }


        UInt16 GenSendTag()
        {
            lock (_sendTagGuard)
            {
                var tag = _sendTag;
                _sendTag++;
                return tag;
            }
        }

        public void Close()
        {
            //关闭socket
            if (_socket == null)
                return;


            if (_socket.Connected)
            {
                _socket.Shutdown(SocketShutdown.Both);
            }

            _socket.Close();

            _socket = null;

        }


    }


}