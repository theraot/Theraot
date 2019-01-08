#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Compiler
{
    /// <summary>
    /// A special subtype of BlockExpression that indicates to the compiler
    /// that this block is a spilled expression and should not allow jumps in.
    /// </summary>
    internal sealed class SpilledExpressionBlock : BlockN
    {
        internal SpilledExpressionBlock(Expression[] expressions)
            : base(expressions)
        {
        }

        internal override BlockExpression Rewrite(ReadOnlyCollection<ParameterExpression> variables, Expression[] args)
        {
            throw ContractUtils.Unreachable;
        }
    }

    internal partial class StackSpiller
    {
        /// <summary>
        /// Creates a special block that is marked as not allowing jumps in.
        /// This should not be used for rewriting BlockExpression itself, or
        /// anything else that supports jumping.
        /// </summary>
        private static Expression MakeBlock(ArrayBuilder<Expression> expressions)
        {
            return new SpilledExpressionBlock(expressions.ToArray());
        }

        /// <summary>
        /// Creates a special block that is marked as not allowing jumps in.
        /// This should not be used for rewriting BlockExpression itself, or
        /// anything else that supports jumping.
        /// </summary>
        private static Expression MakeBlock(params Expression[] expressions)
        {
            return new SpilledExpressionBlock(expressions);
        }
    }
}

#endif