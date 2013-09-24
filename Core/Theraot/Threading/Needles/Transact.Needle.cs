#if FAT

using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : Theraot.Threading.Needles.Needle<T>, IResource
        {
            private LockNeedle<Transact> _lockNeedle = new LockNeedle<Transact>(Transact._lockContext);

            public Needle(T value)
                : base(value)
            {
                //Empty
            }

            public override T Value
            {
                get
                {
                    var transaction = Transact.CurrentTransaction;
                    if (ReferenceEquals(transaction, null))
                    {
                        var tmp = base.Value;
                        return tmp;
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
                            var tmp = base.Value;
                            transaction._readLog.CharyAdd(this, tmp);
                            return tmp;
                        }
                    }
                }
                set
                {
                    var transaction = Transact.CurrentTransaction;
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

            bool IResource.CheckValue()
            {
                var transaction = Transact.CurrentTransaction;
                object value;
                if (transaction._readLog.TryGetValue(this, out value))
                {
                    return EqualityComparer<T>.Default.Equals(base.Value, (T)value);
                }
                return false;
            }

            bool IResource.CheckCapture()
            {
                var transaction = Transact.CurrentTransaction;
                return ReferenceEquals(_lockNeedle.Value, transaction);
            }

            bool IResource.Commit()
            {
                var transaction = Transact.CurrentTransaction;
                if (ReferenceEquals(_lockNeedle.Value, transaction))
                {
                    object value;
                    if (transaction._writeLog.TryGetValue(this, out value))
                    {
                        base.Value = (T)value;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void IResource.Rollback()
            {
                var transaction = Transact.CurrentTransaction;
                if (!ReferenceEquals(transaction, null))
                {
                    transaction._readLog.Remove(this);
                    transaction._writeLog.Remove(this);
                }
                _lockNeedle.Release();
            }

            private void OnDispose()
            {
                //TODO: Remove from read and write log
                _lockNeedle.Release();
            }
        }
    }
}

#endif