#if NET20 || NET30 || NET35

using System.Collections.Generic;
using Theraot.Core;
using Theraot.Threading;

namespace System.Threading
{
    [Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    public sealed class ThreadLocal<T> : IDisposable
    {
        private int _disposing;
        private IThreadLocal<T> _wrapped;

        public ThreadLocal()
            : this(TypeHelper.GetDefault<T>(), false)
        {
            //Empty
        }

        public ThreadLocal(bool trackAllValues)
            : this(TypeHelper.GetDefault<T>(), trackAllValues)
        {
            //Empty
        }

        public ThreadLocal(Func<T> valueFactory)
            : this(valueFactory, false)
        {
            //Empty
        }

        public ThreadLocal(Func<T> valueFactory, bool trackAllValues)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException(nameof(valueFactory));
            }
            if (trackAllValues)
            {
                _wrapped = new TrackingThreadLocal<T>(valueFactory);
            }
            else
            {
                _wrapped = new NoTrackingThreadLocal<T>(valueFactory);
            }
        }

        [Diagnostics.DebuggerNonUserCode]
        ~ThreadLocal()
        {
            try
            {
                //Empty
            }
            finally
            {
                Dispose(false);
            }
        }

        public bool IsValueCreated
        {
            get
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
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
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(ThreadLocal<T>));
                }
                return _wrapped.Value;
            }
            set
            {
                if (Thread.VolatileRead(ref _disposing) == 1)
                {
                    throw new ObjectDisposedException(nameof(ThreadLocal<T>));
                }
                _wrapped.Value = value;
            }
        }

        public IList<T> Values
        {
            get { return _wrapped.Values; }
        }

        internal T ValueForDebugDisplay
        {
            get { return _wrapped.ValueForDebugDisplay; }
        }

        [Diagnostics.DebuggerNonUserCode]
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

        public override string ToString()
        {
            return string.Format(Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        [Diagnostics.DebuggerNonUserCode]
        private void Dispose(bool disposeManagedResources)
        {
            if (disposeManagedResources)
            {
                if (Interlocked.CompareExchange(ref _disposing, 1, 0) == 0)
                {
                    _wrapped.Dispose();
                    _wrapped = null;
                }
            }
        }
    }
}

#endif