#if LESSTHAN_NET35

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Theraot.Reflection;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NotEqualInstruction : Instruction
    {
        // Perf: EqualityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _reference, _boolean, _sByte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;

        private static Instruction _sByteLiftedToNull, _int16LiftedToNull, _charLiftedToNull, _int32LiftedToNull, _int64LiftedToNull, _byteLiftedToNull, _uInt16LiftedToNull, _uInt32LiftedToNull, _uInt64LiftedToNull, _singleLiftedToNull, _doubleLiftedToNull;

        private NotEqualInstruction()
        {
        }

        public override int ConsumedStack => 2;
        public override string InstructionName => "NotEqual";
        public override int ProducedStack => 1;

        public static Instruction Create(Type type, bool liftedToNull)
        {
            if (liftedToNull)
            {
                switch (type.GetNonNullable().GetTypeCode())
                {
                    case TypeCode.Boolean: return ExclusiveOrInstruction.Create(type);
                    case TypeCode.SByte: return _sByteLiftedToNull ?? (_sByteLiftedToNull = new NotEqualSByteLiftedToNull());
                    case TypeCode.Int16: return _int16LiftedToNull ?? (_int16LiftedToNull = new NotEqualInt16LiftedToNull());
                    case TypeCode.Char: return _charLiftedToNull ?? (_charLiftedToNull = new NotEqualCharLiftedToNull());
                    case TypeCode.Int32: return _int32LiftedToNull ?? (_int32LiftedToNull = new NotEqualInt32LiftedToNull());
                    case TypeCode.Int64: return _int64LiftedToNull ?? (_int64LiftedToNull = new NotEqualInt64LiftedToNull());
                    case TypeCode.Byte: return _byteLiftedToNull ?? (_byteLiftedToNull = new NotEqualByteLiftedToNull());
                    case TypeCode.UInt16: return _uInt16LiftedToNull ?? (_uInt16LiftedToNull = new NotEqualUInt16LiftedToNull());
                    case TypeCode.UInt32: return _uInt32LiftedToNull ?? (_uInt32LiftedToNull = new NotEqualUInt32LiftedToNull());
                    case TypeCode.UInt64: return _uInt64LiftedToNull ?? (_uInt64LiftedToNull = new NotEqualUInt64LiftedToNull());
                    case TypeCode.Single: return _singleLiftedToNull ?? (_singleLiftedToNull = new NotEqualSingleLiftedToNull());
                    default:
                        Debug.Assert(type.GetNonNullable().GetTypeCode() == TypeCode.Double);
                        return _doubleLiftedToNull ?? (_doubleLiftedToNull = new NotEqualDoubleLiftedToNull());
                }
            }

            switch (type.GetNonNullable().GetTypeCode())
            {
                case TypeCode.Boolean: return _boolean ?? (_boolean = new NotEqualBoolean());
                case TypeCode.SByte: return _sByte ?? (_sByte = new NotEqualSByte());
                case TypeCode.Int16: return _int16 ?? (_int16 = new NotEqualInt16());
                case TypeCode.Char: return _char ?? (_char = new NotEqualChar());
                case TypeCode.Int32: return _int32 ?? (_int32 = new NotEqualInt32());
                case TypeCode.Int64: return _int64 ?? (_int64 = new NotEqualInt64());
                case TypeCode.Byte: return _byte ?? (_byte = new NotEqualByte());
                case TypeCode.UInt16: return _uInt16 ?? (_uInt16 = new NotEqualUInt16());
                case TypeCode.UInt32: return _uInt32 ?? (_uInt32 = new NotEqualUInt32());
                case TypeCode.UInt64: return _uInt64 ?? (_uInt64 = new NotEqualUInt64());
                case TypeCode.Single: return _single ?? (_single = new NotEqualSingle());
                case TypeCode.Double: return _double ?? (_double = new NotEqualDouble());
                default:
                    // Nullable only valid if one operand is constant null, so this assert is slightly too broad.
                    Debug.Assert(type.CanBeNull());
                    return _reference ?? (_reference = new NotEqualReference());
            }
        }

        private sealed class NotEqualBoolean : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((bool)left != (bool)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((byte)left != (byte)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualByteLiftedToNull : NotEqualInstruction
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
                    frame.Push((byte)left != (byte)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualChar : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((char)left != (char)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualCharLiftedToNull : NotEqualInstruction
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
                    frame.Push((char)left != (char)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualDouble : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((double)left != (double)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualDoubleLiftedToNull : NotEqualInstruction
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
                    frame.Push((double)left != (double)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((short)left != (short)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt16LiftedToNull : NotEqualInstruction
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
                    frame.Push((short)left != (short)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((int)left != (int)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt32LiftedToNull : NotEqualInstruction
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
                    frame.Push((int)left != (int)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((long)left != (long)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualInt64LiftedToNull : NotEqualInstruction
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
                    frame.Push((long)left != (long)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualReference : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                frame.Push(frame.Pop() != frame.Pop());
                return 1;
            }
        }

        private sealed class NotEqualSByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((sbyte)left != (sbyte)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualSByteLiftedToNull : NotEqualInstruction
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
                    frame.Push((sbyte)left != (sbyte)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualSingle : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((float)left != (float)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualSingleLiftedToNull : NotEqualInstruction
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
                    frame.Push((float)left != (float)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((ushort)left != (ushort)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt16LiftedToNull : NotEqualInstruction
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
                    frame.Push((ushort)left != (ushort)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((uint)left != (uint)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt32LiftedToNull : NotEqualInstruction
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
                    frame.Push((uint)left != (uint)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(right != null);
                }
                else if (right == null)
                {
                    frame.Push(true);
                }
                else
                {
                    frame.Push((ulong)left != (ulong)right);
                }
                return 1;
            }
        }

        private sealed class NotEqualUInt64LiftedToNull : NotEqualInstruction
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
                    frame.Push((ulong)left != (ulong)right);
                }
                return 1;
            }
        }
    }
}

#endif