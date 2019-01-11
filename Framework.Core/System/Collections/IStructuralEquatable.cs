#if LESSTHAN_NET40

namespace System.Collections
{
    public interface IStructuralEquatable
    {
        bool Equals(object other, IEqualityComparer comparer);

        int GetHashCode(IEqualityComparer comparer);
    }
}

#endif