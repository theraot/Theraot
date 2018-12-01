// Needed for NET35 (ThreadLocal)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        private SafeDictionary<Thread, INeedle<T>> _slots;
        private Func<T> _valueFactory;

        public TrackingThreadLocal(Func<T> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _slots = new SafeDictionary<Thread, INeedle<T>>(_maxProbingHint);
        }

        Exception IPromise.Exception => null;

        bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;

        bool IPromise.IsCanceled => false;

        bool IPromise.IsCompleted => IsValueCreated;

        bool IPromise.IsFaulted => false;

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

        T IThreadLocal<T>.ValueForDebugDisplay => TryGetValue(Thread.CurrentThread, out var target) ? target : default;

        public IList<T> Values
        {
            get { return _slots.ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>); }
        }

        [DebuggerNonUserCode]
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
            {
                _slots = null;
                _valueFactory = null;
            }
        }

        public void EraseValue()
        {
            EraseValue(Thread.CurrentThread);
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

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
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

        public bool TryGetValue(out T value)
        {
            return TryGetValue(Thread.CurrentThread, out value);
        }

        void IWaitablePromise.Wait()
        {
            GC.KeepAlive(Value);
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
            if (_slots.TryGetOrAdd(thread, ThreadLocalHelper<T>.RecursionGuardNeedle, out var needle))
            {
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
            }
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