#if FAT

using System;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Core
{
    public static class MonitorHelper
    {
        public static void Enter(object obj)
        {
            Monitor.Enter(obj);
        }

        public static void Enter(object obj, ref bool taken)
        {
            Monitor.Enter(obj);
            GC.KeepAlive(taken);
            ThreadingHelper.MemoryBarrier();
            taken = true;
        }

        public static void Enter(object obj, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (!Monitor.TryEnter(obj))
            {
                spinWait.SpinOnce();
                goto retry;
            }
        }

        public static bool TryEnter(object obj)
        {
            return Monitor.TryEnter(obj);
        }

        public static void TryEnter(object obj, ref bool taken)
        {
            GC.KeepAlive(taken);
            taken = Monitor.TryEnter(obj);
        }

        public static bool TryEnter(object obj, int millisecondsTimeout)
        {
            return Monitor.TryEnter(obj, millisecondsTimeout);
        }

        public static void TryEnter(object obj, int millisecondsTimeout, ref bool taken)
        {
            GC.KeepAlive(taken);
            taken = Monitor.TryEnter(obj, millisecondsTimeout);
        }

        public static bool TryEnter(object obj, TimeSpan timeout)
        {
            return Monitor.TryEnter(obj, timeout);
        }

        public static void TryEnter(object obj, TimeSpan timeout, ref bool taken)
        {
            GC.KeepAlive(taken);
            taken = Monitor.TryEnter(obj, timeout);
        }

        public static bool TryEnter(object obj, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (millisecondsTimeout == -1)
            {
                Enter(millisecondsTimeout, cancellationToken);
                return true;
            }
            var spinWait = new SpinWait();
            var start = ThreadingHelper.TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (TryEnter(obj))
            {
                return true;
            }
            if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < millisecondsTimeout)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void TryEnter(object obj, int millisecondsTimeout, ref bool taken, CancellationToken cancellationToken)
        {
            GC.KeepAlive(taken);
            taken = TryEnter(obj, millisecondsTimeout, cancellationToken);
        }

        public static bool TryEnter(object obj, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            var spinWait = new SpinWait();
            var start = ThreadingHelper.TicksNow();
            retry:
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (TryEnter(obj))
            {
                return true;
            }
            if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < milliseconds)
            {
                spinWait.SpinOnce();
                goto retry;
            }
            return false;
        }

        public static void TryEnter(object obj, TimeSpan timeout, ref bool taken, CancellationToken cancellationToken)
        {
            GC.KeepAlive(taken);
            taken = TryEnter(obj, timeout, cancellationToken);
        }
    }
}

#endif