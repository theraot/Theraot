#if FAT

using System;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        [ThreadStatic]
        private static Transact _currentTransaction;

        private readonly Transact _parentTransaction;
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _readLog;
        private readonly WeakHashBucket<IResource, object, WeakNeedle<IResource>> _writeLog;
        private LockNeedleSlot<Transact> _lockSlot;

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

        public bool Commit()
        {
            if (ReferenceEquals(_currentTransaction, this))
            {
                if (CheckValue())
                {
                    ThreadingHelper.SpinWaitUntil(() => LockNeedleContext<Transact>.Instance.ClaimSlot(out _lockSlot));
                    if (_writeLog.Count > 0)
                    {
                        _lockSlot.Value = this;
                        foreach (var resource in _writeLog)
                        {
                            resource.Key.Capture();
                        }
                        foreach (var resource in _readLog)
                        {
                            resource.Key.Capture();
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
                                _lockSlot.Free();
                                _lockSlot = null;
                            }
                            else
                            {
                                Uncapture();
                                _lockSlot.Free();
                                _lockSlot = null;
                            }
                        }
                    }
                    else
                    {
                        //Nothing to commit
                        return true;
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
            Transact currentTransaction;
            do
            {
                currentTransaction = _currentTransaction;
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
            }
            while (true);
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