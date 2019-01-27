#if LESSTHAN_NET40

using System.Diagnostics;
using System.Threading.Tasks;
using Theraot;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading;

namespace System.Threading
{
    [DebuggerDisplay("Current Count = {CurrentCount}")]
    public class SemaphoreSlim : IDisposable
    {
        private readonly int? _maxCount;
        private ThreadSafeQueue<TaskCompletionSource<bool>> _asyncWaiters;
        private ManualResetEventSlim _canEnter;
        private int _count;
        private bool _disposed;
        private int _syncRoot;

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
                throw new ArgumentOutOfRangeException(nameof(initialCount), "initialCount < 0 || initialCount > maxCount");
            }

            if (maxCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount), "maxCount <= 0");
            }

            _maxCount = maxCount;
            _asyncWaiters = new ThreadSafeQueue<TaskCompletionSource<bool>>();
            _count = initialCount;
            _canEnter = new ManualResetEventSlim(_count > 0);
        }

        public WaitHandle AvailableWaitHandle
        {
            get
            {
                CheckDisposed();
                return _canEnter.WaitHandle;
            }
        }

        public int CurrentCount => Volatile.Read(ref _count);

        [DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                throw new ArgumentOutOfRangeException(nameof(releaseCount), "releaseCount is less than 1");
            }

            var spinWait = new SpinWait();
            while (true)
            {
                if (TryOffset(releaseCount, out var expected))
                {
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
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var spinWait = new SpinWait();
            if (TryOffset(-1, out _))
            {
                return true;
            }

            if (millisecondsTimeout == -1)
            {
                while (true)
                {
                    _canEnter.Wait(-1, cancellationToken);
                    // The thread is not allowed here unless there is room in the semaphore
                    if (TryOffset(-1, out _))
                    {
                        return true;
                    }

                    spinWait.SpinOnce();
                }
            }

            var start = ThreadingHelper.TicksNow();
            var remaining = millisecondsTimeout;
            while (_canEnter.Wait(remaining, cancellationToken))
            {
                // The thread is not allowed here unless there is room in the semaphore
                if (TryOffset(-1, out _))
                {
                    return true;
                }

                remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                if (remaining <= 0)
                {
                    break;
                }

                spinWait.SpinOnce();
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
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return TaskEx.FromCanceled<bool>(cancellationToken);
            }

            var source = new TaskCompletionSource<bool>();
            if (_canEnter.Wait(0, cancellationToken) && TryOffset(-1, out var dummy))
            {
                source.SetResult(true);
                return source.Task;
            }

            RootedTimeout.Launch
            (
                () =>
                {
                    try
                    {
                        source.SetResult(false);
                    }
                    catch (InvalidCastException exception)
                    {
                        // Already canceled
                        No.Op(exception);
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
                        No.Op(exception);
                    }
                }
            );
            AddWaiter(source);
            return source.Task;
        }

        protected virtual void Dispose(bool disposing)
        {
            // This is a protected method, the parameter should be kept
            No.Op(disposing);
            _disposed = true;
            _canEnter.Dispose();
            _asyncWaiters = null;
            _canEnter = null;
        }

        private void AddWaiter(TaskCompletionSource<bool> source)
        {
            _asyncWaiters.Add(source);
        }

        private void Awake(ThreadSafeQueue<TaskCompletionSource<bool>> asyncWaiters)
        {
            var spinWait = new SpinWait();
            while (asyncWaiters.TryTake(out var waiter))
            {
                if (waiter.Task.IsCompleted)
                {
                    // Skip - either canceled or timed out
                    continue;
                }

                if (TryOffset(-1, out var dummy))
                {
                    waiter.SetResult(true);
                }
                else
                {
                    // Add it back
                    _asyncWaiters.Add(waiter);
                    break;
                }

                spinWait.SpinOnce();
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(SemaphoreSlim));
            }
        }

        private void SyncWaitHandle()
        {
            var awake = false;
            if (Volatile.Read(ref _count) == 0 == _canEnter.IsSet && Interlocked.CompareExchange(ref _syncRoot, 1, 0) == 0)
            {
                try
                {
                    awake = SyncWaitHandleExtracted();
                }
                finally
                {
                    Volatile.Write(ref _syncRoot, 0);
                }
            }

            if (awake)
            {
                var asyncWaiters = _asyncWaiters;
                ThreadPool.QueueUserWorkItem(_ => Awake(asyncWaiters));
            }

            bool SyncWaitHandleExtracted()
            {
                int found;
                var canEnter = _canEnter;
                if (canEnter == null)
                {
                    return false;
                }

                if ((found = Volatile.Read(ref _count)) == 0 != canEnter.IsSet)
                {
                    return false;
                }

                if (found == 0)
                {
                    canEnter.Reset();
                }
                else
                {
                    canEnter.Set();
                    return true;
                }

                return false;
            }
        }

        private bool TryOffset(int releaseCount, out int previous)
        {
            var expected = Volatile.Read(ref _count);
            previous = expected;
            if (_maxCount.HasValue && expected + (long)releaseCount > (long)_maxCount)
            {
                throw new SemaphoreFullException();
            }

            var result = expected + releaseCount;
            if (result < 0)
            {
                return false;
            }

            var found = Interlocked.CompareExchange(ref _count, result, expected);
            if (found != expected)
            {
                return false;
            }

            SyncWaitHandle();
            return true;
        }
    }
}

#endif