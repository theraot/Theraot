#if FAT

using System;
using System.Collections.Generic;

namespace Theraot.Threading.Needles
{
    [System.Diagnostics.DebuggerNonUserCode]
    public sealed class DefaultNeedle<T> : IReadOnlyNeedle<T>
    {
        private static readonly DefaultNeedle<T> _instance = new DefaultNeedle<T>();

        private DefaultNeedle()
        {
            //Empty
        }

        public static DefaultNeedle<T> Instance
        {
            get { return _instance; }
        }

        bool IReadOnlyNeedle<T>.IsAlive
        {
            get { return true; }
        }

        public T Value
        {
            get { return default(T); }
        }

        public static explicit operator T(DefaultNeedle<T> needle)
        {
            if (needle == null)
            {
                throw new ArgumentNullException("needle");
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
            return EqualityComparer<T>.Default.GetHashCode(default(T));
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}

#endif