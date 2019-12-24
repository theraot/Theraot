#if LESSTHAN_NET45
// Needed for Workaround

#pragma warning disable CA2201 // Do not raise reserved exception types
#pragma warning disable S112 // General exceptions should never be thrown

using System.Runtime.CompilerServices;
using Theraot.Threading;

namespace System.Threading
{
    public static class CancellationTokenSourceTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, int millisecondsDelay)
        {
            if (cancellationTokenSource == null)
            {
                throw new NullReferenceException();
            }
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
                        _ = exception;
                    }
                },
                millisecondsDelay
            );
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CancelAfter(this CancellationTokenSource cancellationTokenSource, TimeSpan delay)
        {
            if (cancellationTokenSource == null)
            {
                throw new NullReferenceException();
            }
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
                        _ = exception;
                    }
                },
                delay
            );
        }
    }
}

#endif