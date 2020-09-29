using System.Runtime.CompilerServices;
using System.Linq;

namespace System.Collections.Generic
{
    internal static class SetEqualityComparer<T>
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool Equals(ICollection<T>? x, ICollection<T>? y, IEqualityComparer<T>? memberEqualityComparer)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null || x.Count != y.Count)
            {
                return false;
            }

            if (memberEqualityComparer == null)
            {
                foreach (var item in x)
                {
                    if (!y.Contains(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                foreach (var item in x)
                {
                    if (!y.Contains(item, memberEqualityComparer))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static int GetHashCode(ICollection<T> obj, IEqualityComparer<T>? memberEqualityComparer)
        {
            if (obj == null)
            {
                return 0;
            }

            var cmp = memberEqualityComparer ?? EqualityComparer<T>.Default;
            var h = 0;
            foreach (var t in obj)
            {
                int next;
                try
                {
                    next = cmp.GetHashCode(t!);
                }
                catch (ArgumentNullException)
                {
                    next = 0;
                }

                h ^= next;
            }

            return h;
        }
    }
}