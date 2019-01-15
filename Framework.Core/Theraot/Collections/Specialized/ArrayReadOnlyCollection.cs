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