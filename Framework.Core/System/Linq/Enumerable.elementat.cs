#if LESSTHAN_NET35

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static TSource ElementAt<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "index < 0");
            }

            switch (source)
            {
                case IList<TSource> list:
                    return list[index];

                case IReadOnlyList<TSource> readOnlyList:
                    return readOnlyList[index];

                default:
                    var count = 0L;
                    foreach (var item in source)
                    {
                        if (index == count)
                        {
                            return item;
                        }

                        count++;
                    }

                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        [return: MaybeNull]
        public static TSource ElementAtOrDefault<TSource>(this IEnumerable<TSource> source, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (index < 0)
            {
                return default;
            }

            switch (source)
            {
                case IList<TSource> list:
                    return index < list.Count ? list[index] : default;

                case IReadOnlyList<TSource> readOnlyList:
                    return index < readOnlyList.Count ? readOnlyList[index] : default;

                default:
                    var count = 0L;
                    foreach (var item in source)
                    {
                        if (index == count)
                        {
                            return item;
                        }

                        count++;
                    }

                    return default;
            }
        }
    }
}

#endif