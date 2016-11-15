using System;
using System.Net;

namespace Cellnet
{
    public class NetworkAddress
    {                
        int _port;
        Action _done;
        IPAddress[] _list;

        public int Port
        {
            get { return _port; }
        }

        public IPAddress[] AddressList
        {
            get { return _list; }
        }

        /// <summary>
        /// 开始解析地址
        /// </summary>
        /// <param name="address">ip:port或 域名:port</param>
        /// <param name="resolveDNS">是否需要解析dns</param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Resolve( string address, bool resolveDNS, Action callback )
        {            
            _done = callback;

            string host;

            if ( !SpliteAddress( address, out host, out _port ) )
            {
                return false;
            }

            if ( resolveDNS )
            {
                Dns.BeginGetHostEntry(host, new AsyncCallback(HandleBeginGetHostEntry), null);
            }
            else
            {
                var addr = IPAddress.Parse(host);

                _list = new IPAddress[] { addr };                

                if (_done != null)
                {
                    _done();
                }
            }

            return true;
        }

        void HandleBeginGetHostEntry(IAsyncResult result)
        {
            var ips = Dns.EndGetHostEntry(result);

            _list = ips.AddressList;

            if (_done != null)
            {
                _done();
            }
        }

        static bool SpliteAddress(string address, out string host, out int port)
        {
            String[] arr = address.Split(':');

            if (arr.Length < 2)
            {
                host = "";
                port = 0;

                return false;
            }

            host = arr[0];
            port = (int)Int32.Parse(arr[1]);

            return true;
        }
    }
}
