using System;
using System.Threading;

namespace Theraot.Threading
{
    internal interface IReadWriteLock : IDisposable
    {
        bool CurrentThreadIsReader { get; }

        bool CurrentThreadIsWriter { get; }

        bool HasReader { get; }

        bool HasWriter { get; }

        IDisposable EnterRead();

        IDisposable EnterWrite();

        bool TryEnterRead(out IDisposable engagement);

        bool TryEnterWrite(out IDisposable engagement);
    }

    public sealed class ReadWriteLock : Theraot.Threading.IReadWriteLock, IDisposable
    {
        private readonly IReadWriteLock _wrapped;

        public ReadWriteLock()
        {
            _wrapped = new NoReentrantReadWriteLock();
        }

        public ReadWriteLock(bool reentrant)
        {
            if (reentrant)
            {
                _wrapped = new ReentrantReadWriteLock();
            }
            else
            {
                _wrapped = new NoReentrantReadWriteLock();
            }
        }

        public bool CurrentThreadIsReader
        {
            get
            {
                return _wrapped.CurrentThreadIsReader;
            }
        }

        public bool CurrentThreadIsWriter
        {
            get
            {
                return _wrapped.CurrentThreadIsWriter;
            }
        }

        public bool HasReader
        {
            get
            {
                return _wrapped.HasReader;
            }
        }

        public bool HasWriter
        {
            get
            {
                return _wrapped.HasWriter;
            }
        }

        public void Dispose()
        {
            _wrapped.Dispose();
        }

        public IDisposable EnterRead()
        {
            return _wrapped.EnterRead();
        }

        public IDisposable EnterWrite()
        {
            return _wrapped.EnterWrite();
        }

        public bool TryEnterRead(out IDisposable engagement)
        {
            return _wrapped.TryEnterRead(out engagement);
        }

        public bool TryEnterWrite(out IDisposable engagement)
        {
            return _wrapped.TryEnterWrite(out engagement);
        }
    }

    internal sealed partial class NoReentrantReadWriteLock : Theraot.Threading.IReadWriteLock
    {
        private int _edge;
        private ManualResetEventSlim _freeToRead = new ManualResetEventSlim(false);
        private ManualResetEventSlim _freeToWrite = new ManualResetEventSlim(false);
        private int _master;
        private Thread _ownerThread;
        private int _readCount;
        private int _writeCount;

        public bool CurrentThreadIsReader
        {
            get
            {
                throw new NotSupportedException("Only a ReentratReadWriteLock keeps tracks of which thread is a reader.");
            }
        }

        public bool CurrentThreadIsWriter
        {
            get
            {
                return Thread.CurrentThread == _ownerThread;
            }
        }

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

        public IDisposable EnterRead()
        {
            WaitCanRead();
            return DisposableAkin.Create(DoneRead);
        }

        public IDisposable EnterWrite()
        {
            WaitCanWrite();
            return DisposableAkin.Create(DoneWrite);
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
            if (!CanWrite())
            {
                return false;
            }
            engagement = DisposableAkin.Create(DoneWrite);
            return true;
        }

        private bool CanRead()
        {
            if (Thread.CurrentThread == ThreadingHelper.VolatileRead(ref _ownerThread))
            {
                Interlocked.Increment(ref _readCount);
                return true;
            }
            else
            {
                if (Interlocked.CompareExchange(ref _master, 1, 0) >= 0)
                {
                    _freeToWrite.Reset();
                    Interlocked.Increment(ref _readCount);
                    return true;
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
            }
            else
            {
                if (Thread.VolatileRead(ref _master) < 0)
                {
                    if (Interlocked.Decrement(ref _readCount) <= Thread.VolatileRead(ref _edge))
                    {
                        Thread.VolatileWrite(ref _master, 0);
                        _freeToWrite.Set();
                    }
                }
                else
                {
                    Interlocked.Decrement(ref _readCount);
                }
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
    }

    internal sealed partial class ReentrantReadWriteLock : Theraot.Threading.IReadWriteLock
    {
        private NoTrackingThreadLocal<int> _currentReadingCount = new NoTrackingThreadLocal<int>(() => 0);
        private int _edge;
        private ManualResetEventSlim _freeToRead = new ManualResetEventSlim(false);
        private ManualResetEventSlim _freeToWrite = new ManualResetEventSlim(false);
        private int _master;
        private Thread _ownerThread;
        private int _readCount;
        private int _writeCount;

        public bool CurrentThreadIsReader
        {
            get
            {
                return _currentReadingCount.IsValueCreated && _currentReadingCount.Value > 0;
            }
        }

        public bool CurrentThreadIsWriter
        {
            get
            {
                return Thread.CurrentThread == _ownerThread;
            }
        }

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
}