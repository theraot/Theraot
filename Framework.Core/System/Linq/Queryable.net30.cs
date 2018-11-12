#if NET20 || NET30

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Theraot.Core;

namespace System.Linq
{
    public static class Queryable
    {
        public static TSource Aggregate<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func)
        {
            LinqCheck.SourceAndFunc(source, func);
            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(func)));
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func)
        {
            LinqCheck.SourceAndFunc(source, func);
            return source.Provider.Execute<TAccumulate>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TAccumulate)),
                           source.Expression,
                           Expression.Constant(seed, typeof(TAccumulate)),
                           Expression.Quote(func)));
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this IQueryable<TSource> source, TAccumulate seed, Expression<Func<TAccumulate, TSource, TAccumulate>> func, Expression<Func<TAccumulate, TResult>> selector)
        {
            LinqCheck.SourceAndFuncAndSelector(source, func, selector);
            return source.Provider.Execute<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TAccumulate), typeof(TResult)),
                           source.Expression,
                           Expression.Constant(seed, typeof(TAccumulate)),
                           Expression.Quote(func),
                           Expression.Quote(selector)));
        }

        public static bool All<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static bool Any<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static bool Any<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TElement> AsQueryable<TElement>(this IEnumerable<TElement> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var queryable = source as IQueryable<TElement>;
            if (queryable != null)
            {
                return queryable;
            }

            return new QueryableEnumerable<TElement>(source);
        }

        public static IQueryable AsQueryable(this IEnumerable source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var queryable = source as IQueryable;
            if (queryable != null)
            {
                return queryable;
            }

            Type ienumerable;
            if (!source.GetType().IsGenericImplementationOf(out ienumerable, typeof(IEnumerable<>)))
            {
                throw new ArgumentException("source is not IEnumerable<>");
            }

            return (IQueryable)Activator.CreateInstance(
                       typeof(QueryableEnumerable<>).MakeGenericType(ienumerable.GetFirstGenericArgument()), source);
        }

        public static double Average(this IQueryable<int> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double? Average(this IQueryable<int?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double Average(this IQueryable<long> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double? Average(this IQueryable<long?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static float Average(this IQueryable<float> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<float>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static float? Average(this IQueryable<float?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<float?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double Average(this IQueryable<double> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double? Average(this IQueryable<double?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static decimal Average(this IQueryable<decimal> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<decimal>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static decimal? Average(this IQueryable<decimal?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<decimal?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double? Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double? Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static float Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<float>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static float? Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<float?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double? Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static decimal Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<decimal>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static decimal? Average<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<decimal?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> Cast<TResult>(this IQueryable source)
        {
            LinqCheck.Source(source);

            return (IQueryable<TResult>)source.Provider.CreateQuery(StaticCall(MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TResult)), source.Expression));
        }

        public static IQueryable<TSource> Concat<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>))));
        }

        public static bool Contains<TSource>(this IQueryable<TSource> source, TSource item)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(item, typeof(TSource))));
        }

        public static bool Contains<TSource>(this IQueryable<TSource> source, TSource item, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(item, typeof(TSource)),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static int Count<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Execute<int, TSource>(MethodBase.GetCurrentMethod());
        }

        public static int Count<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<int>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TSource> DefaultIfEmpty<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static IQueryable<TSource> DefaultIfEmpty<TSource>(this IQueryable<TSource> source, TSource defaultValue)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(defaultValue, typeof(TSource))));
        }

        public static IQueryable<TSource> Distinct<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static IQueryable<TSource> Distinct<TSource>(this IQueryable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static TSource ElementAt<TSource>(this IQueryable<TSource> source, int index)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(index)));
        }

        public static TSource ElementAtOrDefault<TSource>(this IQueryable<TSource> source, int index)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(index)));
        }

        public static IQueryable<TSource> Except<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>))));
        }

        public static IQueryable<TSource> Except<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>)),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static TSource First<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource First<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static TSource FirstOrDefault<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource FirstOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector)));
        }

        public static IQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return source.Provider.CreateQuery<IGrouping<TKey, TSource>>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TKey>))));
        }

        public static IQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector)
        {
            LinqCheck.SourceAndKeyElementSelectors(source, keySelector, elementSelector);

            return source.Provider.CreateQuery<IGrouping<TKey, TElement>>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TElement)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(elementSelector)));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
        {
            LinqCheck.SourceAndKeyResultSelectors(source, keySelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(resultSelector)));
        }

        public static IQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeyElementSelectors(source, keySelector, elementSelector);

            return source.Provider.CreateQuery<IGrouping<TKey, TElement>>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TElement)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(elementSelector),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TKey>))));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
        {
            LinqCheck.GroupBySelectors(source, keySelector, elementSelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TElement), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(elementSelector),
                           Expression.Quote(resultSelector)));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeyResultSelectors(source, keySelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(resultSelector),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TKey>))));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
        {
            LinqCheck.GroupBySelectors(source, keySelector, elementSelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey), typeof(TElement), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Quote(elementSelector),
                           Expression.Quote(resultSelector),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TKey>))));
        }

        public static IQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
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

            return outer.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TOuter), typeof(TInner), typeof(TKey), typeof(TResult)),
                           outer.Expression,
                           Expression.Constant(inner, typeof(IEnumerable<TInner>)),
                           Expression.Quote(outerKeySelector),
                           Expression.Quote(innerKeySelector),
                           Expression.Quote(resultSelector)));
        }

        public static IQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
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

            return outer.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TOuter), typeof(TInner), typeof(TKey), typeof(TResult)),
                           outer.Expression,
                           Expression.Constant(inner, typeof(IEnumerable<TInner>)),
                           Expression.Quote(outerKeySelector),
                           Expression.Quote(innerKeySelector),
                           Expression.Quote(resultSelector),
                           Expression.Constant(comparer)));
        }

        public static IQueryable<TSource> Intersect<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>))));
        }

        public static IQueryable<TSource> Intersect<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>)),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
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
            return outer.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TOuter), typeof(TInner), typeof(TKey), typeof(TResult)),
                           outer.Expression,
                           Expression.Constant(inner, typeof(IEnumerable<TInner>)),
                           Expression.Quote(outerKeySelector),
                           Expression.Quote(innerKeySelector),
                           Expression.Quote(resultSelector)));
        }

        public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector, IEqualityComparer<TKey> comparer)
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
            return outer.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TOuter), typeof(TInner), typeof(TKey), typeof(TResult)),
                           outer.Expression,
                           Expression.Constant(inner, typeof(IEnumerable<TInner>)),
                           Expression.Quote(outerKeySelector),
                           Expression.Quote(innerKeySelector),
                           Expression.Quote(resultSelector),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TKey>))));
        }

        public static TSource Last<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource Last<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static TSource LastOrDefault<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource LastOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static long LongCount<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Execute<long, TSource>(MethodBase.GetCurrentMethod());
        }

        public static long LongCount<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<long>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static TSource Max<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TResult Max<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static TSource Min<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TResult Min<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> OfType<TResult>(this IQueryable source)
        {
            LinqCheck.Source(source);

            return (IQueryable<TResult>)source.Provider.CreateQuery(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TResult)),
                           source.Expression));
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Constant(comparer, typeof(IComparer<TKey>))));
        }

        public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> OrderByDescending<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Constant(comparer, typeof(IComparer<TKey>))));
        }

        public static IQueryable<TSource> Reverse<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> Select<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, TResult>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> SelectMany<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> SelectMany<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TResult>>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static IQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, int, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            LinqCheck.SourceAndCollectionSelectorAndResultSelector(source, collectionSelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TCollection), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(collectionSelector),
                           Expression.Quote(resultSelector)));
        }

        public static IQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            LinqCheck.SourceAndCollectionSelectorAndResultSelector(source, collectionSelector, resultSelector);

            return source.Provider.CreateQuery<TResult>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TCollection), typeof(TResult)),
                           source.Expression,
                           Expression.Quote(collectionSelector),
                           Expression.Quote(resultSelector)));
        }

        public static bool SequenceEqual<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>))));
        }

        public static bool SequenceEqual<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.Execute<bool>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>)),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static TSource Single<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource Single<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static TSource SingleOrDefault<TSource>(this IQueryable<TSource> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression));
        }

        public static TSource SingleOrDefault<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.Execute<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TSource> Skip<TSource>(this IQueryable<TSource> source, int count)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(count)));
        }

        public static IQueryable<TSource> SkipWhile<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TSource> SkipWhile<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static int Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<int>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static int? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<int?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static long Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<long>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static long? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<long?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static float Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<float>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static float? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, float?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<float?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static double? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, double?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static decimal Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<decimal>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static decimal? Sum<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, decimal?>> selector)
        {
            LinqCheck.SourceAndSelector(source, selector);

            return source.Provider.Execute<decimal?>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(selector)));
        }

        public static int Sum(this IQueryable<int> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<int>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static int? Sum(this IQueryable<int?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<int?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static long Sum(this IQueryable<long> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<long>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static long? Sum(this IQueryable<long?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<long?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static float Sum(this IQueryable<float> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<float>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static float? Sum(this IQueryable<float?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<float?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double Sum(this IQueryable<double> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static double? Sum(this IQueryable<double?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<double?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static decimal Sum(this IQueryable<decimal> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<decimal>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static decimal? Sum(this IQueryable<decimal?> source)
        {
            LinqCheck.Source(source);

            return source.Provider.Execute<decimal?>(
                       StaticCall(
                           (MethodInfo)MethodBase.GetCurrentMethod(),
                           source.Expression));
        }

        public static IQueryable<TSource> Take<TSource>(this IQueryable<TSource> source, int count)
        {
            LinqCheck.Source(source);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Constant(count)));
        }

        public static IQueryable<TSource> TakeWhile<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TSource> TakeWhile<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Constant(comparer, typeof(IComparer<TKey>))));
        }

        public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector)));
        }

        public static IOrderedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, IComparer<TKey> comparer)
        {
            LinqCheck.SourceAndKeySelector(source, keySelector);

            return (IOrderedQueryable<TSource>)source.Provider.CreateQuery(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource), typeof(TKey)),
                           source.Expression,
                           Expression.Quote(keySelector),
                           Expression.Constant(comparer, typeof(IComparer<TKey>))));
        }

        public static IQueryable<TSource> Union<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>))));
        }

        public static IQueryable<TSource> Union<TSource>(this IQueryable<TSource> source1, IEnumerable<TSource> source2, IEqualityComparer<TSource> comparer)
        {
            LinqCheck.Source1AndSource2(source1, source2);

            return source1.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source1.Expression,
                           Expression.Constant(source2, typeof(IEnumerable<TSource>)),
                           Expression.Constant(comparer, typeof(IEqualityComparer<TSource>))));
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
        {
            LinqCheck.SourceAndPredicate(source, predicate);

            return source.Provider.CreateQuery<TSource>(
                       StaticCall(
                           MakeGeneric(MethodBase.GetCurrentMethod(), typeof(TSource)),
                           source.Expression,
                           Expression.Quote(predicate)));
        }

        private static TRet Execute<TRet, TSource>(this IQueryable<TSource> source, MethodBase current)
        {
            return source.Provider.Execute<TRet>(
                       StaticCall(
                           MakeGeneric(current, typeof(TSource)),
                           source.Expression));
        }

        private static MethodInfo MakeGeneric(MethodBase method, params Type[] parameters)
        {
            return ((MethodInfo)method).MakeGenericMethod(parameters);
        }

        private static Expression StaticCall(MethodInfo method, params Expression[] expressions)
        {
            return Expression.Call(null, method, expressions);
        }
    }
}

#endif