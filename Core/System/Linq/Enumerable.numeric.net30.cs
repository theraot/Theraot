#if NET20 || NET30

using System.Collections.Generic;
using Theraot.Core;

namespace System.Linq
{
    public static partial class Enumerable
    {
        public static double Average(this IEnumerable<int> source)
        {
            long sum = 0L;
            long count = 0L;
            foreach (int item in Check.NotNullArgument(source, "source"))
            {
                sum += (long)item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return (double)sum / (double)count;
            }
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return Average(Select<TSource, int>(source, selector));
        }

        public static double? Average(this IEnumerable<int?> source)
        {
            long sum = 0L;
            long count = 0L;
            foreach (int? item in Where<int?>(Check.NotNullArgument(source, "source"), (int? n) => n.HasValue))
            {
                sum += (long)item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            else
            {
                return (double)sum / (double)count;
            }
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            return Average(Select<TSource, int?>(source, selector));
        }

        public static double Average(this IEnumerable<long> source)
        {
            long sum = 0L;
            long count = 0L;
            foreach (long item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return (double)sum / (double)count;
            }
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return Average(Select<TSource, long>(source, selector));
        }

        public static double? Average(this IEnumerable<long?> source)
        {
            long sum = 0L;
            long count = 0L;
            foreach (long? item in Where<long?>(Check.NotNullArgument(source, "source"), (long? n) => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            else
            {
                return (double)sum / (double)count;
            }
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            return Average(Select<TSource, long?>(source, selector));
        }

        public static float Average(this IEnumerable<float> source)
        {
            float sum = 0.0f;
            long count = 0L;
            foreach (float item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return sum / (float)count;
            }
        }

        public static float Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return Average(Select<TSource, float>(source, selector));
        }

        public static float? Average(this IEnumerable<float?> source)
        {
            float sum = 0.0f;
            long count = 0L;
            foreach (float? nullable in Where<float?>(Check.NotNullArgument(source, "source"), (float? n) => n.HasValue))
            {
                sum += nullable.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            else
            {
                return sum / (float)count;
            }
        }

        public static float? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return Average(Select<TSource, float?>(source, selector));
        }

        public static double Average(this IEnumerable<double> source)
        {
            double sum = 0.0d;
            long count = 0L;
            foreach (double item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return sum / (double)count;
            }
        }

        public static double Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return Average(Select<TSource, double>(source, selector));
        }

        public static double? Average(this IEnumerable<double?> source)
        {
            double sum = 0.0d;
            long count = 0L;
            foreach (double? item in Where<double?>(Check.NotNullArgument(source, "source"), (double? n) => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == 0L)
            {
                return null;
            }
            else
            {
                return sum / (double)count;
            }
        }

        public static double? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return Average(Select<TSource, double?>(source, selector));
        }

        public static decimal Average(this IEnumerable<decimal> source)
        {
            decimal sum = 0.0m;
            long count = 0L;
            foreach (decimal item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
                count++;
            }
            if (count == 0L)
            {
                throw new InvalidOperationException();
            }
            else
            {
                return sum / count;
            }
        }

        public static decimal Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return Average(Select<TSource, decimal>(source, selector));
        }

        public static decimal? Average(this IEnumerable<decimal?> source)
        {
            decimal sum = 0.0m;
            long count = 0L;
            foreach (decimal? item in Where<decimal?>(Check.NotNullArgument(source, "source"), (decimal? n) => n.HasValue))
            {
                sum += item.Value;
                count++;
            }
            if (count == (long)0)
            {
                return null;
            }
            else
            {
                return sum / count;
            }
        }

        public static decimal? Average<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Average(Select<TSource, decimal?>(source, selector));
        }

