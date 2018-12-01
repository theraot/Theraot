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

        public T Value => (T)_target;

        public static explicit operator T(ReadOnlyNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static implicit operator ReadOnlyNeedle<T>(T field)
        {
            return new ReadOnlyNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return !ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return true;
            }
            return !left._target.Equals(right._target);
        }

        public static bool operator ==(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            if (ReferenceEquals(right, null))
            {
                return false;
            }
            return left._target.Equals(right._target);
        }

        public override bool Equals(object obj)
        {
            return (obj is ReadOnlyNeedle<T>) ? this == (ReadOnlyNeedle<T>)obj : _target.Equals(obj);
        }

        public bool Equals(ReadOnlyNeedle<T> other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode((T)_target);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

#endif