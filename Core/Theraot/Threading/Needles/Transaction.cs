#if FAT

using System;
using System.Collections.Generic;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed partial class Transaction
    {
        [ThreadStatic]
        private static Transaction _currentTransaction;

        private readonly Transaction _parentTransaction;
        private readonly Dictionary<object, IResource> _resources;
        
        private Transaction()
        {
            _resources = new Dictionary<object, IResource>();
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
        }

        internal static Transaction CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
        }

        public static Transaction Create()
        {
            return new Transaction();
        }

        public static Transaction GetTransaction(bool createNew)
        {
            if (createNew)
            {
                return new Transaction();
            }
            else
            {
                return _currentTransaction;
            }
        }

        private bool Check()
        {
            bool check;
            foreach (var resource in _resources.Values)
            {
                if (!resource.Check())
                {
                    check = false;
                }
            }
            check = true;
            return check;
        }

        public bool Commit()
        {
            if (Check())
            {
                Needles.Needle<Thread> thread = null;
                if (_resources.Count > 0)
                {
                    foreach (var resource in _resources.Values)
                    {
                        resource.Capture(ref thread);
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
                                foreach (var resource in _resources.Values)
                                {
                                    resource.Commit();
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

        public T Read<T>(Func<T> source)
        {
            var transaction = Transaction.CurrentTransaction;
            if (transaction == null)
            {
                return source.Invoke();
            }
            else
            {
                return Needle<T>.Read(source).Value;
            }
        }

        private void SetResource(object key, IResource value)
        {
            _resources[key] = value;
        }

        private bool TryGetResource(object key, out IResource resource)
        {
            try
            {
                resource = _resources[key];
                return true;
            }
            catch
            {
                resource = null;
                return false;
            }
        }

        public void Write<T>(Action<T> target, T value)
        {
            var transaction = Transaction.CurrentTransaction;
            if (transaction == null)
            {
                target.Invoke(value);
            }
            else
            {
                Needle<T>.Write(target).Value = value;
            }
        }
    }
}

#endif