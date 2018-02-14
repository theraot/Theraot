#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NumericConvertInstruction : Instruction
    {
        internal readonly TypeCode From, To;
        private readonly bool _isLiftedToNull;

        protected NumericConvertInstruction(TypeCode from, TypeCode to, bool isLiftedToNull)
        {
            From = from;
            To = to;
            _isLiftedToNull = isLiftedToNull;
        }

        public sealed override int Run(InterpretedFrame frame)
        {
            var obj = frame.Pop();
            object converted;
            if (obj == null)
            {
                if (_isLiftedToNull)
                {
                    converted = null;
                }
                else
                {
                    // We cannot have null in a non-lifted numeric context. Throw the exception
                    // about not Nullable object requiring a value.
                    converted = (int)(int?)obj; // TODO: Test coverage?
                    GC.KeepAlive(converted);
                    throw Assert.Unreachable;
                }
            }
            else
            {
                converted = Convert(obj);
            }

            frame.Push(converted);
            return +1;
        }

        protected abstract object Convert(object obj);

        public override string InstructionName
        {
            get { return "NumericConvert"; }
        }

        public override int ConsumedStack
        {
            get { return 1; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string ToString()
        {
            return InstructionName + "(" + From + "->" + To + ")";
        }

        internal sealed class Unchecked : NumericConvertInstruction
        {
            public override string InstructionName
            {
                get { return "UncheckedConvert"; }
            }

            public Unchecked(TypeCode from, TypeCode to, bool isLiftedToNull)
                : base(from, to, isLiftedToNull)
            {
            }

            protected override object Convert(object obj)
            {
                switch (From)
                {
                    case TypeCode.Byte:
                        return ConvertInt32((byte)obj);

                    case TypeCode.SByte:
                        return ConvertInt32((sbyte)obj);

                    case TypeCode.Int16:
                        return ConvertInt32((short)obj);

                    case TypeCode.Char:
                        return ConvertInt32((char)obj);

                    case TypeCode.Int32:
                        return ConvertInt32((int)obj);

                    case TypeCode.Int64:
                        return ConvertInt64((long)obj);

                    case TypeCode.UInt16:
                        return ConvertInt32((ushort)obj);

                    case TypeCode.UInt32:
                        return ConvertInt64((uint)obj);

                    case TypeCode.UInt64:
                        return ConvertUInt64((ulong)obj);

                    case TypeCode.Single:
                        return ConvertDouble((float)obj);

                    case TypeCode.Double:
                        return ConvertDouble((double)obj);

                    default:
                        throw Assert.Unreachable;
                }
            }

            private object ConvertInt32(int obj)
            {
                unchecked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertInt64(long obj)
            {
                unchecked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertUInt64(ulong obj)
            {
                unchecked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertDouble(double obj)
            {
                unchecked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }
        }

        internal sealed class Checked : NumericConvertInstruction
        {
            public override string InstructionName
            {
                get { return "CheckedConvert"; }
            }

            public Checked(TypeCode from, TypeCode to, bool isLiftedToNull)
                : base(from, to, isLiftedToNull)
            {
            }

            protected override object Convert(object obj)
            {
                switch (From)
                {
                    case TypeCode.Byte:
                        return ConvertInt32((byte)obj);

                    case TypeCode.SByte:
                        return ConvertInt32((sbyte)obj);

                    case TypeCode.Int16:
                        return ConvertInt32((short)obj);

                    case TypeCode.Char:
                        return ConvertInt32((char)obj);

                    case TypeCode.Int32:
                        return ConvertInt32((int)obj);

                    case TypeCode.Int64:
                        return ConvertInt64((long)obj);

                    case TypeCode.UInt16:
                        return ConvertInt32((ushort)obj);

                    case TypeCode.UInt32:
                        return ConvertInt64((uint)obj);

                    case TypeCode.UInt64:
                        return ConvertUInt64((ulong)obj);

                    case TypeCode.Single:
                        return ConvertDouble((float)obj);

                    case TypeCode.Double:
                        return ConvertDouble((double)obj);

                    default:
                        throw Assert.Unreachable;
                }
            }

            private object ConvertInt32(int obj)
            {
                checked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertInt64(long obj)
            {
                checked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertUInt64(ulong obj)
            {
                checked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return (double)obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }

            private object ConvertDouble(double obj)
            {
                checked
                {
                    switch (To)
                    {
                        case TypeCode.Byte:
                            return (byte)obj;

                        case TypeCode.SByte:
                            return (sbyte)obj;

                        case TypeCode.Int16:
                            return (short)obj;

                        case TypeCode.Char:
                            return (char)obj;

                        case TypeCode.Int32:
                            return (int)obj;

                        case TypeCode.Int64:
                            return (long)obj;

                        case TypeCode.UInt16:
                            return (ushort)obj;

                        case TypeCode.UInt32:
                            return (uint)obj;

                        case TypeCode.UInt64:
                            return (ulong)obj;

                        case TypeCode.Single:
                            return (float)obj;

                        case TypeCode.Double:
                            return obj;

                        default:
                            throw Assert.Unreachable;
                    }
                }
            }
        }
    }
}

#endif