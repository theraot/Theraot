// Needed for NET35 (ThreadLocal)

using System;
using System.Threading;

using Theraot.Core;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class NoTrackingThreadLocal<T> : IThreadLocal<T>, IWaitablePromise<T>, ICacheNeedle<T>, IObserver<T>
    {
        private int _disposing;
        private LocalDataStoreSlot _slot;
        private Func<T> _valueFactory;

        public NoTrackingThreadLocal()
            : this(TypeHelper.GetCreateOrDefault<T>())
        {
            // Empty
        }

        public NoTrackingThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
            _slot = Thread.AllocateDataSlot();
        }

        public bool IsValueCreated
        {
            get
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                return Thread.GetData(_slot) is ReadOnlyStructNeedle<T>;
            }
        }

        public T Value
        {
            get
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                var bundle = Thread.GetData(_slot);
                var needle = bundle as INeedle<T>;
                if (needle == null)
                {
                    try
                    {
                        Thread.SetData(_slot, ThreadLocalHelper<T>.RecursionGuardNeedle);
                        var result = _valueFactory.Invoke();
                        Thread.SetData(_slot, new ReadOnlyStructNeedle<T>(result));
                        return result;
                    }
                    catch (Exception exception)
                    {
                        if (!ReferenceEquals(exception, ThreadLocalHelper.RecursionGuardException))
                        {
                            Thread.SetData(_slot, new ExceptionStructNeedle<T>(exception));
                        }
                        throw;
                    }
                }
                return needle.Value;
            }
            set
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                Thread.SetData(_slot, new ReadOnlyStructNeedle<T>(value));
            }
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

        T IThreadLocal<T>.ValueForDebugDisplay
        {
            get { return ValueForDebugDisplay; }
        }

        System.Collections.Generic.IList<T> IThreadLocal<T>.Values
        {
            get { throw new InvalidOperationException(); }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
            {
                _slot = null;
                _valueFactory = null;
            }
        }

        public void EraseValue()
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            Thread.SetData(_slot, null);
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        public bool TryGetValue(out T value)
        {
            var bundle = Thread.GetData(_slot);
            var container = bundle as INeedle<T>;
            if (container == null)
            {
                value = default(T);
                return false;
            }
            value = container.Value;
            return true;
        }

        void IObserver<T>.OnCompleted()
        {
            GC.KeepAlive(Value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            Thread.SetData(_slot, new ExceptionStructNeedle<T>(error));
        }

        void IObserver<T>.OnNext(T value)
        {
            Value = value;
        }

        void IWaitablePromise.Wait()
        {
            GC.KeepAlive(Value);
        }

        internal T ValueForDebugDisplay
        {
            get
            {
                T target;
                return TryGetValue(out target) ? target : default(T);
            }
        }
    }
}