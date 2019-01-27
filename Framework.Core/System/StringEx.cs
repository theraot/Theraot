#pragma warning disable RECS0017 // Possible compare of value type with 'null'

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace System
{
    public static partial class StringEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Concat(params string[] value)
        {
            return string.Concat(value);
        }

        public static string Concat(string[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            return arrayIndex == array.Length ? string.Empty : ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Concat(string[] array, int arrayIndex, int countLimit)
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
                throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", nameof(array));
            }

            return arrayIndex == array.Length ? string.Empty : ConcatExtracted(array, arrayIndex, countLimit);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Concat(params object[] values)
        {
            return string.Concat(values);
        }

        public static string Concat(object[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            return arrayIndex == array.Length ? string.Empty : ConcatExtracted(array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Concat(object[] array, int arrayIndex, int countLimit)
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
                throw new ArgumentException("startIndex plus countLimit is greater than the number of elements in array.", nameof(array));
            }

            return arrayIndex == array.Length ? string.Empty : ConcatExtracted(array, arrayIndex, countLimit);
        }

        public static string Concat<T>(IEnumerable<T> values, Func<T, string> converter)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                var itemToString = converter.Invoke(item);
                stringList.Add(itemToString);
                length += itemToString.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Concat(IEnumerable<string> values)
        {
#if LESSTHAN_NET40
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                stringList.Add(item);
                length += item.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Concat(values);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Concat<T>(IEnumerable<T> values)
        {
#if LESSTHAN_NET40
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var stringList = new List<string>();
            var length = 0;
            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }

                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Concat(values);
#endif
        }

        private static string ConcatExtracted(object[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            var newArray = new string[count];
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (item == null)
                {
                    continue;
                }

                var itemToString = item.ToString();
                newArray[index - startIndex] = itemToString;
                length += itemToString.Length;
            }

            return ConcatExtractedExtracted(newArray, 0, count, length);
        }

        private static string ConcatExtracted(string[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!(item is null))
                {
                    length += item.Length;
                }
            }

            return ConcatExtractedExtracted(array, startIndex, maxIndex, length);
        }

        private static string ConcatExtractedExtracted(string[] array, int startIndex, int maxIndex, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder(length);
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                result.Append(item);
            }

            return result.ToString();
        }
    }

    public static partial class StringEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Join(string separator, IEnumerable<string> values)
        {
#if LESSTHAN_NET40
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator?.Length ?? 0;
            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }

                if (length != 0 && separatorLength != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }

                stringList.Add(item);
                length += item.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Join<T>(string separator, IEnumerable<T> values)
        {
#if LESSTHAN_NET40
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator?.Length ?? 0;
            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }

                if (length != 0 && separatorLength != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }

                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Join(string separator, params object[] values)
        {
#if LESSTHAN_NET40
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var stringList = new List<string>();
            var length = 0;
            var separatorLength = separator?.Length ?? 0;
            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }

                if (length != 0 && separatorLength != 0)
                {
                    stringList.Add(separator);
                    length += separatorLength;
                }

                var itemToString = item.ToString();
                stringList.Add(itemToString);
                length += itemToString.Length;
            }

            return ConcatExtractedExtracted(stringList.ToArray(), 0, stringList.Count, length);
#else
            return string.Join(separator, values);
#endif
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Join(string separator, params string[] values)
        {
            return string.Join(separator, values);
        }

        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static string Join(string separator, string[] values, int startIndex, int count)
        {
            return string.Join(separator, values, startIndex, count);
        }
    }

    public static partial class StringEx
    {
        public static string Implode(string separator, params object[] values)
        {
            if (separator == null)
            {
                return string.Concat(values);
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var array = new string[values.Length];
            var index = 0;
            foreach (var item in values)
            {
                array[index++] = item?.ToString();
            }

            return ImplodeExtracted(separator, array, 0, array.Length);
        }

        public static string Implode(string separator, object[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Implode(string separator, object[] array, int arrayIndex, int countLimit)
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

            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            return ImplodeExtracted(separator, array, arrayIndex, countLimit);
        }

        public static string Implode(string separator, params string[] value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            return ImplodeExtracted(separator, value, 0, value.Length);
        }

        public static string Implode(string separator, string[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex), "Non-negative number is required.");
            }

            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            return ImplodeExtracted(separator, array, arrayIndex, array.Length - arrayIndex);
        }

        public static string Implode(string separator, string[] array, int arrayIndex, int countLimit)
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

            if (arrayIndex == array.Length)
            {
                return string.Empty;
            }

            if (separator is null)
            {
                separator = string.Empty;
            }

            return ImplodeExtracted(separator, array, arrayIndex, countLimit);
        }

        public static string Implode(string separator, IEnumerable<string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values);
            }

            var stringList = values.ToList();
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode<T>(string separator, IEnumerable<T> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values);
            }

            var stringList = values.Select(item => item?.ToString()).ToList();
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values, converter);
            }

            var stringList = values.Select(item => item?.ToString()).ToList();
            return ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count);
        }

        public static string Implode(string separator, IEnumerable<string> values, string start, string end)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values);
            }

            var stringList = values.ToList();

            if (stringList.Count <= 0)
            {
                return string.Empty;
            }

            start = start ?? string.Empty;
            end = end ?? string.Empty;
            return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, string start, string end)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values);
            }

            var stringList = values.Select(item => item?.ToString()).ToList();

            if (stringList.Count <= 0)
            {
                return string.Empty;
            }

            start = start ?? string.Empty;
            end = end ?? string.Empty;
            return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
        }

        public static string Implode<T>(string separator, IEnumerable<T> values, Func<T, string> converter, string start, string end)
        {
            if (converter == null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (separator == null)
            {
                return Concat(values, converter);
            }

            var stringList = values.Select(item => item?.ToString()).ToList();

            if (stringList.Count <= 0)
            {
                return string.Empty;
            }

            start = start ?? string.Empty;
            end = end ?? string.Empty;
            return start + ImplodeExtracted(separator, stringList.ToArray(), 0, stringList.Count) + end;
        }

        private static string ImplodeExtracted(string separator, object[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            var newArray = new string[count];
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (item == null)
                {
                    continue;
                }

                var itemToString = item.ToString();
                newArray[index - startIndex] = itemToString;
                length += itemToString.Length;
            }

            length += separator.Length * (count - 1);
            return ImplodeExtractedExtracted(separator, newArray, 0, count, length);
        }

        private static string ImplodeExtracted(string separator, string[] array, int startIndex, int count)
        {
            var length = 0;
            var maxIndex = startIndex + count;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (!(item is null))
                {
                    length += item.Length;
                }
            }

            length += separator.Length * (count - 1);
            return ImplodeExtractedExtracted(separator, array, startIndex, maxIndex, length);
        }

        private static string ImplodeExtractedExtracted(string separator, string[] array, int startIndex, int maxIndex, int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            var result = new StringBuilder(length);
            var first = true;
            for (var index = startIndex; index < maxIndex; index++)
            {
                var item = array[index];
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append(separator);
                }

                result.Append(item);
            }

            return result.ToString();
        }
    }

    public static partial class StringEx
    {
        [MethodImpl(MethodImplOptionsEx.AggressiveInlining)]
        public static bool IsNullOrWhiteSpace(string value)
        {
#if LESSTHAN_NET40
            //Added in .NET 4.0
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            foreach (var character in value)
            {
                if (!char.IsWhiteSpace(character))
                {
                    return false;
                }
            }

            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }
    }
}
