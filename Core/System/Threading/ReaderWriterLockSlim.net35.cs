#if NET20 || NET30

using System.Collections.Generic;
using System.Security.Permissions;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        private const int RwReadBit = 3;

        /* These values are used to manipulate the corresponding flags in _rwlock field
        */
        private const int RwWait = 1;
        private const int RwWaitUpgrade = 2;
        private const int RwWrite = 4;
        private const int RwRead = 8;

        /* Some explanations: this field is the central point of the lock and keep track of all the requests
        * that are being made. The 3 lowest bits are used as flag to track "destructive" lock entries
        * (i.e attempting to take the write lock with or without having acquired an upgradeable lock beforehand).
        * All the remaining bits are intepreted as the actual number of reader currently using the lock
        * (which mean the lock is limited to 2^29 concurrent readers but since it's a high number there
        * is no overflow safe guard to remain simple).
        */
        private int _rwlock;

        private readonly LockRecursionPolicy _recursionPolicy;
        private readonly bool _noRecursion;

        private readonly AtomicBoolean _upgradableTaken = new AtomicBoolean();
        private

                /* These events are just here for the sake of having a CPU-efficient sleep
                * when the wait for acquiring the lock is too long
                */
                ManualResetEventSlim upgradableEvent = new ManualResetEventSlim(true);
        private ManualResetEventSlim writerDoneEvent = new ManualResetEventSlim(true);
        private ManualResetEventSlim readerDoneEvent = new ManualResetEventSlim(true);

        // This Stopwatch instance is used for all threads since .Elapsed is thread-safe
        private readonly static Stopwatch sw = Stopwatch.StartNew();
        private

                /* For performance sake, these numbers are manipulated via classic increment and
                * decrement operations and thus are (as hinted by MSDN) not meant to be precise
                */
                int numReadWaiters, numUpgradeWaiters, numWriteWaiters;
        private bool disposed;

        private static int idPool = int.MinValue;
        private readonly int id = Interlocked.Increment(ref idPool);

        /* This dictionary is instanciated per thread for all existing ReaderWriterLockSlim instance.
        * Each instance is defined by an internal integer id value used as a key in the dictionary.
        * to avoid keeping unneeded reference to the instance and getting in the way of the GC.
        * Since there is no LockCookie type here, all the useful per-thread infos concerning each
        * instance are kept here.
        */
        [ThreadStatic]
        private static Dictionary<int, ThreadLockState> currentThreadState;
        private

                /* Rwls tries to use this array as much as possible to quickly retrieve the thread-local
                * informations so that it ends up being only an array lookup. When the number of thread
                * using the instance goes past the length of the array, the code fallback to the normal
                * dictionary
                */
                ThreadLockState[] fastStateCache = new ThreadLockState[64];

        public ReaderWriterLockSlim()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        public ReaderWriterLockSlim(LockRecursionPolicy recursionPolicy)
        {
            this._recursionPolicy = recursionPolicy;
            this._noRecursion = recursionPolicy == LockRecursionPolicy.NoRecursion;
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
                    Interlocked.Add(ref _rwlock, RwRead);
                    ctstate.LockState |= LockState.Read;
                    ++ctstate.ReaderRecursiveCount;
                    success = true;
                }

                return true;
            }

            ++numReadWaiters;
            var val = 0;
            var start = millisecondsTimeout == -1 ? 0 : sw.ElapsedMilliseconds;

            do
            {
                /* Check if a writer is present (RwWrite) or if there is someone waiting to
                * acquire a writer lock in the queue (RwWait | RwWaitUpgrade).
                */
                if ((_rwlock & (RwWrite | RwWait | RwWaitUpgrade)) > 0)
                {
                    writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
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
                    if (((val = Interlocked.Add(ref _rwlock, RwRead)) & (RwWrite | RwWait | RwWaitUpgrade)) == 0)
                    {
                        /* If we are the first reader, reset the event to let other threads
                        * sleep correctly if they try to acquire write lock
                        */
                        if (val >> RwReadBit == 1)
                            readerDoneEvent.Reset();

                        ctstate.LockState ^= LockState.Read;
                        ++ctstate.ReaderRecursiveCount;
                        --numReadWaiters;
                        success = true;
                    }
                    else
                    {
                        Interlocked.Add(ref _rwlock, -RwRead);
                    }
                }
                if (success)
                    return true;

                writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
            } while (millisecondsTimeout == -1 || (sw.ElapsedMilliseconds - start) < millisecondsTimeout);

            --numReadWaiters;
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
                    throw new SynchronizationLockException("The current thread has not entered the lock in read mode");

                if (--ctstate.ReaderRecursiveCount == 0)
                {
                    ctstate.LockState ^= LockState.Read;
                    if (Interlocked.Add(ref _rwlock, -RwRead) >> RwReadBit == 0)
                        readerDoneEvent.Set();
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

            ++numWriteWaiters;
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
                if (isUpgradable && _rwlock >= RwRead)
                {
                    try
                    {
                    }
                    finally
                    {
                        if (Interlocked.Add(ref _rwlock, RwWaitUpgrade - RwRead) >> RwReadBit == 0)
                            readerDoneEvent.Set();
                        registered = true;
                    }
                }

                var stateCheck = isUpgradable ? RwWaitUpgrade + RwWait : RwWait;
                var start = millisecondsTimeout == -1 ? 0 : sw.ElapsedMilliseconds;
                var registration = isUpgradable ? RwWaitUpgrade : RwWait;

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
                            var toWrite = state + RwWrite - (registered ? registration : 0);
                            if (Interlocked.CompareExchange(ref _rwlock, toWrite, state) == state)
                            {
                                writerDoneEvent.Reset();
                                ctstate.LockState ^= LockState.Write;
                                ++ctstate.WriterRecursiveCount;
                                --numWriteWaiters;
                                registered = false;
                                success = true;
                            }
                        }
                        if (success)
                            return true;
                    }

                    state = _rwlock;

                    // We register our interest in taking the Write lock (if upgradeable it's already done)
                    if (!isUpgradable)
                    {
                        while ((state & RwWait) == 0)
                        {
                            try
                            {
                            }
                            finally
                            {
                                if (Interlocked.CompareExchange(ref _rwlock, state | RwWait, state) == state)
                                    registered = true;
                            }
                            if (registered)
                                break;
                            state = _rwlock;
                        }
                    }

