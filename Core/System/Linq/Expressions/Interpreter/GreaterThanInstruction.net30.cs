#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class GreaterThanInstruction : Instruction
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
            get { return "GreaterThan"; }
        }

        private GreaterThanInstruction(object nullValue)
        {
            _nullValue = nullValue;
        }

        internal sealed class GreaterThanSByte : GreaterThanInstruction
        {
            public GreaterThanSByte(object nullValue)
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
                    frame.Push(((sbyte)left) > (sbyte)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanInt16 : GreaterThanInstruction
        {
            public GreaterThanInt16(object nullValue)
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
                    frame.Push(((short)left) > (short)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanChar : GreaterThanInstruction
        {
            public GreaterThanChar(object nullValue)
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
                    frame.Push(((char)left) > (char)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanInt32 : GreaterThanInstruction
        {
            public GreaterThanInt32(object nullValue)
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
                    frame.Push(((int)left) > (int)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanInt64 : GreaterThanInstruction
        {
            public GreaterThanInt64(object nullValue)
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
                    frame.Push(((long)left) > (long)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanByte : GreaterThanInstruction
        {
            public GreaterThanByte(object nullValue)
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
                    frame.Push(((byte)left) > (byte)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanUInt16 : GreaterThanInstruction
        {
            public GreaterThanUInt16(object nullValue)
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
                    frame.Push(((ushort)left) > (ushort)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanUInt32 : GreaterThanInstruction
        {
            public GreaterThanUInt32(object nullValue)
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
                    frame.Push(((uint)left) > (uint)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanUInt64 : GreaterThanInstruction
        {
            public GreaterThanUInt64(object nullValue)
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
                    frame.Push(((ulong)left) > (ulong)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanSingle : GreaterThanInstruction
        {
            public GreaterThanSingle(object nullValue)
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
                    frame.Push(((float)left) > (float)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanDouble : GreaterThanInstruction
        {
            public GreaterThanDouble(object nullValue)
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
                    frame.Push(((double)left) > (double)right);
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
                        return _liftedToNullSByte ?? (_liftedToNullSByte = new GreaterThanSByte(null));

                    case TypeCode.Byte:
                        return _liftedToNullByte ?? (_liftedToNullByte = new GreaterThanByte(null));

                    case TypeCode.Char:
                        return _liftedToNullChar ?? (_liftedToNullChar = new GreaterThanChar(null));

                    case TypeCode.Int16:
                        return _liftedToNullInt16 ?? (_liftedToNullInt16 = new GreaterThanInt16(null));

                    case TypeCode.Int32:
                        return _liftedToNullInt32 ?? (_liftedToNullInt32 = new GreaterThanInt32(null));

                    case TypeCode.Int64:
                        return _liftedToNullInt64 ?? (_liftedToNullInt64 = new GreaterThanInt64(null));

                    case TypeCode.UInt16:
                        return _liftedToNullUInt16 ?? (_liftedToNullUInt16 = new GreaterThanUInt16(null));

                    case TypeCode.UInt32:
                        return _liftedToNullUInt32 ?? (_liftedToNullUInt32 = new GreaterThanUInt32(null));

                    case TypeCode.UInt64:
                        return _liftedToNullUInt64 ?? (_liftedToNullUInt64 = new GreaterThanUInt64(null));

                    case TypeCode.Single:
                        return _liftedToNullSingle ?? (_liftedToNullSingle = new GreaterThanSingle(null));

                    case TypeCode.Double:
                        return _liftedToNullDouble ?? (_liftedToNullDouble = new GreaterThanDouble(null));

                    default:
                        throw Error.ExpressionNotSupportedForType("GreaterThan", type);
                }
            }
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sbyte ?? (_sbyte = new GreaterThanSByte(ScriptingRuntimeHelpers.False));

                case TypeCode.Byte:
                    return _byte ?? (_byte = new GreaterThanByte(ScriptingRuntimeHelpers.False));

                case TypeCode.Char:
                    return _char ?? (_char = new GreaterThanChar(ScriptingRuntimeHelpers.False));

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new GreaterThanInt16(ScriptingRuntimeHelpers.False));

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new GreaterThanInt32(ScriptingRuntimeHelpers.False));

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new GreaterThanInt64(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new GreaterThanUInt16(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new GreaterThanUInt32(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new GreaterThanUInt64(ScriptingRuntimeHelpers.False));

                case TypeCode.Single:
                    return _single ?? (_single = new GreaterThanSingle(ScriptingRuntimeHelpers.False));

                case TypeCode.Double:
                    return _double ?? (_double = new GreaterThanDouble(ScriptingRuntimeHelpers.False));

                default:
                    throw Error.ExpressionNotSupportedForType("GreaterThan", type);
            }
        }

        public override string ToString()
        {
            return "GreaterThan()";
        }
    }

    internal abstract class GreaterThanOrEqualInstruction : Instruction
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
            get { return "GreaterThanOrEqual"; }
        }

        private GreaterThanOrEqualInstruction(object nullValue)
        {
            _nullValue = nullValue;
        }

        internal sealed class GreaterThanOrEqualSByte : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualSByte(object nullValue)
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
                    frame.Push(((sbyte)left) >= (sbyte)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualInt16 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualInt16(object nullValue)
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
                    frame.Push(((short)left) >= (short)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualChar : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualChar(object nullValue)
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
                    frame.Push(((char)left) >= (char)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualInt32 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualInt32(object nullValue)
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
                    frame.Push(((int)left) >= (int)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualInt64 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualInt64(object nullValue)
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
                    frame.Push(((long)left) >= (long)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualByte : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualByte(object nullValue)
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
                    frame.Push(((byte)left) >= (byte)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualUInt16 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualUInt16(object nullValue)
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
                    frame.Push(((ushort)left) >= (ushort)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualUInt32 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualUInt32(object nullValue)
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
                    frame.Push(((uint)left) >= (uint)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualUInt64 : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualUInt64(object nullValue)
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
                    frame.Push(((ulong)left) >= (ulong)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualSingle : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualSingle(object nullValue)
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
                    frame.Push(((float)left) >= (float)right);
                }
                return +1;
            }
        }

        internal sealed class GreaterThanOrEqualDouble : GreaterThanOrEqualInstruction
        {
            public GreaterThanOrEqualDouble(object nullValue)
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
                    frame.Push(((double)left) >= (double)right);
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
                        return _liftedToNullSByte ?? (_liftedToNullSByte = new GreaterThanOrEqualSByte(null));

                    case TypeCode.Byte:
                        return _liftedToNullByte ?? (_liftedToNullByte = new GreaterThanOrEqualByte(null));

                    case TypeCode.Char:
                        return _liftedToNullChar ?? (_liftedToNullChar = new GreaterThanOrEqualChar(null));

                    case TypeCode.Int16:
                        return _liftedToNullInt16 ?? (_liftedToNullInt16 = new GreaterThanOrEqualInt16(null));

                    case TypeCode.Int32:
                        return _liftedToNullInt32 ?? (_liftedToNullInt32 = new GreaterThanOrEqualInt32(null));

                    case TypeCode.Int64:
                        return _liftedToNullInt64 ?? (_liftedToNullInt64 = new GreaterThanOrEqualInt64(null));

                    case TypeCode.UInt16:
                        return _liftedToNullUInt16 ?? (_liftedToNullUInt16 = new GreaterThanOrEqualUInt16(null));

                    case TypeCode.UInt32:
                        return _liftedToNullUInt32 ?? (_liftedToNullUInt32 = new GreaterThanOrEqualUInt32(null));

                    case TypeCode.UInt64:
                        return _liftedToNullUInt64 ?? (_liftedToNullUInt64 = new GreaterThanOrEqualUInt64(null));

                    case TypeCode.Single:
                        return _liftedToNullSingle ?? (_liftedToNullSingle = new GreaterThanOrEqualSingle(null));

                    case TypeCode.Double:
                        return _liftedToNullDouble ?? (_liftedToNullDouble = new GreaterThanOrEqualDouble(null));

                    default:
                        throw Error.ExpressionNotSupportedForType("GreaterThanOrEqual", type);
                }
            }
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.SByte:
                    return _sbyte ?? (_sbyte = new GreaterThanOrEqualSByte(ScriptingRuntimeHelpers.False));

                case TypeCode.Byte:
                    return _byte ?? (_byte = new GreaterThanOrEqualByte(ScriptingRuntimeHelpers.False));

                case TypeCode.Char:
                    return _char ?? (_char = new GreaterThanOrEqualChar(ScriptingRuntimeHelpers.False));

                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new GreaterThanOrEqualInt16(ScriptingRuntimeHelpers.False));

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new GreaterThanOrEqualInt32(ScriptingRuntimeHelpers.False));

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new GreaterThanOrEqualInt64(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new GreaterThanOrEqualUInt16(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new GreaterThanOrEqualUInt32(ScriptingRuntimeHelpers.False));

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new GreaterThanOrEqualUInt64(ScriptingRuntimeHelpers.False));

                case TypeCode.Single:
                    return _single ?? (_single = new GreaterThanOrEqualSingle(ScriptingRuntimeHelpers.False));

                case TypeCode.Double:
                    return _double ?? (_double = new GreaterThanOrEqualDouble(ScriptingRuntimeHelpers.False));

                default:
                    throw Error.ExpressionNotSupportedForType("GreaterThanOrEqual", type);
            }
        }

        public override string ToString()
        {
            return "GreaterThanOrEqual()";
        }
    }
}

#endif