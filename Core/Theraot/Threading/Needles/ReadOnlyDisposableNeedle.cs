#if FAT

using System.Collections.Generic;
using Theraot.Core;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class ReadOnlyDisposableNeedle<T> : IReadOnlyNeedle<T>
    {
        private readonly int _hashCode;
        private bool _isAlive;
        private T _target;

        public ReadOnlyDisposableNeedle()
        {
            _isAlive = false;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(default(T));
        }

        public ReadOnlyDisposableNeedle(T target)
        {
            _isAlive = true;
            _target = target;
            _hashCode = EqualityComparer<T>.Default.GetHashCode(target);
        }

        public bool IsAlive
        {
            get
            {
                return _isAlive;
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
        }

        public static explicit operator T(ReadOnlyDisposableNeedle<T> needle)
        {
            return Check.NotNullArgument(needle, "needle").Value;
        }

        public static implicit operator ReadOnlyDisposableNeedle<T>(T field)
        {
            return new ReadOnlyDisposableNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left == null && right == null)
            {
                return false;
            }
            if (left == null || right == null)
            {
                return true;
            }
            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null || right == null)
            {
                return false;
            }
            return EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object obj)
        {
            var needle = obj as ReadOnlyDisposableNeedle<T>;
            if (needle != null)
            {
                return EqualityComparer<T>.Default.Equals(_target, needle._target);
            }
            // Keep the "is" operator
            if (obj is T)
            {
                return EqualityComparer<T>.Default.Equals(_target, (T)obj);
            }
            return false;
        }

        public bool Equals(ReadOnlyDisposableNeedle<T> other)
        {
            return !ReferenceEquals(null, other) && EqualityComparer<T>.Default.Equals(_target, other.Value);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            var target = Value;
            if (_isAlive)
            {
                return target.ToString();
            }
            return "<Dead Needle>";
        }

        private void Kill()
        {
            _isAlive = false;
            _target = default(T);
        }
    }
}

#endif