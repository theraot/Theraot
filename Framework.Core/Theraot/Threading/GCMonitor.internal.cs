// Needed for Workaround

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    public static partial class GCMonitor
    {
        private static class Internal
        {
#if NETSTANDARD1_5 || NETSTANDARD1_6 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4

            private static readonly Action _work;

            static Internal()
            {
                _work = RaiseCollected;
                CollectedEventHandlers = new WeakDelegateCollection(false, false);
            }

            public static void Invoke()
            {
                System.Threading.Tasks.Task.Run(_work);
            }
#else
            private static readonly WaitCallback _work;

            static Internal()
            {
                _work = _ => RaiseCollected();
                CollectedEventHandlers = new WeakDelegateCollection(false, false);
            }

            public static void Invoke()
            {
                ThreadPool.QueueUserWorkItem(_work);
            }

#endif

            public static WeakDelegateCollection CollectedEventHandlers { get; }

            private static void RaiseCollected()
            {
                var check = Volatile.Read(ref _status);
                if (check == _statusReady)
                {
                    try
                    {
                        CollectedEventHandlers.RemoveDeadItems();
                        CollectedEventHandlers.Invoke(null, new EventArgs());
                    }
                    catch (Exception exception)
                    {
                        // Catch them all
                        GC.KeepAlive(exception);
                    }
                    Volatile.Write(ref _status, _statusReady);
                }
            }
        }
    }
}