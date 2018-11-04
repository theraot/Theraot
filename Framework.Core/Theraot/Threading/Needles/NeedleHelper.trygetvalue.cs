// Needed for NET40

namespace Theraot.Threading.Needles
{
    public static partial class NeedleHelper
    {
        public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
        {
            if (needle == null)
            {
                target = default(T);
                return false;
            }
            var cacheNeedle = needle as ICacheNeedle<T>;
            if (cacheNeedle != null)
            {
                return cacheNeedle.TryGetValue(out target);
            }
            target = ((INeedle<T>)needle).Value;
            return needle.IsAlive;
        }
    }
}