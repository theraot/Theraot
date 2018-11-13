// Needed for NET35 (ThreadLocal)

using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}")]
    public sealed class TrackingThreadLocal<T> : IThreadLocal<T>, IWaitablePromise<T>, ICacheNeedle<T>, IObserver<T>
    {
        private const int _maxProbingHint = 4;

        private int _disposing;
        private SafeDictionary<Thread, INeedle<T>> _slots;
        private Func<T> _valueFactory;

        public TrackingThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            _valueFactory = valueFactory;
            _slots = new SafeDictionary<Thread, INeedle<T>>(_maxProbingHint);
        }

        public bool IsValueCreated
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
                }
                INeedle<T> needle;
                if (_slots.TryGetValue(Thread.CurrentThread, out needle))
                {
                    return needle is ReadOnlyStructNeedle<T>;
                }
                return false;
            }
        }

        public T Value
        {
            get { return GetValue(Thread.CurrentThread); }

            set { SetValue(Thread.CurrentThread, value); }
        }

        public IList<T> Values
        {
            get { return _slots.ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>); }
        }

        Exception IPromise.Exception
        {
            get { return null; }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get { return IsValueCreated; }
        }

        bool IPromise.IsCanceled
        {
            get { return false; }
        }

        bool IPromise.IsCompleted
        {
            get { return IsValueCreated; }
        }

        bool IPromise.IsFaulted
        {
            get { return false; }
        }

        [System.Diagnostics.DebuggerNonUserCode]
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

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        public bool TryGetValue(Thread thread, out T target)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(TrackingThreadLocal<T>));
            }
            INeedle<T> tmp;
            if (_slots.TryGetValue(thread, out tmp))
            {
                target = tmp.Value;
                return true;
            }
            target = default(T);
            return false;
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
            INeedle<T> needle;
            if (_slots.TryGetOrAdd(thread, ThreadLocalHelper<T>.RecursionGuardNeedle, out needle))
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

        T IThreadLocal<T>.ValueForDebugDisplay
        {
            get
            {
                T target;
                return TryGetValue(Thread.CurrentThread, out target) ? target : default(T);
            }
        }
    }
}