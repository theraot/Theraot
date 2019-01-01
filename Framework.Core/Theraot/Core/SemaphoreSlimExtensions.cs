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
            return WaitAsyncPrivate(semaphore, millisecondsTimeout);
        }

        public static Task<bool> WaitAsync(this SemaphoreSlim semaphore, TimeSpan timeout)
        {
            if (semaphore == null)
            {
                throw new ArgumentNullException(nameof(semaphore));
            }
            GC.KeepAlive(semaphore.AvailableWaitHandle);
            return WaitAsyncPrivate(semaphore, (int)timeout.TotalMilliseconds);
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

        private static Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore)
        {
            var source = new TaskCompletionSource<bool>();
            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
            return source.Task;
            void ContinuationFunction(Task<bool> task)
            {
                if (task.Result)
                {
                    if (semaphore.Wait(0))
                    {
                        source.TrySetResult(true);
                    }
                    else
                    {
                        TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
                    }
                }
                else
                {
                    source.TrySetResult(false);
                }
            }
        }

        private static Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, int millisecondsTimeout)
        {
            var source = new TaskCompletionSource<bool>();
            var start = ThreadingHelper.TicksNow();
            var remaining = millisecondsTimeout - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, remaining).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
            return source.Task;
            void ContinuationFunction(Task<bool> task)
            {
                if (task.Result)
                {
                    if (semaphore.Wait(0))
                    {
                        source.TrySetResult(true);
                    }
                    else
                    {
                        remaining = millisecondsTimeout - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                        TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, remaining).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
                    }
                }
                else
                {
                    source.TrySetResult(false);
                }
            }
        }

        private static Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, CancellationToken cancellationToken)
        {
            var source = new TaskCompletionSource<bool>();
            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, cancellationToken).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
            return source.Task;
            void ContinuationFunction(Task<bool> task)
            {
                if (task.IsCanceled)
                {
                    source.TrySetCanceled();
                }
                else
                {
                    if (task.Result)
                    {
                        if (semaphore.Wait(0))
                        {
                            source.TrySetResult(true);
                        }
                        else
                        {
                            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, cancellationToken).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
                        }
                    }
                    else
                    {
                        source.TrySetResult(false);
                    }
                }
            }
        }

        private static Task<bool> WaitAsyncPrivate(SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var source = new TaskCompletionSource<bool>();
            var start = ThreadingHelper.TicksNow();
            var remaining = millisecondsTimeout - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, remaining, cancellationToken).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
            return source.Task;
            void ContinuationFunction(Task<bool> task)
            {
                if (task.IsCanceled)
                {
                    source.TrySetCanceled();
                }
                else
                {
                    if (task.Result)
                    {
                        if (semaphore.Wait(0))
                        {
                            source.TrySetResult(true);
                        }
                        else
                        {
                            remaining = millisecondsTimeout - (int)ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                            TaskEx.FromWaitHandle(semaphore.AvailableWaitHandle, remaining, cancellationToken).ContinueWith(ContinuationFunction, TaskContinuationOptions.None);
                        }
                    }
                    else
                    {
                        source.TrySetResult(false);
                    }
                }
            }
        }
    }
}

#endif