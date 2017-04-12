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
            private static readonly WeakDelegateCollection _collectedEventHandlers;
            private static readonly WaitCallback _work;

            static Internal()
            {
                _work = _ => RaiseCollected();
                _collectedEventHandlers = new WeakDelegateCollection(false, false, INT_MaxProbingHint);
            }

            public static WeakDelegateCollection CollectedEventHandlers
            {
                get { return _collectedEventHandlers; }
            }

            public static void Invoke()
            {
                ThreadPool.QueueUserWorkItem(_work);
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
                        // Catch'em all
                        GC.KeepAlive(exception);
                    }
                    Thread.VolatileWrite(ref _status, INT_StatusReady);
                }
            }
        }
    }
}