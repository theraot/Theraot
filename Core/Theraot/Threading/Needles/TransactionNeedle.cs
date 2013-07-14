#if FAT

using System;
using System.Collections.Generic;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    public sealed class TransactionNeedle<T> : ITransactionNeedle<T>
    {
        private readonly IEqualityComparer<T> _comparer;
        private readonly Func<T> _source;
        private readonly Action<T> _target;

        private T _original;
        private int _taken;
        private ThreadLocal<T> _value;
        private TransactionNeedle(Func<T> source, Action<T> target)
        {
            _source = source;
            _target = target;
            if (!ReferenceEquals(_source, null))
            {
                _original = _source.Invoke();
                _value = new ThreadLocal<T>(_source);
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

        public bool Commit()
        {
            if (Interlocked.CompareExchange(ref _taken, 1, 0) == 0)
            {
                try
                {
                    _target.Invoke(_value.Value);
                    return true;
                }
                finally
                {
                    Interlocked.Decrement(ref _taken);
                }
            }
            else
            {
                return false;
            }
        }

        void INeedle<T>.Release()
        {
            //Empty
        }

        public IDisposable Lock()
        {
            if (ReferenceEquals(_source, null))
            {
                return DisposableAkin.Create();
            }
            else
            {
                Interlocked.Increment(ref _taken);
                if (_comparer.Equals(_original, _value.Value))
                {
                    return DisposableAkin.Create(() => Interlocked.Decrement(ref _taken));
                }
                else
                {
                    Interlocked.Decrement(ref _taken);
                    return null;
                }
            }
        }

        public void Rollback()
        {
            _value.Value = _source.Invoke();
        }

        internal static TransactionNeedle<T> Read(Func<T> source)
        {
            var transaction = Transaction.CurrentTransaction;
            if (transaction == null)
            {
                throw new InvalidOperationException("There is no current transaction.");
            }
            else
            {
                ITransactionResource resource;
                if (transaction.TryGetResource(source, out resource))
                {
                    return resource as TransactionNeedle<T>;
                }
                else
                {
                    resource = new TransactionNeedle<T>(source, null);
                    transaction.SetResource(source, resource);
                    return resource as TransactionNeedle<T>;
                }
            }
        }

        internal static TransactionNeedle<T> Write(Action<T> target)
        {
            var transaction = Transaction.CurrentTransaction;
            if (transaction == null)
            {
                throw new InvalidOperationException("There is no current transaction.");
            }
            else
            {
                ITransactionResource resource;
                if (transaction.TryGetResource(target, out resource))
                {
                    return resource as TransactionNeedle<T>;
                }
                else
                {
                    resource = new TransactionNeedle<T>(null, target);
                    transaction.SetResource(target, resource);
                    return resource as TransactionNeedle<T>;
                }
            }
        }
    }
}

#endif