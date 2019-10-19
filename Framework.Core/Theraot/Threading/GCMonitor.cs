// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int _statusNotReady = -2;
        private const int _statusPending = -1;
        private const int _statusReady = 0;
        private static int _status = _statusNotReady;

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
                if (Volatile.Read(ref _status) != _statusReady)
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

        public static bool FinalizingForUnload
        {
            get
            {
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
                return AppDomain.CurrentDomain.IsFinalizingForUnload();
#else
                return false;
#endif
            }
        }

        private static void Initialize()
        {
            switch (Interlocked.CompareExchange(ref _status, _statusPending, _statusNotReady))
            {
                case _statusNotReady:
                    GC.KeepAlive(new GCProbe());
                    Volatile.Write(ref _status, _statusReady);
                    break;

                case _statusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, _statusReady);
                    break;
            }
        }
    }

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11

    public static partial class GCMonitor
    {
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
    }

#endif
}