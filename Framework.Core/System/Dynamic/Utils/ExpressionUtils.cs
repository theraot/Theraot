#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Theraot.Collections;
using Theraot.Reflection;

namespace System.Dynamic.Utils
{
    internal static class ExpressionUtils
    {
        public static void RequiresCanRead(Expression expression, string paramName)
        {
            RequiresCanRead(expression, paramName, -1);
        }

        public static void RequiresCanRead(Expression expression, string paramName, int idx)
        {
            // validate that we can read the node
            switch (expression.NodeType)
            {
                case ExpressionType.Index:
                    var index = (IndexExpression)expression;
                    if (index.Indexer?.CanRead == false)
                    {
                        throw new ArgumentException("Expression must be readable", idx >= 0 ? $"{paramName}[{idx}]" : paramName);
                    }

                    break;

                case ExpressionType.MemberAccess:
                    var member = (MemberExpression)expression;
                    if (member.Member is PropertyInfo prop && !prop.CanRead)
                    {
                        throw new ArgumentException("Expression must be readable", idx >= 0 ? $"{paramName}[{idx}]" : paramName);
                    }

                    break;

                default:
                    break;
            }
        }

        public static T ReturnObject<T>(object collectionOrT) where T : class
        {
            if (collectionOrT is T t)
            {
                return t;
            }

            return ((ReadOnlyCollection<T>)collectionOrT)[0];
        }

        public static ReadOnlyCollection<ParameterExpression> ReturnReadOnly(IParameterProvider provider, ref object collection)
        {
            if (collection is ParameterExpression tObj)
            {
                // otherwise make sure only one read-only collection ever gets exposed
                Interlocked.CompareExchange
                (
                    ref collection,
                    new ReadOnlyCollection<ParameterExpression>(new ListParameterProvider(provider, tObj)),
                    tObj
                );
            }

            // and return what is not guaranteed to be a read-only collection
            return (ReadOnlyCollection<ParameterExpression>)collection;
        }

        public static ReadOnlyCollection<Expression> ReturnReadOnly(IArgumentProvider provider, ref object collection)
        {
            if (collection is Expression tObj)
            {
                // otherwise make sure only one read-only collection ever gets exposed
                Interlocked.CompareExchange
                (
                    ref collection,
                    new ReadOnlyCollection<Expression>(new ListArgumentProvider(provider, tObj)),
                    tObj
                );
            }

            // and return what is not guaranteed to be a read-only collection
            return (ReadOnlyCollection<Expression>)collection;
        }

        // Attempts to auto-quote the expression tree. Returns true if it succeeded, false otherwise.
        public static bool TryQuote(Type parameterType, [NotNull] ref Expression argument)
        {
            // We used to allow quoting of any expression, but the behavior of
            // quote (produce a new tree closed over parameter values), only
            // works consistently for lambdas
            var quotable = typeof(LambdaExpression);

            if (!parameterType.IsSameOrSubclassOfInternal(quotable) || !parameterType.IsInstanceOfType(argument))
            {
                return false;
            }

            argument = Expression.Quote(argument);
            return true;
        }

