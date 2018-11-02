#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class FutureNeedle<T> : LazyNeedle<T>
    {
        private readonly Action<Action> _schedule;
        private int _status;

        public FutureNeedle(Func<T> valueFactory)
            : base(valueFactory)
        {
            _schedule = Schedule;
        }

        public FutureNeedle(Func<T> valueFactory, bool autoSchedule)
            : base(valueFactory)
        {
            _schedule = Schedule;
            if (autoSchedule)
            {
                Schedule();
            }
        }

        public FutureNeedle(Action<Action> schedule, Func<T> valueFactory)
            : base(valueFactory)
        {
            _schedule = schedule;
        }

        public FutureNeedle(Action<Action> schedule, Func<T> valueFactory, bool autoSchedule)
            : base(valueFactory)
        {
            _schedule = schedule;
            if (autoSchedule)
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

        protected override void Initialize(Action beforeInitialize)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait();
            }
            else
            {
                base.Initialize(beforeInitialize);
            }
        }

        public bool Schedule()
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                return false;
            }
            _schedule(base.Initialize);
            return true;
        }

        public override void Wait()
        {
            Initialize();
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait(cancellationToken);
            }
            else
            {
                base.Initialize();
            }
        }

        public override void Wait(int milliseconds)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait(milliseconds);
            }
            else
            {
                var start = ThreadingHelper.TicksNow();
                base.Initialize();
                milliseconds -= (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                if (milliseconds > 0)
                {
                    base.Wait(milliseconds);
                }
            }
        }

        public override void Wait(TimeSpan timeout)
        {
            Wait((int)timeout.TotalMilliseconds);
        }

        public override void Wait(int milliseconds, CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait(milliseconds, cancellationToken);
            }
            else
            {
                var start = ThreadingHelper.TicksNow();
                base.Initialize();
                milliseconds -= (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                if (milliseconds > 0)
                {
                    base.Wait(milliseconds, cancellationToken);
                }
            }
        }

        public override void Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        private static void Schedule(Action action)
        {
            var waitCallback = new WaitCallback(_ => action());
            ThreadPool.QueueUserWorkItem(waitCallback);
        }
    }
}

#endif