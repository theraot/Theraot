#if NET20 || NET30

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

            if (comparer == null)
            {
                comparer = EqualityComparer<TKey>.Default;
            }

            return CreateJoinIterator(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        private static IEnumerable<TResult> CreateJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            // NOTICE this method has no null check
            var innerKeys = ToLookup(inner, innerKeySelector, comparer);

            foreach (var element in outer)
            {
                var outerKey = outerKeySelector(element);
                if (!ReferenceEquals(outerKey, null) && innerKeys.Contains(outerKey))
                {
                    foreach (var innerElement in innerKeys[outerKey])
                    {
                        yield return resultSelector(element, innerElement);
                    }
                }
            }
        }
    }
}

#endif