// Needed for NET40

#pragma warning disable CC0031 // Check for null before calling a delegate

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;
using Theraot.Collections.ThreadSafe;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static T[] AsArray<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return ArrayReservoir<T>.EmptyArray;

                case T[] array:
                    return array;

                case ICollection<T> collection when collection.Count == 0:
                    return ArrayReservoir<T>.EmptyArray;

                case ICollection<T> collection:
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new List<T>(source).ToArray();
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        internal static T[] AsArrayInternal<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return ArrayReservoir<T>.EmptyArray;

                case T[] array:
                    return array;

                case ReadOnlyCollectionEx<T> readOnlyCollectionEx when readOnlyCollectionEx.Wrapped is T[] array:
                    return array;

                case ICollection<T> collection when collection.Count == 0:
                    return ArrayReservoir<T>.EmptyArray;

                case ICollection<T> collection:
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new List<T>(source).ToArray();
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ICollection<T> AsDistinctICollection<T>(IEnumerable<T> source)
        {
#if NET35
            switch (source)
            {
                case null:
                    return ArrayReservoir<T>.EmptyArray;

                case ISet<T> set:
                    return set;

                case HashSet<T> resultHashSet:
                    // Workaround for .NET 3.5 when all you want is Contains and no duplicates
                    // Remember that On .NET 3.5 HashSet is not an ISet
                    return resultHashSet;

                default:
                    return new ProgressiveSet<T>(source);
            }
#else
            return AsISet(source);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ICollection<T> AsICollection<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return ArrayReservoir<T>.EmptyArray;

                case ICollection<T> result:
                    return result;

                default:
                    return new ProgressiveCollection<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IList<T> AsIList<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return ArrayReservoir<T>.EmptyArray;

                case IList<T> list:
                    return list;

                case ICollection<T> collection when collection.Count == 0:
                    return EmptyCollection<T>.Instance;

                case ICollection<T> collection:
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new ProgressiveList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IReadOnlyCollection<T> AsIReadOnlyCollection<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return EmptyCollection<T>.Instance;

                case IReadOnlyCollection<T> result:
                    return result;

                default:
                    return new ProgressiveList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IReadOnlyCollection<T> AsIReadOnlyList<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return EmptyCollection<T>.Instance;

                case IReadOnlyList<T> result:
                    return result;

                default:
                    return new ProgressiveCollection<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ISet<T> AsISet<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    return EmptySet<T>.Instance;

                case ISet<T> resultISet:
                    return resultISet;

                default:
                    return new ProgressiveSet<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IEnumerable<T> AsUnaryIEnumerable<T>(T source)
        {
            yield return source;
        }

        public static ReadOnlyCollectionEx<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            switch (enumerable)
            {
                case null:
                    return EmptyCollection<T>.Instance;
                case ReadOnlyCollectionEx<T> arrayReadOnlyCollection:
                    return arrayReadOnlyCollection;
                default:
                {
                    var array = AsArrayInternal(enumerable);
                    return array.Length == 0 ? EmptyCollection<T>.Instance : ReadOnlyCollectionEx.Create(array);
                }
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static ICollection<T> WrapAsICollection<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case ICollection<T> collection:
                    return collection;

                default:
                    return new EnumerationList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IList<T> WrapAsIList<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case IList<T> list:
                    return list;

                default:
                    return new EnumerationList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case T[] array:
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NET45
                    return new EnumerationList<T>(array);
#else
                    return Array.AsReadOnly(array);
#endif
                case List<T> list:
#if LESSTHAN_NETSTANDARD13 || LESSTHAN_NET45
                    return new EnumerationList<T>(list);
#else
                    return list.AsReadOnly();
#endif

                default:
                    return new EnumerationList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static IReadOnlyList<T> WrapAsIReadOnlyList<T>(IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case T[] array:
#if LESSTHAN_NETSTANDARD20 || LESSTHAN_NETCOREAPP20 || LESSTHAN_NET45
                    return new EnumerationList<T>(array);
#else
                    return Array.AsReadOnly(array);
#endif
                case List<T> list:
#if LESSTHAN_NETSTANDARD13 || LESSTHAN_NET45
                    return new EnumerationList<T>(list);
#else
                    return list.AsReadOnly();
#endif

                default:
                    return new EnumerationList<T>(source);
            }
        }
    }

    public static partial class Extensions
    {
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

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return predicateCount == null ? SkipExtracted(source, skipCount) : SkipExtracted(source, predicateCount, skipCount);
        }

        public static IEnumerable<T> Step<T>(this IEnumerable<T> source, int stepCount)
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

        public static IEnumerable<T> Take<T>(this IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return predicateCount == null ? TakeExtracted(source, takeCount) : TakeExtracted(source, predicateCount, takeCount);
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

        private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
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

        private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, int skipCount)
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

        private static IEnumerable<T> TakeExtracted<T>(IEnumerable<T> source, int takeCount)
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

        private static IEnumerable<T> TakeExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int takeCount)
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