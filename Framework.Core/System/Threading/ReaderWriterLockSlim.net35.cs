#if NET20 || NET30

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
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
        private const int _rwReadBit = 3;

        /* These values are used to manipulate the corresponding flags in _rwlock field
        */
        private const int _rwWait = 1;
        private const int _rwWaitUpgrade = 2;
        private const int _rwWrite = 4;
        private const int _rwRead = 8;

        // This Stopwatch instance is used for all threads since .Elapsed is thread-safe
        private static readonly Stopwatch _stopwatch;

        // Incremented when a new object is created, should not be readonly
        private static int _idPool;

        private readonly LockRecursionPolicy _recursionPolicy;
        private readonly bool _noRecursion;
        private readonly AtomicBoolean _upgradableTaken;

        /* These events are just here for the sake of having a CPU-efficient sleep
        * when the wait for acquiring the lock is too long
        */
        private readonly ManualResetEventSlim _upgradableEvent;
        private readonly ManualResetEventSlim _writerDoneEvent;
        private readonly ManualResetEventSlim _readerDoneEvent;
        private readonly int _id;

        /* Some explanations: this field is the central point of the lock and keep track of all the requests
        * that are being made. The 3 lowest bits are used as flag to track "destructive" lock entries
        * (i.e attempting to take the write lock with or without having acquired an upgradeable lock beforehand).
        * All the remaining bits are intepreted as the actual number of reader currently using the lock
        * (which mean the lock is limited to 2^29 concurrent readers but since it's a high number there
        * is no overflow safe guard to remain simple).
        */
        private int _rwlock;

        /* For performance sake, these numbers are manipulated via classic increment and
        * decrement operations and thus are (as hinted by MSDN) not meant to be precise
        */
        private int _numReadWaiters, _numUpgradeWaiters, _numWriteWaiters;
        private bool _disposed;

        /* This dictionary is instanciated per thread for all existing ReaderWriterLockSlim instance.
        * Each instance is defined by an internal integer id value used as a key in the dictionary.
        * to avoid keeping unneeded reference to the instance and getting in the way of the GC.
        * Since there is no LockCookie type here, all the useful per-thread infos concerning each
        * instance are kept here.
        */

        [ThreadStatic]
        private static Dictionary<int, ThreadLockState> _currentThreadState;

        /* Rwls tries to use this array as much as possible to quickly retrieve the thread-local
        * informations so that it ends up being only an array lookup. When the number of thread
        * using the instance goes past the length of the array, the code fallback to the normal
        * dictionary
        */
        private readonly ThreadLockState[] _fastStateCache;

        static ReaderWriterLockSlim()
        {
            _stopwatch = Stopwatch.StartNew();
            _idPool = int.MinValue;
        }

        public ReaderWriterLockSlim()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
        {
            _recursionPolicy = recursionPolicy;
            _noRecursion = recursionPolicy == LockRecursionPolicy.NoRecursion;
            // ---
            _id = Interlocked.Increment(ref _idPool);
            _fastStateCache = new ThreadLockState[64];
            _upgradableTaken = new AtomicBoolean();
            _upgradableEvent = new ManualResetEventSlim(true);
            _writerDoneEvent = new ManualResetEventSlim(true);
            _readerDoneEvent = new ManualResetEventSlim(true);
        }

        public void EnterReadLock()
        {
            TryEnterReadLock(-1);
        }

        public bool TryEnterReadLock(int millisecondsTimeout)
        {
            var dummy = false;
            return TryEnterReadLock(millisecondsTimeout, ref dummy);
        }

        private bool TryEnterReadLock(int millisecondsTimeout, ref bool success)
        {
            var ctstate = CurrentThreadState;

            if (CheckState(ctstate, millisecondsTimeout, LockState.Read))
            {
                ++ctstate.ReaderRecursiveCount;
                return true;
            }

            // This is downgrading from upgradable, no need for check since
            // we already have a sort-of read lock that's going to disappear
            // after user calls ExitUpgradeableReadLock.
            // Same idea when recursion is allowed and a write thread wants to
            // go for a Read too.
            if (ctstate.LockState.Has(LockState.Upgradable)
                || (!_noRecursion && ctstate.LockState.Has(LockState.Write)))
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                }
                finally
                {
                    Interlocked.Add(ref _rwlock, _rwRead);
                    ctstate.LockState |= LockState.Read;
                    ++ctstate.ReaderRecursiveCount;
                    success = true;
                }

                return true;
            }

            _numReadWaiters++;
            int val;
            var start = millisecondsTimeout == -1 ? 0 : _stopwatch.ElapsedMilliseconds;

            do
            {
                /* Check if a writer is present (RwWrite) or if there is someone waiting to
                * acquire a writer lock in the queue (RwWait | RwWaitUpgrade).
                */
                if ((_rwlock & (_rwWrite | _rwWait | _rwWaitUpgrade)) > 0)
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
                }
                finally
                {
                    if (((val = Interlocked.Add(ref _rwlock, _rwRead)) & (_rwWrite | _rwWait | _rwWaitUpgrade)) == 0)
                    {
                        /* If we are the first reader, reset the event to let other threads
                        * sleep correctly if they try to acquire write lock
                        */
                        if (val >> _rwReadBit == 1)
                        {
                            _readerDoneEvent.Reset();
                        }

                        ctstate.LockState ^= LockState.Read;
                        ++ctstate.ReaderRecursiveCount;
                        --_numReadWaiters;
                        success = true;
                    }
                    else
                    {
                        Interlocked.Add(ref _rwlock, -_rwRead);
                    }
                }
                if (success)
                {
                    return true;
                }

                _writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
            } while (millisecondsTimeout == -1 || (_stopwatch.ElapsedMilliseconds - start) < millisecondsTimeout);

            --_numReadWaiters;
            return false;
        }

        public bool TryEnterReadLock(TimeSpan timeout)
        {
            return TryEnterReadLock(CheckTimeout(timeout));
        }

        public void ExitReadLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                var ctstate = CurrentThreadState;

                if (!ctstate.LockState.Has(LockState.Read))
                {
                    throw new SynchronizationLockException("The current thread has not entered the lock in read mode");
                }

                if (--ctstate.ReaderRecursiveCount == 0)
                {
                    ctstate.LockState ^= LockState.Read;
                    if (Interlocked.Add(ref _rwlock, -_rwRead) >> _rwReadBit == 0)
                    {
                        _readerDoneEvent.Set();
                    }
                }
            }
        }

        public void EnterWriteLock()
        {
            TryEnterWriteLock(-1);
        }

        public bool TryEnterWriteLock(int millisecondsTimeout)
        {
            var ctstate = CurrentThreadState;

            if (CheckState(ctstate, millisecondsTimeout, LockState.Write))
            {
                ++ctstate.WriterRecursiveCount;
                return true;
            }

            ++_numWriteWaiters;
            var isUpgradable = ctstate.LockState.Has(LockState.Upgradable);
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
                if (isUpgradable && _rwlock >= _rwRead)
                {
                    try
                    {
                    }
                    finally
                    {
                        if (Interlocked.Add(ref _rwlock, _rwWaitUpgrade - _rwRead) >> _rwReadBit == 0)
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
                    var state = _rwlock;

                    if (state <= stateCheck)
                    {
                        try
                        {
                        }
                        finally
                        {
                            var toWrite = state + _rwWrite - (registered ? registration : 0);
                            if (Interlocked.CompareExchange(ref _rwlock, toWrite, state) == state)
                            {
                                _writerDoneEvent.Reset();
                                ctstate.LockState ^= LockState.Write;
                                ++ctstate.WriterRecursiveCount;
                                --_numWriteWaiters;
                                registered = false;
                                success = true;
                            }
                        }
                        if (success)
                        {
                            return true;
                        }
                    }

                    state = _rwlock;

                    // We register our interest in taking the Write lock (if upgradeable it's already done)
                    if (!isUpgradable)
                    {
                        while ((state & _rwWait) == 0)
                        {
                            try
                            {
                            }
                            finally
                            {
                                registered |= Interlocked.CompareExchange(ref _rwlock, state | _rwWait, state) == state;
                            }
                            if (registered)
                            {
                                break;
                            }

                            state = _rwlock;
                        }
                    }

                    // Before falling to sleep
                    do
                    {
                        if (_rwlock <= stateCheck)
                        {
                            break;
                        }

                        if ((_rwlock & _rwWrite) != 0)
                        {
                            _writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                        }
                        else if ((_rwlock >> _rwReadBit) > 0)
                        {
                            _readerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                        }
                    } while (millisecondsTimeout < 0 || (_stopwatch.ElapsedMilliseconds - start) < millisecondsTimeout);
                } while (millisecondsTimeout < 0 || (_stopwatch.ElapsedMilliseconds - start) < millisecondsTimeout);

                --_numWriteWaiters;
            }
            finally
            {
                if (registered)
                {
                    Interlocked.Add(ref _rwlock, isUpgradable ? -_rwWaitUpgrade : -_rwWait);
                }
            }

            return false;
        }

        public bool TryEnterWriteLock(TimeSpan timeout)
        {
            return TryEnterWriteLock(CheckTimeout(timeout));
        }

        public void ExitWriteLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                var ctstate = CurrentThreadState;

                if (!ctstate.LockState.Has(LockState.Write))
                {
                    throw new SynchronizationLockException("The current thread has not entered the lock in write mode");
                }

                if (--ctstate.WriterRecursiveCount == 0)
                {
                    var isUpgradable = ctstate.LockState.Has(LockState.Upgradable);
                    ctstate.LockState ^= LockState.Write;

                    var value = Interlocked.Add(ref _rwlock, isUpgradable ? _rwRead - _rwWrite : -_rwWrite);
                    _writerDoneEvent.Set();
                    if (isUpgradable && value >> _rwReadBit == 1)
                    {
                        _readerDoneEvent.Reset();
                    }
                }
            }
        }

        public void EnterUpgradeableReadLock()
        {
            TryEnterUpgradeableReadLock(-1);
        }

        //
        // Taking the Upgradable read lock is like taking a read lock
        // but we limit it to a single upgradable at a time.
        //
        public bool TryEnterUpgradeableReadLock(int millisecondsTimeout)
        {
            var ctstate = CurrentThreadState;

            if (CheckState(ctstate, millisecondsTimeout, LockState.Upgradable))
            {
                ++ctstate.UpgradeableRecursiveCount;
                return true;
            }

            if (ctstate.LockState.Has(LockState.Read))
            {
                throw new LockRecursionException("The current thread has already entered read mode");
            }

            ++_numUpgradeWaiters;
            var start = millisecondsTimeout == -1 ? 0 : _stopwatch.ElapsedMilliseconds;
            var taken = false;
            var success = false;

            // We first try to obtain the upgradeable right
            try
            {
                while (!_upgradableEvent.IsSet() || !taken)
                {
                    try
                    {
                    }
                    finally
                    {
                        taken = _upgradableTaken.TryRelaxedSet();
                    }
                    if (taken)
                    {
                        break;
                    }

                    if (millisecondsTimeout != -1 && (_stopwatch.ElapsedMilliseconds - start) > millisecondsTimeout)
                    {
                        --_numUpgradeWaiters;
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
                        ctstate.LockState |= LockState.Upgradable;
                        ctstate.LockState &= ~LockState.Read;
                        --ctstate.ReaderRecursiveCount;
                        ++ctstate.UpgradeableRecursiveCount;
                    }
                    else
                    {
                        _upgradableTaken.Value = false;
                        _upgradableEvent.Set();
                    }
                }

                --_numUpgradeWaiters;
            }
            catch (Exception ex)
            {
                GC.KeepAlive(ex);
                // An async exception occured, if we had taken the upgradable mode, release it
                _upgradableTaken.Value &= !taken || success;
            }

            return success;
        }

        public bool TryEnterUpgradeableReadLock(TimeSpan timeout)
        {
            return TryEnterUpgradeableReadLock(CheckTimeout(timeout));
        }

        public void ExitUpgradeableReadLock()
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                var ctstate = CurrentThreadState;

                if (!ctstate.LockState.Has(LockState.Upgradable | LockState.Read))
                {
                    throw new SynchronizationLockException("The current thread has not entered the lock in upgradable mode");
                }

                if (--ctstate.UpgradeableRecursiveCount == 0)
                {
                    _upgradableTaken.Value = false;
                    _upgradableEvent.Set();

                    ctstate.LockState &= ~LockState.Upgradable;
                    if (Interlocked.Add(ref _rwlock, -_rwRead) >> _rwReadBit == 0)
                    {
                        _readerDoneEvent.Set();
                    }
                }
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

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            if (disposing)
            {
                if (IsReadLockHeld || IsUpgradeableReadLockHeld || IsWriteLockHeld)
                {
                    throw new SynchronizationLockException("The lock is being disposed while still being used");
                }
                _upgradableEvent.Dispose();
                _writerDoneEvent.Dispose();
                _readerDoneEvent.Dispose();
                _disposed = true;
            }
        }

        public bool IsReadLockHeld
        {
            get { return _rwlock >= _rwRead && CurrentThreadState.LockState.Has(LockState.Read); }
        }

        public bool IsWriteLockHeld
        {
            get { return (_rwlock & _rwWrite) > 0 && CurrentThreadState.LockState.Has(LockState.Write); }
        }

        public bool IsUpgradeableReadLockHeld
        {
            get { return _upgradableTaken.Value && CurrentThreadState.LockState.Has(LockState.Upgradable); }
        }

        public int CurrentReadCount
        {
            get { return (_rwlock >> _rwReadBit) - (_upgradableTaken.Value ? 1 : 0); }
        }

        public int RecursiveReadCount
        {
            get { return CurrentThreadState.ReaderRecursiveCount; }
        }

        public int RecursiveUpgradeCount
        {
            get { return CurrentThreadState.UpgradeableRecursiveCount; }
        }

        public int RecursiveWriteCount
        {
            get { return CurrentThreadState.WriterRecursiveCount; }
        }

        public int WaitingReadCount
        {
            get { return _numReadWaiters; }
        }

        public int WaitingUpgradeCount
        {
            get { return _numUpgradeWaiters; }
        }

        public int WaitingWriteCount
        {
            get { return _numWriteWaiters; }
        }

        public LockRecursionPolicy RecursionPolicy
        {
            get { return _recursionPolicy; }
        }

        private ThreadLockState CurrentThreadState
        {
            get
            {
                var tid = Thread.CurrentThread.ManagedThreadId;

                return tid < _fastStateCache.Length ? _fastStateCache[tid] ?? (_fastStateCache[tid] = new ThreadLockState()) : GetGlobalThreadState();
            }
        }

        private ThreadLockState GetGlobalThreadState()
        {
            if (_currentThreadState == null)
            {
                Interlocked.CompareExchange(ref _currentThreadState, new Dictionary<int, ThreadLockState>(), null);
            }

            if (!_currentThreadState.TryGetValue(_id, out ThreadLockState state))
            {
                _currentThreadState[_id] = state = new ThreadLockState();
            }

            return state;
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
            var ctstate = state.LockState;

            if (ctstate != LockState.None && _noRecursion && (!ctstate.Has(LockState.Upgradable) || validState == LockState.Upgradable))
            {
                throw new LockRecursionException("The current thread has already a lock and recursion isn't supported");
            }

            if (_noRecursion)
            {
                return false;
            }

            // If we already had right lock state, just return
            if (ctstate.Has(validState))
            {
                return true;
            }

            // In read mode you can just enter Read recursively
            if (ctstate == LockState.Read)
            {
                throw new LockRecursionException();
            }

            return false;
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
    }
}

#endif