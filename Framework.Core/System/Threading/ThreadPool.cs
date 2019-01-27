#if TARGETS_NETSTANDARD
using Theraot.Collections.ThreadSafe;

namespace System.Threading
{
    public static class ThreadPool
    {
        private static readonly Pool<ThreadPoolThread> _pool = new Pool<ThreadPoolThread>(1024, null);
        private static int _threadCount;

        private static readonly ThreadSafeQueue<Action> _work = new ThreadSafeQueue<Action>();

        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }
            _work.Add(() => callBack(null));
            if (Volatile.Read(ref _threadCount) >= Environment.ProcessorCount)
            {
                return true;
            }

            if (_pool.TryGet(out var thread))
            {
                thread.Awake();
            }
            else
            {
                GC.KeepAlive(new ThreadPoolThread());
            }
            return true;
        }

        private class ThreadPoolThread
        {
            private readonly AutoResetEvent _event;

            public ThreadPoolThread()
            {
                _event = new AutoResetEvent(false);
                var thread = new Thread(Work);
                thread.Start();
            }

            public void Awake()
            {
                _event.Set();
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
        }
    }
}

#endif