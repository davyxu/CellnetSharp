using Cellnet;
using System;
using System.Threading;

namespace UnitTest
{
    public class QPSMeter
    {
        int _qps;
        int _total;
        int _count;
        Timer _timer;

        public QPSMeter( EventDispatcher ed, Action<int> callback )
        {

            _timer = Subscribe.RegisterTimer(ed, 1000, delegate
            {
                var qps = Turn();

                callback(qps);
            });
        }

        public int Acc( )
        {
            _qps++;

            return _count;
        }

        public int Average
        {
            get
            {
                if (_count == 0)
                    return 0;

                return _total / _count;
            }
        }

        int Turn( )
        {
            int ret = 0;

            if ( _qps > 0 )
            {
                ret = _qps;
            }

            _total += _qps;
            _qps = 0;
            _count++;

            return ret;
        }


    }
}
