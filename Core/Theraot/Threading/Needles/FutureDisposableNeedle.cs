#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class FutureDisposableNeedle<T> : LazyDisposableNeedle<T>
        where T : IDisposable
    {
        private int _status;

        public FutureDisposableNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            Schedule();
        }

        public FutureDisposableNeedle(Func<T> valueFactory, bool schedule)
            : base(valueFactory)
        {
            if (schedule)
            {
                Schedule();
            }
        }

        public override void Initialize()
        {
            if (Volatile.Read(ref _status) == 1)
            {
                base.Wait();
            }
            else
            {
                base.Initialize();
            }
        }

        public bool Schedule()
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                return false;
            }
            var waitCallback = new WaitCallback(_ => Initialize());
            ThreadPool.QueueUserWorkItem(waitCallback);
            return true;
        }

        public override void Wait()
        {
            Initialize();
        }
    }
}

#endif