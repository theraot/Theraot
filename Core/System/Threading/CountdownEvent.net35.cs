#if NET20 || NET30 || NET35

using System.Diagnostics;

namespace System.Threading
{
    [DebuggerDisplay("Initial Count={InitialCount}, Current Count={CurrentCount}")]
    public class CountdownEvent : IDisposable
    {
        private int _currentCount;
        private volatile bool _disposed;
        private ManualResetEventSlim _event;
        private int _initialCount;

        public CountdownEvent(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException("initialCount");
            }
            else
            {
                _initialCount = initialCount;
                _currentCount = initialCount;
                _event = new ManualResetEventSlim();
                if (initialCount == 0)
                {
                    this._event.Set();
                }
            }
        }

        public int CurrentCount
        {
            get
            {
                int currentCount = Thread.VolatileRead(ref _currentCount);
                if (currentCount >= 0)
                {
                    return currentCount;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int InitialCount
        {
            get
            {
                return this._initialCount;
            }
        }

        public bool IsSet
        {
            get
            {
                return Thread.VolatileRead(ref _currentCount) <= 0;
            }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                CheckDisposed();
                return this._event.WaitHandle;
            }
        }

        public void AddCount()
        {
            AddCount(1);
        }

        public void AddCount(int signalCount)
        {
            if (!TryAddCount(signalCount))
            {
                throw new InvalidOperationException("Zero");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            Reset(_initialCount);
        }

        public void Reset(int count)
        {
            this.CheckDisposed();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                Thread.VolatileWrite(ref _currentCount, count);
                _initialCount = count;
                if (count == 0)
                {
                    _event.Set();
                }
                else
                {
                    _event.Reset();
                }
            }
        }

        public bool Signal()
        {
            this.CheckDisposed();
            if (Thread.VolatileRead(ref _currentCount) <= 0)
            {
                throw new InvalidOperationException("Zero");
            }
            else
            {
                int currentCount = Interlocked.Decrement(ref _currentCount);
                if (currentCount == 0)
                {
                    _event.Set();
                    return true;
                }
                else
                {
                    if (currentCount < 0)
                    {
                        throw new InvalidOperationException("Zero");
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        public bool Signal(int signalCount)
        {
            if (signalCount <= 0)
            {
                throw new ArgumentOutOfRangeException("signalCount");
            }
            else
            {
                CheckDisposed();
                int finalCount;
                if (Theraot.Threading.ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _currentCount, -signalCount, out finalCount))
                {
                    if (finalCount > 0)
                    {
                        return false;
                    }
                    else
                    {
                        _event.Set();
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool TryAddCount()
        {
            return this.TryAddCount(1);
        }

        public bool TryAddCount(int signalCount)
        {
            if (signalCount <= 0)
            {
                throw new ArgumentOutOfRangeException("signalCount");
            }
            else
            {
                this.CheckDisposed();
                if (Theraot.Threading.ThreadingHelper.SpinWaitRelativeSetUnlessNegative(ref _currentCount, -signalCount))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public void Wait()
        {
            this.Wait(-1, new CancellationToken());
        }

        public void Wait(CancellationToken cancellationToken)
        {
            this.Wait(-1, cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            long totalMilliseconds = (long)timeout.TotalMilliseconds;
            if (totalMilliseconds < (long)-1 || totalMilliseconds > (long)int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            else
            {
                return this.Wait((int)totalMilliseconds, new CancellationToken());
            }
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            bool isSet = this.IsSet;
            if (!isSet)
            {
                isSet = _event.Wait(timeout, cancellationToken);
            }
            return isSet;
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Wait(millisecondsTimeout, new CancellationToken());
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return Wait(TimeSpan.FromMilliseconds(millisecondsTimeout), cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _event.Dispose();
                _disposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("CountdownEvent");
            }
        }
    }
}

#endif