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
            }
            else
            {
                var waitHandle = semaphore.AvailableWaitHandle;
                var registration = new RegisteredWaitHandle[1];
                if (millisecondsTimeout == -1)
                {
                    registration[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, CallbackWithoutTimeout, null, -1, true);
                    void CallbackWithoutTimeout(object state, bool timeOut)
                    {
                        Unregister();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            source.TrySetCanceled();
                            return;
                        }
                        if (semaphore.Wait(0))
                        {
                            source.TrySetResult(true);
                            return;
                        }
                        var newRegistration = ThreadPool.RegisterWaitForSingleObject(waitHandle, CallbackWithoutTimeout, null, -1, true);
                        Volatile.Write(ref registration[0], newRegistration);
                    }
                }
                else
                {
                    var start = ThreadingHelper.TicksNow();
                    registration[0] = ThreadPool.RegisterWaitForSingleObject(waitHandle, CallbackWithTimeout, null, millisecondsTimeout, true);
                    void CallbackWithTimeout(object state, bool timeOut)
                    {
                        Unregister();
                        if (cancellationToken.IsCancellationRequested)
                        {
                            source.TrySetCanceled();
                            return;
                        }
                        long timeout;
                        if (timeOut || (timeout = millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start)) <= 0)
                        {
                            source.TrySetResult(false);
                            return;
                        }
                        if (semaphore.Wait(0))
                        {
                            source.TrySetResult(true);
                            return;
                        }
                        var newRegistration = ThreadPool.RegisterWaitForSingleObject(waitHandle, CallbackWithTimeout, null, timeout, true);
                        Volatile.Write(ref registration[0], newRegistration);
                    }
                }
                cancellationToken.Register(Unregister);
                void Unregister()
                {
                    Volatile.Read(ref registration[0]).Unregister(null);
                }
            }
            return source.Task;
        }
    }
}

#endif