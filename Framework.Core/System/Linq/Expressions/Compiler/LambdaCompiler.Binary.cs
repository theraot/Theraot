#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection.Emit;
using Theraot.Reflection;
using static System.Linq.Expressions.CachedReflectionInfo;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        private void EmitBinaryExpression(Expression expr, CompilationFlags flags = CompilationFlags.EmitAsNoTail)
        {
            var b = (BinaryExpression)expr;

            Debug.Assert(b.NodeType != ExpressionType.AndAlso && b.NodeType != ExpressionType.OrElse && b.NodeType != ExpressionType.Coalesce);

            if (b.Method != null)
            {
                EmitBinaryMethod(b, flags);
                return;
            }

            // For EQ and NE, if there is a user-specified method, use it.
            // Otherwise implement the C# semantics that allow equality
            // comparisons on non-primitive nullable structs that don't
            // overload "=="
            if
            (
                (b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual)
                && (b.Type == typeof(bool) || b.Type == typeof(bool?))
            )
            {
                // If we have x==null, x!=null, null==x or null!=x where x is
                // nullable but not null, then generate a call to x.HasValue.
                Debug.Assert(!b.IsLiftedToNull || b.Type == typeof(bool?));
                if (ConstantCheck.IsNull(b.Left) && !ConstantCheck.IsNull(b.Right) && b.Right.Type.IsNullable())
                {
                    EmitNullEquality(b.NodeType, b.Right, b.IsLiftedToNull);
                    return;
                }

                if (ConstantCheck.IsNull(b.Right) && !ConstantCheck.IsNull(b.Left) && b.Left.Type.IsNullable())
                {
                    EmitNullEquality(b.NodeType, b.Left, b.IsLiftedToNull);
                    return;
                }

                // For EQ and NE, we can avoid some conversions if we're
                // ultimately just comparing two managed pointers.
                EmitExpression(GetEqualityOperand(b.Left));
                EmitExpression(GetEqualityOperand(b.Right));
            }
            else
            {
                // Otherwise generate it normally
                EmitExpression(b.Left);
                EmitExpression(b.Right);
            }

            EmitBinaryOperator(b.NodeType, b.Left.Type, b.Right.Type, b.Type, b.IsLiftedToNull);
        }

        private void EmitBinaryMethod(BinaryExpression b, CompilationFlags flags)
        {
            if (b.IsLifted)
            {
                var p1 = Expression.Variable(b.Left.Type.GetNonNullable(), null);
                var p2 = Expression.Variable(b.Right.Type.GetNonNullable(), null);
                var mc = Expression.Call(null, b.Method, p1, p2);
                Type resultType;
                if (b.IsLiftedToNull)
                {
                    resultType = mc.Type.GetNullable();
                }
                else
                {
                    Debug.Assert(mc.Type == typeof(bool));
                    Debug.Assert
                    (
                        b.NodeType == ExpressionType.Equal
                        || b.NodeType == ExpressionType.NotEqual
                        || b.NodeType == ExpressionType.LessThan
                        || b.NodeType == ExpressionType.LessThanOrEqual
                        || b.NodeType == ExpressionType.GreaterThan
                        || b.NodeType == ExpressionType.GreaterThanOrEqual
                    );

                    resultType = typeof(bool);
                }

                Debug.Assert(p1.Type.IsReferenceAssignableFromInternal(b.Left.Type.GetNonNullable()));
                Debug.Assert(p2.Type.IsReferenceAssignableFromInternal(b.Right.Type.GetNonNullable()));
                EmitLift(b.NodeType, resultType, mc, new[] {p1, p2}, new[] {b.Left, b.Right});
            }
            else
            {
                EmitMethodCallExpression(Expression.Call(null, b.Method, b.Left, b.Right), flags);
            }
        }

        private void EmitBinaryOperator(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
        {
            Debug.Assert(op != ExpressionType.Coalesce);
            if (op == ExpressionType.ArrayIndex)
            {
                Debug.Assert(rightType == typeof(int));
                EmitGetArrayElement(leftType);
            }
            else if (leftType.IsNullable() || rightType.IsNullable())
            {
                EmitLiftedBinaryOp(op, leftType, rightType, resultType, liftedToNull);
            }
            else
            {
                EmitUnliftedBinaryOp(op, leftType, rightType);
            }
        }

        // Binary/unary operations on 8 and 16 bit operand types will leave a
        // 32-bit value on the stack, because that's how IL works. For these
        // cases, we need to cast it back to the resultType, possibly using a
        // checked conversion if the original operator was convert
        private void EmitConvertArithmeticResult(ExpressionType op, Type resultType)
        {
            Debug.Assert(!resultType.IsNullable());

            switch (resultType.GetTypeCode())
            {
                case TypeCode.Byte:
                    IL.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1);
                    break;

                case TypeCode.SByte:
                    IL.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1);
                    break;

                case TypeCode.UInt16:
                    IL.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2);
                    break;

                case TypeCode.Int16:
                    IL.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2);
                    break;
                default:
                    break;
            }
        }

        private void EmitLiftedBinaryArithmetic(ExpressionType op, Type leftType, Type rightType, Type resultType)
        {
            var leftIsNullable = leftType.IsNullable();
            var rightIsNullable = rightType.IsNullable();

            Debug.Assert(leftIsNullable || rightIsNullable);

            var labIfNull = IL.DefineLabel();
            var labEnd = IL.DefineLabel();
            var locLeft = GetLocal(leftType);
            var locRight = GetLocal(rightType);
            var locResult = GetLocal(resultType);

            // store values (reverse order since they are already on the stack)
            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Stloc, locLeft);

            // test for null
            // don't use short circuiting
            if (leftIsNullable)
            {
                IL.Emit(OpCodes.Ldloca, locLeft);
                IL.EmitHasValue(leftType);
            }

            if (rightIsNullable)
            {
                IL.Emit(OpCodes.Ldloca, locRight);
                IL.EmitHasValue(rightType);
                if (leftIsNullable)
                {
                    IL.Emit(OpCodes.And);
                }
            }

            IL.Emit(OpCodes.Brfalse_S, labIfNull);

            // do op on values
            if (leftIsNullable)
            {
                IL.Emit(OpCodes.Ldloca, locLeft);
                IL.EmitGetValueOrDefault(leftType);
            }
            else
            {
                IL.Emit(OpCodes.Ldloc, locLeft);
            }

            if (rightIsNullable)
            {
                IL.Emit(OpCodes.Ldloca, locRight);
                IL.EmitGetValueOrDefault(rightType);
            }
            else
            {
                IL.Emit(OpCodes.Ldloc, locRight);
            }

            //RELEASING locLeft locRight
            FreeLocal(locLeft);
            FreeLocal(locRight);

            EmitBinaryOperator(op, leftType.GetNonNullable(), rightType.GetNonNullable(), resultType.GetNonNullable(), false);

            // construct result type
            var ci = resultType.GetConstructor(new[] {resultType.GetNonNullable()});
            // ReSharper disable once AssignNullToNotNullAttribute
            IL.Emit(OpCodes.Newobj, ci);
            IL.Emit(OpCodes.Stloc, locResult);
            IL.Emit(OpCodes.Br_S, labEnd);

            // if null then create a default one
            IL.MarkLabel(labIfNull);
            IL.Emit(OpCodes.Ldloca, locResult);
            IL.Emit(OpCodes.Initobj, resultType);

            IL.MarkLabel(labEnd);

            IL.Emit(OpCodes.Ldloc, locResult);

            //RELEASING locResult
            FreeLocal(locResult);
        }

        private void EmitLiftedBinaryOp(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
        {
            Debug.Assert(leftType.IsNullable() || rightType.IsNullable());
            switch (op)
            {
                case ExpressionType.And:
                    if (leftType == typeof(bool?))
                    {
                        EmitLiftedBooleanAnd();
                    }
                    else
                    {
                        EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
                    }

                    break;

                case ExpressionType.Or:
                    if (leftType == typeof(bool?))
                    {
                        EmitLiftedBooleanOr();
                    }
                    else
                    {
                        EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
                    }

                    break;

                case ExpressionType.ExclusiveOr:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.LeftShift:
                case ExpressionType.RightShift:
                    EmitLiftedBinaryArithmetic(op, leftType, rightType, resultType);
                    break;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    Debug.Assert(leftType == rightType);
                    if (liftedToNull)
                    {
                        Debug.Assert(resultType == typeof(bool?));
                        EmitLiftedToNullRelational(op, leftType);
                    }
                    else
                    {
                        Debug.Assert(resultType == typeof(bool));
                        EmitLiftedRelational(op, leftType);
                    }

                    break;
                default:
                    break;
            }
        }

        private void EmitLiftedBooleanAnd()
        {
            var type = typeof(bool?);
            var returnRight = IL.DefineLabel();
            var exit = IL.DefineLabel();
            // store values (reverse order since they are already on the stack)
            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);
            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            // if left == true
            IL.Emit(OpCodes.Brtrue_S, returnRight);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitGetValueOrDefault(type);
            IL.Emit(OpCodes.Or);
            // if !(left != null | right == true)
            IL.Emit(OpCodes.Brfalse_S, returnRight);
            IL.Emit(OpCodes.Ldloc, locLeft);
            FreeLocal(locLeft);
            IL.Emit(OpCodes.Br_S, exit);
            IL.MarkLabel(returnRight);
            IL.Emit(OpCodes.Ldloc, locRight);
            FreeLocal(locRight);
            IL.MarkLabel(exit);
        }

        private void EmitLiftedBooleanOr()
        {
            var type = typeof(bool?);
            var returnLeft = IL.DefineLabel();
            var exit = IL.DefineLabel();
            // store values (reverse order since they are already on the stack)
            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);
            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            // if left == true
            IL.Emit(OpCodes.Brtrue_S, returnLeft);
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

        private void EmitLiftedRelational(ExpressionType op, Type type)
        {
            // Equal is (left.GetValueOrDefault() == right.GetValueOrDefault()) & (left.HasValue == right.HasValue)
            // NotEqual is !((left.GetValueOrDefault() == right.GetValueOrDefault()) & (left.HasValue == right.HasValue))
            // Others are (left.GetValueOrDefault() op right.GetValueOrDefault()) & (left.HasValue & right.HasValue)

            var invert = op == ExpressionType.NotEqual;
            if (invert)
            {
                op = ExpressionType.Equal;
            }

            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);

            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitGetValueOrDefault(type);
            var unnullable = type.GetNonNullable();
            EmitUnliftedBinaryOp(op, unnullable, unnullable);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitHasValue(type);
            FreeLocal(locLeft);
            FreeLocal(locRight);
            IL.Emit(op == ExpressionType.Equal ? OpCodes.Ceq : OpCodes.And);
            IL.Emit(OpCodes.And);
            if (invert)
            {
                IL.Emit(OpCodes.Ldc_I4_0);
                IL.Emit(OpCodes.Ceq);
            }
        }

        private void EmitLiftedToNullRelational(ExpressionType op, Type type)
        {
            // (left.HasValue & right.HasValue) ? left.GetValueOrDefault() op right.GetValueOrDefault() : default(bool?)
            var notNull = IL.DefineLabel();
            var end = IL.DefineLabel();

            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);

            IL.Emit(OpCodes.Stloc, locRight);
            IL.Emit(OpCodes.Stloc, locLeft);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitHasValue(type);
            IL.Emit(OpCodes.And);
            IL.Emit(OpCodes.Brtrue_S, notNull);
            IL.EmitDefault(typeof(bool?), this);
            IL.Emit(OpCodes.Br_S, end);
            IL.MarkLabel(notNull);
            IL.Emit(OpCodes.Ldloca, locLeft);
            IL.EmitGetValueOrDefault(type);
            IL.Emit(OpCodes.Ldloca, locRight);
            IL.EmitGetValueOrDefault(type);
            FreeLocal(locLeft);
            FreeLocal(locRight);
            var unnullable = type.GetNonNullable();
            EmitUnliftedBinaryOp(op, unnullable, unnullable);
            IL.Emit(OpCodes.Newobj, NullableBooleanCtor);
            IL.MarkLabel(end);
        }

        private void EmitNullEquality(ExpressionType op, Expression e, bool isLiftedToNull)
        {
            Debug.Assert(e.Type.IsNullable());
            Debug.Assert(op == ExpressionType.Equal || op == ExpressionType.NotEqual);
            // If we are lifted to null then just evaluate the expression for its side effects, discard,
            // and generate null.  If we are not lifted to null then generate a call to HasValue.
            if (isLiftedToNull)
            {
                EmitExpressionAsVoid(e);
                IL.EmitDefault(typeof(bool?), this);
            }
            else
            {
                EmitAddress(e, e.Type);
                IL.EmitHasValue(e.Type);
                if (op == ExpressionType.Equal)
                {
                    IL.Emit(OpCodes.Ldc_I4_0);
                    IL.Emit(OpCodes.Ceq);
                }
            }
        }

        // Shift operations have undefined behavior if the shift amount exceeds
        // the number of bits in the value operand. See CLI III.3.58 and C# 7.9
        // for the bit mask used below.
        private void EmitShiftMask(Type leftType)
        {
            var mask = leftType.IsInteger64() ? 0x3F : 0x1F;
            IL.EmitPrimitive(mask);
            IL.Emit(OpCodes.And);
        }

        private void EmitUnliftedBinaryOp(ExpressionType op, Type leftType, Type rightType)
        {
            Debug.Assert(!leftType.IsNullable());
            Debug.Assert(!rightType.IsNullable());
            Debug.Assert(leftType.IsPrimitive || (op == ExpressionType.Equal || op == ExpressionType.NotEqual) && (!leftType.IsValueType || leftType.IsEnum));

            switch (op)
            {
                case ExpressionType.NotEqual:
                    if (leftType.GetTypeCode() == TypeCode.Boolean)
                    {
                        goto case ExpressionType.ExclusiveOr;
                    }

                    IL.Emit(OpCodes.Ceq);
                    IL.Emit(OpCodes.Ldc_I4_0);
                    goto case ExpressionType.Equal;
                case ExpressionType.Equal:
                    IL.Emit(OpCodes.Ceq);
                    return;

                case ExpressionType.Add:
                    IL.Emit(OpCodes.Add);
                    break;

                case ExpressionType.AddChecked:
                    IL.Emit(leftType.IsFloatingPoint() ? OpCodes.Add : leftType.IsUnsigned() ? OpCodes.Add_Ovf_Un : OpCodes.Add_Ovf);
                    break;

                case ExpressionType.Subtract:
                    IL.Emit(OpCodes.Sub);
                    break;

                case ExpressionType.SubtractChecked:
                    if (leftType.IsUnsigned())
                    {
                        IL.Emit(OpCodes.Sub_Ovf_Un);
                        // Guaranteed to fit within result type: no conversion
                        return;
                    }
                    else
                    {
                        IL.Emit(leftType.IsFloatingPoint() ? OpCodes.Sub : OpCodes.Sub_Ovf);
                    }

                    break;

                case ExpressionType.Multiply:
                    IL.Emit(OpCodes.Mul);
                    break;

                case ExpressionType.MultiplyChecked:
                    IL.Emit(leftType.IsFloatingPoint() ? OpCodes.Mul : leftType.IsUnsigned() ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf);
                    break;

                case ExpressionType.Divide:
                    IL.Emit(leftType.IsUnsigned() ? OpCodes.Div_Un : OpCodes.Div);
                    break;

                case ExpressionType.Modulo:
                    IL.Emit(leftType.IsUnsigned() ? OpCodes.Rem_Un : OpCodes.Rem);
                    // Guaranteed to fit within result type: no conversion
                    return;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    IL.Emit(OpCodes.And);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    IL.Emit(OpCodes.Or);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.LessThan:
                    IL.Emit(leftType.IsUnsigned() ? OpCodes.Clt_Un : OpCodes.Clt);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.LessThanOrEqual:
                    IL.Emit(leftType.IsUnsigned() || leftType.IsFloatingPoint() ? OpCodes.Cgt_Un : OpCodes.Cgt);
                    IL.Emit(OpCodes.Ldc_I4_0);
                    IL.Emit(OpCodes.Ceq);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.GreaterThan:
                    IL.Emit(leftType.IsUnsigned() ? OpCodes.Cgt_Un : OpCodes.Cgt);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.GreaterThanOrEqual:
                    IL.Emit(leftType.IsUnsigned() || leftType.IsFloatingPoint() ? OpCodes.Clt_Un : OpCodes.Clt);
                    IL.Emit(OpCodes.Ldc_I4_0);
                    IL.Emit(OpCodes.Ceq);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.ExclusiveOr:
                    IL.Emit(OpCodes.Xor);
                    // Not an arithmetic operation: no conversion
                    return;

                case ExpressionType.LeftShift:
                    Debug.Assert(rightType == typeof(int));
                    EmitShiftMask(leftType);
                    IL.Emit(OpCodes.Shl);
                    break;

                case ExpressionType.RightShift:
                    Debug.Assert(rightType == typeof(int));
                    EmitShiftMask(leftType);
                    IL.Emit(leftType.IsUnsigned() ? OpCodes.Shr_Un : OpCodes.Shr);
                    // Guaranteed to fit within result type: no conversion
                    return;
                default:
                    break;
            }

            EmitConvertArithmeticResult(op, leftType);
        }
    }
}

#endif