        public static int Max(this IEnumerable<int> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = int.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static long Max(this IEnumerable<long> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static double Max(this IEnumerable<double> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static float Max(this IEnumerable<float> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static decimal Max(this IEnumerable<decimal> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(element, max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static int? Max(this IEnumerable<int?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = int.MinValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static long? Max(this IEnumerable<long?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static double? Max(this IEnumerable<double?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static float? Max(this IEnumerable<float?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static decimal? Max(this IEnumerable<decimal?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var comparer = Comparer<TSource>.Default;
            TSource max = default(TSource);
            if (typeof(TSource).CanBeNull())
            {
                foreach (var element in _source)
                {
                    if (element == null)
                    {
                        continue;
                    }
                    else
                    {
                        if (max == null || comparer.Compare(element, max) > 0)
                        {
                            max = element;
                        }
                    }
                }
                return max;
            }
            else
            {
                bool found = false;
                foreach (var element in _source)
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
                        continue;
                    }
                }
                if (found)
                {
                    return max;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = int.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(_selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(_selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(_selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(_selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
            {
                max = Math.Max(_selector(element), max);
                found = true;
            }
            if (found)
            {
                return max;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            int? max = null;
            foreach (var element in _source)
            {
                int? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            long? max = null;
            foreach (var element in _source)
            {
                long? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            double? max = null;
            foreach (var element in _source)
            {
                double? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            float? max = null;
            foreach (var element in _source)
            {
                float? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            decimal? max = null;
            foreach (var element in _source)
            {
                decimal? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            Func<TSource, int, TResult> __selector = (TSource item, int i) => _selector(item);
            return Max
                   (
                       Enumerable.SelectExtracted<TSource, TResult>
                       (
                           _source,
                           __selector
                       )
                   );
        }

        public static int Min(this IEnumerable<int> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = int.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(element, min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static long Min(this IEnumerable<long> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = long.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(element, min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static double Min(this IEnumerable<double> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = double.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(element, min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static float Min(this IEnumerable<float> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = float.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(element, min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static decimal Min(this IEnumerable<decimal> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = decimal.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(element, min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static int? Min(this IEnumerable<int?> source)
        {
            var _source = Check.NotNullArgument(source, "source");

            bool found = false;
            var min = int.MaxValue;

            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static long? Min(this IEnumerable<long?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = long.MaxValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static double? Min(this IEnumerable<double?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = double.MaxValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static float? Min(this IEnumerable<float?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = float.MaxValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static decimal? Min(this IEnumerable<decimal?> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = decimal.MaxValue;
            foreach (var element in _source)
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
            else
            {
                return null;
            }
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source)
        {
            var _source = Check.NotNullArgument(source, "source");
            var comparer = Comparer<TSource>.Default;
            TSource min = default(TSource);
            if (default(TSource) == null)
            {
                foreach (var element in _source)
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
            else
            {
                bool found = false;
                foreach (var element in _source)
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
                        continue;
                    }
                }
                if (found)
                {
                    return min;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = int.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(_selector(element), min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = long.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(_selector(element), min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = double.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(_selector(element), min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = float.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(_selector(element), min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = decimal.MaxValue;
            foreach (var element in _source)
            {
                min = Math.Min(_selector(element), min);
                found = true;
            }
            if (found)
            {
                return min;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            int? min = null;
            foreach (var element in _source)
            {
                int? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");

            bool found = false;
            long? min = null;
            foreach (var element in _source)
            {
                long? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            float? min = null;
            foreach (var element in _source)
            {
                float? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            double? min = null;
            foreach (var element in _source)
            {
                double? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            decimal? min = null;
            foreach (var element in _source)
            {
                decimal? item = _selector(element);
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
            else
            {
                return null;
            }
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            Func<TSource, int, TResult> __selector = (TSource item, int i) => _selector(item);
            return Min
                   (
                       Enumerable.SelectExtracted<TSource, TResult>
                       (
                           _source,
                           __selector
                       )
                   );
        }

        public static IEnumerable<int> Range(int start, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count");
            }
            else
            {
                if (((long)start + count) - 1L > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                    int end = start + count;
                    for (int index = start; index < end; index++)
                    {
                        yield return index;
                    }
                }
            }
        }

        public static int Sum(this IEnumerable<int> source)
        {
            int count = 0;
            foreach (int item in Check.NotNullArgument(source, "source"))
            {
                count += item;
            }
            return count;
        }

        public static int Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            return Sum(Select<TSource, int>(source, selector));
        }

        public static int? Sum(this IEnumerable<int?> source)
        {
            int sum = 0;
            foreach (int? item in Check.NotNullArgument(source, "source"))
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
            return Sum(Select<TSource, int?>(source, selector));
        }

        public static long Sum(this IEnumerable<long> source)
        {
            long sum = 0L;
            foreach (long item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
            }
            return sum;
        }

        public static long Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            return Sum(Select<TSource, long>(source, selector));
        }

        public static long? Sum(this IEnumerable<long?> source)
        {
            long sum = 0L;
            foreach (long? item in Check.NotNullArgument(source, "source"))
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
            return Sum(Select<TSource, long?>(source, selector));
        }

        public static float Sum(this IEnumerable<float> source)
        {
            float sum = 0.0f;
            foreach (float item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
            }
            return sum;
        }

        public static float Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector)
        {
            return Sum(Select<TSource, float>(source, selector));
        }

        public static float? Sum(this IEnumerable<float?> source)
        {
            float sum = 0f;
            foreach (float? item in Check.NotNullArgument(source, "source"))
            {
                if (item.HasValue)
                {
                    sum += (float)item.GetValueOrDefault();
                }
            }
            return sum;
        }

        public static float? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            return Sum(Select<TSource, float?>(source, selector));
        }

        public static double Sum(this IEnumerable<double> source)
        {
            double sum = 0.0d;
            foreach (double item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
            }
            return sum;
        }

        public static double Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector)
        {
            return Sum(Select<TSource, double>(source, selector));
        }

        public static double? Sum(this IEnumerable<double?> source)
        {
            double sum = 0;
            foreach (double? item in Check.NotNullArgument(source, "source"))
            {
                if (item.HasValue)
                {
                    sum += (double)item.GetValueOrDefault();
                }
            }
            return new double?(sum);
        }

        public static double? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            return Sum(Select<TSource, double?>(source, selector));
        }

        public static decimal Sum(this IEnumerable<decimal> source)
        {
            decimal sum = 0.0m;
            foreach (decimal item in Check.NotNullArgument(source, "source"))
            {
                sum += item;
            }
            return sum;
        }

        public static decimal Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector)
        {
            return Sum(Select<TSource, decimal>(source, selector));
        }

        public static decimal? Sum(this IEnumerable<decimal?> source)
        {
            decimal sum = 0.0m;
            foreach (decimal? item in Check.NotNullArgument(source, "source"))
            {
                if (item.HasValue)
                {
                    sum += item.GetValueOrDefault();
                }
            }
            return new decimal?(sum);
        }

        public static decimal? Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            return Sum(Select<TSource, decimal?>(source, selector));
        }
    }
}

#endif