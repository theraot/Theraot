#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class RightShiftInstruction : Instruction
    {
        private static Instruction? _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64;

        private RightShiftInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "RightShift";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.SByte: return _sByte ??= new RightShiftSByte();
                case TypeCode.Int16: return _int16 ??= new RightShiftInt16();
                case TypeCode.Int32: return _int32 ??= new RightShiftInt32();
                case TypeCode.Int64: return _int64 ??= new RightShiftInt64();
                case TypeCode.Byte: return _byte ??= new RightShiftByte();
                case TypeCode.UInt16: return _uInt16 ??= new RightShiftUInt16();
                case TypeCode.UInt32: return _uInt32 ??= new RightShiftUInt32();
                case TypeCode.UInt64: return _uInt64 ??= new RightShiftUInt64();
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class RightShiftByte : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((byte)((byte)value >> (int)shift));
                }

                return 1;
            }
        }

        private sealed class RightShiftInt16 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)((short)value >> (int)shift));
                }

                return 1;
            }
        }

        private sealed class RightShiftInt32 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((int)value >> (int)shift);
                }

                return 1;
            }
        }

        private sealed class RightShiftInt64 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((long)value >> (int)shift);
                }

                return 1;
            }
        }

        private sealed class RightShiftSByte : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((sbyte)((sbyte)value >> (int)shift));
                }

                return 1;
            }
        }

        private sealed class RightShiftUInt16 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ushort)((ushort)value >> (int)shift));
                }

                return 1;
            }
        }

        private sealed class RightShiftUInt32 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((uint)value >> (int)shift);
                }

                return 1;
            }
        }

        private sealed class RightShiftUInt64 : RightShiftInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var shift = frame.Pop();
                var value = frame.Pop();
                if (value == null || shift == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ulong)value >> (int)shift);
                }

                return 1;
            }
        }
    }
}

#endif