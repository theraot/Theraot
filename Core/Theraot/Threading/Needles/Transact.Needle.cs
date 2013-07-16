#if FAT

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transact
    {
        public sealed partial class Needle<T> : IResource, INeedle<T>
        {
            private readonly Func<T> _source;
            private readonly Action<T> _target;

            private T _original;
            private Needles.Needle<Thread> _owner = new Needles.Needle<Thread>();
            private Transact _transaction;
            private ThreadLocal<T> _value;

            internal Needle(Func<T> source, Action<T> target)
            {
                _transaction = Transact.CurrentTransaction;
                if (ReferenceEquals(_transaction, null))
                {
                    throw new InvalidOperationException("Can't create a needle without an active Transaction.");
                }
                else
                {
                    _source = source;
                    _target = target ?? ActionHelper.GetNoopAction<T>();
                    if (!ReferenceEquals(_source, null))
                    {
                        _original = _source.Invoke();
                    }
                    _value = new ThreadLocal<T>();
                }
            }

            bool IReadOnlyNeedle<T>.IsAlive
            {
                get
                {
                    return true;
                }
            }

            public T Value
            {
                get
                {
                    if (_value.IsValueCreated)
                    {
                        return _value.Value;
                    }
                    else
                    {
                        if (ReferenceEquals(_source, null))
                        {
                            throw new InvalidOperationException("Unable to read write only needle");
                        }
                        else
                        {
                            _value.Value = _original;
                            return _original;
                        }
                    }
                }
                set
                {
                    _value.Value = value;
                }
            }

            void INeedle<T>.Release()
            {
                //Empty
            }

            void IResource.Capture(ref Needles.Needle<Thread> thread)
            {
                _owner.Unify(ref thread);
            }

            bool IResource.Check()
            {
                if (ReferenceEquals(_source, null))
                {
                    return true;
                }
                else
                {
                    if (EqualityComparer<T>.Default.Equals(_original, _value.Value))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            bool IResource.Commit()
            {
                if (_owner.Value.Equals(Thread.CurrentThread))
                {
                    _target.Invoke(_value.Value);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            void IResource.Rollback()
            {
                _value.Value = _source.Invoke();
            }

            private void OnDispose()
            {
                _value.Dispose();
            }
        }
    }
}

#endif
