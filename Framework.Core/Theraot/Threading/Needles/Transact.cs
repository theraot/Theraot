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

        internal readonly Transact ParentTransaction;
        internal readonly ThreadSafeDictionary<IResource, object> ReadLog;
        private readonly Thread _thread;
        internal readonly ThreadSafeDictionary<IResource, object> WriteLog;
        internal LockSlot<Thread> LockSlot;

        public Transact()
        {
            WriteLog = new ThreadSafeDictionary<IResource, object>();
            ReadLog = new ThreadSafeDictionary<IResource, object>();
            ParentTransaction = _currentTransaction;
            _currentTransaction = this;
            _thread = Thread.CurrentThread;
        }

        public static Transact CurrentTransaction => _currentTransaction;

        public bool IsRoot => ParentTransaction == null;

        internal static LockContext<Thread> Context { get; } = new LockContext<Thread>(512);

        public static TransactNeedle<T> CreateNeedle<T>(T value)
        {
            return new TransactNeedle<T>(value);
        }

        public bool Commit()
        {
            if (_currentTransaction != this)
            {
                throw new InvalidOperationException("Cannot commit a non-current transaction.");
            }

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
                    ThreadingHelper.SpinWaitUntil(() => Context.ClaimSlot(out LockSlot));
                    LockSlot.Value = Thread.CurrentThread;
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
                    foreach (var resource in WriteLog)
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
                                System.Diagnostics.Debug.Fail("unexpected - partially committed transaction");
                            }
                            return false;
                        }
                    }
                    return true;
                }
                finally
                {
                    if (LockSlot != null)
                    {
                        LockSlot.Close();
                        LockSlot = null;
                    }
                }
            }
            finally
            {
                Release(false);
            }
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
            foreach (var resource1 in WriteLog)
            {
                resource1.Key.Capture();
                result = true;
            }

            if (!result)
            {
                return result;
            }

            foreach (var resource2 in ReadLog)
            {
                resource2.Key.Capture();
            }
            return result;
        }

        private bool CheckCapture()
        {
            // Keep foreach loop
            foreach (var resource in ReadLog)
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
            foreach (var resource in ReadLog)
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
            for (var currentTransaction = _currentTransaction; currentTransaction != null && currentTransaction != this; currentTransaction = currentTransaction.ParentTransaction)
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
                _currentTransaction = ParentTransaction;
            }
        }

        private void Release()
        {
            foreach (var resource in ReadLog)
            {
                resource.Key.Release();
            }
            foreach (var resource in WriteLog)
            {
                resource.Key.Release();
            }
            ReadLog.Clear();
            WriteLog.Clear();
        }
    }
}

#endif