#if NET20 || NET30 || NET35

using Theraot.Threading;

namespace System.Threading
{
    public partial class ManualResetEventSlim : IDisposable
    {
        private const int _defaultSpinCount = 10;

        private readonly int _spinCount;
        private ManualResetEvent _handle;

        private int _status;

        public ManualResetEventSlim()
            : this(false)
        {
            //Empty
        }

        public ManualResetEventSlim(bool initialState)
        {
            _status = initialState ? (int)Status.Set : (int)Status.NotSet;
            _spinCount = _defaultSpinCount;
        }

        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0 || spinCount > 2047)
            {
                throw new ArgumentOutOfRangeException("spinCount");
            }
            _spinCount = spinCount;
            _status = initialState ? (int)Status.Set : (int)Status.NotSet;
        }

        public bool IsSet
        {
            get
            {
                // The value returned by this property should be considered out of sync
                // But won't be out of sync
                var spinWait = new SpinWait();
                while (true)
                {
                    var status = (Status)Thread.VolatileRead(ref _status);
                    switch (status)
                    {
                        case Status.Disposed:
                            // Disposed
                            throw new ObjectDisposedException(GetType().FullName);

                        case Status.NotSet:
                            // Not Set
                            return false;

                        case Status.Set:
                            // Set
                            return true;

                        case Status.HandleRequested:
                            // Another thread is creating the wait handle
                            // SpinWait
                            break;

                        case Status.HandleReadyNotSet:
                            // NotSet
                            return false;

                        case Status.HandleReadySet:
                            // Set
                            return true;

                        default:
                            // Should not happen
                            break;
                    }
                    spinWait.SpinOnce();
                }
            }
        }

        public int SpinCount
        {
            get { return _spinCount; }
        }

        public WaitHandle WaitHandle
        {
            get
            {
                // If Disposed, throw
                if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                // Get the wait handle
                return GetOrCreateWaitHandle();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var status = (Status)Thread.VolatileRead(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        throw new ObjectDisposedException(GetType().FullName);

                    case Status.NotSet:
                        // Nothing to do
                        return;

                    case Status.Set:
                        // Reset if Set
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.NotSet, (int)Status.Set);
                        if (status == Status.NotSet || status == Status.Set)
                        {
                            // We Reset it or it was already Reset
                            // Either way, we are done
                            return;
                        }
                        // Must has been disposed, or the wait handle requested
                        break;

                    case Status.HandleRequested:
                        // Another thread is creating the wait handle
                        // SpinWait
                        break;

                    case Status.HandleReadyNotSet:
                        // Nothing to do
                        return;

                    case Status.HandleReadySet:
                        // Reset if Set
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.HandleReadyNotSet, (int)Status.HandleReadySet);
                        if (status == Status.HandleReadySet)
                        {
                            // We reset it
                            // Update the wait handle
                            var handle = Volatile.Read(ref _handle);
                            if (handle != null)
                            {
                                // Reset it
                                handle.Reset();
                                // Done
                                return;
                            }
                        }
                        if (status == Status.HandleReadyNotSet)
                        {
                            // Another thread reset it
                            // we are done
                            return;
                        }
                        // Probably Disposed
                        break;

                    default:
                        // Should not happen
                        break;
                }
                spinWait.SpinOnce();
            }
        }

        public void Set()
        {
            var spinWait = new SpinWait();
            while (true)
            {
                var status = (Status)Thread.VolatileRead(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        // Fail sailently
                        return;

                    case Status.NotSet:
                        // Set if Reset
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.Set, (int)Status.NotSet);
                        if (status == Status.NotSet || status == Status.Set)
                        {
                            // We Set it or it was already Set
                            // Either way, we are done
                            return;
                        }
                        // Must has been disposed, or the wait handle requested
                        break;

                    case Status.Set:
                        // Nothing to do
                        return;

                    case Status.HandleRequested:
                        // Another thread is creating the wait handle
                        // SpinWait
                        break;

                    case Status.HandleReadyNotSet:
                        // Set if Reset
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.HandleReadySet, (int)Status.HandleReadyNotSet);
                        if (status == Status.HandleReadyNotSet)
                        {
                            // We set it
                            // Update the wait handle
                            var handle = Volatile.Read(ref _handle);
                            if (handle != null)
                            {
                                // Reset it
                                handle.Set();
                                // Done
                                return;
                            }
                        }
                        if (status == Status.HandleReadySet)
                        {
                            // Another thread set it
                            // we are done
                            return;
                        }
                        // Probably Disposed
                        break;

                    case Status.HandleReadySet:
                        // Nothing to do
                        return;

                    default:
                        // Should not happen
                        break;
                }
                spinWait.SpinOnce();
            }
        }

        public void Wait()
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            var spinWait = new SpinWait();
            var spinCount = _spinCount;
            if (IsSet)
            {
                return;
            }
            retry:
            if (!IsSet)
            {
                if (spinCount > 0)
                {
                    spinCount--;
                    spinWait.SpinOnce();
                    goto retry;
                }
                var handle = GetOrCreateWaitHandle();
                handle.WaitOne();
            }
        }

        public bool Wait(int millisecondsTimeout)
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (millisecondsTimeout == Timeout.Infinite)
            {
                Wait();
                return true;
            }
            return WaitExtracted(millisecondsTimeout);
        }

        public bool Wait(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            if (millisecondsTimeout == Timeout.Infinite)
            {
                WaitExtracted(cancellationToken);
                return true;
            }
            return WaitExtracted(millisecondsTimeout, cancellationToken);
        }

        public void Wait(CancellationToken cancellationToken)
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            WaitExtracted(cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            if (milliseconds == Timeout.Infinite)
            {
                Wait();
                return true;
            }
            return WaitExtracted((int)milliseconds);
        }

        public bool Wait(TimeSpan timeout, CancellationToken cancellationToken)
        {
            // If Disposed, throw
            if (Thread.VolatileRead(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("timeout");
            }
            if (milliseconds == Timeout.Infinite)
            {
                Wait(cancellationToken);
                return true;
            }
            return WaitExtracted((int)milliseconds, cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Set diposed
                if (Interlocked.Exchange(ref _status, (int)Status.Disposed) != (int)Status.Disposed)
                {
                    // Get and erase handle
                    var handle = Interlocked.Exchange(ref _handle, null);
                    if (handle != null)
                    {
                        // Close it
                        handle.Close();
                    }
                }
            }
        }

        private ManualResetEvent GetOrCreateWaitHandle()
        {
            // At the end of this method: _status will be (int)Status.HandleCreated or ObjectDisposedException is thrown
            var spinWait = new SpinWait();
            while (true)
            {
                var status = (Status)Thread.VolatileRead(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        throw new ObjectDisposedException(GetType().FullName);

                    case Status.NotSet:
                    case Status.Set:
                        // Indicate we will be creating the handle
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.HandleRequested, (int)status);
                        if (status == Status.NotSet || status == Status.Set)
                        {
                            // Store the status we status
                            var isSet = status == Status.Set;
                            // Create the handle
                            var created = new ManualResetEvent(isSet);
                            // Set the handle
                            Volatile.Write(ref _handle, created);
                            // Notify that the handle is ready
                            Thread.VolatileWrite(ref _status, isSet ? (int)Status.HandleReadySet : (int)Status.HandleReadyNotSet);
                            // Return the handle we created
                            return created;
                        }
                        // Must has been disposed, or another thread is creating the handle
                        break;

                    case Status.HandleRequested:
                        // Another thread is creating the wait handle
                        // SpinWait
                        break;

                    case Status.HandleReadyNotSet:
                    case Status.HandleReadySet:
                        // The handle already exists
                        // Get the handle that is already created
                        var handle = Volatile.Read(ref _handle);
                        if (handle != null)
                        {
                            // Return it
                            return handle;
                        }
                        // Probably Disposed
                        break;

                    default:
                        // Should not happen
                        break;
                }
                spinWait.SpinOnce();
            }
        }

        private bool WaitExtracted(int millisecondsTimeout)
        {
            var spinWait = new SpinWait();
            var spinCount = _spinCount;
            if (IsSet)
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            retry_longTimeout:
            if (IsSet)
            {
                return true;
            }
            var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            if (elapsed < millisecondsTimeout)
            {
                if (spinCount > 0)
                {
                    spinCount--;
                    spinWait.SpinOnce();
                    goto retry_longTimeout;
                }
                var handle = GetOrCreateWaitHandle();
                var remaining = millisecondsTimeout - (int)elapsed;
                if (remaining > 0)
                {
                    return handle.WaitOne(remaining);
                }
            }
            return false;
        }

        private bool WaitExtracted(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var spinCount = _spinCount;
            if (IsSet)
            {
                return true;
            }
            var start = ThreadingHelper.TicksNow();
            retry_longTimeout:
            if (IsSet)
            {
                return true;
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var elapsed = ThreadingHelper.Milliseconds(ThreadingHelper.TicksNow() - start);
            if (elapsed < millisecondsTimeout)
            {
                if (spinCount > 0)
                {
                    spinCount--;
                    spinWait.SpinOnce();
                    goto retry_longTimeout;
                }
                var handle = GetOrCreateWaitHandle();
                var remaining = millisecondsTimeout - (int)elapsed;
                if (remaining > 0)
                {
                    var result = WaitHandle.WaitAny
                        (
                            new[]
                            {
                                    handle,
                                    cancellationToken.WaitHandle
                            },
                            remaining
                        );
                    cancellationToken.ThrowIfCancellationRequested();
                    GC.KeepAlive(cancellationToken.WaitHandle);
                    return result != WaitHandle.WaitTimeout;
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            return false;
        }

        private void WaitExtracted()
        {
            var spinWait = new SpinWait();
            var spinCount = _spinCount;
            retry:
            if (!IsSet)
            {
                if (spinCount > 0)
                {
                    spinCount--;
                    spinWait.SpinOnce();
                    goto retry;
                }
                var handle = GetOrCreateWaitHandle();
                handle.WaitOne();
            }
        }

        private void WaitExtracted(CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var spinCount = _spinCount;
            if (IsSet)
            {
                return;
            }
            retry:
            if (IsSet)
            {
                return;
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (spinCount > 0)
            {
                spinCount--;
                spinWait.SpinOnce();
                goto retry;
            }
            var handle = GetOrCreateWaitHandle();
            WaitHandle.WaitAny
                (
                    new[]
                    {
                        handle,
                        cancellationToken.WaitHandle
                    }
                );
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
        }
    }
}

#endif