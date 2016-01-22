#if NET20 || NET30 || NET35

using System.Threading.Tasks;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading
{
    [Diagnostics.DebuggerDisplayAttribute("Current Count = {CurrentCount}")]
    public class SemaphoreSlim : IDisposable
    {
        private SafeQueue<TaskCompletionSource<bool>> _asyncWaiters;
        private ManualResetEventSlim _event;
        private readonly int _maxCount;
        private int _count;
        private bool _disposed;

        public SemaphoreSlim(int initialCount)
            : this(initialCount, int.MaxValue)
        {
            //Empty
        }

        public SemaphoreSlim(int initialCount, int maxCount)
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
            _count = maxCount - initialCount;
            _event = new ManualResetEventSlim(_count < maxCount);
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
            get
            {
                return _maxCount - Thread.VolatileRead(ref _count);
            }
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
            int oldValue;
            if (ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _count, -releaseCount, out oldValue))
            {
                Awake();
                return oldValue;
            }
            throw new SemaphoreFullException();
        }

        private void Awake()
        {
            // Call this to notify that there is room in the semaphore
            // Allow sync waiters to proceed
            _event.Set();
            while (Thread.VolatileRead(ref _count) < _maxCount)
            {
                TaskCompletionSource<bool> waiter;
                if (_asyncWaiters.TryTake(out waiter))
                {
                    if (waiter.Task.IsCompleted)
                    {
                        // Skip - either cancelled or timed out
                        continue;
                    }
                    if (TryEnter())
                    {
                        waiter.SetResult(true);
                    }
                    else
                    {
                        _asyncWaiters.Add(waiter);
                    }
                }
            }
        }

        public void Wait()
        {
            Wait(CancellationToken.None);
        }

        public bool Wait(TimeSpan timeout)
        {
            return Wait((int)timeout.TotalMilliseconds, CancellationToken.None);
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Wait(millisecondsTimeout, CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            Wait(-1, cancellationToken);
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
                ThreadingHelper.SpinWaitRelativeSet(ref _count, 1);
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            var remaining = millisecondsTimeout;
            while(_event.Wait(remaining, cancellationToken))
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

        private bool TryEnter()
        {
            // Should only be called when there is room in the semaphore
            // No check is done to verify that
            var result = Thread.VolatileRead(ref _count) + 1;
            if (Interlocked.CompareExchange(ref _count, result, _count) == _count)
            {
                // It may be the case that there is no longer room in the semaphore because we just took one slot
                if (Thread.VolatileRead(ref _count) == _maxCount)
                {
                    // Cause sync waitets to halt
                    _event.Reset();
                    // It is possible that another thread has just released more slots and called _event.Set() and we have just undone it...
                    // Check if that is the case
                    if (Thread.VolatileRead(ref _count) < _maxCount)
                    {
                        // Allow sync waiters to proceed
                        _event.Set();
                    }
                }
                return true;
            }
            return false;
        }

        public Task WaitAsync()
        {
            CheckDisposed();
            var source = new TaskCompletionSource<bool>();
            _asyncWaiters.Add(source);
            return source.Task;
        }

        public Task WaitAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCancellation(cancellationToken);
            }
            var source = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => source.SetCanceled());
            _asyncWaiters.Add(source);
            return source.Task;
        }

        public Task<bool> WaitAsync(int millisecondsTimeout)
        {
            CheckDisposed();
            var source = new TaskCompletionSource<bool>();
            GC.KeepAlive
            (
                new Theraot.Threading.Timeout(() => source.SetResult(false), millisecondsTimeout)
                {
                    Rooted = true
                }
            );
            _asyncWaiters.Add(source);
            return source.Task;
        }

        public Task<bool> WaitAsync(TimeSpan timeout)
        {
            CheckDisposed();
            var source = new TaskCompletionSource<bool>();
            GC.KeepAlive
            (
                new Theraot.Threading.Timeout(() => source.SetResult(false), timeout)
                {
                    Rooted = true
                }
            );
            _asyncWaiters.Add(source);
            return source.Task;
        }

        public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task<bool>.FromCancellation(cancellationToken);
            }
            CheckDisposed();
            var source = new TaskCompletionSource<bool>();
            GC.KeepAlive
            (
                new Theraot.Threading.Timeout(() => source.SetResult(false), millisecondsTimeout, cancellationToken)
                {
                    Rooted = true
                }
            );
            cancellationToken.Register(() => source.SetCanceled());
            _asyncWaiters.Add(source);
            return source.Task;
        }

        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task<bool>.FromCancellation(cancellationToken);
            }
            CheckDisposed();
            var source = new TaskCompletionSource<bool>();
            GC.KeepAlive
            (
                new Theraot.Threading.Timeout
                (
                    () =>
                    {
                        try
                        {
                            source.SetResult(false);
                        }
                        catch (InvalidOperationException exception)
                        {
                            // Already cancelled
                            GC.KeepAlive(exception);
                        }
                    },
                    timeout,
                    cancellationToken
                )
                {
                    Rooted = true
                }
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
            _asyncWaiters.Add(source);
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

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}

#endif