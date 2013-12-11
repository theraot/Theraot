using System;
using System.Threading;

using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public partial class CacheNeedle<T> : WeakNeedle<T>, ICacheNeedle<T>
        where T : class
    {
        private readonly int _hashCode;
        private int _status;
        private Func<T> _valueFactory;
        private StructNeedle<ManualResetEvent> _waitHandle;

        public CacheNeedle(Func<T> valueFactory)
            : this(valueFactory, false)
        {
            //Empty
        }

        public CacheNeedle(Func<T> valueFactory, bool trackResurrection)
            : this(valueFactory, default(T), trackResurrection)
        {
            //Empty
        }

        public CacheNeedle(Func<T> valueFactory, T target)
            : this(valueFactory, target, false)
        {
            //Empty
        }

        public CacheNeedle(Func<T> valueFactory, T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
            Func<T> __valueFactory = valueFactory ?? FuncHelper.GetReturnFunc(target);
            Thread thread = null;
            _waitHandle = new StructNeedle<ManualResetEvent>(new ManualResetEvent(false));
            _valueFactory = () => FullMode(__valueFactory, ref thread);
            if (ReferenceEquals(target, null))
            {
                _hashCode = base.GetHashCode();
            }
            else
            {
                _hashCode = target.GetHashCode();
            }
        }

        public T CachedTarget
        {
            get
            {
                return base.Value;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IExpected.IsCanceled
        {
            get
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns false")]
        bool IExpected.IsFaulted
        {
            get
            {
                return false;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return (Thread.VolatileRead(ref _status) != 0) && IsAlive;
            }
        }

        public override T Value
        {
            get
            {
                Initialize();
                return base.Value;
            }
            set
            {
                Allocate(value, TrackResurrection);
                Thread.VolatileWrite(ref _status, 1);
            }
        }

        protected INeedle<ManualResetEvent> WaitHandle
        {
            get
            {
                return _waitHandle;
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as CacheNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return object.Equals(_obj.Value, Value);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public virtual void Initialize()
        {
            _valueFactory.Invoke();
        }

        public virtual void InvalidateCache()
        {
            Thread.VolatileWrite(ref _status, 0);
        }

        protected virtual void Initialize(Action beforeInitialize)
        {
            var _beforeInitialize = Check.NotNullArgument(beforeInitialize, "beforeInitialize");
            if (Thread.VolatileRead(ref _status) == 0)
            {
                try
                {
                    _beforeInitialize.Invoke();
                }
                finally
                {
                    _valueFactory.Invoke();
                }
            }
        }

        private T FullMode(Func<T> valueFactory, ref Thread thread)
        {
        back:
            if (Interlocked.CompareExchange(ref _status, 2, 0) == 0)
            {
                try
                {
                    thread = Thread.CurrentThread;
                    var _target = valueFactory.Invoke();
                    Allocate(_target, TrackResurrection);
                    Thread.VolatileWrite(ref _status, 1);
                    return _target;
                }
                catch (Exception)
                {
                    Thread.VolatileWrite(ref _status, 0);
                    throw;
                }
                finally
                {
                    _waitHandle.Value.Set();
                    thread = null;
                }
            }
            else
            {
                if (ReferenceEquals(thread, Thread.CurrentThread))
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    _waitHandle.Value.WaitOne();
                    if (Thread.VolatileRead(ref _status) == 1)
                    {
                        return base.Value;
                    }
                    else
                    {
                        goto back;
                    }
                }
            }
        }

        private void UnmanagedDispose()
        {
            var waitHandle = _waitHandle.Value;
            if (!ReferenceEquals(waitHandle, null))
            {
                waitHandle.Close();
            }
            _waitHandle.Value = null;
        }
    }
}