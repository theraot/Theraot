#if NET40

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Theraot.Core
{
    public static class SemaphoreSlimExtensions
    {
        public static Task WaitAsync(this SemaphoreSlim semaphore)
        {
            return WaitAsync(semaphore, Timeout.Infinite, CancellationToken.None);
        }

        public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            return WaitAsync(semaphore, Timeout.Infinite, cancellationToken);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout)
        {
            return WaitAsync(semaphore, millisecondsTimeout, CancellationToken.None);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore, (int)timeout.TotalMilliseconds, CancellationToken.None);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore, (int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore, millisecondsTimeout, cancellationToken);
        }

        private static Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return TaskEx.FromCanceled<bool>(cancellationToken);
            }
            var source = new TaskCompletionSource<bool>();
            if (semaphore.Wait(0))
            {
                source.SetResult(true);
                return source.Task;
            }
            var waiterThread = new Thread
            (
                () =>
                {
                    try
                    {
                        var result = semaphore.Wait(millisecondsTimeout, cancellationToken);
                        source.TrySetResult(result);
                    }
                    catch (OperationCanceledException exception)
                    {
                        GC.KeepAlive(exception);
                        source.TrySetCanceled();
                    }
                }
            );
            waiterThread.Start();
            return source.Task;
        }
    }
}

#endif