#if LESSTHAN_NET40

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Theraot.Core;
using Theraot.Threading;

namespace System.Threading
{
    [DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    public sealed class ThreadLocal<T> : IDisposable
    {
        private int _disposing;
        private IThreadLocal<T> _wrapped;

        public ThreadLocal()
            : this(FuncHelper.GetDefaultFunc<T>(), false)
        {
            // Empty
        }

        public ThreadLocal(bool trackAllValues)
            : this(FuncHelper.GetDefaultFunc<T>(), trackAllValues)
        {
            // Empty
        }

        public ThreadLocal(Func<T> valueFactory)
            : this(valueFactory, false)
        {
            // Empty
        }

        public ThreadLocal(Func<T> valueFactory, bool trackAllValues)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }

            _wrapped = trackAllValues ? (IThreadLocal<T>)new TrackingThreadLocal<T>(valueFactory) : new NoTrackingThreadLocal<T>(valueFactory);
        }

        public bool IsValueCreated
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(ThreadLocal<T>));
                }

                return _wrapped.IsValueCreated;
            }
        }

        public T Value
        {
            get
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(ThreadLocal<T>));
                }

                return _wrapped.Value;
            }
            set
            {
                if (Volatile.Read(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(ThreadLocal<T>));
                }

                _wrapped.Value = value;
            }
        }

        public IList<T> Values => _wrapped.Values;

        internal T ValueForDebugDisplay => _wrapped.ValueForDebugDisplay;

        [DebuggerNonUserCode]
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

        [DebuggerNonUserCode]
        ~ThreadLocal()
        {
            try
            {
                // Empty
            }
            finally
            {
                Dispose(false);
            }
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        [DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (!disposeManagedResources)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref _disposing, 1, 0) != 0)
            {
                return;
            }

            _wrapped.Dispose();
            _wrapped = null;
        }
    }
}

#endif