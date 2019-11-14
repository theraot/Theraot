#if NET40

#pragma warning disable CA2201 // Do not raise reserved exception types

using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Theraot.Threading;

namespace System.Threading
{
    public static class SemaphoreSlimTheraotExtensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WaitAsync(this SemaphoreSlim semaphore)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitPrivateAsync(semaphore);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task WaitAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitPrivateAsync(semaphore, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }
            return millisecondsTimeout == -1 ? WaitPrivateAsync(semaphore) : WaitPrivateAsync(semaphore, millisecondsTimeout);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            var millisecondsTimeout = (int)timeout.TotalMilliseconds;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            return millisecondsTimeout == -1 ? WaitPrivateAsync(semaphore) : WaitPrivateAsync(semaphore, millisecondsTimeout);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            var millisecondsTimeout = (int)timeout.TotalMilliseconds;
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            return millisecondsTimeout == -1 ? WaitPrivateAsync(semaphore, cancellationToken) : WaitPrivateAsync(semaphore, (int)timeout.TotalMilliseconds, cancellationToken);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (semaphore == null)
            {
                throw new NullReferenceException();
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }
            return millisecondsTimeout == -1 ? WaitPrivateAsync(semaphore, cancellationToken) : WaitPrivateAsync(semaphore, millisecondsTimeout, cancellationToken);
        }

        private static async Task<bool> WaitPrivateAsync(SemaphoreSlim semaphore)
        {
            if (semaphore.Wait(0))
            {
                return true;
            }
            while (true)
            {
                await TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle).ConfigureAwait(false);
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
                await TaskExEx.FromWaitHandleInternal
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
                await TaskExEx.FromWaitHandleInternal(semaphore.AvailableWaitHandle, cancellationToken).ConfigureAwait(false);
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
                && await TaskExEx.FromWaitHandleInternal
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