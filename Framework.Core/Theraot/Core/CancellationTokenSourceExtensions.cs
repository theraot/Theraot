// Needed for Workaround

using System;
using System.Threading;
using Theraot.Threading;

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
using System.Runtime.CompilerServices;
#endif

namespace Theraot.Core
{
    public static class CancellationTokenSourceExtensions
    {
#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
                        GC.KeepAlive(exception);
                    }
                },
                millisecondsDelay
            );
        }

#if NET45 || NET46 || NET47 || NETCOREAPP1_0 || NETCOREAPP1_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2 || NETSTANDARD1_3 || NETSTANDARD1_4 || NETSTANDARD1_5 || NETSTANDARD1_6
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif

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
                        GC.KeepAlive(exception);
                    }
                },
                delay
            );
        }
    }
}