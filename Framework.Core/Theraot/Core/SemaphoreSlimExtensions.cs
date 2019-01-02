#if NET40

using System;
using System.Threading;
using System.Threading.Tasks;
using Theraot.Threading;

namespace Theraot.Core
{
    public static class SemaphoreSlimExtensions
    {
        public static Task WaitAsync(this SemaphoreSlim semaphore)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore);
        }

        public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore, cancellationToken);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }
            if (millisecondsTimeout == -1)
            {
                return WaitAsyncPrivate(semaphore);
            }
            return WaitAsyncPrivate(semaphore, millisecondsTimeout);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            var millisecondsTimeout = (int)timeout.TotalMilliseconds;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            if (millisecondsTimeout == -1)
            {
                return WaitAsyncPrivate(semaphore);
            }
            return WaitAsyncPrivate(semaphore, millisecondsTimeout);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            var millisecondsTimeout = (int)timeout.TotalMilliseconds;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            if (millisecondsTimeout == -1)
            {
                return WaitAsyncPrivate(semaphore, cancellationToken);
            }
            return WaitAsyncPrivate(semaphore, (int)timeout.TotalMilliseconds, cancellationToken);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }
            if (millisecondsTimeout == -1)
            {
                return WaitAsyncPrivate(semaphore, cancellationToken);
            }
            return WaitAsyncPrivate(semaphore, millisecondsTimeout, cancellationToken);
        }

        private static async Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            while (true)
            {
                await TaskEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle);
                if (semaphore.Wait(0))
                {
                    return true;
                }
            }
        }

        private static async Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, int millisecondsTimeout)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            var timeout = millisecondsTimeout;
            while
            (
                await TaskEx.FromWaitHandleInternal
                (
                    semaphore.AvailableWaitHandle,
                    timeout
                )
            )
            {
                if (semaphore.Wait(0))
                {
                    return true;
                }
                timeout = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
            }
            return false;
        }

        private static async Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (semaphore.Wait(0))
            {
                return true;
            }
            while (true)
            {
                await TaskEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, cancellationToken);
                if (semaphore.Wait(0))
                {
                    return true;
                }
            }
        }

        private static async Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            var timeout = millisecondsTimeout;
            while
            (
                timeout > 0 &&
                await TaskEx.FromWaitHandleInternal
                (
                    semaphore.AvailableWaitHandle,
                    timeout,
                    cancellationToken
                )
            )
            {
                if (semaphore.Wait(0))
                {
                    return true;
                }
                timeout = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
            }
            return false;
        }
    }
}

#endif