#if FAT

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : Theraot.Threading.Needles.Needle<T>, IResource
        {
            private Needles.Needle<Thread> _owner = new Needles.Needle<Thread>();
            private Transact _transaction;

            public Needle(T value)
                : base(value)
            {
                _transaction = Transact.CurrentTransaction;
            }

            public override T Value
            {
                get
                {
                    _transaction = Transact.CurrentTransaction;
                    if (ReferenceEquals(_transaction, null))
                    {
                        return base.Value;
                    }
                    else
                    {
                        object value;
                        if (_transaction._writeLog.TryGetValue(this, out value))
                        {
                            return (T)value;
                        }
                        else
                        {
                            var tmp = base.Value;
                            _transaction._readLog.CharyAdd(this, tmp);
                            return tmp;
                        }
                    }
                }
                set
                {
                    _transaction = Transact.CurrentTransaction;
                    if (ReferenceEquals(_transaction, null))
                    {
                        base.Value = value;
                    }
                    else
                    {
                        _transaction._writeLog.Set(this, value);
                    }
                }
            }

            void IResource.Capture(ref Needles.Needle<Thread> thread)
            {
                _owner.Unify(ref thread);
            }

            bool IResource.Check()
            {
                _transaction = Transact.CurrentTransaction;
                object value;
                if (_transaction._readLog.TryGetValue(this, out value))
                {
                    return EqualityComparer<T>.Default.Equals(base.Value, (T)value);
                }
                else
                {
                    return false;
                }
            }

            bool IResource.Commit()
            {
                _transaction = Transact.CurrentTransaction;
                if (_owner.Value.Equals(Thread.CurrentThread))
                {
                    object value;
                    if (_transaction._writeLog.TryGetValue(this, out value))
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
                OnDispose();
            }

            private void OnDispose()
            {
                _transaction = Transact.CurrentTransaction;
                _transaction._readLog.Remove(this);
                _transaction._writeLog.Remove(this);
            }
        }
    }
}

#endif
