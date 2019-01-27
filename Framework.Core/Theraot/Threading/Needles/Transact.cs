#if FAT
using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        [ThreadStatic]
        private static Transact _currentTransaction;

        private readonly Transact _parentTransaction;
        private readonly SafeDictionary<IResource, object> _readLog;
        private readonly Thread _thread;
        private readonly SafeDictionary<IResource, object> _writeLog;
        private LockSlot<Thread> _lockSlot;

        public Transact()
        {
            _writeLog = new SafeDictionary<IResource, object>();
            _readLog = new SafeDictionary<IResource, object>();
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
            _thread = Thread.CurrentThread;
        }

        public static Transact CurrentTransaction => _currentTransaction;

        public bool IsRoot => _parentTransaction == null;

        private static LockContext<Thread> Context { get; } = new LockContext<Thread>(512);

        public static Needle<T> CreateNeedle<T>(T value)
        {
            return new Needle<T>(value);
        }

        public bool Commit()
        {
            if (_currentTransaction == this)
            {
                ThreadingHelper.MemoryBarrier();
                try
                {
                    if (!CheckValue())
                    {
                        //the resources has been modified by another thread
                        return false;
                    }
                    try
                    {
                        ThreadingHelper.SpinWaitUntil(() => Context.ClaimSlot(out _lockSlot));
                        _lockSlot.Value = Thread.CurrentThread;
                        if (!Capture())
                        {
                            //Nothing to commit
                            return true;
                        }
                        ThreadingHelper.MemoryBarrier();
                        if (!CheckCapture() || !CheckValue())
                        {
                            //the resources has been claimed by another thread
                            return false;
                        }
                        var written = false;
                        foreach (var resource in _writeLog)
                        {
                            if (resource.Key.Commit())
                            {
                                written = true;
                            }
                            else
                            {
                                //unexpected
                                if (written)
                                {
                                    // TODO - the transaction was partially written, this should not be possible.
                                    System.Diagnostics.Debug.Fail("unexpected - partially commited transaction");
                                }
                                return false;
                            }
                        }
                        return true;
                    }
                    finally
                    {
                        if (_lockSlot != null)
                        {
                            _lockSlot.Close();
                            _lockSlot = null;
                        }
                    }
                }
                finally
                {
                    Release(false);
                }
            }
            throw new InvalidOperationException("Cannot commit a non-current transaction.");
        }

        public void Rollback()
        {
            if (Thread.CurrentThread == _thread)
            {
                Release(false);
            }
            else
            {
                throw new InvalidOperationException("Unable to rollback a transaction that belongs to another thread.");
            }
        }

        private bool Capture()
        {
            var result = false;
            foreach (var resource1 in _writeLog)
            {
                resource1.Key.Capture();
                result = true;
            }
            if (result)
            {
                foreach (var resource2 in _readLog)
                {
                    resource2.Key.Capture();
                }
            }
            return result;
        }

        private bool CheckCapture()
        {
            // Keep foreach loop
            foreach (var resource in _readLog)
            {
                if (!resource.Key.CheckCapture())
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckValue()
        {
            // Keep foreach loop
            foreach (var resource in _readLog)
            {
                if (!resource.Key.CheckValue())
                {
                    return false;
                }
            }
            return true;
        }

        private void Release(bool dispose)
        {
            for (var currentTransaction = _currentTransaction; currentTransaction != null && currentTransaction != this; currentTransaction = currentTransaction._parentTransaction)
            {
                if (dispose)
                {
                    currentTransaction.Dispose();
                }
                else
                {
                    currentTransaction.Release();
                }
            }
            Release();
            if (dispose)
            {
                _currentTransaction = _parentTransaction;
            }
        }

        private void Release()
        {
            foreach (var resource in _readLog)
            {
                resource.Key.Release();
            }
            foreach (var resource in _writeLog)
            {
                resource.Key.Release();
            }
            _readLog.Clear();
            _writeLog.Clear();
        }
    }
}

#endif