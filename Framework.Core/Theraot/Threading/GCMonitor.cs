// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
using System.Runtime.ConstrainedExecution;

#endif

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int StatusNotReady = -2;
        private const int StatusPending = -1;
        private const int StatusReady = 0;
        private static int _status = StatusNotReady;

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
        private const int _statusFinished = 1;

        static GCMonitor()
        {
            var currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += ReportApplicationDomainExit;
            currentAppDomain.DomainUnload += ReportApplicationDomainExit;
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Volatile.Write(ref _status, _statusFinished);
        }

#endif

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
                    if (value == null)
                    {
                        return;
                    }

                    throw;
                }
            }

            remove
            {
                if (Volatile.Read(ref _status) != StatusReady)
                {
                    return;
                }

                try
                {
                    Internal.CollectedEventHandlers.Remove(value);
                }
                catch
                {
                    if (value == null)
                    {
                        return;
                    }

                    throw;
                }
            }
        }

        public static bool FinalizingForUnload =>
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
                AppDomain.CurrentDomain.IsFinalizingForUnload();
#else
                false;
#endif

        private static void Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, StatusPending, StatusNotReady);
            switch (check)
            {
                case StatusNotReady:
                    GC.KeepAlive(new GCProbe());
                    Volatile.Write(ref _status, StatusReady);
                    break;

                case StatusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, StatusReady);
                    break;
            }
        }

        [DebuggerNonUserCode]
        private sealed class GCProbe
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
            : CriticalFinalizerObject
#endif
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
                        var check = Volatile.Read(ref _status);
                        if (check == StatusReady)
                        {
                            GC.ReRegisterForFinalize(this);
                            Internal.Invoke();
                        }
                    }
                    catch (Exception exception)
                    {
                        // Catch them all - there shouldn't be exceptions here, yet we really don't want them
                        No.Op(exception);
                    }
                }
            }
        }
    }
}