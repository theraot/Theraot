#if LESSTHAN_NET35

using System.Collections.Generic;

namespace System.Linq
{
    public interface IQueryable<T> : IQueryable, IEnumerable<T>
    {
        //Empty
    }
}

#endif