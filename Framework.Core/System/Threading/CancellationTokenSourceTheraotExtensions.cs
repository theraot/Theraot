#if LESSTHAN_NET45
// Needed for Workaround

using System.Runtime.CompilerServices;
using Theraot;
using Theraot.Threading;

namespace System.Threading
{
    public static class CancellationTokenSourceTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, int millisecondsDelay)
        {
            GC.KeepAlive(cancellationTokenSource.Token);
            RootedTimeout.Launch
            (
                () =>
                {
                    try
                    {
                        cancellationTokenSource.Cancel();
                    }
                    catch (ObjectDisposedException exception)
                    {
                        No.Op(exception);
                    }
                },
                millisecondsDelay
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan delay)
        {
            GC.KeepAlive(cancellationTokenSource.Token);
            RootedTimeout.Launch
            (
                () =>
                {
                    try
                    {
                        cancellationTokenSource.Cancel();
                    }
                    catch (ObjectDisposedException exception)
                    {
                        No.Op(exception);
                    }
                },
                delay
            );
        }
    }
}
#endif