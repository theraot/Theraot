#if NET20 || NET30 || NET35

using Theraot.Threading;

namespace System.Threading
{
    public class ManualResetEventSlim : IDisposable
    {
        private const int _defaultSpinCount = 10;

        private readonly int _spinCount;
        private ManualResetEvent _handle;

        // _requested: -1 = Disposed, 0 = Not requested, 1 = Requested, 2 = _handle ready
        private int _requested;

        // _state: -1 = Disposed, 0 = Not Set, 1 = Set -- Do not set to 0 or 1 when _requested == 1
        private int _state;

        public ManualResetEventSlim()
            : this(false)
        {
            //Empty
        }

        public ManualResetEventSlim(bool initialState)
        {
            _state = initialState ? 1 : 0;
            _spinCount = _defaultSpinCount;
        }

        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0 || spinCount > 2047)
            {
                throw new ArgumentOutOfRangeException("spinCount");
            }
            _spinCount = spinCount;
            _state = initialState ? 1 : 0;
        }

        public bool IsSet
        {
            get
            {
                // The value returned by this property should be considered out of sync
                if (Thread.VolatileRead(ref _state) == -1)
                {
                    return false;
                }
                var handle = GetWaitHandle();
                if (handle != null)
                {
                    return handle.WaitOne(0);
                }
                return Thread.VolatileRead(ref _state) != 0;
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
                return RetriveWaitHandle();
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
            var handle = GetWaitHandle();
            if (handle != null)
            {
                handle.Reset();
            }
            Thread.VolatileWrite(ref _state, 0);
        }

        public void Set()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                // Silent fail
            }
            else
            {
                var handle = GetWaitHandle();
                if (handle != null)
                {
                    handle.Set();
                }
                if (Interlocked.CompareExchange(ref _state, 1, 0) == 0)
                {
                    handle = GetWaitHandle();
                    if (handle != null)
                    {
                        handle.Set();
                    }
                }
            }
        }

        public void Wait()
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (IsSet)
            {
                return;
            }
            var start = ThreadingHelper.TicksNow();
            if (!SpinWait())
            {
                var handle = RetriveWaitHandle();
                handle.WaitOne();
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (millisecondsTimeout == -1)
            {
                Wait();
                return true;
            }
            return WaitExtracted(millisecondsTimeout);
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (millisecondsTimeout == -1)
            {
                WaitExtracted(cancellationToken);
                return true;
            }
            return WaitExtracted(millisecondsTimeout, cancellationToken);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            WaitExtracted(cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            var milliseconds = timeout.TotalMilliseconds;
            return WaitExtracted((int)milliseconds);
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (Thread.VolatileRead(ref _state) == -1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var milliseconds = timeout.TotalMilliseconds;
            return WaitExtracted((int)milliseconds, cancellationToken);
        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "False Positive")]
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Interlocked.Exchange(ref _state, -1) != -1)
                {
                    Thread.VolatileWrite(ref _requested, -1);
                    var handle = Interlocked.Exchange(ref _handle, null);
                    if (handle != null)
                    {
                        handle.Close();
                    }
                }
            }
        }

        private ManualResetEvent GetWaitHandle()
        {
            var found = Thread.VolatileRead(ref _requested);
            switch (found)
            {
                case -1:
                    throw new ObjectDisposedException(GetType().FullName);
                case 0:
                    return null;

                case 1:
                    // Found 1, another thread is creating the wait handle
                    ThreadingHelper.SpinWaitUntil(ref _requested, 2);
                    goto default;
                default:
                    // Found 2, the wait handle is already created
                    // Check if dispose has been called
                    return TryGetWaitHandleExtracted();
            }
        }

        private ManualResetEvent RetriveWaitHandle()
        {
            // At the end of this method: _requested will be 2 or ObjectDisposedException is thrown
            var found = Interlocked.CompareExchange(ref _requested, 1, 0);
            switch (found)
            {
                case -1:
                    throw new ObjectDisposedException(GetType().FullName);
                case 0:
                    // Found 0, was set to 1, create the wait handle
                    var isSet = Thread.VolatileRead(ref _state) != 0;
                    // State may have been set here
                    var created = new ManualResetEvent(isSet);
                    if (Interlocked.CompareExchange(ref _handle, created, null) != null)
                    {
                        created.Close();
                    }
                    Thread.VolatileWrite(ref _requested, 2);
                    goto default;
                case 1:
                    // Found 1, another thread is creating the wait handle
                    ThreadingHelper.SpinWaitUntil(ref _requested, 2);
                    goto default;
                default:
                    // Found 2, the wait handle is already created
                    // Check if dispose has been called
                    return TryGetWaitHandleExtracted();
            }
        }

        private ManualResetEvent TryGetWaitHandleExtracted()
        {
            var handle = Volatile.Read(ref _handle);
            if (handle != null)
            {
                if (Thread.VolatileRead(ref _requested) == 2)
                {
                    return handle;
                }
                handle.Close();
            }
            Thread.VolatileWrite(ref _requested, -1);
            throw new ObjectDisposedException(GetType().FullName);
        }

        private bool WaitExtracted(int millisecondsTimeout)
        {
            var spinWait = new SpinWait();
            if (IsSet)
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            if (!SpinWait(start, millisecondsTimeout))
            {
                retry_shortTimeout:
                if (IsSet)
                {
                    return true;
                }
                var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                if (elapsed < millisecondsTimeout)
                {
                    spinWait.SpinOnce();
                    goto retry_shortTimeout;
                }
            }
            return false;
        }

        private bool WaitExtracted(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            if (IsSet)
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            if (!SpinWait(start, millisecondsTimeout, cancellationToken))
            {
                retry_shortTimeout:
                if (IsSet)
                {
                    return true;
                }
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
                var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
                if (elapsed < millisecondsTimeout)
                {
                    spinWait.SpinOnce();
                    goto retry_shortTimeout;
                }
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
            }
            return false;
        }

        private void WaitExtracted(CancellationToken cancellationToken)
        {
            if (!IsSet)
            {
                if (!SpinWait(cancellationToken))
                {
                    var handle = RetriveWaitHandle();
                    while
                    (
                        WaitHandle.WaitTimeout == WaitHandle.WaitAny
                        (
                            new[]
                            {
                                handle,
                                cancellationToken.WaitHandle
                            },
                            10000
                        )
                    )
                    {
                        Thread.Sleep(1);
                    }
                }
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
            }
        }

        private bool SpinWait()
        {
            if (IsSet)
            {
                return true;
            }
            var spinCount = _spinCount;
            var spinWait = new SpinWait();
            while (!IsSet && spinCount > 0)
            {
                spinWait.SpinOnce();
                spinCount--;
            }
            return IsSet;
        }

        private bool SpinWait(CancellationToken cancellationToken)
        {
            if (IsSet)
            {
                return true;
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var spinCount = _spinCount;
            var spinWait = new SpinWait();
            while (!IsSet && spinCount > 0)
            {
                spinWait.SpinOnce();
                cancellationToken.ThrowIfCancellationRequested();
                GC.KeepAlive(cancellationToken.WaitHandle);
                spinCount--;
            }
            return IsSet;
        }

        private bool SpinWait(long start, int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (IsSet)
            {
                return true;
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var spinCount = _spinCount;
            var spinWait = new SpinWait();
            var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            if (elapsed < millisecondsTimeout)
            {
                while (!IsSet && spinCount > 0)
                {
                    spinWait.SpinOnce();
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
                    spinCount--;
                }
            }
            return IsSet;
        }

        private bool SpinWait(long start, int millisecondsTimeout)
        {
            if (IsSet)
            {
                return true;
            }
            var spinCount = _spinCount;
            var spinWait = new SpinWait();
            var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            if (elapsed < millisecondsTimeout)
            {
                while (!IsSet && spinCount > 0)
                {
                    spinWait.SpinOnce();
                    spinCount--;
                }
            }
            return IsSet;
        }
    }
}

#endif