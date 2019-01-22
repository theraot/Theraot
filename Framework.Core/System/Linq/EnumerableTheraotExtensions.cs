#if NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableTheraotExtensions
    {
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
}

#endif