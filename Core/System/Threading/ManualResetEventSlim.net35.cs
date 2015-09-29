#if NET20 || NET30 || NET35

using Theraot.Threading;

namespace System.Threading
{
    public class ManualResetEventSlim : IDisposable
    {
        private const int INT_DefaultSpinCount = 10;
        private const int INT_LongTimeOutHint = 160;

        private readonly int _spinCount;
        private ManualResetEvent _handle;
        private int _requested;
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
                    return false;
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
                if (Thread.VolatileRead(ref _state) == -1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                else
                {
                    return RetrieveWaitHandle();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
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
                // Silent fail
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
                if (!IsSet)
                {
                    var start = ThreadingHelper.TicksNow();
                    retry:
                    if (!IsSet)
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
                GC.KeepAlive(cancellationToken.WaitHandle);
                if (millisecondsTimeout < -1)
                {
                    throw new ArgumentOutOfRangeException("millisecondsTimeout");
                }
                else if (millisecondsTimeout == -1)
                {
                    WaitExtracted(cancellationToken);
                    return true;
                }
                else
                {
                    return WaitExtracted(millisecondsTimeout, cancellationToken);
                }
            }
        }

        public void Wait(CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
                WaitExtracted(cancellationToken);
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
                GC.KeepAlive(cancellationToken.WaitHandle);
                var milliseconds = timeout.TotalMilliseconds;
                return WaitExtracted((int)milliseconds, cancellationToken);
            }
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False Positive")]
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
            var start = ThreadingHelper.TicksNow();
            if (IsSet)
            {
                return true;
            }
            else
            {
                if (millisecondsTimeout > INT_LongTimeOutHint)
                {
                    retry_longTimeout:
                    if (IsSet)
                    {
                        return true;
                    }
                    else
                    {
                        var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                        if (elapsed < millisecondsTimeout)
                        {
                            if (elapsed < INT_LongTimeOutHint)
                            {
                                ThreadingHelper.SpinOnce(ref count);
                                goto retry_longTimeout;
                            }
                            else
                            {
                                var handle = RetrieveWaitHandle();
                                var remaining = (int)(millisecondsTimeout - ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start));
                                if (remaining > 0)
                                {
                                    return handle.WaitOne(remaining);
                                }
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    retry_shortTimeout:
                    if (IsSet)
                    {
                        return true;
                    }
                    else
                    {
                        var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                        if (elapsed < millisecondsTimeout)
                        {
                            ThreadingHelper.SpinOnce(ref count);
                            goto retry_shortTimeout;
                        }
                        return false;
                    }
                }
            }
        }

        private bool WaitExtracted(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            int count = 0;
            var start = ThreadingHelper.TicksNow();
            if (IsSet)
            {
                return true;
            }
            else
            {
                retry:
                if (IsSet)
                {
                    return true;
                }
                else
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start) < millisecondsTimeout)
                    {
                        GC.KeepAlive(cancellationToken.WaitHandle);
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
                                if (handle.WaitOne(remaining))
                                {
                                    cancellationToken.ThrowIfCancellationRequested();
                                    return true;
                                }
                            }
                        }
                    }
                    cancellationToken.ThrowIfCancellationRequested();
                    return false;
                }
            }
        }

        private void WaitExtracted(CancellationToken cancellationToken)
        {
            int count = 0;
            var start = ThreadingHelper.TicksNow();
            if (!IsSet)
            {
                retry:
                if (!IsSet)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
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
}

#endif