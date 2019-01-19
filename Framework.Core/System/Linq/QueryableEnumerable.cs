#if LESSTHAN_NET40

// QueryableEnumerable<TElement>.cs
//
// Authors:
//  Roei Erez (roeie@mainsoft.com)
//  Jb Evain (jbevain@novell.com)
//
// Copyright (C) 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Needed for NET35 (LINQ)

using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System.Linq
{
    internal interface IQueryableEnumerable : IQueryable
    {
        IEnumerable GetEnumerable();
    }

    internal interface IQueryableEnumerable<TElement> : IQueryableEnumerable, IOrderedQueryable<TElement>
    {
        // Empty
    }

    internal class QueryableEnumerable<TElement> : IQueryableEnumerable<TElement>, IQueryProvider
    {
        private readonly IEnumerable<TElement> _enumerable;

        public QueryableEnumerable(IEnumerable<TElement> enumerable)
        {
            Expression = Expression.Constant(this);
            _enumerable = enumerable;
        }

        public QueryableEnumerable(Expression expression)
        {
            Expression = expression;
        }

        public Type ElementType => typeof(TElement);

        public Expression Expression { get; }

        public IQueryProvider Provider => this;

        public IQueryable CreateQuery(Expression expression)
        {
            return (IQueryable)Activator.CreateInstance
                    (
                        typeof(QueryableEnumerable<>).MakeGenericType
                        (
                            expression.Type.GetGenericArguments()[0]
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
            return _enumerable;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Execute<IEnumerable<TElement>>(Expression).GetEnumerator();
        }

        public override string ToString()
        {
            if (_enumerable != null)
            {
                return _enumerable.ToString();
            }
            if (Expression == null)
            {
                return base.ToString();
            }
            return Expression is ConstantExpression constant && constant.Value == this ? base.ToString() : Expression.ToString();
        }

        private static Expression TransformQueryable(Expression expression)
        {
            return new QueryableTransformer().Transform(expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

#endif