#if NET20 || NET30

using System.Reflection;
using System.Reflection.Emit;
using Theraot.Core;

namespace System.Linq.Expressions
{
    public sealed class ConstantExpression : Expression
    {
        private object value;

        internal ConstantExpression(object value, Type type)
            : base(ExpressionType.Constant, type)
        {
            this.value = value;
        }

        public object Value
        {
            get { return value; }
        }
        internal static bool IsNull(Expression e)
        {
            var c = e as ConstantExpression;
            return c != null && c.value == null;
        }

        internal override void Emit(EmitContext ec)
        {
            if (Type.IsNullable())
            {
                EmitNullableConstant(ec, Type, value);
                return;
            }

            EmitConstant(ec, Type, value);
        }

        private void EmitConstant(EmitContext ec, Type type, object value)
        {
            var ig = ec.ig;

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
                        ig.Emit(OpCodes.Ldc_I4_1);
                    else
                        ec.ig.Emit(OpCodes.Ldc_I4_0);
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
                        Decimal v = (decimal)value;
                        int[] words = Decimal.GetBits(v);
                        int power = (words[3] >> 16) & 0xff;
                        Type ti = typeof(int);

                        if (power == 0 && v <= int.MaxValue && v >= int.MinValue)
                        {
                            ig.Emit(OpCodes.Ldc_I4, (int)v);

                            ig.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[1] { ti }));
                            return;
                        }
                        ig.Emit(OpCodes.Ldc_I4, words[0]);
                        ig.Emit(OpCodes.Ldc_I4, words[1]);
                        ig.Emit(OpCodes.Ldc_I4, words[2]);
                        // sign
                        ig.Emit(OpCodes.Ldc_I4, words[3] >> 31);

                        // power
                        ig.Emit(OpCodes.Ldc_I4, power);

                        ig.Emit(OpCodes.Newobj, typeof(Decimal).GetConstructor(new Type[5] { ti, ti, ti, typeof(bool), typeof(byte) }));
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
                    EmitIfNotNull(ec, c => c.ig.Emit(OpCodes.Ldstr, (string)value));
                    return;

                case TypeCode.Object:
                    EmitIfNotNull(ec, c => c.EmitReadGlobal(value));
                    return;
            }

            throw new NotImplementedException(String.Format("No support for constants of type {0} yet", Type));
        }

        private void EmitIfNotNull(EmitContext ec, Action<EmitContext> emit)
        {
            if (value == null)
            {
                ec.ig.Emit(OpCodes.Ldnull);
                return;
            }

            emit(ec);
        }

        private void EmitNullableConstant(EmitContext ec, Type type, object value)
        {
            if (value == null)
            {
                var local = ec.ig.DeclareLocal(type);
                ec.EmitNullableInitialize(local);
            }
            else
            {
                EmitConstant(ec, type.GetFirstGenericArgument(), value);
                ec.EmitNullableNew(type);
            }
        }
    }
}

#endif