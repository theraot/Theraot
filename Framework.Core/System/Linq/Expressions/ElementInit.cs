﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Collections;

namespace System.Linq.Expressions
{
    /// <summary>
    ///     Represents the initialization of a list.
    /// </summary>
    public sealed class ElementInit : IArgumentProvider
    {
        private readonly ReadOnlyCollectionEx<Expression> _argumentsAsReadOnlyCollection;

        internal ElementInit(MethodInfo addMethod, Expression[] arguments)
        {
            AddMethod = addMethod;
            _argumentsAsReadOnlyCollection = ReadOnlyCollectionEx.Create(arguments);
        }

        /// <summary>
        ///     Gets the <see cref="MethodInfo" /> used to add elements to the object.
        /// </summary>
        public MethodInfo AddMethod { get; }

        /// <summary>
        ///     Gets the number of argument expressions of the node.
        /// </summary>
        public int ArgumentCount => Arguments.Count;

        /// <summary>
        ///     Gets the list of elements to be added to the object.
        /// </summary>
        public ReadOnlyCollection<Expression> Arguments => _argumentsAsReadOnlyCollection;

        /// <summary>
        ///     Gets the argument expression with the specified <paramref name="index" />.
        /// </summary>
        /// <param name="index">The index of the argument expression to get.</param>
        /// <returns>The expression representing the argument at the specified <paramref name="index" />.</returns>
        public Expression GetArgument(int index)
        {
            return Arguments[index] ?? Expression.Constant(value: null);
        }

        /// <summary>
        ///     Creates a <see cref="string" /> representation of the node.
        /// </summary>
        /// <returns>A <see cref="string" /> representation of the node.</returns>
        public override string ToString()
        {
            return ExpressionStringBuilder.ElementInitBindingToString(this);
        }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="arguments">The <see cref="Arguments" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ElementInit Update(IEnumerable<Expression> arguments)
        {
            return arguments == Arguments ? this : Expression.ElementInit(AddMethod, arguments);
        }
    }

    public partial class Expression
    {
        /// <summary>
        ///     Creates an <see cref="Expressions.ElementInit" /> expression that represents the initialization of a list.
        /// </summary>
        /// <param name="addMethod">The <see cref="MethodInfo" /> for the list's Add method.</param>
        /// <param name="arguments">An array containing the Expressions to be used to initialize the list.</param>
        /// <returns>The created <see cref="Expressions.ElementInit" /> expression.</returns>
        public static ElementInit ElementInit(MethodInfo addMethod, params Expression[] arguments)
        {
            return ElementInit(addMethod, arguments as IEnumerable<Expression>);
        }

        /// <summary>
        ///     Creates an <see cref="Expressions.ElementInit" /> expression that represents the initialization of a list.
        /// </summary>
        /// <param name="addMethod">The <see cref="MethodInfo" /> for the list's Add method.</param>
        /// <param name="arguments">
        ///     An <see cref="IEnumerable{T}" /> containing <see cref="Expression" /> elements to initialize
        ///     the list.
        /// </param>
        /// <returns>The created <see cref="Expressions.ElementInit" /> expression.</returns>
        public static ElementInit ElementInit(MethodInfo addMethod, IEnumerable<Expression> arguments)
        {
            ContractUtils.RequiresNotNull(addMethod, nameof(addMethod));
            ContractUtils.RequiresNotNull(arguments, nameof(arguments));

            var array = arguments.AsArrayInternal();
            return ElementInitExtracted(addMethod, nameof(arguments), ref array);
        }

        private static ElementInit ElementInitExtracted(MethodInfo addMethod, string paramName, ref Expression[] array)
        {
            RequiresCanRead(array, paramName);
            ValidateElementInitAddMethodInfo(addMethod, nameof(addMethod));
            ValidateArgumentTypes(addMethod, ExpressionType.Call, ref array, nameof(addMethod));
            return new ElementInit(addMethod, array);
        }

        private static void ValidateElementInitAddMethodInfo(MethodInfo addMethod, string paramName)
        {
            ValidateMethodInfo(addMethod, paramName);
            var pis = addMethod.GetParameters();
            if (pis.Length == 0)
            {
                throw new ArgumentException("Element initializer method must have at least 1 parameter", paramName);
            }

            if (!addMethod.Name.Equals("Add", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Element initializer method must be named 'Add'", paramName);
            }

            if (addMethod.IsStatic)
            {
                throw new ArgumentException("Element initializer method must be an instance method", paramName);
            }

            foreach (var pi in pis)
            {
                if (pi.ParameterType.IsByRef)
                {
                    throw new ArgumentException($"Parameter '{pi.Name}' of element initializer method '{addMethod.Name}' must not be a pass by reference parameter", paramName);
                }
            }
        }
    }
}

#endif