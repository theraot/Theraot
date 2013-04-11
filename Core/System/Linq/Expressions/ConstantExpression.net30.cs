#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public sealed class ConstantExpression : Expression
    {
        private object _value;

        internal ConstantExpression(object value, Type type)
            : base(ExpressionType.Constant, type)
        {
            this._value = value;
        }

        public object Value
        {
            get
            {
                return _value;
            }
        }

        internal override void Emit(EmitContext emitContext)
        {
            if (Type.IsNullable())
            {
                EmitNullableConstant(emitContext, Type, _value);
                return;
            }
            EmitConstant(emitContext, Type, _value);
        }

        private void EmitConstant(EmitContext emitContext, Type type, object value)
        {
            var ig = emitContext.ILGenerator;
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    ig.Emit(OpCodes.Ldc_I4, (int)((byte)value));
                    return;

                case TypeCode.SByte:
                    ig.Emit(OpCodes.Ldc_I4, (int)((sbyte)value));
                    return;

                case TypeCode.Int16:
                    ig.Emit(OpCodes.Ldc_I4, (int)((short)value));
                    return;

                case TypeCode.UInt16:
                    ig.Emit(OpCodes.Ldc_I4, (int)((ushort)value));
                    return;

                case TypeCode.Int32:
                    ig.Emit(OpCodes.Ldc_I4, (int)value);
                    return;

                case TypeCode.UInt32:
                    ig.Emit(OpCodes.Ldc_I4, unchecked((int)((uint)Value)));
                    return;

                case TypeCode.Int64:
                    ig.Emit(OpCodes.Ldc_I8, (long)value);
                    return;

                case TypeCode.UInt64:
                    ig.Emit(OpCodes.Ldc_I8, unchecked((long)((ulong)value)));
                    return;

                case TypeCode.Boolean:
                    if ((bool)Value)
                    {
                        ig.Emit(OpCodes.Ldc_I4_1);
                    }
                    else
                    {
                        emitContext.ILGenerator.Emit(OpCodes.Ldc_I4_0);
                    }
                    return;

                case TypeCode.Char:
                    ig.Emit(OpCodes.Ldc_I4, (int)((char)value));
                    return;

                case TypeCode.Single:
                    ig.Emit(OpCodes.Ldc_R4, (float)value);
                    return;

                case TypeCode.Double:
                    ig.Emit(OpCodes.Ldc_R8, (double)value);
                    return;

                case TypeCode.Decimal:
                    {
                        decimal v = (decimal)value;
                        int[] words = decimal.GetBits(v);
                        int power = (words[3] >> 16) & 0xff;
                        Type ti = typeof(int);
                        if (power == 0 && v <= int.MaxValue && v >= int.MinValue)
                        {
                            ig.Emit(OpCodes.Ldc_I4, (int)v);
                            ig.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[1] { ti }));
                            return;
                        }
                        ig.Emit(OpCodes.Ldc_I4, words[0]);
                        ig.Emit(OpCodes.Ldc_I4, words[1]);
                        ig.Emit(OpCodes.Ldc_I4, words[2]);

                        // sign
                        ig.Emit(OpCodes.Ldc_I4, words[3] >> 31);

                        // power
                        ig.Emit(OpCodes.Ldc_I4, power);
                        ig.Emit(OpCodes.Newobj, typeof(decimal).GetConstructor(new Type[5] { ti, ti, ti, typeof(bool), typeof(byte) }));
                        return;
                    }
                case TypeCode.DateTime:
                    {
                        var date = (DateTime)value;
                        var local = ig.DeclareLocal(typeof(DateTime));
                        ig.Emit(OpCodes.Ldloca, local);
                        ig.Emit(OpCodes.Ldc_I8, date.Ticks);
                        ig.Emit(OpCodes.Ldc_I4, (int)date.Kind);
                        ig.Emit(OpCodes.Call, typeof(DateTime).GetConstructor(new[] { typeof(long), typeof(DateTimeKind) }));
                        ig.Emit(OpCodes.Ldloc, local);
                        return;
                    }
                case TypeCode.DBNull:
                    ig.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value", BindingFlags.Public | BindingFlags.Static));
                    return;

                case TypeCode.String:
                    EmitIfNotNull(emitContext, c => c.ILGenerator.Emit(OpCodes.Ldstr, (string)value));
                    return;

                case TypeCode.Object:
                    EmitIfNotNull(emitContext, c => c.EmitReadGlobal(value));
                    return;

                default:
                    throw new NotImplementedException(string.Format("No support for constants of type {0} yet", Type));
            }
        }

        private void EmitIfNotNull (EmitContext emitContext, Action<EmitContext> emit)
        {
            if (_value == null)
            {
                emitContext.ILGenerator.Emit (OpCodes.Ldnull);
                return;
            }
            emit (emitContext);
        }

        private void EmitNullableConstant(EmitContext emitContext, Type type, object value)
        {
            if (value == null)
            {
                var local = emitContext.ILGenerator.DeclareLocal(type);
                emitContext.EmitNullableInitialize(local);
            }
            else
            {
                EmitConstant(emitContext, type.GetFirstGenericArgument(), value);
                emitContext.EmitNullableNew(type);
            }
        }
    }
}

#endif