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
            private ThreadLocal<T> _value;
            private Transact _transaction;
            private Needles.Needle<Thread> _owner = new Needles.Needle<Thread>();

            private Needle(Func<T> source, Action<T> target)
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
                        _value = new ThreadLocal<T>(_source);
                    }
                    else
                    {
                        _value = new ThreadLocal<T>();
                    }
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
                    return _value.Value;
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

            void IResource.Capture(ref Needles.Needle<Thread> thread)
            {
                _owner.Unify(ref thread);
            }

            void IResource.Rollback()
            {
                _value.Value = _source.Invoke();
            }

            internal static Needle<T> Read(Func<T> source)
            {
                var transaction = Transact.CurrentTransaction;
                if (transaction == null)
                {
                    throw new InvalidOperationException("There is no current transaction.");
                }
                else
                {
                    IResource resource;
                    if (transaction.TryGetResource(source, out resource))
                    {
                        return resource as Needle<T>;
                    }
                    else
                    {
                        resource = new Needle<T>(source, null);
                        transaction.SetResource(source, resource);
                        return resource as Needle<T>;
                    }
                }
            }

            internal static Needle<T> Write(Action<T> target)
            {
                var transaction = Transact.CurrentTransaction;
                if (transaction == null)
                {
                    throw new InvalidOperationException("There is no current transaction.");
                }
                else
                {
                    IResource resource;
                    if (transaction.TryGetResource(target, out resource))
                    {
                        return resource as Needle<T>;
                    }
                    else
                    {
                        resource = new Needle<T>(null, target);
                        transaction.SetResource(target, resource);
                        return resource as Needle<T>;
                    }
                }
            }

            private void OnDispose()
            {
                _value.Dispose();
            }
        }
    }
}

#endif
