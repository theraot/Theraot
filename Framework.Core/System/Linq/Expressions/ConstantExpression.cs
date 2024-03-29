﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions
{
    /// <summary>
    ///     Represents an expression that has a constant value.
    /// </summary>
    [DebuggerTypeProxy(typeof(ConstantExpressionProxy))]
    public class ConstantExpression : Expression
    {
        internal ConstantExpression(object? value)
        {
            Value = value;
        }

        /// <summary>
        ///     Returns the node type of this Expression. Extension nodes should return
        ///     ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> of the expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Constant;

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => Value?.GetType() ?? typeof(object);

        /// <summary>
        ///     Gets the value of the constant expression.
        /// </summary>
        public object? Value { get; }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitConstant(this);
        }
    }

    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="ConstantExpression" /> that has the <see cref="ConstantExpression.Value" /> property set to
        ///     the specified value. .
        /// </summary>
        /// <param name="value">An <see cref="object" /> to set the <see cref="ConstantExpression.Value" /> property equal to.</param>
        /// <returns>
        ///     A <see cref="ConstantExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Constant" /> and the <see cref="ConstantExpression.Value" /> property set to the
        ///     specified value.
        /// </returns>
        public static ConstantExpression Constant(object? value)
        {
            return new ConstantExpression(value);
        }

        /// <summary>
        ///     Creates a <see cref="ConstantExpression" /> that has the <see cref="ConstantExpression.Value" />
        ///     and <see cref="ConstantExpression.Type" /> properties set to the specified values. .
        /// </summary>
        /// <param name="value">An <see cref="object" /> to set the <see cref="ConstantExpression.Value" /> property equal to.</param>
        /// <param name="type">A <see cref="Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <returns>
        ///     A <see cref="ConstantExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Constant" /> and the <see cref="ConstantExpression.Value" /> and
        ///     <see cref="Type" /> properties set to the specified values.
        /// </returns>
        public static ConstantExpression Constant(object? value, Type type)
        {
            ContractUtils.RequiresNotNull(type, nameof(type));
            TypeUtils.ValidateType(type, nameof(type));
            if (value == null)
            {
                if (type == typeof(object))
                {
                    return new ConstantExpression(value: null);
                }

                if (!type.IsValueType || type.IsNullable())
                {
                    return new TypedConstantExpression(value: null, type);
                }
            }
            else
            {
                var valueType = value.GetType();
                if (type == valueType)
                {
                    return new ConstantExpression(value);
                }

                if (type.IsAssignableFrom(valueType))
                {
                    return new TypedConstantExpression(value, type);
                }
            }

            throw new ArgumentException("Argument types do not match", nameof(type));
        }
    }

    internal class TypedConstantExpression : ConstantExpression
    {
        internal TypedConstantExpression(object? value, Type type)
            : base(value)
        {
            Type = type;
        }

        public sealed override Type Type { get; }
    }
}

#endif