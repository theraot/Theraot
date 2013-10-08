using System;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public class LazyNeedle<T> : Needle<T>, ICacheNeedle<T>, IEquatable<LazyNeedle<T>>, IPromise<T>
    {
        private int _status;
        private Func<T> _valueFactory;

        private StructNeedle<ManualResetEvent> _waitHandle;

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
            _waitHandle = new StructNeedle<ManualResetEvent>(new ManualResetEvent(false));
            _valueFactory =
                () =>
                {
                    return FullMode(__valueFactory, ref thread);
                };
        }

        ~LazyNeedle()
        {
            var waitHandle = _waitHandle.Value;
            if (!ReferenceEquals(waitHandle, null))
            {
                waitHandle.Close();
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
                return Thread.VolatileRead(ref _status) == 1;
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
            var _obj = obj as LazyNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return base.Equals(obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(LazyNeedle<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            else
            {
                return base.Equals(other as Needle<T>);
            }
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
            _waitHandle.Value.WaitOne();
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
                    SetTarget(_target);
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
    }
}