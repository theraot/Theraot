using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class LazyNeedle<T> : Needle<T>, ICacheNeedle<T>, IEquatable<LazyNeedle<T>>, IPromise<T>
    {
        private const int INT_StatusCompleted = 1;
        private const int INT_StatusFree = 0;
        private const int INT_StatusWorking = 2;
        private readonly Func<T> _valueFactory;
        private int _status;
        private StructNeedle<ManualResetEventSlim> _waitHandle;

        public LazyNeedle(Func<T> valueFactory)
            : this(valueFactory, default(T))
        {
            //Empty
        }

        public LazyNeedle(Func<T> valueFactory, T target)
            : base(target)
        {
            Func<T> __valueFactory = valueFactory ?? FuncHelper.GetReturnFunc(target);
            Thread thread = null;
            _waitHandle = new StructNeedle<ManualResetEventSlim>(new ManualResetEventSlim(false));
            _valueFactory = () => FullMode(__valueFactory, ref thread);
        }

        ~LazyNeedle()
        {
            var waitHandle = _waitHandle.Value;
            if (!ReferenceEquals(waitHandle, null))
            {
                waitHandle.Dispose();
            }
            _waitHandle.Value = null;
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

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Returns null")]
        Exception IPromise.Error
        {
            get
            {
                return null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref _status) == INT_StatusCompleted;
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
                SetTarget(value);
                Thread.VolatileWrite(ref _status, INT_StatusCompleted);
            }
        }

        protected INeedle<ManualResetEventSlim> WaitHandle
        {
            get
            {
                return _waitHandle;
            }
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as LazyNeedle<T>;
            return !ReferenceEquals(null, _obj) && base.Equals(obj);
        }

        public bool Equals(LazyNeedle<T> other)
        {
            return !ReferenceEquals(other, null) && base.Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public virtual void Initialize()
        {
            _valueFactory.Invoke();
        }

        public void Wait()
        {
            _waitHandle.Value.Wait();
        }

        protected virtual void Initialize(Action beforeInitialize)
        {
            var _beforeInitialize = Check.NotNullArgument(beforeInitialize, "beforeInitialize");
            if (Thread.VolatileRead(ref _status) == INT_StatusFree)
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
            if (Interlocked.CompareExchange(ref _status, INT_StatusWorking, INT_StatusFree) == INT_StatusFree)
            {
                try
                {
                    thread = Thread.CurrentThread;
                    GC.KeepAlive(thread);
                    var _target = valueFactory.Invoke();
                    SetTarget(_target);
                    Thread.VolatileWrite(ref _status, INT_StatusCompleted);
                    return _target;
                }
                catch (Exception)
                {
                    Thread.VolatileWrite(ref _status, INT_StatusFree);
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
                    _waitHandle.Value.Wait();
                    if (Thread.VolatileRead(ref _status) == INT_StatusCompleted)
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
    }
}