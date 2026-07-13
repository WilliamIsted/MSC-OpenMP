using System;
using System.Collections.Generic;

namespace MscOpenMp.Mod.Net
{
    // Socket threads enqueue; Unity main thread drains in Update().
    public class InboundQueue
    {
        readonly Queue<Action> _q = new Queue<Action>();
        readonly object _lock = new object();

        public void Post(Action a) { lock (_lock) _q.Enqueue(a); }

        public void DrainOnMainThread()
        {
            while (true)
            {
                Action a;
                lock (_lock)
                {
                    if (_q.Count == 0) return;
                    a = _q.Dequeue();
                }
                a();
            }
        }
    }
}
