// Needed for Workaround

using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int INT_CapacityHint = 1024;
        private const int INT_MaxProbingHint = 128;
        private const int INT_StatusFinished = 1;
        private const int INT_StatusNotReady = -2;
        private const int INT_StatusPending = -1;
        private const int INT_StatusReady = 0;
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
                    Internal.CollectedEventHandlers.Add(value);
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
                        Internal.CollectedEventHandlers.Remove(value);
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
                    GC.KeepAlive(new GCProbe());
                    Thread.VolatileWrite(ref _status, INT_StatusReady);
                    break;

                case INT_StatusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, INT_StatusReady);
                    break;
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
                    // Empty
                }
                finally
                {
                    try
                    {
                        var check = Thread.VolatileRead(ref _status);
                        if (check == INT_StatusReady)
                        {
                            GC.ReRegisterForFinalize(this);
                            Internal.Invoke();
                        }
                    }
                    catch (Exception exception)
                    {
                        // Pokemon - there shouldn't be exceptions here, yet we really don't want them
                        GC.KeepAlive(exception);
                    }
                }
            }
        }
    }
}