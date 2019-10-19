#if LESSTHAN_NET35

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey>? comparer)
        {
            if (outer == null)
            {
                throw new ArgumentNullException(nameof(outer));
            }
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }
            if (outerKeySelector == null)
            {
                throw new ArgumentNullException(nameof(outerKeySelector));
            }
            if (innerKeySelector == null)
            {
                throw new ArgumentNullException(nameof(innerKeySelector));
            }
            if (resultSelector == null)
            {
                throw new ArgumentNullException(nameof(resultSelector));
            }
            if (comparer == null)
            {
                return CreateGroupJoinIterator(EqualityComparer<TKey>.Default);
            }
            return CreateGroupJoinIterator(comparer);

            IEnumerable<TResult> CreateGroupJoinIterator(IEqualityComparer<TKey> notNullComparer)
            {
                var innerKeys = ToLookup(inner, innerKeySelector, notNullComparer);

                foreach (var element in outer)
                {
                    var outerKey = outerKeySelector(element);
                    if (outerKey != null && innerKeys.Contains(outerKey))
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
}

#endif