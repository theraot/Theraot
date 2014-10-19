using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    public static partial class GCMonitor
    {
        private static class Internal
        {
            private static readonly WeakDelegateSet _collectedEventHandlers;
#if FAT
            private static readonly Work _work;
#endif

            static Internal()
            {
#if FAT
                _work = WorkContext.DefaultContext.AddWork(RaiseCollected);
#endif
                _collectedEventHandlers = new WeakDelegateSet(INT_CapacityHint, false, false, INT_MaxProbingHint);
            }

            public static WeakDelegateSet CollectedEventHandlers
            {
                get
                {
                    return _collectedEventHandlers;
                }
            }

            public static void Invoke()
            {
#if FAT
                _work.Start();
#else
                ThreadPool.QueueUserWorkItem(_ => RaiseCollected());
#endif
            }

            private static void RaiseCollected()
            {
                var check = Thread.VolatileRead(ref _status);
                if (check == INT_StatusReady)
                {
                    try
                    {
                        _collectedEventHandlers.RemoveDeadItems();
                        _collectedEventHandlers.Invoke(null, new EventArgs());
                    }
                    catch (Exception exception)
                    {
                        // Pokemon
                        GC.KeepAlive(exception);
                    }
                    Thread.VolatileWrite(ref _status, INT_StatusReady);
                }
            }
        }
    }
}