        public static void ValidateArgumentCount(MethodBase method, ExpressionType nodeKind, int count, ParameterInfo[] pis)
        {
            if (pis.Length == count)
            {
                return;
            }

            // Throw the right error for the node we were given
            switch (nodeKind)
            {
                case ExpressionType.New:
                    throw new ArgumentException("Incorrect number of arguments for constructor");
                case ExpressionType.Invoke:
                    throw new InvalidOperationException("Incorrect number of arguments supplied for lambda invocation");
                case ExpressionType.Dynamic:
                case ExpressionType.Call:
                    throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{method}'", nameof(method));
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        public static void ValidateArgumentCount(this LambdaExpression lambda)
        {
            if (((IParameterProvider)lambda).ParameterCount >= ushort.MaxValue)
            {
                throw new InvalidProgramException();
            }
        }

        public static void ValidateArgumentTypes(MethodBase method, ExpressionType nodeKind, ref Expression[] arguments, string methodParamName)
        {
            Debug.Assert(nodeKind == ExpressionType.Invoke || nodeKind == ExpressionType.Call || nodeKind == ExpressionType.Dynamic || nodeKind == ExpressionType.New);

            var pis = GetParametersForValidation(method, nodeKind);

            ValidateArgumentCount(method, nodeKind, arguments.Length, pis);

            Expression[]? newArgs = null;
            for (int i = 0, n = pis.Length; i < n; i++)
            {
                var arg = arguments[i];
                var pi = pis[i];
                arg = ValidateOneArgument(method, nodeKind, arg, pi, methodParamName, nameof(arguments), i);

                if (newArgs == null && arg != arguments[i])
                {
                    newArgs = new Expression[arguments.Length];
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
                arguments = newArgs;
            }
        }

        public static Expression ValidateOneArgument(MethodBase method, ExpressionType nodeKind, Expression arguments, ParameterInfo pi, string methodParamName, string argumentParamName, int index = -1)
        {
            ContractUtils.RequiresNotNull(arguments, argumentParamName, index);
            RequiresCanRead(arguments, argumentParamName, index);
            var pType = pi.ParameterType;
            if (pType.IsByRef)
            {
                pType = pType.GetElementType();
            }

            TypeUtils.ValidateType(pType, methodParamName, allowByRef: true, allowPointer: true);
            if (pType.IsReferenceAssignableFromInternal(arguments.Type) || TryQuote(pType, ref arguments))
            {
                return arguments;
            }

            // Throw the right error for the node we were given
            switch (nodeKind)
            {
                case ExpressionType.New:
                    throw new ArgumentException($"Expression of type '{arguments.Type}' cannot be used for constructor parameter of type '{pType}'", index >= 0 ? $"{argumentParamName}[{index}]" : argumentParamName);
                case ExpressionType.Invoke:
                    throw new ArgumentException($"Expression of type '{arguments.Type}' cannot be used for parameter of type '{pType}'", index >= 0 ? $"{argumentParamName}[{index}]" : argumentParamName);
                case ExpressionType.Dynamic:
                case ExpressionType.Call:
                    throw new ArgumentException($"Expression of type '{arguments.Type}' cannot be used for parameter of type '{pType}' of method '{method}'", index >= 0 ? $"{argumentParamName}[{index}]" : argumentParamName);
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        internal static ParameterInfo[] GetParametersForValidation(MethodBase method, ExpressionType nodeKind)
        {
            var pis = method.GetParameters();

            if (nodeKind == ExpressionType.Dynamic)
            {
                pis = pis.RemoveFirst(); // ignore CallSite argument
            }

            return pis;
        }

        internal static bool SameElements<T>(ICollection<T>? replacement, T[] current)
            where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current.Length == 0;
            }

            return SameElementsInCollection(replacement, current);
        }

        internal static bool SameElements<T>([NotNullIfNotNull("replacement")] ref IEnumerable<T>? replacement, T[] current)
            where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current.Length == 0;
            }

            // Ensure arguments is safe to enumerate twice.
            // If we have to build a collection, build a ArrayReadOnlyCollection<T>
            // so it won't be built a second time if used.
            if (replacement is not ICollection<T> replacementCol)
            {
                replacement = replacementCol = replacement.ToReadOnlyCollection();
            }

            return SameElementsInCollection(replacementCol, current);
        }

        internal static bool SameElementsInCollectionWithPossibleNulls<T>(ICollection<T?> replacement, T?[] current)
            where T : class
        {
            var count = current.Length;
            if (replacement.Count != count)
            {
                return false;
            }

            if (count == 0)
            {
                return true;
            }

            var index = 0;
            foreach (var replacementObject in replacement)
            {
                if (replacementObject != current[index])
                {
                    return false;
                }

                index++;
            }

            return true;
        }

        internal static bool SameElementsWithPossibleNulls<T>(ICollection<T?>? replacement, T?[] current)
            where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current.Length == 0;
            }

            return SameElementsInCollectionWithPossibleNulls(replacement, current);
        }

        internal static bool SameElementsWithPossibleNulls<T>(ref IEnumerable<T?>? replacement, T?[] current)
            where T : class
        {
            if (replacement == current) // Relatively common case, so particularly useful to take the short-circuit.
            {
                return true;
            }

            if (replacement == null) // Treat null as empty.
            {
                return current.Length == 0;
            }

            // Ensure arguments is safe to enumerate twice.
            // If we have to build a collection, build a ArrayReadOnlyCollection<T>
            // so it won't be built a second time if used.
            if (replacement is not ICollection<T?> replacementCol)
            {
                replacement = replacementCol = replacement.ToReadOnlyCollection();
            }

            return SameElementsInCollectionWithPossibleNulls(replacementCol, current);
        }

        private static bool SameElementsInCollection<T>(ICollection<T> replacement, T[] current)
            where T : class
        {
            var count = current.Length;
            if (replacement.Count != count)
            {
                return false;
            }

            if (count == 0)
            {
                return true;
            }

            var index = 0;
            foreach (var replacementObject in replacement)
            {
                if (replacementObject != current[index])
                {
                    return false;
                }

                index++;
            }

            return true;
        }
    }
}

#endif