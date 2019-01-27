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
        private int _status;
        private Thread _ownerThread;
        private int _readCount;
        private int _writeCount;

        public bool HasReader => _readCount > 0;

        public bool HasWriter => _ownerThread != null;

        public bool IsCurrentThreadReader => throw new NotSupportedException("Only a ReentrantReadWriteLock keeps tracks of which thread is a reader.");

        public bool IsCurrentThreadWriter => Thread.CurrentThread == _ownerThread;

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
            var status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.ReadMode, (int)Status.Free);
            if (status == Status.ReadMode || status == Status.Free)
            {
                _freeToWrite.Reset();
                Interlocked.Increment(ref _readCount);
                return true;
            }
            return false;
        }

        private bool CanWrite()
        {
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
            {
                Interlocked.Increment(ref _writeCount);
                return true;
            }
            var status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.WriteMode, (int)Status.Free);
            if (status == Status.Free)
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

        private void DoneRead()
        {
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
            {
                Interlocked.Decrement(ref _readCount);
            }
            else
            {
                var status = (Status)Volatile.Read(ref _status);
                if (status == Status.WriteMode || status == Status.WriteRequested)
                {
                    if (Interlocked.Decrement(ref _readCount) <= Volatile.Read(ref _edge))
                    {
                        Volatile.Write(ref _status, (int)Status.Free);
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
                Volatile.Write(ref _status, (int)Status.Free);
                Volatile.Write(ref _ownerThread, null);
                _freeToRead.Set();
                _freeToWrite.Set();
            }
        }

        private void WaitCanRead()
        {
            if (Thread.CurrentThread != Volatile.Read(ref _ownerThread))
            {
                var spinWait = new SpinWait();
                while (true)
                {
                    var status = (Status)Volatile.Read(ref _status);
                    switch (status)
                    {
                        case Status.WriteRequested:
                            // Write mode already requested
                            goto case Status.WriteMode;

                        case Status.WriteMode:
                            // There is a writer
                            // Go to wait
                            _freeToRead.Wait();
                            // Status must have changed
                            break;

                        case Status.Free:
                            // Free to proceed READ
                            // GO!
                            // Change to read mode
                            status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.ReadMode, (int)Status.Free);
                            if (status == Status.Free)
                            {
                                // Did change to read mode, no writer should enter
                                _freeToWrite.Reset();
                            }
                            break;

                        case Status.ReadMode:
                            // There are readers currently
                            // GO!
                            Interlocked.Increment(ref _readCount);
                            return;

                        default:
                            // Should not happen
                            break;
                    }
                    spinWait.SpinOnce();
                }
            }
        }

        private void WaitCanWrite()
        {
            if (Thread.CurrentThread != Volatile.Read(ref _ownerThread))
            {
                var spinWait = new SpinWait();
                while (true)
                {
                    var status = (Status)Volatile.Read(ref _status);
                    switch (status)
                    {
                        case Status.WriteRequested:
                            // Write mode already requested
                            goto case Status.WriteMode;

                        case Status.WriteMode:
                            // There is another writer
                            // Go to wait
                            _freeToWrite.Wait();
                            // Status must have changed
                            break;

                        case Status.Free:
                            // Free to proceed WRITE
                            // GO!
                            // Change to write mode
                            status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.WriteMode, (int)Status.Free);
                            if (status == Status.Free)
                            {
                                // Did change to write mode, no more readers should enter
                                _freeToRead.Reset();
                                // Take the lock
                                if (Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null)
                                {
                                    // Success
                                    Interlocked.Increment(ref _writeCount);
                                    return;
                                }
                            }
                            // Write mode was taken by another thread
                            break;

                        case Status.ReadMode:
                            // There are readers currently
                            // Requesting write mode
                            status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.WriteRequested, (int)Status.ReadMode);
                            if (status == Status.ReadMode)
                            {
                                // We requested write mode, no more readers should enter
                                _freeToRead.Reset();
                            }
                            break;

                        default:
                            // Should not happen
                            break;
                    }
                    spinWait.SpinOnce();
                }
            }
        }
    }
}

#endif