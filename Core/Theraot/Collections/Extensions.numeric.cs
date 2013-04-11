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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = int.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(element, max) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(element, max) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(element, max) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(element, max) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(element, max) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = int.MinValue;

            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
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

        public static long? Max(this IEnumerable<long?> source, IComparer<long> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
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

        public static double? Max(this IEnumerable<double?> source, IComparer<double> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
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

        public static float? Max(this IEnumerable<float?> source, IComparer<float> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
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

        public static decimal? Max(this IEnumerable<decimal?> source, IComparer<decimal> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(element.Value, max) > 0)
                    {
                        max = element.Value;
                    }
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

        public static TSource Max<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
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
                        if (max == null || _comparer.Compare(element, max) > 0)
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
                        if (_comparer.Compare(element, max) > 0)
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

        public static int Max<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, IComparer<int> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = int.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(_element, max) > 0)
                {
                    max = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = long.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(_element, max) > 0)
                {
                    max = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = double.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(_element, max) > 0)
                {
                    max = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = float.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(_element, max) > 0)
                {
                    max = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var max = decimal.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(_element, max) > 0)
                {
                    max = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            int? max = null;
            foreach (var element in _source)
            {
                int? item = _selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(item.Value, max.Value) > 0)
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

        public static long? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, IComparer<long> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            long? max = null;
            foreach (var element in _source)
            {
                long? item = _selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(item.Value, max.Value) > 0)
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

        public static double? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, IComparer<double> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            double? max = null;
            foreach (var element in _source)
            {
                double? item = _selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(item.Value, max.Value) > 0)
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

        public static float? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, IComparer<float> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            float? max = null;
            foreach (var element in _source)
            {
                float? item = _selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(item.Value, max.Value) > 0)
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

        public static decimal? Max<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, IComparer<decimal> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            decimal? max = null;
            foreach (var element in _source)
            {
                decimal? item = _selector(element);
                if (max.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(item.Value, max.Value) > 0)
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

        public static TResult Max<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Comparer<TResult> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            Func<TSource, int, TResult> __selector = (TSource item, int i) => _selector(item);
            return Max
                   (
                       Enumerable.Select<TSource, TResult>
                       (
                           _source,
                           __selector
                       ),
                       _comparer
                   );
        }

        public static int Min(this IEnumerable<int> source, IComparer<int> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = int.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(min, element) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = long.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(min, element) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = double.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(min, element) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = float.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(min, element) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = decimal.MinValue;
            foreach (var element in _source)
            {
                if (_comparer.Compare(min, element) > 0)
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = int.MinValue;

            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
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

        public static long? Min(this IEnumerable<long?> source, IComparer<long> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = long.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
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

        public static double? Min(this IEnumerable<double?> source, IComparer<double> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = double.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
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

        public static float? Min(this IEnumerable<float?> source, IComparer<float> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = float.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
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

        public static decimal? Min(this IEnumerable<decimal?> source, IComparer<decimal> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            bool found = false;
            var min = decimal.MinValue;
            foreach (var element in _source)
            {
                if (element.HasValue)
                {
                    if (_comparer.Compare(min, element.Value) > 0)
                    {
                        min = element.Value;
                    }
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

        public static TSource Min<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            TSource min = default(TSource);
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
                        if (min == null || _comparer.Compare(min, element) > 0)
                        {
                            min = element;
                        }
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
                        if (_comparer.Compare(min, element) > 0)
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

        public static int Min<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector, IComparer<int> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = int.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(min, _element) > 0)
                {
                    min = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = long.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(min, _element) > 0)
                {
                    min = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = double.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(min, _element) > 0)
                {
                    min = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = float.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(min, _element) > 0)
                {
                    min = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            var min = decimal.MinValue;
            foreach (var element in _source)
            {
                var _element = _selector.Invoke(element);
                if (_comparer.Compare(min, _element) > 0)
                {
                    min = _element;
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
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            int? min = null;
            foreach (var element in _source)
            {
                int? item = _selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(min.Value, item.Value) > 0)
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

        public static long? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, long?> selector, IComparer<long> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            long? min = null;
            foreach (var element in _source)
            {
                long? item = _selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(min.Value, item.Value) > 0)
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

        public static double? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, double?> selector, IComparer<double> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            double? min = null;
            foreach (var element in _source)
            {
                double? item = _selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(min.Value, item.Value) > 0)
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

        public static float? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, float?> selector, IComparer<float> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            float? min = null;
            foreach (var element in _source)
            {
                float? item = _selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(min.Value, item.Value) > 0)
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

        public static decimal? Min<TSource>(this IEnumerable<TSource> source, Func<TSource, decimal?> selector, IComparer<decimal> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            bool found = false;
            decimal? min = null;
            foreach (var element in _source)
            {
                decimal? item = _selector(element);
                if (min.HasValue)
                {
                    if (item.HasValue && _comparer.Compare(min.Value, item.Value) > 0)
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

        public static TResult Min<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, Comparer<TResult> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            var _source = Check.NotNullArgument(source, "source");
            var _selector = Check.NotNullArgument(selector, "selector");
            Func<TSource, int, TResult> __selector = (TSource item, int i) => _selector(item);
            return Min
                   (
                       Enumerable.Select<TSource, TResult>
                       (
                           _source,
                           __selector
                       ),
                       _comparer
                   );
        }
    }
}