// Needed for NET40

using System;
using System.Collections;
using System.Collections.Generic;

namespace Theraot.Collections
{
    public static partial class StructExtensions
    {
        public static IReadOnlyCollection<TSource?> AsNullableStructReadOnlyCollection<TSource>(this IReadOnlyCollection<TSource> source)
            where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new StructNullableCollection<TSource>(source);
        }

        private sealed class StructNullableCollection<TSource> : IReadOnlyCollection<TSource?>, ICollection<TSource?>
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
                Extensions.CanCopyTo(Count, array, arrayIndex);
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
                return System.Linq.Enumerable.Contains(_source, item);
            }
        }
    }

    public static partial class StructExtensions
    {
        public static IEnumerable<TSource?> AsNullableStructEnumerable<TSource>(this IEnumerable<TSource> source)
            where TSource : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return new NullableStructEnumerable<TSource>(source);
        }

        private sealed class NullableStructEnumerable<TSource> : IEnumerable<TSource?>
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
}