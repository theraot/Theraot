using System;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading
{
    [System.Diagnostics.DebuggerDisplay("IsValueCreated={IsValueCreated}, Value={ValueForDebugDisplay}")]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class NoTrackingThreadLocal<T> : IDisposable, IThreadLocal<T>
    {
        private int _disposing;
        private LocalDataStoreSlot _slot;
        private Func<T> _valueFactory;

        public NoTrackingThreadLocal()
            : this(TypeHelper.GetCreateOrDefault<T>())
        {
            //Empty
        }

        public NoTrackingThreadLocal(Func<T> valueFactory)
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            _valueFactory = valueFactory;
        }

        [global::System.Diagnostics.DebuggerNonUserCode]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralexceptionTypes", Justification = "Pokemon")]
        ~NoTrackingThreadLocal()
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
                    if (_slot == null)
                    {
                        return false;
                    }
                    else
                    {
                        return Thread.GetData(_slot) is Container;
                    }
                }
            }
        }

        System.Collections.Generic.IList<T> IThreadLocal<T>.Values
        {
            get
            {
                throw new InvalidOperationException();
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
                    if (_slot == null)
                    {
                        _slot = Thread.AllocateDataSlot();
                        T result = _valueFactory.Invoke();
                        Thread.SetData(_slot, new Container(result));
                        return result;
                    }
                    else
                    {
                        var bundle = Thread.GetData(_slot);
                        var container = bundle as Container;
                        if (container == null)
                        {
                            T result = _valueFactory.Invoke();
                            Thread.SetData(_slot, new Container(result));
                            return result;
                        }
                        else
                        {
                            return container.Value;
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
                    if (_slot == null)
                    {
                        _slot = Thread.AllocateDataSlot();
                        Thread.SetData(_slot, new Container(value));
                    }
                    else
                    {
                        var bundle = Thread.GetData(_slot);
                        var container = bundle as Container;
                        if (container == null)
                        {
                            Thread.SetData(_slot, new Container(value));
                        }
                        else
                        {
                            container.Value = value;
                        }
                    }
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
                    _slot = null;
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