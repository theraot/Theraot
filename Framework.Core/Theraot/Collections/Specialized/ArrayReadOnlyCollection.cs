using System.Collections.ObjectModel;

namespace Theraot.Collections.Specialized
{
    public class ArrayReadOnlyCollection<T> : ReadOnlyCollectionEx<T>
    {
        public ArrayReadOnlyCollection(params T[] list)
            : base(list)
        {
            Wrapped = list;
        }

        internal T[] Wrapped { get; }
    }
}