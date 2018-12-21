// Needed for Workaround

using System;
using System.Threading;
using Theraot.Threading;

namespace Theraot.Core
{
    public static class CancellationTokenSourceExtensions
    {
        public static void CancelAfter(this CancellationTokenSource cancellationToken, int millisecondsDelay)
        {
            RootedTimeout.Launch(cancellationToken.Cancel, millisecondsDelay);
        }

        public static void CancelAfter(this CancellationTokenSource cancellationToken, TimeSpan delay)
        {
            RootedTimeout.Launch(cancellationToken.Cancel, delay);
        }
    }
}