using System;
using System.Collections.Generic;
using System.Threading;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed class PromiseNeedle<T> : IReadOnlyNeedle<T>, ICacheNeedle<T>, IEquatable<LazyNeedle<T>>
    {
        private T _target;
        private int _isValueCreated;

        private ManualResetEvent _waitHandle;

        public PromiseNeedle(out Action<T> set)
        {
            _waitHandle = new ManualResetEvent(false);
            set = input =>
            {
                _target = input;
                Thread.VolatileWrite(ref _isValueCreated, 1);
                _waitHandle.Set();
            };
        }

        public PromiseNeedle(out Action<T> set, T target)
        {
            Thread.VolatileWrite(ref _isValueCreated, 1);
            _waitHandle = new ManualResetEvent(true);
            _target = target;
            set = input =>
            {
                _target = input;
            };
        }

        public bool IsCached
        {
            get
            {
                return Thread.VolatileRead(ref _isValueCreated) == 1;
            }
        }

        public T Value
        {
            get
            {
                _waitHandle.WaitOne();
                return _target;
            }
        }

        public static explicit operator T(PromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
        }

        public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            var _obj = obj as PromiseNeedle<T>;
            if (!ReferenceEquals(null, _obj))
            {
                return EqualsExtracted(this, _obj);
            }
            else
            {
                return _target.Equals(obj);
            }
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
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
            return EqualityComparer<T>.Default.GetHashCode((T)_target);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (Thread.VolatileRead(ref _isValueCreated) == 0)
            {
                return "{Promise: [Not Created]}";
            }
            else
            {
                return string.Format("{Promise: {0}}", _target);
            }
        }

        private static bool EqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return left._target.Equals(left._target);
            }
        }

        private static bool NotEqualsExtracted(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                if (ReferenceEquals(right, null))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return !left._target.Equals(left._target);
            }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get
            {
                return IsCached;
            }
        }
    }
}
