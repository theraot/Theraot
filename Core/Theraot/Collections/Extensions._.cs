using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Theraot.Collections.Specialized;
using Theraot.Core;

namespace Theraot.Collections
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    public static partial class Extensions
    {
        public static void CanCopyTo(int count, Array array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (count > array.Length)
            {
                throw new ArgumentException("array", "The array can not contain the number of elements.");
            }
        }

        public static void CanCopyTo(int count, Array array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("array", "The array can not contain the number of elements.");
            }
        }

        public static void CanCopyTo<TItem>(int count, TItem[] array)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (count > array.Length)
            {
                throw new ArgumentException("array", "The array can not contain the number of elements.");
            }
        }

        public static void CanCopyTo<TItem>(int count, TItem[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (count > array.Length - arrayIndex)
            {
                throw new ArgumentException("array", "The array can not contain the number of elements.");
            }
        }

        public static void CanCopyTo<TItem>(TItem[] array, int arrayIndex, int countLimit)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", "Non-negative number is required.");
            }
            if (countLimit < 0)
            {
                throw new ArgumentOutOfRangeException("countLimit", "Non-negative number is required.");
            }
            if (countLimit > array.Length - arrayIndex)
            {
                throw new ArgumentException("array", "The array can not contain the number of elements.");
            }
        }

        public static IEnumerable<TItem> Clone<TItem>(this IEnumerable<TItem> target)
        {
            return new List<TItem>(target);
        }

        public static bool Contains<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> items)
        {
            var comparer = EqualityComparer<TItem>.Default;
            var _collection = new ProgressiveSet<TItem>(Check.NotNullArgument(collection, "collection"));
            foreach (TItem item in Check.NotNullArgument(items, "items"))
            {
                if (!_collection.Contains(item, comparer))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Contains<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> items, IEqualityComparer<TItem> comparer)
        {
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            var _collection = new ProgressiveSet<TItem>(Check.NotNullArgument(collection, "collection"));
            foreach (TItem item in Check.NotNullArgument(items, "items"))
            {
                if (!_collection.Contains(item, _comparer))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool ContainsAny<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> items)
        {
            IEqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;
            var _collection = new ProgressiveSet<TItem>(Check.NotNullArgument(collection, "collection"));
            foreach (TItem item in Check.NotNullArgument(items, "items"))
            {
                if (_collection.Contains(item, comparer))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsAny<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> items, IEqualityComparer<TItem> comparer)
        {
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            var _collection = new ProgressiveSet<TItem>(Check.NotNullArgument(collection, "collection"));
            foreach (TItem item in Check.NotNullArgument(items, "items"))
            {
                if (_collection.Contains(item, _comparer))
                {
                    return true;
                }
            }
            return false;
        }

        public static TOutput Convert<TOutput, TInput>(this TInput item, Converter<TInput, TOutput> converter)
        {
            return Check.NotNullArgument(converter, "converter")(item);
        }

        public static List<TOutput> ConvertAll<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var result = new List<TOutput>();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                result.Add(_converter(item));
            }
            return result;
        }

        public static TList ConvertAll<TItem, TOutput, TList>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter)
            where TList : ICollection<TOutput>, new()
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var result = new TList();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                result.Add(_converter(item));
            }
            return result;
        }

        public static List<TOutput> ConvertFiltered<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            var result = new List<TOutput>();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    result.Add(_converter(item));
                }
            }
            return result;
        }

        public static List<TOutput> ConvertFiltered<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new List<TOutput>();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    result.Add(_converter(item));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertFiltered<TItem, TOutput, TList>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
            where TList : ICollection<TOutput>, new()
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            var result = new TList();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    result.Add(_converter(item));
                }
            }
            return result;
        }

        public static TList ConvertFiltered<TItem, TOutput, TList>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
            where TList : ICollection<TOutput>, new()
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new TList();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    result.Add(_converter(item));
                }
                index++;
            }
            return result;
        }

        public static List<KeyValuePair<int, TOutput>> ConvertIndexed<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new List<KeyValuePair<int, TOutput>>();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, _converter(item)));
                }
                index++;
            }
            return result;
        }

        public static List<KeyValuePair<int, TOutput>> ConvertIndexed<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new List<KeyValuePair<int, TOutput>>();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, _converter(item)));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertIndexed<TItem, TOutput, TList>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
            where TList : ICollection<KeyValuePair<int, TOutput>>, new()
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new TList();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, _converter(item)));
                }
                index++;
            }
            return result;
        }

        public static TList ConvertIndexed<TItem, TOutput, TList>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
            where TList : ICollection<KeyValuePair<int, TOutput>>, new()
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            var result = new TList();
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    result.Add(new KeyValuePair<int, TOutput>(index, _converter(item)));
                }
                index++;
            }
            return result;
        }

        public static IEnumerable<TOutput> ConvertProgressive<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                yield return _converter(item);
            }
        }

        public static IEnumerable<TOutput> ConvertProgressiveFiltered<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    yield return _converter(item);
                }
            }
        }

        public static IEnumerable<TOutput> ConvertProgressiveFiltered<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    yield return _converter(item);
                }
                index++;
            }
        }

        public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Predicate<TItem> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item))
                {
                    yield return new KeyValuePair<int, TOutput>(index, _converter(item));
                }
                index++;
            }
        }

        public static IEnumerable<KeyValuePair<int, TOutput>> ConvertProgressiveIndexed<TItem, TOutput>(this IEnumerable<TItem> collection, Converter<TItem, TOutput> converter, Func<TItem, int, bool> filter)
        {
            var _converter = Check.NotNullArgument(converter, "converter");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter(item, index))
                {
                    yield return new KeyValuePair<int, TOutput>(index, _converter(item));
                }
                index++;
            }
        }

        public static void CopyTo<TItem>(this IEnumerable<TItem> collection, TItem[] array)
        {
            CopyTo(collection, array, 0);
        }

        public static void CopyTo<TItem>(this IEnumerable<TItem> collection, TItem[] array, int arrayIndex)
        {
            var _array = Check.NotNullArgument(array, "array");
            try
            {
                int index = arrayIndex;
                foreach (var item in Check.NotNullArgument(collection, "collection"))
                {
                    _array[index] = item;
                    index++;
                }
            }
            catch (IndexOutOfRangeException exception)
            {
                throw new ArgumentException("array", exception.Message);
            }
        }

        public static void CopyTo<TItem>(this IEnumerable<TItem> collection, TItem[] array, int arrayIndex, int countLimit)
        {
            CopyTo(collection.TakeItems(countLimit), array, arrayIndex);
        }

        public static int CountContiguousItems<TItem>(this IEnumerable<TItem> collection, TItem item)
        {
            int result = 0;
            foreach (TItem value in Check.NotNullArgument(collection, "collection"))
            {
                if (Comparer.Equals(value, item))
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

        public static int CountContiguousItemsWhere<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int result = 0;
            Predicate<TItem> _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate(item))
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

        public static int CountItems<TItem>(this IEnumerable<TItem> collection)
        {
            int result = 0;
            foreach (TItem value in Check.NotNullArgument(collection, "collection"))
            {
                result++;
            }
            return result;
        }

        public static int CountItems<TItem>(this IEnumerable<TItem> collection, TItem item)
        {
            int result = 0;
            foreach (TItem value in Check.NotNullArgument(collection, "collection"))
            {
                if (Comparer.Equals(value, item))
                {
                    result++;
                }
            }
            return result;
        }

        public static int CountItemsWhere<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int result = 0;
            Predicate<TItem> _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate(item))
                {
                    result++;
                }
            }
            return result;
        }

        public static void DeprecatedCopyTo<T>(this IEnumerable<T> collection, Array array)
        {
            int index = 0;
            foreach (var item in collection)
            {
                array.SetValue(item, index++);
            }
        }

        public static void DeprecatedCopyTo<T>(this IEnumerable<T> collection, Array array, int index)
        {
            foreach (var item in collection)
            {
                array.SetValue(item, index++);
            }
        }

        public static int ExceptWith<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            int count = 0;
            var _collection = Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(other, "other"))
            {
                while (_collection.Remove(item))
                {
                    count++;
                }
            }
            return count;
        }

        public static IEnumerable<TItem> ExceptWithEnumerable<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            foreach (var item in Check.NotNullArgument(other, "other"))
            {
                while (_collection.Remove(item))
                {
                    yield return item;
                }
            }
        }

        public static bool Exists<TItem>(this IEnumerable<TItem> collection, TItem value)
        {
            IEqualityComparer<TItem> comparer = EqualityComparer<TItem>.Default;
            foreach (TItem local in Check.NotNullArgument(collection, "collection"))
            {
                if (comparer.Equals(local, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists<TItem>(this IEnumerable<TItem> collection, TItem value, IEqualityComparer<TItem> comparer)
        {
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            foreach (TItem local in Check.NotNullArgument(collection, "collection"))
            {
                if (_comparer.Equals(local, value))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Exists<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            Predicate<TItem> _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate(item))
                {
                    return true;
                }
            }
            return false;
        }

        public static TItem Find<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                    currentIndex++;
                }
                return default(TItem);
            }
        }

        public static TItem Find<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                    currentIndex++;
                }
                return default(TItem);
            }
        }

        public static TItem Find<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        return enumerator.Current;
                    }
                    currentIndex++;
                }
                return default(TItem);
            }
        }

        public static int FindIndex<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int FindIndex<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int FindIndex<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static TItem FindLast<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            TItem result = default(TItem);
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static TItem FindLast<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            TItem result = default(TItem);
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static TItem FindLast<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            TItem result = default(TItem);
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        result = enumerator.Current;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int FindLastIndex<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int FindLastIndex<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int FindLastIndex<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static List<TItem> FindWhere<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            var result = new List<TItem>();
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static TList FindWhere<TItem, TList>(this IEnumerable<TItem> collection, Predicate<TItem> predicate)
            where TList : ICollection<TItem>, new()
        {
            var result = new TList();
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static void For<TItem>(this IEnumerable<TItem> collection, Action<int, TItem> action)
        {
            var _action = Check.NotNullArgument(action, "action");
            var _collection = Check.NotNullArgument(collection, "collection");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                _action.Invoke(index, item);
                index++;
            }
        }

        public static void For<TItem>(this IEnumerable<TItem> collection, Action<int, TItem> action, Predicate<TItem> predicate)
        {
            var _action = Check.NotNullArgument(action, "action");
            var _collection = Check.NotNullArgument(collection, "collection");
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate.Invoke(item))
                {
                    _action.Invoke(index, item);
                }
                index++;
            }
        }

        public static void For<TItem>(this IEnumerable<TItem> collection, Action<int, TItem> action, Func<TItem, int, bool> filter)
        {
            var _action = Check.NotNullArgument(action, "action");
            var _collection = Check.NotNullArgument(collection, "collection");
            var _filter = Check.NotNullArgument(filter, "filter");
            int index = 0;
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_filter.Invoke(item, index))
                {
                    _action.Invoke(index, item);
                }
                index++;
            }
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> collection, Action<TItem> action)
        {
            var _action = Check.NotNullArgument(action, "action");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                _action.Invoke(item);
            }
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> collection, Action<TItem> action, Predicate<TItem> predicate)
        {
            var _action = Check.NotNullArgument(action, "action");
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foreach (TItem item in Check.NotNullArgument(collection, "collection"))
            {
                if (_predicate.Invoke(item))
                {
                    _action.Invoke(item);
                }
            }
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        {
            TValue value;
            var _dictionary = Check.NotNullArgument(dictionary, "dictionary");
            if (_dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                TValue newValue = TypeHelper.CreateOrDefault<TValue>();
                _dictionary.Add(key, newValue);
                return newValue;
            }
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue newValue)
        {
            TValue value;
            var _dictionary = Check.NotNullArgument(dictionary, "dictionary");
            if (_dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                _dictionary.Add(key, newValue);
                return newValue;
            }
        }

        public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> create)
        {
            TValue value;
            var _dictionary = Check.NotNullArgument(dictionary, "dictionary");
            if (_dictionary.TryGetValue(key, out value))
            {
                return value;
            }
            else
            {
                TValue newValue = create.SafeInvoke(default(TValue));
                _dictionary.Add(key, newValue);
                return newValue;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, int count)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _comparer = EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, int count, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index)
        {
            int currentIndex = 0;
            var _comparer = EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item)
        {
            int currentIndex = 0;
            var _comparer = EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static int IndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        return currentIndex;
                    }
                    currentIndex++;
                }
                return -1;
            }
        }

        public static IEnumerable<T> InterleaveMany<T>(this IEnumerable<IEnumerable<T>> source)
        {
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                var ok = true;
                while (ok)
                {
                    ok = false;
                    foreach (var enumerator in enumerators)
                    {
                        if (enumerator.MoveNext())
                        {
                            yield return enumerator.Current;
                            ok = true;
                        }
                    }
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    enumerator.Dispose();
                }
            }
        }

        public static int IntersectWith<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            var _other = new ProgressiveSet<TItem>(other);
            return collection.RemoveWhere
                   (
                       (TItem input) =>
                       {
                           return !_other.Contains(input);
                       }
                   );
        }

        public static IEnumerable<TItem> IntersectWithEnumerable<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            var _other = new ProgressiveSet<TItem>(other);
            return collection.RemoveWhereEnumerable
                   (
                       (TItem input) =>
                       {
                           return !_other.Contains(input);
                       }
                   );
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !source.Any();
        }

        public static bool IsProperSubsetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other)
        {
            return IsSubsetOf(collection, other, true);
        }

        public static bool IsProperSupersetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other)
        {
            return IsSupersetOf(collection, other, true);
        }

        public static bool IsSubsetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other)
        {
            return IsSubsetOf(collection, other, false);
        }

        public static bool IsSupersetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other)
        {
            return IsSupersetOf(collection, other, false);
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index)
        {
            int currentIndex = 0;
            var _comparer = EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item)
        {
            int currentIndex = 0;
            var _comparer = EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, int count, IEqualityComparer<TItem> comparer)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _comparer = comparer ?? EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static int LastIndexOf<TItem>(this IEnumerable<TItem> collection, TItem item, int index, int count)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _comparer = EqualityComparer<TItem>.Default;
            int result = -1;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_comparer.Equals(enumerator.Current, item))
                    {
                        result = currentIndex;
                    }
                    currentIndex++;
                }
                return result;
            }
        }

        public static void Move<T>(this IList<T> list, int oldIndex, int newIndex)
        {
            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            if (newIndex > oldIndex)
            {
                newIndex--;
            }
            list.Insert(newIndex, item);
        }

        public static bool Overlaps<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> items)
        {
            return ContainsAny(collection, items);
        }

        public static IEnumerable<TPackage> Pack<T, TPackage>(this IEnumerable<T> source, int size)
            where TPackage : ICollection<T>, new()
        {
            int count = 0;
            var currentPackage = new TPackage();
            foreach (var item in source)
            {
                currentPackage.Add(item);
                count++;
                if (count == size)
                {
                    yield return currentPackage;
                    currentPackage = new TPackage();
                    count = 0;
                }
            }
            if (count > 0)
            {
                yield return currentPackage;
            }
        }

        public static IEnumerable<T[]> Pack<T>(this IEnumerable<T> source, int size)
        {
            int index = 0;
            var currentPackage = new T[size];
            foreach (var item in source)
            {
                currentPackage[index] = item;
                index++;
                if (index == size)
                {
                    yield return currentPackage;
                    currentPackage = new T[size];
                    index = 0;
                }
            }
            if (index > 0)
            {
                Array.Resize(ref currentPackage, index);
                yield return currentPackage;
            }
        }

        public static bool Remove<TItem>(this ICollection<TItem> collection, TItem item, IEqualityComparer<TItem> comparer)
        {
            var _comparer = Check.NotNullArgument(comparer, "comparer");
            using (var enumerator = Check.NotNullArgument(collection, "collection").RemoveWhereEnumerable(input => _comparer.Equals(input, item)).GetEnumerator())
            {
                return enumerator.MoveNext();
            }
        }

        public static int RemoveWhere<TItem>(this ICollection<TItem> collection, Predicate<TItem> predicate)
        {
            return RemoveWhere(collection, items => Where(items, Check.NotNullArgument(predicate, "predicate")));
        }

        public static int RemoveWhere<TItem>(this ICollection<TItem> collection, Converter<IEnumerable<TItem>, IEnumerable<TItem>> converter)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            return ExceptWith
                   (
                       _collection,
                       Enumerable.ToList(Check.NotNullArgument(converter, "converter").Invoke(_collection))
                   );
        }

        public static IEnumerable<TItem> RemoveWhereEnumerable<TItem>(this ICollection<TItem> collection, Predicate<TItem> predicate)
        {
            return RemoveWhereEnumerable(collection, items => Where(items, Check.NotNullArgument(predicate, "predicate")));
        }

        public static IEnumerable<TItem> RemoveWhereEnumerable<TItem>(this ICollection<TItem> collection, Converter<IEnumerable<TItem>, IEnumerable<TItem>> converter)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            return ExceptWithEnumerable
                   (
                       _collection,
                       Enumerable.ToList(Check.NotNullArgument(converter, "converter").Invoke(_collection))
                   );
        }

        public static void Reverse<T>(this IList<T> list, int index, int count)
        {
            var _list = Check.NotNullArgument(list, "list");
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
            }
            int Count = list.Count;
            if (count > Count - index)
            {
                throw new ArgumentException("list", "The list does not contain the number of elements.");
            }
            int end = index + count;
            for (; index < end; index++, end++)
            {
                SwapExtracted<T>(list, index, end);
            }
        }

        public static bool SetEquals<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            var _other = Check.NotNullArgument(other, "other");
            var _that = new ProgressiveSet<TItem>(_other);
            foreach (var item in _that.Where(input => !_collection.Contains(input)))
            {
                return false;
            }
            foreach (var item in _collection.Where(input => !_that.Contains(input)))
            {
                return false;
            }
            return true;
        }

        public static void Sort<T>(this IList<T> list, int index, int count, IComparer<T> comparer)
        {
            var _list = Check.NotNullArgument(list, "list");
            var _comparer = comparer ?? Comparer<T>.Default;
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", "Non-negative number is required.");
            }
            else if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Non-negative number is required.");
            }
            else
            {
                int Count = list.Count;
                if (count > Count - index)
                {
                    throw new ArgumentException("list", "The list does not contain the number of elements.");
                }
                SortExtracted<T>(_list, index, count + index, _comparer);
            }
        }

        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            var _list = Check.NotNullArgument(list, "list");
            if (indexA < 0)
            {
                throw new ArgumentOutOfRangeException("indexA", "Non-negative number is required.");
            }
            if (indexB < 0)
            {
                throw new ArgumentOutOfRangeException("indexB", "Non-negative number is required.");
            }
            int Count = list.Count;
            if (indexA >= Count || indexB >= Count)
            {
                throw new ArgumentException("list", "The list does not contain the number of elements.");
            }
            if (indexA != indexB)
            {
                SwapExtracted<T>(_list, indexA, indexB);
            }
        }

        public static int SymmetricExceptWith<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            return collection.AddRange(Extensions.Where(other, (TItem input) => !collection.Remove(input)));
        }

        public static IEnumerable<TItem> SymmetricExceptWithEnumerable<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            return collection.AddRangeEnumerable(Extensions.Where(other, (TItem input) => !collection.Remove(input)));
        }

        public static TItem TakeAndReturn<TItem>(this IDropPoint<TItem> dropPoint)
        {
            TItem item;
            if (Check.NotNullArgument(dropPoint, "dropPoint").TryTake(out item))
            {
                return item;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public static TItem[] ToArray<TItem>(this ICollection<TItem> collection)
        {
            Check.NotNullArgument(collection, "collection");
            return (new List<TItem>(collection)).ToArray();
        }

        public static ReadOnlyCollection<TSource> ToReadOnlyCollection<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                return new ReadOnlyCollection<TSource>(EmptyList<TSource>.Instance);
            }
            else
            {
                var sourceAsReadOnlyCollection = source as ReadOnlyCollection<TSource>;
                if (sourceAsReadOnlyCollection != null)
                {
                    return sourceAsReadOnlyCollection;
                }
                else
                {
                    return new ReadOnlyCollection<TSource>(source.ToArray<TSource>());
                }
            }
        }

        public static bool TryFind<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate, out TItem fountItem)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        fountItem = enumerator.Current;
                        return true;
                    }
                    currentIndex++;
                }
                fountItem = default(TItem);
                return false;
            }
        }

        public static bool TryFind<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate, out TItem fountItem)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        fountItem = enumerator.Current;
                        return true;
                    }
                    currentIndex++;
                }
                fountItem = default(TItem);
                return false;
            }
        }

        public static bool TryFind<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate, out TItem fountItem)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        fountItem = enumerator.Current;
                        return true;
                    }
                    currentIndex++;
                }
                fountItem = default(TItem);
                return false;
            }
        }

        public static bool TryFindLast<TItem>(this IEnumerable<TItem> collection, int index, int count, Predicate<TItem> predicate, out TItem foundItem)
        {
            int currentIndex = 0;
            int limit = index + count;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foundItem = default(TItem);
            bool found = false;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (currentIndex == limit)
                    {
                        break;
                    }
                    if (_predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                    currentIndex++;
                }
                return found;
            }
        }

        public static bool TryFindLast<TItem>(this IEnumerable<TItem> collection, int index, Predicate<TItem> predicate, out TItem foundItem)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foundItem = default(TItem);
            bool found = false;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    currentIndex++;
                    if (currentIndex == index)
                    {
                        break;
                    }
                }
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                    currentIndex++;
                }
                return found;
            }
        }

        public static bool TryFindLast<TItem>(this IEnumerable<TItem> collection, Predicate<TItem> predicate, out TItem foundItem)
        {
            int currentIndex = 0;
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            foundItem = default(TItem);
            bool found = false;
            using (var enumerator = Check.NotNullArgument(collection, "collection").GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (_predicate(enumerator.Current))
                    {
                        foundItem = enumerator.Current;
                        found = true;
                    }
                    currentIndex++;
                }
                return found;
            }
        }

        public static bool TryTakeAndIgnore<TItem>(this IDropPoint<TItem> dropPoint)
        {
            TItem item;
            return Check.NotNullArgument(dropPoint, "dropPoint").TryTake(out item);
        }

        public static bool TryTakeUntil<TItem>(this IDropPoint<TItem> dropPoint, Predicate<TItem> check, out TItem item)
        {
            var _check = Check.NotNullArgument(check, "check");
            var _dropPoint = Check.NotNullArgument(dropPoint, "dropPoint");
        back:
            if (_dropPoint.TryTake(out item))
            {
                if (_check(item))
                {
                    return true;
                }
                else
                {
                    goto back;
                }
            }
            else
            {
                return false;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By Design")]
        public static bool TryTakeUntil<TItem>(this IDropPoint<TItem> dropPoint, Predicate<TItem> check, out TItem item, ICollection<TItem> trail)
        {
            var _check = Check.NotNullArgument(check, "check");
            var _dropPoint = Check.NotNullArgument(dropPoint, "dropPoint");
            var _trail = Check.NotNullArgument(trail, "trail");
        back:
            if (_dropPoint.TryTake(out item))
            {
                if (_check(item))
                {
                    return true;
                }
                else
                {
                    _trail.Add(item);
                    goto back;
                }
            }
            else
            {
                return false;
            }
        }

        public static int UnionWith<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            return Extensions.AddRange(collection, other.Where(input => !collection.Contains(input)));
        }

        public static IEnumerable<TItem> UnionWithEnumerable<TItem>(this ICollection<TItem> collection, IEnumerable<TItem> other)
        {
            return Extensions.AddRangeEnumerable(collection, other.Where(input => !collection.Contains(input)));
        }

        public static void Waste<T>(this IEnumerable<T> source)
        {
            foreach (T element in Check.NotNullArgument(source, "source"))
            {
                //Empty
            }
        }

        public static IEnumerable<TItem> Where<TItem>(IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(collection, "collection");
            return WhereExtracted(_collection, _predicate);
        }

        public static IEnumerable<TItem> Where<TItem>(this IEnumerable<TItem> collection, Func<TItem, int, bool> predicate)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(collection, "collection");
            return WhereExtracted(_collection, _predicate);
        }

        public static IEnumerable<TItem> Where<TItem>(this IEnumerable<TItem> source, Func<TItem, bool> predicate, Action whereNot)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(source, "source");
            if (whereNot == null)
            {
                return WhereExtracted(_collection, _predicate);
            }
            else
            {
                return WhereExtracted(_collection, _predicate, whereNot);
            }
        }

        public static IEnumerable<TItem> Where<TItem>(this IEnumerable<TItem> source, Func<TItem, bool> predicate, Action<TItem> whereNot)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(source, "source");
            if (whereNot == null)
            {
                return WhereExtracted(_collection, _predicate);
            }
            else
            {
                return WhereExtracted(_collection, _predicate, whereNot);
            }
        }

        public static IEnumerable<TItem> Where<TItem>(this IEnumerable<TItem> source, Func<TItem, int, bool> predicate, Action whereNot)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(source, "source");
            if (whereNot == null)
            {
                return WhereExtracted(_collection, _predicate);
            }
            else
            {
                return WhereExtracted(_collection, _predicate, whereNot);
            }
        }

        public static IEnumerable<TItem> Where<TItem>(this IEnumerable<TItem> source, Func<TItem, int, bool> predicate, Action<TItem> whereNot)
        {
            var _predicate = Check.NotNullArgument(predicate, "predicate");
            var _collection = Check.NotNullArgument(source, "source");
            if (whereNot == null)
            {
                return WhereExtracted(_collection, _predicate);
            }
            else
            {
                return WhereExtracted(_collection, _predicate, whereNot);
            }
        }

        public static IEnumerable<T> WhereType<T>(IEnumerable enumerable)
        {
            return new EnumerableFromDelegate<T>(enumerable.GetEnumerator);
        }

        public static IEnumerable<TResult> ZipMany<T, TResult>(this IEnumerable<IEnumerable<T>> source, Func<IEnumerable<T>, TResult> func)
        {
            var enumerators = source.Select(x => x.GetEnumerator()).ToArray();
            try
            {
                while (enumerators.All(enumerator => enumerator.MoveNext()))
                {
                    yield return func(enumerators.Select(enumerator => enumerator.Current));
                }
            }
            finally
            {
                foreach (var enumerator in enumerators)
                {
                    enumerator.Dispose();
                }
            }
        }

        private static bool IsSubsetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other, bool proper)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            var _other = Check.NotNullArgument(other, "other");
            var _this = AsSet<TItem>(_collection);
            var _that = AsSet<TItem>(_other);
            int elementCount = 0;
            int matchCount = 0;
            foreach (var item in _that)
            {
                elementCount++;
                if (_collection.Contains(item))
                {
                    matchCount++;
                }
            }
            if (proper)
            {
                return matchCount == _this.Count && elementCount > _this.Count;
            }
            else
            {
                return matchCount == _this.Count;
            }
        }

        private static bool IsSupersetOf<TItem>(this IEnumerable<TItem> collection, IEnumerable<TItem> other, bool proper)
        {
            var _collection = Check.NotNullArgument(collection, "collection");
            var _other = Check.NotNullArgument(other, "other");
            var _this = AsSet<TItem>(_collection);
            var _that = AsSet<TItem>(_other);
            int elementCount = 0;
            foreach (var item in other)
            {
                elementCount++;
                if (!_this.Contains(item))
                {
                    return false;
                }
            }
            if (proper)
            {
                return elementCount < _this.Count;
            }
            else
            {
                return true;
            }
        }

        private static void SortExtracted<T>(IList<T> list, int indexStart, int indexEnd, IComparer<T> comparer)
        {
            int low = indexStart;
            int high = indexEnd;
            T pivot = list[low + ((high - low) / 2)];
            while (low <= high)
            {
                while (low < indexEnd && comparer.Compare(list[low], pivot) < 0)
                {
                    low++;
                }
                while (high > indexStart && comparer.Compare(pivot, list[high]) < 0)
                {
                    high--;
                }
                if (low == high)
                {
                    low++;
                    high--;
                }
                else if (low < high)
                {
                    SwapExtracted(list, low, high);
                    low++;
                    high--;
                }
            }
            if (indexStart < high)
            {
                SortExtracted<T>(list, indexStart, high, comparer);
            }
            if (low < indexEnd)
            {
                SortExtracted<T>(list, low, indexEnd, comparer);
            }
        }

        private static void SwapExtracted<T>(IList<T> list, int indexA, int indexB)
        {
            T itemA = list[indexA];
            T itemB = list[indexB];
            list[indexA] = itemB;
            list[indexB] = itemA;
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> collection, Predicate<TItem> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> collection, Func<TItem, int, bool> predicate)
        {
            int index = 0;
            foreach (var item in collection)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                index++;
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> source, Func<TItem, bool> predicate, Action whereNot)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    whereNot();
                }
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> source, Func<TItem, bool> predicate)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> source, Func<TItem, int, bool> predicate, Action whereNot)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                else
                {
                    whereNot();
                }
                index++;
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> source, Func<TItem, bool> predicate, Action<TItem> whereNot)
        {
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return item;
                }
                else
                {
                    whereNot(item);
                }
            }
        }

        private static IEnumerable<TItem> WhereExtracted<TItem>(IEnumerable<TItem> source, Func<TItem, int, bool> predicate, Action<TItem> whereNot)
        {
            int index = 0;
            foreach (var item in source)
            {
                if (predicate(item, index))
                {
                    yield return item;
                }
                else
                {
                    whereNot(item);
                }
                index++;
            }
        }
    }
}