// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Theraot.Collections;
using Theraot.Core;

namespace System.Dynamic.Utils
{
    internal static class ExpressionUtils
    {
        public static ReadOnlyCollection<T> ReturnReadOnly<T>(ref IList<T> collection)
        {
            var value = collection;

            // if it's already read-only just return it.
            var res = value as ReadOnlyCollection<T>;
            if (res != null)
            {
                return res;
            }

            // otherwise make sure only readonly collection every gets exposed
            Interlocked.CompareExchange<IList<T>>(
                ref collection,
                value.ToReadOnly(),
                value
            );

            // and return it
            return (ReadOnlyCollection<T>)collection;
        }

        public static ReadOnlyCollection<Expression> ReturnReadOnly(IArgumentProvider provider, ref object collection)
        {
            var tObj = collection as Expression;
            if (tObj != null)
            {
                // otherwise make sure only one readonly collection ever gets exposed
                Interlocked.CompareExchange(
                    ref collection,
                    new ReadOnlyCollection<Expression>(new ListArgumentProvider(provider, tObj)),
                    tObj
                );
            }

            // and return what is not guaranteed to be a readonly collection
            return (ReadOnlyCollection<Expression>)collection;
        }

        public static T ReturnObject<T>(object collectionOrT) where T : class
        {
            var t = collectionOrT as T;
            if (t != null)
            {
                return t;
            }

            return ((ReadOnlyCollection<T>)collectionOrT)[0];
        }

        public static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref ReadOnlyCollection<Expression> arguments)
        {
#if NET35
            Debug.Assert(nodeKind == ExpressionType.Invoke || nodeKind == ExpressionType.Call || nodeKind == ExpressionType.New);
#else
            Debug.Assert(nodeKind == ExpressionType.Invoke || nodeKind == ExpressionType.Call || nodeKind == ExpressionType.Dynamic || nodeKind == ExpressionType.New);
#endif

            var pis = GetParametersForValidation(method, nodeKind);

            ValidateArgumentCount(method, nodeKind, arguments.Count, pis);

            Expression[] newArgs = null;
            var n = pis.Length;
            for (int i = 0; i < n; i++)
            {
                var arg = arguments[i];
                var pi = pis[i];
                arg = ValidateOneArgument(method, nodeKind, arg, pi);

                if (newArgs == null && arg != arguments[i])
                {
                    newArgs = new Expression[arguments.Count];
                    for (var j = 0; j < i; j++)
                    {
                        newArgs[j] = arguments[j];
                    }
                }
                if (newArgs != null)
                {
                    newArgs[i] = arg;
                }
            }
            if (newArgs != null)
            {
                arguments = new ReadOnlyCollection<Expression>(newArgs);
            }
        }

        public static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
        {
            if (pis.Length != count)
            {
                // Throw the right error for the node we were given
                switch (nodeKind)
                {
                    case ExpressionType.New:
                        throw Error.IncorrectNumberOfConstructorArguments();
                    case ExpressionType.Invoke:
                        throw Error.IncorrectNumberOfLambdaArguments();
#if !NET35
                    case ExpressionType.Dynamic:
#endif
                    case ExpressionType.Call:
                        throw Error.IncorrectNumberOfMethodCallArguments(method);
                    default:
                        throw ContractUtils.Unreachable;
                }
            }
        }

        public static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arg, ParameterInfo pi)
        {
            RequiresCanRead(arg, "arguments");
            var pType = pi.ParameterType;
            if (pType.IsByRef)
            {
                pType = pType.GetElementType();
            }
            TypeHelper.ValidateType(pType);
            if (!TypeHelper.AreReferenceAssignable(pType, arg.Type))
            {
                if (!TryQuote(pType, ref arg))
                {
                    // Throw the right error for the node we were given
                    switch (nodeKind)
                    {
                        case ExpressionType.New:
                            throw Error.ExpressionTypeDoesNotMatchConstructorParameter(arg.Type, pType);
                        case ExpressionType.Invoke:
                            throw Error.ExpressionTypeDoesNotMatchParameter(arg.Type, pType);
#if !NET35
                        case ExpressionType.Dynamic:
#endif
                        case ExpressionType.Call:
                            throw Error.ExpressionTypeDoesNotMatchMethodParameter(arg.Type, pType, method);
                        default:
                            throw ContractUtils.Unreachable;
                    }
                }
            }
            return arg;
        }

        public static void RequiresCanRead(Expression expression, string paramName)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(paramName);
            }

            // validate that we can read the node
            switch (expression.NodeType)
            {
#if !NET35
                case ExpressionType.Index:
                    var index = (IndexExpression)expression;
                    if (index.Indexer != null && !index.Indexer.CanRead)
                    {
                        throw new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
                    }
                    break;
#endif
                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)expression;
                    var prop = member.Member as PropertyInfo;
                    if (prop != null)
                    {
                        if (!prop.CanRead)
                        {
                            throw new ArgumentException(Strings.ExpressionMustBeReadable, paramName);
                        }
                    }
                    break;
            }
        }

        // Attempts to auto-quote the expression tree. Returns true if it succeeded, false otherwise.
        public static bool TryQuote(Type parameterType, ref Expression argument)
        {
            // We used to allow quoting of any expression, but the behavior of
            // quote (produce a new tree closed over parameter values), only
            // works consistently for lambdas
            var quoteable = typeof(LambdaExpression);
            if (parameterType.IsSameOrSubclassOf(quoteable)
                && parameterType.IsInstanceOfType(argument))
            {
                argument = Expression.Quote(argument);
                return true;
            }
            return false;
        }

        internal static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
        {
            var pis = method.GetParameters();
#if !NET35
            if (nodeKind == ExpressionType.Dynamic)
            {
                pis = pis.RemoveFirst(); // ignore CallSite argument
            }
#endif
            return pis;
        }
    }
}