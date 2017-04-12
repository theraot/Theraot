#if NET20 || NET30

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using Theraot.Core;

namespace System.Linq.Expressions.Interpreter
{
    internal abstract class MulInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uint16, _uint32, _uint64, _single, _double;

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
            get { return "Mul"; }
        }

        private MulInstruction()
        {
        }

        internal sealed class MulInt32 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(unchecked((int)l * (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulInt16 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = (short)unchecked((short)l * (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulInt64 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = unchecked((long)l * (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulUInt16 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = (ushort)unchecked((ushort)l * (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulUInt32 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = unchecked((uint)l * (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulUInt64 : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = unchecked((ulong)l * (ulong)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulSingle : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = (float)l * (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulDouble : MulInstruction
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
                    frame.Data[frame.StackIndex - 2] = (double)l * (double)r;
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
                    return _int16 ?? (_int16 = new MulInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new MulInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new MulInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new MulUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new MulUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new MulUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new MulSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new MulDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("Mul", type);
            }
        }

        public override string ToString()
        {
            return "Mul()";
        }
    }

    internal abstract class MulOvfInstruction : Instruction
    {
        private static Instruction _int16, _int32, _int64, _uint16, _uint32, _uint64, _single, _double;

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
            get { return "MulOvf"; }
        }

        private MulOvfInstruction()
        {
        }

        internal sealed class MulOvfInt32 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = ScriptingRuntimeHelpers.Int32ToObject(checked((int)l * (int)r));
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfInt16 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = (short)checked((short)l * (short)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfInt64 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = checked((long)l * (long)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfUInt16 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = (ushort)checked((ushort)l * (ushort)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfUInt32 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = checked((uint)l * (uint)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfUInt64 : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = checked((ulong)l * (ulong)r);
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfSingle : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = (float)l * (float)r;
                }
                frame.StackIndex--;
                return +1;
            }
        }

        internal sealed class MulOvfDouble : MulOvfInstruction
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
                    frame.Data[frame.StackIndex - 2] = (double)l * (double)r;
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
                    return _int16 ?? (_int16 = new MulOvfInt16());

                case TypeCode.Int32:
                    return _int32 ?? (_int32 = new MulOvfInt32());

                case TypeCode.Int64:
                    return _int64 ?? (_int64 = new MulOvfInt64());

                case TypeCode.UInt16:
                    return _uint16 ?? (_uint16 = new MulOvfUInt16());

                case TypeCode.UInt32:
                    return _uint32 ?? (_uint32 = new MulOvfUInt32());

                case TypeCode.UInt64:
                    return _uint64 ?? (_uint64 = new MulOvfUInt64());

                case TypeCode.Single:
                    return _single ?? (_single = new MulOvfSingle());

                case TypeCode.Double:
                    return _double ?? (_double = new MulOvfDouble());

                default:
                    throw Error.ExpressionNotSupportedForType("MulOvf", type);
            }
        }

        public override string ToString()
        {
            return "MulOvf()";
        }
    }
}

#endif