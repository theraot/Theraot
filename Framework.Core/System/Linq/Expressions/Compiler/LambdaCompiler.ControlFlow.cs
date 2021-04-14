#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic.Utils;

namespace System.Linq.Expressions.Compiler
{
    // The part of the LambdaCompiler dealing with low level control flow
    // break, continue, return, exceptions, etc
    internal partial class LambdaCompiler
    {
        private static (LabelScopeInfo parent, LabelScopeKind kind, IList<Expression>? nodes)? GetLabelScopeChangeInfo(bool emitStart, LabelScopeInfo labelBlock, Expression node)
        {
            if (!emitStart)
            {
                return null;
            }

            // Anything that is "statement-like" -- e.g. has no associated
            // stack state can be jumped into, with the exception of try-blocks
            // We indicate this by a "Block"
            //
            // Otherwise, we push an "Expression" to indicate that it can't be
            // jumped into
            switch (node.NodeType)
            {
                default:
                    if (labelBlock.Kind == LabelScopeKind.Expression)
                    {
                        return null;
                    }

                    return (labelBlock, LabelScopeKind.Expression, null);

                case ExpressionType.Label:
                    // LabelExpression is a bit special, if it's directly in a
                    // block it becomes associate with the block's scope. Same
                    // thing if it's in a switch case body.
                    if (labelBlock.Kind != LabelScopeKind.Block)
                    {
                        return (labelBlock, LabelScopeKind.Statement, null);
                    }

                    var label = ((LabelExpression)node).Target;
                    if (labelBlock.ContainsTarget(label))
                    {
                        return null;
                    }

                    if (labelBlock.Parent?.Kind == LabelScopeKind.Switch && labelBlock.Parent.ContainsTarget(label))
                    {
                        return null;
                    }

                    return (labelBlock, LabelScopeKind.Statement, null);

                case ExpressionType.Block:
                    if (node is SpilledExpressionBlock)
                    {
                        // treat it as an expression
                        goto default;
                    }

                    return labelBlock.Parent?.Kind != LabelScopeKind.Switch
                        ? (labelBlock, LabelScopeKind.Block, new[] { node })
                        : (labelBlock, LabelScopeKind.Block, null);

                case ExpressionType.Switch:
                    var nodes = new List<Expression>();
                    var @switch = (SwitchExpression)node;
                    foreach (var c in @switch.Cases)
                    {
                        nodes.Add(c.Body);
                    }

                    if (@switch.DefaultBody != null)
                    {
                        nodes.Add(@switch.DefaultBody);
                    }

                    return (labelBlock, LabelScopeKind.Switch, nodes);

                // Remove this when Convert(Void) goes away.
                case ExpressionType.Convert:
                    if (node.Type != typeof(void))
                    {
                        // treat it as an expression
                        goto default;
                    }

                    return (labelBlock, LabelScopeKind.Statement, null);

                case ExpressionType.Conditional:
                case ExpressionType.Loop:
                case ExpressionType.Goto:
                    return (labelBlock, LabelScopeKind.Statement, null);
            }
        }

        // See if this lambda has a return label
        // If so, we'll create it now and mark it as allowing the "ret" opcode
        // This allows us to generate better IL
        private void AddReturnLabel(LambdaExpression lambda)
        {
            var expression = lambda.Body;

            while (true)
            {
                switch (expression.NodeType)
                {
                    default:
                        // Didn't find return label
                        return;

                    case ExpressionType.Label:
                        // Found the label. We can directly return from this place
                        // only if the label type is reference assignable to the lambda return type.
                        var label = ((LabelExpression)expression).Target;
                        _labelInfo.Add(label, new LabelInfo(IL, label, lambda.ReturnType.IsReferenceAssignableFromInternal(label.Type)));
                        return;

                    case ExpressionType.Block:
                        // Look in the last significant expression of a block
                        var body = (BlockExpression)expression;
                        // omit empty and debuginfo at the end of the block since they
                        // are not going to emit any IL
                        if (body.ExpressionCount == 0)
                        {
                            return;
                        }

                        for (var i = body.ExpressionCount - 1; i >= 0; i--)
                        {
                            expression = body.GetExpression(i);
                            if (Significant(expression))
                            {
                                break;
                            }
                        }

                        continue;
                }
            }
        }

