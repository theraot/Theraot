#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class SubInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private SubInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Sub";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new SubInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new SubInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new SubInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new SubUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new SubUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new SubUInt64());
                case TypeCode.Single: return _single ?? (_single = new SubSingle());
                case TypeCode.Double: return _double ?? (_double = new SubDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class SubDouble : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((double)left - (double)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubInt16 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((short)((short)left - (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubInt32 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)left - (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubInt64 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((long)left - (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubSingle : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((float)left - (float)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubUInt16 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ushort)((ushort)left - (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubUInt32 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((uint)left - (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubUInt64 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ulong)left - (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }

    internal abstract class SubOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64;

        private SubOvfInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "SubOvf";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new SubOvfInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new SubOvfInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new SubOvfInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new SubOvfUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new SubOvfUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new SubOvfUInt64());
                default:
                    return SubInstruction.Create(type);
            }
        }

        private sealed class SubOvfInt16 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((short)((short)left - (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubOvfInt32 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(checked((int)left - (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubOvfInt64 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((long)left - (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubOvfUInt16 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ushort)((ushort)left - (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubOvfUInt32 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((uint)left - (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class SubOvfUInt64 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ulong)left - (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }
}

#endif