#if FAT

using System;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class GCMonitor
    {
        private const int IntCapacityHint = 1024;
        private const int IntMaxProbingHint = 32;
        private const int IntStatusNotReady = -2;
        private const int IntStatusPending = -1;
        private const int IntStatusReady = 0;
        private static AutoResetEvent _collectedEvent;
        private static WeakDelegateSet _collectedEventHandlers;
        private static int _finished = 0;
        private static int _status = IntStatusNotReady;

        static GCMonitor()
        {
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += new EventHandler(ReportApplicationDomainExit);
            currentAppDomain.DomainUnload += new EventHandler(ReportApplicationDomainExit);
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
                if (Thread.VolatileRead(ref _status) == IntStatusReady)
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Pokemon")]
        private static void ExecuteCollected()
        {
            Thread thread = Thread.CurrentThread;
            while (true)
            {
                thread.IsBackground = true;
                WaitOne();
                if (Thread.VolatileRead(ref _finished) == 1 || AppDomain.CurrentDomain.IsFinalizingForUnload())
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
                    catch (Exception)
                    {
                        //Pokemon
                    }
                }
            }
        }

        private static bool Initialize()
        {
            var check = Interlocked.CompareExchange(ref _status, IntStatusPending, IntStatusNotReady);
            if (check == -2)
            {
                _collectedEvent = new AutoResetEvent(false);
                _collectedEventHandlers = new WeakDelegateSet(IntCapacityHint, false, false, IntMaxProbingHint);
                Thread.VolatileWrite(ref _status, IntStatusReady);
                return true;
            }
            else if (check == 0)
            {
                return false;
            }
            else
            {
                ThreadingHelper.SpinWaitUntil(ref _status, IntStatusReady);
                return false;
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            Thread.VolatileWrite(ref _finished, 1);
            if (Thread.VolatileRead(ref _status) == IntStatusReady)
            {
                _collectedEvent.Set();
            }
        }

        private static void StartRunner()
        {
            new GCProbe();
            (
                new Thread(ExecuteCollected)
                {
                    Name = string.Format(CultureInfo.InvariantCulture, "{0} runner.", typeof(GCMonitor).Name)
                }
            ).Start();
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
                if (Thread.VolatileRead(ref _finished) == 1 || AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    return;
                }
                else
                {
                    GC.ReRegisterForFinalize(this);
                    _collectedEvent.Set();
                }
            }
        }
    }
}

#endif