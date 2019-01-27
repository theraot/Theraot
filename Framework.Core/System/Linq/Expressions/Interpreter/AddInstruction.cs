#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class AddInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private AddInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Add";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new AddInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new AddInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new AddInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new AddUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new AddUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new AddUInt64());
                case TypeCode.Single: return _single ?? (_single = new AddSingle());
                case TypeCode.Double: return _double ?? (_double = new AddDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class AddDouble : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((double)left + (double)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddInt16 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((short)((short)left + (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddInt32 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)left + (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddInt64 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((long)left + (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddSingle : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((float)left + (float)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddUInt16 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ushort)((ushort)left + (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddUInt32 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((uint)left + (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddUInt64 : AddInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ulong)left + (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }

    internal abstract class AddOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64;

        private AddOvfInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "AddOvf";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(type.IsArithmetic());
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new AddOvfInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new AddOvfInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new AddOvfInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new AddOvfUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new AddOvfUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new AddOvfUInt64());
                default:
                    return AddInstruction.Create(type);
            }
        }

        private sealed class AddOvfInt16 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((short)((short)left + (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddOvfInt32 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject(checked((int)left + (int)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddOvfInt64 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((long)left + (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddOvfUInt16 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ushort)((ushort)left + (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddOvfUInt32 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((uint)left + (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class AddOvfUInt64 : AddOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)checked((ulong)left + (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }
}

#endif