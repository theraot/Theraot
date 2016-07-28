#if NET35

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
                throw new ArgumentOutOfRangeException("initialCount");
            }
            else
            {
                _initialCount = initialCount;
                _currentCount = initialCount;
                _event = new ManualResetEventSlim();
                if (initialCount == 0)
                {
                    _event.Set();
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
                return _initialCount;
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
            CheckDisposed();
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
            CheckDisposed();
            if (Thread.VolatileRead(ref _currentCount) <= 0)
            {
                throw new InvalidOperationException("Below Zero");
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
                        throw new InvalidOperationException("Below Zero");
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
                int lastValue;
                if (ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _currentCount, -signalCount, out lastValue))
                {
                    var result = lastValue - signalCount;
                    if (result == 0)
                    {
                        _event.Set();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    throw new InvalidOperationException("Below Zero");
                }
            }
        }

        public bool TryAddCount()
        {
            return TryAddCount(1);
        }

        public bool TryAddCount(int signalCount)
        {
            if (signalCount <= 0)
            {
                throw new ArgumentOutOfRangeException("signalCount");
            }
            else
            {
                CheckDisposed();
                int lastValue;
                if (ThreadingHelper.SpinWaitRelativeExchangeBounded(ref _currentCount, signalCount, 1, int.MaxValue, out lastValue))
                {
                    return true;
                }
                else
                {
                    if (lastValue < 1)
                    {
                        return false;
                    }
                    else
                    {
                        throw new InvalidOperationException("Max");
                    }
                }
            }
        }

        public void Wait()
        {
            Wait(-1, CancellationToken.None);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            Wait(-1, cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            var totalMilliseconds = (long)timeout.TotalMilliseconds;
            if (totalMilliseconds < -1L || totalMilliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            else
            {
                return Wait((int)totalMilliseconds, CancellationToken.None);
            }
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            CheckDisposed();
            cancellationToken.ThrowIfCancellationRequested();
            bool isSet = IsSet;
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
                throw new ObjectDisposedException("CountdownEvent");
            }
        }
    }
}

#endif