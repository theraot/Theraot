using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
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
                    stringList.Add(separator!);
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
                if (item is null)
                {
                    continue;
                }

                if (length != 0 && separatorLength != 0)
                {
                    stringList.Add(separator!);
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
                    stringList.Add(separator!);
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
}