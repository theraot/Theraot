// Needed for Workaround

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Core
{
    public static class CancellationTokenSourceExtensions
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