using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

#if FAT && (NET20 || NET30 || NET35)
using System.Threading.Tasks;
#endif

namespace Theraot.Threading
{
    public static partial class GCMonitor
    {
        private static class Internal
        {
            private static readonly WeakDelegateCollection _collectedEventHandlers;
#if FAT && (NET20 || NET30 || NET35)
            private static readonly Task _task;
#endif

            static Internal()
            {
#if FAT && (NET20 || NET30 || NET35)
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
#if FAT && (NET20 || NET30 || NET35)
                _task.Restart();
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