// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Collections.Specialized;
using Theraot.Threading.Needles;

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

                case ICollection<T> collection:
                    return collection;

                default:
                    return EnumerationList<T>.Create(source);
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
                    return EnumerationList<T>.Create(source);
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

#if GREATERTHAN_NET45 || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                case T[] array:
                    return Array.AsReadOnly(array);
#endif

                case ListEx<T> list:
                    return list.AsReadOnly();

#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
                case List<T> list:
                    return list.AsReadOnly();
#endif

                case IReadOnlyCollection<T> result:
                    return result;

                default:
                    return EnumerationList<T>.Create(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyList<T> AsIReadOnlyList<T>(this IEnumerable<T>? source)
        {
            switch (source)
            {
                case null:
                    return EmptyCollection<T>.Instance;

#if GREATERTHAN_NET45 || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                case T[] array:
                    return Array.AsReadOnly(array);
#endif

                case ListEx<T> list:
                    return list.AsReadOnly();

#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
                case List<T> list:
                    return list.AsReadOnly();
#endif

                case IReadOnlyList<T> result:
                    return result;

                default:
                    return EnumerationList<T>.Create(source);
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
        public static ICollection<T> WrapAsICollection<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case ICollection<T> collection:
                    return collection;

                default:
                    return EnumerationList<T>.Create(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IList<T> WrapAsIList<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

                case IList<T> list:
                    return list;

                default:
                    return EnumerationList<T>.Create(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyCollection<T> WrapAsIReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

#if GREATERTHAN_NET45 || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                case T[] array:
                    return Array.AsReadOnly(array);
#endif
                case ListEx<T> list:
                    return list.AsReadOnly();

#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
                case List<T> list:
                    return list.AsReadOnly();
#endif

                case IReadOnlyCollection<T> result:
                    return result;

                default:
                    return EnumerationList<T>.Create(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static IReadOnlyList<T> WrapAsIReadOnlyList<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

#if GREATERTHAN_NET45 || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                case T[] array:
                    return Array.AsReadOnly(array);
#endif
                case ListEx<T> list:
                    return list.AsReadOnly();

#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
                case List<T> list:
                    return list.AsReadOnly();
#endif

                case IReadOnlyList<T> result:
                    return result;

                default:
                    return EnumerationList<T>.Create(source);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        [return: NotNull]
        public static ICollection<T> WrapAsReadOnlyICollection<T>(this IEnumerable<T> source)
        {
            switch (source)
            {
                case null:
                    throw new ArgumentNullException(nameof(source));

#if GREATERTHAN_NET45 || GREATERTHAN_NETCOREAPP11 || GREATERTHAN_NETSTANDARD16
                case T[] array:
                    return Array.AsReadOnly(array);
#endif
                case ListEx<T> list:
                    return list.AsReadOnly();

#if GREATERTHAN_NET45 || TARGETS_NETCORE || TARGETS_NETSTANDARD
                case List<T> list:
                    return list.AsReadOnly();
#endif

                default:
                    return EnumerationList<T>.Create(source);
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

                case ICollection<T> collection when collection.Count == 0:
                    return ArrayEx.Empty<T>();

                case ICollection<T> collection:
                    var result = new T[collection.Count];
                    collection.CopyTo(result, 0);
                    return result;

                default:
                    return new List<T>(source).ToArray();
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

        public static IEnumerable<TSource?> AsNullableClassEnumerable<TSource>(this IEnumerable<TSource> source)
                    where TSource : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new NullableClassEnumerable<TSource>(source);
        }

        public static IEnumerable<TSource?> AsNullableStructEnumerable<TSource>(this IEnumerable<TSource> source)
            where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new NullableStructEnumerable<TSource>(source);
        }

        private class NeedleEnumerable<TSource> : IEnumerable<ReadOnlyStructNeedle<TSource>>
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

        private class NullableClassEnumerable<TSource> : IEnumerable<TSource?>
                    where TSource : class
        {
            private readonly IEnumerable<TSource> _source;

            public NullableClassEnumerable(IEnumerable<TSource> source)
            {
                _source = source;
            }

            public IEnumerator<TSource?> GetEnumerator()
            {
                foreach (var item in _source)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class NullableStructEnumerable<TSource> : IEnumerable<TSource?>
                    where TSource : struct
        {
            private readonly IEnumerable<TSource> _source;

            public NullableStructEnumerable(IEnumerable<TSource> source)
            {
                _source = source;
            }

            public IEnumerator<TSource?> GetEnumerator()
            {
                foreach (var item in _source)
                {
                    yield return item;
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
        public static IReadOnlyCollection<TSource?> AsNullableClassReadOnlyCollection<TSource>(this IReadOnlyCollection<TSource> source)
            where TSource : class
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new ClassNullableCollection<TSource>(source);
        }

        public static IReadOnlyCollection<TSource?> AsNullableStructReadOnlyCollection<TSource>(this IReadOnlyCollection<TSource> source)
            where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new StructNullableCollection<TSource>(source);
        }

        private class ClassNullableCollection<TSource> : IReadOnlyCollection<TSource?>, ICollection<TSource?>
            where TSource : class
        {
            private readonly IReadOnlyCollection<TSource> _source;

            public ClassNullableCollection(IReadOnlyCollection<TSource> source)
            {
                _source = source;
            }

            public int Count => _source.Count;

            bool ICollection<TSource?>.IsReadOnly => true;

            void ICollection<TSource?>.Add(TSource? item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TSource?>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TSource? item)
            {
                return item != null && ContainsExtracted(item);
            }

            public void CopyTo(TSource?[] array, int arrayIndex)
            {
                CanCopyTo(Count, array, arrayIndex);
                Extensions.CopyTo(this, array, arrayIndex);
            }

            public IEnumerator<TSource?> GetEnumerator()
            {
                foreach (var item in _source)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool ICollection<TSource?>.Remove(TSource? item)
            {
                throw new NotSupportedException();
            }

            private bool ContainsExtracted(TSource item)
            {
                return System.Linq.Enumerable.Contains<TSource>(_source, item);
            }
        }

        private class StructNullableCollection<TSource> : IReadOnlyCollection<TSource?>, ICollection<TSource?>
            where TSource : struct
        {
            private readonly IReadOnlyCollection<TSource> _source;

            public StructNullableCollection(IReadOnlyCollection<TSource> source)
            {
                _source = source;
            }

            public int Count => _source.Count;

            bool ICollection<TSource?>.IsReadOnly => true;

            void ICollection<TSource?>.Add(TSource? item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TSource?>.Clear()
            {
                throw new NotSupportedException();
            }

            public bool Contains(TSource? item)
            {
                return item.HasValue && ContainsExtracted(item.Value);
            }

            public void CopyTo(TSource?[] array, int arrayIndex)
            {
                CanCopyTo(Count, array, arrayIndex);
                Extensions.CopyTo(this, array, arrayIndex);
            }

            public IEnumerator<TSource?> GetEnumerator()
            {
                foreach (var item in _source)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            bool ICollection<TSource?>.Remove(TSource? item)
            {
                throw new NotSupportedException();
            }

            private bool ContainsExtracted(TSource item)
            {
                return System.Linq.Enumerable.Contains<TSource>(_source, item);
            }
        }
    }
}