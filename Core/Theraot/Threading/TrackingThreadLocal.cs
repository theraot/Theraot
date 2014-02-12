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
        private HashBucket<Thread, T> _slots;
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
            if (Environment.ProcessorCount < INT_MaxProcessorCount)
            {
                _slots = new HashBucket<Thread, T>(Environment.ProcessorCount * 2, INT_MaxProbingHint);
            }
            else
            {
                _slots = new HashBucket<Thread, T>(INT_MaxProcessorCount * 2, INT_MaxProbingHint);
            }
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

        bool IPromise<T>.IsAlive
        {
            get
            {
                return IsValueCreated;
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
                    return _slots.ContainsKey(Thread.CurrentThread);
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
                    T value;
                    if (!_slots.TryGetValue(Thread.CurrentThread, out value))
                    {
                        value = _valueFactory.Invoke();
                        _slots.Set(Thread.CurrentThread, value);
                    }
                    return value;
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
                    _slots.Set(Thread.CurrentThread, value);
                }
            }
        }

        public IList<T> Values
        {
            get
            {
                return _slots.GetPairs().ConvertAll(input => input.Value);
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

        void IPromise.Wait()
        {
            GC.KeepAlive(Value);
        }

        void IPromised.OnCompleted()
        {
            GC.KeepAlive(Value);
        }

        void IPromised.OnError(Exception error)
        {
            //Empty
        }

        void IPromised.OnNext(T value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
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

        private sealed class Container
        {
            private T _value;

            public Container(T value)
            {
                _value = value;
            }

            public T Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                }
            }
        }
    }
}