#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class GreaterThanInstruction : Instruction
    {
        private static Instruction _liftedToNullSByte, _liftedToNullInt16, _liftedToNullChar, _liftedToNullInt32, _liftedToNullInt64, _liftedToNullByte, _liftedToNullUInt16, _liftedToNullUInt32, _liftedToNullUInt64, _liftedToNullSingle, _liftedToNullDouble;
        private static Instruction _sByte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;
        private readonly object _nullValue;

        private GreaterThanInstruction(object nullValue)
        {
            _nullValue = nullValue;
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "GreaterThan";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type, bool liftedToNull = false)
        {
            Debug.Assert(!type.IsEnum);
            if (liftedToNull)
            {
                switch (type.GetNonNullable().GetTypeCode())
                {
                    case TypeCode.SByte: return _liftedToNullSByte ?? (_liftedToNullSByte = new GreaterThanSByte(null));
                    case TypeCode.Int16: return _liftedToNullInt16 ?? (_liftedToNullInt16 = new GreaterThanInt16(null));
                    case TypeCode.Char: return _liftedToNullChar ?? (_liftedToNullChar = new GreaterThanChar(null));
                    case TypeCode.Int32: return _liftedToNullInt32 ?? (_liftedToNullInt32 = new GreaterThanInt32(null));
                    case TypeCode.Int64: return _liftedToNullInt64 ?? (_liftedToNullInt64 = new GreaterThanInt64(null));
                    case TypeCode.Byte: return _liftedToNullByte ?? (_liftedToNullByte = new GreaterThanByte(null));
                    case TypeCode.UInt16: return _liftedToNullUInt16 ?? (_liftedToNullUInt16 = new GreaterThanUInt16(null));
                    case TypeCode.UInt32: return _liftedToNullUInt32 ?? (_liftedToNullUInt32 = new GreaterThanUInt32(null));
                    case TypeCode.UInt64: return _liftedToNullUInt64 ?? (_liftedToNullUInt64 = new GreaterThanUInt64(null));
                    case TypeCode.Single: return _liftedToNullSingle ?? (_liftedToNullSingle = new GreaterThanSingle(null));
                    case TypeCode.Double: return _liftedToNullDouble ?? (_liftedToNullDouble = new GreaterThanDouble(null));
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.SByte: return _sByte ?? (_sByte = new GreaterThanSByte(Utils.BoxedFalse));
                case TypeCode.Int16: return _int16 ?? (_int16 = new GreaterThanInt16(Utils.BoxedFalse));
                case TypeCode.Char: return _char ?? (_char = new GreaterThanChar(Utils.BoxedFalse));
                case TypeCode.Int32: return _int32 ?? (_int32 = new GreaterThanInt32(Utils.BoxedFalse));
                case TypeCode.Int64: return _int64 ?? (_int64 = new GreaterThanInt64(Utils.BoxedFalse));
                case TypeCode.Byte: return _byte ?? (_byte = new GreaterThanByte(Utils.BoxedFalse));
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new GreaterThanUInt16(Utils.BoxedFalse));
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new GreaterThanUInt32(Utils.BoxedFalse));
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new GreaterThanUInt64(Utils.BoxedFalse));
                case TypeCode.Single: return _single ?? (_single = new GreaterThanSingle(Utils.BoxedFalse));
                case TypeCode.Double: return _double ?? (_double = new GreaterThanDouble(Utils.BoxedFalse));
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class GreaterThanByte : GreaterThanInstruction
        {
            public GreaterThanByte(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((byte)left > (byte)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanChar : GreaterThanInstruction
        {
            public GreaterThanChar(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((char)left > (char)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanDouble : GreaterThanInstruction
        {
            public GreaterThanDouble(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((double)left > (double)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanInt16 : GreaterThanInstruction
        {
            public GreaterThanInt16(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((short)left > (short)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanInt32 : GreaterThanInstruction
        {
            public GreaterThanInt32(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((int)left > (int)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanInt64 : GreaterThanInstruction
        {
            public GreaterThanInt64(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((long)left > (long)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanSByte : GreaterThanInstruction
        {
            public GreaterThanSByte(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((sbyte)left > (sbyte)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanSingle : GreaterThanInstruction
        {
            public GreaterThanSingle(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((float)left > (float)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanUInt16 : GreaterThanInstruction
        {
            public GreaterThanUInt16(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((ushort)left > (ushort)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanUInt32 : GreaterThanInstruction
        {
            public GreaterThanUInt32(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((uint)left > (uint)right);
                }

                return 1;
            }
        }

        private sealed class GreaterThanUInt64 : GreaterThanInstruction
        {
            public GreaterThanUInt64(object nullValue)
                : base(nullValue)
            {
                // Empty
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push((ulong)left > (ulong)right);
                }

                return 1;
            }
        }
    }
}

#endif