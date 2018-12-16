#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class ModuloInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private ModuloInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Modulo";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new ModuloInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new ModuloInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new ModuloInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new ModuloUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new ModuloUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new ModuloUInt64());
                case TypeCode.Single: return _single ?? (_single = new ModuloSingle());
                case TypeCode.Double: return _double ?? (_double = new ModuloDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class ModuloDouble : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((double)left % (double)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloInt16 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((short)((short)left % (short)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloInt32 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : ScriptingRuntimeHelpers.Int32ToObject((int)left % (int)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloInt64 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((long)left % (long)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloSingle : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((float)left % (float)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloUInt16 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)unchecked((ushort)((ushort)left % (ushort)right));
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloUInt32 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((uint)left % (uint)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }

        private sealed class ModuloUInt64 : ModuloInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var index = frame.StackIndex;
                var stack = frame.Data;
                var left = stack[index - 2];
                if (left != null)
                {
                    var right = stack[index - 1];
                    stack[index - 2] = right == null ? null : (object)((ulong)left % (ulong)right);
                }

                frame.StackIndex = index - 1;
                return 1;
            }
        }
    }
}

#endif