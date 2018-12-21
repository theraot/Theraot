#if NET20 || NET30

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class IncrementInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private IncrementInstruction()
        {
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Increment";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new IncrementInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new IncrementInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new IncrementInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new IncrementUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new IncrementUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new IncrementUInt64());
                case TypeCode.Single: return _single ?? (_single = new IncrementSingle());
                case TypeCode.Double: return _double ?? (_double = new IncrementDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class IncrementDouble : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(1 + (double)obj);
                }
                return 1;
            }
        }

        private sealed class IncrementInt16 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((short)(1 + (short)obj)));
                }
                return 1;
            }
        }

        private sealed class IncrementInt32 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(1 + (int)obj));
                }
                return 1;
            }
        }

        private sealed class IncrementInt64 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(1 + (long)obj));
                }
                return 1;
            }
        }

        private sealed class IncrementSingle : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(1 + (float)obj);
                }
                return 1;
            }
        }

        private sealed class IncrementUInt16 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked((ushort)(1 + (ushort)obj)));
                }
                return 1;
            }
        }

        private sealed class IncrementUInt32 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(1 + (uint)obj));
                }
                return 1;
            }
        }

        private sealed class IncrementUInt64 : IncrementInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var obj = frame.Pop();
                if (obj == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push(unchecked(1 + (ulong)obj));
                }
                return 1;
            }
        }
    }
}

#endif