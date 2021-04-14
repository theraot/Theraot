#if LESSTHAN_NET40

// QueryableTransformer.cs
//
// Authors:
//    Roei Erez (roeie@mainsoft.com)
//    Jb Evain (jbevain@novell.com)
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
using System.Dynamic.Utils;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Theraot.Reflection;

namespace System.Linq
{
    internal class QueryableTransformer : ExpressionTransformer
    {
        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value is not IQueryableEnumerable qe)
            {
                return constant;
            }

            var enumerable = qe.GetEnumerable();
            return enumerable != null ? Expression.Constant(enumerable) : Visit(qe.Expression);
        }

        protected override Expression VisitLambda(LambdaExpression lambda)
        {
            return lambda;
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            return IsQueryableExtension(methodCall.Method) ? ReplaceQueryableMethod(methodCall) : base.VisitMethodCall(methodCall);
        }

        private static Type GetComparableType(Type type)
        {
            if (type.IsGenericInstanceOf(typeof(IQueryable<>)))
            {
                return typeof(IEnumerable<>).MakeGenericTypeFrom(type);
            }

            if (type.IsGenericInstanceOf(typeof(IOrderedQueryable<>)))
            {
                return typeof(IOrderedEnumerable<>).MakeGenericTypeFrom(type);
            }

            if (type.IsGenericInstanceOf(typeof(Expression<>)))
            {
                return type.GetGenericArguments()[0];
            }

            return type == typeof(IQueryable) ? typeof(IEnumerable) : type;
        }

        private static MethodInfo? GetMatchingMethod(MethodInfo method, Type declaring)
        {
            return (
                from candidate
                    in declaring.GetMethods()
                where MethodMatch(candidate, method)
                select method.IsGenericMethod ? candidate.MakeGenericMethodFrom(method) : candidate
            ).FirstOrDefault();
        }

        private static Type GetTargetDeclaringType(MethodInfo method)
        {
            return method.DeclaringType == typeof(Queryable) ? typeof(Enumerable) : method.DeclaringType;
        }

        private static bool HasExtensionAttribute(MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(ExtensionAttribute), inherit: false).Length > 0;
        }

        private static bool IsQueryableExtension(MethodInfo method)
        {
            return HasExtensionAttribute(method) && method.GetParameters()[0].ParameterType.IsAssignableTo(typeof(IQueryable));
        }

        private static bool MethodMatch(MethodInfo candidate, MethodInfo method)
        {
            if (!string.Equals(candidate.Name, method.Name, StringComparison.Ordinal) || !HasExtensionAttribute(candidate))
            {
                return false;
            }

            var parameters = method.GetParameterTypes();
            if (parameters.Length != candidate.GetParameters().Length)
            {
                return false;
            }

            if (method.IsGenericMethod)
            {
                if (!candidate.IsGenericMethod || candidate.GetGenericArguments().Length != method.GetGenericArguments().Length)
                {
                    return false;
                }

                candidate = candidate.MakeGenericMethodFrom(method);
            }

            if (!TypeMatch(candidate.ReturnType, method.ReturnType))
            {
                return false;
            }

            var candidateParameters = candidate.GetParameterTypes();
            if (candidateParameters[0] != GetComparableType(parameters[0]))
            {
                return false;
            }

            for (var index = 1; index < candidateParameters.Length; ++index)
            {
                if (!TypeMatch(candidateParameters[index], parameters[index]))
                {
                    return false;
                }
            }

            return true;
        }

        private static MethodInfo ReplaceQueryableMethodCore(MethodInfo method)
        {
            var targetType = GetTargetDeclaringType(method);
            var result = GetMatchingMethod(method, targetType);
            if (result != null)
            {
                return result;
            }

            throw new InvalidOperationException($"There is no method {method.Name} on type {targetType.FullName} that matches the specified arguments");
        }

        private static bool TypeMatch(Type candidate, Type type)
        {
            if (candidate == type)
            {
                return true;
            }

            return candidate == GetComparableType(type);
        }

        private static Expression UnquoteIfNeeded(Expression expression, Type delegateType)
        {
            if (expression.NodeType != ExpressionType.Quote)
            {
                return expression;
            }

            var lambda = (LambdaExpression)((UnaryExpression)expression).Operand!;
            return lambda.Type == delegateType ? lambda : expression;
        }

        private MethodCallExpression ReplaceQueryableMethod(MethodCallExpression old)
        {
            Expression? target = null;
            if (old.Object != null)
            {
                target = Visit(old.Object);
            }

            var method = ReplaceQueryableMethodCore(old.Method);
            var parameters = method.GetParameters();
            var arguments = new Expression[old.Arguments.Count];
            for (var index = 0; index < arguments.Length; index++)
            {
                arguments[index] = UnquoteIfNeeded
                (
                    Visit(old.Arguments[index]),
                    parameters[index].ParameterType
                );
            }

            return Expression.Call(target, method, arguments);
        }
    }
}

#endif