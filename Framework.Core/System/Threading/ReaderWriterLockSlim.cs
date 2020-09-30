#if LESSTHAN_NET35

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using Theraot.Reflection;
using Theraot.Threading;

namespace System.Threading
{
    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
    [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
    public class ReaderWriterLockSlim : IDisposable
    {
        /* Position of each bit isn't really important
        * but their relative order is
        */
        private const int _rwRead = 8;
        private const int _rwReadBit = 3;

        /* These values are used to manipulate the corresponding flags in _rwLock field
        */
        private const int _rwWait = 1;
        private const int _rwWaitUpgrade = 2;
        private const int _rwWrite = 4;

        // This Stopwatch instance is used for all threads since .Elapsed is thread-safe
        private static readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        [ThreadStatic]
        private static Dictionary<int, ThreadLockState>? _currentThreadState;

        // Incremented when a new object is created, should not be readonly
        private static int _idPool = int.MinValue;

        private readonly ThreadLockState[] _fastStateCache;
        private readonly int _id;
        private readonly bool _noRecursion;
        private readonly ManualResetEventSlim _readerDoneEvent;
        private readonly ManualResetEventSlim _upgradableEvent;
        private readonly AtomicBoolean _upgradableTaken;

        /* These events are just here for the sake of having a CPU-efficient sleep
        * when the wait for acquiring the lock is too long
        */
        private readonly ManualResetEventSlim _writerDoneEvent;

        /* Some explanations: this field is the central point of the lock and keep track of all the requests
        * that are being made. The 3 lowest bits are used as flag to track "destructive" lock entries
        * (i.e attempting to take the write lock with or without having acquired an upgradeable lock beforehand).
        * All the remaining bits are interpreted as the actual number of reader currently using the lock
        * (which mean the lock is limited to 2^29 concurrent readers but since it's a high number there
        * is no overflow safe guard to remain simple).
        */
        private bool _disposed;
        private int _rwLock;

        public ReaderWriterLockSlim()
            : this(LockRecursionPolicy.NoRecursion)
        {
            // Empty
        }

        public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
        {
            RecursionPolicy = recursionPolicy;
            _noRecursion = recursionPolicy == LockRecursionPolicy.NoRecursion;
            // ---
            _id = Interlocked.Increment(ref _idPool);
            _fastStateCache = new ThreadLockState[64];
            _upgradableTaken = new AtomicBoolean();
            _upgradableEvent = new ManualResetEventSlim(true);
            _writerDoneEvent = new ManualResetEventSlim(true);
            _readerDoneEvent = new ManualResetEventSlim(true);
        }

        public int CurrentReadCount => (_rwLock >> _rwReadBit) - (_upgradableTaken.Value ? 1 : 0);

        public bool IsReadLockHeld => _rwLock >= _rwRead && (CurrentThreadState.LockState & LockState.Read) > 0;

        public bool IsUpgradeableReadLockHeld => _upgradableTaken.Value && (CurrentThreadState.LockState & LockState.Upgradable) > 0;

        public bool IsWriteLockHeld => (_rwLock & _rwWrite) > 0 && (CurrentThreadState.LockState & LockState.Write) > 0;

        public LockRecursionPolicy RecursionPolicy { get; }

        public int RecursiveReadCount => CurrentThreadState.ReaderRecursiveCount;

        public int RecursiveUpgradeCount => CurrentThreadState.UpgradeableRecursiveCount;

        public int RecursiveWriteCount => CurrentThreadState.WriterRecursiveCount;

        public int WaitingReadCount { get; private set; }

        public int WaitingUpgradeCount { get; private set; }

        public int WaitingWriteCount { get; private set; }

        private ThreadLockState CurrentThreadState => TryGetThreadLockState(Thread.CurrentThread.ManagedThreadId) ?? GetGlobalThreadState();

        [DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void EnterReadLock()
        {
            TryEnterReadLock(-1);
        }

        public void EnterUpgradeableReadLock()
        {
            TryEnterUpgradeableReadLock(-1);
        }

        public void EnterWriteLock()
        {
            TryEnterWriteLock(-1);
        }

        public void ExitReadLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            SynchronizationLockException? exception = null;
            try
            {
                // Empty
            }
            finally
            {
                var currentThreadState = CurrentThreadState;
                if (!((currentThreadState.LockState & LockState.Read) > 0))
                {
                    exception = new SynchronizationLockException("The current thread has not entered the lock in read mode");
                }
                else if (--currentThreadState.ReaderRecursiveCount == 0)
                {
                    currentThreadState.LockState ^= LockState.Read;
                    if (Interlocked.Add(ref _rwLock, -_rwRead) >> _rwReadBit == 0)
                    {
                        _readerDoneEvent.Set();
                    }
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public void ExitUpgradeableReadLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            SynchronizationLockException? exception = null;
            try
            {
                // Empty
            }
            finally
            {
                var currentThreadState = CurrentThreadState;
                if (!((currentThreadState.LockState & (LockState.Upgradable | LockState.Read)) > 0))
                {
                    exception = new SynchronizationLockException("The current thread has not entered the lock in upgradable mode");
                }
                else if (--currentThreadState.UpgradeableRecursiveCount == 0)
                {
                    _upgradableTaken.Value = false;
                    _upgradableEvent.Set();

                    currentThreadState.LockState &= ~LockState.Upgradable;
                    if (Interlocked.Add(ref _rwLock, -_rwRead) >> _rwReadBit == 0)
                    {
                        _readerDoneEvent.Set();
                    }
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public void ExitWriteLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            SynchronizationLockException? exception = null;
            try
            {
                // Empty
            }
            finally
            {
                var currentThreadState = CurrentThreadState;
                if (!((currentThreadState.LockState & LockState.Write) > 0))
                {
                    exception = new SynchronizationLockException("The current thread has not entered the lock in write mode");
                }
                else if (--currentThreadState.WriterRecursiveCount == 0)
                {
                    var isUpgradable = (currentThreadState.LockState & LockState.Upgradable) > 0;
                    currentThreadState.LockState ^= LockState.Write;

                    var value = Interlocked.Add(ref _rwLock, isUpgradable ? _rwRead - _rwWrite : -_rwWrite);
                    _writerDoneEvent.Set();
                    if (isUpgradable && value >> _rwReadBit == 1)
                    {
                        _readerDoneEvent.Reset();
                    }
                }
            }

            if (exception != null)
            {
                throw exception;
            }
        }

        public bool TryEnterReadLock(int millisecondsTimeout)
        {
            var dummy = false;
            return TryEnterReadLock(millisecondsTimeout, ref dummy);
        }

        public bool TryEnterReadLock(TimeSpan timeout)
        {
            return TryEnterReadLock(CheckTimeout(timeout));
        }

        //
        // Taking the Upgradable read lock is like taking a read lock
        // but we limit it to a single upgradable at a time.
        //
        public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
        {
            var currentThreadState = CurrentThreadState;

            if (CheckState(currentThreadState, millisecondsTimeout, LockState.Upgradable))
            {
                ++currentThreadState.UpgradeableRecursiveCount;
                return true;
            }

            if ((currentThreadState.LockState & LockState.Read) > 0)
            {
                throw new LockRecursionException("The current thread has already entered read mode");
            }

            ++WaitingUpgradeCount;
            var start = millisecondsTimeout == -1 ? 0 : _stopwatch.ElapsedMilliseconds;
            var taken = false;
            var success = false;

            // We first try to obtain the upgradeable right
            try
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                while (!_upgradableEvent.IsSet || !taken)
                {
                    taken = _upgradableTaken.TryRelaxedSet();
                    if (taken)
                    {
                        break;
                    }

                    if (millisecondsTimeout != -1 && _stopwatch.ElapsedMilliseconds - start > millisecondsTimeout)
                    {
                        --WaitingUpgradeCount;
                        return false;
                    }

                    _upgradableEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                }

                _upgradableEvent.Reset();

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    // Then it's a simple reader lock acquiring
                    TryEnterReadLock(ComputeTimeout(millisecondsTimeout, start), ref success);
                }
                finally
                {
                    if (success)
                    {
                        currentThreadState.LockState |= LockState.Upgradable;
                        currentThreadState.LockState &= ~LockState.Read;
                        --currentThreadState.ReaderRecursiveCount;
                        ++currentThreadState.UpgradeableRecursiveCount;
                    }
                    else
                    {
                        _upgradableTaken.Value = false;
                        _upgradableEvent.Set();
                    }
                }

                --WaitingUpgradeCount;
            }
            catch (Exception ex)
            {
                _ = ex;
                // An async exception occurred, if we had taken the upgradable mode, release it
                _upgradableTaken.Value &= !taken || success;
            }

            return success;
        }

        public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
        {
            return TryEnterUpgradeableReadLock(CheckTimeout(timeout));
        }

        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            var currentThreadState = CurrentThreadState;

            if (CheckState(currentThreadState, millisecondsTimeout, LockState.Write))
            {
                ++currentThreadState.WriterRecursiveCount;
                return true;
            }

            ++WaitingWriteCount;
            var isUpgradable = (currentThreadState.LockState & LockState.Upgradable) > 0;
            var registered = false;
            var success = false;

            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                /* If the code goes there that means we had a read lock beforehand
                * that need to be suppressed, we also take the opportunity to register
                * our interest in the write lock to avoid other write wannabe process
                * coming in the middle
                */
                if (isUpgradable && _rwLock >= _rwRead)
                {
                    try
                    {
                        // Empty
                    }
                    finally
                    {
                        if (Interlocked.Add(ref _rwLock, _rwWaitUpgrade - _rwRead) >> _rwReadBit == 0)
                        {
                            _readerDoneEvent.Set();
                        }

                        registered = true;
                    }
                }

                var stateCheck = isUpgradable ? _rwWaitUpgrade + _rwWait : _rwWait;
                var start = millisecondsTimeout == -1 ? 0 : _stopwatch.ElapsedMilliseconds;
                var registration = isUpgradable ? _rwWaitUpgrade : _rwWait;

                do
                {
                    var state = _rwLock;

                    if (state <= stateCheck)
                    {
                        try
                        {
                            // Empty
                        }
                        finally
                        {
                            var toWrite = state + _rwWrite - (registered ? registration : 0);
                            if (Interlocked.CompareExchange(ref _rwLock, toWrite, state) == state)
                            {
                                _writerDoneEvent.Reset();
                                currentThreadState.LockState ^= LockState.Write;
                                ++currentThreadState.WriterRecursiveCount;
                                --WaitingWriteCount;
                                registered = false;
                                success = true;
                            }
                        }

                        if (success)
                        {
                            return true;
                        }
                    }

                    state = _rwLock;

                    // We register our interest in taking the Write lock (if upgradeable it's already done)
                    if (!isUpgradable)
                    {
                        while ((state & _rwWait) == 0)
                        {
                            try
                            {
                                // Empty
                            }
                            finally
                            {
                                registered |= Interlocked.CompareExchange(ref _rwLock, state | _rwWait, state) == state;
                            }

                            if (registered)
                            {
                                break;
                            }

                            state = _rwLock;
                        }
                    }

                    // Before falling to sleep
                    do
                    {
                        if (_rwLock <= stateCheck)
                        {
                            break;
                        }

                        if ((_rwLock & _rwWrite) != 0)
                        {
                            _writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                        }
                        else if (_rwLock >> _rwReadBit > 0)
                        {
                            _readerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                        }
                    } while (millisecondsTimeout < 0 || _stopwatch.ElapsedMilliseconds - start < millisecondsTimeout);
                } while (millisecondsTimeout < 0 || _stopwatch.ElapsedMilliseconds - start < millisecondsTimeout);

                --WaitingWriteCount;
            }
            finally
            {
                if (registered)
                {
                    Interlocked.Add(ref _rwLock, isUpgradable ? -_rwWaitUpgrade : -_rwWait);
                }
            }

            return false;
        }

        public bool TryEnterWriteLock(TimeSpan timeout)
        {
            return TryEnterWriteLock(CheckTimeout(timeout));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed || !disposing)
            {
                return;
            }

            if (IsReadLockHeld || IsUpgradeableReadLockHeld || IsWriteLockHeld)
            {
                throw new SynchronizationLockException("The lock is being disposed while still being used");
            }

            _upgradableEvent.Dispose();
            _writerDoneEvent.Dispose();
            _readerDoneEvent.Dispose();
            _disposed = true;
        }

        private static int CheckTimeout(TimeSpan timeout)
        {
            try
            {
                return checked((int)timeout.TotalMilliseconds);
            }
            catch (OverflowException)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout));
            }
        }

        private static int ComputeTimeout(int millisecondsTimeout, long start)
        {
            return millisecondsTimeout == -1 ? -1 : (int)Math.Max(_stopwatch.ElapsedMilliseconds - start - millisecondsTimeout, 1);
        }

        private bool CheckState(ThreadLockState state, int millisecondsTimeout, LockState validState)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ReaderWriterLockSlim));
            }

            if (millisecondsTimeout < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(millisecondsTimeout));
            }

            // Detect and prevent recursion
            var stateLockState = state.LockState;

            if (stateLockState != LockState.None && _noRecursion && (!((stateLockState & LockState.Upgradable) > 0) || validState == LockState.Upgradable))
            {
                throw new LockRecursionException("The current thread has already a lock and recursion isn't supported");
            }

            if (_noRecursion)
            {
                return false;
            }

            // If we already had right lock state, just return
            if ((stateLockState & validState) > 0)
            {
                return true;
            }

            // In read mode you can just enter Read recursively
            if (stateLockState == LockState.Read)
            {
                throw new LockRecursionException();
            }

            return false;
        }

        private ThreadLockState GetGlobalThreadState()
        {
            var currentThreadState = TypeHelper.LazyCreateNew(ref _currentThreadState);

            if (!currentThreadState.TryGetValue(_id, out var state))
            {
                currentThreadState[_id] = state = new ThreadLockState();
            }

            return state;
        }

        private bool TryEnterReadLock(int millisecondsTimeout, ref bool success)
        {
            var currentThreadState = CurrentThreadState;

            if (CheckState(currentThreadState, millisecondsTimeout, LockState.Read))
            {
                ++currentThreadState.ReaderRecursiveCount;
                return true;
            }

            // This is downgrading from upgradable, no need for check since
            // we already have a sort-of read lock that's going to disappear
            // after user calls ExitUpgradeableReadLock.
            // Same idea when recursion is allowed and a write thread wants to
            // go for a Read too.
            if ((currentThreadState.LockState & LockState.Upgradable) > 0 || (!_noRecursion && (currentThreadState.LockState & LockState.Write) > 0))
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    // Empty
                }
                finally
                {
                    Interlocked.Add(ref _rwLock, _rwRead);
                    currentThreadState.LockState |= LockState.Read;
                    ++currentThreadState.ReaderRecursiveCount;
                    success = true;
                }

                return true;
            }

            WaitingReadCount++;
            var start = millisecondsTimeout == -1 ? 0 : _stopwatch.ElapsedMilliseconds;

            do
            {
                /* Check if a writer is present (RwWrite) or if there is someone waiting to
                * acquire a writer lock in the queue (RwWait | RwWaitUpgrade).
                */
                if ((_rwLock & (_rwWrite | _rwWait | _rwWaitUpgrade)) > 0)
                {
                    _writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                    continue;
                }

                /* Optimistically try to add ourselves to the reader value
                * if the adding was too late and another writer came in between
                * we revert the operation.
                */
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    // Empty
                }
                finally
                {
                    int add;
                    if (((add = Interlocked.Add(ref _rwLock, _rwRead)) & (_rwWrite | _rwWait | _rwWaitUpgrade)) == 0)
                    {
                        /* If we are the first reader, reset the event to let other threads
                        * sleep correctly if they try to acquire write lock
                        */
                        if (add >> _rwReadBit == 1)
                        {
                            _readerDoneEvent.Reset();
                        }

                        currentThreadState.LockState ^= LockState.Read;
                        ++currentThreadState.ReaderRecursiveCount;
                        --WaitingReadCount;
                        success = true;
                    }
                    else
                    {
                        Interlocked.Add(ref _rwLock, -_rwRead);
                    }
                }

                if (success)
                {
                    return true;
                }

                _writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
            } while (millisecondsTimeout == -1 || _stopwatch.ElapsedMilliseconds - start < millisecondsTimeout);

            --WaitingReadCount;
            return false;
        }

        private ThreadLockState? TryGetThreadLockState(int managedThreadId)
        {
            if (managedThreadId < _fastStateCache.Length)
            {
                return _fastStateCache[managedThreadId] ?? (_fastStateCache[managedThreadId] = new ThreadLockState());
            }

            return null;
        }
    }
}

#endif