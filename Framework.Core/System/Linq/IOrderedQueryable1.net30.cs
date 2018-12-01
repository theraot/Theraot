#if NET20 || NET30

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