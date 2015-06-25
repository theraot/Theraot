#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public sealed class UnaryExpression : Expression
    {
        private bool is_lifted;
        private MethodInfo method;
        private Expression operand;

        internal UnaryExpression(ExpressionType node_type, Expression operand, Type type)
            : base(node_type, type)
        {
            this.operand = operand;
        }

        internal UnaryExpression(ExpressionType node_type, Expression operand, Type type, MethodInfo method, bool is_lifted)
            : base(node_type, type)
        {
            this.operand = operand;
            this.method = method;
            this.is_lifted = is_lifted;
        }

        public bool IsLifted
        {
            get { return is_lifted; }
        }

        public bool IsLiftedToNull
        {
            get { return is_lifted && this.Type.IsNullable(); }
        }

        public MethodInfo Method
        {
            get { return method; }
        }

        public Expression Operand
        {
            get { return operand; }
        }

        internal override void Emit(EmitContext ec)
        {
            if (method != null)
            {
                EmitUserDefinedOperator(ec);
                return;
            }

            switch (this.NodeType)
            {
                case ExpressionType.ArrayLength:
                    EmitArrayLength(ec);
                    return;

                case ExpressionType.TypeAs:
                    EmitTypeAs(ec);
                    return;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    EmitConvert(ec);
                    return;

                case ExpressionType.Not:
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.UnaryPlus:
                    EmitArithmeticUnary(ec);
                    return;

                case ExpressionType.Quote:
                    EmitQuote(ec);
                    return;

                default:
                    throw new NotImplementedException(this.NodeType.ToString());
            }
        }

        private void EmitArithmeticUnary(EmitContext ec)
        {
            if (!IsLifted)
            {
                operand.Emit(ec);
                EmitUnaryOperator(ec);
            }
            else
                EmitLiftedUnary(ec);
        }

        private void EmitArrayLength(EmitContext ec)
        {
            operand.Emit(ec);
            ec.ig.Emit(OpCodes.Ldlen);
        }

        private void EmitBox(EmitContext ec)
        {
            ec.ig.Emit(OpCodes.Box, operand.Type);
        }

        private void EmitCast(EmitContext ec)
        {
            operand.Emit(ec);

            if (IsBoxing())
            {
                EmitBox(ec);
            }
            else if (IsUnBoxing())
            {
                EmitUnbox(ec);
            }
            else
                ec.ig.Emit(OpCodes.Castclass, Type);
        }

        private void EmitConvert(EmitContext ec)
        {
            var from = operand.Type;
            var target = Type;

            if (from == target)
                operand.Emit(ec);
            else if (from.IsNullable() && !target.IsNullable())
                EmitConvertFromNullable(ec);
            else if (!from.IsNullable() && target.IsNullable())
                EmitConvertToNullable(ec);
            else if (from.IsNullable() && target.IsNullable())
                EmitConvertFromNullableToNullable(ec);
            else if (IsReferenceConversion(from, target))
                EmitCast(ec);
            else if (IsPrimitiveConversion(from, target))
                EmitPrimitiveConversion(ec);
            else
                throw new NotImplementedException();
        }

        private void EmitConvertFromNullable(EmitContext ec)
        {
            if (IsBoxing())
            {
                ec.Emit(operand);
                EmitBox(ec);
                return;
            }

            ec.EmitCall(operand, operand.Type.GetMethod("get_Value"));

            if (operand.Type.GetNotNullableType() != Type)
            {
                EmitPrimitiveConversion(ec,
                    operand.Type.GetNotNullableType(),
                    Type);
            }
        }

        private void EmitConvertFromNullableToNullable(EmitContext ec)
        {
            EmitLiftedUnary(ec);
        }

        private void EmitConvertToNullable(EmitContext ec)
        {
            ec.Emit(operand);

            if (IsUnBoxing())
            {
                EmitUnbox(ec);
                return;
            }

            if (operand.Type != Type.GetNotNullableType())
            {
                EmitPrimitiveConversion(ec,
                    operand.Type,
                    Type.GetNotNullableType());
            }

            ec.EmitNullableNew(Type);
        }

        private void EmitLiftedUnary(EmitContext ec)
        {
            var ig = ec.ig;

            var from = ec.EmitStored(operand);
            var to = ig.DeclareLocal(Type);

            var has_value = ig.DefineLabel();
            var done = ig.DefineLabel();

            ec.EmitNullableHasValue(from);
            ig.Emit(OpCodes.Brtrue, has_value);

            // if not has value
            ec.EmitNullableInitialize(to);

            ig.Emit(OpCodes.Br, done);

            ig.MarkLabel(has_value);
            // if has value
            ec.EmitNullableGetValueOrDefault(from);

            EmitUnaryOperator(ec);

            ec.EmitNullableNew(Type);

            ig.MarkLabel(done);
        }

        private void EmitPrimitiveConversion(EmitContext ec, bool is_unsigned,
            OpCode signed, OpCode unsigned, OpCode signed_checked, OpCode unsigned_checked)
        {
            if (this.NodeType != ExpressionType.ConvertChecked)
                ec.ig.Emit(is_unsigned ? unsigned : signed);
            else
                ec.ig.Emit(is_unsigned ? unsigned_checked : signed_checked);
        }

        private void EmitPrimitiveConversion(EmitContext ec)
        {
            operand.Emit(ec);

            EmitPrimitiveConversion(ec, operand.Type, Type);
        }

        private void EmitPrimitiveConversion(EmitContext ec, Type from, Type to)
        {
            var is_unsigned = IsUnsigned(from);

            switch (Type.GetTypeCode(to))
            {
                case TypeCode.SByte:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I1,
                        OpCodes.Conv_U1,
                        OpCodes.Conv_Ovf_I1,
                        OpCodes.Conv_Ovf_I1_Un);
                    return;

                case TypeCode.Byte:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I1,
                        OpCodes.Conv_U1,
                        OpCodes.Conv_Ovf_U1,
                        OpCodes.Conv_Ovf_U1_Un);
                    return;

                case TypeCode.Int16:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I2,
                        OpCodes.Conv_U2,
                        OpCodes.Conv_Ovf_I2,
                        OpCodes.Conv_Ovf_I2_Un);
                    return;

                case TypeCode.UInt16:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I2,
                        OpCodes.Conv_U2,
                        OpCodes.Conv_Ovf_U2,
                        OpCodes.Conv_Ovf_U2_Un);
                    return;

                case TypeCode.Int32:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I4,
                        OpCodes.Conv_U4,
                        OpCodes.Conv_Ovf_I4,
                        OpCodes.Conv_Ovf_I4_Un);
                    return;

                case TypeCode.UInt32:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I4,
                        OpCodes.Conv_U4,
                        OpCodes.Conv_Ovf_U4,
                        OpCodes.Conv_Ovf_U4_Un);
                    return;

                case TypeCode.Int64:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I8,
                        OpCodes.Conv_U8,
                        OpCodes.Conv_Ovf_I8,
                        OpCodes.Conv_Ovf_I8_Un);
                    return;

                case TypeCode.UInt64:
                    EmitPrimitiveConversion(ec,
                        is_unsigned,
                        OpCodes.Conv_I8,
                        OpCodes.Conv_U8,
                        OpCodes.Conv_Ovf_U8,
                        OpCodes.Conv_Ovf_U8_Un);
                    return;

                case TypeCode.Single:
                    if (is_unsigned)
                        ec.ig.Emit(OpCodes.Conv_R_Un);
                    ec.ig.Emit(OpCodes.Conv_R4);
                    return;

                case TypeCode.Double:
                    if (is_unsigned)
                        ec.ig.Emit(OpCodes.Conv_R_Un);
                    ec.ig.Emit(OpCodes.Conv_R8);
                    return;

                default:
                    throw new NotImplementedException(this.Type.ToString());
            }
        }

        private void EmitQuote(EmitContext ec)
        {
            ec.EmitScope();

            ec.EmitReadGlobal(operand, typeof(Expression));

            if (ec.HasHoistedLocals)
                ec.EmitLoadHoistedLocalsStore();
            else
                ec.ig.Emit(OpCodes.Ldnull);

            ec.EmitIsolateExpression();
        }

        private void EmitTypeAs(EmitContext ec)
        {
            var type = this.Type;

            ec.EmitIsInst(operand, type);

            if (type.IsNullable())
                ec.ig.Emit(OpCodes.Unbox_Any, type);
        }

        private void EmitUnaryOperator(EmitContext ec)
        {
            var ig = ec.ig;

            switch (NodeType)
            {
                case ExpressionType.Not:
                    if (operand.Type.GetNotNullableType() == typeof(bool))
                    {
                        ig.Emit(OpCodes.Ldc_I4_0);
                        ig.Emit(OpCodes.Ceq);
                    }
                    else
                        ig.Emit(OpCodes.Not);
                    break;

                case ExpressionType.Negate:
                    ig.Emit(OpCodes.Neg);
                    break;

                case ExpressionType.NegateChecked:
                    ig.Emit(OpCodes.Ldc_I4_M1);
                    ig.Emit(IsUnsigned(operand.Type) ? OpCodes.Mul_Ovf_Un : OpCodes.Mul_Ovf);
                    break;

                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    // Called when converting from nullable from nullable
                    EmitPrimitiveConversion(ec,
                        operand.Type.GetNotNullableType(),
                        Type.GetNotNullableType());
                    break;
            }
        }

        private void EmitUnbox(EmitContext ec)
        {
            ec.ig.Emit(OpCodes.Unbox_Any, Type);
        }

        private void EmitUserDefinedLiftedOperator(EmitContext ec)
        {
            var local = ec.EmitStored(operand);
            ec.EmitNullableGetValue(local);
            ec.EmitCall(method);
        }

        private void EmitUserDefinedLiftedToNullOperator(EmitContext ec)
        {
            var ig = ec.ig;
            var local = ec.EmitStored(operand);

            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();

            ec.EmitNullableHasValue(local);
            ig.Emit(OpCodes.Brfalse, ret);

            ec.EmitNullableGetValueOrDefault(local);
            ec.EmitCall(method);
            ec.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);

            ig.MarkLabel(ret);

            var temp = ig.DeclareLocal(Type);
            ec.EmitNullableInitialize(temp);

            ig.MarkLabel(done);
        }

        private void EmitUserDefinedOperator(EmitContext ec)
        {
            if (!IsLifted)
            {
                ec.Emit(operand);
                ec.EmitCall(method);
            }
            else if (IsLiftedToNull)
            {
                EmitUserDefinedLiftedToNullOperator(ec);
            }
            else
                EmitUserDefinedLiftedOperator(ec);
        }

        private bool IsBoxing()
        {
            return operand.Type.IsValueType && !Type.IsValueType;
        }

        private bool IsUnBoxing()
        {
            return !operand.Type.IsValueType && Type.IsValueType;
        }
    }
}

#endif