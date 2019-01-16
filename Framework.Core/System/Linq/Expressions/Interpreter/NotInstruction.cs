#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NotInstruction : Instruction
    {
        public static Instruction Boolean, Int64, Int32, Int16, UInt64, UInt32, UInt16, Byte, SByte;

        private NotInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Not";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Boolean: return Boolean ?? (Boolean = new NotBoolean());
                case TypeCode.Int64: return Int64 ?? (Int64 = new NotInt64());
                case TypeCode.Int32: return Int32 ?? (Int32 = new NotInt32());
                case TypeCode.Int16: return Int16 ?? (Int16 = new NotInt16());
                case TypeCode.UInt64: return UInt64 ?? (UInt64 = new NotUInt64());
                case TypeCode.UInt32: return UInt32 ?? (UInt32 = new NotUInt32());
                case TypeCode.UInt16: return UInt16 ?? (UInt16 = new NotUInt16());
                case TypeCode.Byte: return Byte ?? (Byte = new NotByte());
                case TypeCode.SByte: return SByte ?? (SByte = new NotSByte());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class NotBoolean : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(!(bool)value);
                }
                return 1;
            }
        }

        private sealed class NotByte : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((byte)~(byte)value));
                }
                return 1;
            }
        }

        private sealed class NotInt16 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)~(short)value);
                }
                return 1;
            }
        }

        private sealed class NotInt32 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(int)value);
                }
                return 1;
            }
        }

        private sealed class NotInt64 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(long)value);
                }
                return 1;
            }
        }

        private sealed class NotSByte : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((sbyte)~(sbyte)value);
                }
                return 1;
            }
        }

        private sealed class NotUInt16 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((ushort)~(ushort)value));
                }
                return 1;
            }
        }

        private sealed class NotUInt32 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(uint)value);
                }
                return 1;
            }
        }

        private sealed class NotUInt64 : NotInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var value = frame.Pop();
                if (value == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(~(ulong)value);
                }
                return 1;
            }
        }
    }
}

#endif