#if LESSTHAN_NET35

#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections.Generic;
using Theraot.Reflection;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static double Average(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            var count = 0L;
            foreach (var item in source)
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            return sum / (double)count;
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return Average(Select(source, selector));
        }

        public static double? Average(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            var count = 0L;
            foreach (var item in Where(source, n => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            return sum / (double)count;
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return Average(Select(source, selector));
        }

        public static double Average(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            var count = 0L;
            foreach (var item in source)
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            return sum / (double)count;
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return Average(Select(source, selector));
        }

        public static double? Average(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            var count = 0L;
            foreach (var item in Where(source, n => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            return sum / (double)count;
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            return Average(Select(source, selector));
        }

        public static float Average(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0f;
            var count = 0L;
            foreach (var item in source)
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            return sum / count;
        }

        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return Average(Select(source, selector));
        }

        public static float? Average(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0f;
            var count = 0L;
            foreach (var nullable in Where(source, n => n.HasValue))
            {
                sum += nullable.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            return sum / count;
        }

        public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return Average(Select(source, selector));
        }

        public static double Average(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0d;
            var count = 0L;
            foreach (var item in source)
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            return sum / count;
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return Average(Select(source, selector));
        }

        public static double? Average(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0d;
            var count = 0L;
            foreach (var item in Where(source, n => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            return sum / count;
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return Average(Select(source, selector));
        }

        public static decimal Average(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0m;
            var count = 0L;
            foreach (var item in source)
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            return sum / count;
        }

        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return Average(Select(source, selector));
        }

        public static decimal? Average(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0m;
            var count = 0L;
            foreach (var item in Where(source, n => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0)
            {
                return null;
            }
            return sum / count;
        }

        public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Average(Select(source, selector));
        }

        public static int Max(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = int.MinValue;
            foreach (var element in source)
            {
                if (element > max)
                {
                    max = element;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static long Max(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                if (element > max)
                {
                    max = element;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static double Max(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException();
                }
                // Skin NaN
                var max = enumerator.Current;
                while (double.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    max = enumerator.Current;
                }
                while (enumerator.MoveNext())
                {
                    var element = enumerator.Current;
                    if (element > max)
                    {
                        max = element;
                    }
                }
                return max;
            }
        }

        public static float Max(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException();
                }
                // Skin NaN
                var max = enumerator.Current;
                while (float.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    max = enumerator.Current;
                }
                while (enumerator.MoveNext())
                {
                    var element = enumerator.Current;
                    if (element > max)
                    {
                        max = element;
                    }
                }
                return max;
            }
        }

        public static decimal Max(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                if (element > max)
                {
                    max = element;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static int? Max(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = int.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static long? Max(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static double? Max(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
                // Skip null
                var found = enumerator.Current;
                while (!found.HasValue)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                    found = enumerator.Current;
                }
                // Skip NaN
                var max = found.Value;
                while (double.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    // Skip null
                    found = enumerator.Current;
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = enumerator.Current;
                    }
                    max = found.Value;
                }
                while (enumerator.MoveNext())
                {
                    // Skip null
                    found = enumerator.Current;
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = enumerator.Current;
                    }
                    var value = found.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
                // Skip null
                var found = enumerator.Current;
                while (!found.HasValue)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                    found = enumerator.Current;
                }
                // Skip NaN
                var max = found.Value;
                while (float.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    // Skip null
                    found = enumerator.Current;
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = enumerator.Current;
                    }
                    max = found.Value;
                }
                while (enumerator.MoveNext())
                {
                    // Skip null
                    found = enumerator.Current;
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = enumerator.Current;
                    }
                    var value = found.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static decimal? Max(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var comparer = Comparer<TSource>.Default;
            var max = default(TSource);
            if (typeof(TSource).CanBeNull())
            {
                foreach (var element in source)
                {
                    if (element != null && (max == null || comparer.Compare(element, max) > 0))
                    {
                        max = element;
                    }
                }
                return max;
            }
            var found = false;
            foreach (var element in source)
            {
                if (found)
                {
                    if (comparer.Compare(element, max) > 0)
                    {
                        max = element;
                    }
                }
                else
                {
                    max = element;
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var max = int.MinValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value > max)
                {
                    max = value;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value > max)
                {
                    max = value;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException();
                }
                // Skin NaN
                var max = selector(enumerator.Current);
                while (double.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    max = selector(enumerator.Current);
                }
                while (enumerator.MoveNext())
                {
                    var value = selector(enumerator.Current);
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new InvalidOperationException();
                }
                // Skin NaN
                var max = selector(enumerator.Current);
                while (float.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    max = selector(enumerator.Current);
                }
                while (enumerator.MoveNext())
                {
                    var value = selector(enumerator.Current);
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value > max)
                {
                    max = value;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            int? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item > max)
                    {
                        max = item;
                    }
                }
                else
                {
                    max = item;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            long? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item > max)
                    {
                        max = item;
                    }
                }
                else
                {
                    max = item;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
                // Skip null
                var found = selector(enumerator.Current);
                while (!found.HasValue)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                    found = selector(enumerator.Current);
                }
                // Skip NaN
                var max = found.Value;
                while (double.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    // Skip null
                    found = selector(enumerator.Current);
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = selector(enumerator.Current);
                    }
                    max = found.Value;
                }
                while (enumerator.MoveNext())
                {
                    // Skip null
                    found = selector(enumerator.Current);
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = selector(enumerator.Current);
                    }
                    var value = found.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            using (var enumerator = source.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
                // Skip null
                var found = selector(enumerator.Current);
                while (!found.HasValue)
                {
                    if (!enumerator.MoveNext())
                    {
                        return null;
                    }
                    found = selector(enumerator.Current);
                }
                // Skip NaN
                var max = found.Value;
                while (float.IsNaN(max))
                {
                    if (!enumerator.MoveNext())
                    {
                        return max;
                    }
                    // Skip null
                    found = selector(enumerator.Current);
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = selector(enumerator.Current);
                    }
                    max = found.Value;
                }
                while (enumerator.MoveNext())
                {
                    // Skip null
                    found = selector(enumerator.Current);
                    while (!found.HasValue)
                    {
                        if (!enumerator.MoveNext())
                        {
                            return max;
                        }
                        found = selector(enumerator.Current);
                    }
                    var value = found.Value;
                    if (value > max)
                    {
                        max = value;
                    }
                }
                return max;
            }
        }

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            decimal? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item > max)
                    {
                        max = item;
                    }
                }
                else
                {
                    max = item;
                }
                found = true;
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            TResult ClosureSelector(TSource item, int _) => selector(item);
            return Max
            (
                SelectExtracted
                (
                    source,
                    ClosureSelector
                )
            );
        }

        public static int Min(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = int.MaxValue;
            foreach (var element in source)
            {
                if (element < min)
                {
                    min = element;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static long Min(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = long.MaxValue;
            foreach (var element in source)
            {
                if (element < min)
                {
                    min = element;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static double Min(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = double.MaxValue;
            foreach (var element in source)
            {
                if (double.IsNaN(element))
                {
                    return element;
                }
                if (element < min)
                {
                    min = element;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static float Min(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = float.MaxValue;
            foreach (var element in source)
            {
                if (float.IsNaN(element))
                {
                    return element;
                }
                if (element < min)
                {
                    min = element;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static decimal Min(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = decimal.MaxValue;
            foreach (var element in source)
            {
                if (element < min)
                {
                    min = element;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static int? Min(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = int.MaxValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value < min)
                    {
                        min = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static long? Min(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = long.MaxValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value < min)
                    {
                        min = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static double? Min(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = double.MaxValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (double.IsNaN(value))
                    {
                        return value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static float? Min(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = float.MaxValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (float.IsNaN(value))
                    {
                        return value;
                    }
                    if (value < min)
                    {
                        min = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static decimal? Min(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var min = decimal.MaxValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    var value = element.Value;
                    if (value < min)
                    {
                        min = value;
                    }
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var comparer = Comparer<TSource>.Default;
            var min = default(TSource);
            if (default(TSource) == null)
            {
                foreach (var element in source)
                {
                    if (element == null)
                    {
                        continue;
                    }
                    if (min == null || comparer.Compare(element, min) < 0)
                    {
                        min = element;
                    }
                }
                return min;
            }
            var found = false;
            foreach (var element in source)
            {
                if (found)
                {
                    if (comparer.Compare(element, min) < 0)
                    {
                        min = element;
                    }
                }
                else
                {
                    min = element;
                    found = true;
                }
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var min = int.MaxValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value < min)
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var min = long.MaxValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value < min)
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var min = double.MaxValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (double.IsNaN(value))
                {
                    return value;
                }
                if (value < min)
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var min = float.MaxValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (float.IsNaN(value))
                {
                    return value;
                }
                if (value < min)
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            var min = decimal.MaxValue;
            foreach (var element in source)
            {
                var value = selector(element);
                if (value < min)
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            throw new InvalidOperationException();
        }

        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            int? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item < min)
                    {
                        min = item;
                    }
                }
                else
                {
                    min = item;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            long? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item < min)
                    {
                        min = item;
                    }
                }
                else
                {
                    min = item;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            float? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (!item.HasValue)
                {
                    continue;
                }
                var value = item.Value;
                if (float.IsNaN(value))
                {
                    return value;
                }
                if (min.HasValue)
                {
                    if (value < min)
                    {
                        min = value;
                    }
                }
                else
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            double? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (!item.HasValue)
                {
                    continue;
                }
                var value = item.Value;
                if (double.IsNaN(value))
                {
                    return value;
                }
                if (min.HasValue)
                {
                    if (value < min)
                    {
                        min = value;
                    }
                }
                else
                {
                    min = value;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            var found = false;
            decimal? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item < min)
                    {
                        min = item;
                    }
                }
                else
                {
                    min = item;
                }
                found = true;
            }
            if (found)
            {
                return min;
            }
            return null;
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }
            TResult ClosureSelector(TSource item, int _) => selector(item);
            return Min
            (
                SelectExtracted
                (
                    source,
                    ClosureSelector
                )
            );
        }

        public static IEnumerable<int> Range(int start, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if ((long)start + count - 1L > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(start));
            }
            return RangeExtracted();

            IEnumerable<int> RangeExtracted()
            {
                for (var index = start; count > 0; count--, index++)
                {
                    yield return index;
                }
            }
        }

        public static int Sum(this IEnumerable<int> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var count = 0;
            foreach (var item in source)
            {
                count += item;
            }
            return count;
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return Sum(Select(source, selector));
        }

        public static int? Sum(this IEnumerable<int?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0;
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static int? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return Sum(Select(source, selector));
        }

        public static long Sum(this IEnumerable<long> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return Sum(Select(source, selector));
        }

        public static long? Sum(this IEnumerable<long?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0L;
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static long? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            return Sum(Select(source, selector));
        }

        public static float Sum(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0f;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return Sum(Select(source, selector));
        }

        public static float? Sum(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0f;
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return Sum(Select(source, selector));
        }

        public static double Sum(this IEnumerable<double> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0d;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return Sum(Select(source, selector));
        }

        public static double? Sum(this IEnumerable<double?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            double sum = 0;
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return Sum(Select(source, selector));
        }

        public static decimal Sum(this IEnumerable<decimal> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0m;
            foreach (var item in source)
            {
                sum += item;
            }
            return sum;
        }

        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return Sum(Select(source, selector));
        }

        public static decimal? Sum(this IEnumerable<decimal?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var sum = 0.0m;
            foreach (var item in source)
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Sum(Select(source, selector));
        }
    }
}

#endif