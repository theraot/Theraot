﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using System.Reflection;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents getting the length of a one-dimensional, zero-based
        ///     array.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.ArrayLength" /> and the <see cref="UnaryExpression.Operand" /> property equal to
        ///     <paramref name="array" />.
        /// </returns>
        /// <param name="array">An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="array" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="array" />.Type does not represent a single-dimensional, zero-based array type.
        /// </exception>
        public static UnaryExpression ArrayLength(Expression array)
        {
            ContractUtils.RequiresNotNull(array, nameof(array));
            ExpressionUtils.RequiresCanRead(array, nameof(array));
            if (array.Type.IsSafeArray())
            {
                return new UnaryExpression(ExpressionType.ArrayLength, array, typeof(int), method: null);
            }

            if (!array.Type.IsArray || !typeof(Array).IsAssignableFrom(array.Type))
            {
                throw new ArgumentException("Argument must be array", nameof(array));
            }

            throw new ArgumentException("Argument must be single dimensional array type", nameof(array));
        }

        /// <summary>Creates a <see cref="UnaryExpression" /> that represents a conversion operation.</summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Convert" /> and the <see cref="UnaryExpression.Operand" /> and <see cref="Type" />
        ///     properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="type">A <see cref="System.Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> or <paramref name="type" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     No conversion operator is defined between <paramref name="expression" />
        ///     .Type and <paramref name="type" />.
        /// </exception>
        public static UnaryExpression Convert(Expression expression, Type type)
        {
            return Convert(expression, type, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a conversion operation for which the implementing
        ///     method is specified.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Convert" /> and the <see cref="UnaryExpression.Operand" />, <see cref="Type" />, and
        ///     <see cref="UnaryExpression.Method" /> properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="type">A <see cref="System.Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> or <paramref name="type" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="method" /> is not null and the method it represents returns void, is not static (Shared in Visual
        ///     Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     More than one method that matches the <paramref name="method" /> description
        ///     was found.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     No conversion operator is defined between <paramref name="expression" />
        ///     .Type and <paramref name="type" />.-or-<paramref name="expression" />.Type is not assignable to the argument type
        ///     of the method represented by <paramref name="method" />.-or-The return type of the method represented by
        ///     <paramref name="method" /> is not assignable to <paramref name="type" />.-or-<paramref name="expression" />.Type or
        ///     <paramref name="type" /> is a nullable value type and the corresponding non-nullable value type does not equal the
        ///     argument type or the return type, respectively, of the method represented by <paramref name="method" />.
        /// </exception>
        public static UnaryExpression Convert(Expression expression, Type type, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type));
            if (method != null)
            {
                return GetMethodBasedCoercionOperator(ExpressionType.Convert, expression, type, method);
            }

            if (expression.Type.HasIdentityPrimitiveOrNullableConversionToInternal(type) || expression.Type.HasReferenceConversionToInternal(type))
            {
                return new UnaryExpression(ExpressionType.Convert, expression, type, method: null);
            }

            return GetUserDefinedCoercionOrThrow(ExpressionType.Convert, expression, type);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a conversion operation that throws an exception if
        ///     the target type is overflowed.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.ConvertChecked" /> and the <see cref="UnaryExpression.Operand" /> and <see cref="Type" />
        ///     properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="type">A <see cref="System.Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> or <paramref name="type" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     No conversion operator is defined between <paramref name="expression" />
        ///     .Type and <paramref name="type" />.
        /// </exception>
        public static UnaryExpression ConvertChecked(Expression expression, Type type)
        {
            return ConvertChecked(expression, type, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a conversion operation that throws an exception if
        ///     the target type is overflowed and for which the implementing method is specified.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.ConvertChecked" /> and the <see cref="UnaryExpression.Operand" />, <see cref="Type" />,
        ///     and <see cref="UnaryExpression.Method" /> properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="type">A <see cref="System.Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> or <paramref name="type" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="method" /> is not null and the method it represents returns void, is not static (Shared in Visual
        ///     Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="AmbiguousMatchException">
        ///     More than one method that matches the <paramref name="method" /> description
        ///     was found.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     No conversion operator is defined between <paramref name="expression" />
        ///     .Type and <paramref name="type" />.-or-<paramref name="expression" />.Type is not assignable to the argument type
        ///     of the method represented by <paramref name="method" />.-or-The return type of the method represented by
        ///     <paramref name="method" /> is not assignable to <paramref name="type" />.-or-<paramref name="expression" />.Type or
        ///     <paramref name="type" /> is a nullable value type and the corresponding non-nullable value type does not equal the
        ///     argument type or the return type, respectively, of the method represented by <paramref name="method" />.
        /// </exception>
        public static UnaryExpression ConvertChecked(Expression expression, Type type, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type));
            if (method != null)
            {
                return GetMethodBasedCoercionOperator(ExpressionType.ConvertChecked, expression, type, method);
            }

            if (expression.Type.HasIdentityPrimitiveOrNullableConversionToInternal(type))
            {
                return new UnaryExpression(ExpressionType.ConvertChecked, expression, type, method: null);
            }

            return expression.Type.HasReferenceConversionToInternal(type)
                ? new UnaryExpression(ExpressionType.Convert, expression, type, method: null)
                : GetUserDefinedCoercionOrThrow(ExpressionType.ConvertChecked, expression, type);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the decrementing of the expression by 1.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to decrement.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the decremented expression.</returns>
        public static UnaryExpression Decrement(Expression expression)
        {
            return Decrement(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the decrementing of the expression by 1.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to decrement.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the decremented expression.</returns>
        public static UnaryExpression Decrement(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.Decrement, expression, method);
            }

            return expression.Type.IsArithmetic()
                ? new UnaryExpression(ExpressionType.Decrement, expression, expression.Type, method: null)
                : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Decrement, "op_Decrement", expression);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the incrementing of the expression by 1.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to increment.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the incremented expression.</returns>
        public static UnaryExpression Increment(Expression expression)
        {
            return Increment(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the incrementing of the expression by 1.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to increment.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the incremented expression.</returns>
        public static UnaryExpression Increment(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.Increment, expression, method);
            }

            return expression.Type.IsArithmetic()
                ? new UnaryExpression(ExpressionType.Increment, expression, expression.Type, method: null)
                : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Increment, "op_Increment", expression);
        }

        /// <summary>
        ///     Returns whether the expression evaluates to false.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to evaluate.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression IsFalse(Expression expression)
        {
            return IsFalse(expression, method: null);
        }

        /// <summary>
        ///     Returns whether the expression evaluates to false.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to evaluate.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression IsFalse(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.IsFalse, expression, method);
            }

            return expression.Type.IsBool()
                ? new UnaryExpression(ExpressionType.IsFalse, expression, expression.Type, method: null)
                : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsFalse, "op_False", expression);
        }

        /// <summary>
        ///     Returns whether the expression evaluates to true.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to evaluate.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression IsTrue(Expression expression)
        {
            return IsTrue(expression, method: null);
        }

        /// <summary>
        ///     Returns whether the expression evaluates to true.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to evaluate.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression IsTrue(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.IsTrue, expression, method);
            }

            return expression.Type.IsBool()
                ? new UnaryExpression(ExpressionType.IsTrue, expression, expression.Type, method: null)
                : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.IsTrue, "op_True", expression);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" />, given an operand, by calling the appropriate factory method.
        /// </summary>
        /// <param name="unaryType">The <see cref="ExpressionType" /> that specifies the type of unary operation.</param>
        /// <param name="operand">An <see cref="Expression" /> that represents the operand.</param>
        /// <param name="type">The <see cref="Type" /> that specifies the type to be converted to (pass null if not applicable).</param>
        /// <returns>The <see cref="UnaryExpression" /> that results from calling the appropriate factory method.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="unaryType" /> does not correspond to a unary
        ///     expression.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operand" /> is null.</exception>
        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression operand, Type type)
        {
            return MakeUnary(unaryType, operand, type, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" />, given an operand and implementing method, by calling the appropriate
        ///     factory method.
        /// </summary>
        /// <param name="unaryType">The <see cref="ExpressionType" /> that specifies the type of unary operation.</param>
        /// <param name="operand">An <see cref="Expression" /> that represents the operand.</param>
        /// <param name="type">The <see cref="Type" /> that specifies the type to be converted to (pass null if not applicable).</param>
        /// <param name="method">The <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>The <see cref="UnaryExpression" /> that results from calling the appropriate factory method.</returns>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="unaryType" /> does not correspond to a unary
        ///     expression.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="operand" /> is null.</exception>
        public static UnaryExpression MakeUnary(ExpressionType unaryType, Expression? operand, Type type, MethodInfo? method)
        {
            switch (unaryType)
            {
                case ExpressionType.Negate:
                    return Negate(operand!, method);

                case ExpressionType.NegateChecked:
                    return NegateChecked(operand!, method);

                case ExpressionType.Not:
                    return Not(operand!, method);

                case ExpressionType.IsFalse:
                    return IsFalse(operand!, method);

                case ExpressionType.IsTrue:
                    return IsTrue(operand!, method);

                case ExpressionType.OnesComplement:
                    return OnesComplement(operand!, method);

                case ExpressionType.ArrayLength:
                    return ArrayLength(operand!);

                case ExpressionType.Convert:
                    return Convert(operand!, type, method);

                case ExpressionType.ConvertChecked:
                    return ConvertChecked(operand!, type, method);

                case ExpressionType.Throw:
                    return Throw(operand, type);

                case ExpressionType.TypeAs:
                    return TypeAs(operand!, type);

                case ExpressionType.Quote:
                    return Quote(operand!);

                case ExpressionType.UnaryPlus:
                    return UnaryPlus(operand!, method);

                case ExpressionType.Unbox:
                    return Unbox(operand!, type);

                case ExpressionType.Increment:
                    return Increment(operand!, method);

                case ExpressionType.Decrement:
                    return Decrement(operand!, method);

                case ExpressionType.PreIncrementAssign:
                    return PreIncrementAssign(operand!, method);

                case ExpressionType.PostIncrementAssign:
                    return PostIncrementAssign(operand!, method);

                case ExpressionType.PreDecrementAssign:
                    return PreDecrementAssign(operand!, method);

                case ExpressionType.PostDecrementAssign:
                    return PostDecrementAssign(operand!, method);

                default:
                    throw new ArgumentException($"Unhandled unary: {unaryType}", nameof(unaryType));
            }
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an arithmetic negation operation.
        /// </summary>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Negate" /> and the <see cref="UnaryExpression.Operand" /> properties set to the specified
        ///     value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the unary minus operator is not defined for
        ///     <paramref name="expression" />.Type.
        /// </exception>
        public static UnaryExpression Negate(Expression expression)
        {
            return Negate(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an arithmetic negation operation.
        /// </summary>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Negate" /> and the <see cref="UnaryExpression.Operand" /> and
        ///     <see cref="UnaryExpression.Method" /> properties set to the specified value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression" /> is null.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="method" /> is not null and the method it represents
        ///     returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <paramref name="method" /> is null and the unary minus operator
        ///     is not defined for <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable
        ///     value type) is not assignable to the argument type of the method represented by method.
        /// </exception>
        public static UnaryExpression Negate(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.Negate, expression, method);
            }

            if (expression.Type.IsArithmetic() && !expression.Type.IsUnsignedInteger())
            {
                return new UnaryExpression(ExpressionType.Negate, expression, expression.Type, method: null);
            }

            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Negate, "op_UnaryNegation", expression);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an arithmetic negation operation that has overflow
        ///     checking.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.NegateChecked" /> and the <see cref="UnaryExpression.Operand" /> property set to the
        ///     specified value.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the unary minus operator is not defined for
        ///     <paramref name="expression" />.Type.
        /// </exception>
        public static UnaryExpression NegateChecked(Expression expression)
        {
            return NegateChecked(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an arithmetic negation operation that has overflow
        ///     checking. The implementing method can be specified.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.NegateChecked" /> and the <see cref="UnaryExpression.Operand" /> and
        ///     <see cref="UnaryExpression.Method" /> properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="method" /> is not null and the method it represents returns void, is not static (Shared in Visual
        ///     Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="method" /> is null and the unary minus operator is not defined for <paramref name="expression" />
        ///     .Type.-or-<paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value
        ///     type) is not assignable to the argument type of the method represented by <paramref name="method" />.
        /// </exception>
        public static UnaryExpression NegateChecked(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.NegateChecked, expression, method);
            }

            if (expression.Type.IsArithmetic() && !expression.Type.IsUnsignedInteger())
            {
                return new UnaryExpression(ExpressionType.NegateChecked, expression, expression.Type, method: null);
            }

            return GetUserDefinedUnaryOperatorOrThrow(ExpressionType.NegateChecked, "op_UnaryNegation", expression);
        }

        /// <summary>Creates a <see cref="UnaryExpression" /> that represents a bitwise complement operation.</summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Not" /> and the <see cref="UnaryExpression.Operand" /> property set to the specified
        ///     value.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     The unary not operator is not defined for <paramref name="expression" />
        ///     .Type.
        /// </exception>
        public static UnaryExpression Not(Expression expression)
        {
            return Not(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a bitwise complement operation. The implementing
        ///     method can be specified.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Not" /> and the <see cref="UnaryExpression.Operand" /> and
        ///     <see cref="UnaryExpression.Method" /> properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="method" /> is not null and the method it represents returns void, is not static (Shared in Visual
        ///     Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="method" /> is null and the unary not operator is not defined for <paramref name="expression" />
        ///     .Type.-or-<paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable value
        ///     type) is not assignable to the argument type of the method represented by <paramref name="method" />.
        /// </exception>
        public static UnaryExpression Not(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.Not, expression, method);
            }

            if (expression.Type.IsIntegerOrBool())
            {
                return new UnaryExpression(ExpressionType.Not, expression, expression.Type, method: null);
            }

            var u = GetUserDefinedUnaryOperator(ExpressionType.Not, "op_LogicalNot", expression);
            return u ?? GetUserDefinedUnaryOperatorOrThrow(ExpressionType.Not, "op_OnesComplement", expression);
        }

        /// <summary>
        ///     Returns the expression representing the ones complement.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" />.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression OnesComplement(Expression expression)
        {
            return OnesComplement(expression, method: null);
        }

        /// <summary>
        ///     Returns the expression representing the ones complement.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" />.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression OnesComplement(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.OnesComplement, expression, method);
            }

            return expression.Type.IsInteger()
                ? new UnaryExpression(ExpressionType.OnesComplement, expression, expression.Type, method: null)
                : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.OnesComplement, "op_OnesComplement", expression);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the assignment of the expression
        ///     followed by a subsequent decrement by 1 of the original expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PostDecrementAssign(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the assignment of the expression
        ///     followed by a subsequent decrement by 1 of the original expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PostDecrementAssign(Expression expression, MethodInfo? method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PostDecrementAssign, expression, method);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the assignment of the expression
        ///     followed by a subsequent increment by 1 of the original expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PostIncrementAssign(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents the assignment of the expression
        ///     followed by a subsequent increment by 1 of the original expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PostIncrementAssign(Expression expression, MethodInfo? method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PostIncrementAssign, expression, method);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that decrements the expression by 1
        ///     and assigns the result back to the expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PreDecrementAssign(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that decrements the expression by 1
        ///     and assigns the result back to the expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PreDecrementAssign(Expression expression, MethodInfo? method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PreDecrementAssign, expression, method);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that increments the expression by 1
        ///     and assigns the result back to the expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PreIncrementAssign(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that increments the expression by 1
        ///     and assigns the result back to the expression.
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to apply the operations on.</param>
        /// <param name="method">A <see cref="MethodInfo" /> that represents the implementing method.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the resultant expression.</returns>
        public static UnaryExpression PreIncrementAssign(Expression expression, MethodInfo? method)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return MakeOpAssignUnary(ExpressionType.PreIncrementAssign, expression, method);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an expression that has a constant value of type
        ///     <see cref="Expression" />.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Quote" /> and the <see cref="UnaryExpression.Operand" /> property set to the specified
        ///     value.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> is null.
        /// </exception>
        [return: NotNull]
        public static UnaryExpression Quote(Expression expression)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (expression is LambdaExpression lambda)
            {
                return new UnaryExpression(ExpressionType.Quote, lambda, lambda.PublicType, method: null);
            }

            throw new ArgumentException("Quoted expression must be a lambda", nameof(expression));
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a rethrowing of an exception.
        /// </summary>
        /// <returns>A <see cref="UnaryExpression" /> that represents a rethrowing of an exception.</returns>
        public static UnaryExpression Rethrow()
        {
            return Throw(value: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a rethrowing of an exception with a given type.
        /// </summary>
        /// <param name="type">The new <see cref="Type" /> of the expression.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents a rethrowing of an exception.</returns>
        public static UnaryExpression Rethrow(Type type)
        {
            return Throw(value: null, type);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a throwing of an exception.
        /// </summary>
        /// <param name="value">An <see cref="Expression" />.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the exception.</returns>
        public static UnaryExpression Throw(Expression? value)
        {
            return Throw(value, typeof(void));
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a throwing of a value with a given type.
        /// </summary>
        /// <param name="value">An <see cref="Expression" />.</param>
        /// <param name="type">The new <see cref="Type" /> of the expression.</param>
        /// <returns>A <see cref="UnaryExpression" /> that represents the exception.</returns>
        public static UnaryExpression Throw(Expression? value, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type));
            if (value == null)
            {
                return new UnaryExpression(ExpressionType.Throw, expression: null, type, method: null);
            }

            ExpressionUtils.RequiresCanRead(value, nameof(value));
            if (value.Type.IsValueType)
            {
                throw new ArgumentException("Argument must not have a value type.", nameof(value));
            }

            return new UnaryExpression(ExpressionType.Throw, value, type, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents an explicit reference or boxing conversion where null
        ///     is supplied if the conversion fails.
        /// </summary>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.TypeAs" /> and the <see cref="UnaryExpression.Operand" /> and <see cref="Type" />
        ///     properties set to the specified values.
        /// </returns>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="type">A <see cref="System.Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="expression" /> or <paramref name="type" /> is null.
        /// </exception>
        public static UnaryExpression TypeAs(Expression expression, Type type)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type));
            if (type.IsValueType && !type.IsNullable())
            {
                throw new ArgumentException($"The type used in TypeAs Expression must be of reference or nullable type, {type} is neither", nameof(type));
            }

            return new UnaryExpression(ExpressionType.TypeAs, expression, type, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a unary plus operation.
        /// </summary>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.UnaryPlus" /> and the <see cref="UnaryExpression.Operand" /> property set to the
        ///     specified value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression" /> is null.</exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when the unary minus operator is not defined for
        ///     <paramref name="expression" />.Type.
        /// </exception>
        public static UnaryExpression UnaryPlus(Expression expression)
        {
            return UnaryPlus(expression, method: null);
        }

        /// <summary>
        ///     Creates a <see cref="UnaryExpression" /> that represents a unary plus operation.
        /// </summary>
        /// <param name="expression">
        ///     An <see cref="Expression" /> to set the <see cref="UnaryExpression.Operand" /> property equal
        ///     to.
        /// </param>
        /// <param name="method">A <see cref="MethodInfo" /> to set the <see cref="UnaryExpression.Method" /> property equal to.</param>
        /// <returns>
        ///     A <see cref="UnaryExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.UnaryPlus" /> and the <see cref="UnaryExpression.Operand" /> and
        ///     <see cref="UnaryExpression.Method" />property set to the specified value.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression" /> is null.</exception>
        /// <exception cref="ArgumentException">
        ///     Thrown when <paramref name="method" /> is not null and the method it represents
        ///     returns void, is not static (Shared in Visual Basic), or does not take exactly one argument.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///     Thrown when <paramref name="method" /> is null and the unary minus operator
        ///     is not defined for <paramref name="expression" />.Type (or its corresponding non-nullable type if it is a nullable
        ///     value type) is not assignable to the argument type of the method represented by method.
        /// </exception>
        public static UnaryExpression UnaryPlus(Expression expression, MethodInfo? method)
        {
            ContractUtils.RequiresNotNull(expression, nameof(expression));
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            if (method != null)
            {
                return GetMethodBasedUnaryOperator(ExpressionType.UnaryPlus, expression, method);
            }

            return expression.Type.IsArithmetic() ? new UnaryExpression(ExpressionType.UnaryPlus, expression, expression.Type, method: null) : GetUserDefinedUnaryOperatorOrThrow(ExpressionType.UnaryPlus, "op_UnaryPlus", expression);
        }

        /// <summary>
        ///     <summary>Creates a <see cref="UnaryExpression" /> that represents an explicit unboxing.</summary>
        /// </summary>
        /// <param name="expression">An <see cref="Expression" /> to unbox.</param>
        /// <param name="type">The new <see cref="System.Type" /> of the expression.</param>
        /// <returns>An instance of <see cref="UnaryExpression" />.</returns>
        public static UnaryExpression Unbox(Expression expression, Type type)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            if (!expression.Type.IsInterface && expression.Type != typeof(object))
            {
                throw new ArgumentException("Can only unbox from an object or interface type to a value type.", nameof(expression));
            }

            if (!type.IsValueType)
            {
                throw new ArgumentException("Can only unbox from an object or interface type to a value type.", nameof(type));
            }

            TypeUtils.ValidateType(type, nameof(type));
            return new UnaryExpression(ExpressionType.Unbox, expression, type, method: null);
        }

        private static UnaryExpression GetMethodBasedCoercionOperator(ExpressionType unaryType, Expression operand, Type convertToType, MethodInfo method)
        {
            ValidateOperator(method);
            var pms = method.GetParameters();
            if (pms.Length != 1)
            {
                throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{method}'", nameof(method));
            }

            if (ParameterIsAssignable(pms[0], operand.Type) && TypeUtils.AreEquivalent(method.ReturnType, convertToType))
            {
                return new UnaryExpression(unaryType, operand, method.ReturnType, method);
            }

            // check for lifted call
            if
            (
                (operand.Type.IsNullable() || convertToType.IsNullable())
                && ParameterIsAssignable(pms[0], operand.Type.GetNonNullable())
                && (TypeUtils.AreEquivalent(method.ReturnType, convertToType.GetNonNullable())
                    || TypeUtils.AreEquivalent(method.ReturnType, convertToType))
            )
            {
                return new UnaryExpression(unaryType, operand, convertToType, method);
            }

            throw new InvalidOperationException($"The operands for operator '{unaryType}' do not match the parameters of method '{method.Name}'.");
        }

        private static UnaryExpression GetMethodBasedUnaryOperator(ExpressionType unaryType, Expression operand, MethodInfo method)
        {
            ValidateOperator(method);
            var pms = method.GetParameters();
            if (pms.Length != 1)
            {
                throw new ArgumentException($"Incorrect number of arguments supplied for call to method '{method}'", nameof(method));
            }

            if (ParameterIsAssignable(pms[0], operand.Type))
            {
                ValidateParamsWithOperandsOrThrow(pms[0].ParameterType, operand.Type, unaryType, method.Name);
                return new UnaryExpression(unaryType, operand, method.ReturnType, method);
            }

            // check for lifted call
            if
            (
                operand.Type.IsNullable()
                && ParameterIsAssignable(pms[0], operand.Type.GetNonNullable())
                && method.ReturnType.IsValueType && !method.ReturnType.IsNullable()
            )
            {
                return new UnaryExpression(unaryType, operand, method.ReturnType.GetNullable(), method);
            }

            throw new InvalidOperationException($"The operands for operator '{unaryType}' do not match the parameters of method '{method.Name}'.");
        }

        private static UnaryExpression? GetUserDefinedCoercion(ExpressionType coercionType, Expression expression, Type convertToType)
        {
            var method = TypeHelper.GetUserDefinedConversionMethod(expression.Type, convertToType, implicitOnly: false);
            return method != null
                ? new UnaryExpression(coercionType, expression, convertToType, method)
                : null;
        }

        private static UnaryExpression GetUserDefinedCoercionOrThrow(ExpressionType coercionType, Expression expression, Type convertToType)
        {
            var u = GetUserDefinedCoercion(coercionType, expression, convertToType);
            if (u != null)
            {
                return u;
            }

            throw new InvalidOperationException($"No coercion operator is defined between types '{expression.Type}' and '{convertToType}'.");
        }

        private static UnaryExpression? GetUserDefinedUnaryOperator(ExpressionType unaryType, string name, Expression operand)
        {
            var operandType = operand.Type;
            Type[] types = { operandType };
            var nnOperandType = operandType.GetNonNullable();
            var method = nnOperandType.GetStaticMethodInternal(name, types);
            if (method != null)
            {
                return new UnaryExpression(unaryType, operand, method.ReturnType, method);
            }

            // try lifted call
            if (!operandType.IsNullable())
            {
                return null;
            }

            types[0] = nnOperandType;
            method = nnOperandType.GetStaticMethodInternal(name, types);
            if (method?.ReturnType.IsValueType == true && !method.ReturnType.IsNullable())
            {
                return new UnaryExpression(unaryType, operand, method.ReturnType.GetNullable(), method);
            }

            return null;
        }

        private static UnaryExpression GetUserDefinedUnaryOperatorOrThrow(ExpressionType unaryType, string name, Expression operand)
        {
            var u = GetUserDefinedUnaryOperator(unaryType, name, operand);
            if (u == null)
            {
                throw new InvalidOperationException($"The unary operator {unaryType} is not defined for the type '{operand.Type}'.");
            }

            ValidateParamsWithOperandsOrThrow(u.Method!.GetParameters()[0].ParameterType, operand.Type, unaryType, name);
            return u;
        }

        private static UnaryExpression MakeOpAssignUnary(ExpressionType kind, Expression expression, MethodInfo? method)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            RequiresCanWrite(expression, nameof(expression));

            UnaryExpression result;
            if (method == null)
            {
                if (expression.Type.IsArithmetic())
                {
                    return new UnaryExpression(kind, expression, expression.Type, method: null);
                }

                string name;
                if (kind == ExpressionType.PreIncrementAssign || kind == ExpressionType.PostIncrementAssign)
                {
                    name = "op_Increment";
                }
                else
                {
                    Debug.Assert(kind == ExpressionType.PreDecrementAssign || kind == ExpressionType.PostDecrementAssign);
                    name = "op_Decrement";
                }

                result = GetUserDefinedUnaryOperatorOrThrow(kind, name, expression);
            }
            else
            {
                result = GetMethodBasedUnaryOperator(kind, expression, method);
            }

            // return type must be assignable back to the operand type
            if (!expression.Type.IsReferenceAssignableFromInternal(result.Type))
            {
                throw new ArgumentException($"The user-defined operator method '{method?.Name}' for operator '{kind}' must return the same type as its parameter or a derived type.", string.Empty);
            }

            return result;
        }
    }

    /// <summary>
    ///     Represents an expression that has a unary operator.
    /// </summary>
    [DebuggerTypeProxy(typeof(UnaryExpressionProxy))]
    public sealed class UnaryExpression : Expression
    {
        internal UnaryExpression(ExpressionType nodeType, Expression? expression, Type type, MethodInfo? method)
        {
            NodeType = nodeType;
            Operand = expression;
            Method = method;
            Type = type;
        }

        /// <summary>
        ///     Gets a value that indicates whether the expression tree node can be reduced.
        /// </summary>
        public override bool CanReduce
        {
            get
            {
                switch (NodeType)
                {
                    case ExpressionType.PreIncrementAssign:
                    case ExpressionType.PreDecrementAssign:
                    case ExpressionType.PostIncrementAssign:
                    case ExpressionType.PostDecrementAssign:
                        return true;

                    default:
                        return false;
                }
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the expression tree node represents a lifted call to an operator.
        /// </summary>
        /// <returns>true if the node represents a lifted call; otherwise, false.</returns>
        public bool IsLifted
        {
            get
            {
                if (NodeType == ExpressionType.TypeAs || NodeType == ExpressionType.Quote || NodeType == ExpressionType.Throw)
                {
                    return false;
                }

                var operandIsNullable = Operand!.Type.IsNullable();
                var resultIsNullable = Type.IsNullable();
                if (Method != null)
                {
                    return (operandIsNullable && !TypeUtils.AreEquivalent(Method.GetParameters()[0].ParameterType, Operand.Type))
                           || (resultIsNullable && !TypeUtils.AreEquivalent(Method.ReturnType, Type));
                }

                return operandIsNullable || resultIsNullable;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the expression tree node represents a lifted call to an operator whose return
        ///     type is lifted to a nullable type.
        /// </summary>
        /// <returns>true if the operator's return type is lifted to a nullable type; otherwise, false.</returns>
        public bool IsLiftedToNull => IsLifted && Type.IsNullable();

        /// <summary>
        ///     Gets the implementing method for the unary operation.
        /// </summary>
        /// <returns>The <see cref="MethodInfo" /> that represents the implementing method.</returns>
        public MethodInfo? Method { get; }

        /// <summary>
        ///     Returns the node type of this <see cref="Expression" />. (Inherited from
        ///     <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> that represents this expression.</returns>
        public override ExpressionType NodeType { get; }

        /// <summary>
        ///     Gets the operand of the unary operation.
        /// </summary>
        /// <returns> An <see cref="ExpressionType" /> that represents the operand of the unary operation.</returns>
        /// <remarks>Can be null when <see cref="NodeType"/> is <see cref="ExpressionType.Throw"/>. </remarks>
        public Expression? Operand { get; }

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        ///     (Inherited from <see cref="Expression" />.)
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type { get; }

        private bool IsPrefix => NodeType == ExpressionType.PreIncrementAssign || NodeType == ExpressionType.PreDecrementAssign;

        /// <summary>
        ///     Reduces the expression node to a simpler expression.
        ///     If CanReduce returns true, this should return a valid expression.
        ///     This method is allowed to return another node which itself
        ///     must be reduced.
        /// </summary>
        /// <returns>The reduced expression.</returns>
        public override Expression Reduce()
        {
            if (!CanReduce)
            {
                return this;
            }

            switch (Operand!.NodeType)
            {
                case ExpressionType.Index:
                    return ReduceIndex();

                case ExpressionType.MemberAccess:
                    return ReduceMember();

                default:
                    Debug.Assert(Operand.NodeType == ExpressionType.Parameter);
                    return ReduceVariable();
            }
        }

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="operand">The <see cref="Operand" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public UnaryExpression Update(Expression? operand)
        {
            return operand == Operand ? this : MakeUnary(NodeType, operand, Type, Method);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitUnary(this);
        }

        private UnaryExpression FunctionalOp(Expression operand)
        {
            ExpressionType functional;
            if (NodeType == ExpressionType.PreIncrementAssign || NodeType == ExpressionType.PostIncrementAssign)
            {
                functional = ExpressionType.Increment;
            }
            else
            {
                Debug.Assert(NodeType == ExpressionType.PreDecrementAssign || NodeType == ExpressionType.PostDecrementAssign);
                functional = ExpressionType.Decrement;
            }

            return new UnaryExpression(functional, operand, operand.Type, Method);
        }

        private Expression ReduceIndex()
        {
            // left[a0, a1, ... aN] (op)
            //
            // ... is reduced into ...
            //
            // tempObj = left
            // tempArg0 = a0
            // ...
            // tempArgN = aN
            // tempValue = tempObj[tempArg0, ... tempArgN]
            // tempObj[tempArg0, ... tempArgN] = op(tempValue)
            // tempValue

            var prefix = IsPrefix;
            var index = (IndexExpression)Operand!;
            var count = index.ArgumentCount;
            var block = new Expression[count + (prefix ? 2 : 4)];
            var temps = new ParameterExpression[count + (prefix ? 1 : 2)];
            var args = new Expression[count];

            var i = 0;
            temps[i] = Parameter(index.Object!.Type, name: null);
            block[i] = Assign(temps[i], index.Object);
            i++;
            while (i <= count)
            {
                var arg = index.GetArgument(i - 1);
                args[i - 1] = temps[i] = Parameter(arg.Type, name: null);
                block[i] = Assign(temps[i], arg);
                i++;
            }

            index = MakeIndex(temps[0], index.Indexer, ReadOnlyCollectionEx.Create(args));
            if (!prefix)
            {
                var lastTemp = temps[i] = Parameter(index.Type, name: null);
                block[i] = Assign(temps[i], index);
                i++;
                Debug.Assert(i == temps.Length);
                block[i++] = Assign(index, FunctionalOp(lastTemp));
                block[i] = lastTemp;
            }
            else
            {
                Debug.Assert(i == temps.Length);
                block[i] = Assign(index, FunctionalOp(index));
            }

            Debug.Assert(i == block.Length);
            return Block(ReadOnlyCollectionEx.Create(temps), ReadOnlyCollectionEx.Create(block));
        }

        private Expression ReduceMember()
        {
            var member = (MemberExpression)Operand!;
            if (member.Expression == null)
            {
                //static member, reduce the same as variable
                return ReduceVariable();
            }

            var temp1 = Parameter(member.Expression.Type, name: null);
            var initTemp1 = Assign(temp1, member.Expression);
            member = MakeMemberAccess(temp1, member.Member);

            if (IsPrefix)
            {
                // (op) value.member
                // ... is reduced into ...
                // temp1 = value
                // temp1.member = op(temp1.member)
                return Block
                (
                    ReadOnlyCollectionEx.Create(temp1),
                    ReadOnlyCollectionEx.Create<Expression>
                    (
                        initTemp1,
                        Assign(member, FunctionalOp(member))
                    )
                );
            }

            // value.member (op)
            // ... is reduced into ...
            // temp1 = value
            // temp2 = temp1.member
            // temp1.member = op(temp2)
            // temp2
            var temp2 = Parameter(member.Type, name: null);
            return Block
            (
                ReadOnlyCollectionEx.Create(temp1, temp2),
                ReadOnlyCollectionEx.Create<Expression>
                (
                    initTemp1,
                    Assign(temp2, member),
                    Assign(member, FunctionalOp(temp2)),
                    temp2
                )
            );
        }

        private Expression ReduceVariable()
        {
            if (IsPrefix)
            {
                // (op) var
                // ... is reduced into ...
                // var = op(var)
                return Assign(Operand!, FunctionalOp(Operand!));
            }

            // var (op)
            // ... is reduced into ...
            // temp = var
            // var = op(var)
            // temp
            var temp = Parameter(Operand!.Type, name: null);
            return Block
            (
                ReadOnlyCollectionEx.Create(temp),
                ReadOnlyCollectionEx.Create<Expression>(Assign(temp, Operand), Assign(Operand, FunctionalOp(temp)), temp)
            );
        }
    }
}

#endif