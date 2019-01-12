#if LESSTHAN_NET40

using System.Diagnostics;
using Theraot.Threading;

namespace System.Threading
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public partial class ManualResetEventSlim : IDisposable
    {
        private const int _defaultSpinCount = 10;
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
            SpinCount = _defaultSpinCount;
        }

        public ManualResetEventSlim(bool initialState, int spinCount)
        {
            if (spinCount < 0 || spinCount > 2047)
            {
                throw new ArgumentOutOfRangeException(nameof(spinCount));
            }
            SpinCount = spinCount;
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
                    var status = (Status)Volatile.Read(ref _status);
                    switch (status)
                    {
                        case Status.Disposed:
                            // Disposed
                            // A disposed ManualResetEventSlim should report not set
                            return false;

                        case Status.NotSet:
                        case Status.HandleRequestedNotSet:
                        case Status.HandleReadyNotSet:
                            // Not Set
                            return false;

                        case Status.Set:
                        case Status.HandleRequestedSet:
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

        public int SpinCount { get; }

        public WaitHandle WaitHandle
        {
            get
            {
                // If Disposed, throw
                if (Volatile.Read(ref _status) == (int)Status.Disposed)
                {
                    throw new ObjectDisposedException(nameof(ManualResetEventSlim));
                }
                // Get the wait handle
                return GetOrCreateWaitHandle();
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
            var spinWait = new SpinWait();
            while (true)
            {
                var status = (Status)Volatile.Read(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        throw new ObjectDisposedException(nameof(ManualResetEventSlim));

                    case Status.NotSet:
                    case Status.HandleRequestedNotSet:
                    case Status.HandleReadyNotSet:
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

                    case Status.HandleRequestedSet:
                        // Another thread is creating the wait handle
                        // SpinWait
                        break;

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
                var status = (Status)Volatile.Read(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        // Fail saliently
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

                    case Status.HandleRequestedNotSet:
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

                    case Status.Set:
                    case Status.HandleRequestedSet:
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
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            var spinWait = new SpinWait();
            var spinCount = SpinCount;
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
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
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
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
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
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            WaitExtracted(cancellationToken);
        }

        public bool Wait(TimeSpan timeout)
        {
            // If Disposed, throw
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
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
            if (Volatile.Read(ref _status) == (int)Status.Disposed)
            {
                throw new ObjectDisposedException(nameof(ManualResetEventSlim));
            }
            cancellationToken.ThrowIfCancellationRequested();
            GC.KeepAlive(cancellationToken.WaitHandle);
            var milliseconds = (long)timeout.TotalMilliseconds;
            if (milliseconds < -1L || milliseconds > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
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
                // Set disposed
                if (Interlocked.Exchange(ref _status, (int)Status.Disposed) != (int)Status.Disposed)
                {
                    // Get and erase handle
                    var handle = Interlocked.Exchange(ref _handle, null);
                    // Close it
                    handle?.Close();
                }
            }
        }

        private ManualResetEvent GetOrCreateWaitHandle()
        {
            // At the end of this method: _status will be (int)Status.HandleCreated or ObjectDisposedException is thrown
            var spinWait = new SpinWait();
            while (true)
            {
                var status = (Status)Volatile.Read(ref _status);
                switch (status)
                {
                    case Status.Disposed:
                        // Disposed
                        throw new ObjectDisposedException(nameof(ManualResetEventSlim));

                    case Status.NotSet:
                        // Indicate we will be creating the handle
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.HandleRequestedNotSet, (int)status);
                        if (status == Status.NotSet)
                        {
                            // Create the handle
                            var created = new ManualResetEvent(false);
                            // Set the handle
                            Volatile.Write(ref _handle, created);
                            // Notify that the handle is ready
                            Volatile.Write(ref _status, (int)Status.HandleReadyNotSet);
                            // Return the handle we created
                            return created;
                        }
                        // Must has been disposed, or another thread is creating the handle
                        break;

                    case Status.Set:
                        // Indicate we will be creating the handle
                        status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.HandleRequestedSet, (int)status);
                        if (status == Status.Set)
                        {
                            // Create the handle
                            var created = new ManualResetEvent(true);
                            // Set the handle
                            Volatile.Write(ref _handle, created);
                            // Notify that the handle is ready
                            Volatile.Write(ref _status, (int)Status.HandleReadySet);
                            // Return the handle we created
                            return created;
                        }
                        // Must has been disposed, or another thread is creating the handle
                        break;

                    case Status.HandleRequestedNotSet:
                    case Status.HandleRequestedSet:
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
            var spinCount = SpinCount;
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
            var spinCount = SpinCount;
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

        private void WaitExtracted(CancellationToken cancellationToken)
        {
            var spinWait = new SpinWait();
            var spinCount = SpinCount;
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