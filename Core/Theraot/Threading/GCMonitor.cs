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
        private static AutoResetEvent collectedEvent;
        private static WeakDelegateSet collectedEventHandlers;
        private static volatile bool finished;

        static GCMonitor()
        {
            collectedEvent = new AutoResetEvent(false);
            collectedEventHandlers = new WeakDelegateSet(false, false);
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            currentAppDomain.ProcessExit += new EventHandler(ReportApplicationDomainExit);
            currentAppDomain.DomainUnload += new EventHandler(ReportApplicationDomainExit);
            new GCProbe();
            (
                new Thread(ExecuteCollected)
                {
                    Name = string.Format(CultureInfo.InvariantCulture, "{0} runner.", typeof(GCMonitor).Name)
                }
            ).Start();
        }

        public static event EventHandler Collected
        {
            add
            {
                try
                {
                    collectedEventHandlers.Add(value);
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
                try
                {
                    collectedEventHandlers.Remove(value);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Pokemon")]
        private static void ExecuteCollected()
        {
            Thread thread = Thread.CurrentThread;
            while (true)
            {
                thread.IsBackground = true;
                WaitOne();
                if (finished || AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    return;
                }
                thread.IsBackground = false;
                try
                {
                    collectedEventHandlers.RemoveDeadItems();
                    collectedEventHandlers.Invoke(null, new EventArgs());
                }
                catch (Exception)
                {
                    //Pokemon
                }
            }
        }

        private static void ReportApplicationDomainExit(object sender, EventArgs e)
        {
            finished = true;
            collectedEvent.Set();
        }

        private static void WaitOne()
        {
            collectedEvent.WaitOne();
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        private sealed class GCProbe : CriticalFinalizerObject
        {
            ~GCProbe()
            {
                if (finished || AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    return;
                }
                GC.ReRegisterForFinalize(this);
                collectedEvent.Set();
            }
        }
    }
}

#endif