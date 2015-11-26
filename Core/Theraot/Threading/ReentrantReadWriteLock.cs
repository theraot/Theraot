#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class ReentrantReadWriteLock : IReadWriteLock
    {
        private NoTrackingThreadLocal<int> _currentReadingCount = new NoTrackingThreadLocal<int>(() => 0);
        private int _edge;
        private ManualResetEventSlim _freeToRead = new ManualResetEventSlim(false);
        private ManualResetEventSlim _freeToWrite = new ManualResetEventSlim(false);
        private int _master;
        private Thread _ownerThread;
        private int _readCount;
        private int _writeCount;

        public bool HasReader
        {
            get
            {
                return _readCount > 0;
            }
        }

        public bool HasWriter
        {
            get
            {
                return _ownerThread != null;
            }
        }

        public bool IsCurrentThreadReader
        {
            get
            {
                return _currentReadingCount.IsValueCreated && _currentReadingCount.Value > 0;
            }
        }

        public bool IsCurrentThreadWriter
        {
            get
            {
                return Thread.CurrentThread == _ownerThread;
            }
        }

        public IDisposable EnterRead()
        {
            WaitCanRead();
            return DisposableAkin.Create(DoneRead);
        }

        public IDisposable EnterWrite()
        {
            if (_currentReadingCount.Value > 0)
            {
                if (WaitUpgrade())
                {
                    return DisposableAkin.Create(DoneUpgrade);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                WaitCanWrite();
                return DisposableAkin.Create(DoneWrite);
            }
        }

        public bool TryEnterRead(out IDisposable engagement)
        {
            engagement = null;
            if (!CanRead())
            {
                return false;
            }
            engagement = DisposableAkin.Create(DoneRead);
            return true;
        }

        public bool TryEnterWrite(out IDisposable engagement)
        {
            engagement = null;
            if (_currentReadingCount.Value > 0)
            {
                if (CanUpgrade())
                {
                    engagement = DisposableAkin.Create(DoneUpgrade);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (!CanWrite())
                {
                    return false;
                }
                engagement = DisposableAkin.Create(DoneWrite);
                return true;
            }
        }

        private bool CanRead()
        {
            if (Thread.CurrentThread == ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                Interlocked.Increment(ref _readCount);
                _currentReadingCount.Value++;
                return true;
            }
            else
            {
                if (Interlocked.CompareExchange(ref _master, 1, 0) >= 0)
                {
                    _freeToWrite.Reset();
                    Interlocked.Increment(ref _readCount);
                    _currentReadingCount.Value++;
                    return true;
                }
                return false;
            }
        }

        private bool CanUpgrade()
        {
            if (Thread.CurrentThread == ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                Interlocked.Increment(ref _writeCount);
                return true;
            }
            else
            {
                var check = Interlocked.CompareExchange(ref _master, -2, 1);
                if (check == 1)
                {
                    _freeToRead.Reset();
                    // --
                    if
                        (
                        Thread.VolatileRead(ref _readCount) <= _currentReadingCount.Value
                        && Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null
                        )
                    {
                        Thread.VolatileWrite(ref _master, -1);
                        Interlocked.Increment(ref _writeCount);
                        return true;
                    }
                }
                return false;
            }
        }

        private bool CanWrite()
        {
            if (Thread.CurrentThread == ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                Interlocked.Increment(ref _writeCount);
                return true;
            }
            else
            {
                if (Interlocked.CompareExchange(ref _master, -1, 0) == 0)
                {
                    _freeToRead.Reset();
                    if (Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null)
                    {
                        // Success
                        Interlocked.Increment(ref _writeCount);
                        return true;
                    }
                }
                return false;
            }
        }

        private void DoneRead()
        {
            if (Thread.CurrentThread == ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                Interlocked.Decrement(ref _readCount);
                _currentReadingCount.Value--;
            }
            else
            {
                if (Thread.VolatileRead(ref _master) < 0)
                {
                    _currentReadingCount.Value--;
                    if (Interlocked.Decrement(ref _readCount) <= Thread.VolatileRead(ref _edge))
                    {
                        Thread.VolatileWrite(ref _master, 0);
                        _freeToWrite.Set();
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _readCount);
                    _currentReadingCount.Value--;
                }
            }
        }

        private void DoneUpgrade()
        {
            if (Interlocked.Decrement(ref _writeCount) == 0)
            {
                Thread.VolatileWrite(ref _edge, 0);
                ThreadingHelper.VolatileWrite(ref _ownerThread, null);
                _freeToRead.Set();
            }
        }

        private void DoneWrite()
        {
            if (Interlocked.Decrement(ref _writeCount) == 0)
            {
                Thread.VolatileWrite(ref _master, 0);
                ThreadingHelper.VolatileWrite(ref _ownerThread, null);
                _freeToRead.Set();
                _freeToWrite.Set();
            }
        }

        private void WaitCanRead()
        {
            if (Thread.CurrentThread != ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                var check = Interlocked.CompareExchange(ref _master, 1, 0);
                while (true)
                {
                    switch (check)
                    {
                        case -2:
                        // Write mode already requested
                        case -1:
                            // There is a writer
                            // Go to wait
                            _freeToRead.Wait();
                            check = Interlocked.CompareExchange(ref _master, 1, 0);
                            break;

                        case 0:
                            // Free to proceed
                            // GO!
                            _freeToWrite.Reset();
                            goto case 1;

                        case 1:
                            // There are readers currently
                            // GO!
                            Interlocked.Increment(ref _readCount);
                            _currentReadingCount.Value++;
                            return;
                    }
                }
            }
        }

        private void WaitCanWrite()
        {
            if (Thread.CurrentThread != ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                var check = Interlocked.CompareExchange(ref _master, -1, 0);
                while (true)
                {
                    switch (check)
                    {
                        case -2:
                        // Write mode already requested
                        case -1:
                            // There is another writer
                            // Go to wait
                            _freeToWrite.Wait();
                            check = Interlocked.CompareExchange(ref _master, -1, 0);
                            break;

                        case 0:
                            // Free to proceed
                            // GO!
                            _freeToRead.Reset();
                            if (Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null)
                            {
                                // Success
                                Interlocked.Increment(ref _writeCount);
                                return;
                            }
                            else
                            {
                                // It was reserved by another thread
                                break;
                            }

                        case 1:
                            // There are readers currently
                            // Requesting write mode
                            check = Interlocked.CompareExchange(ref _master, -2, 1);
                            if (check == 1)
                            {
                                _freeToRead.Reset();
                                check = -2;
                            }
                            break;
                    }
                }
            }
        }

        private bool WaitUpgrade()
        {
            var owner = ThreadingHelper.VolatileRead(ref _ownerThread);
            if (owner == null || owner == Thread.CurrentThread)
            {
                var check = 1;
                while (true)
                {
                    switch (check)
                    {
                        case -2:
                            // Write mode already requested
                            // We are going to steal it
                            // Reserve the lock - so no other writer can take it
                            owner = Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null);
                            if (owner == null || owner == Thread.CurrentThread)
                            {
                                // Set the edge
                                Thread.VolatileWrite(ref _edge, _currentReadingCount.Value);
                            }
                            else
                            {
                                // It was reserved by another thread - abort mission
                                return false;
                            }
                            if (Thread.VolatileRead(ref _readCount) > Thread.VolatileRead(ref _edge))
                            {
                                // We still need every other reader to finish
                                _freeToWrite.Wait();
                                check = Interlocked.CompareExchange(ref _master, -1, 0);
                            }
                            else
                            {
                                // None to wait
                                Thread.VolatileWrite(ref _master, -1);
                                check = -1;
                            }
                            break;

                        case -1:
                            // There is a writer
                            // Abort mission
                            _freeToRead.Reset();
                            Interlocked.Increment(ref _writeCount);
                            return true;

                        case 0:
                            // Free to proceed
                            return true;

                        case 1:
                            // There are readers currently - of course, current thread is a reader
                            // Requesting write mode
                            check = Interlocked.CompareExchange(ref _master, -2, 1);
                            if (check == 1)
                            {
                                _freeToRead.Reset();
                                check = -2;
                            }
                            break;
                    }
                }
            }
            else
            {
                return false;
            }
        }
    }

    internal sealed partial class ReentrantReadWriteLock : IExtendedDisposable
    {
        private int _status;

        [System.Diagnostics.DebuggerNonUserCode]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~ReentrantReadWriteLock()
        {
            try
            {
                // Empty
            }
            finally
            {
                Dispose(false);
            }
        }

        public bool IsDisposed
        {
            [System.Diagnostics.DebuggerNonUserCode]
            get
            {
                return _status == -1;
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

        [System.Diagnostics.DebuggerNonUserCode]
        public void DisposedConditional(Action whenDisposed, Action whenNotDisposed)
        {
            if (_status == -1)
            {
                if (!ReferenceEquals(whenDisposed, null))
                {
                    whenDisposed.Invoke();
                }
            }
            else
            {
                if (!ReferenceEquals(whenNotDisposed, null))
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _status);
                        }
                    }
                    else
                    {
                        if (!ReferenceEquals(whenDisposed, null))
                        {
                            whenDisposed.Invoke();
                        }
                    }
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public TReturn DisposedConditional<TReturn>(Func<TReturn> whenDisposed, Func<TReturn> whenNotDisposed)
        {
            if (_status == -1)
            {
                if (ReferenceEquals(whenDisposed, null))
                {
                    return default(TReturn);
                }
                else
                {
                    return whenDisposed.Invoke();
                }
            }
            else
            {
                if (ReferenceEquals(whenNotDisposed, null))
                {
                    return default(TReturn);
                }
                else
                {
                    if (ThreadingHelper.SpinWaitRelativeSet(ref _status, 1, -1))
                    {
                        try
                        {
                            return whenNotDisposed.Invoke();
                        }
                        finally
                        {
                            Interlocked.Decrement(ref _status);
                        }
                    }
                    else
                    {
                        if (ReferenceEquals(whenDisposed, null))
                        {
                            return default(TReturn);
                        }
                        else
                        {
                            return whenDisposed.Invoke();
                        }
                    }
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
        private void Dispose(bool disposeManagedResources)
        {
            if (TakeDisposalExecution())
            {
                try
                {
                    if (disposeManagedResources)
                    {
                        _freeToRead.Dispose();
                        _freeToWrite.Dispose();
                        _currentReadingCount.Dispose();
                    }
                }
                finally
                {
                    _freeToRead = null;
                    _freeToWrite = null;
                    _currentReadingCount = null;
                }
            }
        }

        private bool TakeDisposalExecution()
        {
            if (_status == -1)
            {
                return false;
            }
            else
            {
                return ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
            }
        }
    }
}

#endif