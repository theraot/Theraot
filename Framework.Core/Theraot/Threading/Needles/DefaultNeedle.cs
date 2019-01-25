#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class DefaultNeedle<T> : IReadOnlyNeedle<T>
    {
        private DefaultNeedle()
        {
            // Empty
        }

        public static DefaultNeedle<T> Instance { get; } = new DefaultNeedle<T>();

        bool IReadOnlyNeedle<T>.IsAlive => true;

        public T Value => default;

        public static explicit operator T(DefaultNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException(nameof(needle));
            }
            return needle.Value;
        }

        public static bool operator !=(DefaultNeedle<T> left, DefaultNeedle<T> right)
        {
            return false;
        }

        public static bool operator ==(DefaultNeedle<T> left, DefaultNeedle<T> right)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return obj is DefaultNeedle<T>;
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(default);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

#endif