#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class NoReentrantReadWriteLock : IReadWriteLock
    {
        private int _edge;
        private ManualResetEventSlim _freeToRead = new ManualResetEventSlim(false); // Disposed
        private ManualResetEventSlim _freeToWrite = new ManualResetEventSlim(false); // Disposed
        private int _master;
        private Thread _ownerThread;
        private int _readCount;
        private int _writeCount;

        public bool HasReader
        {
            get { return _readCount > 0; }
        }

        public bool HasWriter
        {
            get { return _ownerThread != null; }
        }

        public bool IsCurrentThreadReader
        {
            get { throw new NotSupportedException("Only a ReentratReadWriteLock keeps tracks of which thread is a reader."); }
        }

        public bool IsCurrentThreadWriter
        {
            get { return Thread.CurrentThread == _ownerThread; }
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
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
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
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
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
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
            {
                Interlocked.Decrement(ref _readCount);
            }
            else
            {
                if (Volatile.Read(ref _master) < 0)
                {
                    if (Interlocked.Decrement(ref _readCount) <= Volatile.Read(ref _edge))
                    {
                        Volatile.Write(ref _master, 0);
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
                Volatile.Write(ref _master, 0);
                Volatile.Write(ref _ownerThread, null);
                _freeToRead.Set();
                _freeToWrite.Set();
            }
        }

        private void WaitCanRead()
        {
            if (Thread.CurrentThread != Volatile.Read(ref _ownerThread))
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
            if (Thread.CurrentThread != Volatile.Read(ref _ownerThread))
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
}

#endif