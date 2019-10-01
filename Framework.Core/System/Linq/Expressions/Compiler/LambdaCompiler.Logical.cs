#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection.Emit;
using Theraot.Reflection;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        // For optimized Equal/NotEqual, we can eliminate reference
        // conversions. IL allows comparing managed pointers regardless of
        // type. See ECMA-335 "Binary Comparison or Branch Operations", in
        // Partition III, Section 1.5 Table 4.
        private static Expression GetEqualityOperand(Expression expression)
        {
            if (expression.NodeType != ExpressionType.Convert)
            {
                return expression;
            }

            var convert = (UnaryExpression)expression;
            return convert.Type.IsReferenceAssignableFromInternal(convert.Operand.Type) ? convert.Operand : expression;
        }

        private static bool NotEmpty(Expression node)
        {
            return !(node is DefaultExpression empty) || empty.Type != typeof(void);
        }

        private static bool Significant(Expression node)
        {
            if (!(node is BlockExpression block))
            {
                return NotEmpty(node) && !(node is DebugInfoExpression);
            }

            for (var i = 0; i < block.ExpressionCount; i++)
            {
                if (Significant(block.GetExpression(i)))
                {
                    return true;
                }
            }

            return false;
        }

        private void EmitAndAlsoBinaryExpression(LabelScopeInfo labelBlock, Expression expr, CompilationFlags flags)
        {
            var b = (BinaryExpression)expr;

            if (b.Method != null)
            {
                if (b.IsLiftedLogical)
                {
                    EmitExpression(labelBlock, b.ReduceUserDefinedLifted());
                }
                else
                {
                    EmitMethodAndAlso(labelBlock, b, flags);
                }
            }
            else if (b.Left.Type == typeof(bool?))
            {
                EmitLiftedAndAlso(labelBlock, b);
            }
            else
            {
                EmitUnliftedAndAlso(labelBlock, b);
            }
        }

        // Generates optimized AndAlso with branch == true
        // or optimized OrElse with branch == false
        private void EmitBranchAnd(LabelScopeInfo labelBlock, bool branch, BinaryExpression node, Label label)
        {
            // if (left) then
            //   if (right) branch label
            // endif

            var endif = IL.DefineLabel();
            EmitExpressionAndBranch(labelBlock, !branch, node.Left, endif);
            EmitExpressionAndBranch(labelBlock, branch, node.Right, label);
            IL.MarkLabel(endif);
        }

        private void EmitBranchBlock(LabelScopeInfo labelBlock, bool branch, BlockExpression node, Label label)
        {
            EnterScope(node);

            var count = node.ExpressionCount;
            for (var i = 0; i < count - 1; i++)
            {
                EmitExpressionAsVoid(labelBlock, node.GetExpression(i));
            }

            EmitExpressionAndBranch(labelBlock, branch, node.GetExpression(count - 1), label);

            ExitScope(node);
        }

        private void EmitBranchComparison(LabelScopeInfo labelBlock, bool branch, BinaryExpression node, Label label)
        {
            Debug.Assert(node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual);
            Debug.Assert(!node.IsLiftedToNull);

            // To share code paths, we want to treat NotEqual as an inverted Equal
            var branchWhenEqual = branch == (node.NodeType == ExpressionType.Equal);

            if (node.Method != null)
            {
                EmitBinaryMethod(labelBlock, node, CompilationFlags.EmitAsNoTail);
                // EmitBinaryMethod takes into account the Equal/NotEqual
                // node kind, so use the original branch value
                EmitBranchOp(branch, label);
            }
            else if (ConstantCheck.IsNull(node.Left))
            {
                if (node.Right.Type.IsNullable())
                {
                    EmitAddress(labelBlock, node.Right, node.Right.Type);
                    IL.EmitHasValue(node.Right.Type);
                }
                else
                {
                    Debug.Assert(!node.Right.Type.IsValueType);
                    EmitExpression(labelBlock, GetEqualityOperand(node.Right));
                }

                EmitBranchOp(!branchWhenEqual, label);
            }
            else if (ConstantCheck.IsNull(node.Right))
            {
                if (node.Left.Type.IsNullable())
                {
                    EmitAddress(labelBlock, node.Left, node.Left.Type);
                    IL.EmitHasValue(node.Left.Type);
                }
                else
                {
                    Debug.Assert(!node.Left.Type.IsValueType);
                    EmitExpression(labelBlock, GetEqualityOperand(node.Left));
                }

                EmitBranchOp(!branchWhenEqual, label);
            }
            else if (node.Left.Type.IsNullable() || node.Right.Type.IsNullable())
            {
                EmitBinaryExpression(labelBlock, node);
                // EmitBinaryExpression takes into account the Equal/NotEqual
                // node kind, so use the original branch value
                EmitBranchOp(branch, label);
            }
            else
            {
                EmitExpression(labelBlock, GetEqualityOperand(node.Left));
                EmitExpression(labelBlock, GetEqualityOperand(node.Right));
                IL.Emit(branchWhenEqual ? OpCodes.Beq : OpCodes.Bne_Un, label);
            }
        }

        private void EmitBranchLogical(LabelScopeInfo labelBlock, bool branch, BinaryExpression node, Label label)
        {
            Debug.Assert(node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse);
            Debug.Assert(!node.IsLiftedToNull);

            if (node.Method != null || node.IsLifted)
            {
                EmitExpression(labelBlock, node);
                EmitBranchOp(branch, label);
                return;
            }

            var isAnd = node.NodeType == ExpressionType.AndAlso;

            // To share code, we make the following substitutions:
            //     if (!(left || right)) branch value
            // becomes:
            //     if (!left && !right) branch value
            // and:
            //     if (!(left && right)) branch value
            // becomes:
            //     if (!left || !right) branch value
            //
            // The observation is that "brtrue(x && y)" has the same codegen as
            // "brfalse(x || y)" except the branches have the opposite sign.
            // Same for "brfalse(x && y)" and "brtrue(x || y)".
            //
            if (branch == isAnd)
            {
                EmitBranchAnd(labelBlock, branch, node, label);
            }
            else
            {
                EmitBranchOr(labelBlock, branch, node, label);
            }
        }

        private void EmitBranchNot(LabelScopeInfo labelBlock, bool branch, UnaryExpression node, Label label)
        {
            if (node.Method != null)
            {
                EmitExpression(labelBlock, node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                EmitBranchOp(branch, label);
                return;
            }

            EmitExpressionAndBranch(labelBlock, !branch, node.Operand, label);
        }

        private void EmitBranchOp(bool branch, Label label)
        {
            IL.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
        }

        // Generates optimized OrElse with branch == true
        // or optimized AndAlso with branch == false
        private void EmitBranchOr(LabelScopeInfo labelBlock, bool branch, BinaryExpression node, Label label)
        {
            // if (left OR right) branch label

            EmitExpressionAndBranch(labelBlock, branch, node.Left, label);
            EmitExpressionAndBranch(labelBlock, branch, node.Right, label);
        }

        private void EmitCoalesceBinaryExpression(LabelScopeInfo labelBlock, Expression expr)
        {
            var b = (BinaryExpression)expr;
            Debug.Assert(b.Method == null);

            if (b.Left.Type.IsNullable())
            {
                EmitNullableCoalesce(labelBlock, b);
            }
            else
            {
                Debug.Assert(!b.Left.Type.IsValueType);
                if (b.Conversion != null)
                {
                    EmitLambdaReferenceCoalesce(labelBlock, b);
                }
                else
                {
                    EmitReferenceCoalesceWithoutConversion(labelBlock, b);
                }
            }
        }

        private void EmitConditionalExpression(LabelScopeInfo labelBlock, Expression expr, CompilationFlags flags)
        {
            var node = (ConditionalExpression)expr;
            Debug.Assert(node.Test.Type == typeof(bool));
            var labFalse = IL.DefineLabel();
            EmitExpressionAndBranch(labelBlock, false, node.Test, labFalse);
            EmitExpressionAsType(labelBlock, node.IfTrue, node.Type, flags);

            if (NotEmpty(node.IfFalse))
            {
                var labEnd = IL.DefineLabel();
                if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
                {
                    // We know the conditional expression is at the end of the lambda,
                    // so it is safe to emit Ret here.
                    IL.Emit(OpCodes.Ret);
                }
                else
                {
                    IL.Emit(OpCodes.Br, labEnd);
                }

                IL.MarkLabel(labFalse);
                EmitExpressionAsType(labelBlock, node.IfFalse, node.Type, flags);
                IL.MarkLabel(labEnd);
            }
            else
            {
                IL.MarkLabel(labFalse);
            }
        }

        private void EmitExpressionAndBranch(LabelScopeInfo labelBlock, bool branchValue, Expression node, Label label)
        {
            Debug.Assert(node.Type == typeof(bool));
            var labelScopeChangeInfo = GetLabelScopeChangeInfo(true, labelBlock, node);
            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = new LabelScopeInfo(labelScopeChangeInfo.Value.parent, labelScopeChangeInfo.Value.kind);
                DefineBlockLabels(labelBlock, labelScopeChangeInfo.Value.nodes);
            }

            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    EmitBranchNot(labelBlock, branchValue, (UnaryExpression)node, label);
                    break;

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    EmitBranchLogical(labelBlock, branchValue, (BinaryExpression)node, label);
                    break;

                case ExpressionType.Block:
                    EmitBranchBlock(labelBlock, branchValue, (BlockExpression)node, label);
                    break;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    EmitBranchComparison(labelBlock, branchValue, (BinaryExpression)node, label);
                    break;

                default:
                    EmitExpression(labelBlock, node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                    EmitBranchOp(branchValue, label);
                    break;
            }

            if (labelScopeChangeInfo.HasValue)
            {
                labelBlock = labelScopeChangeInfo.Value.parent;
            }
        }

        private void EmitLambdaReferenceCoalesce(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var loc = GetLocal(b.Left.Type);
            var labEnd = IL.DefineLabel();
            var labNotNull = IL.DefineLabel();
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Stloc, loc);
            IL.Emit(OpCodes.Brtrue, labNotNull);
            EmitExpression(labelBlock, b.Right);
            IL.Emit(OpCodes.Br, labEnd);

            // if not null, call conversion
            IL.MarkLabel(labNotNull);
            Debug.Assert(b.Conversion!.ParameterCount == 1);

            // emit the delegate instance
            EmitLambdaExpression(labelBlock, b.Conversion);

            // emit argument
            IL.Emit(OpCodes.Ldloc, loc);
            FreeLocal(loc);

            // emit call to invoke
            IL.Emit(OpCodes.Callvirt, b.Conversion.Type.GetInvokeMethod());

            IL.MarkLabel(labEnd);
        }

        private void EmitLiftedAndAlso(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var type = typeof(bool?);
            var returnLeft = IL.DefineLabel();
            var returnRight = IL.DefineLabel();
            var exit = IL.DefineLabel();
            // Compute left
            EmitExpression(labelBlock, b.Left);
            var locLeft = GetLocal(type);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            IL.Emit(OpCodes.Not);
            IL.Emit(OpCodes.And);
            // if left == false
            IL.Emit(OpCodes.Brtrue, returnLeft);
            // Compute right
            EmitExpression(labelBlock, b.Right);
            var locRight = GetLocal(type);
            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            // if left == true
            IL.Emit(OpCodes.Brtrue_S, returnRight);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitGetValueOrDefault(type);
            // if right == true
            IL.Emit(OpCodes.Brtrue_S, returnLeft);
            IL.MarkLabel(returnRight);
            IL.Emit(OpCodes.Ldloc, locRight);
            FreeLocal(locRight);
            IL.Emit(OpCodes.Br_S, exit);
            IL.MarkLabel(returnLeft);
            IL.Emit(OpCodes.Ldloc, locLeft);
            FreeLocal(locLeft);
            IL.MarkLabel(exit);
        }

        private void EmitLiftedOrElse(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var type = typeof(bool?);
            var returnLeft = IL.DefineLabel();
            var exit = IL.DefineLabel();
            var locLeft = GetLocal(type);
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            // if left == true
            IL.Emit(OpCodes.Brtrue, returnLeft);
            EmitExpression(labelBlock, b.Right);
            var locRight = GetLocal(type);
            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitGetValueOrDefault(type);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.Or);
            // if !(right == true | left != null)
            IL.Emit(OpCodes.Brfalse_S, returnLeft);
            IL.Emit(OpCodes.Ldloc, locRight);
            FreeLocal(locRight);
            IL.Emit(OpCodes.Br_S, exit);
            IL.MarkLabel(returnLeft);
            IL.Emit(OpCodes.Ldloc, locLeft);
            FreeLocal(locLeft);
            IL.MarkLabel(exit);
        }

        private void EmitMethodAndAlso(LabelScopeInfo labelBlock, BinaryExpression b, CompilationFlags flags)
        {
            Debug.Assert(b.Method!.IsStatic);

            var labEnd = IL.DefineLabel();
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Dup);
            var opFalse = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_False");
            Debug.Assert(opFalse != null, "factory should check that the method exists");
            IL.Emit(OpCodes.Call, opFalse);
            IL.Emit(OpCodes.Brtrue, labEnd);

            EmitExpression(labelBlock, b.Right);
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
            {
                IL.Emit(OpCodes.Tailcall);
            }

            IL.Emit(OpCodes.Call, b.Method);
            IL.MarkLabel(labEnd);
        }

        private void EmitMethodOrElse(LabelScopeInfo labelBlock, BinaryExpression b, CompilationFlags flags)
        {
            Debug.Assert(b.Method!.IsStatic);

            var labEnd = IL.DefineLabel();
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Dup);
            var opTrue = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_True");
            Debug.Assert(opTrue != null, "factory should check that the method exists");

            IL.Emit(OpCodes.Call, opTrue);
            IL.Emit(OpCodes.Brtrue, labEnd);
            EmitExpression(labelBlock, b.Right);
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
            {
                IL.Emit(OpCodes.Tailcall);
            }

            IL.Emit(OpCodes.Call, b.Method);
            IL.MarkLabel(labEnd);
        }

        private void EmitNullableCoalesce(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            Debug.Assert(b.Method == null);

            var loc = GetLocal(b.Left.Type);
            var labIfNull = IL.DefineLabel();
            var labEnd = IL.DefineLabel();
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Stloc, loc);
            IL.Emit(OpCodes.Ldloca, loc);
            IL.EmitHasValue(b.Left.Type);
            IL.Emit(OpCodes.Brfalse, labIfNull);

            var nnLeftType = b.Left.Type.GetNonNullable();
            if (b.Conversion != null)
            {
                Debug.Assert(b.Conversion.ParameterCount == 1);
                var p = b.Conversion.GetParameter(0);
                Debug.Assert(p.Type.IsAssignableFrom(b.Left.Type) || p.Type.IsAssignableFrom(nnLeftType));

                // emit the delegate instance
                EmitLambdaExpression(labelBlock, b.Conversion);

                // emit argument
                if (!p.Type.IsAssignableFrom(b.Left.Type))
                {
                    IL.Emit(OpCodes.Ldloca, loc);
                    IL.EmitGetValueOrDefault(b.Left.Type);
                }
                else
                {
                    IL.Emit(OpCodes.Ldloc, loc);
                }

                // emit call to invoke
                IL.Emit(OpCodes.Callvirt, b.Conversion.Type.GetInvokeMethod());
            }
            else if (TypeUtils.AreEquivalent(b.Type, b.Left.Type))
            {
                IL.Emit(OpCodes.Ldloc, loc);
            }
            else
            {
                IL.Emit(OpCodes.Ldloca, loc);
                IL.EmitGetValueOrDefault(b.Left.Type);
                if (!TypeUtils.AreEquivalent(b.Type, nnLeftType))
                {
                    IL.EmitConvertToType(nnLeftType, b.Type, true, this);
                }
            }

            FreeLocal(loc);

            IL.Emit(OpCodes.Br, labEnd);
            IL.MarkLabel(labIfNull);
            EmitExpression(labelBlock, b.Right);
            if (!TypeUtils.AreEquivalent(b.Right.Type, b.Type))
            {
                IL.EmitConvertToType(b.Right.Type, b.Type, true, this);
            }

            IL.MarkLabel(labEnd);
        }

        private void EmitOrElseBinaryExpression(LabelScopeInfo labelBlock, Expression expr, CompilationFlags flags)
        {
            var b = (BinaryExpression)expr;

            if (b.Method != null)
            {
                if (b.IsLiftedLogical)
                {
                    EmitExpression(labelBlock, b.ReduceUserDefinedLifted());
                }
                else
                {
                    EmitMethodOrElse(labelBlock, b, flags);
                }
            }
            else if (b.Left.Type == typeof(bool?))
            {
                EmitLiftedOrElse(labelBlock, b);
            }
            else
            {
                EmitUnliftedOrElse(labelBlock, b);
            }
        }

        private void EmitReferenceCoalesceWithoutConversion(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var labEnd = IL.DefineLabel();
            var labCast = IL.DefineLabel();
            EmitExpression(labelBlock, b.Left);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Brtrue, labCast);
            IL.Emit(OpCodes.Pop);
            EmitExpression(labelBlock, b.Right);
            if (!TypeUtils.AreEquivalent(b.Right.Type, b.Type))
            {
                if (b.Right.Type.IsValueType)
                {
                    IL.Emit(OpCodes.Box, b.Right.Type);
                }

                IL.Emit(OpCodes.Castclass, b.Type);
            }

            IL.Emit(OpCodes.Br_S, labEnd);
            IL.MarkLabel(labCast);
            if (!TypeUtils.AreEquivalent(b.Left.Type, b.Type))
            {
                Debug.Assert(!b.Left.Type.IsValueType);
                IL.Emit(OpCodes.Castclass, b.Type);
            }

            IL.MarkLabel(labEnd);
        }

        private void EmitUnliftedAndAlso(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var @else = IL.DefineLabel();
            var end = IL.DefineLabel();
            EmitExpressionAndBranch(labelBlock, false, b.Left, @else);
            EmitExpression(labelBlock, b.Right);
            IL.Emit(OpCodes.Br, end);
            IL.MarkLabel(@else);
            IL.Emit(OpCodes.Ldc_I4_0);
            IL.MarkLabel(end);
        }

        private void EmitUnliftedOrElse(LabelScopeInfo labelBlock, BinaryExpression b)
        {
            var @else = IL.DefineLabel();
            var end = IL.DefineLabel();
            EmitExpressionAndBranch(labelBlock, false, b.Left, @else);
            IL.Emit(OpCodes.Ldc_I4_1);
            IL.Emit(OpCodes.Br, end);
            IL.MarkLabel(@else);
            EmitExpression(labelBlock, b.Right);
            IL.MarkLabel(end);
        }
    }
}

#endif