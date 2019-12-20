// Needed for NET35 (ThreadLocal)

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
// ReSharper disable ConstantNullCoalescingCondition

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct ReadOnlyStructNeedle<T> : INeedle<T>, IEquatable<ReadOnlyStructNeedle<T>>
    {
        public ReadOnlyStructNeedle(T target)
        {
            Value = target;
        }

        public bool IsAlive => Value != null;

        public T Value { get; }

        T INeedle<T>.Value { get => Value; set => throw new NotSupportedException(); }

        public static bool operator !=(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (!left.IsAlive)
            {
                return right.IsAlive;
            }

            var rightValue = right.Value;
            return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
        }

        public static bool operator ==(ReadOnlyStructNeedle<T> left, ReadOnlyStructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (!left.IsAlive)
            {
                return !right.IsAlive;
            }

            var rightValue = right.Value;
            return right.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
        }

        public override bool Equals(object? obj)
        {
            switch (obj)
            {
                case ReadOnlyStructNeedle<T> other:
                    return Equals(other);

                case T otherValue:
                    return Equals(otherValue);

                default:
                    return false;
            }
        }

        public bool Equals(ReadOnlyStructNeedle<T> other)
        {
            if (other.TryGetValue(out var value))
            {
                return Equals(value);
            }
            return !IsAlive;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            var value = Value;
            return IsAlive ? value!.ToString() ?? "<?>" : "<Dead Needle>";
        }

        private bool Equals(T otherValue)
        {
            var value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
        }
    }
}