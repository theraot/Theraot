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
            if (expression.NodeType == ExpressionType.Convert)
            {
                var convert = (UnaryExpression)expression;
                if (convert.Type.IsReferenceAssignableFromInternal(convert.Operand.Type))
                {
                    return convert.Operand;
                }
            }
            return expression;
        }
        private static bool NotEmpty(Expression node)
        {
            return !(node is DefaultExpression empty) || empty.Type != typeof(void);
        }

        private static bool Significant(Expression node)
        {
            if (node is BlockExpression block)
            {
                for (var i = 0; i < block.ExpressionCount; i++)
                {
                    if (Significant(block.GetExpression(i)))
                    {
                        return true;
                    }
                }
                return false;
            }
            return NotEmpty(node) && !(node is DebugInfoExpression);
        }

        private void EmitAndAlsoBinaryExpression(Expression expr, CompilationFlags flags)
        {
            var b = (BinaryExpression)expr;

            if (b.Method != null)
            {
                if (b.IsLiftedLogical)
                {
                    EmitExpression(b.ReduceUserDefinedLifted());
                }
                else
                {
                    EmitMethodAndAlso(b, flags);
                }
            }
            else if (b.Left.Type == typeof(bool?))
            {
                EmitLiftedAndAlso(b);
            }
            else
            {
                EmitUnliftedAndAlso(b);
            }
        }

        // Generates optimized AndAlso with branch == true
        // or optimized OrElse with branch == false
        private void EmitBranchAnd(bool branch, BinaryExpression node, Label label)
        {
            // if (left) then
            //   if (right) branch label
            // endif

            var endif = IL.DefineLabel();
            EmitExpressionAndBranch(!branch, node.Left, endif);
            EmitExpressionAndBranch(branch, node.Right, label);
            IL.MarkLabel(endif);
        }

        private void EmitBranchBlock(bool branch, BlockExpression node, Label label)
        {
            EnterScope(node);

            var count = node.ExpressionCount;
            for (var i = 0; i < count - 1; i++)
            {
                EmitExpressionAsVoid(node.GetExpression(i));
            }
            EmitExpressionAndBranch(branch, node.GetExpression(count - 1), label);

            ExitScope(node);
        }

        private void EmitBranchComparison(bool branch, BinaryExpression node, Label label)
        {
            Debug.Assert(node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.NotEqual);
            Debug.Assert(!node.IsLiftedToNull);

            // To share code paths, we want to treat NotEqual as an inverted Equal
            var branchWhenEqual = branch == (node.NodeType == ExpressionType.Equal);

            if (node.Method != null)
            {
                EmitBinaryMethod(node, CompilationFlags.EmitAsNoTail);
                // EmitBinaryMethod takes into account the Equal/NotEqual
                // node kind, so use the original branch value
                EmitBranchOp(branch, label);
            }
            else if (ConstantCheck.IsNull(node.Left))
            {
                if (node.Right.Type.IsNullable())
                {
                    EmitAddress(node.Right, node.Right.Type);
                    IL.EmitHasValue(node.Right.Type);
                }
                else
                {
                    Debug.Assert(!node.Right.Type.IsValueType);
                    EmitExpression(GetEqualityOperand(node.Right));
                }
                EmitBranchOp(!branchWhenEqual, label);
            }
            else if (ConstantCheck.IsNull(node.Right))
            {
                if (node.Left.Type.IsNullable())
                {
                    EmitAddress(node.Left, node.Left.Type);
                    IL.EmitHasValue(node.Left.Type);
                }
                else
                {
                    Debug.Assert(!node.Left.Type.IsValueType);
                    EmitExpression(GetEqualityOperand(node.Left));
                }
                EmitBranchOp(!branchWhenEqual, label);
            }
            else if (node.Left.Type.IsNullable() || node.Right.Type.IsNullable())
            {
                EmitBinaryExpression(node);
                // EmitBinaryExpression takes into account the Equal/NotEqual
                // node kind, so use the original branch value
                EmitBranchOp(branch, label);
            }
            else
            {
                EmitExpression(GetEqualityOperand(node.Left));
                EmitExpression(GetEqualityOperand(node.Right));
                IL.Emit(branchWhenEqual ? OpCodes.Beq : OpCodes.Bne_Un, label);
            }
        }

        private void EmitBranchLogical(bool branch, BinaryExpression node, Label label)
        {
            Debug.Assert(node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse);
            Debug.Assert(!node.IsLiftedToNull);

            if (node.Method != null || node.IsLifted)
            {
                EmitExpression(node);
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
                EmitBranchAnd(branch, node, label);
            }
            else
            {
                EmitBranchOr(branch, node, label);
            }
        }

        private void EmitBranchNot(bool branch, UnaryExpression node, Label label)
        {
            if (node.Method != null)
            {
                EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                EmitBranchOp(branch, label);
                return;
            }

            EmitExpressionAndBranch(!branch, node.Operand, label);
        }

        private void EmitBranchOp(bool branch, Label label)
        {
            IL.Emit(branch ? OpCodes.Brtrue : OpCodes.Brfalse, label);
        }

        // Generates optimized OrElse with branch == true
        // or optimized AndAlso with branch == false
        private void EmitBranchOr(bool branch, BinaryExpression node, Label label)
        {
            // if (left OR right) branch label

            EmitExpressionAndBranch(branch, node.Left, label);
            EmitExpressionAndBranch(branch, node.Right, label);
        }

        private void EmitCoalesceBinaryExpression(Expression expr)
        {
            var b = (BinaryExpression)expr;
            Debug.Assert(b.Method == null);

            if (b.Left.Type.IsNullable())
            {
                EmitNullableCoalesce(b);
            }
            else
            {
                Debug.Assert(!b.Left.Type.IsValueType);
                if (b.Conversion != null)
                {
                    EmitLambdaReferenceCoalesce(b);
                }
                else
                {
                    EmitReferenceCoalesceWithoutConversion(b);
                }
            }
        }

        private void EmitConditionalExpression(Expression expr, CompilationFlags flags)
        {
            var node = (ConditionalExpression)expr;
            Debug.Assert(node.Test.Type == typeof(bool));
            var labFalse = IL.DefineLabel();
            EmitExpressionAndBranch(false, node.Test, labFalse);
            EmitExpressionAsType(node.IfTrue, node.Type, flags);

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
                EmitExpressionAsType(node.IfFalse, node.Type, flags);
                IL.MarkLabel(labEnd);
            }
            else
            {
                IL.MarkLabel(labFalse);
            }
        }

        /// <summary>
        /// Emits the expression and then either brtrue/brfalse to the label.
        /// </summary>
        /// <param name="branchValue">True for brtrue, false for brfalse.</param>
        /// <param name="node">The expression to emit.</param>
        /// <param name="label">The label to conditionally branch to.</param>
        /// <remarks>
        /// <para>
        /// This function optimizes equality and short circuiting logical
        /// operators to avoid double-branching, minimize instruction count,
        /// and generate similar IL to the C# compiler. This is important for
        /// the JIT to optimize patterns like:
        ///     x != null AndAlso x.GetType() == typeof(SomeType)
        /// </para>
        /// <para>
        /// One optimization we don't do: we always emits at least one
        /// conditional branch to the label, and always possibly falls through,
        /// even if we know if the branch will always succeed or always fail.
        /// We do this to avoid generating unreachable code, which is fine for
        /// the CLR JIT, but doesn't verify with peverify.
        /// </para>
        /// <para>
        /// This kind of optimization could be implemented safely, by doing
        /// constant folding over conditionals and logical expressions at the
        /// tree level.
        /// </para>
        /// </remarks>
        private void EmitExpressionAndBranch(bool branchValue, Expression node, Label label)
        {
            Debug.Assert(node.Type == typeof(bool));
            var startEmitted = EmitExpressionStart(node);
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    EmitBranchNot(branchValue, (UnaryExpression)node, label);
                    break;

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    EmitBranchLogical(branchValue, (BinaryExpression)node, label);
                    break;

                case ExpressionType.Block:
                    EmitBranchBlock(branchValue, (BlockExpression)node, label);
                    break;

                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    EmitBranchComparison(branchValue, (BinaryExpression)node, label);
                    break;

                default:
                    EmitExpression(node, CompilationFlags.EmitAsNoTail | CompilationFlags.EmitNoExpressionStart);
                    EmitBranchOp(branchValue, label);
                    break;
            }

            EmitExpressionEnd(startEmitted);
        }

        private void EmitLambdaReferenceCoalesce(BinaryExpression b)
        {
            var loc = GetLocal(b.Left.Type);
            var labEnd = IL.DefineLabel();
            var labNotNull = IL.DefineLabel();
            EmitExpression(b.Left);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Stloc, loc);
            IL.Emit(OpCodes.Brtrue, labNotNull);
            EmitExpression(b.Right);
            IL.Emit(OpCodes.Br, labEnd);

            // if not null, call conversion
            IL.MarkLabel(labNotNull);
            Debug.Assert(b.Conversion.ParameterCount == 1);

            // emit the delegate instance
            EmitLambdaExpression(b.Conversion);

            // emit argument
            IL.Emit(OpCodes.Ldloc, loc);
            FreeLocal(loc);

            // emit call to invoke
            IL.Emit(OpCodes.Callvirt, b.Conversion.Type.GetInvokeMethod());

            IL.MarkLabel(labEnd);
        }

        private void EmitLiftedAndAlso(BinaryExpression b)
        {
            var type = typeof(bool?);
            var returnLeft = IL.DefineLabel();
            var returnRight = IL.DefineLabel();
            var exit = IL.DefineLabel();
            // Compute left
            EmitExpression(b.Left);
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
            EmitExpression(b.Right);
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

        private void EmitLiftedOrElse(BinaryExpression b)
        {
            var type = typeof(bool?);
            var returnLeft = IL.DefineLabel();
            var exit = IL.DefineLabel();
            var locLeft = GetLocal(type);
            EmitExpression(b.Left);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            // if left == true
            IL.Emit(OpCodes.Brtrue, returnLeft);
            EmitExpression(b.Right);
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

        private void EmitMethodAndAlso(BinaryExpression b, CompilationFlags flags)
        {
            Debug.Assert(b.Method.IsStatic);

            var labEnd = IL.DefineLabel();
            EmitExpression(b.Left);
            IL.Emit(OpCodes.Dup);
            var opFalse = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_False");
            Debug.Assert(opFalse != null, "factory should check that the method exists");
            IL.Emit(OpCodes.Call, opFalse);
            IL.Emit(OpCodes.Brtrue, labEnd);

            EmitExpression(b.Right);
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
            {
                IL.Emit(OpCodes.Tailcall);
            }

            IL.Emit(OpCodes.Call, b.Method);
            IL.MarkLabel(labEnd);
        }

        private void EmitMethodOrElse(BinaryExpression b, CompilationFlags flags)
        {
            Debug.Assert(b.Method.IsStatic);

            var labEnd = IL.DefineLabel();
            EmitExpression(b.Left);
            IL.Emit(OpCodes.Dup);
            var opTrue = TypeUtils.GetBooleanOperator(b.Method.DeclaringType, "op_True");
            Debug.Assert(opTrue != null, "factory should check that the method exists");

            IL.Emit(OpCodes.Call, opTrue);
            IL.Emit(OpCodes.Brtrue, labEnd);
            EmitExpression(b.Right);
            if ((flags & CompilationFlags.EmitAsTailCallMask) == CompilationFlags.EmitAsTail)
            {
                IL.Emit(OpCodes.Tailcall);
            }

            IL.Emit(OpCodes.Call, b.Method);
            IL.MarkLabel(labEnd);
        }

        private void EmitNullableCoalesce(BinaryExpression b)
        {
            Debug.Assert(b.Method == null);

            var loc = GetLocal(b.Left.Type);
            var labIfNull = IL.DefineLabel();
            var labEnd = IL.DefineLabel();
            EmitExpression(b.Left);
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
                EmitLambdaExpression(b.Conversion);

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
                    IL.EmitConvertToType(nnLeftType, b.Type, isChecked: true, locals: this);
                }
            }

            FreeLocal(loc);

            IL.Emit(OpCodes.Br, labEnd);
            IL.MarkLabel(labIfNull);
            EmitExpression(b.Right);
            if (!TypeUtils.AreEquivalent(b.Right.Type, b.Type))
            {
                IL.EmitConvertToType(b.Right.Type, b.Type, isChecked: true, locals: this);
            }
            IL.MarkLabel(labEnd);
        }

        private void EmitOrElseBinaryExpression(Expression expr, CompilationFlags flags)
        {
            var b = (BinaryExpression)expr;

            if (b.Method != null)
            {
                if (b.IsLiftedLogical)
                {
                    EmitExpression(b.ReduceUserDefinedLifted());
                }
                else
                {
                    EmitMethodOrElse(b, flags);
                }
            }
            else if (b.Left.Type == typeof(bool?))
            {
                EmitLiftedOrElse(b);
            }
            else
            {
                EmitUnliftedOrElse(b);
            }
        }

        private void EmitReferenceCoalesceWithoutConversion(BinaryExpression b)
        {
            var labEnd = IL.DefineLabel();
            var labCast = IL.DefineLabel();
            EmitExpression(b.Left);
            IL.Emit(OpCodes.Dup);
            IL.Emit(OpCodes.Brtrue, labCast);
            IL.Emit(OpCodes.Pop);
            EmitExpression(b.Right);
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

        private void EmitUnliftedAndAlso(BinaryExpression b)
        {
            var @else = IL.DefineLabel();
            var end = IL.DefineLabel();
            EmitExpressionAndBranch(false, b.Left, @else);
            EmitExpression(b.Right);
            IL.Emit(OpCodes.Br, end);
            IL.MarkLabel(@else);
            IL.Emit(OpCodes.Ldc_I4_0);
            IL.MarkLabel(end);
        }

        private void EmitUnliftedOrElse(BinaryExpression b)
        {
            var @else = IL.DefineLabel();
            var end = IL.DefineLabel();
            EmitExpressionAndBranch(false, b.Left, @else);
            IL.Emit(OpCodes.Ldc_I4_1);
            IL.Emit(OpCodes.Br, end);
            IL.MarkLabel(@else);
            EmitExpression(b.Right);
            IL.MarkLabel(end);
        }
    }
}

#endif