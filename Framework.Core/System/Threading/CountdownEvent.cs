#if LESSTHAN_NET40

using System.Diagnostics;
using Theraot.Threading;

namespace System.Threading
{
    [DebuggerDisplay("Initial Count={InitialCount}, Current Count={CurrentCount}")]
    public class CountdownEvent : IDisposable
    {
        private int _currentCount;
        private ManualResetEventSlim _event;

        public CountdownEvent(int initialCount)
        {
            if (initialCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCount));
            }
            InitialCount = initialCount;
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
                var currentCount = Volatile.Read(ref _currentCount);
                return currentCount >= 0 ? currentCount : 0;
            }
        }

        public int InitialCount { get; private set; }

        public bool IsSet => Volatile.Read(ref _currentCount) <= 0;

        public WaitHandle WaitHandle => GetEvent().WaitHandle;

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

        [DebuggerNonUserCode]
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
            Reset(InitialCount);
        }

        public void Reset(int count)
        {
            var e = GetEvent();
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            Volatile.Write(ref _currentCount, count);
            InitialCount = count;
            if (count == 0)
            {
                e.Set();
            }
            else
            {
                e.Reset();
            }
        }

        public bool Signal()
        {
            var e = GetEvent();
            if (Volatile.Read(ref _currentCount) <= 0)
            {
                throw new InvalidOperationException("Below Zero");
            }
            var currentCount = Interlocked.Decrement(ref _currentCount);
            if (currentCount == 0)
            {
                e.Set();
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
            var e = GetEvent();
            if (ThreadingHelper.SpinWaitRelativeExchangeUnlessNegative(ref _currentCount, -signalCount, out var lastValue))
            {
                var result = lastValue - signalCount;
                if (result == 0)
                {
                    e.Set();
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
            GC.KeepAlive(GetEvent());
            if (ThreadingHelper.SpinWaitRelativeExchangeBounded(ref _currentCount, signalCount, 1, int.MaxValue, out var lastValue))
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
            var e = GetEvent();
            cancellationToken.ThrowIfCancellationRequested();
            var isSet = IsSet;
            if (!isSet)
            {
                isSet = e.Wait(timeout, cancellationToken);
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
            // Thread Safe Dispose
            if (disposing)
            {
                var e = Interlocked.Exchange(ref _event, null);
                e?.Dispose();
            }
        }

        private ManualResetEventSlim GetEvent()
        {
            var e = Volatile.Read(ref _event);
            if (e == null)
            {
                throw new ObjectDisposedException(nameof(CountdownEvent));
            }
            return e;
        }
    }
}

#endif