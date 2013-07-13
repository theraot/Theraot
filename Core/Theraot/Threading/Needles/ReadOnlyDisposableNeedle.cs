using System;
using System.Collections.Generic;
namespace Theraot.Threading.Needles
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public sealed partial class ReadOnlyDisposableNeedle<T> : IReadOnlyNeedle<T>
    {
        private bool _isAlive;
        private T _target;

        public ReadOnlyDisposableNeedle()
        {
            _isAlive = false;
        }

        public ReadOnlyDisposableNeedle(T target)
        {
            _isAlive = true;
            _target = target;
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
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
            }
            else
            {
                return needle.Value;
            }
        }

        public static implicit operator ReadOnlyDisposableNeedle<T>(T field)
        {
            return new ReadOnlyDisposableNeedle<T>(field);
        }

        public static bool operator !=(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(ReadOnlyDisposableNeedle<T> left, ReadOnlyDisposableNeedle<T> right)
        {
            return EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyDisposableNeedle<T>)
            {
                return EqualityComparer<T>.Default.Equals(_target, ((ReadOnlyDisposableNeedle<T>)obj)._target);
            }
            else
            {
                if (obj is T)
                {
                    return EqualityComparer<T>.Default.Equals(_target, (T)obj);
                }
                else
                {
                    return false;
                }
            }
        }

        public bool Equals(ReadOnlyDisposableNeedle<T> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            else
            {
                return EqualityComparer<T>.Default.Equals(_target, other.Value);
            }
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(_target);
        }

        private void Kill()
        {
            _isAlive = false;
            _target = default(T);
        }
    }
}
