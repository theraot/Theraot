using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Serializable]
    [global::System.Diagnostics.DebuggerNonUserCode]
    public struct ReadOnlyNeedle<T> : IReadOnlyNeedle<T>, IEquatable<ReadOnlyNeedle<T>>
    {
        private readonly T _target;

        public ReadOnlyNeedle(T target)
        {
            _target = target;
        }

        public bool IsAlive
        {
            get
            {
                return true;
            }
        }

        public T Value
        {
            get
            {
                return _target;
            }
        }

        public static bool operator !=(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return !EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public static bool operator ==(ReadOnlyNeedle<T> left, ReadOnlyNeedle<T> right)
        {
            return EqualityComparer<T>.Default.Equals(left._target, right._target);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyNeedle<T>)
            {
                return EqualityComparer<ReadOnlyNeedle<T>>.Default.Equals(this, (ReadOnlyNeedle<T>)obj);
            }
            else
            {
                return false;
            }
        }

        public bool Equals(ReadOnlyNeedle<T> other)
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
    }
}