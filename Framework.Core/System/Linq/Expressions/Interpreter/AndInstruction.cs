#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class AndInstruction : Instruction
    {
        private static Instruction? _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _boolean;

        private AndInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "And";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.SByte: return _sByte ??= new AndSByte();
                case TypeCode.Int16: return _int16 ??= new AndInt16();
                case TypeCode.Int32: return _int32 ??= new AndInt32();
                case TypeCode.Int64: return _int64 ??= new AndInt64();
                case TypeCode.Byte: return _byte ??= new AndByte();
                case TypeCode.UInt16: return _uInt16 ??= new AndUInt16();
                case TypeCode.UInt32: return _uInt32 ??= new AndUInt32();
                case TypeCode.UInt64: return _uInt64 ??= new AndUInt64();
                case TypeCode.Boolean: return _boolean ??= new AndBoolean();
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class AndBoolean : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    if (right == null)
                    {
                        frame.Push(null);
                    }
                    else
                    {
                        frame.Push((bool)right ? null : Utils.BoxedFalse);
                    }

                    return 1;
                }

                if (right == null)
                {
                    frame.Push((bool)left ? null : Utils.BoxedFalse);
                    return 1;
                }

                frame.Push((bool)left && (bool)right);
                return 1;
            }
        }

        private sealed class AndByte : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((byte)((byte)left & (byte)right));
                return 1;
            }
        }

        private sealed class AndInt16 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((short)((short)left & (short)right));
                return 1;
            }
        }

        private sealed class AndInt32 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((int)left & (int)right);
                return 1;
            }
        }

        private sealed class AndInt64 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((long)left & (long)right);
                return 1;
            }
        }

        private sealed class AndSByte : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((sbyte)((sbyte)left & (sbyte)right));
                return 1;
            }
        }

        private sealed class AndUInt16 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((ushort)((ushort)left & (ushort)right));
                return 1;
            }
        }

        private sealed class AndUInt32 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((uint)left & (uint)right);
                return 1;
            }
        }

        private sealed class AndUInt64 : AndInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                    return 1;
                }

                frame.Push((ulong)left & (ulong)right);
                return 1;
            }
        }
    }
}

#endif