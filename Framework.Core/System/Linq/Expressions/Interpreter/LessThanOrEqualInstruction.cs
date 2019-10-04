#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class LessThanOrEqualInstruction : Instruction
    {
        private static Instruction? _liftedToNullSByte, _liftedToNullInt16, _liftedToNullChar, _liftedToNullInt32, _liftedToNullInt64, _liftedToNullByte, _liftedToNullUInt16, _liftedToNullUInt32, _liftedToNullUInt64, _liftedToNullSingle, _liftedToNullDouble;
        private static Instruction? _sByte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;
        private readonly object? _nullValue;

        private LessThanOrEqualInstruction(object? nullValue)
        {
            _nullValue = nullValue;
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "LessThanOrEqual";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type, bool liftedToNull = false)
        {
            Debug.Assert(!type.IsEnum);
            if (liftedToNull)
            {
                switch (Type.GetTypeCode(type.GetNonNullable()))
                {
                    case TypeCode.SByte: return _liftedToNullSByte ??= new LessThanOrEqualSByte(null);
                    case TypeCode.Int16: return _liftedToNullInt16 ??= new LessThanOrEqualInt16(null);
                    case TypeCode.Char: return _liftedToNullChar ??= new LessThanOrEqualChar(null);
                    case TypeCode.Int32: return _liftedToNullInt32 ??= new LessThanOrEqualInt32(null);
                    case TypeCode.Int64: return _liftedToNullInt64 ??= new LessThanOrEqualInt64(null);
                    case TypeCode.Byte: return _liftedToNullByte ??= new LessThanOrEqualByte(null);
                    case TypeCode.UInt16: return _liftedToNullUInt16 ??= new LessThanOrEqualUInt16(null);
                    case TypeCode.UInt32: return _liftedToNullUInt32 ??= new LessThanOrEqualUInt32(null);
                    case TypeCode.UInt64: return _liftedToNullUInt64 ??= new LessThanOrEqualUInt64(null);
                    case TypeCode.Single: return _liftedToNullSingle ??= new LessThanOrEqualSingle(null);
                    case TypeCode.Double: return _liftedToNullDouble ??= new LessThanOrEqualDouble(null);
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.SByte: return _sByte ??= new LessThanOrEqualSByte(Utils.BoxedFalse);
                case TypeCode.Int16: return _int16 ??= new LessThanOrEqualInt16(Utils.BoxedFalse);
                case TypeCode.Char: return _char ??= new LessThanOrEqualChar(Utils.BoxedFalse);
                case TypeCode.Int32: return _int32 ??= new LessThanOrEqualInt32(Utils.BoxedFalse);
                case TypeCode.Int64: return _int64 ??= new LessThanOrEqualInt64(Utils.BoxedFalse);
                case TypeCode.Byte: return _byte ??= new LessThanOrEqualByte(Utils.BoxedFalse);
                case TypeCode.UInt16: return _uInt16 ??= new LessThanOrEqualUInt16(Utils.BoxedFalse);
                case TypeCode.UInt32: return _uInt32 ??= new LessThanOrEqualUInt32(Utils.BoxedFalse);
                case TypeCode.UInt64: return _uInt64 ??= new LessThanOrEqualUInt64(Utils.BoxedFalse);
                case TypeCode.Single: return _single ??= new LessThanOrEqualSingle(Utils.BoxedFalse);
                case TypeCode.Double: return _double ??= new LessThanOrEqualDouble(Utils.BoxedFalse);
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class LessThanOrEqualByte : LessThanOrEqualInstruction
        {
            public LessThanOrEqualByte(object? nullValue)
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
                    frame.Push((byte)left <= (byte)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualChar : LessThanOrEqualInstruction
        {
            public LessThanOrEqualChar(object? nullValue)
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
                    frame.Push((char)left <= (char)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualDouble : LessThanOrEqualInstruction
        {
            public LessThanOrEqualDouble(object? nullValue)
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
                    frame.Push((double)left <= (double)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualInt16 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt16(object? nullValue)
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
                    frame.Push((short)left <= (short)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualInt32 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt32(object? nullValue)
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
                    frame.Push((int)left <= (int)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualInt64 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt64(object? nullValue)
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
                    frame.Push((long)left <= (long)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualSByte : LessThanOrEqualInstruction
        {
            public LessThanOrEqualSByte(object? nullValue)
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
                    frame.Push((sbyte)left <= (sbyte)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualSingle : LessThanOrEqualInstruction
        {
            public LessThanOrEqualSingle(object? nullValue)
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
                    frame.Push((float)left <= (float)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualUInt16 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt16(object? nullValue)
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
                    frame.Push((ushort)left <= (ushort)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualUInt32 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt32(object? nullValue)
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
                    frame.Push((uint)left <= (uint)right);
                }

                return 1;
            }
        }

        private sealed class LessThanOrEqualUInt64 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt64(object? nullValue)
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
                    frame.Push((ulong)left <= (ulong)right);
                }

                return 1;
            }
        }
    }
}

#endif