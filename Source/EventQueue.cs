using System;
using System.Collections.Generic;
using System.Threading;

namespace Cellnet
{
    struct EventData
    {        
        public object Data;
        DateTime TimeStamp;

        public EventData( object data, DateTime tm)
        {
            Data = data;
            TimeStamp = tm;
        }

        public bool IsTimeup(uint delayMs)
        {
            var dur = DateTime.UtcNow - TimeStamp;
            return (uint)dur.TotalMilliseconds > delayMs;
        }
    }


    public class EventQueue
    {
        Queue<EventData> _msgQueue = new Queue<EventData>();
        object _msgQueueGuard = new object();

        public Action<object> OnEvent = null;

        // 延迟模拟
        Queue<EventData> _delayQueue = new Queue<EventData>();
        public uint EmulateDelayMS
        {
            get;
            set;
        }        

        public void Post( object data)
        {
            lock (_msgQueueGuard)
            {                
                _msgQueue.Enqueue(new EventData(data, DateTime.UtcNow));
            }
        }



        public void Polling( )
        {
            // 有延迟消息到达投递点
            if (EmulateDelayMS > 0 && _delayQueue.Count > 0)
            {
                var dm = _delayQueue.Peek();
                if (dm.IsTimeup(EmulateDelayMS))
                {
                    _delayQueue.Dequeue();

                    if (OnEvent != null)
                    {
                        OnEvent(dm.Data);
                    }
                }
            }

            EventData ev;

            lock (_msgQueueGuard)
            {
                if (_msgQueue.Count == 0)
                {
                    return;
                }

                ev = _msgQueue.Dequeue();
            }

            if (EmulateDelayMS > 0)
            {                
                _delayQueue.Enqueue(ev);
            }
            else
            {
                if (OnEvent != null)
                {
                    OnEvent(ev.Data);
                }                
            }
        }
    }
}
