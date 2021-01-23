// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;
using Theraot.Threading.Needles;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static IEnumerable<ReadOnlyStructNeedle<TSource>> AsNeedleEnumerable<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new NeedleEnumerable<TSource>(source);
        }

        private sealed class NeedleEnumerable<TSource> : IEnumerable<ReadOnlyStructNeedle<TSource>>
        {
            private readonly IEnumerable<TSource> _source;

            public NeedleEnumerable(IEnumerable<TSource> source)
            {
                _source = source;
            }

            public IEnumerator<ReadOnlyStructNeedle<TSource>> GetEnumerator()
            {
                foreach (var item in _source)
                {
                    yield return new ReadOnlyStructNeedle<TSource>(item);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }

    public static partial class Extensions
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static T[] AsArray<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is T[] array)
            {
                return array;
            }

            if (source is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return ArrayEx.Empty<T>();
                }

                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }

            // ReSharper disable once RemoveConstructorInvocation
            return new List<T>(source).ToArray();
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> AsDistinctICollection<T>(this IEnumerable<T>? source)
        {
#if NET35
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is ISet<T> set)
            {
                return set;
            }

            if (source is HashSet<T> resultHashSet)
            {
                // Workaround for .NET 3.5 when all you want is Contains and no duplicates
                // Remember that On .NET 3.5 HashSet is not an ISet
                return resultHashSet;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(), EqualityComparer<T>.Default);
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
            if (source == null)
            {
                return new EmptySet<T>(comparer);
            }

            if (source is HashSet<T> sourceAsHashSet && sourceAsHashSet.Comparer.Equals(comparer))
            {
                return sourceAsHashSet;
            }

            if (source is SortedSet<T> sourceAsSortedSet && sourceAsSortedSet.Comparer.Equals(comparer))
            {
                return sourceAsSortedSet;
            }

            if (source is ProgressiveSet<T> sourceAsProgressiveSet && sourceAsProgressiveSet.Comparer.Equals(comparer))
            {
                return sourceAsProgressiveSet;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(), comparer);
#else
            return AsISet(source, comparer);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> AsICollection<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is ICollection<T> collection)
            {
                return collection;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IList<T> AsIList<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is IList<T> list)
            {
                return list;
            }

            if (source is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return ArrayEx.Empty<T>();
                }

                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyCollection<T> AsIReadOnlyCollection<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (source is T[] array)
            {
#if LESSTHAN_NET45
                return new ReadOnlyCollectionEx<T>(array);
#else
                return ArrayEx.AsReadOnly(array);
#endif
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }
#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
            if (source is List<T> list)
            {
                return list.AsReadOnly();
            }
