#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Dynamic.Utils;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class DecrementInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

        private DecrementInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 1;
        public override string InstructionName => "Decrement";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Int16: return _int16 ?? (_int16 = new DecrementInt16());
                case TypeCode.Int32: return _int32 ?? (_int32 = new DecrementInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new DecrementInt64());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new DecrementUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new DecrementUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new DecrementUInt64());
                case TypeCode.Single: return _single ?? (_single = new DecrementSingle());
                case TypeCode.Double: return _double ?? (_double = new DecrementDouble());
                default:
                    throw ContractUtils.Unreachable;
            }
        }

        private sealed class DecrementDouble : DecrementInstruction
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
                    frame.Push((double)obj - 1);
                }

                return 1;
            }
        }

        private sealed class DecrementInt16 : DecrementInstruction
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
                    frame.Push(unchecked((short)((short)obj - 1)));
                }

                return 1;
            }
        }

        private sealed class DecrementInt32 : DecrementInstruction
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
                    frame.Push(unchecked((int)obj - 1));
                }

                return 1;
            }
        }

        private sealed class DecrementInt64 : DecrementInstruction
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
                    frame.Push(unchecked((long)obj - 1));
                }

                return 1;
            }
        }

        private sealed class DecrementSingle : DecrementInstruction
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
                    frame.Push((float)obj - 1);
                }

                return 1;
            }
        }

        private sealed class DecrementUInt16 : DecrementInstruction
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
                    frame.Push(unchecked((ushort)((ushort)obj - 1)));
                }

                return 1;
            }
        }

        private sealed class DecrementUInt32 : DecrementInstruction
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
                    frame.Push(unchecked((uint)obj - 1));
                }

                return 1;
            }
        }

        private sealed class DecrementUInt64 : DecrementInstruction
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
                    frame.Push(unchecked((ulong)obj - 1));
                }

                return 1;
            }
        }
    }
}

#endif