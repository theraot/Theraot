#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Runtime.CompilerServices;
using Theraot.Reflection;
using static System.Linq.Expressions.CachedReflectionInfo;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        /// Creates a <see cref="TypeBinaryExpression"/> that compares run-time type identity.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> to set the <see cref="Expression"/> property equal to.</param>
        /// <param name="type">A <see cref="Type"/> to set the <see cref="TypeBinaryExpression.TypeOperand"/> property equal to.</param>
        /// <returns>A <see cref="TypeBinaryExpression"/> for which the <see cref="NodeType"/> property is equal to <see cref="ExpressionType.TypeEqual"/> and for which the <see cref="TypeBinaryExpression.Expression"/> and <see cref="TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.</returns>
        public static TypeBinaryExpression TypeEqual(Expression expression, Type type)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            if (type.IsByRef)
            {
                throw Error.TypeMustNotBeByRef(nameof(type));
            }

            return new TypeBinaryExpression(expression, type, ExpressionType.TypeEqual);
        }

        /// <summary>
        /// Creates a <see cref="TypeBinaryExpression"/>.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> to set the <see cref="Expression"/> property equal to.</param>
        /// <param name="type">A <see cref="Type"/> to set the <see cref="TypeBinaryExpression.TypeOperand"/> property equal to.</param>
        /// <returns>A <see cref="TypeBinaryExpression"/> for which the <see cref="NodeType"/> property is equal to <see cref="ExpressionType.TypeIs"/> and for which the <see cref="TypeBinaryExpression.Expression"/> and <see cref="TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.</returns>
        public static TypeBinaryExpression TypeIs(Expression expression, Type type)
        {
            ExpressionUtils.RequiresCanRead(expression, nameof(expression));
            ContractUtils.RequiresNotNull(type, nameof(type));
            if (type.IsByRef)
            {
                throw Error.TypeMustNotBeByRef(nameof(type));
            }

            return new TypeBinaryExpression(expression, type, ExpressionType.TypeIs);
        }
    }

    /// <summary>
    /// Represents an operation between an expression and a type.
    /// </summary>
    [DebuggerTypeProxy(typeof(TypeBinaryExpressionProxy))]
    public sealed class TypeBinaryExpression : Expression
    {
        internal TypeBinaryExpression(Expression expression, Type typeOperand, ExpressionType nodeType)
        {
            Expression = expression;
            TypeOperand = typeOperand;
            NodeType = nodeType;
        }

        /// <summary>
        /// Gets the expression operand of a type test operation.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public override ExpressionType NodeType { get; }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression"/> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type"/> that represents the static type of the expression.</returns>
        public override Type Type => typeof(bool);

        /// <summary>
        /// Gets the type operand of a type test operation.
        /// </summary>
        public Type TypeOperand { get; }

        #region Reduce TypeEqual

        internal Expression ReduceTypeEqual()
        {
            Type cType = Expression.Type;

            if (cType.IsValueType || TypeOperand.IsPointer)
            {
                if (cType.IsNullableType())
                {
                    // If the expression type is a nullable type, it will match if
                    // the value is not null and the type operand
                    // either matches or is its type argument (T to its T?).
                    if (cType.GetNonNullableType() != TypeOperand.GetNonNullableType())
                    {
                        return Block(Expression, Utils.Constant(value: false));
                    }

                    return NotEqual(Expression, Constant(null, Expression.Type));
                }

                // For other value types (including Void), we can
                // determine the result now
                return Block(Expression, Utils.Constant(cType == TypeOperand.GetNonNullableType()));
            }

            Debug.Assert(TypeUtils.AreReferenceAssignable(typeof(object), Expression.Type), "Expecting reference types only after this point.");

            // Can check the value right now for constants.
            if (Expression.NodeType == ExpressionType.Constant)
            {
                return ReduceConstantTypeEqual();
            }

            // expression is a ByVal parameter. Can safely reevaluate.
            if (Expression is ParameterExpression parameter && !parameter.IsByRef)
            {
                return ByValParameterTypeEqual(parameter);
            }

            // Create a temp so we only evaluate the left side once
            parameter = Parameter(typeof(object));

            return Block(
                new TrueReadOnlyCollection<ParameterExpression>(parameter),
                new TrueReadOnlyCollection<Expression>(
                    Assign(parameter, Expression),
                    ByValParameterTypeEqual(parameter)
                )
            );
        }

        // Helper that is used when re-eval of LHS is safe.
        private Expression ByValParameterTypeEqual(ParameterExpression value)
        {
            Expression getType = Call(value, ObjectGetType);

            // In remoting scenarios, obj.GetType() can return an interface.
            // But JIT32's optimized "obj.GetType() == typeof(ISomething)" codegen,
            // causing it to always return false.
            // We workaround this optimization by generating different, less optimal IL
            // if TypeOperand is an interface.
            if (TypeOperand.IsInterface)
            {
                ParameterExpression temp = Parameter(typeof(Type));
                getType = Block(
                    new TrueReadOnlyCollection<ParameterExpression>(temp),
                    new TrueReadOnlyCollection<Expression>(
                        Assign(temp, getType),
                        temp
                    )
                );
            }

            // We use reference equality when comparing to null for correctness
            // (don't invoke a user defined operator), and reference equality
            // on types for performance (so the JIT can optimize the IL).
            return AndAlso(
                ReferenceNotEqual(value, Utils.Null),
                ReferenceEqual(
                    getType,
                    Constant(TypeOperand.GetNonNullableType(), typeof(Type))
                )
            );
        }

        private Expression ReduceConstantTypeEqual()
        {
            ConstantExpression ce = Expression as ConstantExpression;
            //TypeEqual(null, T) always returns false.
            // ReSharper disable once PossibleNullReferenceException
            if (ce.Value == null)
            {
                return Utils.Constant(value: false);
            }

            return Utils.Constant(TypeOperand.GetNonNullableType() == ce.Value.GetType());
        }

        #endregion Reduce TypeEqual

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression"/> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public TypeBinaryExpression Update(Expression expression)
        {
            if (expression == Expression)
            {
                return this;
            }
            if (NodeType == ExpressionType.TypeIs)
            {
                return TypeIs(expression, TypeOperand);
            }
            return TypeEqual(expression, TypeOperand);
        }

        /// <summary>
        /// Dispatches to the specific visit method for this node type.
        /// </summary>
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitTypeBinary(this);
        }
    }
}

#endif