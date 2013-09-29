#if FAT

using System;
using System.Threading;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        private static readonly LockNeedleContext<Transact> _lockContext = new LockNeedleContext<Transact>();

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
            if (CheckValue())
            {
                ThreadingHelper.SpinWait(() => _lockContext.ClaimSlot(out _lockSlot));
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
                    try
                    {
                        if (CheckCapture() && CheckValue())
                        {
                            foreach (var resource in _writeLog)
                            {
                                if (!resource.Key.Commit())
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
                    finally
                    {
                        if (rollback)
                        {
                            Rollback();
                        }
                        _lockSlot.Release();
                        _lockSlot = null;
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

        private void Rollback()
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
                    currentTransaction.Dispose();
                }
            }
            while (true);
            foreach (var resource in _readLog)
            {
                resource.Key.Rollback();
            }
            foreach (var resource in _writeLog)
            {
                resource.Key.Rollback();
            }
            _readLog.Clear();
            _writeLog.Clear();
            _currentTransaction = _currentTransaction._parentTransaction;
        }
    }
}

#endif