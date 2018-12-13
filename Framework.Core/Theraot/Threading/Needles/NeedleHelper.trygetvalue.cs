#if FAT

namespace Theraot.Threading.Needles
{
    public static partial class NeedleHelper
    {
        public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
        {
            if (needle == null)
            {
                target = default;
                return false;
            }
            if (needle is ICacheNeedle<T> cacheNeedle)
            {
                return cacheNeedle.TryGetValue(out target);
            }
            target = ((INeedle<T>)needle).Value;
            return needle.IsAlive;
        }
    }
}

#endif