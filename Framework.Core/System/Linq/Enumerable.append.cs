#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource element)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Appended();

            IEnumerable<TSource> Appended()
            {
                yield return element;
                foreach (var item in source)
                {
                    yield return item;
                }
            }
        }
    }
}

#endif