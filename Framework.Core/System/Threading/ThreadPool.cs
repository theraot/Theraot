#if NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6

using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace System.Threading
{
    [Runtime.InteropServices.ComVisible(true)]
    public delegate void WaitCallback(object state);

    public static class ThreadPool
    {
        private class ThreadPoolThread
        {
            private readonly AutoResetEvent _event;

            public ThreadPoolThread()
            {
                _event = new AutoResetEvent(false);
                var thread = new Thread(Work);
                thread.Start();
            }

            private void Work()
            {
                while (true)
                {
                    Interlocked.Increment(ref _threadCount);
                    try
                    {
                        while (_work.TryTake(out var action))
                        {
                            // Unhandled exceptions?
                            action();
                        }
                    }
                    finally
                    {
                        Interlocked.Decrement(ref _threadCount);
                    }
                    _pool.Donate(this);
                    _event.WaitOne();
                }
            }

            public void Awake()
            {
                _event.Set();
            }
        }

        private static readonly SafeQueue<Action> _work = new SafeQueue<Action>();
        private static readonly Pool<ThreadPoolThread> _pool = new Pool<ThreadPoolThread>(1024, null);
        private static int _threadCount;

        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            _work.Add(() => callBack(null));
            if (Volatile.Read(ref _threadCount) < EnvironmentHelper.ProcessorCount)
            {
                if (_pool.TryGet(out var thread))
                {
                    thread.Awake();
                }
                else
                {
                    GC.KeepAlive(new ThreadPoolThread());
                }
            }
            return true;
        }
    }
}

#endif