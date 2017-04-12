#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class EqualInstruction : Instruction
    {
        // Perf: EqualityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _reference, _boolean, _sbyte, _int16, _char, _int32, _int64, _byte, _uint16, _uint32, _uint64, _single, _double;

        private static Instruction _referenceLiftedToNull, _booleanLiftedToNull, _sbyteLiftedToNull, _int16LiftedToNull, _charLiftedToNull, _int32LiftedToNull, _int64LiftedToNull, _byteLiftedToNull, _uint16LiftedToNull, _uint32LiftedToNull, _uint64LiftedToNull, _singleLiftedToNull, _doubleLiftedToNull;

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
            get { return "Equal"; }
        }

        private EqualInstruction()
        {
        }

        internal sealed class EqualBoolean : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((bool)left) == ((bool)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualSByte : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((sbyte)left) == ((sbyte)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt16 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((short)left) == ((short)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualChar : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((char)left) == ((char)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt32 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((int)left) == ((int)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt64 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((long)left) == ((long)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualByte : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((byte)left) == ((byte)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt16 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ushort)left) == ((ushort)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt32 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((uint)left) == ((uint)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt64 : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ulong)left) == ((ulong)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualSingle : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((float)left) == ((float)right))); // No, don't try to be clever about float comparison
                }
                return +1;
            }
        }

        internal sealed class EqualDouble : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right == null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.False);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((double)left) == ((double)right))); // No, don't try to be clever about double comparison
                }
                return +1;
            }
        }

        internal sealed class EqualReference : EqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                frame.Push(ScriptingRuntimeHelpers.BooleanToObject(left == right));
                return +1;
            }
        }

        internal sealed class EqualBooleanLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((bool)left) == ((bool)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualSByteLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((sbyte)left) == ((sbyte)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt16LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((short)left) == ((short)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualCharLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((char)left) == ((char)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt32LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((int)left) == ((int)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualInt64LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((long)left) == ((long)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualByteLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((byte)left) == ((byte)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt16LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ushort)left) == ((ushort)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt32LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((uint)left) == ((uint)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualUInt64LiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ulong)left) == ((ulong)right)));
                }
                return +1;
            }
        }

        internal sealed class EqualSingleLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((float)left) == ((float)right))); // No, don't try to be clever about float comparison
                }
                return +1;
            }
        }

        internal sealed class EqualDoubleLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((double)left) == ((double)right))); // No, don't try to be clever about double comparison
                }
                return +1;
            }
        }

        internal sealed class EqualReferenceLiftedToNull : EqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(left == right));
                }
                return +1;
            }
        }

        public static Instruction Create(Type type, bool liftedToNull)
        {
            // Boxed enums can be unboxed as their underlying types:
            if (liftedToNull)
            {
                switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return _booleanLiftedToNull ?? (_booleanLiftedToNull = new EqualBooleanLiftedToNull());

                    case TypeCode.SByte:
                        return _sbyteLiftedToNull ?? (_sbyteLiftedToNull = new EqualSByteLiftedToNull());

                    case TypeCode.Byte:
                        return _byteLiftedToNull ?? (_byteLiftedToNull = new EqualByteLiftedToNull());

                    case TypeCode.Char:
                        return _charLiftedToNull ?? (_charLiftedToNull = new EqualCharLiftedToNull());

                    case TypeCode.Int16:
                        return _int16LiftedToNull ?? (_int16LiftedToNull = new EqualInt16LiftedToNull());

                    case TypeCode.Int32:
                        return _int32LiftedToNull ?? (_int32LiftedToNull = new EqualInt32LiftedToNull());

                    case TypeCode.Int64:
                        return _int64LiftedToNull ?? (_int64LiftedToNull = new EqualInt64LiftedToNull());

                    case TypeCode.UInt16:
                        return _uint16LiftedToNull ?? (_uint16LiftedToNull = new EqualUInt16LiftedToNull());

                    case TypeCode.UInt32:
                        return _uint32LiftedToNull ?? (_uint32LiftedToNull = new EqualUInt32LiftedToNull());

                    case TypeCode.UInt64:
                        return _uint64LiftedToNull ?? (_uint64LiftedToNull = new EqualUInt64LiftedToNull());

                    case TypeCode.Single:
                        return _singleLiftedToNull ?? (_singleLiftedToNull = new EqualSingleLiftedToNull());

                    case TypeCode.Double:
                        return _doubleLiftedToNull ?? (_doubleLiftedToNull = new EqualDoubleLiftedToNull());

                    case TypeCode.String:
                    case TypeCode.Object:
                        if (!type.IsValueType)
                        {
                            return _referenceLiftedToNull ?? (_referenceLiftedToNull = new EqualReferenceLiftedToNull());
                        }
                        // TODO: Nullable<T>
                        throw Error.ExpressionNotSupportedForNullableType("Equal", type);

                    default:
                        throw Error.ExpressionNotSupportedForType("Equal", type);
                }
            }
            else
            {
                switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return _boolean ?? (_boolean = new EqualBoolean());

                    case TypeCode.SByte:
                        return _sbyte ?? (_sbyte = new EqualSByte());

                    case TypeCode.Byte:
                        return _byte ?? (_byte = new EqualByte());

                    case TypeCode.Char:
                        return _char ?? (_char = new EqualChar());

                    case TypeCode.Int16:
                        return _int16 ?? (_int16 = new EqualInt16());

                    case TypeCode.Int32:
                        return _int32 ?? (_int32 = new EqualInt32());

                    case TypeCode.Int64:
                        return _int64 ?? (_int64 = new EqualInt64());

                    case TypeCode.UInt16:
                        return _uint16 ?? (_uint16 = new EqualUInt16());

                    case TypeCode.UInt32:
                        return _uint32 ?? (_uint32 = new EqualUInt32());

                    case TypeCode.UInt64:
                        return _uint64 ?? (_uint64 = new EqualUInt64());

                    case TypeCode.Single:
                        return _single ?? (_single = new EqualSingle());

                    case TypeCode.Double:
                        return _double ?? (_double = new EqualDouble());

                    case TypeCode.String:
                    case TypeCode.Object:
                        if (!type.IsValueType)
                        {
                            return _reference ?? (_reference = new EqualReference());
                        }
                        // TODO: Nullable<T>
                        throw Error.ExpressionNotSupportedForNullableType("Equal", type);

                    default:
                        throw Error.ExpressionNotSupportedForType("Equal", type);
                }
            }
        }

        public override string ToString()
        {
            return "Equal()";
        }
    }
}

#endif