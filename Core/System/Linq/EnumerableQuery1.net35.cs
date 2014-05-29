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
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
            get
            {
                return queryable.ElementType;
            }
        }

        Expression IQueryable.Expression
        {
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
            get
            {
                return queryable.Expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
            get
            {
                return queryable;
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return queryable.GetEnumerator();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return queryable.GetEnumerator();
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            return queryable.CreateQuery(expression);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        IQueryable<TElem> IQueryProvider.CreateQuery<TElem>(Expression expression)
        {
            return new EnumerableQuery<TElem>(expression);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
        object IQueryProvider.Execute(Expression expression)
        {
            return queryable.Execute(expression);
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "Microsoft's Design")]
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