#if FAT

using System;
using System.Threading;

namespace Theraot.Threading
{
    internal sealed partial class ReentrantReadWriteLock : IReadWriteLock
    {
        private TrackingThreadLocal<int> _currentReadingCount = new TrackingThreadLocal<int>(() => 0); // Disposed
        private int _edge;
        private ManualResetEventSlim _freeToRead = new ManualResetEventSlim(false); // Disposed
        private ManualResetEventSlim _freeToWrite = new ManualResetEventSlim(false); // Disposed
        private int _status;
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
            get { return _currentReadingCount.IsValueCreated && _currentReadingCount.Value > 0; }
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
            if (_currentReadingCount.Value > 0)
            {
                if (WaitUpgrade())
                {
                    return DisposableAkin.Create(DoneUpgrade);
                }
                throw new InvalidOperationException();
            }
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
            if (_currentReadingCount.Value > 0)
            {
                if (CanUpgrade())
                {
                    engagement = DisposableAkin.Create(DoneUpgrade);
                    return true;
                }
                return false;
            }
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
                _currentReadingCount.Value++;
                return true;
            }
            var status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.ReadMode, (int)Status.Free);
            if (status == Status.ReadMode || status == Status.Free)
            {
                _freeToWrite.Reset();
                Interlocked.Increment(ref _readCount);
                _currentReadingCount.Value++;
                return true;
            }
            return false;
        }

        private bool CanUpgrade()
        {
            if (Thread.CurrentThread == Volatile.Read(ref _ownerThread))
            {
                Interlocked.Increment(ref _writeCount);
                return true;
            }
            var status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.WriteRequested, (int)Status.ReadMode);
            if (status == Status.ReadMode)
            {
                _freeToRead.Reset();
                // --
                if (Volatile.Read(ref _readCount) <= _currentReadingCount.Value && Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null) == null)
                {
                    Volatile.Write(ref _status, (int)Status.WriteMode);
                    Interlocked.Increment(ref _writeCount);
                    return true;
                }
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
                // --
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
                _currentReadingCount.Value--;
            }
            else
            {
                var status = (Status)Volatile.Read(ref _status);
                if (status == Status.WriteMode || status == Status.WriteRequested)
                {
                    _currentReadingCount.Value--;
                    if (Interlocked.Decrement(ref _readCount) <= Volatile.Read(ref _edge))
                    {
                        Volatile.Write(ref _status, (int)Status.Free);
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
                Volatile.Write(ref _edge, 0);
                Volatile.Write(ref _ownerThread, null);
                _freeToRead.Set();
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
                            _currentReadingCount.Value++;
                            return;

                        default:
                            // Should not happen
                            continue;
                    }
                }
            }
        }

        private void WaitCanWrite()
        {
            if (Thread.CurrentThread != Volatile.Read(ref _ownerThread))
            {
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
                            continue;
                    }
                }
            }
        }

        private bool WaitUpgrade()
        {
            var owner = Volatile.Read(ref _ownerThread);
            if (owner == null || owner == Thread.CurrentThread)
            {
                while (true)
                {
                    var status = (Status)Volatile.Read(ref _status);
                    switch (status)
                    {
                        case Status.WriteRequested:
                            // Write mode already requested
                            // We are going to steal it
                            // Reserve the lock - so no other writer can take it
                            owner = Interlocked.CompareExchange(ref _ownerThread, Thread.CurrentThread, null);
                            if (owner == null || owner == Thread.CurrentThread)
                            {
                                // Set the edge
                                Volatile.Write(ref _edge, _currentReadingCount.Value);
                            }
                            else
                            {
                                // It was reserved by another thread - abort mission
                                return false;
                            }
                            if (Volatile.Read(ref _readCount) > Volatile.Read(ref _edge))
                            {
                                // We still need every other reader to finish
                                _freeToWrite.Wait();
                                // Status must have changed
                            }
                            else
                            {
                                // None to wait
                                // Change to write mode
                                Interlocked.CompareExchange(ref _status, (int)Status.WriteMode, (int)Status.WriteRequested);
                            }
                            break;

                        case Status.WriteMode:
                            // There is a writer
                            // Abort mission
                            _freeToRead.Reset();
                            Interlocked.Increment(ref _writeCount);
                            return true;

                        case Status.Free:
                            // Free to proceed UPGRADE
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
                                    return true;
                                }
                            }
                            // Write mode was taken by another thread
                            break;

                        case Status.ReadMode:
                            // There are readers currently - of course, current thread is a reader
                            // Requesting write mode
                            status = (Status)Interlocked.CompareExchange(ref _status, (int)Status.WriteRequested, (int)Status.ReadMode);
                            if (status == Status.ReadMode)
                            {
                                // Write has been requested, no more readers should enter
                                _freeToRead.Reset();
                            }
                            break;

                        default:
                            // Should not happen
                            continue;
                    }
                }
            }
            return false;
        }
    }
}

#endif