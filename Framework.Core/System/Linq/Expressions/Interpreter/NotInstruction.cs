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
        private static Instruction? _boolean, _int64, _int32, _int16, _uInt64, _uInt32, _uInt16, _byte, _sByte;

        private NotInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Not";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.Boolean: return _boolean ??= new NotBoolean();
                case TypeCode.Int64: return _int64 ??= new NotInt64();
                case TypeCode.Int32: return _int32 ??= new NotInt32();
                case TypeCode.Int16: return _int16 ??= new NotInt16();
                case TypeCode.UInt64: return _uInt64 ??= new NotUInt64();
                case TypeCode.UInt32: return _uInt32 ??= new NotUInt32();
                case TypeCode.UInt16: return _uInt16 ??= new NotUInt16();
                case TypeCode.Byte: return _byte ??= new NotByte();
                case TypeCode.SByte: return _sByte ??= new NotSByte();
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