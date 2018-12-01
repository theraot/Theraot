// Needed for NET40

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public struct StructNeedle<T> : IEquatable<StructNeedle<T>>, IRecyclableNeedle<T>
    {
        public StructNeedle(T target)
        {
            Value = target;
        }

        public bool IsAlive => !ReferenceEquals(Value, null);

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
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return right.IsAlive;
        }

        public static bool operator ==(StructNeedle<T> left, StructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return !right.IsAlive;
        }

        public override bool Equals(object obj)
        {
            if (obj is StructNeedle<T> needle)
            {
                return this == needle;
            }
            // Keep the "is" operator
            if (obj is T variable)
            {
                var target = Value;
                return IsAlive && EqualityComparer<T>.Default.Equals(target, variable);
            }
            return false;
        }

        public bool Equals(StructNeedle<T> other)
        {
            return this == other;
        }

        void IRecyclableNeedle<T>.Free()
        {
            Value = default;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            var target = Value;
            if (IsAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }
    }
}