using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Theraot.Collections.Specialized
{
    public class ArrayReadOnlyCollection<T> : ReadOnlyCollectionEx<T>
    {
        protected ArrayReadOnlyCollection(params T[] list)
            : base(list)
        {
            Wrapped = list;
        }

        internal T[] Wrapped { get; }

        public static ArrayReadOnlyCollection<T> Create(params T[] list)
        {
            return new ArrayReadOnlyCollection<T>(list);
        }

        public static ArrayReadOnlyCollection<T> Create(IEnumerable<T> enumerable)
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
            return array.Length == 0 ? EmptyCollection<T>.Instance : ArrayReadOnlyCollection<T>.Create(array);
        }
    }
}