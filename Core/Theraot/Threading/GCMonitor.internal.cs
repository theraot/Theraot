using System;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    public static partial class GCMonitor
    {
        private static class Internal
        {
            private static readonly WeakDelegateCollection _collectedEventHandlers;
#if FAT
            private static readonly Task _task;
#endif

            static Internal()
            {
#if FAT
                _task = TaskScheduler.Default.AddWork(RaiseCollected);
#endif
                _collectedEventHandlers = new WeakDelegateCollection(false, false, INT_MaxProbingHint);
            }

            public static WeakDelegateCollection CollectedEventHandlers
            {
                get
                {
                    return _collectedEventHandlers;
                }
            }

            public static void Invoke()
            {
#if FAT
                _task.Start();
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