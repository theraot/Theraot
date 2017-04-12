#if FAT

using System;
using System.Collections.Generic;
using System.Linq;

using Theraot.Core;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static int Max(this IEnumerable<int> source, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = int.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(element, max) > 0)
                {
                    max = element;
                }
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

        public static long Max(this IEnumerable<long> source, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(element, max) > 0)
                {
                    max = element;
                }
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

        public static double Max(this IEnumerable<double> source, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(element, max) > 0)
                {
                    max = element;
                }
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

        public static float Max(this IEnumerable<float> source, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(element, max) > 0)
                {
                    max = element;
                }
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

        public static decimal Max(this IEnumerable<decimal> source, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(element, max) > 0)
                {
                    max = element;
                }
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

        public static int? Max(this IEnumerable<int?> source, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = int.MinValue;

            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
                    found = true;
                }
            }
            return found ? max : (int?)null;
        }

        public static long? Max(this IEnumerable<long?> source, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
                    found = true;
                }
            }
            return found ? max : (long?)null;
        }

        public static double? Max(this IEnumerable<double?> source, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
                    found = true;
                }
            }
            return found ? max : (double?)null;
        }

        public static float? Max(this IEnumerable<float?> source, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
                    found = true;
                }
            }
            return found ? max : (float?)null;
        }

        public static decimal? Max(this IEnumerable<decimal?> source, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
                    found = true;
                }
            }
            return found ? max : (decimal?)null;
        }

        public static TSource Max<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
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
            else
            {
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
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var max = int.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(selectorResult, max) > 0)
                {
                    max = selectorResult;
                }
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

        public static long Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var max = long.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(selectorResult, max) > 0)
                {
                    max = selectorResult;
                }
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

        public static double Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var max = double.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(selectorResult, max) > 0)
                {
                    max = selectorResult;
                }
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

        public static float Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var max = float.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(selectorResult, max) > 0)
                {
                    max = selectorResult;
                }
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

        public static decimal Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var max = decimal.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(selectorResult, max) > 0)
                {
                    max = selectorResult;
                }
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

        public static int? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            int? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && comparer.Compare(item.Value, max.Value) > 0)
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
            return found ? max : null;
        }

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            long? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && comparer.Compare(item.Value, max.Value) > 0)
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
            return found ? max : null;
        }

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            double? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && comparer.Compare(item.Value, max.Value) > 0)
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
            return found ? max : null;
        }

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            float? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && comparer.Compare(item.Value, max.Value) > 0)
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
            return found ? max : null;
        }

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            decimal? max = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && comparer.Compare(item.Value, max.Value) > 0)
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
            return found ? max : null;
        }

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Comparer<TResult> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            Func<TSource, int, TResult> selectorWrapper = (item, i) => selector(item);
            return Max(source.Select(selectorWrapper), comparer);
        }

        public static int Min(this IEnumerable<int> source, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = int.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(min, element) > 0)
                {
                    min = element;
                }
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

        public static long Min(this IEnumerable<long> source, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = long.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(min, element) > 0)
                {
                    min = element;
                }
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

        public static double Min(this IEnumerable<double> source, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = double.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(min, element) > 0)
                {
                    min = element;
                }
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

        public static float Min(this IEnumerable<float> source, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = float.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(min, element) > 0)
                {
                    min = element;
                }
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

        public static decimal Min(this IEnumerable<decimal> source, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = decimal.MinValue;
            foreach (var element in source)
            {
                if (comparer.Compare(min, element) > 0)
                {
                    min = element;
                }
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

        public static int? Min(this IEnumerable<int?> source, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = int.MinValue;

            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
                    found = true;
                }
            }
            return found ? min : (int?)null;
        }

        public static long? Min(this IEnumerable<long?> source, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = long.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
                    found = true;
                }
            }
            return found ? min : (long?)null;
        }

        public static double? Min(this IEnumerable<double?> source, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = double.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
                    found = true;
                }
            }
            return found ? min : (double?)null;
        }

        public static float? Min(this IEnumerable<float?> source, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = float.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
                    found = true;
                }
            }
            return found ? min : (float?)null;
        }

        public static decimal? Min(this IEnumerable<decimal?> source, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var found = false;
            var min = decimal.MinValue;
            foreach (var element in source)
            {
                if (element.HasValue)
                {
                    if (comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
                    found = true;
                }
            }
            return found ? min : (decimal?)null;
        }

        public static TSource Min<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var min = default(TSource);
            if (typeof(TSource).CanBeNull())
            {
                foreach (var element in source)
                {
                    if (!ReferenceEquals(element, null))
                    {
                        if (ReferenceEquals(min, null) || comparer.Compare(min, element) > 0)
                        {
                            min = element;
                        }
                    }
                }
                return min;
            }
            else
            {
                var found = false;
                foreach (var element in source)
                {
                    if (found)
                    {
                        if (comparer.Compare(min, element) > 0)
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
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var min = int.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(min, selectorResult) > 0)
                {
                    min = selectorResult;
                }
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

        public static long Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var min = long.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(min, selectorResult) > 0)
                {
                    min = selectorResult;
                }
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

        public static double Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var min = double.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(min, selectorResult) > 0)
                {
                    min = selectorResult;
                }
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

        public static float Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float> selector, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var min = float.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(min, selectorResult) > 0)
                {
                    min = selectorResult;
                }
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

        public static decimal Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal> selector, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            var min = decimal.MinValue;
            foreach (var element in source)
            {
                var selectorResult = selector.Invoke(element);
                if (comparer.Compare(min, selectorResult) > 0)
                {
                    min = selectorResult;
                }
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

        public static int? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int?> selector, IComparer<int> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            int? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && comparer.Compare(min.Value, item.Value) > 0)
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
            return found ? min : null;
        }

        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, IComparer<long> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            long? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && comparer.Compare(min.Value, item.Value) > 0)
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
            return found ? min : null;
        }

        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, IComparer<double> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            double? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && comparer.Compare(min.Value, item.Value) > 0)
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
            return found ? min : null;
        }

        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, IComparer<float> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            float? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && comparer.Compare(min.Value, item.Value) > 0)
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
            return found ? min : null;
        }

        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, IComparer<decimal> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            var found = false;
            decimal? min = null;
            foreach (var element in source)
            {
                var item = selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && comparer.Compare(min.Value, item.Value) > 0)
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
            return found ? min : null;
        }

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Comparer<TResult> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            Func<TSource, int, TResult> selectorWrapper = (item, i) => selector(item);
            return Min(source.Select(selectorWrapper), comparer);
        }
    }
}

#endif