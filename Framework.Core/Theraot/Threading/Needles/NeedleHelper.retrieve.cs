// Needed for NET40

namespace Theraot.Threading.Needles
{
    public static
#if FAT
        partial
#endif
        class NeedleHelper
    {
        public static bool Retrieve<T, TNeedle>(this TNeedle needle, out T target)
            where TNeedle : IRecyclableNeedle<T>
        {
            if (needle == null)
            {
                target = default;
                return false;
            }
            bool done;
            if (!(needle is ICacheNeedle<T> cacheNeedle))
            {
                target = ((INeedle<T>)needle).Value;
                done = needle.IsAlive;
            }
            else
            {
                done = cacheNeedle.TryGetValue(out target);
            }
            if (done)
            {
                needle.Free();
            }
            return done;
        }
    }
}