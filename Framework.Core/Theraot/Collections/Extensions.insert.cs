#if FAT
using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class Extensions
    {
        public static void InsertRange<T, TCollection>(this TCollection source, int index, T item)
            where TCollection : class, IList<T>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            source.Insert(index, item);
        }

        public static void InsertRange<T, TCollection>(this TCollection source, int index, Func<T> item)
            where TCollection : class, IList<T>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            source.Insert(index, item());
        }

        public static int InsertRange<T, TCollection>(this TCollection source, int index, IEnumerable<T> items)
            where TCollection : class, IList<T>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var initialIndex = index;
            foreach (var item in items)
            {
                source.Insert(initialIndex, item);
                checked
                {
                    index++;
                }
            }
            return index - initialIndex;
        }

        public static int InsertRange<T, TCollection>(this TCollection source, int index, IEnumerable<Func<T>> items)
            where TCollection : class, IList<T>
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }
            var initialIndex = index;
            foreach (var item in items)
            {
                source.Insert(initialIndex, item());
                checked
                {
                    index++;
                }
            }
            return index - initialIndex;
        }
    }
}

#endif