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
    public sealed class TrackingThreadLocal<T> : IThreadLocal<T>, IWaitablePromise<T>, ICacheNeedle<T>, IObserver<T>
    {
        private const int _maxProbingHint = 4;

        private int _disposing;
        private ThreadSafeDictionary<Thread, INeedle<T>> _slots;
        private Func<T> _valueFactory;

        public TrackingThreadLocal(Func<T> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _slots = new ThreadSafeDictionary<Thread, INeedle<T>>(_maxProbingHint);
        }

        public bool TryGetValue(out T value)
        {
            return TryGetValue(Thread.CurrentThread, out value);
        }

        void IObserver<T>.OnCompleted()
        {
            // Empty
        }

        void IObserver<T>.OnError(Exception error)
        {
            SetError(Thread.CurrentThread, error);
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

                if (_slots.TryGetValue(Thread.CurrentThread, out var needle))
                {
                    return needle is ReadOnlyStructNeedle<T>;
                }

                return false;
            }
        }

        public T Value
        {
            get => GetValue(Thread.CurrentThread);

            set => SetValue(Thread.CurrentThread, value);
        }

        public IList<T> Values => _slots.ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>);

        T IThreadLocal<T>.ValueForDebugDisplay => TryGetValue(Thread.CurrentThread, out var target) ? target : default;

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

        Exception IPromise.Exception => null;

        bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;

        bool IPromise.IsCanceled => false;

        bool IPromise.IsCompleted => IsValueCreated;

        bool IPromise.IsFaulted => false;

        void IWaitablePromise.Wait()
        {
            GC.KeepAlive(Value);
        }

        public void EraseValue()
        {
            EraseValue(Thread.CurrentThread);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool TryGetValue(Thread thread, out T target)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            if (_slots.TryGetValue(thread, out var tmp))
            {
                target = tmp.Value;
                return true;
            }

            target = default;
            return false;
        }

        private void EraseValue(Thread thread)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Remove(thread);
        }

        private T GetValue(Thread thread)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            if (!_slots.TryGetOrAdd(thread, ThreadLocalHelper<T>.RecursionGuardNeedle, out var needle))
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

            _slots.Set(thread, needle);
            return needle.Value;
        }

        private void SetError(Thread thread, Exception error)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Set(thread, new ExceptionStructNeedle<T>(error));
        }

        private void SetValue(Thread thread, T value)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }

            _slots.Set(thread, new ReadOnlyStructNeedle<T>(value));
        }
    }
}