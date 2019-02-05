#if FAT
using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed class TransactNeedle<T> : Needle<T>, IResource, IDisposable
    {
        private readonly ICloner<T> _cloner;
        private readonly IEqualityComparer<T> _comparer;
        private readonly UniqueId _id;
        private readonly NeedleLock<Thread> _needleLock;
        private int _status;
        private int _inUse;

        public TransactNeedle(T value)
            : base(value)
        {
            _cloner = CloneHelper<T>.GetCloner();
            if (_cloner == null)
            {
                throw new InvalidOperationException("Unable to get a cloner for " + typeof(T));
            }
            _comparer = EqualityComparer<T>.Default;
            _needleLock = new NeedleLock<Thread>(Transact.Context);
            _id = RuntimeUniqueIdProvider.GetNextId();
        }

        public TransactNeedle(T value, ICloner<T> cloner)
            : base(value)
        {
            _cloner = cloner ?? throw new ArgumentNullException(nameof(cloner));
            _comparer = EqualityComparer<T>.Default;
            _needleLock = new NeedleLock<Thread>(Transact.Context);
            _id = RuntimeUniqueIdProvider.GetNextId();
        }

        public TransactNeedle(T value, IEqualityComparer<T> comparer)
            : base(value)
        {
            _cloner = CloneHelper<T>.GetCloner();
            if (_cloner == null)
            {
                throw new InvalidOperationException("Unable to get a cloner for " + typeof(T));
            }
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _needleLock = new NeedleLock<Thread>(Transact.Context);
            _id = RuntimeUniqueIdProvider.GetNextId();
        }

        public TransactNeedle(T value, ICloner<T> cloner, IEqualityComparer<T> comparer)
            : base(value)
        {
            _cloner = cloner ?? throw new ArgumentNullException(nameof(cloner));
            _comparer = comparer ?? EqualityComparer<T>.Default;
            _needleLock = new NeedleLock<Thread>(Transact.Context);
            _id = RuntimeUniqueIdProvider.GetNextId();
        }

        [System.Diagnostics.DebuggerNonUserCode]
        ~TransactNeedle()
        {
            try
            {
                // Empty
            }
            finally
            {
                try
                {
                    Dispose(false);
                }
                catch (Exception exception)
                {
                    // Fields may be partially collected.
                    No.Op(exception);
                }
            }
        }

        public override T Value
        {
            get
            {
                var transaction = Transact.CurrentTransaction;
                return RetrieveClone(transaction);
            }
            set
            {
                if (NeedleReservoir.Recycling)
                {
                    if (Volatile.Read(ref _inUse) == 1)
                    {
                        throw new InvalidOperationException("The Needle has been used in a transaction.");
                    }
                    StoreValue(null, value);
                }
                else
                {
                    var transaction = Transact.CurrentTransaction;
                    StoreValue(transaction, value);
                }
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public override void Free()
        {
            Value = default;
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }

        bool IResource.Capture()
        {
            var transaction = Transact.CurrentTransaction;
            if (transaction == null)
            {
                return false;
            }
            var lockSlot = transaction.LockSlot;
            if (lockSlot == null)
            {
                return false;
            }
            _needleLock.Capture(lockSlot);
            return true;
        }

        bool IResource.CheckCapture()
        {
            var thread = Thread.CurrentThread;
            var check = _needleLock.Value;
            return check == thread;
        }

        bool IResource.CheckValue()
        {
            Volatile.Write(ref _inUse, 1);
            var transaction = Transact.CurrentTransaction;
            if (!transaction.ReadLog.TryGetValue(this, out var value))
            {
                return false;
            }

            var original = RetrieveValue(transaction.ParentTransaction);
            var found = (T)value;
            return _comparer.Equals(original, found);
        }

        bool IResource.Commit()
        {
            var transaction = Transact.CurrentTransaction;
            if (_needleLock.Value != Thread.CurrentThread)
            {
                return false;
            }

            Volatile.Write(ref _inUse, 1);
            if (transaction.WriteLog.TryGetValue(this, out var value))
            {
                StoreValue(transaction.ParentTransaction, (T)value);
            }
            return true;
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (!TakeDisposalExecution())
            {
                return;
            }

            if (disposeManagedResources)
            {
                OnDispose();
            }
        }

        private void OnDispose()
        {
            var transaction = Transact.CurrentTransaction;
            if (transaction == null)
            {
                return;
            }

            if (transaction.LockSlot != null)
            {
                _needleLock.Release(transaction.LockSlot);
            }
            _needleLock.Release();
            transaction.ReadLog.Remove(this);
            transaction.WriteLog.Remove(this);
        }

        void IResource.Release()
        {
            // Release is only called on a thread that did capture the Needle
            OnDispose();
        }

        private T RetrieveClone(Transact transaction)
        {
            if (transaction == null)
            {
                return base.Value;
            }
            Volatile.Write(ref _inUse, 1);
            if (transaction.WriteLog.TryGetValue(this, out var value))
            {
                return (T)value;
            }
            if (transaction.ReadLog.TryGetValue(this, out value))
            {
                return (T)value;
            }
            var original = RetrieveValue(transaction.ParentTransaction);
            var clone = _cloner.Clone(original);
            if (!_comparer.Equals(clone, original))
            {
                transaction.WriteLog.Set(this, clone);
            }

            transaction.ReadLog.TryAdd(this, original);

            return clone;
        }

        private T RetrieveValue(Transact transaction)
        {
            if (transaction == null)
            {
                return base.Value;
            }
            Volatile.Write(ref _inUse, 1);
            if (transaction.WriteLog.TryGetValue(this, out var value))
            {
                return (T)value;
            }
            if (transaction.ReadLog.TryGetValue(this, out value))
            {
                return (T)value;
            }
            var original = RetrieveValue(transaction.ParentTransaction);

            transaction.ReadLog.TryAdd(this, original);

            return original;
        }

        private void StoreValue(Transact transaction, T value)
        {
            if (!IsAlive || transaction == null)
            {
                base.Value = value;
            }
            else
            {
                Volatile.Write(ref _inUse, 1);
                transaction.WriteLog.Set(this, value);
            }
        }

        private bool TakeDisposalExecution()
        {
            return _status != -1 && ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
        }
    }
}

#endif