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
        public sealed partial class Needle<T> : Needles.Needle<T>, IResource
        {
            private readonly ICloner<T> _cloner;
            private readonly IEqualityComparer<T> _comparer;
            private readonly RuntimeUniqueIdProdiver.UniqueId _id;
            private readonly Pin _pin;

            public Needle(T value)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException(string.Format("Unable to get a cloner for {0}", typeof(T)));
                }
                _comparer = EqualityComparer<T>.Default;
                _pin = new Pin(Context);
                _id = _idProvider.GetNextId();
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
                _pin = new Pin(Context);
                _id = _idProvider.GetNextId();
            }

            public Needle(T value, IEqualityComparer<T> comparer)
                : base(value)
            {
                _cloner = CloneHelper<T>.GetCloner();
                if (ReferenceEquals(_cloner, null))
                {
                    throw new InvalidOperationException(string.Format("Unable to get a cloner for {0}", typeof(T)));
                }
                _comparer = comparer ?? EqualityComparer<T>.Default;
                _pin = new Pin(Context);
                _id = _idProvider.GetNextId();
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
                _pin = new Pin(Context);
                _id = _idProvider.GetNextId();
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
                    var transaction = CurrentTransaction;
                    StoreValue(transaction, value);
                }
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override void Free()
            {
                Thread.MemoryBarrier();
                if (NeedleReservoir<T,Needle<T>>.Recycling)
                {
                    base.Free();
                }
                else
                {
                    Value = default(T);
                }
            }

            public override int GetHashCode()
            {
                return _id.GetHashCode();
            }

            bool IResource.Capture()
            {
                var transaction = CurrentTransaction;
                if (ReferenceEquals(transaction, null))
                {
                    return false;
                }
                return _pin.Capture(transaction._lockSlot);
            }

            bool IResource.CheckCapture()
            {
                return _pin.CheckCapture();
            }

            bool IResource.CheckValue()
            {
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
                if (_pin.CheckCapture())
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

            private void OnDispose()
            {
                var transaction = CurrentTransaction;
                if (!ReferenceEquals(transaction, null))
                {
                    _pin.Release(transaction._lockSlot);
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
                    transaction._writeLog.Set(this, value);
                }
            }
        }
    }
}

#endif