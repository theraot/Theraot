#if LESSTHAN_NET35

namespace System.Linq
{
    public interface IOrderedQueryable : IQueryable
    {
        // Empty
    }

    public interface IOrderedQueryable<T> : IOrderedQueryable, IQueryable<T>
    {
        //Empty
    }
}

#endif