// Needed for NET35 (ThreadLocal)

#if NET20 || NET30 || NET35 || NET40 || NET45 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2

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
            _valueFactory = valueFactory ?? throw new ArgumentNullException(nameof(valueFactory));
            _slot = Thread.AllocateDataSlot();
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
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }
                return Thread.GetData(_slot) is ReadOnlyStructNeedle<T>;
            }
        }

        public T Value
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }
                var bundle = Thread.GetData(_slot);
                if (!(bundle is INeedle<T> needle))
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
                        if (exception != ThreadLocalHelper.RecursionGuardException)
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
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
                }
                Thread.SetData(_slot, new ReadOnlyStructNeedle<T>(value));
            }
        }

        T IThreadLocal<T>.ValueForDebugDisplay => ValueForDebugDisplay;

        IList<T> IThreadLocal<T>.Values => throw new InvalidOperationException();

        internal T ValueForDebugDisplay => TryGetValue(out var target) ? target : default;

        [DebuggerNonUserCode]
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
            if (Volatile.Read(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(nameof(NoTrackingThreadLocal<T>));
            }
            Thread.SetData(_slot, null);
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
            Thread.SetData(_slot, new ExceptionStructNeedle<T>(error));
        }

        void IObserver<T>.OnNext(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public bool TryGetValue(out T value)
        {
            var bundle = Thread.GetData(_slot);
            if (!(bundle is INeedle<T> container))
            {
                value = default;
                return false;
            }
            value = container.Value;
            return true;
        }

        void IWaitablePromise.Wait()
        {
            GC.KeepAlive(Value);
        }
    }
}

#endif