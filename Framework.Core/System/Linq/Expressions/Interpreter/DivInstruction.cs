#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class DivInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private DivInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Div";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new DivInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new DivInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new DivInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new DivUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new DivUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new DivUInt64());
                case TypeCode.Single: return _single ?? (_single = new DivSingle());
                case TypeCode.Double: return _double ?? (_double = new DivDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class DivDouble : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((double)left / (double)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivInt16 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((short)((short)left / (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivInt32 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject((int)left / (int)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivInt64 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((long)left / (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivSingle : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((float)left / (float)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivUInt16 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ushort)((ushort)left / (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivUInt32 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((uint)left / (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class DivUInt64 : DivInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((ulong)left / (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }
}

#endif