// Needed for NET40

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static T[] AsArray<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return ArrayEx.Empty<T>();

                case T[] array:
                    return array;

                case ICollection<T> collection:
                    if (collection.Count == 0)
                    {
                        return ArrayEx.Empty<T>();
                    }
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new List<T>(source).ToArray();
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> AsDistinctICollection<T>(this IEnumerable<T>? source)
        {
#if NET35
            switch (source)
            {
                case null:
                    return ArrayEx.Empty<T>();

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
        [return: NotNull]
        public static ICollection<T> AsDistinctICollection<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
        {
            comparer ??= EqualityComparer<T>.Default;
#if NET35
            switch (source)
            {
                case null:
                    return new EmptySet<T>(comparer);

                case HashSet<T> sourceAsHashSet when sourceAsHashSet.Comparer.Equals(comparer):
                    return sourceAsHashSet;

                case ProgressiveSet<T> sourceAsProgressiveSet when sourceAsProgressiveSet.Comparer.Equals(comparer):
                    return sourceAsProgressiveSet;

                default:
                    return new ProgressiveSet<T>(source, comparer);
            }
#else
            return AsISet(source, comparer);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> AsICollection<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return ArrayEx.Empty<T>();

                case ICollection<T> result:
                    return result;

                default:
                    return new ProgressiveCollection<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IList<T> AsIList<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return ArrayEx.Empty<T>();

                case IList<T> list:
                    return list;

                case ICollection<T> collection:
                    if (collection.Count == 0)
                    {
                        return ArrayEx.Empty<T>();
                    }
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new ProgressiveList<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyCollection<T> AsIReadOnlyCollection<T>(this IEnumerable<T>? source)
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
        [return: NotNull]
        public static IReadOnlyCollection<T> AsIReadOnlyList<T>(this IEnumerable<T>? source)
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
        [return: NotNull]
        public static ISet<T> AsISet<T>(this IEnumerable<T>? source)
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
        [return: NotNull]
        public static ISet<T> AsISet<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
        {
            comparer ??= EqualityComparer<T>.Default;
            switch (source)
            {
                case null:
                    return EmptySet<T>.Instance;

#if !NET35
                case HashSet<T> sourceAsHashSet when sourceAsHashSet.Comparer.Equals(comparer):
                    return sourceAsHashSet;
#endif

                case SortedSet<T> sourceAsSortedSet when sourceAsSortedSet.Comparer.Equals(comparer):
                    return sourceAsSortedSet;

                case ProgressiveSet<T> sourceAsProgressiveSet when sourceAsProgressiveSet.Comparer.Equals(comparer):
                    return sourceAsProgressiveSet;

                default:
                    return new ProgressiveSet<T>(source, comparer);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static List<T> AsList<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return new List<T>();

                case T[] array:
                    return new List<T>(array);

                case List<T> list:
                    return list;

                case ICollection<T> collection:
                    if (collection.Count == 0)
                    {
                        return new List<T>();
                    }
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return new List<T>(result);

                default:
                    return new List<T>(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IEnumerable<T> AsUnaryIEnumerable<T>([AllowNull] this T source)
        {
            yield return source;
        }

        [return: NotNull]
        public static ReadOnlyCollectionEx<T> ToReadOnlyCollection<T>(this IEnumerable<T>? enumerable)
        {
            switch (enumerable)
            {
                case null:
                    return EmptyCollection<T>.Instance;

                case ReadOnlyCollectionEx<T> readOnlyCollectionEx:
                    return readOnlyCollectionEx;

                case T[] array:
                    return array.Length == 0 ? EmptyCollection<T>.Instance : new ReadOnlyCollectionEx<T>(array);

                case ICollection<T> collection:
                    if (collection.Count == 0)
                    {
                        return EmptyCollection<T>.Instance;
                    }
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return new ReadOnlyCollectionEx<T>(result);

                default:
                    return new ReadOnlyCollectionEx<T>(new List<T>(enumerable));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> WrapAsICollection<T>([NotNull] this IEnumerable<T> source)
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
        [return: NotNull]
        public static IList<T> WrapAsIList<T>([NotNull] this IEnumerable<T> source)
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
        [return: NotNull]
        public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>([NotNull] this IEnumerable<T> source)
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
        [return: NotNull]
        public static IReadOnlyList<T> WrapAsIReadOnlyList<T>([NotNull] this IEnumerable<T> source)
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
        [return: NotNull]
        internal static T[] AsArrayInternal<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return ArrayEx.Empty<T>();

                case T[] array:
                    return array;

                case ReadOnlyCollectionEx<T> readOnlyCollectionEx:
                    return readOnlyCollectionEx.Wrapped is T[] wrappedArray ? wrappedArray : readOnlyCollectionEx.ToArray();

                case ICollection<T> collection:
                    if (collection.Count == 0)
                    {
                        return ArrayEx.Empty<T>();
                    }
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new List<T>(source).ToArray();
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return new HashSet<T>();

                case HashSet<T> hashSet:
                    return hashSet;

                default:
                    return new HashSet<T>(source);
            }
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
        {
            comparer ??= EqualityComparer<T>.Default;
            switch (source)
            {
                case null:
                    return new HashSet<T>(comparer);

                case HashSet<T> hashSet when hashSet.Comparer.Equals(comparer):
                    return hashSet;

                default:
                    return new HashSet<T>(source, comparer);
            }
        }
    }

    public static partial class Extensions
    {
        public static bool HasAtLeast<TSource>([NotNull] this IEnumerable<TSource> source, int count)
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

        [return: NotNull]
        public static IEnumerable<T> Skip<T>([NotNull] this IEnumerable<T> source, Predicate<T>? predicateCount, int skipCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return predicateCount == null ? SkipExtracted(source, skipCount) : SkipExtracted(source, predicateCount, skipCount);
        }

        [return: NotNull]
        public static IEnumerable<T> Step<T>([NotNull] this IEnumerable<T> source, int stepCount)
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

        [return: NotNull]
        public static IEnumerable<T> Take<T>([NotNull] this IEnumerable<T> source, Predicate<T>? predicateCount, int takeCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return predicateCount == null ? TakeExtracted(source, takeCount) : TakeExtracted(source, predicateCount, takeCount);
        }

        [return: NotNull]
        public static T[] ToArray<T>([NotNull] this IEnumerable<T> source, int count)
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

        private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, [NotNull] Predicate<T> predicateCount, int skipCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count < skipCount)
                {
                    if (predicateCount!(item))
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

        private static IEnumerable<T> TakeExtracted<T>(IEnumerable<T> source, [NotNull] Predicate<T> predicateCount, int takeCount)
        {
            var count = 0;
            foreach (var item in source)
            {
                if (count == takeCount)
                {
                    break;
                }

                yield return item;
                if (predicateCount!(item))
                {
                    count++;
                }
            }
        }
    }
}