#if FAT

using System;
using System.Threading;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public class FutureAction : LazyAction
    {
        private readonly Action<Action> _schedule;
        private int _status;

        public FutureAction(Action action)
            : base(action)
        {
            _schedule = Schedule;
        }

        public FutureAction(Action action, bool autoSchedule)
            : base(action)
        {
            _schedule = Schedule;
            if (autoSchedule)
            {
                Schedule();
            }
        }

        public FutureAction(Action<Action> schedule, Action action)
            : base(action)
        {
            _schedule = schedule;
        }

        public FutureAction(Action<Action> schedule, Action action, bool autoSchedule)
            : base(action)
        {
            _schedule = schedule;
            if (autoSchedule)
            {
                Schedule();
            }
        }

        public override void Execute()
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait();
            }
            else
            {
                base.Execute();
            }
        }

        protected override void Execute(Action beforeInitialize)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait();
            }
            else
            {
                base.Execute(beforeInitialize);
            }
        }

        public bool Schedule()
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                return false;
            }
            _schedule(base.Execute);
            return true;
        }

        public override void Wait()
        {
            Execute();
        }

        public override void Wait(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref _status, 1, 0) != 0)
            {
                base.Wait(cancellationToken);
            }
            else
            {
                base.Execute();
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
                base.Execute();
                milliseconds = milliseconds - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
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
                base.Execute();
                milliseconds = milliseconds - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
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