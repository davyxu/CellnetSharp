using System;
using System.Collections.Generic;
using System.Threading;

namespace Cellnet
{
    public class EventDispatcher
    {
        Dictionary<uint, Action<object>> _msgCallbacks = new Dictionary<uint, Action<object>>();

        EventQueue _queue = new EventQueue();

        Thread _thread;
        bool _running;
        object _runningGuard = new object();
        AutoResetEvent _exitSignal = new AutoResetEvent(false);

        public EventQueue Queue
        {
            get{

                return _queue;
            }
        }

        public EventDispatcher( )
        {
            _queue.OnEvent += Invoke;
        }        

        public void Add(uint msgid, Action<object> callback)
        {
            Action<object> callbacks;
            if (_msgCallbacks.TryGetValue(msgid, out callbacks))
            {
                callbacks += callback;
                _msgCallbacks[msgid] = callbacks;
            }
            else
            {
                callbacks += callback;

                _msgCallbacks.Add(msgid, callbacks);
            }
        }

        public void Remove(uint msgid, Action<object> callback)
        {
            Action<object> callbacks;
            if (_msgCallbacks.TryGetValue(msgid, out callbacks))
            {
                callbacks -= callback;
                _msgCallbacks[msgid] = callbacks;
            }
        }

        public void Invoke( object msg)
        {
            var ev = (SessionEvent)msg;


            Action<object> callbacks;
            if (!_msgCallbacks.TryGetValue(ev.ID, out callbacks))
            {
                return;
            }

            callbacks.Invoke(msg);
        }



        public bool Running
        {
            get
            {
                lock (_runningGuard)
                {
                    return _running;
                }
            }

            set
            {
                lock (_runningGuard)
                {
                    _running = value;
                }
            }
        }

        public void Start( )
        {
            Running = true;
            _thread = new Thread(EventLoop);
            _thread.IsBackground = true;
            _thread.Start();
        }

        public void Stop( )
        {
            Running = false;
        }


        public void Wait( )
        {
            _exitSignal.WaitOne();
        }

        void EventLoop()
        {
            while( Running )
            {
                _queue.Polling();
            }

            _exitSignal.Set();
        }
    }

}
