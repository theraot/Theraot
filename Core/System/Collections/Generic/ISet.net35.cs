#if NET35

namespace System.Collections.Generic
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Backport")]
    public interface ISet<T> : ICollection<T>
    {
        new bool Add(T item);

        void ExceptWith(IEnumerable<T> other);

        void IntersectWith(IEnumerable<T> other);

        bool IsProperSubsetOf(IEnumerable<T> other);

        bool IsProperSupersetOf(IEnumerable<T> other);

        bool IsSubsetOf(IEnumerable<T> other);

        bool IsSupersetOf(IEnumerable<T> other);

        bool Overlaps(IEnumerable<T> other);

        bool SetEquals(IEnumerable<T> other);

        void SymmetricExceptWith(IEnumerable<T> other);

        void UnionWith(IEnumerable<T> other);
    }
}

#endif