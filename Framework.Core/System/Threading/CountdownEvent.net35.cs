#if NET20 || NET30 || NET35

using System.Diagnostics;
using Theraot.Threading;

namespace System.Threading
{
    [DebuggerDisplay("Initial Count={InitialCount}, Current Count={CurrentCount}")]
    public class CountdownEvent : IDisposable
    {
        private readonly ManualResetEventSlim _event;
        private int _currentCount;
        private volatile bool _disposed;
        private int _initialCount;

        public CountdownEvent(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            }
            _initialCount = initialCount;
            _currentCount = initialCount;
            _event = new ManualResetEventSlim();
            if (initialCount == 0)
            {
                _event.Set();
            }
        }

        public int CurrentCount
        {
            get
            {
                var currentCount = Thread.VolatileRead(ref _currentCount);
                return currentCount >= 0 ? currentCount : 0;
            }
        }

        public int InitialCount
        {
            get { return _initialCount; }
        }

        public bool IsSet
        {
            get { return Thread.VolatileRead(ref _currentCount) <= 0; }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                CheckDisposed();
                return _event.WaitHandle;
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
                throw new InvalidOperationException("Already Zero");
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Reset()
        {
            Reset(_initialCount);
        }

        public void Reset(int count)
        {
            CheckDisposed();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
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

        public bool Signal()
        {
            CheckDisposed();
            if (Thread.VolatileRead(ref _currentCount) <= 0)
            {
                throw new InvalidOperationException("Below Zero");
            }
            var currentCount = Interlocked.Decrement(ref _currentCount);
            if (currentCount == 0)
            {
                _event.Set();
                return true;
            }
            if (currentCount < 0)
            {
                throw new InvalidOperationException("Below Zero");
            }
            return false;
        }

        public bool Signal(int signalCount)
        {
            if (signalCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalCount));
            }
            CheckDisposed();
            int lastValue;
            if (ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _currentCount, -signalCount, out lastValue))
            {
                var result = lastValue - signalCount;
                if (result == 0)
                {
                    _event.Set();
                    return true;
                }
                return false;
            }
            throw new InvalidOperationException("Below Zero");
        }

        public bool TryAddCount()
        {
            return TryAddCount(1);
        }

        public bool TryAddCount(int signalCount)
        {
            if (signalCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(signalCount));
            }
            CheckDisposed();
            int lastValue;
            if (ThreadingHelper.SpinWaitRelativeExchangeBounded(ref _currentCount, signalCount, 1, int.MaxValue, out lastValue))
            {
                return true;
            }
            if (lastValue < 1)
            {
                return false;
            }
            throw new InvalidOperationException("Max");
        }

        public void Wait()
        {
            Wait(Timeout.Infinite, CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            Wait(Timeout.Infinite, cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
            if (milliseconds == -1)
            {
                Wait();
                return true;
            }
            return Wait((int)milliseconds, CancellationToken.None);
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            var isSet = IsSet;
            if (!isSet)
            {
                isSet = _event.Wait(timeout, cancellationToken);
            }
            return isSet;
        }

        public bool Wait(int millisecondsTimeout)
        {
            return Wait(millisecondsTimeout, CancellationToken.None);
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
                throw new ObjectDisposedException(nameof(CountdownEvent));
            }
        }
    }
}

#endif