#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        private readonly StackGuard _guard = new StackGuard();

        private static bool IsChecked(ExpressionType op)
        {
            switch (op)
            {
                case ExpressionType.AddChecked:
                case ExpressionType.ConvertChecked:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NegateChecked:
                case ExpressionType.SubtractChecked:
                case ExpressionType.AddAssignChecked:
                case ExpressionType.MultiplyAssignChecked:
                case ExpressionType.SubtractAssignChecked:
                    return true;

                default:
                    return false;
            }
        }

        private void EmitExpression(LabelScopeInfo labelBlock, Expression node, CompilationFlags flags)
        {
            // When compiling deep trees, we run the risk of triggering a terminating StackOverflowException,
            // so we use the StackGuard utility here to probe for sufficient stack and continue the work on
            // another thread when we run out of stack space.
            if (!_guard.TryEnterOnCurrentStack())
            {
                _guard.RunOnEmptyStack((@this, n, f) => @this.EmitExpression(labelBlock, n, f), this, node, flags);
                return;
            }

            var emitStart = (flags & CompilationFlags.EmitExpressionStartMask) == CompilationFlags.EmitExpressionStart;
            var labelScopeChangeInfo = GetLabelScopeChangeInfo(emitStart, labelBlock, node);
            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = new LabelScopeInfo(labelScopeChangeInfo.Value.parent, labelScopeChangeInfo.Value.kind);
                DefineBlockLabels(labelBlock, labelScopeChangeInfo.Value.nodes);
            }

            // only pass tail call flags to emit the expression
            flags &= CompilationFlags.EmitAsTailCallMask;

            switch (node.NodeType)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.And:
                case ExpressionType.ArrayIndex:
                case ExpressionType.Divide:
                case ExpressionType.Equal:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LeftShift:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.NotEqual:
                case ExpressionType.Or:
                case ExpressionType.Power:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    EmitBinaryExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.AndAlso:
                    EmitAndAlsoBinaryExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.OrElse:
                    EmitOrElseBinaryExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Coalesce:
                    EmitCoalesceBinaryExpression(labelBlock, node);
                    break;

                case ExpressionType.Assign:
                    EmitAssignBinaryExpression(labelBlock, node);
                    break;

                case ExpressionType.ArrayLength:
                case ExpressionType.Decrement:
                case ExpressionType.Increment:
                case ExpressionType.IsFalse:
                case ExpressionType.IsTrue:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.OnesComplement:
                case ExpressionType.TypeAs:
                case ExpressionType.UnaryPlus:
                    EmitUnaryExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    EmitConvertUnaryExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Quote:
                    EmitQuoteUnaryExpression(node);
                    break;

                case ExpressionType.Throw:
                    EmitThrowUnaryExpression(labelBlock, node);
                    break;

                case ExpressionType.Unbox:
                    EmitUnboxUnaryExpression(labelBlock, node);
                    break;

                case ExpressionType.Call:
                    EmitMethodCallExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Conditional:
                    EmitConditionalExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Constant:
                    EmitConstantExpression(node);
                    break;

                case ExpressionType.Invoke:
                    EmitInvocationExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Lambda:
                    EmitLambdaExpression(labelBlock, node);
                    break;

                case ExpressionType.ListInit:
                    EmitListInitExpression(labelBlock, node);
                    break;

                case ExpressionType.MemberAccess:
                    EmitMemberExpression(labelBlock, node);
                    break;

                case ExpressionType.MemberInit:
                    EmitMemberInitExpression(labelBlock, node);
                    break;

                case ExpressionType.New:
                    EmitNewExpression(labelBlock, node);
                    break;

                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    EmitNewArrayExpression(labelBlock, node);
                    break;

                case ExpressionType.Parameter:
                    EmitParameterExpression(node);
                    break;

                case ExpressionType.TypeEqual:
                case ExpressionType.TypeIs:
                    EmitTypeBinaryExpression(labelBlock, node);
                    break;

                case ExpressionType.Block:
                    EmitBlockExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.DebugInfo:
                    EmitDebugInfoExpression(node);
                    break;

                case ExpressionType.Dynamic:
                    EmitDynamicExpression(labelBlock, node);
                    break;

                case ExpressionType.Default:
                    EmitDefaultExpression(node);
                    break;

                case ExpressionType.Goto:
                    EmitGotoExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Index:
                    EmitIndexExpression(labelBlock, node);
                    break;

                case ExpressionType.Label:
                    EmitLabelExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.RuntimeVariables:
                    EmitRuntimeVariablesExpression(node);
                    break;

                case ExpressionType.Loop:
                    EmitLoopExpression(labelBlock, node);
                    break;

                case ExpressionType.Switch:
                    EmitSwitchExpression(labelBlock, node, flags);
                    break;

                case ExpressionType.Try:
                    EmitTryExpression(labelBlock, node);
                    break;

                default:
                    break;
            }

            // if (labelScopeChangeInfo.HasValue)
            // {
            //     labelBlock = labelScopeChangeInfo.Value.parent;
            // }
        }
    }
}

#endif