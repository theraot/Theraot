using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Collections;
using Theraot.Collections.ThreadSafe;
using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    public sealed class TrackingThreadLocal<T> : IDisposable, IThreadLocal<T>, IPromise<T>, ICacheNeedle<T>, IObserver<T>
    {
        private const int INT_MaxProbingHint = 4;

        private int _disposing;
        private SafeDictionary<Thread, INeedle<T>> _slots;
        private Func<T> _valueFactory;

        public TrackingThreadLocal()
            : this(TypeHelper.GetCreateOrDefault<T>())
        {
            //Empty
        }

        public TrackingThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
            _slots = new SafeDictionary<Thread, INeedle<T>>(INT_MaxProbingHint);
        }

        public bool IsValueCreated
        {
            get
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
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
            get
            {
                return GetValue(Thread.CurrentThread);
            }
            set
            {
                SetValue(Thread.CurrentThread, value);
            }
        }

        public IList<T> Values
        {
            get
            {
                return _slots.ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>);
            }
        }

        Exception IPromise.Error
        {
            get
            {
                return null;
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return IsValueCreated;
            }
        }

        bool IPromise.IsCanceled
        {
            get
            {
                return false;
            }
        }

        bool IPromise.IsCompleted
        {
            get
            {
                return IsValueCreated;
            }
        }

        bool IPromise.IsFaulted
        {
            get
            {
                return false;
            }
        }
        T IThreadLocal<T>.ValueForDebugDisplay
        {
            get
            {
                T target;
                if (TryGetValue(Thread.CurrentThread, out target))
                {
                    return target;
                }
                return default(T);
            }
        }
        [global::System.Diagnostics.DebuggerNonUserCode]
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
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
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

        public bool TryGetValue(out T target)
        {
            return TryGetValue(Thread.CurrentThread, out target);
        }

        private void EraseValue(Thread thread)
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _slots.Remove(thread);
        }

        private T GetValue(Thread thread)
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
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
                    if (!ReferenceEquals(exception, ThreadLocalHelper.RecursionGuardException))
                    {
                        needle = new ExceptionStructNeedle<T>(exception);
                    }
                }
                _slots.Set(thread, needle);
            }
            return needle.Value;
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

        private void SetError(Thread thread, Exception error)
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _slots.Set(thread, new ExceptionStructNeedle<T>(error));
        }

        private void SetValue(Thread thread, T value)
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            _slots.Set(thread, new ReadOnlyStructNeedle<T>(value));
        }

        void IPromise.Wait()
        {
            GC.KeepAlive(Value);
        }
    }
}