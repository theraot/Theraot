using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Theraot.Collections.Specialized
{
    public static class ArrayReadOnlyCollection
    {
        public static ArrayReadOnlyCollection<T> Create<T>(params T[] list)
        {
            return new ArrayReadOnlyCollection<T>(list);
        }

        public static ArrayReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
            {
                return EmptyCollection<T>.Instance;
            }
            if (enumerable is ArrayReadOnlyCollection<T> arrayReadOnlyCollection)
            {
                return arrayReadOnlyCollection;
            }
            var array = Extensions.AsArray(enumerable);
            return array.Length == 0 ? EmptyCollection<T>.Instance : Create(array);
        }
    }

    public class ArrayReadOnlyCollection<T> : ReadOnlyCollectionEx<T>
    {
        protected internal ArrayReadOnlyCollection(params T[] list)
            : base(list)
        {
            Wrapped = list;
        }

        internal T[] Wrapped { get; }
    }
}