// Needed for NET40

#pragma warning disable CS0659 // Type overrides Object.Equals but does not override GetHashCode.
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct StructNeedle<T> : IEquatable<StructNeedle<T>>, IRecyclable<T>
    {
        public StructNeedle(T target)
        {
            Value = target;
        }

        public bool IsAlive => Value != null;

        public T Value { get; set; }

        public static explicit operator T(StructNeedle<T> needle)
        {
            return needle.Value;
        }

        public static implicit operator StructNeedle<T>(T field)
        {
            return new StructNeedle<T>(field);
        }

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

        public override bool Equals(object obj)
        {
            if (obj is StructNeedle<T> right)
            {
                if (!right.IsAlive)
                {
                    return !IsAlive;
                }

                obj = right.Value;
            }

            if (!(obj is T rightValue))
            {
                return false;
            }

            var value = Value;
            return IsAlive && EqualityComparer<T>.Default.Equals(value, rightValue);
        }

        public bool Equals(StructNeedle<T> other)
        {
            var leftValue = Value;
            if (!IsAlive)
            {
                return !other.IsAlive;
            }

            var rightValue = other.Value;
            return other.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
        }

        void IRecyclable<T>.Free()
        {
            Value = default;
        }

        public override string ToString()
        {
            var target = Value;
            return IsAlive ? target.ToString() : "<Dead Needle>";
        }
    }
}