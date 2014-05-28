#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public sealed class BinaryExpression : Expression
    {
        private readonly LambdaExpression _conversion;
        private readonly bool _isLifted;
        private readonly Expression _left;
        private readonly bool _liftToNull;
        private readonly MethodInfo _method;
        private readonly Expression _right;

        internal BinaryExpression(ExpressionType nodeType, Type type, Expression left, Expression right)
            : base(nodeType, type)
        {
            _left = left;
            _right = right;
        }

        internal BinaryExpression(ExpressionType nodeType, Type type, Expression left, Expression right, MethodInfo method)
            : base(nodeType, type)
        {
            _left = left;
            _right = right;
            _method = method;
        }

        internal BinaryExpression(ExpressionType nodeType, Type type, Expression left, Expression right, bool liftToNull, bool isLifted, MethodInfo method, LambdaExpression conversion)
            : base(nodeType, type)
        {
            _left = left;
            _right = right;
            _method = method;
            _conversion = conversion;
            _liftToNull = liftToNull;
            _isLifted = isLifted;
        }

        public LambdaExpression Conversion
        {
            get
            {
                return _conversion;
            }
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
                return _liftToNull;
            }
        }

        public Expression Left
        {
            get
            {
                return _left;
            }
        }

        public MethodInfo Method
        {
            get
            {
                return _method;
            }
        }

        public Expression Right
        {
            get
            {
                return _right;
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
                case ExpressionType.ArrayIndex:
                    EmitArrayAccess(emitContext);
                    return;

                case ExpressionType.Coalesce:
                    if (_conversion != null)
                    {
                        EmitConvertedCoalesce(emitContext);
                    }
                    else
                    {
                        EmitCoalesce(emitContext);
                    }
                    return;

                case ExpressionType.Power:
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Divide:
                case ExpressionType.ExclusiveOr:
                case ExpressionType.LeftShift:
                case ExpressionType.Modulo:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.RightShift:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    EmitArithmeticBinary(emitContext);
                    return;

                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    EmitRelationalBinary(emitContext);
                    return;

                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    EmitLogicalBinary(emitContext);
                    return;

                default:
                    throw new NotSupportedException(NodeType.ToString());
            }
        }

        private static bool IsInt32OrInt64(Type type)
        {
            return type == typeof(int) || type == typeof(long);
        }

        private static bool IsSingleOrDouble(Type type)
        {
            return type == typeof(float) || type == typeof(double);
        }

        private void EmitArithmeticBinary(EmitContext emitContext)
        {
            if (!IsLifted)
            {
                EmitNonLiftedBinary(emitContext);
            }
            else
            {
                EmitLiftedArithmeticBinary(emitContext);
            }
        }

        private void EmitArrayAccess(EmitContext emitContext)
        {
            _left.Emit(emitContext);
            _right.Emit(emitContext);
            emitContext.ILGenerator.Emit(OpCodes.Ldelem, Type);
        }

        private void EmitBinaryOperator(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            bool is_unsigned = IsUnsigned(_left.Type);
            switch (NodeType)
            {
                case ExpressionType.Add:
                    ig.Emit(OpCodes.Add);
                    break;

                case ExpressionType.AddChecked:
                    if (IsInt32OrInt64(_left.Type))
                    {
                        ig.Emit(OpCodes.Add_Ovf);
                    }
                    else
                    {
                        ig.Emit(is_unsigned ? OpCodes.Add_Ovf_Un : OpCodes.Add);
                    }
                    break;

                case ExpressionType.Subtract:
                    ig.Emit(OpCodes.Sub);
                    break;

                case ExpressionType.SubtractChecked:
                    if (IsInt32OrInt64(_left.Type))
                    {
                        ig.Emit(OpCodes.Sub_Ovf);
                    }
                    else
                    {
                        ig.Emit(is_unsigned ? OpCodes.Sub_Ovf_Un : OpCodes.Sub);
                    }
                    break;

                case ExpressionType.Multiply:
                    ig.Emit(OpCodes.Mul);
                    break;

                case ExpressionType.MultiplyChecked:
                    if (IsInt32OrInt64(_left.Type))
                    {
                        ig.Emit(OpCodes.Mul_Ovf);
                    }
                    else
                    {
                        ig.Emit(is_unsigned ? OpCodes.Mul_Ovf_Un : OpCodes.Mul);
                    }
                    break;

                case ExpressionType.Divide:
                    ig.Emit(is_unsigned ? OpCodes.Div_Un : OpCodes.Div);
                    break;

                case ExpressionType.Modulo:
                    ig.Emit(is_unsigned ? OpCodes.Rem_Un : OpCodes.Rem);
                    break;

                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                    ig.Emit(OpCodes.Ldc_I4, _left.Type == typeof(int) ? 0x1f : 0x3f);
                    ig.Emit(OpCodes.And);
                    if (NodeType == ExpressionType.RightShift)
                    {
                        ig.Emit(is_unsigned ? OpCodes.Shr_Un : OpCodes.Shr);
                    }
                    else
                    {
                        ig.Emit(OpCodes.Shl);
                    }
                    break;

                case ExpressionType.And:
                    ig.Emit(OpCodes.And);
                    break;

                case ExpressionType.Or:
                    ig.Emit(OpCodes.Or);
                    break;

                case ExpressionType.ExclusiveOr:
                    ig.Emit(OpCodes.Xor);
                    break;

                case ExpressionType.GreaterThan:
                    ig.Emit(is_unsigned ? OpCodes.Cgt_Un : OpCodes.Cgt);
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    if (is_unsigned || IsSingleOrDouble(_left.Type))
                    {
                        ig.Emit(OpCodes.Clt_Un);
                    }
                    else
                    {
                        ig.Emit(OpCodes.Clt);
                    }
                    ig.Emit(OpCodes.Ldc_I4_0);
                    ig.Emit(OpCodes.Ceq);
                    break;

                case ExpressionType.LessThan:
                    ig.Emit(is_unsigned ? OpCodes.Clt_Un : OpCodes.Clt);
                    break;

                case ExpressionType.LessThanOrEqual:
                    if (is_unsigned || IsSingleOrDouble(_left.Type))
                    {
                        ig.Emit(OpCodes.Cgt_Un);
                    }
                    else
                    {
                        ig.Emit(OpCodes.Cgt);
                    }
                    ig.Emit(OpCodes.Ldc_I4_0);
                    ig.Emit(OpCodes.Ceq);
                    break;

                case ExpressionType.Equal:
                    ig.Emit(OpCodes.Ceq);
                    break;

                case ExpressionType.NotEqual:
                    ig.Emit(OpCodes.Ceq);
                    ig.Emit(OpCodes.Ldc_I4_0);
                    ig.Emit(OpCodes.Ceq);
                    break;

                case ExpressionType.Power:
                    ig.Emit(OpCodes.Call, typeof(Math).GetMethod("Pow"));
                    break;

                default:
                    throw new InvalidOperationException(
                        string.Format("Internal error: BinaryExpression contains non-Binary nodetype {0}", NodeType));
            }
        }

        private void EmitCoalesce(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var done = ig.DefineLabel();
            var load_right = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            var left_is_nullable = left.LocalType.IsNullable();
            if (left_is_nullable)
            {
                emitContext.EmitNullableHasValue(left);
            }
            else
            {
                emitContext.EmitLoad(left);
            }
            ig.Emit(OpCodes.Brfalse, load_right);
            if (left_is_nullable && !Type.IsNullable())
            {
                emitContext.EmitNullableGetValue(left);
            }
            else
            {
                emitContext.EmitLoad(left);
            }
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(load_right);
            emitContext.Emit(_right);
            ig.MarkLabel(done);
        }

        private void EmitConvertedCoalesce(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var done = ig.DefineLabel();
            var load_right = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            if (left.LocalType.IsNullable())
            {
                emitContext.EmitNullableHasValue(left);
            }
            else
            {
                emitContext.EmitLoad(left);
            }
            ig.Emit(OpCodes.Brfalse, load_right);
            emitContext.Emit(_conversion);
            emitContext.EmitLoad(left);
            ig.Emit(OpCodes.Callvirt, _conversion.Type.GetInvokeMethod());
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(load_right);
            emitContext.Emit(_right);
            ig.MarkLabel(done);
        }

        private void EmitLeftLiftedToNullBinary(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            emitContext.EmitNullableHasValue(left);
            ig.Emit(OpCodes.Brfalse, ret);
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.Emit(_right);
            EmitBinaryOperator(emitContext);
            emitContext.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret);
            var temp = ig.DeclareLocal(Type);
            emitContext.EmitNullableInitialize(temp);
            ig.MarkLabel(done);
        }

        private void EmitLiftedArithmeticBinary(EmitContext emitContext)
        {
            if (IsLeftLiftedBinary())
            {
                EmitLeftLiftedToNullBinary(emitContext);
            }
            else
            {
                EmitLiftedToNullBinary(emitContext);
            }
        }

        private void EmitLiftedLogical(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var and = NodeType == ExpressionType.And;
            var left = emitContext.EmitStored(_left);
            var right = emitContext.EmitStored(_right);
            var ret_from_left = ig.DefineLabel();
            var ret_from_right = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.EmitNullableGetValueOrDefault(left);
            ig.Emit(OpCodes.Brtrue, ret_from_left);
            emitContext.EmitNullableGetValueOrDefault(right);
            ig.Emit(OpCodes.Brtrue, ret_from_right);
            emitContext.EmitNullableHasValue(left);
            ig.Emit(OpCodes.Brfalse, ret_from_left);
            ig.MarkLabel(ret_from_right);
            emitContext.EmitLoad(and ? left : right);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_from_left);
            emitContext.EmitLoad(and ? right : left);
            ig.MarkLabel(done);
        }

        private void EmitLiftedLogicalShortCircuit(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var and = NodeType == ExpressionType.AndAlso;
            var left_is_null = ig.DefineLabel();
            var ret_from_left = ig.DefineLabel();
            var ret_null = ig.DefineLabel();
            var ret_new = ig.DefineLabel();
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            emitContext.EmitNullableHasValue(left);
            ig.Emit(OpCodes.Brfalse, left_is_null);
            emitContext.EmitNullableGetValueOrDefault(left);
            ig.Emit(OpCodes.Ldc_I4_0);
            ig.Emit(OpCodes.Ceq);
            ig.Emit(and ? OpCodes.Brtrue : OpCodes.Brfalse, ret_from_left);
            ig.MarkLabel(left_is_null);
            var right = emitContext.EmitStored(_right);
            emitContext.EmitNullableHasValue(right);
            ig.Emit(OpCodes.Brfalse_S, ret_null);
            emitContext.EmitNullableGetValueOrDefault(right);
            ig.Emit(OpCodes.Ldc_I4_0);
            ig.Emit(OpCodes.Ceq);
            ig.Emit(and ? OpCodes.Brtrue : OpCodes.Brfalse, ret_from_left);
            emitContext.EmitNullableHasValue(left);
            ig.Emit(OpCodes.Brfalse, ret_null);
            ig.Emit(and ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            ig.Emit(OpCodes.Br_S, ret_new);
            ig.MarkLabel(ret_from_left);
            ig.Emit(and ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
            ig.MarkLabel(ret_new);
            emitContext.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_null);
            var ret = ig.DeclareLocal(Type);
            emitContext.EmitNullableInitialize(ret);
            ig.MarkLabel(done);
        }

        private void EmitLiftedRelationalBinary(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var left = emitContext.EmitStored(_left);
            var right = emitContext.EmitStored(_right);
            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitNullableGetValueOrDefault(right);
            switch (NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    ig.Emit(OpCodes.Bne_Un, ret);
                    break;

                default:
                    EmitBinaryOperator(emitContext);
                    ig.Emit(OpCodes.Brfalse, ret);
                    break;
            }
            emitContext.EmitNullableHasValue(left);
            emitContext.EmitNullableHasValue(right);
            switch (NodeType)
            {
                case ExpressionType.Equal:
                    ig.Emit(OpCodes.Ceq);
                    break;

                case ExpressionType.NotEqual:
                    ig.Emit(OpCodes.Ceq);
                    ig.Emit(OpCodes.Ldc_I4_0);
                    ig.Emit(OpCodes.Ceq);
                    break;

                default:
                    ig.Emit(OpCodes.And);
                    break;
            }
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret);
            ig.Emit(NodeType == ExpressionType.NotEqual ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
            ig.MarkLabel(done);
        }

        private void EmitLiftedToNullBinary(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var left = emitContext.EmitStored(_left);
            var right = emitContext.EmitStored(_right);
            var result = ig.DeclareLocal(Type);
            var has_value = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.EmitNullableHasValue(left);
            emitContext.EmitNullableHasValue(right);
            ig.Emit(OpCodes.And);
            ig.Emit(OpCodes.Brtrue, has_value);
            emitContext.EmitNullableInitialize(result);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(has_value);
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitNullableGetValueOrDefault(right);
            EmitBinaryOperator(emitContext);
            emitContext.EmitNullableNew(result.LocalType);
            ig.MarkLabel(done);
        }

        private void EmitLiftedToNullUserDefinedOperator(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            var right = emitContext.EmitStored(_right);
            emitContext.EmitNullableHasValue(left);
            emitContext.EmitNullableHasValue(right);
            ig.Emit(OpCodes.And);
            ig.Emit(OpCodes.Brfalse, ret);
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitNullableGetValueOrDefault(right);
            emitContext.EmitCall(_method);
            emitContext.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret);
            var temp = ig.DeclareLocal(Type);
            emitContext.EmitNullableInitialize(temp);
            ig.MarkLabel(done);
        }

        private void EmitLiftedUserDefinedOperator(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var ret_true = ig.DefineLabel();
            var ret_false = ig.DefineLabel();
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            var right = emitContext.EmitStored(_right);
            emitContext.EmitNullableHasValue(left);
            emitContext.EmitNullableHasValue(right);
            switch (NodeType)
            {
                case ExpressionType.Equal:
                    ig.Emit(OpCodes.Bne_Un, ret_false);
                    emitContext.EmitNullableHasValue(left);
                    ig.Emit(OpCodes.Brfalse, ret_true);
                    break;

                case ExpressionType.NotEqual:
                    ig.Emit(OpCodes.Bne_Un, ret_true);
                    emitContext.EmitNullableHasValue(left);
                    ig.Emit(OpCodes.Brfalse, ret_false);
                    break;

                default:
                    ig.Emit(OpCodes.And);
                    ig.Emit(OpCodes.Brfalse, ret_false);
                    break;
            }
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitNullableGetValueOrDefault(right);
            emitContext.EmitCall(_method);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_true);
            ig.Emit(OpCodes.Ldc_I4_1);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_false);
            ig.Emit(OpCodes.Ldc_I4_0);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(done);
        }

        private void EmitLogical(EmitContext emitContext)
        {
            EmitNonLiftedBinary(emitContext);
        }

        private void EmitLogicalBinary(EmitContext emitContext)
        {
            switch (NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.Or:
                    if (!IsLifted)
                    {
                        EmitLogical(emitContext);
                    }
                    else if (Type == typeof(bool?))
                    {
                        EmitLiftedLogical(emitContext);
                    }
                    else
                    {
                        EmitLiftedArithmeticBinary(emitContext);
                    }
                    break;

                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    if (!IsLifted)
                    {
                        EmitLogicalShortCircuit(emitContext);
                    }
                    else
                    {
                        EmitLiftedLogicalShortCircuit(emitContext);
                    }
                    break;
            }
        }

        private void EmitLogicalShortCircuit(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var and = NodeType == ExpressionType.AndAlso;
            var ret = ig.DefineLabel();
            var done = ig.DefineLabel();
            emitContext.Emit(_left);
            ig.Emit(and ? OpCodes.Brfalse : OpCodes.Brtrue, ret);
            emitContext.Emit(_right);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret);
            ig.Emit(and ? OpCodes.Ldc_I4_0 : OpCodes.Ldc_I4_1);
            ig.MarkLabel(done);
        }

        private void EmitNonLiftedBinary(EmitContext emitContext)
        {
            emitContext.Emit(_left);
            emitContext.Emit(_right);
            EmitBinaryOperator(emitContext);
        }

        private void EmitRelationalBinary(EmitContext emitContext)
        {
            if (!IsLifted)
            {
                EmitNonLiftedBinary(emitContext);
            }
            else if (IsLiftedToNull)
            {
                EmitLiftedToNullBinary(emitContext);
            }
            else
            {
                EmitLiftedRelationalBinary(emitContext);
            }
        }

        private void EmitUserDefinedLiftedLogicalShortCircuit(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var and = NodeType == ExpressionType.AndAlso;
            var left_is_null = ig.DefineLabel();
            var ret_left = ig.DefineLabel();
            var ret_null = ig.DefineLabel();
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            emitContext.EmitNullableHasValue(left);
            ig.Emit(OpCodes.Brfalse, and ? ret_null : left_is_null);
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitCall(and ? GetFalseOperator() : GetTrueOperator());
            ig.Emit(OpCodes.Brtrue, ret_left);
            ig.MarkLabel(left_is_null);
            var right = emitContext.EmitStored(_right);
            emitContext.EmitNullableHasValue(right);
            ig.Emit(OpCodes.Brfalse, ret_null);
            emitContext.EmitNullableGetValueOrDefault(left);
            emitContext.EmitNullableGetValueOrDefault(right);
            emitContext.EmitCall(_method);
            emitContext.EmitNullableNew(Type);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_left);
            emitContext.EmitLoad(left);
            ig.Emit(OpCodes.Br, done);
            ig.MarkLabel(ret_null);
            var ret = ig.DeclareLocal(Type);
            emitContext.EmitNullableInitialize(ret);
            ig.MarkLabel(done);
        }

        private void EmitUserDefinedLogicalShortCircuit(EmitContext emitContext)
        {
            var ig = emitContext.ILGenerator;
            var and = NodeType == ExpressionType.AndAlso;
            var done = ig.DefineLabel();
            var left = emitContext.EmitStored(_left);
            emitContext.EmitLoad(left);
            ig.Emit(OpCodes.Dup);
            emitContext.EmitCall(and ? GetFalseOperator() : GetTrueOperator());
            ig.Emit(OpCodes.Brtrue, done);
            emitContext.Emit(_right);
            emitContext.EmitCall(_method);
            ig.MarkLabel(done);
        }

        private void EmitUserDefinedOperator(EmitContext emitContext)
        {
            if (!IsLifted)
            {
                switch (NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        EmitUserDefinedLogicalShortCircuit(emitContext);
                        break;

                    default:
                        _left.Emit(emitContext);
                        _right.Emit(emitContext);
                        emitContext.EmitCall(_method);
                        break;
                }
            }
            else if (IsLiftedToNull)
            {
                switch (NodeType)
                {
                    case ExpressionType.AndAlso:
                    case ExpressionType.OrElse:
                        EmitUserDefinedLiftedLogicalShortCircuit(emitContext);
                        break;

                    default:
                        EmitLiftedToNullUserDefinedOperator(emitContext);
                        break;
                }
            }
            else
            {
                EmitLiftedUserDefinedOperator(emitContext);
            }
        }

        private MethodInfo GetFalseOperator()
        {
            return GetFalseOperator(_left.Type.GetNotNullableType());
        }

        private MethodInfo GetTrueOperator()
        {
            return GetTrueOperator(_left.Type.GetNotNullableType());
        }

        private bool IsLeftLiftedBinary()
        {
            return _left.Type.IsNullable() && !_right.Type.IsNullable();
        }
    }
}

#endif