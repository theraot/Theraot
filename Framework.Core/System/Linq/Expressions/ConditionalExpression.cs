#if LESSTHAN_NET35

#pragma warning disable CA1062 // Validate arguments of public methods

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic.Utils;
using AstUtils = System.Linq.Expressions.Utils;

namespace System.Linq.Expressions
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents an expression that has a conditional operator.
    /// </summary>
    [DebuggerTypeProxy(typeof(ConditionalExpressionProxy))]
    public class ConditionalExpression : Expression
    {
        internal ConditionalExpression(Expression test, Expression ifTrue)
        {
            Test = test;
            IfTrue = ifTrue;
        }

        /// <summary>
        ///     Gets the expression to execute if the test evaluates to false.
        /// </summary>
        public Expression IfFalse => GetFalse();

        /// <summary>
        ///     Gets the expression to execute if the test evaluates to true.
        /// </summary>
        public Expression IfTrue { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Returns the node type of this Expression. Extension nodes should return
        ///     ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="System.Linq.Expressions.ExpressionType" /> of the expression.</returns>
        public sealed override ExpressionType NodeType => ExpressionType.Conditional;

        /// <summary>
        ///     Gets the test of the conditional operation.
        /// </summary>
        public Expression Test { get; }

        /// <inheritdoc />
        /// <summary>
        ///     Gets the static type of the expression that this <see cref="System.Linq.Expressions.Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => IfTrue.Type;

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="test">The <see cref="Test" /> property of the result.</param>
        /// <param name="ifTrue">The <see cref="IfTrue" /> property of the result.</param>
        /// <param name="ifFalse">The <see cref="IfFalse" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public ConditionalExpression Update(Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (test == Test && ifTrue == IfTrue && ifFalse == IfFalse)
            {
                return this;
            }

            return Condition(test, ifTrue, ifFalse, Type);
        }

        [return: NotNull]
        internal static ConditionalExpression Make(Expression test, Expression ifTrue, Expression ifFalse, Type type)
        {
            if (ifTrue.Type != type || ifFalse.Type != type)
            {
                return new FullConditionalExpressionWithType(test, ifTrue, ifFalse, type);
            }

            if (ifFalse is DefaultExpression && ifFalse.Type == typeof(void))
            {
                return new ConditionalExpression(test, ifTrue);
            }

            return new FullConditionalExpression(test, ifTrue, ifFalse);
        }

        [return: NotNull]
        internal virtual Expression GetFalse()
        {
            // Using a singleton here to ensure a stable object identity for IfFalse, which Update relies on.
            return AstUtils.Empty;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitConditional(this);
        }
    }

    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="test">An <see cref="Expression" /> to set the <see cref="ConditionalExpression.Test" /> property equal to.</param>
        /// <param name="ifTrue">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfTrue" /> property equal
        ///     to.
        /// </param>
        /// <param name="ifFalse">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfFalse" /> property
        ///     equal to.
        /// </param>
        /// <returns>
        ///     A <see cref="ConditionalExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Conditional" /> and the <see cref="ConditionalExpression.Test" />,
        ///     <see cref="ConditionalExpression.IfTrue" />,
        ///     and <see cref="ConditionalExpression.IfFalse" /> properties set to the specified values.
        /// </returns>
        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse)
        {
            ExpressionUtils.RequiresCanRead(test, nameof(test));
            ExpressionUtils.RequiresCanRead(ifTrue, nameof(ifTrue));
            ExpressionUtils.RequiresCanRead(ifFalse, nameof(ifFalse));

            if (test.Type != typeof(bool))
            {
                throw new ArgumentException("Argument must be boolean", nameof(test));
            }

            if (!TypeUtils.AreEquivalent(ifTrue.Type, ifFalse.Type))
            {
                throw new ArgumentException("Argument types do not match");
            }

            return ConditionalExpression.Make(test, ifTrue, ifFalse, ifTrue.Type);
        }

        /// <summary>
        ///     Creates a <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="test">An <see cref="Expression" /> to set the <see cref="ConditionalExpression.Test" /> property equal to.</param>
        /// <param name="ifTrue">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfTrue" /> property equal
        ///     to.
        /// </param>
        /// <param name="ifFalse">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfFalse" /> property
        ///     equal to.
        /// </param>
        /// <param name="type">A <see cref="Type" /> to set the <see cref="Type" /> property equal to.</param>
        /// <returns>
        ///     A <see cref="ConditionalExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Conditional" /> and the <see cref="ConditionalExpression.Test" />,
        ///     <see cref="ConditionalExpression.IfTrue" />,
        ///     and <see cref="ConditionalExpression.IfFalse" /> properties set to the specified values.
        /// </returns>
        /// <remarks>
        ///     This method allows explicitly unifying the result type of the conditional expression in cases where the types of
        ///     <paramref name="ifTrue" />
        ///     and <paramref name="ifFalse" /> expressions are not equal. Types of both <paramref name="ifTrue" /> and
        ///     <paramref name="ifFalse" /> must be implicitly
        ///     reference assignable to the result type. The <paramref name="type" /> is allowed to be <see cref="System.Void" />.
        /// </remarks>
        public static ConditionalExpression Condition(Expression test, Expression ifTrue, Expression ifFalse, Type type)
        {
            ExpressionUtils.RequiresCanRead(test, nameof(test));
            ExpressionUtils.RequiresCanRead(ifTrue, nameof(ifTrue));
            ExpressionUtils.RequiresCanRead(ifFalse, nameof(ifFalse));
            ContractUtils.RequiresNotNull(type, nameof(type));

            if (test.Type != typeof(bool))
            {
                throw new ArgumentException("Argument must be boolean", nameof(test));
            }

            if (type != typeof(void) && (!type.IsReferenceAssignableFromInternal(ifTrue.Type) || !type.IsReferenceAssignableFromInternal(ifFalse.Type)))
            {
                throw new ArgumentException("Argument types do not match");
            }

            return ConditionalExpression.Make(test, ifTrue, ifFalse, type);
        }

        /// <summary>
        ///     Creates a <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="test">An <see cref="Expression" /> to set the <see cref="ConditionalExpression.Test" /> property equal to.</param>
        /// <param name="ifTrue">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfTrue" /> property equal
        ///     to.
        /// </param>
        /// <returns>
        ///     A <see cref="ConditionalExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Conditional" /> and the <see cref="ConditionalExpression.Test" />,
        ///     <see cref="ConditionalExpression.IfTrue" />,
        ///     properties set to the specified values. The <see cref="ConditionalExpression.IfFalse" /> property is set to default
        ///     expression and
        ///     the type of the resulting <see cref="ConditionalExpression" /> returned by this method is
        ///     <see cref="System.Void" />.
        /// </returns>
        public static ConditionalExpression IfThen(Expression test, Expression ifTrue)
        {
            return Condition(test, ifTrue, Empty(), typeof(void));
        }

        /// <summary>
        ///     Creates a <see cref="ConditionalExpression" />.
        /// </summary>
        /// <param name="test">An <see cref="Expression" /> to set the <see cref="ConditionalExpression.Test" /> property equal to.</param>
        /// <param name="ifTrue">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfTrue" /> property equal
        ///     to.
        /// </param>
        /// <param name="ifFalse">
        ///     An <see cref="Expression" /> to set the <see cref="ConditionalExpression.IfFalse" /> property
        ///     equal to.
        /// </param>
        /// <returns>
        ///     A <see cref="ConditionalExpression" /> that has the <see cref="NodeType" /> property equal to
        ///     <see cref="ExpressionType.Conditional" /> and the <see cref="ConditionalExpression.Test" />,
        ///     <see cref="ConditionalExpression.IfTrue" />,
        ///     and <see cref="ConditionalExpression.IfFalse" /> properties set to the specified values. The type of the resulting
        ///     <see cref="ConditionalExpression" />
        ///     returned by this method is <see cref="System.Void" />.
        /// </returns>
        public static ConditionalExpression IfThenElse(Expression test, Expression ifTrue, Expression ifFalse)
        {
            return Condition(test, ifTrue, ifFalse, typeof(void));
        }
    }

    internal class FullConditionalExpression : ConditionalExpression
    {
        private readonly Expression _false;

        internal FullConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
            : base(test, ifTrue)
        {
            _false = ifFalse;
        }

        internal override Expression GetFalse()
        {
            return _false;
        }
    }

    internal sealed class FullConditionalExpressionWithType : FullConditionalExpression
    {
        internal FullConditionalExpressionWithType(Expression test, Expression ifTrue, Expression ifFalse, Type type)
            : base(test, ifTrue, ifFalse)
        {
            Type = type;
        }

        public override Type Type { get; }
    }
}

#endif