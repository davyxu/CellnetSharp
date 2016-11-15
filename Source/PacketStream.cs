using System;
using System.IO;

namespace Cellnet
{
    public class PacketStream
    {
        const int StaticLength = 512;

        byte[] _data = new byte[StaticLength];
        int _size;
        int _offset;

        Action<PacketStream, object> _callback;
        object _obj;
        Session _ses;

        public byte[] Buff
        {
            get { return _data; }
        }

        public int Size
        {
            get { return _size; }
        }

        public int Offset
        {
            get { return _offset; }            
        }

        public int BytesToOperate
        {
            get { return _size - _offset; }
        }

        public void Notify( )
        {
            _callback(this, _obj);
        }

        public MemoryStream ToStream()
        {
            return new MemoryStream(_data, 0, _size);
        }

        public PacketStream( int size, Action<PacketStream, object> callback, object obj )
        {            
            _callback = callback;

            _obj = obj;

            var final = Math.Max(size, StaticLength);

            _data = new byte[final];

            _size = size;
        }

        public bool OperateDone( int size )
        {
            _offset += size;

            return _offset == _size;
        }        
    }
}
