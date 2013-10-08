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
        //TODO: thread safety

        private int _cached;
        private Func<T> _function;
        private int _hashCode;

        public CacheNeedle(Func<T> function)
        {
            Thread.VolatileWrite(ref _cached, 0);
            _function = function ?? FuncHelper.GetDefaultFunc<T>();
            _hashCode = base.GetHashCode();
        }

        public CacheNeedle(Func<T> function, bool trackResurrection)
            : base(trackResurrection)
        {
            Thread.VolatileWrite(ref _cached, 0);
            _function = function ?? FuncHelper.GetDefaultFunc<T>();
            _hashCode = base.GetHashCode();
        }

        public CacheNeedle(Func<T> function, T target)
            : base(target)
        {
            Thread.VolatileWrite(ref _cached, 1);
            _function = function ?? FuncHelper.GetDefaultFunc<T>();
            if (ReferenceEquals(target, null))
            {
                _hashCode = base.GetHashCode();
            }
            else
            {
                _hashCode = target.GetHashCode();
            }
        }

        public CacheNeedle(Func<T> function, T target, bool trackResurrection)
            : base(target, trackResurrection)
        {
            Thread.VolatileWrite(ref _cached, 1);
            _function = function ?? FuncHelper.GetDefaultFunc<T>();
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
                return (Thread.VolatileRead(ref _cached) != 0) && IsAlive;
            }
        }

        public override T Value
        {
            get
            {
                if (IsCompleted)
                {
                    return base.Value;
                }
                else
                {
                    var result = _function.Invoke();
                    base.Value = result;
                    return result;
                }
            }
            set
            {
                base.Value = value;
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

        public virtual void InvalidateCache()
        {
            Thread.VolatileWrite(ref _cached, 0);
        }
    }
}