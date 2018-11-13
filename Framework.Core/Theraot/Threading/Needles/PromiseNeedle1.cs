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
            _hashCode = ReferenceEquals(target, null) ? base.GetHashCode() : target.GetHashCode();
        }

        public bool IsAlive
        {
            get { return !ReferenceEquals(_target, null); }
        }

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

        public static bool operator !=(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            return !left._target.Equals(right._target);
        }

        public static bool operator ==(PromiseNeedle<T> left, PromiseNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left._target.Equals(right._target);
        }

        public static explicit operator T(PromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public override bool Equals(object obj)
        {
            var needle = obj as PromiseNeedle<T>;
            if (needle != null)
            {
                return this == needle;
            }
            return IsCompleted && Value.Equals(obj);
        }

        public bool Equals(PromiseNeedle<T> other)
        {
            return this == other;
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
                ? (Exception == null
                    ? _target.ToString()
                    : Exception.ToString())
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