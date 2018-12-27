// Needed for NET40

using System;
using System.Collections.Generic;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

#if NET20 || NET30 || NET35

using System.Runtime.CompilerServices;

#endif

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static T[] AsArray<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                return ArrayReservoir<T>.EmptyArray;
            }
            if (source is T[] array)
            {
                return array;
            }
#if NET20 || NET30 || NET35
            if (source is TrueReadOnlyCollection<T> trueReadOnlyCollection)
            {
                return trueReadOnlyCollection.Wrapped;
            }
#endif
            if (source is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return ArrayReservoir<T>.EmptyArray;
                }
                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
            return new List<T>(source).ToArray();
        }

        public static ICollection<T> AsDistinctICollection<T>(IEnumerable<T> source)
        {
#if NET35
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            // Workaround for .NET 3.5 when all you want is Contains and no duplicates
            // Remember that On .NET 3.5 HashSet is not an ISet
            if (source is HashSet<T> resultHashSet)
            {
                return resultHashSet;
            }
            var resultISet = source as ISet<T>;
            return resultISet ?? new ProgressiveSet<T>(source);
#else
            return AsISet(source);
#endif
        }

        public static ICollection<T> AsICollection<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            var result = source as ICollection<T>;
            return result ?? new ProgressiveCollection<T>(source);
        }

        public static IList<T> AsIList<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (source is IList<T> list)
            {
                return list;
            }
            if (source is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return EmptyCollection<T>.Instance;
                }
                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }
            return new ProgressiveList<T>(source);
        }

        public static ISet<T> AsISet<T>(IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            // Remember that On .NET 3.5 HashSet is not an ISet
            var resultISet = source as ISet<T>;
            return resultISet ?? new ProgressiveSet<T>(source);
        }

        public static IEnumerable<T> AsUnaryIEnumerable<T>(T source)
        {
            yield return source;
        }

        public static bool HasAtLeast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (count == 0)
            {
                return true;
            }
            if (source is ICollection<TSource> sourceAsCollection)
            {
                return sourceAsCollection.Count >= count;
            }
            var result = 0;
            using (var item = source.GetEnumerator())
            {
                while (item.MoveNext())
                {
                    checked
                    {
                        result++;
                    }
                    if (result == count)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> source, int skipCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return SkipItemsExtracted(source, skipCount);
        }

        public static IEnumerable<T> SkipItems<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return predicateCount == null ? SkipItemsExtracted(source, skipCount) : SkipItemsExtracted(source, predicateCount, skipCount);
        }

        public static IEnumerable<T> StepItems<T>(this IEnumerable<T> source, int stepCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return StepItemsExtracted();

            IEnumerable<T> StepItemsExtracted()
            {
                var count = 0;
                foreach (var item in source)
                {
                    if (count % stepCount == 0)
                    {
                        count++;
                    }
                    else
                    {
                        yield return item;
                        count++;
                    }
                }
            }
        }

        public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> source, int takeCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return TakeItemsExtracted(source, takeCount);
        }

        public static IEnumerable<T> TakeItems<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return predicateCount == null ? TakeItemsExtracted(source, takeCount) : TakeItemsExtracted(source, predicateCount, takeCount);
        }

        public static T[] ToArray<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (count < 0)
            {
                throw new ArgumentNullException(nameof(count));
            }
            if (source is ICollection<T> collection && count >= collection.Count)
            {
                var array = new T[collection.Count];
                collection.CopyTo(array, 0);
                return array;
            }
            if (source is string str && count >= str.Length)
            {
                var array = new char[str.Length];
                str.CopyTo(0, array, 0, str.Length);
                return (T[])(object)array;
            }
            var result = new List<T>(count);
            foreach (var item in source)
            {
                if (result.Count == count)
                {
                    break;
                }
                result.Add(item);
            }
            return result.ToArray();
        }

        public static IList<T> WrapAsIList<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case IList<T> result:
                    return result;

                default:
                    return new EnumerationList<T>(source);
            }
        }

        public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));
                case T[] array:
                    return new EnumerationList<T>(array);

                case ICollection<T> collection:
                    return new EnumerationList<T>(collection);

                case IReadOnlyCollection<T> result:
                    return result;

                default:
                    return new EnumerationList<T>(source);
            }
        }

        private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count < skipCount)
                {
                    if (predicateCount(item))
                    {
                        count++;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> SkipItemsExtracted<T>(IEnumerable<T> source, int skipCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count < skipCount)
                {
                    count++;
                }
                else
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> source, int takeCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count == takeCount)
                {
                    break;
                }
                yield return item;
                count++;
            }
        }

        private static IEnumerable<T> TakeItemsExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count == takeCount)
                {
                    break;
                }
                yield return item;
                if (predicateCount(item))
                {
                    count++;
                }
            }
        }
    }
}