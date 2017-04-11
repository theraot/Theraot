#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class LessThanInstruction : Instruction
    {
        private readonly object _nullValue;

        private static Instruction _sbyte, _int16, _char, _int32, _int64, _byte, _uint16, _uint32, _uint64, _single, _double;
        private static Instruction _liftedToNullSByte, _liftedToNullInt16, _liftedToNullChar, _liftedToNullInt32, _liftedToNullInt64, _liftedToNullByte, _liftedToNullUInt16, _liftedToNullUInt32, _liftedToNullUInt64, _liftedToNullSingle, _liftedToNullDouble;

        public override int ConsumedStack
        {
            get { return 2; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "LessThan"; }
        }

        private LessThanInstruction(object nullValue)
        {
            _nullValue = nullValue;
        }

        internal sealed class LessThanSByte : LessThanInstruction
        {
            public LessThanSByte(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((SByte)left) < (SByte)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanInt16 : LessThanInstruction
        {
            public LessThanInt16(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int16)left) < (Int16)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanChar : LessThanInstruction
        {
            public LessThanChar(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Char)left) < (Char)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanInt32 : LessThanInstruction
        {
            public LessThanInt32(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int32)left) < (Int32)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanInt64 : LessThanInstruction
        {
            public LessThanInt64(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int64)left) < (Int64)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanByte : LessThanInstruction
        {
            public LessThanByte(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Byte)left) < (Byte)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanUInt16 : LessThanInstruction
        {
            public LessThanUInt16(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt16)left) < (UInt16)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanUInt32 : LessThanInstruction
        {
            public LessThanUInt32(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt32)left) < (UInt32)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanUInt64 : LessThanInstruction
        {
            public LessThanUInt64(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt64)left) < (UInt64)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanSingle : LessThanInstruction
        {
            public LessThanSingle(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Single)left) < (Single)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanDouble : LessThanInstruction
        {
            public LessThanDouble(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Double)left) < (Double)right);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            return Create(type, false);
        }

        public static Instruction Create(Type type, bool liftedToNull)
        {
            Debug.Assert(!type.IsEnum);
            if (liftedToNull)
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.SByte:
                        return _liftedToNullSByte ?? (_liftedToNullSByte = new LessThanSByte(null));

                    case TypeCode.Byte:
                        return _liftedToNullByte ?? (_liftedToNullByte = new LessThanByte(null));

                    case TypeCode.Char:
                        return _liftedToNullChar ?? (_liftedToNullChar = new LessThanChar(null));

                    case TypeCode.Int16:
                        return _liftedToNullInt16 ?? (_liftedToNullInt16 = new LessThanInt16(null));

                    case TypeCode.Int32:
                        return _liftedToNullInt32 ?? (_liftedToNullInt32 = new LessThanInt32(null));

                    case TypeCode.Int64:
                        return _liftedToNullInt64 ?? (_liftedToNullInt64 = new LessThanInt64(null));

                    case TypeCode.UInt16:
                        return _liftedToNullUInt16 ?? (_liftedToNullUInt16 = new LessThanUInt16(null));

                    case TypeCode.UInt32:
                        return _liftedToNullUInt32 ?? (_liftedToNullUInt32 = new LessThanUInt32(null));

                    case TypeCode.UInt64:
                        return _liftedToNullUInt64 ?? (_liftedToNullUInt64 = new LessThanUInt64(null));

                    case TypeCode.Single:
                        return _liftedToNullSingle ?? (_liftedToNullSingle = new LessThanSingle(null));

                    case TypeCode.Double:
                        return _liftedToNullDouble ?? (_liftedToNullDouble = new LessThanDouble(null));

                    default:
                        throw Error.ExpressionNotSupportedForType("LessThan", type);
                }
            }
            else
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.SByte:
                        return _sbyte ?? (_sbyte = new LessThanSByte(ScriptingRuntimeHelpers.False));

                    case TypeCode.Byte:
                        return _byte ?? (_byte = new LessThanByte(ScriptingRuntimeHelpers.False));

                    case TypeCode.Char:
                        return _char ?? (_char = new LessThanChar(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int16:
                        return _int16 ?? (_int16 = new LessThanInt16(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int32:
                        return _int32 ?? (_int32 = new LessThanInt32(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int64:
                        return _int64 ?? (_int64 = new LessThanInt64(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt16:
                        return _uint16 ?? (_uint16 = new LessThanUInt16(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt32:
                        return _uint32 ?? (_uint32 = new LessThanUInt32(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt64:
                        return _uint64 ?? (_uint64 = new LessThanUInt64(ScriptingRuntimeHelpers.False));

                    case TypeCode.Single:
                        return _single ?? (_single = new LessThanSingle(ScriptingRuntimeHelpers.False));

                    case TypeCode.Double:
                        return _double ?? (_double = new LessThanDouble(ScriptingRuntimeHelpers.False));

                    default:
                        throw Error.ExpressionNotSupportedForType("LessThan", type);
                }
            }
        }

        public override string ToString()
        {
            return "LessThan()";
        }
    }

    internal abstract class LessThanOrEqualInstruction : Instruction
    {
        private readonly object _nullValue;
        private static Instruction _sbyte, _int16, _char, _int32, _int64, _byte, _uint16, _uint32, _uint64, _single, _double;
        private static Instruction _liftedToNullSByte, _liftedToNullInt16, _liftedToNullChar, _liftedToNullInt32, _liftedToNullInt64, _liftedToNullByte, _liftedToNullUInt16, _liftedToNullUInt32, _liftedToNullUInt64, _liftedToNullSingle, _liftedToNullDouble;

        public override int ConsumedStack
        {
            get { return 2; }
        }

        public override int ProducedStack
        {
            get { return 1; }
        }

        public override string InstructionName
        {
            get { return "LessThanOrEqual"; }
        }

        private LessThanOrEqualInstruction(object nullValue)
        {
            _nullValue = nullValue;
        }

        internal sealed class LessThanOrEqualSByte : LessThanOrEqualInstruction
        {
            public LessThanOrEqualSByte(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((SByte)left) <= (SByte)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualInt16 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt16(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int16)left) <= (Int16)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualChar : LessThanOrEqualInstruction
        {
            public LessThanOrEqualChar(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Char)left) <= (Char)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualInt32 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt32(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int32)left) <= (Int32)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualInt64 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualInt64(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Int64)left) <= (Int64)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualByte : LessThanOrEqualInstruction
        {
            public LessThanOrEqualByte(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Byte)left) <= (Byte)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualUInt16 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt16(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt16)left) <= (UInt16)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualUInt32 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt32(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt32)left) <= (UInt32)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualUInt64 : LessThanOrEqualInstruction
        {
            public LessThanOrEqualUInt64(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((UInt64)left) <= (UInt64)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualSingle : LessThanOrEqualInstruction
        {
            public LessThanOrEqualSingle(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Single)left) <= (Single)right);
                }
                return +1;
            }
        }

        internal sealed class LessThanOrEqualDouble : LessThanOrEqualInstruction
        {
            public LessThanOrEqualDouble(object nullValue)
                : base(nullValue)
            {
            }

            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null || right == null)
                {
                    frame.Push(_nullValue);
                }
                else
                {
                    frame.Push(((Double)left) <= (Double)right);
                }
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            return Create(type, false);
        }

        public static Instruction Create(Type type, bool liftedToNull)
        {
            Debug.Assert(!type.IsEnum);
            if (liftedToNull)
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.SByte:
                        return _liftedToNullSByte ?? (_liftedToNullSByte = new LessThanOrEqualSByte(null));

                    case TypeCode.Byte:
                        return _liftedToNullByte ?? (_liftedToNullByte = new LessThanOrEqualByte(null));

                    case TypeCode.Char:
                        return _liftedToNullChar ?? (_liftedToNullChar = new LessThanOrEqualChar(null));

                    case TypeCode.Int16:
                        return _liftedToNullInt16 ?? (_liftedToNullInt16 = new LessThanOrEqualInt16(null));

                    case TypeCode.Int32:
                        return _liftedToNullInt32 ?? (_liftedToNullInt32 = new LessThanOrEqualInt32(null));

                    case TypeCode.Int64:
                        return _liftedToNullInt64 ?? (_liftedToNullInt64 = new LessThanOrEqualInt64(null));

                    case TypeCode.UInt16:
                        return _liftedToNullUInt16 ?? (_liftedToNullUInt16 = new LessThanOrEqualUInt16(null));

                    case TypeCode.UInt32:
                        return _liftedToNullUInt32 ?? (_liftedToNullUInt32 = new LessThanOrEqualUInt32(null));

                    case TypeCode.UInt64:
                        return _liftedToNullUInt64 ?? (_liftedToNullUInt64 = new LessThanOrEqualUInt64(null));

                    case TypeCode.Single:
                        return _liftedToNullSingle ?? (_liftedToNullSingle = new LessThanOrEqualSingle(null));

                    case TypeCode.Double:
                        return _liftedToNullDouble ?? (_liftedToNullDouble = new LessThanOrEqualDouble(null));

                    default:
                        throw Error.ExpressionNotSupportedForType("LessThanOrEqual", type);
                }
            }
            else
            {
                switch (type.GetNonNullableType().GetTypeCode())
                {
                    case TypeCode.SByte:
                        return _sbyte ?? (_sbyte = new LessThanOrEqualSByte(ScriptingRuntimeHelpers.False));

                    case TypeCode.Byte:
                        return _byte ?? (_byte = new LessThanOrEqualByte(ScriptingRuntimeHelpers.False));

                    case TypeCode.Char:
                        return _char ?? (_char = new LessThanOrEqualChar(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int16:
                        return _int16 ?? (_int16 = new LessThanOrEqualInt16(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int32:
                        return _int32 ?? (_int32 = new LessThanOrEqualInt32(ScriptingRuntimeHelpers.False));

                    case TypeCode.Int64:
                        return _int64 ?? (_int64 = new LessThanOrEqualInt64(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt16:
                        return _uint16 ?? (_uint16 = new LessThanOrEqualUInt16(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt32:
                        return _uint32 ?? (_uint32 = new LessThanOrEqualUInt32(ScriptingRuntimeHelpers.False));

                    case TypeCode.UInt64:
                        return _uint64 ?? (_uint64 = new LessThanOrEqualUInt64(ScriptingRuntimeHelpers.False));

                    case TypeCode.Single:
                        return _single ?? (_single = new LessThanOrEqualSingle(ScriptingRuntimeHelpers.False));

                    case TypeCode.Double:
                        return _double ?? (_double = new LessThanOrEqualDouble(ScriptingRuntimeHelpers.False));

                    default:
                        throw Error.ExpressionNotSupportedForType("LessThanOrEqual", type);
                }
            }
        }

        public override string ToString()
        {
            return "LessThanOrEqual()";
        }
    }
}

#endif