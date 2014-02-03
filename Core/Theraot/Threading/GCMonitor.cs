using System;
using System.Globalization;
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
        private static AutoResetEvent _collectedEvent;
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
                    var startRunner = Initialize();
                    _collectedEventHandlers.Add(value);
                    if (startRunner)
                    {
                        StartRunner();
                    }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Pokemon")]
        private static void ExecuteCollected()
        {
            Thread thread = Thread.CurrentThread;
            while (true)
            {
                thread.IsBackground = true;
                WaitOne();
                if (Thread.VolatileRead(ref _finished) == INT_BoolTrue || AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    return;
                }
                else
                {
                    thread.IsBackground = false;
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
        }

        private static bool Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, INT_StatusPending, INT_StatusNotReady);
            if (check == INT_StatusNotReady)
            {
                _collectedEvent = new AutoResetEvent(false);
                _collectedEventHandlers = new WeakDelegateSet(INT_CapacityHint, false, false, INT_MaxProbingHint);
                Thread.VolatileWrite(ref _status, INT_StatusReady);
                return true;
            }
            else
            {
                if (check == INT_StatusReady)
                {
                    return false;
                }
                else
                {
                    ThreadingHelper.SpinWaitUntil(ref _status, INT_StatusReady);
                    return false;
                }
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Thread.VolatileWrite(ref _finished, INT_BoolTrue);
            if (Thread.VolatileRead(ref _status) == INT_StatusReady)
            {
                _collectedEvent.Set();
            }
        }

        private static void StartRunner()
        {
            GC.KeepAlive(new GCProbe());
            Thread.MemoryBarrier();
            var runnerThread = new Thread(ExecuteCollected)
            {
                Name = string.Format(CultureInfo.InvariantCulture, "{0} runner.", typeof(GCMonitor).Name)
            };
            runnerThread.Start();
        }

        private static void WaitOne()
        {
            _collectedEvent.WaitOne();
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
                            var collectedEvent = _collectedEvent;
                            if (!ReferenceEquals(collectedEvent, null))
                            {
                                collectedEvent.Set();
                            }
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