using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    internal interface IQueryableEnumerable : IQueryable
    {
        IEnumerable GetEnumerable();
    }

    internal interface IQueryableEnumerable<TElement> : IQueryableEnumerable, IQueryable<TElement>, IOrderedQueryable<TElement>
    {
        //Empty
    }

    internal class QueryableEnumerable<TElement> : IQueryableEnumerable<TElement>, IQueryProvider
    {
        private IEnumerable<TElement> _enumerable;
        private Expression _expression;

        public QueryableEnumerable(IEnumerable<TElement> enumerable)
        {
            _expression = Expression.Constant(this);
            _enumerable = enumerable;
        }

        public QueryableEnumerable(Expression expression)
        {
            _expression = expression;
        }

        public Type ElementType
        {
            get
            {
                return typeof(TElement);
            }
        }

        public IEnumerable<TElement> Enumerable
        {
            get
            {
                return _enumerable;
            }
            set
            {
                _enumerable = value;
            }
        }

        public Expression Expression
        {
            get
            {
                return _expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this;
            }
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)Activator.CreateInstance
                   (
                       typeof(QueryableEnumerable<>).MakeGenericType
                       (
                           expression.Type.GetFirstGenericArgument()
                       ),
                       expression
                   );
        }

        public IQueryable<TElem> CreateQuery<TElem>(Expression expression)
        {
            return new QueryableEnumerable<TElem>(expression);
        }

        public object Execute(Expression expression)
        {
            var lambda = Expression.Lambda(TransformQueryable(expression));
            return lambda.Compile().DynamicInvoke();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var lambda = Expression.Lambda<Func<TResult>>(TransformQueryable(expression));
            return lambda.Compile().Invoke();
        }

        public IEnumerable GetEnumerable()
        {
            return Enumerable;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Execute<IEnumerable<TElement>>(_expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            if (Enumerable != null)
            {
                return Enumerable.ToString();
            }
            if (_expression == null)
            {
                return base.ToString();
            }
            var constant = _expression as ConstantExpression;
            if (constant != null && constant.Value == this)
            {
                return base.ToString();
            }
            return _expression.ToString();
        }

        internal static Expression TransformQueryable(Expression expression)
        {
            return new QueryableTransformer().Transform(expression);
        }
    }
}