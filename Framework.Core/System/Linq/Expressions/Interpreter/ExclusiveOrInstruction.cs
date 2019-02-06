#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class ExclusiveOrInstruction : Instruction
    {
        private static Instruction _sByte, _int16, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _boolean;

        private ExclusiveOrInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "ExclusiveOr";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.SByte: return _sByte ?? (_sByte = new ExclusiveOrSByte());
                case TypeCode.Int16: return _int16 ?? (_int16 = new ExclusiveOrInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new ExclusiveOrInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new ExclusiveOrInt64());
                case TypeCode.Byte: return _byte ?? (_byte = new ExclusiveOrByte());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new ExclusiveOrUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new ExclusiveOrUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new ExclusiveOrUInt64());
                case TypeCode.Boolean: return _boolean ?? (_boolean = new ExclusiveOrBoolean());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class ExclusiveOrBoolean : ExclusiveOrInstruction
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

                frame.Push((bool)left ^ (bool)right);
                return 1;
            }
        }

        private sealed class ExclusiveOrByte : ExclusiveOrInstruction
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

                frame.Push((byte)((byte)left ^ (byte)right));
                return 1;
            }
        }

        private sealed class ExclusiveOrInt16 : ExclusiveOrInstruction
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

                frame.Push((short)((short)left ^ (short)right));
                return 1;
            }
        }

        private sealed class ExclusiveOrInt32 : ExclusiveOrInstruction
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

                frame.Push((int)left ^ (int)right);
                return 1;
            }
        }

        private sealed class ExclusiveOrInt64 : ExclusiveOrInstruction
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

                frame.Push((long)left ^ (long)right);
                return 1;
            }
        }

        private sealed class ExclusiveOrSByte : ExclusiveOrInstruction
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

                frame.Push((sbyte)((sbyte)left ^ (sbyte)right));
                return 1;
            }
        }

        private sealed class ExclusiveOrUInt16 : ExclusiveOrInstruction
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

                frame.Push((ushort)((ushort)left ^ (ushort)right));
                return 1;
            }
        }

        private sealed class ExclusiveOrUInt32 : ExclusiveOrInstruction
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

                frame.Push((uint)left ^ (uint)right);
                return 1;
            }
        }

        private sealed class ExclusiveOrUInt64 : ExclusiveOrInstruction
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

                frame.Push((ulong)left ^ (ulong)right);
                return 1;
            }
        }
    }
}

#endif