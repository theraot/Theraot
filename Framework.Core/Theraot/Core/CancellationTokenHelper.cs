using System;
using System.Threading;

namespace Theraot.Core
{
    public static class CancellationTokenHelper
    {
        public static void ThrowIfSourceDisposed(this CancellationToken cancellationToken)
        {
            //CancellationToken.WaitHandle is documented to throw ObjectDisposedException if the source of the CancellationToken is disposed.
            GC.KeepAlive(cancellationToken.WaitHandle);
        }
    }
}