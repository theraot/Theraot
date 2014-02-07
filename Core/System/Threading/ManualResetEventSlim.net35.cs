#if NET20 || NET30 || NET35

using Theraot.Threading;

namespace System.Threading
{
    public class ManualResetEventSlim : IDisposable
    {
        private const int INT_DefaultSpinCount = 10;
        private const int INT_LongTimeOutHint = 160;
        private const int INT_SleepCountHint = 5;
        private const int INT_SpinWaitHint = 20;

        private static readonly TimeSpan _timeout = TimeSpan.FromMilliseconds(INT_LongTimeOutHint);

        private ManualResetEvent _handle;
        private int _requested;
        private int _spinCount;
        private int _state;

        public ManualResetEventSlim()
            : this(false)
        {
            //Empty
        }

        public ManualResetEventSlim(bool initialState)
        {
            _state = initialState ? 1 : 0;
            _spinCount = INT_DefaultSpinCount;
        }

        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0 || spinCount > 2047)
            {
                throw new ArgumentOutOfRangeException("spinCount");
            }
            else
            {
                _spinCount = spinCount;
                _state = initialState ? 1 : 0;
            }
        }

        public bool IsSet
        {
            get
            {
                if (Thread.VolatileRead(ref _state) == -1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                else
                {
                    if (Thread.VolatileRead(ref _requested) != 0)
                    {
                        var handle = RetrieveWaitHandle();
                        return handle.WaitOne(0);
                    }
                    else
                    {
                        return Thread.VolatileRead(ref _state) != 0;
                    }
                }
            }
        }

        public int SpinCount
        {
            get
            {
                return _spinCount;
            }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                return RetrieveWaitHandle();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                Thread.VolatileWrite(ref _state, 0);
                if (Thread.VolatileRead(ref _requested) != 0)
                {
                    var handle = RetrieveWaitHandle();
                    handle.Reset();
                }
            }
        }

        public void Set()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                Thread.VolatileWrite(ref _state, 1);
                if (Thread.VolatileRead(ref _requested) != 0)
                {
                    var handle = RetrieveWaitHandle();
                    handle.Set();
                }
            }
        }

        public void Wait()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                int count = 0;
                if (IsSet)
                {
                    return;
                }
                else
                {
                    var start = ThreadingHelper.TicksNow();
                retry:
                    if (IsSet)
                    {
                        return;
                    }
                    else
                    {
                        if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < INT_LongTimeOutHint)
                        {
                            ThreadingHelper.SpinOnce(ref count);
                            goto retry;
                        }
                        else
                        {
                            var handle = RetrieveWaitHandle();
                            handle.WaitOne();
                        }
                    }
                }
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                if (millisecondsTimeout < -1)
                {
                    throw new ArgumentOutOfRangeException("millisecondsTimeout");
                }
                else if (millisecondsTimeout == -1)
                {
                    Wait();
                    return true;
                }
                else
                {
                    return WaitExtracted(millisecondsTimeout);
                }
            }
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.ThrowIfSourceDisposed();
                if (millisecondsTimeout < -1)
                {
                    throw new ArgumentOutOfRangeException("millisecondsTimeout");
                }
                else if (millisecondsTimeout == -1)
                {
                    Wait();
                    return true;
                }
                else
                {
                    return WaitExtracted(millisecondsTimeout, cancellationToken);
                }
            }
        }

        public bool Wait(TimeSpan timeout)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                var milliseconds = timeout.TotalMilliseconds;
                return WaitExtracted((int)milliseconds);
            }
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
                cancellationToken.ThrowIfSourceDisposed();
                var milliseconds = timeout.TotalMilliseconds;
                return WaitExtracted((int)milliseconds, cancellationToken);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False Positive")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Interlocked.Exchange(ref _state, -1) != -1)
                {
                    Thread.VolatileWrite(ref _requested, 0);
                    var handle = Interlocked.Exchange(ref _handle, null);
                    if (handle != null)
                    {
                        handle.Close();
                    }
                }
            }
        }

        private ManualResetEvent RetrieveWaitHandle()
        {
            if (Interlocked.CompareExchange(ref _requested, 1, 0) == 0)
            {
                var isSet = Thread.VolatileRead(ref _state) != 0;
                ThreadingHelper.VolatileWrite(ref _handle, new ManualResetEvent(isSet));
            }
            else if (_handle == null)
            {
                ThreadingHelper.SpinWaitWhileNull(ref _handle);
            }
            return ThreadingHelper.VolatileRead(ref _handle);
        }

        private bool WaitExtracted(int millisecondsTimeout)
        {
            int count = 0;
            if (IsSet)
            {
                return true;
            }
            else
            {
                var start = ThreadingHelper.TicksNow();
            retry:
                if (IsSet)
                {
                    return true;
                }
                else if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) > millisecondsTimeout)
                {
                    return false;
                }
                else
                {
                    if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < INT_LongTimeOutHint)
                    {
                        ThreadingHelper.SpinOnce(ref count);
                        goto retry;
                    }
                    else
                    {
                        var handle = RetrieveWaitHandle();
                        var remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                        if (remaining > 0)
                        {
                            return handle.WaitOne(remaining);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        private bool WaitExtracted(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int count = 0;
            if (IsSet)
            {
                return true;
            }
            else
            {
                var start = ThreadingHelper.TicksNow();
            retry:
                if (IsSet)
                {
                    return true;
                }
                else if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) > millisecondsTimeout)
                {
                    return false;
                }
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    cancellationToken.ThrowIfSourceDisposed();
                    if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < INT_LongTimeOutHint)
                    {
                        ThreadingHelper.SpinOnce(ref count);
                        goto retry;
                    }
                    else
                    {
                        var handle = RetrieveWaitHandle();
                        var remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                        if (remaining > 0)
                        {
                            return handle.WaitOne(remaining);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }
    }
}

#endif