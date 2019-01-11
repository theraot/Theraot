#if LESSTHAN_NET40

namespace System.Collections
{
    public interface IStructuralComparable
    {
        int CompareTo(object other, IComparer comparer);
    }
}

#endif