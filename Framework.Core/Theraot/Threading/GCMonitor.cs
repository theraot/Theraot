// Needed for Workaround

using System;
using System.Diagnostics;
using System.Threading;

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

using System.Runtime.ConstrainedExecution;

#endif

namespace Theraot.Threading
{
    [DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        // ReSharper disable once UnusedMember.Local
        private const int _statusFinished = 1;

        private const int _statusNotReady = -2;
        private const int _statusPending = -1;
        private const int _statusReady = 0;
        private static int _status = _statusNotReady;

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

        static GCMonitor()
        {
            var currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += ReportApplicationDomainExit;
            currentAppDomain.DomainUnload += ReportApplicationDomainExit;
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
                    if (ReferenceEquals(value, null))
                    {
                        return;
                    }
                    throw;
                }
            }
            remove
            {
                if (Volatile.Read(ref _status) == _statusReady)
                {
                    try
                    {
                        Internal.CollectedEventHandlers.Remove(value);
                    }
                    catch
                    {
                        if (ReferenceEquals(value, null))
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
                // If you need to get rid of this, just set this property to return false
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6
                return AppDomain.CurrentDomain.IsFinalizingForUnload();
#else
                return false;
#endif
            }
        }

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

#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5 && !NETSTANDARD1_6

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Volatile.Write(ref _status, _statusFinished);
        }

#endif

        [DebuggerNonUserCode]
        private sealed class GCProbe
#if !NETCOREAPP1_0 && !NETCOREAPP1_1 && !NETSTANDARD1_0 && !NETSTANDARD1_1 && !NETSTANDARD1_2 && !NETSTANDARD1_3 && !NETSTANDARD1_4 && !NETSTANDARD1_5  && !NETSTANDARD1_6
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
                        GC.KeepAlive(exception);
                    }
                }
            }
        }
    }
}