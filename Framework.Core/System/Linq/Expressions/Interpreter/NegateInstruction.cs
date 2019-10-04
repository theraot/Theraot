#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NegateCheckedInstruction : Instruction
    {
        private static Instruction? _int16, _int32, _int64;

        private NegateCheckedInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "NegateChecked";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.Int16: return _int16 ??= new NegateCheckedInt16();
                case TypeCode.Int32: return _int32 ??= new NegateCheckedInt32();
                case TypeCode.Int64: return _int64 ??= new NegateCheckedInt64();
                default:
                    return NegateInstruction.Create(type);
            }
        }

        private sealed class NegateCheckedInt16 : NegateCheckedInstruction
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
                    frame.Push(checked((short)-(short)obj));
                }

                return 1;
            }
        }

        private sealed class NegateCheckedInt32 : NegateCheckedInstruction
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
                    frame.Push(checked(-(int)obj));
                }

                return 1;
            }
        }

        private sealed class NegateCheckedInt64 : NegateCheckedInstruction
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
                    frame.Push(checked(-(long)obj));
                }

                return 1;
            }
        }
    }

    internal abstract class NegateInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _single, _double;

        private NegateInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Negate";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (Type.GetTypeCode(type.GetNonNullable()))
            {
                case TypeCode.Int16: return _int16 ??= new NegateInt16();
                case TypeCode.Int32: return _int32 ??= new NegateInt32();
                case TypeCode.Int64: return _int64 ??= new NegateInt64();
                case TypeCode.Single: return _single ??= new NegateSingle();
                case TypeCode.Double: return _double ??= new NegateDouble();
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class NegateDouble : NegateInstruction
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
                    frame.Push(-(double)obj);
                }

                return 1;
            }
        }

        private sealed class NegateInt16 : NegateInstruction
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
                    frame.Push(unchecked((short)-(short)obj));
                }

                return 1;
            }
        }

        private sealed class NegateInt32 : NegateInstruction
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
                    frame.Push(unchecked(-(int)obj));
                }

                return 1;
            }
        }

        private sealed class NegateInt64 : NegateInstruction
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
                    frame.Push(unchecked(-(long)obj));
                }

                return 1;
            }
        }

        private sealed class NegateSingle : NegateInstruction
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
                    frame.Push(-(float)obj);
                }

                return 1;
            }
        }
    }
}

#endif