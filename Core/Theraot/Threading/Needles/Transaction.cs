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

        public bool Commit()
        {
            var disposal = new List<IDisposable>();
            try
            {
                foreach (var resource in _resources.Values)
                {
                    var dispose = resource.Lock();
                    if (dispose == null)
                    {
                        return false;
                    }
                    else
                    {
                        disposal.Add(dispose);
                    }
                }
                foreach (var resource in _resources.Values)
                {
                    resource.Commit();
                }
                return true;
            }
            finally
            {
                foreach (var item in disposal)
                {
                    item.Dispose();
                }
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