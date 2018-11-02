#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class SubInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

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
            get { return "Sub"; }
        }

        private SubInstruction()
        {
        }

        internal sealed class SubInt32 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)l - (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubInt16 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (short)unchecked((short)l - (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubInt64 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((long)l - (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubUInt16 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ushort)unchecked((ushort)l - (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubUInt32 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((uint)l - (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubUInt64 : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = unchecked((ulong)l - (ulong)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubSingle : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (float)l - (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubDouble : SubInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (double)l - (double)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new SubInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new SubInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new SubInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new SubUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new SubUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new SubUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new SubSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new SubDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Sub", type);
            }
        }

        public override string ToString()
        {
            return "Sub()";
        }
    }

    internal abstract class SubOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uInt16, _uInt32, _uInt64, _single, _double;

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
            get { return "SubOvf"; }
        }

        private SubOvfInstruction()
        {
        }

        internal sealed class SubOvfInt32 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(checked((int)l - (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfInt16 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (short)checked((short)l - (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfInt64 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = checked((long)l - (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfUInt16 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ushort)checked((ushort)l - (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfUInt32 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = checked((uint)l - (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfUInt64 : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (ulong)checked((short)l - (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfSingle : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (float)l - (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class SubOvfDouble : SubOvfInstruction
        {
            public override int Run(InterpretedFrame frame)
            {
                var l = frame.Data[frame.StackIndex - 2];
                var r = frame.Data[frame.StackIndex - 1];
                if (l == null || r == null)
                {
                    frame.Data[frame.StackIndex - 2] = null;
                }
                else
                {
                    frame.Data[frame.StackIndex - 2] = (double)l - (double)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        public static Instruction Create(Type type)
        {
            Debug.Assert(!type.IsEnum);
            switch (type.GetNonNullableType().GetTypeCode())
            {
                case TypeCode.Int16:
                    return _int16 ?? (_int16 = new SubOvfInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new SubOvfInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new SubOvfInt64());

                case TypeCode.UInt16:
                    return _uInt16 ?? (_uInt16 = new SubOvfUInt16());

                case TypeCode.UInt32:
                    return _uInt32 ?? (_uInt32 = new SubOvfUInt32());

                case TypeCode.UInt64:
                    return _uInt64 ?? (_uInt64 = new SubOvfUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new SubOvfSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new SubOvfDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("SubOvf", type);
            }
        }

        public override string ToString()
        {
            return "SubOvf()";
        }
    }
}

#endif