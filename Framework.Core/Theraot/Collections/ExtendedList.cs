using System;
using System.Collections.Generic;

namespace Theraot.Collections
{
    [Serializable]
    [System.Diagnostics.DebuggerNonUserCode]
    [System.Diagnostics.DebuggerDisplay("Count={Count}")]
    public sealed class ExtendedList<T> : List<T>, IReadOnlyList<T>
    {
        public ExtendedList()
        {
            // Empty
        }

        public ExtendedList(int capacity)
            : base(capacity)
        {
            // Empty
        }

        public ExtendedList(IEnumerable<T> collection)
            : base(collection)
        {
            // Empty
        }

        public bool Remove(T item, IEqualityComparer<T> comparer)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            foreach (var foundItem in this.RemoveWhereEnumerable(input => comparer.Equals(input, item)))
            {
                GC.KeepAlive(foundItem);
                return true;
            }
            return false;
        }
    }
}