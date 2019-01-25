// Needed for NET40

namespace Theraot.Threading.Needles
{
    public static class NeedleHelper
    {
        public static bool TryGetValue<T>(this IReadOnlyNeedle<T> needle, out T target)
        {
            switch (needle)
            {
                case null:
                    target = default;
                    return false;
                case ICacheNeedle<T> cacheNeedle:
                    return cacheNeedle.TryGetValue(out target);
                default:
                    target = needle.Value;
                    return needle.IsAlive;
            }
        }
    }
}