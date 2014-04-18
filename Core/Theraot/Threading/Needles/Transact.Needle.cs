#if FAT

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : Theraot.Threading.Needles.Needle<T>, IResource
        {
            private readonly ICloner<T> _cloner;
            private readonly LockNeedle<Thread> _lockNeedle;

            public Needle(T value)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException(string.Format("Unable to get a cloner for {0}", typeof(T)));
                }
                else
                {
                    _lockNeedle = new LockNeedle<Thread>(Transact.Context);
                }
            }

            public Needle(T value, ICloner<T> cloner)
                : base(value)
            {
                if (ReferenceEquals(cloner, null))
                {
                    throw new ArgumentNullException("cloner");
                }
                else
                {
                    _lockNeedle = new LockNeedle<Thread>(Transact.Context);
                    _cloner = cloner;
                }
            }

            public override T Value
            {
                get
                {
                    var transaction = Transact.CurrentTransaction;
                    return RetrieveValue(transaction);
                }
                set
                {
                    var transaction = Transact.CurrentTransaction;
                    StoreValue(transaction, value);
                }
            }

            bool IResource.Capture()
            {
                var transaction = Transact.CurrentTransaction;
                if (ReferenceEquals(transaction, null))
                {
                    return false;
                }
                else
                {
                    var slot = transaction._lockSlot;
                    if (ReferenceEquals(slot, null))
                    {
                        return false;
                    }
                    else
                    {
                        slot.Capture(_lockNeedle);
                        return true;
                    }
                }
            }

            bool IResource.CheckCapture()
            {
                var thread = Thread.CurrentThread;
                var check = _lockNeedle.Value;
                return ReferenceEquals(check, thread);
            }

            bool IResource.CheckValue()
            {
                var transaction = Transact.CurrentTransaction;
                object value;
                if (transaction._readLog.TryGetValue(this, out value))
                {
                    var check = base.Value;
                    return EqualityComparer<T>.Default.Equals(check, (T)value);
                }
                else
                {
                    return false;
                }
            }

            bool IResource.Commit()
            {
                var transaction = Transact.CurrentTransaction;
                var thread = Thread.CurrentThread;
                var check = _lockNeedle.Value;
                if (ReferenceEquals(check, thread))
                {
                    object value;
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        StoreValue(transaction._parentTransaction, (T)value);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void IResource.Release()
            {
                OnDispose();
            }

            private void OnDispose()
            {
                var transaction = Transact.CurrentTransaction;
                if (!ReferenceEquals(transaction, null))
                {
                    var slot = transaction._lockSlot;
                    if (!ReferenceEquals(slot, null))
                    {
                        slot.Uncapture(_lockNeedle);
                    }
                    transaction._readLog.Remove(this);
                    transaction._writeLog.Remove(this);
                }
                _lockNeedle.Free();
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
                    else
                    {
                        if (transaction._readLog.TryGetValue(this, out value))
                        {
                            return (T)value;
                        }
                        else
                        {
                            var clone = _cloner.Clone(RetrieveValue(transaction._parentTransaction));
                            transaction._readLog.CharyAdd(this, clone);
                            return clone;
                        }
                    }
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