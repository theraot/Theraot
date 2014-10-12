#if NET20 || NET30

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.JoinSelectors(outer, inner, outerKeySelector, innerKeySelector, resultSelector);

            if (comparer == null)
            {
                comparer = EqualityComparer<TKey>.Default;
            }

            return CreateGroupJoinIterator(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer);
        }

        private static IEnumerable<TResult> CreateGroupJoinIterator<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            ILookup<TKey, TInner> innerKeys = ToLookup(inner, innerKeySelector, comparer);

            foreach (TOuter element in outer)
            {
                TKey outerKey = outerKeySelector(element);
                if (!ReferenceEquals(outerKey, null) && innerKeys.Contains(outerKey))
                {
                    yield return resultSelector(element, innerKeys[outerKey]);
                }
                else
                {
                    yield return resultSelector(element, Empty<TInner>());
                }
            }
        }
    }
}

#endif