#if LESSTHAN_NET35

using System.Collections;
using System.Linq.Expressions;

namespace System.Linq
{
    public interface IQueryable : IEnumerable
    {
        Type ElementType { get; }

        Expression Expression { get; }

        IQueryProvider Provider { get; }
    }
}

#endif