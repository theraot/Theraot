#if NET20 || NET30 || NET35

using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
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
            return ZipExtracted();

            IEnumerable<TResult> ZipExtracted()
            {
                using (var enumeratorFirst = first.GetEnumerator())
                using (var enumeratorSecond = second.GetEnumerator())
                {
                    while
                    (
                        enumeratorFirst.MoveNext()
                        && enumeratorSecond.MoveNext()
                    )
                    {
                        yield return resultSelector
                        (
                            enumeratorFirst.Current,
                            enumeratorSecond.Current
                        );
                    }
                }
            }
        }
    }
}

#endif