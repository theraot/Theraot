#if NET20 || NET30 || NET35

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    public class EnumerableQuery<T> : EnumerableQuery, IOrderedQueryable<T>, IQueryProvider
    {
        private readonly QueryableEnumerable<T> queryable;

        public EnumerableQuery(Expression expression)
        {
            queryable = new QueryableEnumerable<T>(expression);
        }

        public EnumerableQuery(IEnumerable<T> enumerable)
        {
            queryable = new QueryableEnumerable<T>(enumerable);
        }

        Type IQueryable.ElementType
        {
            get
            {
                return queryable.ElementType;
            }
        }

        Expression IQueryable.Expression
        {
            get
            {
                return queryable.Expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return queryable;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queryable.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return queryable.GetEnumerator();
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return queryable.CreateQuery(expression);
        }

        IQueryable<TElem> IQueryProvider.CreateQuery<TElem>(Expression expression)
        {
            return new EnumerableQuery<TElem>(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return queryable.Execute(expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return queryable.Execute<TResult>(expression);
        }

        public override string ToString()
        {
            return queryable.ToString();
        }
    }
}

#endif