#if GREATERTHAN_NET30 || TARGETS_NETCORE || TARGETS_NETSTANDARD

using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableTheraotExtensions
    {
#if LESSTHAN_NET40
        public static IEnumerable<TReturn> Zip<T1, T2, TReturn>(this IEnumerable<T1> first, IEnumerable<T2> second, Func<T1, T2, TReturn> resultSelector)
        {
            if (first == null)
            {
                throw new ArgumentNullException(nameof(first));
            }
            if (second == null)
            {
                throw new ArgumentNullException(nameof(second));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            return ZipIterator();
            IEnumerable<TReturn> ZipIterator()
            {
                using (var enumerator1 = first.GetEnumerator())
                using (var enumerator2 = second.GetEnumerator())
                {
                    while
                    (
                        enumerator1.MoveNext()
                        && enumerator2.MoveNext()
                    )
                    {
                        yield return resultSelector
                        (
                            enumerator1.Current,
                            enumerator2.Current
                        );
                    }
                }
            }
        }
#endif
#if TARGETS_NET || LESSTHAN_NETSTANDARD16
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
#endif
    }
}

#endif