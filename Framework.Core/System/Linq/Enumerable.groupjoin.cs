﻿#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            return GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector, comparer: null);
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

            return CreateGroupJoinIterator(comparer ?? EqualityComparer<TKey>.Default);

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