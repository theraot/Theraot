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
            No.Op<Func<TAccumulate, Func<TAccumulate, TSource, TAccumulate>, Func<TAccumulate, TResult>, TResult>>(enumerable.Aggregate);
            No.Op<Func<TAccumulate, Func<TAccumulate, TSource, TAccumulate>, TAccumulate>>(enumerable.Aggregate);
            No.Op<Func<Func<TSource, TSource, TSource>, TSource>>(enumerable.Aggregate);
            No.Op<Func<Func<TSource, bool>, bool>>(enumerable.All);
            No.Op<Func<bool>>(enumerable.Any);
            No.Op<Func<Func<TSource, bool>, bool>>(enumerable.Any);
            No.Op<Func<TSource, IEnumerable<TSource>>>(enumerable.Append);
            No.Op<Func<IEnumerable<TSource>>>(enumerable.AsEnumerable);
            No.Op<Func<IEnumerable<TSource>, IEnumerable<TSource>>>(enumerable.Concat);
            No.Op<Func<TSource, bool>>(enumerable.Contains);
            No.Op<Func<TSource, IEqualityComparer<TSource>, bool>>(enumerable.Contains);
            No.Op<Func<int>>(enumerable.Count);
            No.Op<Func<IEnumerable<TSecond>, Func<TSource, TSecond, TResult>, IEnumerable<TResult>>>(enumerable.Zip);
        }

        public static void EnumerableMethodAvailability()
        {
            No.Op<Func<IEnumerable, IEnumerable<TResult>>>(Enumerable.Cast<TResult>);
            No.Op<Func<int, int, IEnumerable<int>>>(Enumerable.Range);
            No.Op<Func<TResult, int, IEnumerable<TResult>>>(Enumerable.Repeat);
        }

        public static void NumericEnumerableMethodAvailability()
        {
            IEnumerable<TSource> enumerable;
            IEnumerable<float> floatEnumerable;
            IEnumerable<long> longEnumerable;
            IEnumerable<long> intEnumerable;
            IEnumerable<double> doubleEnumerable;
            IEnumerable<decimal> decimalEnumerable;
            IEnumerable<float?> nullableFloatEnumerable;
            IEnumerable<long?> nullableLongEnumerable;
            IEnumerable<long?> nullableIntEnumerable;
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
        }

        private class TAccumulate { }

        private class TResult { }

        private class TSecond { }

        private class TSource { }
    }
}