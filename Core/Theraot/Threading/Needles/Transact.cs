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
        [ThreadStatic]
        private static Transact _currentTransaction;

        private readonly Transact _parentTransaction;
        private readonly HashBucket<Delegate, IResource> _resources;
        
        public Transact()
        {
            _resources = new HashBucket<Delegate, IResource>(DelegateEqualityComparer.Instance);
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
        }

        internal static Transact CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
        }

        public static Transact Create()
        {
            return new Transact();
        }

        public static T Read<T>(Func<T> source)
        {
            var transaction = Transact.CurrentTransaction;
            if (transaction == null)
            {
                return source.Invoke();
            }
            else
            {
                return Needle<T>.Read(source).Value;
            }
        }

        public static void Write<T>(Action<T> target, T value)
        {
            var transaction = Transact.CurrentTransaction;
            if (transaction == null)
            {
                target.Invoke(value);
            }
            else
            {
                Needle<T>.Write(target).Value = value;
            }
        }

        public bool Commit()
        {
            if (Check())
            {
                Needles.Needle<Thread> thread = null;
                if (_resources.Count > 0)
                {
                    foreach (var resource in _resources.Pairs)
                    {
                        resource.Value.Capture(ref thread);
                    }
                    //Should not be null
                    thread = thread.Simplify();
                    if
                    (
                        ReferenceEquals
                        (
                            thread.CompareExchange
                            (
                                new StructNeedle<Thread>(Thread.CurrentThread),
                                null
                            ),
                            null
                        )
                    )
                    {
                        try
                        {
                            if (Check())
                            {
                                foreach (var resource in _resources.Pairs)
                                {
                                    resource.Value.Commit();
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        finally
                        {
                            thread.Value = null;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private bool Check()
        {
            bool check;
            foreach (var resource in _resources.Pairs)
            {
                if (!resource.Value.Check())
                {
                    check = false;
                }
            }
            check = true;
            return check;
        }

        private void Rollback()
        {
            //Empty
        }

        private IResource TryAddResource(Delegate key, IResource value)
        {
            return _resources.TryAdd(key, value);
        }

        private bool TryGetResource(Delegate key, out IResource resource)
        {
            return _resources.TryGetValue(key, out resource);
        }
    }
}

#endif
