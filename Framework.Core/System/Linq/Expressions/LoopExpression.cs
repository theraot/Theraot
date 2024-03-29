﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions
{
    public partial class Expression
    {
        /// <summary>
        ///     Creates a <see cref="LoopExpression" /> with the given body.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <returns>The created <see cref="LoopExpression" />.</returns>
        public static LoopExpression Loop(Expression body)
        {
            return Loop(body, @break: null);
        }

        /// <summary>
        ///     Creates a <see cref="LoopExpression" /> with the given body and break target.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <returns>The created <see cref="LoopExpression" />.</returns>
        public static LoopExpression Loop(Expression body, LabelTarget? @break)
        {
            return Loop(body, @break, @continue: null);
        }

        /// <summary>
        ///     Creates a <see cref="LoopExpression" /> with the given body.
        /// </summary>
        /// <param name="body">The body of the loop.</param>
        /// <param name="break">The break target used by the loop body.</param>
        /// <param name="continue">The continue target used by the loop body.</param>
        /// <returns>The created <see cref="LoopExpression" />.</returns>
        public static LoopExpression Loop(Expression body, LabelTarget? @break, LabelTarget? @continue)
        {
            ContractUtils.RequiresNotNull(body, nameof(body));
            ExpressionUtils.RequiresCanRead(body, nameof(body));
            if (@continue != null && @continue.Type != typeof(void))
            {
                throw new ArgumentException("Type must be System.Void for this label argument", nameof(@continue));
            }

            return new LoopExpression(body, @break, @continue);
        }
    }

    /// <summary>
    ///     Represents an infinite loop. It can be exited with "break".
    /// </summary>
    [DebuggerTypeProxy(typeof(LoopExpressionProxy))]
    public sealed class LoopExpression : Expression
    {
        internal LoopExpression(Expression body, LabelTarget? @break, LabelTarget? @continue)
        {
            Body = body;
            BreakLabel = @break;
            ContinueLabel = @continue;
        }

        /// <summary>
        ///     Gets the <see cref="Expression" /> that is the body of the loop.
        /// </summary>
        public Expression Body { get; }

        /// <summary>
        ///     Gets the <see cref="LabelTarget" /> that is used by the loop body as a break statement target.
        /// </summary>
        public LabelTarget? BreakLabel { get; }

        /// <summary>
        ///     Gets the <see cref="LabelTarget" /> that is used by the loop body as a continue statement target.
        /// </summary>
        public LabelTarget? ContinueLabel { get; }

        /// <summary>
        ///     Returns the node type of this Expression. Extension nodes should return
        ///     ExpressionType.Extension when overriding this method.
        /// </summary>
        /// <returns>The <see cref="ExpressionType" /> of the expression.</returns>
        public override ExpressionType NodeType => ExpressionType.Loop;

        /// <summary>
        ///     Gets the static type of the expression that this <see cref="Expression" /> represents.
        /// </summary>
        /// <returns>The <see cref="System.Type" /> that represents the static type of the expression.</returns>
        public override Type Type => BreakLabel?.Type ?? typeof(void);

        /// <summary>
        ///     Creates a new expression that is like this one, but using the
        ///     supplied children. If all of the children are the same, it will
        ///     return this expression.
        /// </summary>
        /// <param name="breakLabel">The <see cref="BreakLabel" /> property of the result.</param>
        /// <param name="continueLabel">The <see cref="ContinueLabel" /> property of the result.</param>
        /// <param name="body">The <see cref="Body" /> property of the result.</param>
        /// <returns>This expression if no children changed, or an expression with the updated children.</returns>
        public LoopExpression Update(LabelTarget? breakLabel, LabelTarget? continueLabel, Expression body)
        {
            if (breakLabel == BreakLabel && continueLabel == ContinueLabel && body == Body)
            {
                return this;
            }

            return Loop(body, breakLabel, continueLabel);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            return visitor.VisitLoop(this);
        }
    }
}

#endif