#if LESSTHAN_NETSTANDARD13

#pragma warning disable CA1001 // Types that own disposable fields should be disposable

using Theraot.Collections.ThreadSafe;

namespace System.Threading
{
    public static class ThreadPool
    {
        private static readonly Pool<ThreadPoolThread> _pool = new Pool<ThreadPoolThread>(1024, recycler: null);
        private static readonly ThreadSafeQueue<Action> _work = new ThreadSafeQueue<Action>();
        private static int _threadCount;

        public static bool QueueUserWorkItem(WaitCallback callBack)
        {
            if (callBack == null)
            {
                throw new ArgumentNullException(nameof(callBack));
            }

            _work.Add(() => callBack(state: null));
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

        private sealed class ThreadPoolThread
        {
            private AutoResetEvent _event;

            public ThreadPoolThread()
            {
                _event = new AutoResetEvent(initialState: false);
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
                    var e = Volatile.Read(ref _event);
                    if (e == null)
                    {
                        break;
                    }

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
                    e.WaitOne();
                }
            }
        }
    }
}

#endif