// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable InconsistentNaming

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Theraot;

namespace TestRunner
{
    public static class LinqAvailabilityTests
    {
        public static void EnumerableExtensionMethodAvailability()
        {
            IEnumerable<TSource> enumerable;
            No.Op<Func<Func<TSource, TSource, TSource>, TSource>>(enumerable.Aggregate);
            No.Op<Func<TAccumulate, Func<TAccumulate, TSource, TAccumulate>, TAccumulate>>(enumerable.Aggregate);
            No.Op<Func<TAccumulate, Func<TAccumulate, TSource, TAccumulate>, Func<TAccumulate, TResult>, TResult>>(enumerable.Aggregate);
            No.Op<Func<Func<TSource, bool>, bool>>(enumerable.All);
            No.Op<Func<bool>>(enumerable.Any);
            No.Op<Func<Func<TSource, bool>, bool>>(enumerable.Any);
            No.Op<Func<TSource, IEnumerable<TSource>>>(enumerable.Append);
            No.Op<Func<IEnumerable<TSource>>>(enumerable.AsEnumerable);
            No.Op<Func<IEnumerable<TSource>, IEnumerable<TSource>>>(enumerable.Concat);
            No.Op<Func<TSource, bool>>(enumerable.Contains);
            No.Op<Func<TSource, IEqualityComparer<TSource>, bool>>(enumerable.Contains);
            No.Op<Func<int>>(enumerable.Count);
            No.Op<Func<Func<TSource, bool>, int>>(enumerable.Count);
            No.Op<Func<IEnumerable<TSource>>>(enumerable.DefaultIfEmpty);
            No.Op<Func<TSource, IEnumerable<TSource>>>(enumerable.DefaultIfEmpty);
            No.Op<Func<IEnumerable<TSource>>>(enumerable.Distinct);
            No.Op<Func<IEqualityComparer<TSource>, IEnumerable<TSource>>>(enumerable.Distinct);
            No.Op<Func<int, TSource>>(enumerable.ElementAt);
            No.Op<Func<int, TSource>>(enumerable.ElementAtOrDefault);
            No.Op<Func<IEnumerable<TSource>, IEnumerable<TSource>>>(enumerable.Except);
            No.Op<Func<IEnumerable<TSource>, IEqualityComparer<TSource>, IEnumerable<TSource>>>(enumerable.Except);
            No.Op<Func<TSource>>(enumerable.First);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.First);
            No.Op<Func<TSource>>(enumerable.FirstOrDefault);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.FirstOrDefault);
            No.Op<Func<Func<TSource, TKey>, IEnumerable<IGrouping<TKey, TSource>>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, IEqualityComparer<TKey>, IEnumerable<IGrouping<TKey, TSource>>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TKey, IEnumerable<TSource>, TResult>, IEnumerable<TResult>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, IEnumerable<IGrouping<TKey, TElement>>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TKey, IEnumerable<TSource>, TResult>, IEqualityComparer<TKey>, IEnumerable<TResult>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, IEnumerable<IGrouping<TKey, TElement>>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, Func<TKey, IEnumerable<TElement>, TResult>, IEnumerable<TResult>>>(enumerable.GroupBy);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, Func<TKey, IEnumerable<TElement>, TResult>, IEqualityComparer<TKey>, IEnumerable<TResult>>>(enumerable.GroupBy);
            No.Op<Func<IEnumerable<TInner>, Func<TSource, TKey>, Func<TInner, TKey>, Func<TSource, IEnumerable<TInner>, TResult>, IEnumerable<TResult>>>(enumerable.GroupJoin);
            No.Op<Func<IEnumerable<TInner>, Func<TSource, TKey>, Func<TInner, TKey>, Func<TSource, IEnumerable<TInner>, TResult>, IEqualityComparer<TKey>, IEnumerable<TResult>>>(enumerable.GroupJoin);
            No.Op<Func<IEnumerable<TSource>, IEnumerable<TSource>>>(enumerable.Intersect);
            No.Op<Func<IEnumerable<TSource>, IEqualityComparer<TSource>, IEnumerable<TSource>>>(enumerable.Intersect);
            No.Op<Func<IEnumerable<TInner>, Func<TSource, TKey>, Func<TInner, TKey>, Func<TSource, TInner, TResult>, IEnumerable<TResult>>>(enumerable.Join);
            No.Op<Func<IEnumerable<TInner>, Func<TSource, TKey>, Func<TInner, TKey>, Func<TSource, TInner, TResult>, IEqualityComparer<TKey>, IEnumerable<TResult>>>(enumerable.Join);
            No.Op<Func<TSource>>(enumerable.Last);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.Last);
            No.Op<Func<TSource>>(enumerable.LastOrDefault);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.LastOrDefault);
            No.Op<Func<long>>(enumerable.LongCount);
            No.Op<Func<Func<TSource, bool>, long>>(enumerable.LongCount);
            No.Op<Func<Func<TSource, TKey>, IOrderedEnumerable<TSource>>>(enumerable.OrderBy);
            No.Op<Func<Func<TSource, TKey>, IComparer<TKey>, IOrderedEnumerable<TSource>>>(enumerable.OrderBy);
            No.Op<Func<Func<TSource, TKey>, IOrderedEnumerable<TSource>>>(enumerable.OrderByDescending);
            No.Op<Func<Func<TSource, TKey>, IComparer<TKey>, IOrderedEnumerable<TSource>>>(enumerable.OrderByDescending);
            No.Op<Func<TSource, IEnumerable<TSource>>>(enumerable.Prepend);
            No.Op<Func<IEnumerable<TSource>>>(enumerable.Reverse);
            No.Op<Func<Func<TSource, TResult>, IEnumerable<TResult>>>(enumerable.Select);
            No.Op<Func<Func<TSource, int, TResult>, IEnumerable<TResult>>>(enumerable.Select);
            No.Op<Func<Func<TSource, IEnumerable<TResult>>, IEnumerable<TResult>>>(enumerable.SelectMany);
            No.Op<Func<Func<TSource, int, IEnumerable<TResult>>, IEnumerable<TResult>>>(enumerable.SelectMany);
            No.Op<Func<Func<TSource, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>, IEnumerable<TResult>>>(enumerable.SelectMany);
            No.Op<Func<Func<TSource, int, IEnumerable<TCollection>>, Func<TSource, TCollection, TResult>, IEnumerable<TResult>>>(enumerable.SelectMany);
            No.Op<Func<IEnumerable<TSource>, bool>>(enumerable.SequenceEqual);
            No.Op<Func<IEnumerable<TSource>, IEqualityComparer<TSource>, bool>>(enumerable.SequenceEqual);
            No.Op<Func<TSource>>(enumerable.Single);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.Single);
            No.Op<Func<TSource>>(enumerable.SingleOrDefault);
            No.Op<Func<Func<TSource, bool>, TSource>>(enumerable.SingleOrDefault);
            No.Op<Func<int, IEnumerable<TSource>>>(enumerable.Skip);
            No.Op<Func<int, IEnumerable<TSource>>>(enumerable.SkipLast);
            No.Op<Func<Func<TSource, bool>, IEnumerable<TSource>>>(enumerable.SkipWhile);
            No.Op<Func<Func<TSource, int, bool>, IEnumerable<TSource>>>(enumerable.SkipWhile);
            No.Op<Func<int, IEnumerable<TSource>>>(enumerable.Take);
            No.Op<Func<int, IEnumerable<TSource>>>(enumerable.TakeLast);
            No.Op<Func<Func<TSource, bool>, IEnumerable<TSource>>>(enumerable.TakeWhile);
            No.Op<Func<Func<TSource, int, bool>, IEnumerable<TSource>>>(enumerable.TakeWhile);
            No.Op<Func<TSource[]>>(enumerable.ToArray);
            No.Op<Func<Func<TSource, TKey>, Dictionary<TKey, TSource>>>(enumerable.ToDictionary);
            No.Op<Func<Func<TSource, TKey>, IEqualityComparer<TKey>, Dictionary<TKey, TSource>>>(enumerable.ToDictionary);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, Dictionary<TKey, TElement>>>(enumerable.ToDictionary);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, Dictionary<TKey, TElement>>>(enumerable.ToDictionary);
            No.Op<Func<HashSet<TSource>>>(enumerable.ToHashSet);
            No.Op<Func<IEqualityComparer<TSource>, HashSet<TSource>>>(enumerable.ToHashSet);
            No.Op<Func<List<TSource>>>(enumerable.ToList);
            No.Op<Func<Func<TSource, TKey>, ILookup<TKey, TSource>>>(enumerable.ToLookup);
            No.Op<Func<Func<TSource, TKey>, IEqualityComparer<TKey>, ILookup<TKey, TSource>>>(enumerable.ToLookup);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, ILookup<TKey, TElement>>>(enumerable.ToLookup);
            No.Op<Func<Func<TSource, TKey>, Func<TSource, TElement>, IEqualityComparer<TKey>, ILookup<TKey, TElement>>>(enumerable.ToLookup);
            No.Op<Func<IEnumerable<TSource>, IEnumerable<TSource>>>(enumerable.Union);
            No.Op<Func<IEnumerable<TSource>, IEqualityComparer<TSource>, IEnumerable<TSource>>>(enumerable.Union);
            No.Op<Func<Func<TSource, bool>, IEnumerable<TSource>>>(enumerable.Where);
            No.Op<Func<Func<TSource, int, bool>, IEnumerable<TSource>>>(enumerable.Where);
            No.Op<Func<IEnumerable<TSecond>, Func<TSource, TSecond, TResult>, IEnumerable<TResult>>>(enumerable.Zip);
        }

        public static void EnumerableMethodAvailability()
        {
            No.Op<Func<IEnumerable, IEnumerable<TResult>>>(Enumerable.Cast<TResult>);
            No.Op<Func<IEnumerable<TResult>>>(Enumerable.Empty<TResult>);
            No.Op<Func<int, int, IEnumerable<int>>>(Enumerable.Range);
            No.Op<Func<TResult, int, IEnumerable<TResult>>>(Enumerable.Repeat);
        }

        public static void NumericEnumerableMethodAvailability()
        {
            IEnumerable<TSource> enumerable;
            IEnumerable<float> floatEnumerable;
            IEnumerable<long> longEnumerable;
            IEnumerable<int> intEnumerable;
            IEnumerable<double> doubleEnumerable;
            IEnumerable<decimal> decimalEnumerable;
            IEnumerable<float?> nullableFloatEnumerable;
            IEnumerable<long?> nullableLongEnumerable;
            IEnumerable<int?> nullableIntEnumerable;
            IEnumerable<double?> nullableDoubleEnumerable;
            IEnumerable<decimal?> nullableDecimalEnumerable;

            No.Op<Func<float>>(floatEnumerable.Average);
            No.Op<Func<double>>(longEnumerable.Average);
            No.Op<Func<double>>(intEnumerable.Average);
            No.Op<Func<double>>(doubleEnumerable.Average);
            No.Op<Func<decimal>>(decimalEnumerable.Average);
            No.Op<Func<float?>>(nullableFloatEnumerable.Average);
            No.Op<Func<double?>>(nullableLongEnumerable.Average);
            No.Op<Func<double?>>(nullableIntEnumerable.Average);
            No.Op<Func<double?>>(nullableDoubleEnumerable.Average);
            No.Op<Func<decimal?>>(nullableDecimalEnumerable.Average);

            No.Op<Func<Func<TSource, float>, float>>(enumerable.Average);
            No.Op<Func<Func<TSource, double>, double>>(enumerable.Average);
            No.Op<Func<Func<TSource, decimal>, decimal>>(enumerable.Average);
            No.Op<Func<Func<TSource, float?>, float?>>(enumerable.Average);
            No.Op<Func<Func<TSource, double?>, double?>>(enumerable.Average);
            No.Op<Func<Func<TSource, decimal?>, decimal?>>(enumerable.Average);

            No.Op<Func<float>>(floatEnumerable.Max);
            No.Op<Func<long>>(longEnumerable.Max);
            No.Op<Func<int>>(intEnumerable.Max);
            No.Op<Func<double>>(doubleEnumerable.Max);
            No.Op<Func<decimal>>(decimalEnumerable.Max);
            No.Op<Func<float?>>(nullableFloatEnumerable.Max);
            No.Op<Func<long?>>(nullableLongEnumerable.Max);
            No.Op<Func<int?>>(nullableIntEnumerable.Max);
            No.Op<Func<double?>>(nullableDoubleEnumerable.Max);
            No.Op<Func<decimal?>>(nullableDecimalEnumerable.Max);

            No.Op<Func<Func<TSource, float>, float>>(enumerable.Max);
            No.Op<Func<Func<TSource, double>, double>>(enumerable.Max);
            No.Op<Func<Func<TSource, decimal>, decimal>>(enumerable.Max);
            No.Op<Func<Func<TSource, float?>, float?>>(enumerable.Max);
            No.Op<Func<Func<TSource, double?>, double?>>(enumerable.Max);
            No.Op<Func<Func<TSource, decimal?>, decimal?>>(enumerable.Max);

            No.Op<Func<float>>(floatEnumerable.Min);
            No.Op<Func<long>>(longEnumerable.Min);
            No.Op<Func<int>>(intEnumerable.Min);
            No.Op<Func<double>>(doubleEnumerable.Min);
            No.Op<Func<decimal>>(decimalEnumerable.Min);
            No.Op<Func<float?>>(nullableFloatEnumerable.Min);
            No.Op<Func<long?>>(nullableLongEnumerable.Min);
            No.Op<Func<int?>>(nullableIntEnumerable.Min);
            No.Op<Func<double?>>(nullableDoubleEnumerable.Min);
            No.Op<Func<decimal?>>(nullableDecimalEnumerable.Min);

            No.Op<Func<Func<TSource, float>, float>>(enumerable.Min);
            No.Op<Func<Func<TSource, double>, double>>(enumerable.Min);
            No.Op<Func<Func<TSource, decimal>, decimal>>(enumerable.Min);
            No.Op<Func<Func<TSource, float?>, float?>>(enumerable.Min);
            No.Op<Func<Func<TSource, double?>, double?>>(enumerable.Min);
            No.Op<Func<Func<TSource, decimal?>, decimal?>>(enumerable.Min);

            No.Op<Func<float>>(floatEnumerable.Sum);
            No.Op<Func<long>>(longEnumerable.Sum);
            No.Op<Func<int>>(intEnumerable.Sum);
            No.Op<Func<double>>(doubleEnumerable.Sum);
            No.Op<Func<decimal>>(decimalEnumerable.Sum);
            No.Op<Func<float?>>(nullableFloatEnumerable.Sum);
            No.Op<Func<long?>>(nullableLongEnumerable.Sum);
            No.Op<Func<int?>>(nullableIntEnumerable.Sum);
            No.Op<Func<double?>>(nullableDoubleEnumerable.Sum);
            No.Op<Func<decimal?>>(nullableDecimalEnumerable.Sum);

            No.Op<Func<Func<TSource, float>, float>>(enumerable.Sum);
            No.Op<Func<Func<TSource, double>, double>>(enumerable.Sum);
            No.Op<Func<Func<TSource, decimal>, decimal>>(enumerable.Sum);
            No.Op<Func<Func<TSource, float?>, float?>>(enumerable.Sum);
            No.Op<Func<Func<TSource, double?>, double?>>(enumerable.Sum);
            No.Op<Func<Func<TSource, decimal?>, decimal?>>(enumerable.Sum);
        }

        public static void OfTypeAvailability()
        {
            IEnumerable enumerable;
            No.Op<Func<IEnumerable<TResult>>>(enumerable.OfType<TResult>);
        }

        public static void OrderedEnumerableMethodAvailability()
        {
            IOrderedEnumerable<TSource> orderedEnumerable;
            No.Op<Func<Func<TSource, TKey>, IOrderedEnumerable<TSource>>>(orderedEnumerable.ThenBy);
            No.Op<Func<Func<TSource, TKey>, IComparer<TKey>, IOrderedEnumerable<TSource>>>(orderedEnumerable.ThenBy);
            No.Op<Func<Func<TSource, TKey>, IOrderedEnumerable<TSource>>>(orderedEnumerable.ThenByDescending);
            No.Op<Func<Func<TSource, TKey>, IComparer<TKey>, IOrderedEnumerable<TSource>>>(orderedEnumerable.ThenByDescending);
        }

        private class TAccumulate
        {
            // Empty
        }

        private class TCollection
        {
            // Empty
        }

        private class TElement
        {
            // Empty
        }

        private class TInner
        {
            // Empty
        }

        private class TKey
        {
            // Empty
        }

        private class TResult
        {
            // Empty
        }

        private class TSecond
        {
            // Empty
        }

        private class TSource
        {
            // Empty
        }
    }
}