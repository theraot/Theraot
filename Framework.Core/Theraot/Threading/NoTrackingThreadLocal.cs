// Needed for NET35 (ThreadLocal)

#if TARGETS_NET || GREATERTHAN_NETCOREAPP11

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Theraot.Reflection;
using Theraot.Threading.Needles;

namespace Theraot.Threading
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    [DebuggerNonUserCode]
    public sealed class NoTrackingThreadLocal<T> : IThreadLocal<T>, ICacheNeedle<T>, IObserver<T>
    {
        private int _disposing;
        private LocalDataStoreSlot? _slot;
        private Func<T>? _valueFactory;

        public NoTrackingThreadLocal()
            : this(ConstructorHelper.CreateOrDefault<T>)
        {
            // Empty
        }

        public NoTrackingThreadLocal(Func<T> valueFactory)
        {
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _slot = Thread.AllocateDataSlot();
        }

        bool IReadOnlyNeedle<T>.IsAlive => IsValueCreated;
        bool IPromise.IsCompleted => IsValueCreated;

        public bool IsValueCreated
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }

                var slot = _slot;
                return slot != null && Thread.GetData(slot) is ReadOnlyStructNeedle<T>;
            }
        }

        public T Value
        {
            get
            {
                var valueFactory = _valueFactory;
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }

                var slot = _slot;
                if (slot != null)
                {
                    var bundle = Thread.GetData(slot);
                    if (bundle is INeedle<T> needle)
                    {
                        return needle.Value;
                    }
                }
                else
                {
                    slot = InitializeSlot();
                }

                try
                {
                    Thread.SetData(slot, ThreadLocalHelper<T>.RecursionGuardNeedle);
                    var result = valueFactory!.Invoke();
                    Thread.SetData(slot, new ReadOnlyStructNeedle<T>(result));
                    return result;
                }
                catch (Exception exception)
                {
                    if (exception != ThreadLocalHelper.RecursionGuardException)
                    {
                        Thread.SetData(slot, new ExceptionStructNeedle<T>(exception));
                    }

                    throw;
                }
            }
            set
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }

                var slot = _slot ?? InitializeSlot();
                Thread.SetData(slot, new ReadOnlyStructNeedle<T>(value));
            }
        }

        T IThreadLocal<T>.ValueForDebugDisplay => ValueForDebugDisplay;

        IList<T> IThreadLocal<T>.Values => throw new InvalidOperationException();

        internal T ValueForDebugDisplay => TryGetValue(out var target) ? target : default;

        [DebuggerNonUserCode]
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposing, 1, 0) != 0)
            {
                return;
            }

            _slot = null;
            _valueFactory = null;
        }

        public void EraseValue()
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
            }

            var slot = _slot ?? InitializeSlot();
            Thread.SetData(slot, null);
        }

        void IObserver<T>.OnCompleted()
        {
            GC.KeepAlive(Value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
            }

            var slot = _slot ?? InitializeSlot();
            Thread.SetData(slot, new ExceptionStructNeedle<T>(error));
        }

        void IObserver<T>.OnNext(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? string.Empty;
        }

        public bool TryGetValue(out T value)
        {
            var slot = _slot;
            if (slot == null)
            {
                value = default!;
                return false;
            }

            var bundle = Thread.GetData(slot);
            if (!(bundle is INeedle<T> container))
            {
                value = default!;
                return false;
            }

            value = container.Value;
            return true;
        }

        private LocalDataStoreSlot InitializeSlot()
        {
            var slot = Thread.AllocateDataSlot();
            var found = Interlocked.CompareExchange(ref _slot, slot, null);
            if (found != null)
            {
                slot = found;
            }

            return slot;
        }
    }
}

#endif