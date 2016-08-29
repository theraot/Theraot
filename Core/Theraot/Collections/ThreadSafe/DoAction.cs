namespace Theraot.Collections.ThreadSafe
{
    internal delegate void DoAction<T>(int capacity, int position, ref int use, ref FixedSizeBucket<T> first, ref FixedSizeBucket<T> second);
}