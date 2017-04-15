// Needed for Workaround

using System;
using System.Runtime.ConstrainedExecution;
using System.Threading;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int _maxProbingHint = 128;
        private const int _statusFinished = 1;
        private const int _statusNotReady = -2;
        private const int _statusPending = -1;
        private const int _statusReady = 0;
        private static int _status = _statusNotReady;

        static GCMonitor()
        {
            var currentAppDomain = AppDomain.CurrentDomain;
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
                    if (ReferenceEquals(value, null))
                    {
                        return;
                    }
                    throw;
                }
            }
            remove
            {
                if (Thread.VolatileRead(ref _status) == _statusReady)
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
                return AppDomain.CurrentDomain.IsFinalizingForUnload();
            }
        }

        private static void Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, _statusPending, _statusNotReady);
            switch (check)
            {
                case _statusNotReady:
                    GC.KeepAlive(new GCProbe());
                    Thread.VolatileWrite(ref _status, _statusReady);
                    break;

                case _statusPending:
                    ThreadingHelper.SpinWaitUntil(ref _status, _statusReady);
                    break;
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Thread.VolatileWrite(ref _status, _statusFinished);
        }

        [System.Diagnostics.DebuggerNonUserCode]
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
                        if (check == _statusReady)
                        {
                            GC.ReRegisterForFinalize(this);
                            Internal.Invoke();
                        }
                    }
                    catch (Exception exception)
                    {
                        // Catch'em all - there shouldn't be exceptions here, yet we really don't want them
                        GC.KeepAlive(exception);
                    }
                }
            }
        }
    }
}