#endif
            if (source is IReadOnlyCollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyList<T> AsIReadOnlyList<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (source is T[] array)
            {
#if LESSTHAN_NET45
                return new ReadOnlyCollectionEx<T>(array);
#else
                return ArrayEx.AsReadOnly(array);
#endif
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }
#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
            if (source is List<T> list)
            {
                return list.AsReadOnly();
            }
#endif
            if (source is IReadOnlyList<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ISet<T> AsISet<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return EmptySet<T>.Instance;
            }

            if (source is ISet<T> resultISet)
            {
                return resultISet;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(), EqualityComparer<T>.Default);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ISet<T> AsISet<T>(this IEnumerable<T>? source, IEqualityComparer<T>? comparer)
        {
            comparer ??= EqualityComparer<T>.Default;
            if (source == null)
            {
                return EmptySet<T>.Instance;
            }
#if !NET35
            if (source is HashSet<T> sourceAsHashSet && sourceAsHashSet.Comparer.Equals(comparer))
            {
                return sourceAsHashSet;
            }
#endif
            if (source is SortedSet<T> sourceAsSortedSet && sourceAsSortedSet.Comparer.Equals(comparer))
            {
                return sourceAsSortedSet;
            }

            if (source is ProgressiveSet<T> sourceAsProgressiveSet && sourceAsProgressiveSet.Comparer.Equals(comparer))
            {
                return sourceAsProgressiveSet;
            }

            return ProgressiveSet<T>.Create(Progressor<T>.CreateFromIEnumerable(source), new HashSetEx<T>(), comparer);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static List<T> AsList<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return new List<T>();
            }

            if (source is T[] array)
            {
                return new List<T>(array);
            }

            if (source is List<T> list)
            {
                return list;
            }

            if (source is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return new List<T>();
                }

                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return new List<T>(result);
            }

            return new List<T>(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IEnumerable<T> AsUnaryIEnumerable<T>(this T source)
        {
            yield return source!;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ReadOnlyCollectionEx<T> ToReadOnlyCollection<T>(this IEnumerable<T>? enumerable)
        {
            if (enumerable == null)
            {
                return EmptyCollection<T>.Instance;
            }

            if (enumerable is ReadOnlyCollectionEx<T> readOnlyCollectionEx)
            {
                return readOnlyCollectionEx;
            }

            if (enumerable is T[] array)
            {
                return array.Length == 0 ? EmptyCollection<T>.Instance : new ReadOnlyCollectionEx<T>(array);
            }

            if (enumerable is ICollection<T> collection)
            {
                if (collection.Count == 0)
                {
                    return EmptyCollection<T>.Instance;
                }

                var result = new T[collection.Count];
                collection.CopyTo(result, 0);
                return new ReadOnlyCollectionEx<T>(result);
            }

            return new ReadOnlyCollectionEx<T>(new List<T>(enumerable));
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static bool TryGetComparer<TKey, TValue>(this IDictionary<TKey, TValue> source, [NotNullWhen(true)] out IEqualityComparer<TKey>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is IHasComparer<TKey> sourceHasComparer)
            {
                comparer = sourceHasComparer.Comparer;
                return true;
            }

            if (source is Dictionary<TKey, TValue> sourceAsDictionary)
            {
                comparer = sourceAsDictionary.Comparer;
                return true;
            }

            comparer = null;
            return false;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IDictionary<TKey, TValue> WithComparer<TKey, TValue>(this IDictionary<TKey, TValue> source, IEqualityComparer<TKey>? comparer)
        {
            comparer ??= EqualityComparer<TKey>.Default;
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is IHasComparer<TKey> sourceHasComparer && sourceHasComparer.Comparer.Equals(comparer))
            {
                return source;
            }

            if (source is Dictionary<TKey, TValue> sourceAsDictionary && sourceAsDictionary.Comparer.Equals(comparer))
            {
                return sourceAsDictionary;
            }

            return new DictionaryEx<TKey, TValue>(source, comparer);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> WrapAsICollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is ICollection<T> collection)
            {
                return collection;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IList<T> WrapAsIList<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is IList<T> list)
            {
                return list;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is T[] array)
            {
#if LESSTHAN_NET45
                return new ReadOnlyCollectionEx<T>(array);
#else
                return ArrayEx.AsReadOnly(array);
#endif
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }
#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
            if (source is List<T> list)
            {
                return list.AsReadOnly();
            }
#endif
            if (source is IReadOnlyCollection<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyList<T> WrapAsIReadOnlyList<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is T[] array)
            {
#if LESSTHAN_NET45
                return new ReadOnlyCollectionEx<T>(array);
#else
                return ArrayEx.AsReadOnly(array);
#endif
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }
#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
            if (source is List<T> list)
            {
                return list.AsReadOnly();
            }
#endif
            if (source is IReadOnlyList<T> result)
            {
                return result;
            }

            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> WrapAsReadOnlyICollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source is T[] array)
            {
                return ArrayEx.AsReadOnly(array);
            }

            if (source is ListEx<T> listEx)
            {
                return listEx.AsReadOnly();
            }
#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
            if (source is List<T> list)
            {
                return list.AsReadOnly();
            }
#endif
            return EnumerationList<T>.Create(source);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        internal static T[] AsArrayInternal<T>(this IEnumerable<T>? source)
        {
            if (source == null)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is T[] array)
            {
                return array;
            }

            if (source is ReadOnlyCollectionEx<T> readOnlyCollectionEx)
            {
                return readOnlyCollectionEx.Wrapped is T[] wrappedArray ? wrappedArray : readOnlyCollectionEx.ToArray();
            }

            if (source is ICollection<T> collection1 && collection1.Count == 0)
            {
                return ArrayEx.Empty<T>();
            }

            if (source is ICollection<T> collection2)
            {
                var result = new T[collection2.Count];
                collection2.CopyTo(result, 0);
                return result;
            }

            // ReSharper disable once RemoveConstructorInvocation
            return new List<T>(source).ToArray();
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

        [return: NotNull]
        public static IEnumerable<T> Skip<T>(this IEnumerable<T> source, Predicate<T>? predicateCount, int skipCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return predicateCount == null ? SkipExtracted(source, skipCount) : SkipExtracted(source, predicateCount, skipCount);
        }

        [return: NotNull]
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

        [return: NotNull]
        public static IEnumerable<T> Take<T>(this IEnumerable<T> source, Predicate<T>? predicateCount, int takeCount)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return predicateCount == null ? TakeExtracted(source, takeCount) : TakeExtracted(source, predicateCount, takeCount);
        }

        [return: NotNull]
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

        private static IEnumerable<T> SkipExtracted<T>(IEnumerable<T> source, Predicate<T> predicateCount, int skipCount)
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

                if (predicateCount!(item))
                {
                    count++;
                }
            }
        }
    }
}