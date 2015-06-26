#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            return GroupBy(source, keySelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TSource>(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), comparer);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return GroupBy(source, keySelector, elementSelector, null);
        }

        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeyElementSelectors(source, keySelector, elementSelector);

            return CreateGroupByIterator(source, keySelector, elementSelector, comparer);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            return GroupBy(source, keySelector, elementSelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.GroupBySelectors(source, keySelector, elementSelector, resultSelector);

            return CreateGroupByIterator(source, keySelector, elementSelector, resultSelector, comparer);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            return GroupBy(source, keySelector, resultSelector, null);
        }

        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TSource, TResult>(source, keySelector, FuncHelper.GetIdentityFunc<TSource>(), resultSelector, comparer);
        }

        private static IEnumerable<IGrouping<TKey, TElement>> CreateGroupByIterator<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TElement>(source, keySelector, elementSelector, comparer) as IEnumerable<IGrouping<TKey, TElement>>;
        }

        private static IEnumerable<TResult> CreateGroupByIterator<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            return new GroupedEnumerable<TSource, TKey, TElement, TResult>(source, keySelector, elementSelector, resultSelector, comparer) as IEnumerable<TResult>;
        }

        internal class GroupedEnumerable<TSource, TKey, TElement, TResult> : IEnumerable<TResult>
        {
            IEnumerable<TSource> source;
            Func<TSource, TKey> keySelector;
            Func<TSource, TElement> elementSelector;
            IEqualityComparer<TKey> comparer;
            Func<TKey, IEnumerable<TElement>, TResult> resultSelector;

            public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
            {
                if (source == null) throw new ArgumentNullException("source");
                if (keySelector == null) throw new ArgumentNullException("keySelector");
                if (elementSelector == null) throw new ArgumentNullException("elementSelector");
                if (resultSelector == null) throw new ArgumentNullException("resultSelector");
                this.source = source;
                this.keySelector = keySelector;
                this.elementSelector = elementSelector;
                this.comparer = comparer;
                this.resultSelector = resultSelector;
            }

            public IEnumerator<TResult> GetEnumerator()
            {
                Lookup<TKey, TElement> lookup = Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, comparer);
                return lookup.ApplyResultSelector(resultSelector).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        internal class GroupedEnumerable<TSource, TKey, TElement> : IEnumerable<IGrouping<TKey, TElement>>
        {
            IEnumerable<TSource> source;
            Func<TSource, TKey> keySelector;
            Func<TSource, TElement> elementSelector;
            IEqualityComparer<TKey> comparer;

            public GroupedEnumerable(IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
            {
                if (source == null) throw new ArgumentNullException("source");
                if (keySelector == null) throw new ArgumentNullException("keySelector");
                if (elementSelector == null) throw new ArgumentNullException("elementSelector");
                this.source = source;
                this.keySelector = keySelector;
                this.elementSelector = elementSelector;
                this.comparer = comparer;
            }

            public IEnumerator<IGrouping<TKey, TElement>> GetEnumerator()
            {
                return Lookup<TKey, TElement>.Create<TSource>(source, keySelector, elementSelector, comparer).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

    }
}

#endif