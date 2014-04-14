using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static class GCMonitor
    {
        private const int INT_CapacityHint = 1024;
        private const int INT_MaxProbingHint = 128;
        private const int INT_StatusFinished = 1;
        private const int INT_StatusNotReady = -2;
        private const int INT_StatusPending = -1;
        private const int INT_StatusReady = 0;
        private static WeakDelegateSet _collectedEventHandlers;
        private static int _status = INT_StatusNotReady;
        private static Work _work;

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
            switch (check)
            {
                case INT_StatusNotReady:
                    _work = WorkContext.DefaultContext.AddWork(GCMonitor.RaiseCollected);
                    GC.KeepAlive(new GCProbe());
                    _collectedEventHandlers = new WeakDelegateSet(INT_CapacityHint, false, false, INT_MaxProbingHint);
                    Thread.VolatileWrite(ref _status, INT_StatusReady);
                    break;

                case INT_StatusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, INT_StatusReady);
                    break;

                default:
                    break;
            }
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
                catch
                {
                    //Pokemon
                }
                Thread.VolatileWrite(ref _status, INT_StatusReady);
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Thread.VolatileWrite(ref _status, INT_StatusFinished);
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
                        var check = Thread.VolatileRead(ref _status);
                        if (check == INT_StatusReady)
                        {
                            GC.ReRegisterForFinalize(this);
                            _work.Start();
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