        private void DefineBlockLabels(IList<Expression>? nodes)
        {
            if (nodes == null)
            {
                return;
            }

            foreach (var node in nodes)
            {
                DefineBlockLabels(node);
            }
        }

        private void DefineBlockLabels(Expression node)
        {
            if (node is not BlockExpression block || block is SpilledExpressionBlock)
            {
                return;
            }

            for (int i = 0, n = block.ExpressionCount; i < n; i++)
            {
                var e = block.GetExpression(i);

                if (e is LabelExpression label)
                {
                    DefineLabel(label.Target);
                }
            }
        }

        private LabelInfo DefineLabel(LabelTarget? node)
        {
            if (node == null)
            {
                return new LabelInfo(IL, node: null, canReturn: false);
            }

            var result = EnsureLabel(node);
            result.Define(_labelBlock);
            return result;
        }

        private void EmitGotoExpression(Expression expr, CompilationFlags flags)
        {
            var node = (GotoExpression)expr;
            var labelInfo = ReferenceLabel(node.Target);

            var tailCall = flags & CompilationFlags.EmitAsTailCallMask;
            if (tailCall != CompilationFlags.EmitAsNoTail)
            {
                // Since tail call flags are not passed into EmitTryExpression, CanReturn
                // means the goto will be emitted as Ret. Therefore we can emit the goto's
                // default value with tail call. This can be improved by detecting if the
                // target label is equivalent to the return label.
                tailCall = labelInfo.CanReturn ? CompilationFlags.EmitAsTail : CompilationFlags.EmitAsNoTail;
                flags = UpdateEmitAsTailCallFlag(flags, tailCall);
            }

            if (node.Value != null)
            {
                if (node.Target.Type == typeof(void))
                {
                    EmitExpressionAsVoid(node.Value, flags);
                }
                else
                {
                    flags = UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitExpressionStart);
                    EmitExpression(node.Value, flags);
                }
            }

            labelInfo.EmitJump();

            EmitUnreachable(node, flags);
        }

        private void EmitLabelExpression(Expression expr, CompilationFlags flags)
        {
            var node = (LabelExpression)expr;

            // If we're an immediate child of a block, our label will already
            // be defined. If not, we need to define our own block so this
            // label isn't exposed except to its own child expression.
            LabelInfo? label = null;

            if (_labelBlock.Kind == LabelScopeKind.Block)
            {
                label = _labelBlock.GetLabelInfo(node.Target);

                // We're in a block but didn't find our label, try switch
                if (label == null && _labelBlock.Parent?.Kind == LabelScopeKind.Switch)
                {
                    label = _labelBlock.Parent.GetLabelInfo(node.Target);
                }

                // if we're in a switch or block, we should have found the label
                Debug.Assert(label != null);
            }

            if (label == null)
            {
                label = DefineLabel(node.Target);
            }

            if (node.DefaultValue != null)
            {
                if (node.Target.Type == typeof(void))
                {
                    EmitExpressionAsVoid(node.DefaultValue, flags);
                }
                else
                {
                    flags = UpdateEmitExpressionStartFlag(flags, CompilationFlags.EmitExpressionStart);
                    EmitExpression(node.DefaultValue, flags);
                }
            }

            label.Mark();
        }

        // We need to push default(T), unless we're emitting ourselves as
        // void. Even though the code is unreachable, we still have to
        // generate correct IL. We can get rid of this once we have better
        // reachability analysis.
        private void EmitUnreachable(Expression node, CompilationFlags flags)
        {
            if (node.Type != typeof(void) && (flags & CompilationFlags.EmitAsVoidType) == 0)
            {
                IL.EmitDefault(node.Type, this);
            }
        }

        private LabelInfo EnsureLabel(LabelTarget node)
        {
            if (_labelInfo.TryGetValue(node, out var result))
            {
                return result;
            }

            result = new LabelInfo(IL, node, canReturn: false);
            _labelInfo.Add(node, result);
            return result;
        }

        private LabelInfo ReferenceLabel(LabelTarget node)
        {
            var result = EnsureLabel(node);
            result.Reference(_labelBlock);
            return result;
        }
    }
}

#endif