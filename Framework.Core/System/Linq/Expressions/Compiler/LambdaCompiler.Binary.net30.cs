#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Dynamic.Utils;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions.Compiler
{
    internal partial class LambdaCompiler
    {
        private void EmitBinaryExpression(Expression expr)
        {
            EmitBinaryExpression(expr, CompilationFlags.EmitAsNoTail);
        }

        private void EmitBinaryExpression(Expression expr, CompilationFlags flags)
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
            if ((b.NodeType == ExpressionType.Equal || b.NodeType == ExpressionType.NotEqual) &&
                (b.Type == typeof(bool) || b.Type == typeof(bool?)))
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

        private void EmitNullEquality(ExpressionType op, Expression e, bool isLiftedToNull)
        {
            Debug.Assert(e.Type.IsNullable());
            Debug.Assert(op == ExpressionType.Equal || op == ExpressionType.NotEqual);
            // If we are lifted to null then just evaluate the expression for its side effects, discard,
            // and generate null.  If we are not lifted to null then generate a call to HasValue.
            if (isLiftedToNull)
            {
                EmitExpressionAsVoid(e);
                _ilg.EmitDefault(typeof(bool?));
            }
            else
            {
                EmitAddress(e, e.Type);
                _ilg.EmitHasValue(e.Type);
                if (op == ExpressionType.Equal)
                {
                    _ilg.Emit(OpCodes.Ldc_I4_0);
                    _ilg.Emit(OpCodes.Ceq);
                }
            }
        }

        private void EmitBinaryMethod(BinaryExpression b, CompilationFlags flags)
        {
            if (b.IsLifted)
            {
                var p1 = Expression.Variable(b.Left.Type.GetNonNullableType(), null);
                var p2 = Expression.Variable(b.Right.Type.GetNonNullableType(), null);
                var mc = Expression.Call(null, b.Method, p1, p2);
                Type resultType;
                if (b.IsLiftedToNull)
                {
                    resultType = mc.Type.GetNullableType();
                }
                else
                {
                    switch (b.NodeType)
                    {
                        case ExpressionType.Equal:
                        case ExpressionType.NotEqual:
                        case ExpressionType.LessThan:
                        case ExpressionType.LessThanOrEqual:
                        case ExpressionType.GreaterThan:
                        case ExpressionType.GreaterThanOrEqual:
                            if (mc.Type != typeof(bool))
                            {
                                throw Error.ArgumentMustBeBoolean();
                            }
                            resultType = typeof(bool);
                            break;

                        default:
                            resultType = mc.Type.GetNullableType();
                            break;
                    }
                }
                var variables = new[] { p1, p2 };
                var arguments = new[] { b.Left, b.Right };
                ValidateLift(variables, arguments);
                EmitLift(b.NodeType, resultType, mc, variables, arguments);
            }
            else
            {
                EmitMethodCallExpression(Expression.Call(null, b.Method, b.Left, b.Right), flags);
            }
        }

        private void EmitBinaryOperator(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
        {
            var leftIsNullable = leftType.IsNullable();
            var rightIsNullable = rightType.IsNullable();

            switch (op)
            {
                case ExpressionType.ArrayIndex:
                    if (rightType != typeof(int))
                    {
                        throw ContractUtils.Unreachable;
                    }
                    _ilg.EmitLoadElement(leftType.GetElementType());
                    return;

                case ExpressionType.Coalesce:
                    throw Error.UnexpectedCoalesceOperator();
                default:
                    break;
            }

            if (leftIsNullable || rightIsNullable)
            {
                EmitLiftedBinaryOp(op, leftType, rightType, resultType, liftedToNull);
            }
            else
            {
                EmitUnliftedBinaryOp(op, leftType, rightType);
                EmitConvertArithmeticResult(op, resultType);
            }
        }

        private void EmitUnliftedBinaryOp(ExpressionType op, Type leftType, Type rightType)
        {
            Debug.Assert(!leftType.IsNullable());
            Debug.Assert(!rightType.IsNullable());

            if (op == ExpressionType.Equal || op == ExpressionType.NotEqual)
            {
                EmitUnliftedEquality(op, leftType);
                return;
            }
            if (!leftType.IsPrimitive)
            {
                throw Error.OperatorNotImplementedForType(op, leftType);
            }
            switch (op)
            {
                case ExpressionType.Add:
                    _ilg.Emit(OpCodes.Add);
                    break;

                case ExpressionType.AddChecked:
                    if (leftType.IsFloatingPoint())
                    {
                        _ilg.Emit(OpCodes.Add);
                    }
                    else if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Add_Ovf_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Add_Ovf);
                    }
                    break;

                case ExpressionType.Subtract:
                    _ilg.Emit(OpCodes.Sub);
                    break;

                case ExpressionType.SubtractChecked:
                    if (leftType.IsFloatingPoint())
                    {
                        _ilg.Emit(OpCodes.Sub);
                    }
                    else if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Sub_Ovf_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Sub_Ovf);
                    }
                    break;

                case ExpressionType.Multiply:
                    _ilg.Emit(OpCodes.Mul);
                    break;

                case ExpressionType.MultiplyChecked:
                    if (leftType.IsFloatingPoint())
                    {
                        _ilg.Emit(OpCodes.Mul);
                    }
                    else if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Mul_Ovf_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Mul_Ovf);
                    }
                    break;

                case ExpressionType.Divide:
                    if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Div_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Div);
                    }
                    break;

                case ExpressionType.Modulo:
                    if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Rem_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Rem);
                    }
                    break;

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    _ilg.Emit(OpCodes.And);
                    break;

                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    _ilg.Emit(OpCodes.Or);
                    break;

                case ExpressionType.LessThan:
                    if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Clt_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Clt);
                    }
                    break;

                case ExpressionType.LessThanOrEqual:
                    {
                        var labFalse = _ilg.DefineLabel();
                        var labEnd = _ilg.DefineLabel();
                        if (leftType.IsUnsigned())
                        {
                            _ilg.Emit(OpCodes.Ble_Un_S, labFalse);
                        }
                        else
                        {
                            _ilg.Emit(OpCodes.Ble_S, labFalse);
                        }
                        _ilg.Emit(OpCodes.Ldc_I4_0);
                        _ilg.Emit(OpCodes.Br_S, labEnd);
                        _ilg.MarkLabel(labFalse);
                        _ilg.Emit(OpCodes.Ldc_I4_1);
                        _ilg.MarkLabel(labEnd);
                    }
                    break;

                case ExpressionType.GreaterThan:
                    if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Cgt_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Cgt);
                    }
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    {
                        var labFalse = _ilg.DefineLabel();
                        var labEnd = _ilg.DefineLabel();
                        if (leftType.IsUnsigned())
                        {
                            _ilg.Emit(OpCodes.Bge_Un_S, labFalse);
                        }
                        else
                        {
                            _ilg.Emit(OpCodes.Bge_S, labFalse);
                        }
                        _ilg.Emit(OpCodes.Ldc_I4_0);
                        _ilg.Emit(OpCodes.Br_S, labEnd);
                        _ilg.MarkLabel(labFalse);
                        _ilg.Emit(OpCodes.Ldc_I4_1);
                        _ilg.MarkLabel(labEnd);
                    }
                    break;

                case ExpressionType.ExclusiveOr:
                    _ilg.Emit(OpCodes.Xor);
                    break;

                case ExpressionType.LeftShift:
                    if (rightType != typeof(int))
                    {
                        throw ContractUtils.Unreachable;
                    }
                    _ilg.Emit(OpCodes.Shl);
                    break;

                case ExpressionType.RightShift:
                    if (rightType != typeof(int))
                    {
                        throw ContractUtils.Unreachable;
                    }
                    if (leftType.IsUnsigned())
                    {
                        _ilg.Emit(OpCodes.Shr_Un);
                    }
                    else
                    {
                        _ilg.Emit(OpCodes.Shr);
                    }
                    break;

                default:
                    throw Error.UnhandledBinary(op);
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
                    _ilg.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_U1 : OpCodes.Conv_U1);
                    break;

                case TypeCode.SByte:
                    _ilg.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_I1 : OpCodes.Conv_I1);
                    break;

                case TypeCode.UInt16:
                    _ilg.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_U2 : OpCodes.Conv_U2);
                    break;

                case TypeCode.Int16:
                    _ilg.Emit(IsChecked(op) ? OpCodes.Conv_Ovf_I2 : OpCodes.Conv_I2);
                    break;

                default:
                    break;
            }
        }

        private void EmitUnliftedEquality(ExpressionType op, Type type)
        {
            Debug.Assert(op == ExpressionType.Equal || op == ExpressionType.NotEqual);
            if (!type.IsPrimitive && type.IsValueType && !type.IsEnum)
            {
                throw Error.OperatorNotImplementedForType(op, type);
            }
            _ilg.Emit(OpCodes.Ceq);
            if (op == ExpressionType.NotEqual)
            {
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
            }
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
                    EmitLiftedRelational(op, leftType, rightType, resultType, liftedToNull);
                    break;

                // case ExpressionType.AndAlso:
                // case ExpressionType.OrElse:
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private void EmitLiftedRelational(ExpressionType op, Type leftType, Type rightType, Type resultType, bool liftedToNull)
        {
            Debug.Assert(leftType.IsNullable());

            var shortCircuit = _ilg.DefineLabel();
            var locLeft = GetLocal(leftType);
            var locRight = GetLocal(rightType);

            // store values (reverse order since they are already on the stack)
            _ilg.Emit(OpCodes.Stloc, locRight);
            _ilg.Emit(OpCodes.Stloc, locLeft);

            if (op == ExpressionType.Equal)
            {
                // test for both null -> true
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
                _ilg.Emit(OpCodes.And);
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Brtrue_S, shortCircuit);
                _ilg.Emit(OpCodes.Pop);

                // test for either is null -> false
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.And);

                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Brfalse_S, shortCircuit);
                _ilg.Emit(OpCodes.Pop);
            }
            else if (op == ExpressionType.NotEqual)
            {
                // test for both null -> false
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.Or);
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Brfalse_S, shortCircuit);
                _ilg.Emit(OpCodes.Pop);

                // test for either is null -> true
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.Ldc_I4_0);
                _ilg.Emit(OpCodes.Ceq);
                _ilg.Emit(OpCodes.Or);
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Brtrue_S, shortCircuit);
                _ilg.Emit(OpCodes.Pop);
            }
            else
            {
                // test for either is null -> false
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.And);
                _ilg.Emit(OpCodes.Dup);
                _ilg.Emit(OpCodes.Brfalse_S, shortCircuit);
                _ilg.Emit(OpCodes.Pop);
            }

            // do op on values
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitGetValueOrDefault(leftType);
            _ilg.Emit(OpCodes.Ldloca, locRight);
            _ilg.EmitGetValueOrDefault(rightType);

            //RELEASING locLeft locRight
            FreeLocal(locLeft);
            FreeLocal(locRight);

            EmitBinaryOperator(
                op,
                leftType.GetNonNullableType(),
                rightType.GetNonNullableType(),
                resultType.GetNonNullableType(),
                false
            );

            if (!liftedToNull)
            {
                _ilg.MarkLabel(shortCircuit);
            }

            if (resultType != resultType.GetNonNullableType())
            {
                _ilg.EmitConvertToType(resultType.GetNonNullableType(), resultType, true);
            }

            if (liftedToNull)
            {
                var labEnd = _ilg.DefineLabel();
                _ilg.Emit(OpCodes.Br, labEnd);
                _ilg.MarkLabel(shortCircuit);
                _ilg.Emit(OpCodes.Pop);
                _ilg.Emit(OpCodes.Ldnull);
                _ilg.Emit(OpCodes.Unbox_Any, resultType);
                _ilg.MarkLabel(labEnd);
            }
        }

        private void EmitLiftedBinaryArithmetic(ExpressionType op, Type leftType, Type rightType, Type resultType)
        {
            var leftIsNullable = leftType.IsNullable();
            var rightIsNullable = rightType.IsNullable();

            Debug.Assert(leftIsNullable || rightIsNullable);

            var labIfNull = _ilg.DefineLabel();
            var labEnd = _ilg.DefineLabel();
            var locLeft = GetLocal(leftType);
            var locRight = GetLocal(rightType);
            var locResult = GetLocal(resultType);

            // store values (reverse order since they are already on the stack)
            _ilg.Emit(OpCodes.Stloc, locRight);
            _ilg.Emit(OpCodes.Stloc, locLeft);

            // test for null
            // use short circuiting
            if (leftIsNullable)
            {
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitHasValue(leftType);
                _ilg.Emit(OpCodes.Brfalse_S, labIfNull);
            }
            if (rightIsNullable)
            {
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitHasValue(rightType);
                _ilg.Emit(OpCodes.Brfalse_S, labIfNull);
            }

            // do op on values
            if (leftIsNullable)
            {
                _ilg.Emit(OpCodes.Ldloca, locLeft);
                _ilg.EmitGetValueOrDefault(leftType);
            }
            else
            {
                _ilg.Emit(OpCodes.Ldloc, locLeft);
            }

            if (rightIsNullable)
            {
                _ilg.Emit(OpCodes.Ldloca, locRight);
                _ilg.EmitGetValueOrDefault(rightType);
            }
            else
            {
                _ilg.Emit(OpCodes.Ldloc, locRight);
            }

            //RELEASING locLeft locRight
            FreeLocal(locLeft);
            FreeLocal(locRight);

            EmitBinaryOperator(op, leftType.GetNonNullableType(), rightType.GetNonNullableType(), resultType.GetNonNullableType(), false);

            // construct result type
            var ci = resultType.GetConstructor(new[] { resultType.GetNonNullableType() });
            _ilg.Emit(OpCodes.Newobj, ci);
            _ilg.Emit(OpCodes.Stloc, locResult);
            _ilg.Emit(OpCodes.Br_S, labEnd);

            // if null then create a default one
            _ilg.MarkLabel(labIfNull);
            _ilg.Emit(OpCodes.Ldloca, locResult);
            _ilg.Emit(OpCodes.Initobj, resultType);

            _ilg.MarkLabel(labEnd);

            _ilg.Emit(OpCodes.Ldloc, locResult);

            //RELEASING locResult
            FreeLocal(locResult);
        }

        private void EmitLiftedBooleanAnd()
        {
            var type = typeof(bool?);
            var labComputeRight = _ilg.DefineLabel();
            var labReturnFalse = _ilg.DefineLabel();
            var labReturnNull = _ilg.DefineLabel();
            var labReturnValue = _ilg.DefineLabel();
            var labExit = _ilg.DefineLabel();

            // store values (reverse order since they are already on the stack)
            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);
            _ilg.Emit(OpCodes.Stloc, locRight);
            _ilg.Emit(OpCodes.Stloc, locLeft);

            // compute left
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse, labComputeRight);
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitGetValueOrDefault(type);
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Ceq);
            _ilg.Emit(OpCodes.Brtrue, labReturnFalse);

            // compute right
            _ilg.MarkLabel(labComputeRight);
            _ilg.Emit(OpCodes.Ldloca, locRight);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse_S, labReturnNull);
            _ilg.Emit(OpCodes.Ldloca, locRight);

            //RELEASING locRight
            FreeLocal(locRight);

            _ilg.EmitGetValueOrDefault(type);
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Ceq);
            _ilg.Emit(OpCodes.Brtrue_S, labReturnFalse);

            // check left for null again
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse, labReturnNull);

            // return true
            _ilg.Emit(OpCodes.Ldc_I4_1);
            _ilg.Emit(OpCodes.Br_S, labReturnValue);

            // return false
            _ilg.MarkLabel(labReturnFalse);
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Br_S, labReturnValue);

            _ilg.MarkLabel(labReturnValue);
            var ci = type.GetConstructor(new[] { typeof(bool) });
            _ilg.Emit(OpCodes.Newobj, ci);
            _ilg.Emit(OpCodes.Stloc, locLeft);
            _ilg.Emit(OpCodes.Br, labExit);

            // return null
            _ilg.MarkLabel(labReturnNull);
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.Emit(OpCodes.Initobj, type);

            _ilg.MarkLabel(labExit);
            _ilg.Emit(OpCodes.Ldloc, locLeft);

            //RELEASING locLeft
            FreeLocal(locLeft);
        }

        private void EmitLiftedBooleanOr()
        {
            var type = typeof(bool?);
            var labComputeRight = _ilg.DefineLabel();
            var labReturnTrue = _ilg.DefineLabel();
            var labReturnNull = _ilg.DefineLabel();
            var labReturnValue = _ilg.DefineLabel();
            var labExit = _ilg.DefineLabel();

            // store values (reverse order since they are already on the stack)
            var locLeft = GetLocal(type);
            var locRight = GetLocal(type);
            _ilg.Emit(OpCodes.Stloc, locRight);
            _ilg.Emit(OpCodes.Stloc, locLeft);

            // compute left
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse, labComputeRight);
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitGetValueOrDefault(type);
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Ceq);
            _ilg.Emit(OpCodes.Brfalse, labReturnTrue);

            // compute right
            _ilg.MarkLabel(labComputeRight);
            _ilg.Emit(OpCodes.Ldloca, locRight);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse_S, labReturnNull);
            _ilg.Emit(OpCodes.Ldloca, locRight);

            //RELEASING locRight
            FreeLocal(locRight);

            _ilg.EmitGetValueOrDefault(type);
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Ceq);
            _ilg.Emit(OpCodes.Brfalse_S, labReturnTrue);

            // check left for null again
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.EmitHasValue(type);
            _ilg.Emit(OpCodes.Brfalse, labReturnNull);

            // return false
            _ilg.Emit(OpCodes.Ldc_I4_0);
            _ilg.Emit(OpCodes.Br_S, labReturnValue);

            // return true
            _ilg.MarkLabel(labReturnTrue);
            _ilg.Emit(OpCodes.Ldc_I4_1);
            _ilg.Emit(OpCodes.Br_S, labReturnValue);

            _ilg.MarkLabel(labReturnValue);
            var ci = type.GetConstructor(new[] { typeof(bool) });
            _ilg.Emit(OpCodes.Newobj, ci);
            _ilg.Emit(OpCodes.Stloc, locLeft);
            _ilg.Emit(OpCodes.Br, labExit);

            // return null
            _ilg.MarkLabel(labReturnNull);
            _ilg.Emit(OpCodes.Ldloca, locLeft);
            _ilg.Emit(OpCodes.Initobj, type);

            _ilg.MarkLabel(labExit);
            _ilg.Emit(OpCodes.Ldloc, locLeft);

            //RELEASING locLeft
            FreeLocal(locLeft);
        }
    }
}

#endif