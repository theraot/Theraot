#if NET20 ||NET30

using System.Reflection;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public sealed class UnaryExpression : Expression
    {
        private bool _isLifted;
        private MethodInfo _method;
        private Expression _operand;

        internal UnaryExpression(ExpressionType nodeType, Expression operand, Type type)
            : base(nodeType, type)
        {
            _operand = operand;
        }

        internal UnaryExpression(ExpressionType nodeType, Expression operand, Type type, MethodInfo method, bool isLifted)
            : base(nodeType, type)
        {
            _operand = operand;
            _method = method;
            _isLifted = isLifted;
        }

        public bool IsLifted
        {
            get
            {
                return _isLifted;
            }
        }

        public bool IsLiftedToNull
        {
            get
            {
                return _isLifted && Type.IsNullable();
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public Expression Operand
        {
            get
            {
                return _operand;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            if (_method != null)
            {
                EmitUserDefinedOperator(emitContext);
                return;
            }
            switch (NodeType)
            {
                case ExpressionType.ArrayLength:
                    EmitArrayLength(emitContext);
                    return;

                case ExpressionType.TypeAs:
                    EmitTypeAs(emitContext);
                    return;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    EmitConvert(emitContext);
                    return;

                case ExpressionType.Not:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                    EmitArithmeticUnary(emitContext);
                    return;

                case ExpressionType.Quote:
                    EmitQuote(emitContext);
                    return;

                default:
                    throw new NotImplementedException(NodeType.ToString());
            }
        }

        private void EmitArithmeticUnary(EmitContext emitContext)
        {
            if (!IsLifted)
            {
                _operand.Emit(emitContext);
                EmitUnaryOperator(emitContext);
            }
            else
            {
                EmitLiftedUnary(emitContext);
            }
        }

        private void EmitArrayLength(EmitContext emitContext)
        {
            _operand.Emit(emitContext);
            emitContext.ILGenerator.Emit(OpCodes.Ldlen);
        }

        private void EmitBox(EmitContext emitContext)
        {
            emitContext.ILGenerator.Emit(OpCodes.Box, _operand.Type);
        }

        private void EmitCast(EmitContext emitContext)
        {
            _operand.Emit(emitContext);
            if (IsBoxing())
            {
                EmitBox(emitContext);
            }
            else if (IsUnBoxing())
            {
                EmitUnbox(emitContext);
            }
            else
            {
                emitContext.ILGenerator.Emit(OpCodes.Castclass, Type);
            }
        }

        private void EmitConvert(EmitContext emitContext)
        {
            var from = _operand.Type;
            var target = Type;
            if (from == target)
            {
                _operand.Emit(emitContext);
            }
            else if (from.IsNullable() && !target.IsNullable())
            {
                EmitConvertFromNullable(emitContext);
            }
            else if (!from.IsNullable() && target.IsNullable())
            {
                EmitConvertToNullable(emitContext);
            }
            else if (from.IsNullable() && target.IsNullable())
            {
                EmitConvertFromNullableToNullable(emitContext);
            }
            else if (IsReferenceConversion(from, target))
            {
                EmitCast(emitContext);
            }
            else if (IsPrimitiveConversion(from, target))
            {
                EmitPrimitiveConversion(emitContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private void EmitConvertFromNullable(EmitContext emitContext)
        {
            if (IsBoxing())
            {
                emitContext.Emit(_operand);
                EmitBox(emitContext);
                return;
            }
            emitContext.EmitCall(_operand, _operand.Type.GetMethod("get_Value"));
            if (_operand.Type.GetNotNullableType() != Type)
            {
                EmitPrimitiveConversion(emitContext, _operand.Type.GetNotNullableType(), Type);
            }
        }

        private void EmitConvertFromNullableToNullable(EmitContext emitContext)
        {
            EmitLiftedUnary(emitContext);
        }

        private void EmitConvertToNullable(EmitContext emitContext)
        {
            emitContext.Emit(_operand);
            if (IsUnBoxing())
            {
                EmitUnbox(emitContext);
                return;
            }
            if (_operand.Type != Type.GetNotNullableType())
            {
                EmitPrimitiveConversion(emitContext, _operand.Type, Type.GetNotNullableType());
            }
            emitContext.EmitNullableNew(Type);
        }

        private void EmitLiftedUnary(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var from = emitContext.EmitStored(_operand);
            var to = ig.DeclareLocal(Type);
            var has_value = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.EmitNullableHasValue(from);
            ig.Emit(OpCodes.Brtrue, has_value);

            // if not has value
            emitContext.EmitNullableInitialize(to);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(has_value);

            // if has value
            emitContext.EmitNullableGetValueOrDefault(from);
            EmitUnaryOperator(emitContext);
            emitContext.EmitNullableNew(Type);
            ig.MarkLabel(done);
        }

        private void EmitPrimitiveConversion(EmitContext emitContext, bool isUnsigned, OpCode signed, OpCode unsigned, OpCode signedChecked, OpCode unsignedChecked)
        {
            if (NodeType != ExpressionType.ConvertChecked)
            {
                emitContext.ILGenerator.Emit(isUnsigned ? unsigned : signed);
            }
            else
            {
                emitContext.ILGenerator.Emit(isUnsigned ? unsignedChecked : signedChecked);
            }
        }

        private void EmitPrimitiveConversion(EmitContext emitContext)
        {
            _operand.Emit(emitContext);
            EmitPrimitiveConversion(emitContext, _operand.Type, Type);
        }

        private void EmitPrimitiveConversion(EmitContext emitContext, Type from, Type to)
        {
            var is_unsigned = IsUnsigned(from);
            switch (Type.GetTypeCode(to))
            {
                case TypeCode.SByte:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I1,
                        OpCodes.Conv_U1,
                        OpCodes.Conv_Ovf_I1,
                        OpCodes.Conv_Ovf_I1_Un
                    );
                    return;

                case TypeCode.Byte:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I1,
                        OpCodes.Conv_U1,
                        OpCodes.Conv_Ovf_U1,
                        OpCodes.Conv_Ovf_U1_Un
                    );
                    return;

                case TypeCode.Int16:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I2,
                        OpCodes.Conv_U2,
                        OpCodes.Conv_Ovf_I2,
                        OpCodes.Conv_Ovf_I2_Un
                    );
                    return;

                case TypeCode.UInt16:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I2,
                        OpCodes.Conv_U2,
                        OpCodes.Conv_Ovf_U2,
                        OpCodes.Conv_Ovf_U2_Un
                    );
                    return;

                case TypeCode.Int32:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I4,
                        OpCodes.Conv_U4,
                        OpCodes.Conv_Ovf_I4,
                        OpCodes.Conv_Ovf_I4_Un
                    );
                    return;

                case TypeCode.UInt32:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I4,
                        OpCodes.Conv_U4,
                        OpCodes.Conv_Ovf_U4,
                        OpCodes.Conv_Ovf_U4_Un
                    );
                    return;

                case TypeCode.Int64:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I8,
                        OpCodes.Conv_U8,
                        OpCodes.Conv_Ovf_I8,
                        OpCodes.Conv_Ovf_I8_Un
                    );
                    return;

                case TypeCode.UInt64:
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        is_unsigned,
                        OpCodes.Conv_I8,
                        OpCodes.Conv_U8,
                        OpCodes.Conv_Ovf_U8,
                        OpCodes.Conv_Ovf_U8_Un
                    );
                    return;

                case TypeCode.Single:
                    if (is_unsigned)
                    {
                        emitContext.ILGenerator.Emit(OpCodes.Conv_R_Un);
                    }
                    emitContext.ILGenerator.Emit(OpCodes.Conv_R4);
                    return;

                case TypeCode.Double:
                    if (is_unsigned)
                    {
                        emitContext.ILGenerator.Emit(OpCodes.Conv_R_Un);
                    }
                    emitContext.ILGenerator.Emit(OpCodes.Conv_R8);
                    return;

                default:
                    throw new NotImplementedException(Type.ToString());
            }
        }

        private void EmitQuote(EmitContext emitContext)
        {
            emitContext.EmitScope();
            emitContext.EmitReadGlobal(_operand, typeof(Expression));
            if (emitContext.HasHoistedLocals)
            {
                emitContext.EmitLoadHoistedLocalsStore();
            }
            else
            {
                emitContext.ILGenerator.Emit(OpCodes.Ldnull);
            }
            emitContext.EmitIsolateExpression();
        }

        private void EmitTypeAs(EmitContext emitContext)
        {
            var type = Type;
            emitContext.EmitIsInst(_operand, type);
            if (type.IsNullable())
            {
                emitContext.ILGenerator.Emit(OpCodes.Unbox_Any, type);
            }
        }

        private void EmitUnaryOperator(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            switch (NodeType)
            {
                case ExpressionType.Not:
                    if (_operand.Type.GetNotNullableType() == typeof(bool))
                    {
                        ig.Emit(OpCodes.Ldc_I4_0);
                        ig.Emit(OpCodes.Ceq);
                    }
                    else
                    {
                        ig.Emit(OpCodes.Not);
                    }
                    break;

                case ExpressionType.Negate:
                    ig.Emit(OpCodes.Neg);
                    break;

                case ExpressionType.NegateChecked:
                    ig.Emit(OpCodes.Ldc_I4_M1);
                    ig.Emit(IsUnsigned(_operand.Type) ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf);
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:

                    // Called when converting from nullable from nullable
                    EmitPrimitiveConversion
                    (
                        emitContext,
                        _operand.Type.GetNotNullableType(),
                        Type.GetNotNullableType()
                    );
                    break;
            }
        }

        private void EmitUnbox(EmitContext emitContext)
        {
            emitContext.ILGenerator.Emit(OpCodes.Unbox_Any, Type);
        }

        private void EmitUserDefinedLiftedOperator(EmitContext emitContext)
        {
            var local = emitContext.EmitStored(_operand);
            emitContext.EmitNullableGetValue(local);
            emitContext.EmitCall(_method);
        }

        private void EmitUserDefinedLiftedToNullOperator(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var local = emitContext.EmitStored(_operand);
            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.EmitNullableHasValue(local);
            ig.Emit(OpCodes.Brfalse, ret);
            emitContext.EmitNullableGetValueOrDefault(local);
            emitContext.EmitCall(_method);
            emitContext.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret);
            var temp = ig.DeclareLocal(Type);
            emitContext.EmitNullableInitialize(temp);
            ig.MarkLabel(done);
        }

        private void EmitUserDefinedOperator(EmitContext emitContext)
        {
            if (!IsLifted)
            {
                emitContext.Emit(_operand);
                emitContext.EmitCall(_method);
            }
            else if (IsLiftedToNull)
            {
                EmitUserDefinedLiftedToNullOperator(emitContext);
            }
            else
            {
                EmitUserDefinedLiftedOperator(emitContext);
            }
        }

        private bool IsBoxing()
        {
            return _operand.Type.IsValueType && !Type.IsValueType;
        }

        private bool IsUnBoxing()
        {
            return !_operand.Type.IsValueType && Type.IsValueType;
        }
    }
}

#endif