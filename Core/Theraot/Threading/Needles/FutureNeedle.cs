#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class FutureNeedle<T> : LazyNeedle<T>
    {
        private int _status;

        public FutureNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            Schedule();
        }

        public FutureNeedle(Func<T> valueFactory, bool schedule)
            : base(valueFactory)
        {
            if (schedule)
            {
                Schedule();
            }
        }

        public override void Initialize()
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
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