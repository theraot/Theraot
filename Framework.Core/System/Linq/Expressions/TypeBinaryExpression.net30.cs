#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Core;

namespace System.Linq.Expressions
{
    /// <summary>
    /// Represents an operation between an expression and a type.
    /// </summary>
    [DebuggerTypeProxy(typeof(TypeBinaryExpressionProxy))]
    public sealed class TypeBinaryExpression : Expression
    {
        private readonly Expression _expression;
        private readonly Type _typeOperand;
        private readonly ExpressionType _nodeKind;

        internal TypeBinaryExpression(Expression expression, Type typeOperand, ExpressionType nodeKind)
        {
            _expression = expression;
            _typeOperand = typeOperand;
            _nodeKind = nodeKind;
        }

        /// <summary>
        /// Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="Type"/> that represents the static type of the expression.</returns>
        public override Type Type
        {
            get { return typeof(bool); }
        }

        /// <summary>
        /// Returns the node type of this Expression. Extension nodes should return
        /// ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType"/> of the expression.</returns>
        public override ExpressionType NodeType
        {
            get { return _nodeKind; }
        }

        /// <summary>
        /// Gets the expression operand of a type test operation.
        /// </summary>
        public Expression Expression
        {
            get { return _expression; }
        }

        /// <summary>
        /// Gets the type operand of a type test operation.
        /// </summary>
        public Type TypeOperand
        {
            get { return _typeOperand; }
        }

        #region Reduce TypeEqual

        internal Expression ReduceTypeEqual()
        {
            var cType = Expression.Type;

            // For value types (including Void, but not nullables), we can
            // determine the result now
            if (cType.IsValueType && !cType.IsNullable())
            {
                return Block(Expression, Constant(cType == _typeOperand.GetNonNullableType()));
            }

            // Can check the value right now for constants.
            if (Expression.NodeType == ExpressionType.Constant)
            {
                return ReduceConstantTypeEqual();
            }

            // If the operand type is a sealed reference type or a nullable
            // type, it will match if value is not null
            if (cType.IsSealed && (cType == _typeOperand))
            {
                if (cType.IsNullable())
                {
                    return NotEqual(Expression, Constant(null, Expression.Type));
                }
                return ReferenceNotEqual(Expression, Constant(null, Expression.Type));
            }

            // expression is a ByVal parameter. Can safely reevaluate.
            var parameter = Expression as ParameterExpression;
            if (parameter != null && !parameter.IsByRef)
            {
                return ByValParameterTypeEqual(parameter);
            }

            // Create a temp so we only evaluate the left side once
            parameter = Parameter(typeof(object));

            // Convert to object if necessary
            var expression = Expression;
            if (!TypeHelper.AreReferenceAssignable(typeof(object), expression.Type))
            {
                expression = Convert(expression, typeof(object));
            }

            return Block(
                new[] { parameter },
                Assign(parameter, expression),
                ByValParameterTypeEqual(parameter)
            );
        }

        // Helper that is used when re-eval of LHS is safe.
        private Expression ByValParameterTypeEqual(ParameterExpression value)
        {
            Expression getType = Call(value, typeof(object).GetMethod("GetType"));

            // In remoting scenarios, obj.GetType() can return an interface.
            // But JIT32's optimized "obj.GetType() == typeof(ISomething)" codegen,
            // causing it to always return false.
            // We workaround this optimization by generating different, less optimal IL
            // if TypeOperand is an interface.
            if (_typeOperand.IsInterface)
            {
                var temp = Parameter(typeof(Type));
                getType = Block(new[] { temp }, Assign(temp, getType), temp);
            }

            // We use reference equality when comparing to null for correctness
            // (don't invoke a user defined operator), and reference equality
            // on types for performance (so the JIT can optimize the IL).
            return AndAlso(
                ReferenceNotEqual(value, Constant(null)),
                ReferenceEqual(
                    getType,
                    Constant(_typeOperand.GetNonNullableType(), typeof(Type))
                )
            );
        }

        private Expression ReduceConstantTypeEqual()
        {
            var ce = Expression as ConstantExpression;
            //TypeEqual(null, T) always returns false.
            if (ce.Value == null)
            {
                return Constant(false);
            }
            return Constant(_typeOperand.GetNonNullableType() == ce.Value.GetType());
        }

        #endregion Reduce TypeEqual

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitTypeBinary(this);
        }

        /// <summary>
        /// Creates a new expression that is like this one, but using the
        /// supplied children. If all of the children are the same, it will
        /// return this expression.
        /// </summary>
        /// <param name="expression">The <see cref="Expression" /> property of the result.</param>
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
    }

    public partial class Expression
    {
        /// <summary>
        /// Creates a <see cref="TypeBinaryExpression"/>.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> to set the <see cref="Expression"/> property equal to.</param>
        /// <param name="type">A <see cref="Type"/> to set the <see cref="TypeBinaryExpression.TypeOperand"/> property equal to.</param>
        /// <returns>A <see cref="TypeBinaryExpression"/> for which the <see cref="NodeType"/> property is equal to <see cref="TypeIs"/> and for which the <see cref="Expression"/> and <see cref="TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.</returns>
        public static TypeBinaryExpression TypeIs(Expression expression, Type type)
        {
            RequiresCanRead(expression, "expression");
            ContractUtils.RequiresNotNull(type, "type");
            if (type.IsByRef)
            {
                throw Error.TypeMustNotBeByRef();
            }

            return new TypeBinaryExpression(expression, type, ExpressionType.TypeIs);
        }

        /// <summary>
        /// Creates a <see cref="TypeBinaryExpression"/> that compares run-time type identity.
        /// </summary>
        /// <param name="expression">An <see cref="Expression"/> to set the <see cref="Expression"/> property equal to.</param>
        /// <param name="type">A <see cref="Type"/> to set the <see cref="TypeBinaryExpression.TypeOperand"/> property equal to.</param>
        /// <returns>A <see cref="TypeBinaryExpression"/> for which the <see cref="NodeType"/> property is equal to <see cref="TypeEqual"/> and for which the <see cref="Expression"/> and <see cref="TypeBinaryExpression.TypeOperand"/> properties are set to the specified values.</returns>
        public static TypeBinaryExpression TypeEqual(Expression expression, Type type)
        {
            RequiresCanRead(expression, "expression");
            ContractUtils.RequiresNotNull(type, "type");
            if (type.IsByRef)
            {
                throw Error.TypeMustNotBeByRef();
            }

            return new TypeBinaryExpression(expression, type, ExpressionType.TypeEqual);
        }
    }
}

#endif