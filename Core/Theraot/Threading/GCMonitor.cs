using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class GCMonitor
    {
        private const int INT_BoolFalse = 0;
        private const int INT_BoolTrue = 1;
        private const int INT_CapacityHint = 1024;
        private const int INT_MaxProbingHint = 32;
        private const int INT_StatusNotReady = -2;
        private const int INT_StatusPending = -1;
        private const int INT_StatusReady = 0;
        private static WeakDelegateSet _collectedEventHandlers;
        private static int _finished = INT_BoolFalse;
        private static int _status = INT_StatusNotReady;

        static GCMonitor()
        {
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += ReportApplicationDomainExit;
            currentAppDomain.DomainUnload += ReportApplicationDomainExit;
        }

        public static event EventHandler Collected
        {
            add
            {
                try
                {
                    Initialize();
                    _collectedEventHandlers.Add(value);
                }
                catch
                {
                    if (object.ReferenceEquals(value, null))
                    {
                        return;
                    }
                    throw;
                }
            }
            remove
            {
                if (Thread.VolatileRead(ref _status) == INT_StatusReady)
                {
                    try
                    {
                        _collectedEventHandlers.Remove(value);
                    }
                    catch
                    {
                        if (object.ReferenceEquals(value, null))
                        {
                            return;
                        }
                        throw;
                    }
                }
            }
        }

        public static bool FinalizingForUnload
        {
            get
            {
                return AppDomain.CurrentDomain.IsFinalizingForUnload();
            }
        }

        private static void Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, INT_StatusPending, INT_StatusNotReady);
            if (check == INT_StatusNotReady)
            {
                GC.KeepAlive(new GCProbe());
                _collectedEventHandlers = new WeakDelegateSet(INT_CapacityHint, false, false, INT_MaxProbingHint);
                Thread.VolatileWrite(ref _status, INT_StatusReady);
            }
            else
            {
                if (check != INT_StatusReady)
                {
                    ThreadingHelper.SpinWaitUntil(ref _status, INT_StatusReady);
                }
            }
        }

        private static void RaiseCollected()
        {
            if (Thread.VolatileRead(ref _finished) == INT_BoolTrue || AppDomain.CurrentDomain.IsFinalizingForUnload())
            {
                return;
            }
            else
            {
                try
                {
                    _collectedEventHandlers.RemoveDeadItems();
                    _collectedEventHandlers.Invoke(null, new EventArgs());
                }
                catch
                {
                    //Pokemon
                }
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Thread.VolatileWrite(ref _finished, INT_BoolTrue);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private sealed class GCProbe : CriticalFinalizerObject
        {
            ~GCProbe()
            {
                try
                {
                    //Empty
                }
                finally
                {
                    try
                    {
                        if (Thread.VolatileRead(ref _finished) != INT_BoolTrue && !GCMonitor.FinalizingForUnload)
                        {
                            GC.ReRegisterForFinalize(this);
                            WorkContext.DefaultContext.AddWork(GCMonitor.RaiseCollected).Start();
                        }
                    }
                    catch
                    {
                        //Pokemon
                    }
                }
            }
        }
    }
}