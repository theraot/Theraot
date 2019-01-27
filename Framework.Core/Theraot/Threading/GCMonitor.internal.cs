// Needed for Workaround

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
#if LESSTHAN_NETSTANDARD20
    public static partial class GCMonitor
    {
        private static partial class Internal
        {
            private static readonly Action _work = RaiseCollected;

            public static void Invoke()
            {
                System.Threading.Tasks.Task.Run(_work);
            }
        }
    }

#else

    public static partial class GCMonitor
    {
        private static partial class Internal
        {
            private static readonly WaitCallback _work = _ => RaiseCollected();

            public static void Invoke()
            {
                ThreadPool.QueueUserWorkItem(_work);
            }
        }
    }

#endif

    public static partial class GCMonitor
    {
        private static partial class Internal
        {
            public static WeakDelegateCollection CollectedEventHandlers { get; } = new WeakDelegateCollection(false, false);

            private static void RaiseCollected()
            {
                var check = Volatile.Read(ref _status);
                if (check != _statusReady)
                {
                    return;
                }

                try
                {
                    CollectedEventHandlers.RemoveDeadItems();
                    CollectedEventHandlers.Invoke(null, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    // Catch them all
                    No.Op(exception);
                }

                Volatile.Write(ref _status, _statusReady);
            }
        }
    }
}