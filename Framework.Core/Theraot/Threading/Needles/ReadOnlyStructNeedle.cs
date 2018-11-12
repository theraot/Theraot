// Needed for NET35 (ThreadLocal)

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public struct ReadOnlyStructNeedle<T> : INeedle<T>, IEquatable<ReadOnlyStructNeedle<T>>
    {
        private readonly T _value;

        public ReadOnlyStructNeedle(T target)
        {
            _value = target;
        }

        T INeedle<T>.Value
        {
            get { return _value; }

            set { throw new NotSupportedException(); }
        }

        public bool IsAlive
        {
            get { return !ReferenceEquals(_value, null); }
        }

        public T Value
        {
            get { return _value; }
        }

        public static explicit operator T(ReadOnlyStructNeedle<T> needle)
        {
            return needle._value;
        }

        public static implicit operator ReadOnlyStructNeedle<T>(T field)
        {
            return new ReadOnlyStructNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var leftValue = left._value;
            if (left.IsAlive)
            {
                var rightValue = right._value;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return right.IsAlive;
        }

        public static bool operator ==(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var leftValue = left._value;
            if (left.IsAlive)
            {
                var rightValue = right._value;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return !right.IsAlive;
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyStructNeedle<T>)
            {
                return this == (ReadOnlyStructNeedle<T>)obj;
            }
            // Keep the "is" operator
            if (obj is T)
            {
                var target = _value;
                return IsAlive && EqualityComparer<T>.Default.Equals(target, (T)obj);
            }
            return false;
        }

        public bool Equals(ReadOnlyStructNeedle<T> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            if (IsAlive)
            {
                return _value.ToString();
            }
            return "<Dead Needle>";
        }
    }
}