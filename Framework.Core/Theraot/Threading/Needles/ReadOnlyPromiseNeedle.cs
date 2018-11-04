// Needed for Workaround

using System;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class ReadOnlyPromiseNeedle<T> : ReadOnlyPromise, IWaitablePromise<T>, ICacheNeedle<T>, IEquatable<ReadOnlyPromiseNeedle<T>>
    {
        private readonly ICacheNeedle<T> _promised;

        public ReadOnlyPromiseNeedle(ICacheNeedle<T> promised, bool allowWait)
            : base(promised, allowWait)
        {
            _promised = promised;
        }

        T INeedle<T>.Value
        {
            get { return _promised.Value; }

            set { throw new NotSupportedException(); }
        }

        public bool IsAlive
        {
            get { return _promised.IsAlive; }
        }

        public T Value
        {
            get { return _promised.Value; }
        }

        public static bool operator !=(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public static explicit operator T(ReadOnlyPromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            return needle.Value;
        }

        public override bool Equals(object obj)
        {
            var needle = obj as ReadOnlyPromiseNeedle<T>;
            if (obj != null)
            {
                return EqualsExtracted(this, needle);
            }
            return _promised.IsCompleted && _promised.Value.Equals(null);
        }

        public bool Equals(ReadOnlyPromiseNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{{Promise: {0}}}", _promised);
        }

        public bool TryGetValue(out T value)
        {
            return _promised.TryGetValue(out value);
        }

        private static bool EqualsExtracted(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left.Equals(right);
        }

        private static bool NotEqualsExtracted(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            if (left == null)
            {
                return right != null;
            }
            return !left.Equals(right);
        }
    }
}