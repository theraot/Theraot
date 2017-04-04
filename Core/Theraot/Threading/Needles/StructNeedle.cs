// Needed for NET40

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    public struct StructNeedle<T> : IEquatable<StructNeedle<T>>, IRecyclableNeedle<T>
    {
        private T _value;

        public StructNeedle(T target)
        {
            _value = target;
        }

        public bool IsAlive
        {
            get
            {
                return !ReferenceEquals(Value, null);
            }
        }

        public T Value
        {
            get
            {
                // Keep backing field
                return _value;
            }
            set
            {
                // Keep backing field
                _value = value;
            }
        }

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
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(StructNeedle<T> left, StructNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            if (obj is StructNeedle<T>)
            {
                return EqualsExtracted(this, (StructNeedle<T>)obj);
            }
            // Keep the "is" operator
            if (obj is T)
            {
                var target = Value;
                return IsAlive && EqualityComparer<T>.Default.Equals(target, (T)obj);
            }
            return false;
        }

        public bool Equals(StructNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        void IRecyclableNeedle<T>.Free()
        {
            Value = default(T);
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

        private static bool EqualsExtracted(StructNeedle<T> left, StructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return right.IsAlive && EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return !right.IsAlive;
        }

        private static bool NotEqualsExtracted(StructNeedle<T> left, StructNeedle<T> right)
        {
            var leftValue = left.Value;
            if (left.IsAlive)
            {
                var rightValue = right.Value;
                return !right.IsAlive || !EqualityComparer<T>.Default.Equals(leftValue, rightValue);
            }
            return right.IsAlive;
        }
    }
}