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
        private readonly Func<T> _read;
        private readonly Action<T> _write;

        private int _taken;
        private ThreadLocal<T> _value;

        //TODO: hay que garantizar que todos los hilos reciben el mismo TransactionNeedle
        internal TransactionNeedle(T value, Func<T> read, Action<T> write, IEqualityComparer<T> comparer)
        {
            _comparer = comparer ?? EqualityComparerHelper<T>.Default;
            _read = read ?? (() => value);
            _write = write ?? ActionHelper.GetNoopAction<T>();
            _value = new ThreadLocal<T>()
            {
                Value = value
            };
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

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return true;
            }
        }

        public bool Commit()
        {
            if (Interlocked.CompareExchange(ref _taken, 1, 0) == 0)
            {
                try
                {
                    _write.Invoke(_value.Value);
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
            Interlocked.Increment(ref _taken);
            if (_comparer.Equals(_read.Invoke(), _value.Value))
            {
                return DisposableAkin.Create(() => Interlocked.Decrement(ref _taken));
            }
            else
            {
                Interlocked.Decrement(ref _taken);
                return null;
            }
        }

        public void Rollback()
        {
            _value.Value = _read.Invoke();
        }
    }
}

#endif