#if TARGETS_NET || LESSTHAN_NET50 || TARGETS_NETSTANDARD

namespace System.Collections.Generic
{
    public interface IReadOnlySet<T> : IReadOnlyCollection<T>
    {
        public bool Contains(T item);

        public bool IsProperSubsetOf(IEnumerable<T> other);

        public bool IsProperSupersetOf(IEnumerable<T> other);

        public bool IsSubsetOf(IEnumerable<T> other);

        public bool IsSupersetOf(IEnumerable<T> other);

        public bool Overlaps(IEnumerable<T> other);

        public bool SetEquals(IEnumerable<T> other);
    }
}

#endif