#if LESSTHAN_NET35

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
#pragma warning disable RECS0088 // Comparing equal expression for equality is usually useless
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable EqualExpressionComparison

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class EqualInstruction : Instruction
    {
        private static Instruction _booleanLiftedToNull, _sByteLiftedToNull, _int16LiftedToNull, _charLiftedToNull, _int32LiftedToNull, _int64LiftedToNull, _byteLiftedToNull, _uInt16LiftedToNull, _uInt32LiftedToNull, _uInt64LiftedToNull, _singleLiftedToNull, _doubleLiftedToNull;

        // Perf: EqualityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _reference, _boolean, _sByte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;

        private EqualInstruction()
        {
            // Empty
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "Equal";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type, bool liftedToNull)
        {
            if (liftedToNull)
            {
                switch (type.GetNonNullable().GetTypeCode())
                {
                    case TypeCode.Boolean: return _booleanLiftedToNull ?? (_booleanLiftedToNull = new EqualBooleanLiftedToNull());
                    case TypeCode.SByte: return _sByteLiftedToNull ?? (_sByteLiftedToNull = new EqualSByteLiftedToNull());
                    case TypeCode.Int16: return _int16LiftedToNull ?? (_int16LiftedToNull = new EqualInt16LiftedToNull());
                    case TypeCode.Char: return _charLiftedToNull ?? (_charLiftedToNull = new EqualCharLiftedToNull());
                    case TypeCode.Int32: return _int32LiftedToNull ?? (_int32LiftedToNull = new EqualInt32LiftedToNull());
                    case TypeCode.Int64: return _int64LiftedToNull ?? (_int64LiftedToNull = new EqualInt64LiftedToNull());
                    case TypeCode.Byte: return _byteLiftedToNull ?? (_byteLiftedToNull = new EqualByteLiftedToNull());
                    case TypeCode.UInt16: return _uInt16LiftedToNull ?? (_uInt16LiftedToNull = new EqualUInt16LiftedToNull());
                    case TypeCode.UInt32: return _uInt32LiftedToNull ?? (_uInt32LiftedToNull = new EqualUInt32LiftedToNull());
                    case TypeCode.UInt64: return _uInt64LiftedToNull ?? (_uInt64LiftedToNull = new EqualUInt64LiftedToNull());
                    case TypeCode.Single: return _singleLiftedToNull ?? (_singleLiftedToNull = new EqualSingleLiftedToNull());
                    default:
                        Debug.Assert(type.GetNonNullable().GetTypeCode() == TypeCode.Double);
                        return _doubleLiftedToNull ?? (_doubleLiftedToNull = new EqualDoubleLiftedToNull());
                }
            }

            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Boolean: return _boolean ?? (_boolean = new EqualBoolean());
                case TypeCode.SByte: return _sByte ?? (_sByte = new EqualSByte());
                case TypeCode.Int16: return _int16 ?? (_int16 = new EqualInt16());
                case TypeCode.Char: return _char ?? (_char = new EqualChar());
                case TypeCode.Int32: return _int32 ?? (_int32 = new EqualInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new EqualInt64());
                case TypeCode.Byte: return _byte ?? (_byte = new EqualByte());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new EqualUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new EqualUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new EqualUInt64());
                case TypeCode.Single: return _single ?? (_single = new EqualSingle());
                case TypeCode.Double: return _double ?? (_double = new EqualDouble());
                default:
                    // Nullable only valid if one operand is constant null, so this assert is slightly too broad.
                    Debug.Assert(type.CanBeNull());
                    return _reference ?? (_reference = new EqualReference());
            }
        }

        private sealed class EqualBoolean : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((bool)left == (bool)right);
                }

                return 1;
            }
        }

        private sealed class EqualBooleanLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((bool)left == (bool)right);
                }

                return 1;
            }
        }

        private sealed class EqualByte : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((byte)left == (byte)right);
                }

                return 1;
            }
        }

        private sealed class EqualByteLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((byte)left == (byte)right);
                }

                return 1;
            }
        }

        private sealed class EqualChar : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((char)left == (char)right);
                }

                return 1;
            }
        }

        private sealed class EqualCharLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((char)left == (char)right);
                }

                return 1;
            }
        }

        private sealed class EqualDouble : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((double)left == (double)right);
                }

                return 1;
            }
        }

        private sealed class EqualDoubleLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((double)left == (double)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt16 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((short)left == (short)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt16LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((short)left == (short)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt32 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((int)left == (int)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt32LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((int)left == (int)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt64 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((long)left == (long)right);
                }

                return 1;
            }
        }

        private sealed class EqualInt64LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((long)left == (long)right);
                }

                return 1;
            }
        }

        private sealed class EqualReference : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                frame.Push(frame.Pop() == frame.Pop());
                return 1;
            }
        }

        private sealed class EqualSByte : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((sbyte)left == (sbyte)right);
                }

                return 1;
            }
        }

        private sealed class EqualSByteLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((sbyte)left == (sbyte)right);
                }

                return 1;
            }
        }

        private sealed class EqualSingle : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((float)left == (float)right);
                }

                return 1;
            }
        }

        private sealed class EqualSingleLiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((float)left == (float)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt16 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((ushort)left == (ushort)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt16LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ushort)left == (ushort)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt32 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((uint)left == (uint)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt32LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((uint)left == (uint)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt64 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right == null);
                }
                else if (right == null)
                {
                    frame.Push(false);
                }
                else
                {
                    frame.Push((ulong)left == (ulong)right);
                }

                return 1;
            }
        }

        private sealed class EqualUInt64LiftedToNull : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(null);
                }
                else
                {
                    frame.Push((ulong)left == (ulong)right);
                }

                return 1;
            }
        }
    }
}

#endif