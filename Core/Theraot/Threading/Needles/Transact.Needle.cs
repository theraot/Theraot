#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : Needles.Needle<T>, IResource
        {
            // TODO: Free must be under the transaction
            private readonly ICloner<T> _cloner;
            private readonly NeedleLock<Thread> _needleLock;

            public Needle(T value)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException(string.Format("Unable to get a cloner for {0}", typeof(T)));
                }
                _needleLock = new NeedleLock<Thread>(Context);
            }

            public Needle(T value, ICloner<T> cloner)
                : base(value)
            {
                if (ReferenceEquals(cloner, null))
                {
                    throw new ArgumentNullException("cloner");
                }
                _needleLock = new NeedleLock<Thread>(Context);
                _cloner = cloner;
            }

            public override T Value
            {
                get
                {
                    var transaction = CurrentTransaction;
                    return RetrieveValue(transaction);
                }
                set
                {
                    var transaction = CurrentTransaction;
                    StoreValue(transaction, value);
                }
            }

            bool IResource.Capture()
            {
                var transaction = CurrentTransaction;
                if (ReferenceEquals(transaction, null))
                {
                    return false;
                }
                var slot = transaction._lockSlot;
                if (ReferenceEquals(slot, null))
                {
                    return false;
                }
                slot.Capture(_needleLock);
                return true;
            }

            bool IResource.CheckCapture()
            {
                var thread = Thread.CurrentThread;
                var check = _needleLock.Value;
                return ReferenceEquals(check, thread);
            }

            bool IResource.CheckValue()
            {
                var transaction = CurrentTransaction;
                object value;
                if (transaction._readLog.TryGetValue(this, out value))
                {
                    var check = base.Value;
                    return EqualityComparer<T>.Default.Equals(check, (T)value);
                }
                return false;
            }

            bool IResource.Commit()
            {
                var transaction = CurrentTransaction;
                var thread = Thread.CurrentThread;
                var check = _needleLock.Value;
                if (ReferenceEquals(check, thread))
                {
                    object value;
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        StoreValue(transaction._parentTransaction, (T)value);
                    }
                    return true;
                }
                return false;
            }

            void IResource.Release()
            {
                OnDispose();
            }

            private void OnDispose()
            {
                var transaction = CurrentTransaction;
                if (!ReferenceEquals(transaction, null))
                {
                    var slot = transaction._lockSlot;
                    if (!ReferenceEquals(slot, null))
                    {
                        slot.Uncapture(_needleLock);
                    }
                    transaction._readLog.Remove(this);
                    transaction._writeLog.Remove(this);
                }
                _needleLock.Free();
            }

            private T RetrieveValue(Transact transaction)
            {
                if (ReferenceEquals(transaction, null))
                {
                    var value = base.Value;
                    return value;
                }
                else
                {
                    object value;
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    if (transaction._readLog.TryGetValue(this, out value))
                    {
                        return (T)value;
                    }
                    var clone = _cloner.Clone(RetrieveValue(transaction._parentTransaction));
                    transaction._readLog.CharyAdd(this, clone);
                    return clone;
                }
            }

            private void StoreValue(Transact transaction, T value)
            {
                if (ReferenceEquals(transaction, null))
                {
                    base.Value = value;
                }
                else
                {
                    transaction._writeLog.Set(this, value);
                }
            }
        }
    }
}

#endif