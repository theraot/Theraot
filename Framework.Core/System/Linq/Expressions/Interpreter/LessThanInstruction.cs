﻿#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class LessThanInstruction : Instruction
    {
        private static Instruction? _liftedToNullSByte, _liftedToNullInt16, _liftedToNullChar, _liftedToNullInt32, _liftedToNullInt64, _liftedToNullByte, _liftedToNullUInt16, _liftedToNullUInt32, _liftedToNullUInt64, _liftedToNullSingle, _liftedToNullDouble;
        private static Instruction? _sByte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;
        private readonly object? _nullValue;

        private LessThanInstruction(object? nullValue)
        {
            _nullValue = nullValue;
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "LessThan";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type, bool liftedToNull = false)
        {
            Debug.Assert(!type.IsEnum);
            if (liftedToNull)
            {
                switch (Type.GetTypeCode(type.GetNonNullable()))
                {
                    case TypeCode.SByte: return _liftedToNullSByte ??= new LessThanSByte(nullValue: null);
                    case TypeCode.Int16: return _liftedToNullInt16 ??= new LessThanInt16(nullValue: null);
                    case TypeCode.Char: return _liftedToNullChar ??= new LessThanChar(nullValue: null);
                    case TypeCode.Int32: return _liftedToNullInt32 ??= new LessThanInt32(nullValue: null);
                    case TypeCode.Int64: return _liftedToNullInt64 ??= new LessThanInt64(nullValue: null);
                    case TypeCode.Byte: return _liftedToNullByte ??= new LessThanByte(nullValue: null);
                    case TypeCode.UInt16: return _liftedToNullUInt16 ??= new LessThanUInt16(nullValue: null);
                    case TypeCode.UInt32: return _liftedToNullUInt32 ??= new LessThanUInt32(nullValue: null);
                    case TypeCode.UInt64: return _liftedToNullUInt64 ??= new LessThanUInt64(nullValue: null);
                    case TypeCode.Single: return _liftedToNullSingle ??= new LessThanSingle(nullValue: null);
                    case TypeCode.Double: return _liftedToNullDouble ??= new LessThanDouble(nullValue: null);
                    default:
                        throw ContractUtils.Unreachable;
                }
            }

            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.SByte: return _sByte ??= new LessThanSByte(Utils.BoxedFalse);
                case TypeCode.Int16: return _int16 ??= new LessThanInt16(Utils.BoxedFalse);
                case TypeCode.Char: return _char ??= new LessThanChar(Utils.BoxedFalse);
                case TypeCode.Int32: return _int32 ??= new LessThanInt32(Utils.BoxedFalse);
                case TypeCode.Int64: return _int64 ??= new LessThanInt64(Utils.BoxedFalse);
                case TypeCode.Byte: return _byte ??= new LessThanByte(Utils.BoxedFalse);
                case TypeCode.UInt16: return _uInt16 ??= new LessThanUInt16(Utils.BoxedFalse);
                case TypeCode.UInt32: return _uInt32 ??= new LessThanUInt32(Utils.BoxedFalse);
                case TypeCode.UInt64: return _uInt64 ??= new LessThanUInt64(Utils.BoxedFalse);
                case TypeCode.Single: return _single ??= new LessThanSingle(Utils.BoxedFalse);
                case TypeCode.Double: return _double ??= new LessThanDouble(Utils.BoxedFalse);
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class LessThanByte : LessThanInstruction
        {
            public LessThanByte(object? nullValue)
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
                    frame.Push((byte)left < (byte)right);
                }

                return 1;
            }
        }

        private sealed class LessThanChar : LessThanInstruction
        {
            public LessThanChar(object? nullValue)
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
                    frame.Push((char)left < (char)right);
                }

                return 1;
            }
        }

        private sealed class LessThanDouble : LessThanInstruction
        {
            public LessThanDouble(object? nullValue)
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
                    frame.Push((double)left < (double)right);
                }

                return 1;
            }
        }

        private sealed class LessThanInt16 : LessThanInstruction
        {
            public LessThanInt16(object? nullValue)
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
                    frame.Push((short)left < (short)right);
                }

                return 1;
            }
        }

        private sealed class LessThanInt32 : LessThanInstruction
        {
            public LessThanInt32(object? nullValue)
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
                    frame.Push((int)left < (int)right);
                }

                return 1;
            }
        }

        private sealed class LessThanInt64 : LessThanInstruction
        {
            public LessThanInt64(object? nullValue)
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
                    frame.Push((long)left < (long)right);
                }

                return 1;
            }
        }

        private sealed class LessThanSByte : LessThanInstruction
        {
            public LessThanSByte(object? nullValue)
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
                    frame.Push((sbyte)left < (sbyte)right);
                }

                return 1;
            }
        }

        private sealed class LessThanSingle : LessThanInstruction
        {
            public LessThanSingle(object? nullValue)
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
                    frame.Push((float)left < (float)right);
                }

                return 1;
            }
        }

        private sealed class LessThanUInt16 : LessThanInstruction
        {
            public LessThanUInt16(object? nullValue)
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
                    frame.Push((ushort)left < (ushort)right);
                }

                return 1;
            }
        }

        private sealed class LessThanUInt32 : LessThanInstruction
        {
            public LessThanUInt32(object? nullValue)
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
                    frame.Push((uint)left < (uint)right);
                }

                return 1;
            }
        }

        private sealed class LessThanUInt64 : LessThanInstruction
        {
            public LessThanUInt64(object? nullValue)
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
                    frame.Push((ulong)left < (ulong)right);
                }

                return 1;
            }
        }
    }
}

#endif