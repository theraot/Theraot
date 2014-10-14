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
    public sealed class TrackingThreadLocal<T> : IDisposable, IThreadLocal<T>, IPromise<T>, IPromised<T>
    {
        private const int INT_MaxProbingHint = 4;
        private const int INT_MaxProcessorCount = 32;

        private int _disposing;
        private HashBucket<Thread, INeedle<T>> _slots;
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
            int capacity = Environment.ProcessorCount < INT_MaxProcessorCount
                ? Environment.ProcessorCount * 2
                : INT_MaxProcessorCount * 2;
            _slots = new HashBucket<Thread, INeedle<T>>(capacity, INT_MaxProbingHint);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~TrackingThreadLocal()
        {
            try
            {
                //Empty
            }
            finally
            {
                try
                {
                    Dispose(false);
                }
                catch
                {
                    //Pokemon
                }
            }
        }

        bool IExpected.IsCanceled
        {
            get
            {
                return false;
            }
        }

        bool IExpected.IsCompleted
        {
            get
            {
                return IsValueCreated;
            }
        }

        bool IExpected.IsFaulted
        {
            get
            {
                return false;
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

        public bool IsValueCreated
        {
            get
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                else
                {
                    INeedle<T> needle;
                    if (_slots.TryGetValue(Thread.CurrentThread, out needle))
                    {
                        return needle is ReadOnlyStructNeedle<T>;
                    }
                    else
                    {
                        return false;
                    }
                }
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
                else
                {
                    INeedle<T> needle;
                    if (_slots.TryGetValue(Thread.CurrentThread, out needle))
                    {
                        return needle.Value;
                    }
                    else
                    {
                        try
                        {
                            _slots.Set(Thread.CurrentThread, ThreadLocalHelper<T>.RecursionGuardNeedle);
                            T result = _valueFactory.Invoke();
                            _slots.Set(Thread.CurrentThread, new ReadOnlyStructNeedle<T>(result));
                            return result;
                        }
                        catch (Exception exception)
                        {
                            if (!ReferenceEquals(exception, ThreadLocalHelper.RecursionGuardException))
                            {
                                _slots.Set(Thread.CurrentThread, new ExceptionStructNeedle<T>(exception));
                            }
                            throw;
                        }
                    }
                }
            }
            set
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                else
                {
                    _slots.Set(Thread.CurrentThread, new ReadOnlyStructNeedle<T>(value));
                }
            }
        }

        public IList<T> Values
        {
            get
            {
                return _slots.GetPairs().ConvertFiltered(input => input.Value.Value, input => input.Value is ReadOnlyStructNeedle<T>);
            }
        }

        public void Clear()
        {
            _slots.Clear();
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Dispose()
        {
            try
            {
                Dispose(true);
            }
            finally
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Free()
        {
            if (Thread.VolatileRead(ref _disposing) == 1)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            else
            {
                _slots.Remove(Thread.CurrentThread);
            }
        }

        void IObserver<T>.OnCompleted()
        {
            GC.KeepAlive(Value);
        }

        void IObserver<T>.OnError(Exception error)
        {
            //Empty
        }

        void IObserver<T>.OnNext(T value)
        {
            Value = value;
        }

        void IPromise.Wait()
        {
            GC.KeepAlive(Value);
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        public bool TryGet(out T target)
        {
            return _slots.TryGetValue(Thread.CurrentThread, out target);
        }

        public void Uncreate()
        {
            _slots.Remove(Thread.CurrentThread);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
        private void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
                {
                    _slots = null;
                    _valueFactory = null;
                }
            }
        }
    }
}