#if FAT

using System;
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

        internal static Transact CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
        }

        public static Transact Create()
        {
            return new Transact();
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

        private void RollBack()
        {
            foreach (var resource in _readLog)
            {
                resource.Key.Rollback();
            }
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
            if (!ReferenceEquals(_currentTransaction, null))
            {
                _readLog.Clear();
                _writeLog.Clear();
                Transact parentTransaction = _currentTransaction._parentTransaction;
                _currentTransaction.Dispose();
                _currentTransaction = parentTransaction;
            }
        }
    }
}

#endif