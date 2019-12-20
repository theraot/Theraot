// Needed for NET40

#pragma warning disable RECS0017 // Possible compare of value type with 'null'
// ReSharper disable ConstantNullCoalescingCondition
// ReSharper disable NonReadonlyMemberInGetHashCode

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct StructNeedle<T> : INeedle<T>, IEquatable<StructNeedle<T>>, IRecyclable
    {
        public StructNeedle(T target)
        {
            Value = target;
        }

        public bool IsAlive => Value != null;

        public T Value { get; set; }

        public static bool operator !=(StructNeedle<T> left, StructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (!left.IsAlive)
            {
                return right.IsAlive;
            }

            var rightValue = right.Value;
            return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
        }

        public static bool operator ==(StructNeedle<T> left, StructNeedle<T> right)
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
                case StructNeedle<T> other:
                    return Equals(other);

                case T otherValue:
                    return Equals(otherValue);

                default:
                    return false;
            }
        }

        public bool Equals(StructNeedle<T> other)
        {
            var value = other.Value;
            if (other.IsAlive)
            {
                return Equals(value);
            }
            return !IsAlive;
        }

        void IRecyclable.Free()
        {
            Value = default!;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(Value);
        }

        public override string ToString()
        {
            var target = Value;
            return IsAlive ? target!.ToString() ?? "<?>" : "<Dead Needle>";
        }

        private bool Equals(T otherValue)
        {
            var value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, otherValue);
        }
    }
}