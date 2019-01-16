#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class LeftShiftInstruction : Instruction
    {
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64;

        private LeftShiftInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "LeftShift";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.SByte: return _sByte ?? (_sByte = new LeftShiftSByte());
                case TypeCode.Int16: return _int16 ?? (_int16 = new LeftShiftInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new LeftShiftInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new LeftShiftInt64());
                case TypeCode.Byte: return _byte ?? (_byte = new LeftShiftByte());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new LeftShiftUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new LeftShiftUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new LeftShiftUInt64());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class LeftShiftByte : LeftShiftInstruction
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
                    frame.Push(unchecked((byte)((byte)value << (int)shift)));
                }
                return 1;
            }
        }

        private sealed class LeftShiftInt16 : LeftShiftInstruction
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
                    frame.Push(unchecked((short)((short)value << (int)shift)));
                }
                return 1;
            }
        }

        private sealed class LeftShiftInt32 : LeftShiftInstruction
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
                    frame.Push((int)value << (int)shift);
                }
                return 1;
            }
        }

        private sealed class LeftShiftInt64 : LeftShiftInstruction
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
                    frame.Push((long)value << (int)shift);
                }
                return 1;
            }
        }

        private sealed class LeftShiftSByte : LeftShiftInstruction
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
                    frame.Push(unchecked((sbyte)((sbyte)value << (int)shift)));
                }
                return 1;
            }
        }

        private sealed class LeftShiftUInt16 : LeftShiftInstruction
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
                    frame.Push(unchecked((ushort)((ushort)value << (int)shift)));
                }
                return 1;
            }
        }

        private sealed class LeftShiftUInt32 : LeftShiftInstruction
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
                    frame.Push((uint)value << (int)shift);
                }
                return 1;
            }
        }

        private sealed class LeftShiftUInt64 : LeftShiftInstruction
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
                    frame.Push((ulong)value << (int)shift);
                }
                return 1;
            }
        }
    }
}

#endif