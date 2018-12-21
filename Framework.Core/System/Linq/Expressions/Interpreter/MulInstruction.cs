#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class MulInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private MulInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Mul";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new MulInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new MulInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new MulInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new MulUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new MulUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new MulUInt64());
                case TypeCode.Single: return _single ?? (_single = new MulSingle());
                case TypeCode.Double: return _double ?? (_double = new MulDouble());

                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class MulDouble : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((double)left * (double)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulInt16 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((short)((short)left * (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulInt32 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)left * (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulInt64 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((long)left * (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulSingle : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((float)left * (float)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulUInt16 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ushort)((ushort)left * (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulUInt32 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((uint)left * (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulUInt64 : MulInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ulong)left * (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }

    internal abstract class MulOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64;

        private MulOvfInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "MulOvf";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new MulOvfInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new MulOvfInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new MulOvfInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new MulOvfUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new MulOvfUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new MulOvfUInt64());
                default:
                    return MulInstruction.Create(type);
            }
        }

        private sealed class MulOvfInt16 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((short)((short)left * (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulOvfInt32 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(checked((int)left * (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulOvfInt64 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((long)left * (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulOvfUInt16 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ushort)((ushort)left * (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulOvfUInt32 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((uint)left * (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class MulOvfUInt64 : MulOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ulong)left * (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }
}

#endif