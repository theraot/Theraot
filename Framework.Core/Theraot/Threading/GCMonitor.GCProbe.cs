using System;
using System.Diagnostics;
using System.Threading;

namespace Theraot.Threading
{
    public static partial class GCMonitor
    {
        [DebuggerNonUserCode]
        private sealed class GCProbe
#if TARGETS_NET || GREATERTHAN_NETCOREAPP11
            : System.Runtime.ConstrainedExecution.CriticalFinalizerObject
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