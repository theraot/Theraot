// Needed for Workaround

using System;
using System.Diagnostics;

namespace Theraot.Threading.Needles
{
    [DebuggerNonUserCode]
    public class ReadOnlyPromiseNeedle<T> : ReadOnlyPromise, IWaitablePromise<T>, ICacheNeedle<T>, IEquatable<ReadOnlyPromiseNeedle<T>>
    {
        private readonly ICacheNeedle<T> _promised;

        public ReadOnlyPromiseNeedle(ICacheNeedle<T> promised, bool allowWait)
            : base(promised, allowWait)
        {
            _promised = promised;
        }

        public bool IsAlive => _promised.IsAlive;

        T INeedle<T>.Value
        {
            get => _promised.Value;

            set => throw new NotSupportedException();
        }

        public T Value => _promised.Value;

        public static explicit operator T(ReadOnlyPromiseNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static bool operator !=(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            if (left is null)
            {
                return !(right is null);
            }
            if (right is null)
            {
                return true;
            }
            return !left._promised.Equals(right._promised);
        }

        public static bool operator ==(ReadOnlyPromiseNeedle<T> left, ReadOnlyPromiseNeedle<T> right)
        {
            if (left is null)
            {
                return right is null;
            }
            return !(right is null) && left._promised.Equals(right._promised);
        }

        public override bool Equals(object obj)
        {
            if (obj is ReadOnlyPromiseNeedle<T> needle)
            {
                return _promised.Equals(needle._promised);
            }
            return _promised.IsCompleted && _promised.Value.Equals(null);
        }

        public bool Equals(ReadOnlyPromiseNeedle<T> other)
        {
            return !(other is null) && _promised.Equals(other._promised);
        }

        public override int GetHashCode()
        {
            return _promised.GetHashCode();
        }

        public override string ToString()
        {
            return $"{{Promise: {_promised}}}";
        }

        public bool TryGetValue(out T value)
        {
            return _promised.TryGetValue(out value);
        }
    }
}