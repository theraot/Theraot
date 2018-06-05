#if NET20 || NET30 || NET35

using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading
{
    [Diagnostics.DebuggerDisplayAttribute("Current Count = {CurrentCount}")]
    public class SemaphoreSlim : IDisposable
    {
        private readonly int? _maxCount;
        private SafeQueue<TaskCompletionSource<bool>> _asyncWaiters;
        private int _count;
        private bool _disposed;
        private ManualResetEventSlim _event;

        public SemaphoreSlim(int initialCount)
            : this(initialCount, null)
        {
            // Empty
        }

        public SemaphoreSlim(int initialCount, int maxCount)
            : this(initialCount, (int?)maxCount)
        {
            // Empty
        }

        private SemaphoreSlim(int initialCount, int? maxCount)
        {
            if (initialCount < 0 || initialCount > maxCount)
            {
                throw new ArgumentOutOfRangeException("initialCount", "initialCount < 0 || initialCount > maxCount");
            }
            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException("initialCount", "maxCount <= 0");
            }
            _maxCount = maxCount;
            _asyncWaiters = new SafeQueue<TaskCompletionSource<bool>>();
            _count = initialCount;
            _event = new ManualResetEventSlim(_count > 0);
        }

        public WaitHandle AvailableWaitHandle
        {
            get
            {
                CheckDisposed();
                return _event.WaitHandle;
            }
        }

        public int CurrentCount
        {
            get { return Thread.VolatileRead(ref _count); }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public int Release()
        {
            return Release(1);
        }

        public int Release(int releaseCount)
        {
            CheckDisposed();
            if (releaseCount < 1)
            {
                throw new ArgumentOutOfRangeException("releaseCount", "releaseCount is less than 1");
            }
            var spinWait = new SpinWait();
            while (true)
            {
                // The value we expect to see for _count in CompareExchange
                // (The previous count of the SemaphoreSlim)
                var expected = Thread.VolatileRead(ref _count);
                // The value we want to set _count to in CompareExchange
                var result = expected + releaseCount;
                // If there is a maxCount set at constructor
                // And we are exceding it, then fail
                if (_maxCount.HasValue && result > _maxCount)
                {
                    throw new SemaphoreFullException();
                }
                // Attempt to set the new value
                var found = Interlocked.CompareExchange(ref _count, result, expected);
                // If we found what we expected, it means we succeeded
                if (found == expected)
                {
                    // Awake the corresponding threads
                    Awake(releaseCount);
                    // Return the previous count of the SemaphoreSlim.
                    return expected;
                }
                spinWait.SpinOnce();
            }
        }

        public void Wait()
        {
            Wait(Timeout.Infinite, CancellationToken.None);
        }

        public bool Wait(TimeSpan timeout)
        {
            CheckDisposed();
            return Wait((int)timeout.TotalMilliseconds, CancellationToken.None);
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Wait(millisecondsTimeout, CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            Wait(Timeout.Infinite, cancellationToken);
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            return Wait((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (millisecondsTimeout == -1)
            {
                var spinWait = new SpinWait();
                while (!TryEnter())
                {
                    spinWait.SpinOnce();
                }
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            var remaining = millisecondsTimeout;
            while (_event.Wait(remaining, cancellationToken))
            {
                // The thread is not allowed here unless there is room in the semaphore
                if (TryEnter())
                {
                    return true;
                }
                remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                if (remaining <= 0)
                {
                    break;
                }
            }
            // Time out
            return false;
        }

        public Task WaitAsync()
        {
            return WaitAsync(Timeout.Infinite, CancellationToken.None);
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            return WaitAsync(Timeout.Infinite, cancellationToken);
        }

        public Task<bool> WaitAsync(int millisecondsTimeout)
        {
            return WaitAsync(millisecondsTimeout, CancellationToken.None);
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            CheckDisposed();
            return WaitAsync((int)timeout.TotalMilliseconds, CancellationToken.None);
        }

        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            return WaitAsync((int)timeout.TotalMilliseconds, cancellationToken);
        }

        public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (cancellationToken.IsCancellationRequested)
            {
                return Task<bool>.FromCancellation(cancellationToken);
            }
            var source = new TaskCompletionSource<bool>();
            Thread.MemoryBarrier();
            if (Wait(0, cancellationToken))
            {
                source.SetResult(true);
                return source.Task;
            }
            Theraot.Threading.Timeout.Launch
            (
                () =>
                {
                    try
                    {
                        source.SetResult(false);
                    }
                    catch (InvalidCastException exception)
                    {
                        // Already cancelled
                        GC.KeepAlive(exception);
                    }
                },
                millisecondsTimeout,
                cancellationToken
            );
            cancellationToken.Register
            (
                () =>
                {
                    try
                    {
                        source.SetCanceled();
                    }
                    catch (InvalidOperationException exception)
                    {
                        // Already timeout
                        GC.KeepAlive(exception);
                    }
                }
            );
            AddWaiter(source);
            return source.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            // This is a protected method, the parameter should be kept
            _disposed = true;
            _event.Dispose();
            _asyncWaiters = null;
            _event = null;
        }

        private void AddWaiter(TaskCompletionSource<bool> source)
        {
            _asyncWaiters.Add(source);
        }

        private void Awake(int releaseCount)
        {
            // Call this to notify that there is room in the semaphore
            // Allow sync waiters to proceed
            _event.Set();
            TaskCompletionSource<bool> waiter;
            while (releaseCount > 0 && _asyncWaiters.TryTake(out waiter))
            {
                releaseCount--;
                if (waiter.Task.IsCompleted)
                {
                    // Skip - either canceled or timed out
                    continue;
                }
                if (TryEnter())
                {
                    waiter.SetResult(true);
                }
                else
                {
                    // Add it back
                    _asyncWaiters.Add(waiter);
                }
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private bool TryEnter()
        {
            // Should only be called when there is room in the semaphore
            // No check is done to verify that
            var expected = Thread.VolatileRead(ref _count);
            var result = expected - 1;
            if (result < 0)
            {
                return false;
            }
            var found = Interlocked.CompareExchange(ref _count, result, expected);
            if (found == expected)
            {
                // It may be the case that there is no longer room in the semaphore because we just took one slot
                if (Thread.VolatileRead(ref _count) == 0)
                {
                    // Cause sync waitets to halt
                    _event.Reset();
                    // It is possible that another thread has just released more slots and called _event.Set() and we have just undone it...
                    // Check if that is the case
                    if (Thread.VolatileRead(ref _count) > 0)
                    {
                        // Allow sync waiters to proceed
                        _event.Set();
                    }
                }
                return true;
            }
            return false;
        }
    }
}

#endif