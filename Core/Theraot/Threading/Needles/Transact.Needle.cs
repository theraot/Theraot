#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed class Needle<T> : Needles.Needle<T>, IResource
        {
            private readonly ICloner<T> _cloner;
            private readonly IEqualityComparer<T> _comparer;
            private readonly RuntimeUniqueIdProdiver.UniqueId _id;
            private readonly NeedleLock<Thread> _needleLock;
            private int _status;
            private int _inUse;

            public Needle(T value)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException("Unable to get a cloner for " + typeof(T));
                }
                _comparer = EqualityComparer<T>.Default;
                _needleLock = new NeedleLock<Thread>(Context);
                _id = RuntimeUniqueIdProdiver.GetNextId();
            }

            public Needle(T value, ICloner<T> cloner)
                : base(value)
            {
                if (ReferenceEquals(cloner, null))
                {
                    throw new ArgumentNullException("cloner");
                }
                _cloner = cloner;
                _comparer = EqualityComparer<T>.Default;
                _needleLock = new NeedleLock<Thread>(Context);
                _id = RuntimeUniqueIdProdiver.GetNextId();
            }

            public Needle(T value, IEqualityComparer<T> comparer)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException("Unable to get a cloner for " + typeof(T));
                }
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _needleLock = new NeedleLock<Thread>(Context);
                _id = RuntimeUniqueIdProdiver.GetNextId();
            }

            public Needle(T value, ICloner<T> cloner, IEqualityComparer<T> comparer)
                : base(value)
            {
                if (ReferenceEquals(cloner, null))
                {
                    throw new ArgumentNullException("cloner");
                }
                _cloner = cloner;
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _needleLock = new NeedleLock<Thread>(Context);
                _id = RuntimeUniqueIdProdiver.GetNextId();
            }

            [System.Diagnostics.DebuggerNonUserCode]
            ~Needle()
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
                        GC.KeepAlive(exception);
                    }
                }
            }

            public override T Value
            {
                get
                {
                    var transaction = CurrentTransaction;
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
                        var transaction = CurrentTransaction;
                        StoreValue(transaction, value);
                    }
                }
            }

            [System.Diagnostics.DebuggerNonUserCode]
            public void Dispose()
            {
                try
                {
                    Dispose(true);
                }
                finally
                {
                    GC.SuppressFinalize(this);
                }
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override void Free()
            {
                Value = default(T);
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }

            bool IResource.Capture()
            {
                var transaction = CurrentTransaction;
                if (transaction == null)
                {
                    return false;
                }
                var lockSlot = transaction._lockSlot;
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
                var transaction = CurrentTransaction;
                object value;
                if (transaction._readLog.TryGetValue(this, out value))
                {
                    var original = RetrieveValue(transaction._parentTransaction);
                    var found = (T)value;
                    return _comparer.Equals(original, found);
                }
                return false;
            }

            bool IResource.Commit()
            {
                var transaction = CurrentTransaction;
                if (_needleLock.Value == Thread.CurrentThread)
                {
                    object value;
                    Volatile.Write(ref _inUse, 1);
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        StoreValue(transaction._parentTransaction, (T)value);
                    }
                    return true;
                }
                return false;
            }

            [System.Diagnostics.DebuggerNonUserCode]
            private void Dispose(bool disposeManagedResources)
            {
                if (TakeDisposalExecution())
                {
                    if (disposeManagedResources)
                    {
                        OnDispose();
                    }
                }
            }

            private void OnDispose()
            {
                var transaction = CurrentTransaction;
                if (transaction != null)
                {
                    if (!ReferenceEquals(transaction._lockSlot, null))
                    {
                        _needleLock.Uncapture(transaction._lockSlot);
                    }
                    _needleLock.Release();
                    transaction._readLog.Remove(this);
                    transaction._writeLog.Remove(this);
                }
            }

            void IResource.Release()
            {
                // Release is only called on a thread that did capture the Needle
                OnDispose();
            }

            private T RetrieveClone(Transact transaction)
            {
                if (ReferenceEquals(transaction, null))
                {
                    return base.Value;
                }
                else
                {
                    object value;
                    Volatile.Write(ref _inUse, 1);
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    if (transaction._readLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    var original = RetrieveValue(transaction._parentTransaction);
                    var clone = _cloner.Clone(original);
                    if (!_comparer.Equals(clone, original))
                    {
                        transaction._writeLog.Set(this, clone);
                    }

                    transaction._readLog.TryAdd(this, original);

                    return clone;
                }
            }

            private T RetrieveValue(Transact transaction)
            {
                if (ReferenceEquals(transaction, null))
                {
                    return base.Value;
                }
                else
                {
                    object value;
                    Volatile.Write(ref _inUse, 1);
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    if (transaction._readLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    var original = RetrieveValue(transaction._parentTransaction);

                    transaction._readLog.TryAdd(this, original);

                    return original;
                }
            }

            private void StoreValue(Transact transaction, T value)
            {
                if (!IsAlive || ReferenceEquals(transaction, null))
                {
                    base.Value = value;
                }
                else
                {
                    Volatile.Write(ref _inUse, 1);
                    transaction._writeLog.Set(this, value);
                }
            }

            private bool TakeDisposalExecution()
            {
                return _status != -1 && ThreadingHelper.SpinWaitSetUnless(ref _status, -1, 0, -1);
            }
        }
    }
}

#endif