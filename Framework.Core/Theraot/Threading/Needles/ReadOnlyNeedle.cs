#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public class ReadOnlyNeedle<T> : IReadOnlyNeedle<T>, IEquatable<ReadOnlyNeedle<T>>
    {
        private readonly INeedle<T> _target;

        public ReadOnlyNeedle()
        {
            _target = null;
        }

        public ReadOnlyNeedle(T target)
        {
            _target = new StructNeedle<T>(target);
        }

        public ReadOnlyNeedle(INeedle<T> target)
        {
            _target = target;
        }

        public bool IsAlive
        {
            get
            {
                var target = _target;
                return _target != null && target.IsAlive;
            }
        }

        public T Value
        {
            get { return (T)_target; }
        }

        public static explicit operator T(ReadOnlyNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            return needle.Value;
        }

        public static implicit operator ReadOnlyNeedle<T>(T field)
        {
            return new ReadOnlyNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return NotEqualsExtracted(left, right);
        }

        public static bool operator ==(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return EqualsExtracted(left, right);
        }

        public override bool Equals(object obj)
        {
            return (obj is ReadOnlyNeedle<T>) ? EqualsExtracted(this, (ReadOnlyNeedle<T>)obj) : _target.Equals(obj);
        }

        public bool Equals(ReadOnlyNeedle<T> other)
        {
            return EqualsExtracted(this, other);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode((T)_target);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private static bool EqualsExtracted(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            if (left == null)
            {
                return right == null;
            }
            return left._target.Equals(right._target);
        }

        private static bool NotEqualsExtracted(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            if (left == null)
            {
                return right != null;
            }
            return !left._target.Equals(right._target);
        }
    }
}

#endif