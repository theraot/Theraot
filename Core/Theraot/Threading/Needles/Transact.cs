#if FAT

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        private static readonly LockContext<Thread> _context = new LockContext<Thread>(512);

        [ThreadStatic]
        private static Transact _currentTransaction;

        private readonly Transact _parentTransaction;
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _readLog;
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _writeLog;
        private LockSlot<Thread> _lockSlot;

        public Transact()
        {
            _writeLog = new WeakHashBucket<IResource, object, WeakNeedle<IResource>>();
            _readLog = new WeakHashBucket<IResource, object, WeakNeedle<IResource>>();
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
        }

        public static Transact CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
        }

        public bool IsRoot
        {
            get
            {
                return ReferenceEquals(_parentTransaction, null);
            }
        }

        private static LockContext<Thread> Context
        {
            get
            {
                return _context;
            }
        }

        public bool Commit()
        {
            if (ReferenceEquals(_currentTransaction, this))
            {
                if (CheckValue())
                {
                    try
                    {
                        ThreadingHelper.SpinWaitUntil(() => _context.ClaimSlot(out _lockSlot));
                        _lockSlot.Value = Thread.CurrentThread;
                        if (_writeLog.Count > 0)
                        {
                            foreach (var resource1 in _writeLog)
                            {
                                resource1.Key.Capture();
                            }
                            foreach (var resource2 in _readLog)
                            {
                                resource2.Key.Capture();
                            }
                            bool rollback = true;
                            bool written = false;
                            try
                            {
                                if (CheckCapture())
                                {
                                    if (CheckValue())
                                    {
                                        foreach (var resource in _writeLog)
                                        {
                                            if (resource.Key.Commit())
                                            {
                                                written = true;
                                            }
                                            else
                                            {
                                                //unexpected
                                                return false;
                                            }
                                        }
                                        rollback = false;
                                        return true;
                                    }
                                    else
                                    {
                                        //the resources has been modified by another thread
                                        return false;
                                    }
                                }
                                else
                                {
                                    //the resources has been claimed by another thread
                                    return false;
                                }
                            }
                            finally
                            {
                                if (rollback)
                                {
                                    if (written)
                                    {
                                        //TODO
                                    }
                                    Rollback(false);
                                }
                                else
                                {
                                    Uncapture();
                                }
                            }
                        }
                        else
                        {
                            //Nothing to commit
                            return true;
                        }
                    }
                    finally
                    {
                        if (!ReferenceEquals(_lockSlot, null))
                        {
                            _lockSlot.Free();
                            _lockSlot = null;
                        }
                    }
                }
                else
                {
                    //the resources has been modified by another thread
                    return false;
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot commit a non-current transaction.");
            }
        }

        private bool CheckCapture()
        {
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
            foreach (var resource in _readLog)
            {
                if (!resource.Key.CheckValue())
                {
                    return false;
                }
            }
            return true;
        }

        private void Rollback(bool disposing)
        {
            do
            {
                Transact currentTransaction = _currentTransaction;
                if (ReferenceEquals(currentTransaction, this))
                {
                    break;
                }
                else
                {
                    if (disposing)
                    {
                        currentTransaction.Dispose();
                    }
                    else
                    {
                        currentTransaction.Rollback(false);
                    }
                }
            } while (true);
            Uncapture();
            if (disposing)
            {
                _readLog.AutoRemoveDeadItems = false;
                _writeLog.AutoRemoveDeadItems = false;
                _currentTransaction = _currentTransaction._parentTransaction;
            }
        }

        private void Uncapture()
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