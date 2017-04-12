#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class NotEqualInstruction : Instruction
    {
        // Perf: EqualityComparer<T> but is 3/2 to 2 times slower.
        private static Instruction _reference, _boolean, _sbyte, _int16, _char, _int32, _int64, _byte, _uInt16, _uInt32, _uInt64, _single, _double;

        private static Instruction _referenceLiftedToNull, _booleanLiftedToNull, _sByteLiftedToNull, _int16LiftedToNull, _charLiftedToNull, _int32LiftedToNull, _int64LiftedToNull, _byteLiftedToNull, _uInt16LiftedToNull, _uInt32LiftedToNull, _uInt64LiftedToNull, _singleLiftedToNull, _doubleLiftedToNull;

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
            get { return "NotEqual"; }
        }

        private NotEqualInstruction()
        {
        }

        internal sealed class NotEqualBoolean : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((bool)left) != ((bool)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualSByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((sbyte)left) != ((sbyte)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((short)left) != ((short)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualChar : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((char)left) != ((char)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((int)left) != ((int)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((long)left) != ((long)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualByte : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((byte)left) != ((byte)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt16 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ushort)left) != ((ushort)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt32 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((uint)left) != ((uint)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt64 : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ulong)left) != ((ulong)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualSingle : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((float)left) != ((float)right))); // No, don't try to be clever about float comparison
                }
                return +1;
            }
        }

        internal sealed class NotEqualDouble : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var right = frame.Pop();
                var left = frame.Pop();
                if (left == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(right != null));
                }
                else if (right == null)
                {
                    frame.Push(ScriptingRuntimeHelpers.True);
                }
                else
                {
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((double)left) != ((double)right))); // No, don't try to be clever about double comparison
                }
                return +1;
            }
        }

        internal sealed class NotEqualReference : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var left = frame.Pop();
                var right = frame.Pop();
                frame.Push(ScriptingRuntimeHelpers.BooleanToObject(left != right));
                return +1;
            }
        }

        internal sealed class NotEqualBooleanLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((bool)left) != ((bool)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualSByteLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((sbyte)left) != ((sbyte)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt16LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((short)left) != ((short)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualCharLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((char)left) != ((char)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt32LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((int)left) != ((int)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualInt64LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((long)left) != ((long)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualByteLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((byte)left) != ((byte)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt16LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ushort)left) != ((ushort)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt32LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((uint)left) != ((uint)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualUInt64LiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((ulong)left) != ((ulong)right)));
                }
                return +1;
            }
        }

        internal sealed class NotEqualSingleLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((float)left) != ((float)right))); // No, don't try to be clever about float comparison
                }
                return +1;
            }
        }

        internal sealed class NotEqualDoubleLiftedToNull : NotEqualInstruction
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
                    frame.Push(ScriptingRuntimeHelpers.BooleanToObject(((double)left) != ((double)right))); // No, don't try to be clever about double comparison
                }
                return +1;
            }
        }

        internal sealed class NotEqualReferenceLiftedToNull : NotEqualInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                frame.Push(ScriptingRuntimeHelpers.BooleanToObject(frame.Pop() != frame.Pop()));
                return +1;
            }
        }

        public static Instruction Create(Type type, bool liftedToNull)
        {
            if (liftedToNull)
            {
                // Boxed enums can be unboxed as their underlying types:
                switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return _booleanLiftedToNull ?? (_booleanLiftedToNull = new NotEqualBooleanLiftedToNull());

                    case TypeCode.SByte:
                        return _sByteLiftedToNull ?? (_sByteLiftedToNull = new NotEqualSByteLiftedToNull());

                    case TypeCode.Byte:
                        return _byteLiftedToNull ?? (_byteLiftedToNull = new NotEqualByteLiftedToNull());

                    case TypeCode.Char:
                        return _charLiftedToNull ?? (_charLiftedToNull = new NotEqualCharLiftedToNull());

                    case TypeCode.Int16:
                        return _int16LiftedToNull ?? (_int16LiftedToNull = new NotEqualInt16LiftedToNull());

                    case TypeCode.Int32:
                        return _int32LiftedToNull ?? (_int32LiftedToNull = new NotEqualInt32LiftedToNull());

                    case TypeCode.Int64:
                        return _int64LiftedToNull ?? (_int64LiftedToNull = new NotEqualInt64LiftedToNull());

                    case TypeCode.UInt16:
                        return _uInt16LiftedToNull ?? (_uInt16LiftedToNull = new NotEqualUInt16LiftedToNull());

                    case TypeCode.UInt32:
                        return _uInt32LiftedToNull ?? (_uInt32LiftedToNull = new NotEqualUInt32LiftedToNull());

                    case TypeCode.UInt64:
                        return _uInt64LiftedToNull ?? (_uInt64LiftedToNull = new NotEqualUInt64LiftedToNull());

                    case TypeCode.Single:
                        return _singleLiftedToNull ?? (_singleLiftedToNull = new NotEqualSingleLiftedToNull());

                    case TypeCode.Double:
                        return _doubleLiftedToNull ?? (_doubleLiftedToNull = new NotEqualDoubleLiftedToNull());

                    case TypeCode.String:
                    case TypeCode.Object:
                        if (!type.IsValueType)
                        {
                            return _referenceLiftedToNull ?? (_referenceLiftedToNull = new NotEqualReferenceLiftedToNull());
                        }
                        // TODO: Nullable<T>
                        throw Error.ExpressionNotSupportedForNullableType("NotEqual", type);
                    default:
                        throw Error.ExpressionNotSupportedForType("NotEqual", type);
                }
            }
            else
            {
                // Boxed enums can be unboxed as their underlying types:
                switch ((type.IsEnum ? Enum.GetUnderlyingType(type) : type.GetNonNullableType()).GetTypeCode())
                {
                    case TypeCode.Boolean:
                        return _boolean ?? (_boolean = new NotEqualBoolean());

                    case TypeCode.SByte:
                        return _sbyte ?? (_sbyte = new NotEqualSByte());

                    case TypeCode.Byte:
                        return _byte ?? (_byte = new NotEqualByte());

                    case TypeCode.Char:
                        return _char ?? (_char = new NotEqualChar());

                    case TypeCode.Int16:
                        return _int16 ?? (_int16 = new NotEqualInt16());

                    case TypeCode.Int32:
                        return _int32 ?? (_int32 = new NotEqualInt32());

                    case TypeCode.Int64:
                        return _int64 ?? (_int64 = new NotEqualInt64());

                    case TypeCode.UInt16:
                        return _uInt16 ?? (_uInt16 = new NotEqualUInt16());

                    case TypeCode.UInt32:
                        return _uInt32 ?? (_uInt32 = new NotEqualUInt32());

                    case TypeCode.UInt64:
                        return _uInt64 ?? (_uInt64 = new NotEqualUInt64());

                    case TypeCode.Single:
                        return _single ?? (_single = new NotEqualSingle());

                    case TypeCode.Double:
                        return _double ?? (_double = new NotEqualDouble());

                    case TypeCode.String:
                    case TypeCode.Object:
                        if (!type.IsValueType)
                        {
                            return _reference ?? (_reference = new NotEqualReference());
                        }
                        // TODO: Nullable<T>
                        throw Error.ExpressionNotSupportedForNullableType("NotEqual", type);
                    default:
                        throw Error.ExpressionNotSupportedForType("NotEqual", type);
                }
            }
        }

        public override string ToString()
        {
            return "NotEqual()";
        }
    }
}

#endif