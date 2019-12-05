// Needed for NET40

#pragma warning disable CC0031 // Check for null before calling a delegate
#pragma warning disable RCS1224 // Make method an extension method.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Theraot.Collections
{
    [DebuggerNonUserCode]
    public static partial class Extensions
    {
        public static ReadOnlyCollectionEx<T> AddFirst<T>(this ReadOnlyCollection<T> list, T item)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var res = new T[list.Count + 1];
            res[0] = item;
            list.CopyTo(res, 1);
            return ReadOnlyCollectionEx.Create(res);
        }

        public static T[] AddFirst<T>(this T[] array, T item)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var res = new T[array.Length + 1];
            res[0] = item;
            array.CopyTo(res, 1);
            return res;
        }

        public static ReadOnlyCollectionEx<T> AddLast<T>(this ReadOnlyCollection<T> list, T item)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var res = new T[list.Count + 1];
            list.CopyTo(res, 0);
            res[list.Count] = item;
            return ReadOnlyCollectionEx.Create(res);
        }

        public static T[] AddLast<T>(this T[] array, T item)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var res = new T[array.Length + 1];
            array.CopyTo(res, 0);
            res[array.Length] = item;
            return res;
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CanCopyTo(int count, Array array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CanCopyTo(int count, Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CanCopyTo<T>(int count, T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (count > array.Length)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CanCopyTo<T>(int count, T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CanCopyTo<T>(T[] array, int arrayIndex, int countLimit)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(countLimit), "Non-negative number is required.");
            }

            if (countLimit > array.Length - arrayIndex)
            {
                throw new ArgumentException("The array can not contain the number of elements.", nameof(array));
            }
        }

        public static void Consume<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            foreach (var _ in source)
            {
                // Empty
            }
        }

        public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            IEqualityComparer<T> comparer = EqualityComparer<T>.Default;
            var localCollection = AsICollection(source);
            return items.Any(item => localCollection.Contains(item, comparer));
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source, Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array)
        {
            ConvertedCopyTo(source.Skip(sourceIndex), conversion, array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source, Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array, int arrayIndex)
        {
            ConvertedCopyTo(source.Skip(sourceIndex), conversion, array, arrayIndex);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source, Func<TCovered, TUncovered> conversion, int sourceIndex, TUncovered[] array, int arrayIndex, int countLimit)
        {
            ConvertedCopyTo(source.Skip(sourceIndex).Take(countLimit), conversion, array, arrayIndex);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source, Func<TCovered, TUncovered> conversion, TUncovered[] array)
        {
            ConvertedCopyTo(source, conversion, array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TUnderlying, TUncovered>(this IEnumerable<TUnderlying> source, Func<TUnderlying, TUncovered> conversion, TUncovered[] array, int arrayIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (conversion == null)
            {
                throw new ArgumentNullException(nameof(conversion));
            }

            try
            {
                var index = arrayIndex;
                foreach (var item in source)
                {
                    array[index] = conversion(item);
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void ConvertedCopyTo<TCovered, TUncovered>(this IEnumerable<TCovered> source, Func<TCovered, TUncovered> conversion, TUncovered[] array, int arrayIndex, int countLimit)
        {
            ConvertedCopyTo(source.Take(countLimit), conversion, array, arrayIndex);
        }

        public static List<TOutput> ConvertFiltered<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter, Predicate<T> filter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return (from item in source where filter(item) select converter(item)).ToList();
        }

        public static IEnumerable<TOutput> ConvertProgressive<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            return ConvertProgressiveExtracted();

            IEnumerable<TOutput> ConvertProgressiveExtracted()
            {
                foreach (var item in source)
                {
                    yield return converter(item);
                }
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array)
        {
            CopyTo(source.Skip(sourceIndex), array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array, int arrayIndex)
        {
            CopyTo(source.Skip(sourceIndex), array, arrayIndex);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, int sourceIndex, T[] array, int arrayIndex, int countLimit)
        {
            CopyTo(source.Skip(sourceIndex).Take(countLimit), array, arrayIndex);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array)
        {
            CopyTo(source, array, 0);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            try
            {
                var index = arrayIndex;
                foreach (var item in source)
                {
                    array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException(exception.Message, nameof(array));
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void CopyTo<T>(this IEnumerable<T> source, T[] array, int arrayIndex, int countLimit)
        {
            CopyTo(source.Take(countLimit), array, arrayIndex);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            var index = 0;
            foreach (var item in source)
            {
                array.SetValue(item, index++);
            }
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static void DeprecatedCopyTo<T>(this IEnumerable<T> source, Array array, int index)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            foreach (var item in source)
            {
                array.SetValue(item, index++);
            }
        }

        public static bool Dequeue<T>(this Queue<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            if (!comparer.Equals(item, source.Peek()))
            {
                return false;
            }

            source.Dequeue();
            return true;
        }

        public static int ExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var count = 0;
            foreach (var item in other)
            {
                while (source.Remove(item))
                {
                    count++;
                }
            }

            return count;
        }

        public static IEnumerable<T> ExceptWithEnumerable<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return ExceptWithEnumerableExtracted();

            IEnumerable<T> ExceptWithEnumerableExtracted()
            {
                foreach (var item in other)
                {
                    while (source.Remove(item))
                    {
                        yield return item;
                    }
                }
            }
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return FlattenExtracted();

            IEnumerable<T> FlattenExtracted()
            {
                foreach (var key in source)
                {
                    foreach (var item in key)
                    {
                        yield return item;
                    }
                }
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var currentIndex = 0;
            var comparer = EqualityComparer<T>.Default;
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }

                    currentIndex++;
                }

                return -1;
            }
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var currentIndex = 0;
            return comparer == null
                ? IndexOfExtracted(source, item, EqualityComparer<T>.Default, ref currentIndex)
                : IndexOfExtracted(source, item, comparer, ref currentIndex);
        }

        public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var otherAsCollection = AsICollection(other);
            return source.RemoveWhere(input => !otherAsCollection.Contains(input));
        }

        public static int IntersectWith<T>(this ICollection<T> source, IEnumerable<T> other, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            comparer ??= EqualityComparer<T>.Default;
            var otherAsCollection = AsICollection(other);
            return source.RemoveWhere(input => !otherAsCollection.Contains(input, comparer));
        }

        public static bool IsProperSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSubsetOf(source, other, true);
        }

        public static bool IsProperSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSupersetOf(source, other, true);
        }

        public static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSubsetOf(source, other, false);
        }

        public static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return IsSupersetOf(source, other, false);
        }

        public static bool ListEquals<T>(this IList<T> first, IList<T> second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            var count = first.Count;
            if (count != second.Count)
            {
                return false;
            }

            var cmp = EqualityComparer<T>.Default;
            for (var i = 0; i != count; ++i)
            {
                if (!cmp.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex)
            {
                newIndex--;
            }

            list.Insert(newIndex, item);
        }

        public static bool Overlaps<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            return ContainsAny(source, items);
        }

        public static bool Pop<T>(this Stack<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            if (!comparer.Equals(item, source.Peek()))
            {
                return false;
            }

            source.Pop();
            return true;
        }

        public static bool ReadOnlyListEquals<T>(this IReadOnlyList<T> first, IReadOnlyList<T> second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            var count = first.Count;
            if (count != second.Count)
            {
                return false;
            }

            var cmp = EqualityComparer<T>.Default;
            for (var i = 0; i != count; ++i)
            {
                if (!cmp.Equals(first[i], second[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Remove<T>(this ICollection<T> source, T item, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }

            return source.RemoveWhereEnumerable(input => comparer.Equals(input, item)).Any();
        }

        public static T[] RemoveFirst<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var result = new T[array.Length - 1];
            Array.Copy(array, 1, result, 0, result.Length);
            return result;
        }

        public static T[] RemoveLast<T>(this T[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            // Copyright (c) Microsoft. All rights reserved.
            // Licensed under the MIT license. See LICENSE file in the project root for full license information.
            var result = new T[array.Length - 1];
            Array.Copy(array, 0, result, 0, result.Length);
            return result;
        }

        public static int RemoveWhere<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            return ExceptWith
            (
                source,
                new List<T>(converter.Invoke(source))
            );
        }

        public static int RemoveWhere<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return RemoveWhere(source, items => items.Where(predicate));
        }

        public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Func<IEnumerable<T>, IEnumerable<T>> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            return ExceptWithEnumerable
            (
                source,
                new List<T>(converter.Invoke(source))
            );
        }

        public static IEnumerable<T> RemoveWhereEnumerable<T>(this ICollection<T> source, Func<T, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return RemoveWhereEnumerable(source, items => items.Where(predicate));
        }

        public static bool SetEquals<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            var thatAsCollection = AsICollection(other);
            return thatAsCollection.All(source.Contains) && source.All(input => thatAsCollection.Contains(input));
        }

        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            if (list == null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (indexA < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexA), "Non-negative number is required.");
            }

            if (indexB < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(indexB), "Non-negative number is required.");
            }

            var listCount = list.Count;
            if (indexA >= listCount || indexB >= listCount)
            {
                throw new ArgumentException("The list does not contain the number of elements.", nameof(list));
            }

            if (indexA != indexB)
            {
                SwapExtracted(list, indexA, indexB);
            }
        }

        public static int SymmetricExceptWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRange(other.Distinct().Where(input => !source.Remove(input)));
        }

        public static bool TryTake<T>(this Stack<T> stack, out T item)
        {
            if (stack == null)
            {
                throw new ArgumentNullException(nameof(stack));
            }

            try
            {
                item = stack.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default!;
                return false;
            }
        }

        public static int UnionWith<T>(this ICollection<T> source, IEnumerable<T> other)
        {
            return source.AddRange(other.Where(input => !source.Contains(input)));
        }

        private static int IndexOfExtracted<T>(IEnumerable<T> source, T item, IEqualityComparer<T> comparer, ref int currentIndex)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }

                    currentIndex++;
                }

                return -1;
            }
        }

        private static bool IsSubsetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = AsDistinctICollection(source);
            var that = AsDistinctICollection(other);
            var elementCount = 0;
            var matchCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (@this.Contains(item))
                {
                    matchCount++;
                }
            }

            if (proper)
            {
                return matchCount == @this.Count && elementCount > @this.Count;
            }

            return matchCount == @this.Count;
        }

        private static bool IsSupersetOf<T>(this IEnumerable<T> source, IEnumerable<T> other, bool proper)
        {
            var @this = AsDistinctICollection(source);
            var that = AsDistinctICollection(other);
            var elementCount = 0;
            foreach (var item in that)
            {
                elementCount++;
                if (!@this.Contains(item))
                {
                    return false;
                }
            }

            if (proper)
            {
                return elementCount < @this.Count;
            }

            return true;
        }

        private static void SwapExtracted<T>(IList<T> list, int indexA, int indexB)
        {
            var tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
    }

    public static partial class Extensions
    {
        public static List<TOutput> ConvertAll<T, TOutput>(this IEnumerable<T> source, Func<T, TOutput> converter)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            return source.Select(converter).ToList();
        }

        public static TList ConvertAll<T, TOutput, TList>(this IEnumerable<T> source, Func<T, TOutput> converter)
            where TList : ICollection<TOutput>, new()
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var result = new TList();
            foreach (var item in source)
            {
                result.Add(converter(item));
            }

            return result;
        }

        public static int CountContiguousItems<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = 0;
            var equalityComparer = EqualityComparer<T>.Default;
            foreach (var value in source)
            {
                if (equalityComparer.Equals(value, item))
                {
                    result++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static int CountContiguousItemsWhere<T>(this IEnumerable<T> source, Predicate<T> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var result = 0;
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    result++;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static int CountItems<T>(this IEnumerable<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var equalityComparer = EqualityComparer<T>.Default;
            return source.Count(value => equalityComparer.Equals(value, item));
        }

        public static int CountItemsWhere<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return source.Count(predicate);
        }
    }

    public static partial class Extensions
    {
#if NET35
        public static bool Contains<T>(this IEnumerable<T> source, IEnumerable<T> items)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var localComparer = EqualityComparer<T>.Default;
            var localCollection = AsICollection(source);
            return items.All(item => localCollection.Contains(item, localComparer));
        }

        public static bool Contains<T>(this IEnumerable<T> source, IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var localComparer = comparer ?? EqualityComparer<T>.Default;
            var localCollection = AsICollection(source);
            return items.All(item => localCollection.Contains(item, localComparer));
        }

#endif

#if TARGETS_NET || LESSTHAN_NETCOREAPP20 || TARGETS_NETSTANDARD

        public static bool TryDequeue<T>(this Queue<T> source, out T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                item = source.Dequeue();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default!;
                return false;
            }
        }

        public static bool TryPeek<T>(this Stack<T> source, out T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                item = source.Peek();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default!;
                return false;
            }
        }

        public static bool TryPeek<T>(this Queue<T> source, out T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                item = source.Peek();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default!;
                return false;
            }
        }

        public static bool TryPop<T>(this Stack<T> source, out T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                item = source.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                item = default!;
                return false;
            }
        }

#endif
    }
}