// Needed for NET35 (ThreadLocal)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}")]
    public sealed class TrackingThreadLocal<T> : IThreadLocal<T>, ICacheNeedle<T>, IObserver<T>
    {
        private const int MaxProbingHint = 4;

        private int _disposing;
        private ThreadSafeDictionary<UniqueId, INeedle<T>> _slots;
        private Func<T> _valueFactory;

        public TrackingThreadLocal(Func<T> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _slots = new ThreadSafeDictionary<UniqueId, INeedle<T>>(MaxProbingHint);
        }

        public bool TryGetValue(out T value)
        {
            return TryGetValue(ThreadUniqueId.CurrentThreadId, out value);
        }

        bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;

        bool IPromise.IsCompleted => IsValueCreated;

        void IObserver<T>.OnCompleted()
        {
            // Empty
        }

        void IObserver<T>.OnError(Exception error)
        {
            SetError(ThreadUniqueId.CurrentThreadId, error);
        }

        void IObserver<T>.OnNext(T value)
        {
            Value = value;
        }

        public bool IsValueCreated
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
                }

                if (_slots.TryGetValue(ThreadUniqueId.CurrentThreadId, out var needle))
                {
                    return needle is ReadOnlyStructNeedle<T>;
                }

                return false;
            }
        }

        public T Value
        {
            get => GetValue(ThreadUniqueId.CurrentThreadId);

            set => SetValue(ThreadUniqueId.CurrentThreadId, value);
        }

        public IList<T> Values => _slots.ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>);

        T IThreadLocal<T>.ValueForDebugDisplay => TryGetValue(ThreadUniqueId.CurrentThreadId, out var target) ? target : default;

        [DebuggerNonUserCode]
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposing, 1, 0) != 0)
            {
                return;
            }

            _slots = null;
            _valueFactory = null;
        }

        public void EraseValue()
        {
            EraseValue(ThreadUniqueId.CurrentThreadId);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private bool TryGetValue(UniqueId threadUniqueId, out T target)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            if (_slots.TryGetValue(threadUniqueId, out var tmp))
            {
                target = tmp.Value;
                return true;
            }

            target = default;
            return false;
        }

        private void EraseValue(UniqueId threadUniqueId)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Remove(threadUniqueId);
        }

        private T GetValue(UniqueId threadUniqueId)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            if (!_slots.TryGetOrAdd(threadUniqueId, ThreadLocalHelper<T>.RecursionGuardNeedle, out var needle))
            {
                return needle.Value;
            }

            try
            {
                needle = new ReadOnlyStructNeedle<T>(_valueFactory.Invoke());
            }
            catch (Exception exception)
            {
                if (exception != ThreadLocalHelper.RecursionGuardException)
                {
                    needle = new ExceptionStructNeedle<T>(exception);
                }
            }

            _slots.Set(threadUniqueId, needle);
            return needle.Value;
        }

        private void SetError(UniqueId threadUniqueId, Exception error)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Set(threadUniqueId, new ExceptionStructNeedle<T>(error));
        }

        private void SetValue(UniqueId threadUniqueId, T value)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Set(threadUniqueId, new ReadOnlyStructNeedle<T>(value));
        }
    }
}