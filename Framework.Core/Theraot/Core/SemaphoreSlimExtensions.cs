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
            return WaitPrivateAsync(semaphore);
        }

        public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitPrivateAsync(semaphore, cancellationToken);
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
                return WaitPrivateAsync(semaphore);
            }
            return WaitPrivateAsync(semaphore, millisecondsTimeout);
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
                return WaitPrivateAsync(semaphore);
            }
            return WaitPrivateAsync(semaphore, millisecondsTimeout);
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
                return WaitPrivateAsync(semaphore, cancellationToken);
            }
            return WaitPrivateAsync(semaphore, (int)timeout.TotalMilliseconds, cancellationToken);
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
                return WaitPrivateAsync(semaphore, cancellationToken);
            }
            return WaitPrivateAsync(semaphore, millisecondsTimeout, cancellationToken);
        }

        private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            while (true)
            {
                await TaskEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle).ConfigureAwait(false);
                if (semaphore.Wait(0))
                {
                    return true;
                }
            }
        }

        private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, int millisecondsTimeout)
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
                ).ConfigureAwait(false)
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

        private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (semaphore.Wait(0))
            {
                return true;
            }
            while (true)
            {
                await TaskEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, cancellationToken).ConfigureAwait(false);
                if (semaphore.Wait(0))
                {
                    return true;
                }
            }
        }

        private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            var timeout = millisecondsTimeout;
            while
            (
                timeout > 0
                && await TaskEx.FromWaitHandleInternal
                (
                    semaphore.AvailableWaitHandle,
                    timeout,
                    cancellationToken
                ).ConfigureAwait(false)
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