// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;

#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

using System.Runtime.ConstrainedExecution;

#endif

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int _statusNotReady = -2;
        private const int _statusPending = -1;
        private const int _statusReady = 0;
        private static int _status = _statusNotReady;

#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

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

        public static bool FinalizingForUnload =>
                // If you need to get rid of this, just set this property to return false
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
                AppDomain.CurrentDomain.IsFinalizingForUnload();
#else
                false;
#endif

        private static void Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, _statusPending, _statusNotReady);
            switch (check)
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

        [DebuggerNonUserCode]
        private sealed class GCProbe
#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2
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
                        if (check == _statusReady)
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