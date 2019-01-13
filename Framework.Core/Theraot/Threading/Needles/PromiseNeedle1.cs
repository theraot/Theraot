// Needed for Workaround

using System;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class PromiseNeedle<T> : Promise, IWaitablePromise<T>, IRecyclableNeedle<T>, ICacheNeedle<T>
    {
        private readonly int _hashCode;
        private T _target;

        public PromiseNeedle(bool done)
            : base(done)
        {
            _target = default;
            _hashCode = base.GetHashCode();
        }

        public PromiseNeedle(Exception exception)
            : base(exception)
        {
            _target = default;
            _hashCode = exception.GetHashCode();
        }

        protected PromiseNeedle(T target)
            : base(true)
        {
            _target = target;
            _hashCode = target == null ? base.GetHashCode() : target.GetHashCode();
        }

        public bool IsAlive => _target != null;

        public virtual T Value
        {
            get
            {
                var exception = Exception;
                if (exception == null)
                {
                    return _target;
                }
                throw exception;
            }
            set
            {
                _target = value;
                SetCompleted();
            }
        }

        public static PromiseNeedle<T> CreateFromValue(T target)
        {
            return new PromiseNeedle<T>(target);
        }

        public static explicit operator T(PromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }
            if (right is null)
            {
                return true;
            }
            return !left._target.Equals(right._target);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (left is null)
            {
                return right is null;
            }
            if (right is null)
            {
                return false;
            }
            return left._target.Equals(right._target);
        }

        public override bool Equals(object obj)
        {
            if (obj is PromiseNeedle<T> needle)
            {
                return _target.Equals(needle._target);
            }
            return IsCompleted && Value.Equals(obj);
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            if (other is null)
            {
                return false;
            }
            return _target.Equals(other._target);
        }

        public override void Free()
        {
            base.Free();
            _target = default;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return IsCompleted
                ? Exception == null
                    ? _target.ToString()
                    : Exception.ToString()
                : "[Not Created]";
        }

        public bool TryGetValue(out T value)
        {
            var result = IsCompleted;
            value = _target;
            return result;
        }
    }
}