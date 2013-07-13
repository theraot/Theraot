#if FAT

using System;
using System.Collections.Generic;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public class Transaction
    {
        [ThreadStatic]
        private static Transaction _currentTransaction;

        private readonly Transaction _parentTransaction;
        private readonly Dictionary<object, ITransactionResource> _resources;

        public Transaction()
        {
            _resources = new Dictionary<object, ITransactionResource>();
            _parentTransaction = _currentTransaction;
            _currentTransaction = this;
        }

        internal static Transaction CurrentTransaction
        {
            get
            {
                return _currentTransaction;
            }
            set
            {
                _currentTransaction = value;
            }
        }

        public static Transaction Create()
        {
            return new Transaction();
        }

        public static Transaction GetTransaction(bool createIfNotExists)
        {
            if (createIfNotExists && _currentTransaction == null)
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

        public void SetResource(object key, ITransactionResource value)
        {
            _resources[key] = value;
        }

        public bool TryGetResource(object key, out ITransactionResource resource)
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
    }
}

#endif