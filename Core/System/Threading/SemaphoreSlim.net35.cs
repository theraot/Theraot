#if NET20 || NET30 || NET35

using Theraot.Threading;

namespace System.Threading
{
    [System.Diagnostics.DebuggerDisplayAttribute("Current Count = {CurrentCount}")]
    public class SemaphoreSlim : IDisposable
    {
        private int _count;
        private bool _disposed;
        private ManualResetEventSlim _event;
        private readonly int _maxCount;

        public SemaphoreSlim(int initialCount)
            : this(initialCount, int.MaxValue)
        {
            //Empty
        }

        public SemaphoreSlim(int initialCount, int maxCount)
        {
            if (initialCount < 0 || initialCount > maxCount || maxCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount < 0 || initialCount > maxCount || maxCount < 0");
            }
            else
            {
                _maxCount = maxCount;
                _count = maxCount - initialCount;
                _event = new ManualResetEventSlim(initialCount > 0);
            }
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
            else
            {
                int oldValue;
                if (ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _count, -releaseCount, out oldValue))
                {
                    _event.Set();
                    return oldValue;
                }
                else
                {
                    throw new SemaphoreFullException();
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
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
                if (millisecondsTimeout == -1)
                {
                    ThreadingHelper.SpinWaitRelativeSet(ref _count, 1);
                    return true;
                }
                else
                {
                    var start = ThreadingHelper.TicksNow();
                    var remaining = millisecondsTimeout;
                retry:
                    if (_event.Wait(remaining, cancellationToken))
                    {
                        var result = Thread.VolatileRead(ref _count) + 1;
                        if (Interlocked.CompareExchange(ref _count, result, _count) == _count)
                        {
                            return true;
                        }
                        else
                        {
                            remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                            if (remaining > 0)
                            {
                                goto retry;
                            }
                        }
                    }
                    return false;
                }
            }
        }

        //public Task WaitAsync()
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        //public Task WaitAsync(CancellationToken cancellationToken)
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        //public Task<bool> WaitAsync(int millisecondsTimeout)
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        //public Task<bool> WaitAsync(TimeSpan timeout)
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        //public Task<bool> WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        //public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken)
        //{
        //    //TODO Task
        //    throw new NotImplementedException();
        //}

        protected virtual void Dispose(bool disposing)
        {
            _disposed = true;
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