// Before falling to sleep
                    do
                    {
                        if (_rwlock <= stateCheck)
                            break;
                        if ((_rwlock & RwWrite) != 0)
                            writerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                        else if ((_rwlock >> RwReadBit) > 0)
                            readerDoneEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                    } while (millisecondsTimeout < 0 || (sw.ElapsedMilliseconds - start) < millisecondsTimeout);
                } while (millisecondsTimeout < 0 || (sw.ElapsedMilliseconds - start) < millisecondsTimeout);

                --numWriteWaiters;
            }
            finally
            {
                if (registered)
                    Interlocked.Add(ref _rwlock, isUpgradable ? -RwWaitUpgrade : -RwWait);
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
                    throw new SynchronizationLockException("The current thread has not entered the lock in write mode");

                if (--ctstate.WriterRecursiveCount == 0)
                {
                    var isUpgradable = ctstate.LockState.Has(LockState.Upgradable);
                    ctstate.LockState ^= LockState.Write;

                    var value = Interlocked.Add(ref _rwlock, isUpgradable ? RwRead - RwWrite : -RwWrite);
                    writerDoneEvent.Set();
                    if (isUpgradable && value >> RwReadBit == 1)
                        readerDoneEvent.Reset();
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
                throw new LockRecursionException("The current thread has already entered read mode");

            ++numUpgradeWaiters;
            var start = millisecondsTimeout == -1 ? 0 : sw.ElapsedMilliseconds;
            var taken = false;
            var success = false;

            // We first try to obtain the upgradeable right
            try
            {
                while (!upgradableEvent.IsSet() || !taken)
                {
                    try
                    {
                    }
                    finally
                    {
                        taken = _upgradableTaken.TryRelaxedSet();
                    }
                    if (taken)
                        break;
                    if (millisecondsTimeout != -1 && (sw.ElapsedMilliseconds - start) > millisecondsTimeout)
                    {
                        --numUpgradeWaiters;
                        return false;
                    }

                    upgradableEvent.Wait(ComputeTimeout(millisecondsTimeout, start));
                }

                upgradableEvent.Reset();

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
                        upgradableEvent.Set();
                    }
                }

                --numUpgradeWaiters;
            }
            catch
            {
                // An async exception occured, if we had taken the upgradable mode, release it
                if (taken && !success)
                {
                    _upgradableTaken.Value = false;
                }
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
                    throw new SynchronizationLockException("The current thread has not entered the lock in upgradable mode");

                if (--ctstate.UpgradeableRecursiveCount == 0)
                {
                    _upgradableTaken.Value = false;
                    upgradableEvent.Set();

                    ctstate.LockState &= ~LockState.Upgradable;
                    if (Interlocked.Add(ref _rwlock, -RwRead) >> RwReadBit == 0)
                        readerDoneEvent.Set();
                }
            }

        }

        [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Microsoft's Design")]
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                
                if (IsReadLockHeld || IsUpgradeableReadLockHeld || IsWriteLockHeld)
                {
                    throw new SynchronizationLockException("The lock is being disposed while still being used");
                }
                upgradableEvent.Dispose();
                writerDoneEvent.Dispose();
                readerDoneEvent.Dispose();
                disposed = true;
            }
        }

        public bool IsReadLockHeld
        {
            get
            {
                return _rwlock >= RwRead && CurrentThreadState.LockState.Has(LockState.Read);
            }
        }

        public bool IsWriteLockHeld
        {
            get
            {
                return (_rwlock & RwWrite) > 0 && CurrentThreadState.LockState.Has(LockState.Write);
            }
        }

        public bool IsUpgradeableReadLockHeld
        {
            get
            {
                return _upgradableTaken.Value && CurrentThreadState.LockState.Has(LockState.Upgradable);
            }
        }

        public int CurrentReadCount
        {
            get
            {
                return (_rwlock >> RwReadBit) - (_upgradableTaken.Value ? 1 : 0);
            }
        }

        public int RecursiveReadCount
        {
            get
            {
                return CurrentThreadState.ReaderRecursiveCount;
            }
        }

        public int RecursiveUpgradeCount
        {
            get
            {
                return CurrentThreadState.UpgradeableRecursiveCount;
            }
        }

        public int RecursiveWriteCount
        {
            get
            {
                return CurrentThreadState.WriterRecursiveCount;
            }
        }

        public int WaitingReadCount
        {
            get
            {
                return numReadWaiters;
            }
        }

        public int WaitingUpgradeCount
        {
            get
            {
                return numUpgradeWaiters;
            }
        }

        public int WaitingWriteCount
        {
            get
            {
                return numWriteWaiters;
            }
        }

        public LockRecursionPolicy RecursionPolicy
        {
            get
            {
                return _recursionPolicy;
            }
        }

        private ThreadLockState CurrentThreadState
        {
            get
            {
                var tid = Thread.CurrentThread.ManagedThreadId;

                return tid < fastStateCache.Length ? fastStateCache[tid] ?? (fastStateCache[tid] = new ThreadLockState()) : GetGlobalThreadState();
            }
        }

        private ThreadLockState GetGlobalThreadState()
        {
            if (currentThreadState == null)
                Interlocked.CompareExchange(ref currentThreadState, new Dictionary<int, ThreadLockState>(), null);

            ThreadLockState state;
            if (!currentThreadState.TryGetValue(id, out state))
                currentThreadState[id] = state = new ThreadLockState();

            return state;
        }

        private bool CheckState(ThreadLockState state, int millisecondsTimeout, LockState validState)
        {
            if (disposed)
                throw new ObjectDisposedException("ReaderWriterLockSlim");

            if (millisecondsTimeout < -1)
                throw new ArgumentOutOfRangeException("millisecondsTimeout");

            // Detect and prevent recursion
            var ctstate = state.LockState;

            if (ctstate != LockState.None && _noRecursion && (!ctstate.Has(LockState.Upgradable) || validState == LockState.Upgradable))
                throw new LockRecursionException("The current thread has already a lock and recursion isn't supported");

            if (_noRecursion)
                return false;

            // If we already had right lock state, just return
            if (ctstate.Has(validState))
                return true;

            // In read mode you can just enter Read recursively
            if (ctstate == LockState.Read)
                throw new LockRecursionException();

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
                throw new ArgumentOutOfRangeException("timeout");
            }
        }

        private static int ComputeTimeout(int millisecondsTimeout, long start)
        {
            return millisecondsTimeout == -1 ? -1 : (int)Math.Max(sw.ElapsedMilliseconds - start - millisecondsTimeout, 1);
        }
    }
}

#endif