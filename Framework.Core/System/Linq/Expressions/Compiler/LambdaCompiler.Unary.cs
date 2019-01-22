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
        private void EmitConstantOne(Type type)
        {
            switch (type.GetTypeCode())
            {
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    IL.Emit(OpCodes.Ldc_I4_1);
                    IL.Emit(OpCodes.Conv_I8);
                    break;

                case TypeCode.Single:
                    IL.Emit(OpCodes.Ldc_R4, 1.0f);
                    break;

                case TypeCode.Double:
                    IL.Emit(OpCodes.Ldc_R8, 1.0d);
                    break;

                default:
                    IL.Emit(OpCodes.Ldc_I4_1);
                    break;
            }
        }

        private void EmitConvert(UnaryExpression node, CompilationFlags flags)
        {
            if (node.Method != null)
            {
                // User-defined conversions are only lifted if both source and
                // destination types are value types.  The C# compiler gets this wrong.
                // In C#, if you have an implicit conversion from int->MyClass and you
                // "lift" the conversion to int?->MyClass then a null int? goes to a
                // null MyClass.  This is contrary to the specification, which states
                // that the correct behaviour is to unwrap the int?, throw an exception
                // if it is null, and then call the conversion.
                //
                // We cannot fix this in C# but there is no reason why we need to
                // propagate this behavior into the expression tree API.  Unfortunately
                // this means that when the C# compiler generates the lambda
                // (int? i)=>(MyClass)i, we will get different results for converting
                // that lambda to a delegate directly and converting that lambda to
                // an expression tree and then compiling it.  We can live with this
                // discrepancy however.

                if (node.IsLifted && (!node.Type.IsValueType || !node.Operand.Type.IsValueType))
                {
                    var pis = node.Method.GetParameters();
                    Debug.Assert(pis.Length == 1);
                    var paramType = pis[0].ParameterType;
                    if (paramType.IsByRef)
                    {
                        paramType = paramType.GetElementType();
                    }

                    var operand = Expression.Convert(node.Operand, paramType);
                    Debug.Assert(operand.Method == null);

                    node = Expression.Convert(Expression.Call(node.Method, operand), node.Type);

                    Debug.Assert(node.Method == null);
                }
                else
                {
                    EmitUnaryMethod(node, flags);
                    return;
                }
            }

            if (node.Type == typeof(void))
            {
                EmitExpressionAsVoid(node.Operand, flags);
            }
            else
            {
                if (TypeUtils.AreEquivalent(node.Operand.Type, node.Type))
                {
                    EmitExpression(node.Operand, flags);
                }
                else
                {
                    // A conversion is emitted after emitting the operand, no tail call is emitted
                    EmitExpression(node.Operand);
                    IL.EmitConvertToType(node.Operand.Type, node.Type, node.NodeType == ExpressionType.ConvertChecked, this);
                }
            }
        }

        private void EmitConvertUnaryExpression(Expression expr, CompilationFlags flags)
        {
            EmitConvert((UnaryExpression)expr, flags);
        }

        private void EmitQuote(UnaryExpression quote)
        {
            // emit the quoted expression as a runtime constant
            EmitConstant(quote.Operand, quote.Type);

            // Heuristic: only emit the tree rewrite logic if we have hoisted
            // locals.
            if (_scope.NearestHoistedLocals != null)
            {
                // HoistedLocals is internal so emit as System.Object
                EmitConstant(_scope.NearestHoistedLocals, typeof(object));
                _scope.EmitGet(_scope.NearestHoistedLocals.SelfVariable);
                IL.Emit(OpCodes.Call, CachedReflectionInfo.RuntimeOpsQuote);

                Debug.Assert(typeof(LambdaExpression).IsAssignableFrom(quote.Type));
                IL.Emit(OpCodes.Castclass, quote.Type);
            }
        }

        private void EmitQuoteUnaryExpression(Expression expr)
        {
            EmitQuote((UnaryExpression)expr);
        }

        private void EmitThrow(UnaryExpression expr, CompilationFlags flags)
        {
            if (expr.Operand == null)
            {
                CheckRethrow();

                IL.Emit(OpCodes.Rethrow);
            }
            else
            {
                EmitExpression(expr.Operand);
                IL.Emit(OpCodes.Throw);
            }

            EmitUnreachable(expr, flags);
        }

        private void EmitThrowUnaryExpression(Expression expr)
        {
            EmitThrow((UnaryExpression)expr, CompilationFlags.EmitAsDefaultType);
        }

        private void EmitUnary(UnaryExpression node, CompilationFlags flags)
        {
            if (node.Method != null)
            {
                EmitUnaryMethod(node, flags);
            }
            else if (node.NodeType == ExpressionType.NegateChecked && node.Operand.Type.IsInteger())
            {
                var type = node.Type;
                Debug.Assert(type == node.Operand.Type);
                if (type.IsNullable())
                {
                    var nullOrZero = IL.DefineLabel();
                    var end = IL.DefineLabel();
                    EmitExpression(node.Operand);
                    var loc = GetLocal(type);

                    // check for null or zero
                    IL.Emit(OpCodes.Stloc, loc);
                    IL.Emit(OpCodes.Ldloca, loc);
                    IL.EmitGetValueOrDefault(type);
                    IL.Emit(OpCodes.Brfalse_S, nullOrZero);

                    // calculate 0 - operand
                    var nnType = type.GetNonNullable();
                    IL.EmitDefault(nnType, null); // locals won't be used.
                    IL.Emit(OpCodes.Ldloca, loc);
                    IL.EmitGetValueOrDefault(type);
                    EmitBinaryOperator(ExpressionType.SubtractChecked, nnType, nnType, nnType, false);

                    // construct result
                    // ReSharper disable once AssignNullToNotNullAttribute
                    IL.Emit(OpCodes.Newobj, type.GetConstructor(new[] {nnType}));
                    IL.Emit(OpCodes.Br_S, end);

                    // if null then push back on stack
                    IL.MarkLabel(nullOrZero);
                    IL.Emit(OpCodes.Ldloc, loc);
                    FreeLocal(loc);
                    IL.MarkLabel(end);
                }
                else
                {
                    IL.EmitDefault(type, null); // locals won't be used.
                    EmitExpression(node.Operand);
                    EmitBinaryOperator(ExpressionType.SubtractChecked, type, type, type, false);
                }
            }
            else
            {
                EmitExpression(node.Operand);
                EmitUnaryOperator(node.NodeType, node.Operand.Type, node.Type);
            }
        }

        private void EmitUnaryExpression(Expression expr, CompilationFlags flags)
        {
            EmitUnary((UnaryExpression)expr, flags);
        }

        private void EmitUnaryMethod(UnaryExpression node, CompilationFlags flags)
        {
            if (node.IsLifted)
            {
                var v = Expression.Variable(node.Operand.Type.GetNonNullable(), null);
                var mc = Expression.Call(node.Method, v);

                var resultType = mc.Type.GetNullable();
                EmitLift(node.NodeType, resultType, mc, new[] {v}, new[] {node.Operand});
                IL.EmitConvertToType(resultType, node.Type, false, this);
            }
            else
            {
                EmitMethodCallExpression(Expression.Call(node.Method, node.Operand), flags);
            }
        }

        private void EmitUnaryOperator(ExpressionType op, Type operandType, Type resultType)
        {
            var operandIsNullable = operandType.IsNullable();

            if (op == ExpressionType.ArrayLength)
            {
                IL.Emit(OpCodes.Ldlen);
                return;
            }

            if (operandIsNullable)
            {
                switch (op)
                {
                    case ExpressionType.UnaryPlus:
                        return;

                    case ExpressionType.TypeAs:
                        if (operandType != resultType)
                        {
                            IL.Emit(OpCodes.Box, operandType);
                            IL.Emit(OpCodes.Isinst, resultType);
                            if (resultType.IsNullable())
                            {
                                IL.Emit(OpCodes.Unbox_Any, resultType);
                            }
                        }

                        return;

                    default:
                        Debug.Assert(TypeUtils.AreEquivalent(operandType, resultType));
                        var labIfNull = IL.DefineLabel();
                        var labEnd = IL.DefineLabel();
                        var loc = GetLocal(operandType);

                        // check for null
                        IL.Emit(OpCodes.Stloc, loc);
                        IL.Emit(OpCodes.Ldloca, loc);
                        IL.EmitHasValue(operandType);
                        IL.Emit(OpCodes.Brfalse_S, labIfNull);

                        // apply operator to non-null value
                        IL.Emit(OpCodes.Ldloca, loc);
                        IL.EmitGetValueOrDefault(operandType);
                        var nnOperandType = resultType.GetNonNullable();
                        EmitUnaryOperator(op, nnOperandType, nnOperandType);

                        // construct result
                        var ci = resultType.GetConstructor(new[] {nnOperandType});
                        // ReSharper disable once AssignNullToNotNullAttribute
                        IL.Emit(OpCodes.Newobj, ci);
                        IL.Emit(OpCodes.Br_S, labEnd);

                        // if null then push back on stack.
                        IL.MarkLabel(labIfNull);
                        IL.Emit(OpCodes.Ldloc, loc);
                        FreeLocal(loc);
                        IL.MarkLabel(labEnd);
                        return;
                }
            }

            switch (op)
            {
                case ExpressionType.Not:
                    if (operandType == typeof(bool))
                    {
                        IL.Emit(OpCodes.Ldc_I4_0);
                        IL.Emit(OpCodes.Ceq);
                        return;
                    }

                    goto case ExpressionType.OnesComplement;
                case ExpressionType.OnesComplement:
                    IL.Emit(OpCodes.Not);
                    if (!operandType.IsUnsigned())
                    {
                        // Guaranteed to fit within result type: no conversion
                        return;
                    }

                    break;

                case ExpressionType.IsFalse:
                    IL.Emit(OpCodes.Ldc_I4_0);
                    IL.Emit(OpCodes.Ceq);
                    // Not an arithmetic operation -> no conversion
                    return;

                case ExpressionType.IsTrue:
                    IL.Emit(OpCodes.Ldc_I4_1);
                    IL.Emit(OpCodes.Ceq);
                    // Not an arithmetic operation -> no conversion
                    return;

                case ExpressionType.UnaryPlus:
                    // Guaranteed to fit within result type: no conversion
                    return;

                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                    IL.Emit(OpCodes.Neg);
                    // Guaranteed to fit within result type: no conversion
                    // (integer NegateChecked was rewritten to 0 - operand and doesn't hit here).
                    return;

                case ExpressionType.TypeAs:
                    if (operandType != resultType)
                    {
                        if (operandType.IsValueType)
                        {
                            IL.Emit(OpCodes.Box, operandType);
                        }

                        IL.Emit(OpCodes.Isinst, resultType);
                        if (resultType.IsNullable())
                        {
                            IL.Emit(OpCodes.Unbox_Any, resultType);
                        }
                    }

                    // Not an arithmetic operation -> no conversion
                    return;

                case ExpressionType.Increment:
                    EmitConstantOne(resultType);
                    IL.Emit(OpCodes.Add);
                    break;

                case ExpressionType.Decrement:
                    EmitConstantOne(resultType);
                    IL.Emit(OpCodes.Sub);
                    break;
                default:
                    break;
            }

            EmitConvertArithmeticResult(op, resultType);
        }

        private void EmitUnboxUnaryExpression(Expression expr)
        {
            var node = (UnaryExpression)expr;
            Debug.Assert(node.Type.IsValueType);

            // Unbox_Any leaves the value on the stack
            EmitExpression(node.Operand);
            IL.Emit(OpCodes.Unbox_Any, node.Type);
        }
    }
}

#endif