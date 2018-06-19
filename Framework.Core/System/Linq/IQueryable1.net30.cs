#if NET20 || NET30

using System.Collections.Generic;

namespace System.Linq
{
    public interface IQueryable<T> : IQueryable, IEnumerable<T>
    {
        //Empty
    }
}

#endif