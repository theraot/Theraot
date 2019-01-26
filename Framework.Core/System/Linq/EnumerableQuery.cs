#if LESSTHAN_NET40
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    public abstract class EnumerableQuery
    {
        // Empty
    }

    public class EnumerableQuery<T> : EnumerableQuery, IOrderedQueryable<T>, IQueryProvider
    {
        private readonly QueryableEnumerable<T> _queryable;

        public EnumerableQuery(Expression expression)
        {
            _queryable = new QueryableEnumerable<T>(expression);
        }

        public EnumerableQuery(IEnumerable<T> enumerable)
        {
            _queryable = new QueryableEnumerable<T>(enumerable);
        }

        Type IQueryable.ElementType => _queryable.ElementType;

        Expression IQueryable.Expression => _queryable.Expression;

        IQueryProvider IQueryable.Provider => _queryable;

        public override string ToString()
        {
            return _queryable.ToString();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return _queryable.CreateQuery(expression);
        }

        IQueryable<TElem> IQueryProvider.CreateQuery<TElem>(Expression expression)
        {
            return new EnumerableQuery<TElem>(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return _queryable.Execute(expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return _queryable.Execute<TResult>(expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }
    }
}

#endif