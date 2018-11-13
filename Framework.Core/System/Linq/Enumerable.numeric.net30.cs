#if NET20 || NET30

using System.Collections.Generic;
using Theraot.Core;

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
                max = Math.Max(element, max);
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
                max = Math.Max(element, max);
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
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
        }

        public static float Max(this IEnumerable<float> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
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
                max = Math.Max(element, max);
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
                    max = Math.Max(element.Value, max);
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
                    max = Math.Max(element.Value, max);
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
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    max = Math.Max(element.Value, max);
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            return null;
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    max = Math.Max(element.Value, max);
                    found = true;
                }
            }
            if (found)
            {
                return max;
            }
            return null;
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
                    max = Math.Max(element.Value, max);
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
                    if (!ReferenceEquals(element, null))
                    {
                        if (ReferenceEquals(max, null) || comparer.Compare(element, max) > 0)
                        {
                            max = element;
                        }
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
                max = Math.Max(selector(element), max);
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
                max = Math.Max(selector(element), max);
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
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                max = Math.Max(selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
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
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                max = Math.Max(selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            throw new InvalidOperationException();
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
                max = Math.Max(selector(element), max);
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
            var found = false;
            double? max = null;
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
            var found = false;
            float? max = null;
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
            TResult closureSelector(TSource item, int i) => selector(item);
            return Max
            (
                SelectExtracted
                (
                    source,
                    closureSelector
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
                min = Math.Min(element, min);
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
                min = Math.Min(element, min);
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
                min = Math.Min(element, min);
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
                min = Math.Min(element, min);
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
                min = Math.Min(element, min);
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
                    min = Math.Min(element.Value, min);
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
                    min = Math.Min(element.Value, min);
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
                    min = Math.Min(element.Value, min);
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
                    min = Math.Min(element.Value, min);
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
                    min = Math.Min(element.Value, min);
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
            if (ReferenceEquals(default(TSource), null))
            {
                foreach (var element in source)
                {
                    if (ReferenceEquals(element, null))
                    {
                        continue;
                    }
                    if (ReferenceEquals(min, null) || comparer.Compare(element, min) < 0)
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
                min = Math.Min(selector(element), min);
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
                min = Math.Min(selector(element), min);
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
                min = Math.Min(selector(element), min);
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
                min = Math.Min(selector(element), min);
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
                min = Math.Min(selector(element), min);
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
            TResult closureSelector(TSource item, int i) => selector(item);
            return Min
            (
                SelectExtracted
                (
                    source,
                    closureSelector
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
                throw new ArgumentOutOfRangeException();
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