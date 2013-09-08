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
            private TrackingThreadLocal<T> _read;
            private TrackingThreadLocal<T> _write;

            public Needle(T value)
                : base(value)
            {
                _transaction = Transact.CurrentTransaction;
                _read = new TrackingThreadLocal<T>(() => base.Value);
                _write = new TrackingThreadLocal<T>();
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
                        if (_transaction._writeLog.Contains(this))
                        {
                            return _write.Value;
                        }
                        else
                        {
                            _transaction._readLog.Add(this);
                            return _read.Value;
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
                        _write.Value = value;
                        _transaction._writeLog.Add(this);
                    }
                }
            }

            void IResource.Capture(ref Needles.Needle<Thread> thread)
            {
                _owner.Unify(ref thread);
            }

            bool IResource.Check()
            {
                return EqualityComparer<T>.Default.Equals(base.Value, _read.Value);
            }

            bool IResource.Commit()
            {
                if (_owner.Value.Equals(Thread.CurrentThread))
                {
                    base.Value = _write.Value;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void IResource.Rollback()
            {
                _read.Uncreate();
                _write.Uncreate();
            }

            private void OnDispose()
            {
                _read.Dispose();
                _write.Dispose();
            }
        }
    }
}

#endif
