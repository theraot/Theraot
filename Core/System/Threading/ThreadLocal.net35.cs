#if NET20 || NET30 || NET35

using Theraot.Core;
using Theraot.Threading;

namespace System.Threading
{
    [System.Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    public sealed partial class ThreadLocal<T> : IDisposable
    {
        private int _disposing;
        private IThreadLocal<T> _wrapped;

        public ThreadLocal()
            : this(TypeHelper.GetCreateOrDefault<T>(), false)
        {
            //Empty
        }

        public ThreadLocal(bool trackAllValues)
            : this(TypeHelper.GetCreateOrDefault<T>(), trackAllValues)
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
                throw new ArgumentNullException("valueFactory");
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

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~ThreadLocal()
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
                    return _wrapped.IsValueCreated;
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
                    return _wrapped.Value;
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
                    _wrapped.Value = value;
                }
            }
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

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "[ThreadLocal: IsValueCreated={0}, Value={1}]", IsValueCreated, Value);
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive", Justification = "By Design")]
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

#endif